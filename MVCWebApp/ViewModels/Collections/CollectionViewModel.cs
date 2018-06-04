using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CollectionViewModel
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        public List<Tuple<string, string>> CollectionItems { get; set; }
    }
}
