using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class EquipmentAPi
    {
        public string AssignmentId { get; set; }
        public string InstallerId { get; set; }
        public int PatientId { get; set; }
        public DateTime? DateInstalled { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Limb { get; set; }
        public string Side { get; set; }
        public int? ExcerciseEnum { get; set; }
        public string ChairId { get; set; }
        public string Boom1Id { get; set; }
        public string Boom2Id { get; set; }
        public string Boom3Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual User Installer { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
