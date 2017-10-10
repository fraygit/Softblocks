using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqData
    {
        public string AppId { get; set; }
        public string DocumentTypeId { get; set; }
        public string DataId { get; set; }
        public string DataParentId { get; set; }
        public string SubDocumentTypeId { get; set; }
    }
}