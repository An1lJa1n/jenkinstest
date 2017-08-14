using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Encounters : Section
    {
        public Encounters(string title):base(title)
        {
        }
        public List<EncountersList> encountersList { get; set; }
    }
    public class EncountersList
    {
        public string Encounter { get; set; }
        public string EncounterDiagnosis { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
    }
}