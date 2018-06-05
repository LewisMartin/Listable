using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.CollectionMicroservice.DTO
{
    public class Collection
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }
        public List<CollectionItem> CollectionItems { get; set; }
    }

    public class CollectionItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageLocation { get; set; }
    }

    /* Custom fields will be implemented later */
    public class CustomField
    {
        public string DisplayName { get; set; }
    }

    public class StringField : CustomField
    {
        public string Content { get; set; }
    }

    public class DateField : CustomField
    {
        public DateTime Date { get; set; }
    }

    public class ImageField : CustomField
    {
        public Uri ImageLocation { get; set; }
        public string caption { get; set; }
    }

    public class GalleryField : CustomField
    {
        public ICollection<ImageField> Images { get; set; }
    }
}
