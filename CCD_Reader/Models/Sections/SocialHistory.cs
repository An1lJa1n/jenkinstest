using System.Collections.Generic;

namespace Exscribe.Models
{
    public class SocialHistory:Section
    {
        public SocialHistory(string title):base(title)
        {            
        }
        public List<SocialHistoryList> socialHistoryList { get; set; }
    }
    public class SocialHistoryList
    {
        public string SocialHistoryObservation { get; set; }
        public string Description { get; set; }
        public string DatesObserved { get; set; }
        
    }
}