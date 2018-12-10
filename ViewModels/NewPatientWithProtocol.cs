using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class NewPatientWithProtocol
    {
        public NewPatient NewPatient { get; set; }
        public NewPatientRx NewPatientRX { get; set; }
        public ProtocolView ProtocolView { get; set; }
        public string ProviderId { get; set; }
        public int PainThreshold { get; set; }
        public int RateOfChange { get; set; }
        public List<NewPatientRx> NewPatientRXList { get; set; }
        public string returnView { get; set; }
    }
}
