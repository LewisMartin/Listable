using System.Collections.Generic;

namespace GatewayAPI.Models.Collection
{
    public class CollectionView
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool GridDisplay { get; set; }

        public List<CollectionViewItem> CollectionViewItems { get; set; }
    }

    public class CollectionViewItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailUri { get; set; }
    }
}
