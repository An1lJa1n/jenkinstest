using System.Collections.Generic;

namespace Exscribe.Models
{
    public class TreatmentPlan : Section
    {
        public TreatmentPlan(string title):base(title)
        {
            handOffCommunication = new List<HandOffCommunication>();
        }

        public List<HandOffCommunication> handOffCommunication { get; set; }
    }
    public class HandOffCommunication
    {
        public string PlannedCare { get; set; }
        public string StartDate { get; set; }
    }
}