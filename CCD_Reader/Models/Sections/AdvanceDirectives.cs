using System.Collections.Generic;

namespace Exscribe.Models
{
    public class AdvanceDirectives : Section
    {
        public AdvanceDirectives(string title) : base(title) { }
        public List<AdvanceDirectivesList> advanceDirectivesList { get; set; }
    }
    public class AdvanceDirectivesList
    {
        public string Directive { get; set; }
        public string Description { get; set; }
        public string Verification { get; set; }
        public string SupportingDocument { get; set; }
    }
}