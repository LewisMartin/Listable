using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class OverviewViewModel
    {
        public List<Tuple<string, string>> collections { get; set; }
    }
}
