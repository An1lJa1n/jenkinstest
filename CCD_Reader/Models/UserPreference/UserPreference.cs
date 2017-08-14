using System;
using System.Collections.Generic;

namespace Exscribe.Models.UserPreference
{
    public class UserPreference
    {
        public List<SelectedSections> selectedSections { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class SelectedSections
    {
        public string DisplayName { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public bool Selected { get; set; }
        public int OrderSequence { get; set; }
    }
}