using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Trello.Models;

namespace Trello
{
    class Program
    {
        static void Main(string[] args)
        {
            string _appKey = ConfigurationManager.AppSettings["AppKey"];
            Extract extract = new Extract();
            var token = SeleniumHelper.AuthorizeTrello();
            Console.WriteLine("Received token - "+token);
            Console.WriteLine("Calling boards API");
            var boards = ApiHelper.Get<List<Board>>($"1/members/me/boards?key={_appKey}&token={token}");
            Console.WriteLine($"Received {boards.Count} boards from API");
            foreach (var board in boards)
            {
                //Collect data for individual boards for raw data
                var members = ApiHelper.Get<List<Member>>($"1/boards/{board.id}/members?key={_appKey}&token={token}");
                Console.WriteLine($"Received {members.Count} members from API for board {board.name}");
                var labels = ApiHelper.Get<List<Label>>($"1/boards/{board.id}/labels?key={_appKey}&token={token}");
                Console.WriteLine($"Received {labels.Count} labels from API for board {board.name}");
                var lists = ApiHelper.Get<List<List>>($"1/boards/{board.id}/lists?key={_appKey}&token={token}");
                Console.WriteLine($"Received {lists.Count} lists from API for board {board.name}");
                var cards = ApiHelper.Get<List<Card>>($"1/boards/{board.id}/cards?key={_appKey}&token={token}");
                Console.WriteLine($"Received {cards.Count} cards from API for board {board.name}");

                //process the data to form extract
                cards.ForEach(card =>
                {
                    extract.Records.Add(new Extract.Record
                    {
                        Board = board.name,
                        Card = card.name,
                        List = lists.FirstOrDefault(list => list.id == card.idList)?.name
                    });
                });
            }

            string path = GenerateExcel(extract);
            Console.WriteLine($"Excel generated to {path}");

            SendMail(path);
            Console.WriteLine("Mail Sent");
        }

        static string GenerateExcel(Extract extract)
        {
            string path = $"C:\\temp\\TrelloExtract_{DateTime.Now:yyyy-dd-M--HH-mm-ss}.xlsx";
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Consolidated Sheet");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            //  
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "Board";
            workSheet.Cells[1, 2].Value = "Card";
            workSheet.Cells[1, 3].Value = "List";
            //Body of table  
            //  
            int recordIndex = 2;
            foreach (var record in extract.Records)
            {
                workSheet.Cells[recordIndex, 1].Value = record.Board;
                workSheet.Cells[recordIndex, 2].Value = record.Card;
                workSheet.Cells[recordIndex, 3].Value = record.List;
                recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            Stream stream = File.Create(path);
            excel.SaveAs(stream);
            stream.Close();
            return path;
        }

        static void SendMail(string attachmentFilename)
        {
            string _mailid = ConfigurationManager.AppSettings["mailId"];
            string _mailPassword = ConfigurationManager.AppSettings["mailPassword"];
            string _recipients = ConfigurationManager.AppSettings["recipients"];
            MailMessage newmsg = new MailMessage();
            newmsg.From = new MailAddress(_mailid);
            foreach (var recipient in _recipients.Split(';'))
            {
                newmsg.To.Add(new MailAddress(recipient));
            }

            newmsg.Subject = $"Trello Extract - {DateTime.Now.ToShortDateString()}";
            newmsg.Body = "PFA the extract";

            //For File Attachment, more file can also be attached
            Attachment att = new Attachment(attachmentFilename);
            newmsg.Attachments.Add(att);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailid, _mailPassword),
                EnableSsl = true
            };
            smtp.Send(newmsg);
        }
    }
}
