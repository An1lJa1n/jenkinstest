namespace Exscribe.Models
{
    public class Assessments : Section
    {
        public Assessments(string title) :base(title)
        {            
        }
        public string Value { get; set; }
    }
}