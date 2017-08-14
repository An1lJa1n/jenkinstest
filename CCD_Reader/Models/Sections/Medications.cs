using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Medications : Section
    {
        public Medications(string title) : base(title)
        {
        }
        public List<MedicationList> medicationList { get; set; }
    }
    public class MedicationList
    {
        public string Medication { get; set; }
        public string Directions { get; set; }
        public string StartDate { get; set; }
        public string Status { get; set; }
        public string Indications { get; set; }
        public string FillInstructions { get; set; }
    }
}