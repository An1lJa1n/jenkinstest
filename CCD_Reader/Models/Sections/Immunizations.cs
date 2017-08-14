using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Immunizations : Section
    {
        public Immunizations(string title) : base(title)
        {
        }
        public List<ImmunizationsList> immunizationsList { get; set; }
    }
    public class ImmunizationsList
    {
        public string Vaccine { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }
}