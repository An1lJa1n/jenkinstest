using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Results : Section
    {
        public Results(string title) : base(title)
        {
        }
        public List<ResultsList> resultsList { get; set; }
    }
    public class ResultsList
    {
        public string ResultType { get; set; }
        public string ResultValue { get; set; }
        public string RelevantReferenceRange { get; set; }
        public string Interpretation { get; set; }
        public string Date { get; set; }
    }
}