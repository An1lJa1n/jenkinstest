using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Exscribe.Services;

namespace CCD_Reader
{
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
               
            }

            private void fileBrowser_Click(object sender, EventArgs e)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(File.OpenRead(ofd.FileName));
                    PatientServices pi = new PatientServices();
                    var patientData = pi.LoadPatientData(sr);
                    BindingSource bs = new BindingSource();
                    bs.DataSource = typeof(Result);

                    // foreach(string pd in bs)
                       // {
                    
                       // }

                    bs.Add(new Result { SectionName = "Allergies and Adverse Reactions", Count = patientData.allergies.allergiesList != null ? patientData.allergies.allergiesList.Count : 0 });
                    bs.Add(new Result { SectionName = "Medications", Count = patientData.medications.medicationList != null ? patientData.medications.medicationList.Count : 0 });
                    bs.Add(new Result { SectionName = "Problems", Count = patientData.problems.activeConcern != null ? patientData.problems.activeConcern.Count : 0 });
                    bs.Add(new Result { SectionName = "Problems", Count = patientData.problems.resolvedConcern != null ? patientData.problems.resolvedConcern.Count : 0 });
                    bs.Add(new Result { SectionName = "Encounters", Count = patientData.encounters.encountersList != null ? patientData.encounters.encountersList.Count : 0 });
                    bs.Add(new Result { SectionName = "Immunizations", Count = patientData.immunizations.immunizationsList != null ? patientData.immunizations.immunizationsList.Count : 0 });
                    bs.Add(new Result { SectionName = "Vital Signs", Count = patientData.vitalSigns.vitalsSignsKV != null ? patientData.vitalSigns.vitalsSignsKV.Count : 0 });
                    bs.Add(new Result { SectionName = "Social History", Count = patientData.socialHistory.socialHistoryList != null ? patientData.socialHistory.socialHistoryList.Count : 0 });
                    bs.Add(new Result { SectionName = "Procedures", Count = patientData.procedures.proceduresList != null ? patientData.procedures.proceduresList.Count : 0 });
                    bs.Add(new Result { SectionName = "Implants", Count = patientData.implants.implantsList != null ? patientData.implants.implantsList.Count : 0 });
                    bs.Add(new Result { SectionName = "Results", Count = patientData.results.resultsList != null ? patientData.results.resultsList.Count : 0 });
                    bs.Add(new Result { SectionName = "Functional Status", Count = patientData.functionalStatus.functionalStatusList != null ? patientData.functionalStatus.functionalStatusList.Count : 0 });
                    //bs.Add(new Result { SectionName = "Assessments", Count = patientData.assessments. != null ? patientData.encounters.encountersList.Count : 0 });
                    bs.Add(new Result { SectionName = "Treatment Plan", Count = patientData.treatmentPlan.handOffCommunication != null ? patientData.treatmentPlan.handOffCommunication.Count : 0 });
                    bs.Add(new Result { SectionName = "Goals Section", Count = patientData.goals.goalsList != null ? patientData.goals.goalsList.Count : 0 });
                    //bs.Add(new Result { SectionName = "Reason for Referal", Count = patientData.reasonForReferral.Value != null ? patientData.reasonForReferral.Value.Count : 0 });
                    bs.Add(new Result { SectionName = "Mental Status", Count = patientData.mentalStatus.mentalStatusList != null ? patientData.mentalStatus.mentalStatusList.Count : 0 });
                
                    grid.DataSource = bs;
                    grid.AutoGenerateColumns = true;
                }
            }
    }
}
