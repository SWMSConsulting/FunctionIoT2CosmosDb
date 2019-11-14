using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionIoT2CosmoDB
{
    class DataPoint
    {
            public ObjectId Id { get; set; }

            public DateTime CreatedAtUtc { get; set; }

            public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>();

            public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        }
    
}
