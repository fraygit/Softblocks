using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class PagePanels
    {
        public ObjectId Id { get; set; }
        public string PanelType { get; set; }
        public int Order { get; set; }
        public int ColWidth { get; set; }
    }
}
