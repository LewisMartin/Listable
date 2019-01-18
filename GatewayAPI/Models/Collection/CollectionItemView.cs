using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayAPI.Models.Collection
{
    public class CollectionItemView
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool ShowImage { get; set; }

        public string ImageUrl { get; set; }

        public bool DisplayOwnerOptions { get; set; }
    }
}
