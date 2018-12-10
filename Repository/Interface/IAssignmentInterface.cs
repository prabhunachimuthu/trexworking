using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IAssignmentInterface:IDisposable
    {
        EquipmentAssignment getEquipmentAssignment(string lAssignmentId);
        EquipmentAssignment getEquipmentAssignment(string lPatientID, string lTherspistID);
        string InsertEquipmentAssignment(EquipmentAssignment pEquipmentAssignment);
        string UpdateEquipmentAssignment(EquipmentAssignment pEquipmentAssignment);
        List<EquipmentAssignment> getEquipmentAssignment();
    }
}
