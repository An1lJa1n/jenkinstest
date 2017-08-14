namespace Exscribe.Models
{
    public class ReasonForReferral : Section
    {
        public ReasonForReferral(string title) : base(title)
        {

        }
        public string Value { get; set; } 
    }
}