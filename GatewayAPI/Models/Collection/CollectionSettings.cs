using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayAPI.Models.Collection
{
    public class CollectionSettings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool ImageEnabled { get; set; }

        public bool GridDisplay { get; set; }
    }
}
