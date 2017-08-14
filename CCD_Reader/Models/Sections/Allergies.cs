using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Allergies : Section
    {
        public Allergies(string title) :base (title)
        {
           
        }
        public List<AllergiesList> allergiesList { get; set; }
    }
    public class AllergiesList
    {
        public string Substance { get; set; }
        public string Reaction { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
    }
}