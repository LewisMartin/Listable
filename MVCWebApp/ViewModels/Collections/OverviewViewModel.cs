using Listable.CollectionMicroservice.DTO;
using System.Collections.Generic;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class OverviewViewModel
    {
        public List<CollectionOverview> collections { get; set; }
    }

    public class CollectionOverview
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }
    }
}
