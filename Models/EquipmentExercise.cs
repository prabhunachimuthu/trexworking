using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class EquipmentExercise
    {
        public int Id { get; set; }
        public string Limb { get; set; }
        public string ExerciseEnum { get; set; }
        public string FlexionString { get; set; }
        public string FlexionLaymanString { get; set; }
        public string ExtensionString { get; set; }
        public string ExtensionLaymanString { get; set; }
    }
}
