using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqCreateDataView
    {
        public string AppModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataViewType { get; set; }
        public string DocumentTypeId { get; set; }
    }
}