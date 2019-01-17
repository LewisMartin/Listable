using System.Collections.Generic;

namespace GatewayAPI.Models.Collection
{
    public class CollectionQueryResults
    {
        public int Count { get; set; }

        public List<CollectionQueryResult> QueryResults { get; set; }
    }

    public class CollectionQueryResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int CollectionSize { get; set; }

        public bool ImageEnabled { get; set; }
    }
}
