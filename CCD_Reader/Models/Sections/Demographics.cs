namespace Exscribe.Models
{
    public class Demographics
    {
        //public Demographics(string title)
        //{
        //    Title = title;
        //    DisplayName = CommonServices.UppercaseWords(title.ToLower());
        //}

        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatientID { get; set; }
        public string DOB { get; set; }
        public string Sex { get; set; }
        public string Race { get; set; }
        public string Ethnicity { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }
        public string DocumentID { get; set; }
        public string DocumentCreated { get; set; }
        public string CareProvision { get; set; }
        public string PrimaryCarePhysician { get; set; }
        public string Performer { get; set; }
        public string Author { get; set; }
        public string EncounterId { get; set; }
        public string EncounterDate { get; set; }
        public string PersonalRelationship { get; set; }
        public string EnteredBy { get; set; }
        public string Signed { get; set; }
        public string Informant { get; set; }
        public string InformationRecipient { get; set; }
        public string LegalAuthenticator { get; set; }
        public string DocumentMaintainedBy { get; set; }
    }
}