using OneDirect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Response
{
    public class AssignmentResponse
    {
        public EquipmentAssignment EquipmentAssignment { get; set; }

        public int ProtocolCount { get; set; }
    }
}
