using System.Collections.Generic;

namespace GatewayAPI.Models.Collection
{
    public class CollectionsList
    {
        public List<CollectionsListItem> Collections { get; set; }
    }

    public class CollectionsListItem
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}
