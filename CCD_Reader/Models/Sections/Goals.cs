using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Goals : Section
    {
        public Goals(string title):base(title)
        {
           
        }
        public List<GoalsList> goalsList { get; set; }
    }
    public class GoalsList
    {
        public string Goal { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
    }
}