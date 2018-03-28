using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trello.Models
{
    public class Extract
    {
        public Extract()
        {
            Records = new List<Record>();
        }

        public List<Record> Records { get; set; }

        public class Record
        {
            public string Card { get; set; }
            public string Board { get; set; }
            public string List { get; set; }
        }
    }
}
