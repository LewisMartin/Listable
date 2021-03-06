﻿using System.ComponentModel.DataAnnotations;

namespace Listable.MVCWebApp.ViewModels.Collections
{
    public class CreateCollectionViewModel
    {
        public CreateCollectionViewModel()
        {
            CollectionDetails = new CollectionEditor();
        }

        [Required]
        public CollectionEditor CollectionDetails { get; set; }
    }
}
