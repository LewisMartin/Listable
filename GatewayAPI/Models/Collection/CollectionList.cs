using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayAPI.Models.Collection
{
    public class CollectionList
    {
        public List<CollectionListItem> Collections { get; set; }
    }

    public class CollectionListItem
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}
