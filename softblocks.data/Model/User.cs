using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class User : MongoEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhoto { get; set; }
        public string ProfilePhotoFilename { get; set; }
        public int Status { get; set; } // 0 not registered // 1 registered // 2 pending // 3 Invited
        public string VerificationCode { get; set; }
        public string CurrentOrganisation { get; set; }
        public List<ObjectId> Organisations { get; set; }
    }
}
