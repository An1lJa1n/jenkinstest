using System.Collections.Generic;

namespace Exscribe.Models
{
    public class FamilyHistory : Section
    {
        public FamilyHistory(string title): base (title)
        {
        }
        public List<FamilyHistoryList> familyHistoryList { get; set; }
    }
    public class FamilyHistoryList
    {
        public string Condition { get; set; }
        public string FamilyRelationship { get; set; }
        public string AgeOfOnset { get; set; }
    }
}