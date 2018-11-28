using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayAPI.Models.Collection.Forms
{
    public class DeleteCollectionItemFormModel
    {
        public string CollectionId { get; set; }

        public string CollectionItemId { get; set; }
    }
}
