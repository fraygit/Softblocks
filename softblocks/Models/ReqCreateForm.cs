using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ReqCreateForm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentTypeId { get; set; }
        public string AppModuleId { get; set; }
    }
}