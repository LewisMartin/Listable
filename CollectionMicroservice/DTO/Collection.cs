using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Listable.CollectionMicroservice.DTO
{
    public enum CollectionDisplayFormat
    {
        List,
        Grid
    }

    public class Collection
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public int Owner { get; set; }

        public string Name { get; set; }

        public bool PrivateMode { get; set; }

        public bool ImageEnabled { get; set; }

        public CollectionDisplayFormat DisplayFormat { get; set; }

        public List<CollectionItem> CollectionItems { get; set; }
    }

    public class CollectionItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageId { get; set; }
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
