using System.Collections.Generic;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CollectionGridViewModel
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        public List<CollectionGridItem> CollectionItems { get; set; }
    }

    public class CollectionGridItem
    {
        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public string ItemThumbUri { get; set; }
    }
}
