﻿using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Models.Collection.Forms
{
    public class CreateCollectionFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool PrivateMode { get; set; }

        [Required]
        public bool ImageEnabled { get; set; }

        [Required]
        public bool GridDisplay { get; set; }
    }
}
