using System.Collections.Generic;

namespace Exscribe.Models
{
    public class MentalStatus : Section
    {
        public MentalStatus(string title) : base(title)
        {            
        }
        public List<MentalStatusList> mentalStatusList { get; set; }
    }
    public class MentalStatusList
    {
        public string Status { get; set; }
        public string Date { get; set; }
    }
}