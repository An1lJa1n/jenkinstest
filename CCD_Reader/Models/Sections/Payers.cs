using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Payers : Section
    {
        public Payers(string title) : base(title)
        {

        }
        public List<PayersList> payersList { get; set; }
    }
    public class PayersList
    {
        public string PayerName         { get; set; }
        public string PolicyType        { get; set; }
        public string PolicyID          { get; set; }
        public string CoveredPartyID    { get; set; }
        public string PolicyHolder      { get; set; }
    }
}