﻿using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class Discussion : MongoEntity
    {
        public ObjectId OrganisationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Topic> Topics { get; set; }
        public ObjectId CreatedBy { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
