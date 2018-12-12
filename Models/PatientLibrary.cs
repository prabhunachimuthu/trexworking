using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class PatientLibrary
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Patient { get; set; }
        public string Limb { get; set; }
        public string Side { get; set; }
        public string Exercise { get; set; }
        public string Url { get; set; }
        public int Status { get; set; }
        public int? LibraryId { get; set; }
        public string Name { get; set; }
    }
}
