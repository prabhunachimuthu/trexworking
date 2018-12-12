using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneDirect.ViewModels
{
    public partial class LibraryView
    {
        public int Id { get; set; }
        [Required]
        public string Limb { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Side { get; set; }
        [Required]
        public string Exercise { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
