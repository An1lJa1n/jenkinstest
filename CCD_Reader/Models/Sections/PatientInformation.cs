using System.Collections.Generic;

namespace Exscribe.Models
{
    public class PatientInformation
    {
        public PatientInformation()
        {
            toc = new List<TOC>();
        }
        public List<TOC> toc { get; set; }
        public Demographics demographics { get; set; }
        public Allergies allergies { get; set; }
        public Encounters encounters { get; set; }
        public Immunizations immunizations { get; set; }
        public FunctionalStatus functionalStatus { get; set; }
        public Implants implants { get; set; }
        public Goals goals { get; set; }
        public MentalStatus mentalStatus { get; set; }
        public Procedures procedures { get; set; }
        public VitalSigns vitalSigns { get; set; }
        public Medications medications { get; set; }
        public SocialHistory socialHistory { get; set; }
        public Results results { get; set; }
        public Problems problems { get; set; }
        public Assessments assessments { get; set; }
        public ReasonForReferral reasonForReferral { get; set; }
        public HealthConcern healthConcern { get; set; }
        public TreatmentPlan treatmentPlan { get; set; }
        public FamilyHistory familyHistory { get; set; }
        public AdvanceDirectives advanceDirectives { get; set; }
        public Payers payers { get; set; }
        public string Message { get; set; }
    }
}