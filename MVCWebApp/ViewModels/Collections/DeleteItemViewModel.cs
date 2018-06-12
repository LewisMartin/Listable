﻿using System.Collections.Generic;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class DeleteItemViewModel
    {
        public string CollectionId { get; set; }

        public string CollectionName { get; set; }

        public IList<DeleteItemOption> DeleteItemOptions { get; set; }
    }

    public class DeleteItemOption
    {
        public bool IsOptionSelected { get; set; } = false;

        public string ItemId { get; set; }

        public string ItemName { get; set; } 
    }
}
