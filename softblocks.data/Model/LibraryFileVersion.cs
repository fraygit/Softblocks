using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class LibraryFileVersion
    {
        public string Description { get; set; }
        public int Version { get; set; }
        public string Path { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
