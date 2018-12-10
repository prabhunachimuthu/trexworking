using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class Slots
    {
        public string SlotDate { get; set; }

        public List<CheckModel> SlotTimes { get; set; }
        public string AppointmentId { get; set; }
    }

    public class SlotList
    {
        public List<Slots> slotDetails { get; set; }
    }

    public class ConfirmAppointment
    {
        public string AppointmentId { get; set; }
        public string SlotDate { get; set; }
        public string SlotId { get; set; }
    }
}
