using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class EmailNotification : MongoEntity
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public List<string> Bcc { get; set; }
        public List<string> CC { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsHtml { get; set; }
        public int Status { get; set; }
        public DateTime DateSent { get; set; }
    }
}
