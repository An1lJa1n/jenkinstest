using System.Collections.Generic;

namespace Exscribe.Models
{
    public class FunctionalStatus : Section
    {
        public FunctionalStatus(string title):base(title)
        {

        }
        public List<FunctionalStatusList> functionalStatusList { get; set; }
    }
    public class FunctionalStatusList
    {
        public string FunctionalCategory { get; set; }
        public string EffectiveDates { get; set; }
    }
}