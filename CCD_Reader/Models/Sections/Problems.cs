using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Problems : Section
    {
        internal object problemsList;

        public Problems(string title) : base(title)
        {
            activeConcern = new List<ActiveConcern>();
            resolvedConcern = new List<ResolvedConcern>();
        }
        public List<ActiveConcern> activeConcern { get; set; }
        public List<ResolvedConcern> resolvedConcern { get; set; }
    }
    public class ActiveConcern
    {
        public string Problem { get; set; }
    }
    public class ResolvedConcern
    {
        public string Problem { get; set; }
    }
}