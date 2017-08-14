using System.Collections.Generic;

namespace Exscribe.Models
{
    public class VitalSigns: Section
    {
        public VitalSigns(string title) : base(title)
        {  
        }
        public string DateTime { get; set; }
        public List<VitalsSignsKV> vitalsSignsKV { get; set; }
    }
    public class VitalsSignsKV
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}