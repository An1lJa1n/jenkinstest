using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Procedures : Section
    {
        public Procedures(string title) : base(title)
        {
        }
        public List<ProceduresList> proceduresList { get; set; }
    }
    public class ProceduresList
    {
        public string Procedure { get; set; }
        public string Date { get; set; }
    }
}