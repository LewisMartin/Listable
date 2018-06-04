using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class DeleteCollectionViewModel
    {
        public string SelectedCollection { get; set; }

        public List<SelectListItem> Collections { get; set; }
    }
}
