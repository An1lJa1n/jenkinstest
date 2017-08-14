using Exscribe.Common;

namespace Exscribe.Models
{
    public class Reponse
    {
        public string Message { get; set; }
        public string ResponseStatus { get; set; }
    }

    public class Section
    {
        public Reponse Message { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public Section(string title)
        {
            Title = title;
            DisplayName = CommonServices.UppercaseWords(title.ToLower());
        }
    }
}
