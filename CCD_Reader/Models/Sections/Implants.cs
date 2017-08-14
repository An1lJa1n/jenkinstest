using System.Collections.Generic;

namespace Exscribe.Models
{
    public class Implants : Section
    {
        public Implants(string title):base(title)
        {            
        }
        public List<ImplantsList> implantsList { get; set; }
    }
    public class ImplantsList
    {
        public string Implanted { get; set; }
        public string Area { get; set; }
        public string UDI { get; set; }
    }

}