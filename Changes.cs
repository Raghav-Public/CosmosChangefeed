using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosChangefeed
{
    public class Changes
    {
        public Dictionary<String, String> Checkpoints { get; set; }
        public List<Document> UpdatedDocuments { get; set; } 
    }
}
