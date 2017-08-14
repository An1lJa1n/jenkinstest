using System.Collections.Generic;

namespace Exscribe.Models
{
    public class HealthConcern : Section
    {
        public HealthConcern(string title) : base(title)
        {
         
        }
        
        public List<Observations> hcb { get; set; }
        public List<Concerns> hc { get; set; }        
    }
    public class Observations
    {
        public string Observation { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }
    public class Concerns
    {
        public string Concern { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }
}