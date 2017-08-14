using System;
using System.Collections.Generic;
using System.Linq;
using Exscribe.Models;
using Exscribe.Common;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Exscribe.Services
{
    public class PatientServices
    {
        public readonly Dictionary<string, Action<POCD_MT000040Section>> _sectionFunctions;
        PatientInformation _patientInformation = new PatientInformation();

        public PatientServices()
        {

            _sectionFunctions = new Dictionary<string, Action<POCD_MT000040Section>> {

                { "10160-0", LoadMedications },
                { "11369-6", LoadImmunizations },
                { "46240-8", LoadEncounters },
                { "8716-3",  LoadVitalSigns },
                { "29762-2", LoadSocialHistory },
                { "47519-4", LoadProcedures },
                { "30954-2", LoadResults },
                { "47420-5", LoadFunctionalStatus },
                { "11450-4", LoadProblems },
                { "46264-8", LoadImplants },
                { "51848-0", LoadAssessments },
                { "61146-7", LoadGoals },
                { "42349-1", LoadReasonForReferral },
                { "48765-2", LoadAllergies },
                { "10190-7", LoadMentalStatus },
                { "75310-3", LoadHealthConcern },
                { "18776-5", LoadTreatmentPlan },
                { "10157-6", LoadFamilyHistory },
                { "48768-6", LoadPayers },
                { "30954-5", LoadAdvanceDirectives }
            };
        }

       
        private Demographics LoadDemographics(POCD_MT000040ClinicalDocument clinicalDocument)
        {
            string personalRelationship = string.Empty, careProvision = string.Empty, informant = string.Empty;
            //string EncounterDate = string.Empty, EncounterDateLow = string.Empty, EncounterDateHigh = string.Empty, EncounterDateLowTime = string.Empty, EncounterDateHighTime = string.Empty;
            string Author = string.Empty;

            Demographics demographics = new Demographics();
            try
            {
                if (clinicalDocument != null)
                {
                    foreach (var name in clinicalDocument.recordTarget[0].patientRole.patient.name[0].Items)
                    {
                        if (name.GetType().Name == "engiven")
                        {
                            demographics.FirstName = name.Text[0];
                        }
                        else if (name.GetType().Name == "enfamily")
                        {
                            demographics.LastName = name.Text[0];
                        }
                    }

                    demographics.DOB = CommonServices.FormatDate(clinicalDocument.recordTarget[0].patientRole.patient.birthTime.value, "MMMM dd, yyyy");
                    demographics.Race = clinicalDocument.recordTarget[0].patientRole.patient.raceCode.displayName;
                    demographics.Sex = clinicalDocument.recordTarget[0].patientRole.patient.administrativeGenderCode.displayName;
                    demographics.Ethnicity = clinicalDocument.recordTarget[0].patientRole.patient.ethnicGroupCode.displayName;
                    demographics.PatientID = clinicalDocument.recordTarget[0].patientRole.id[0].extension + " " + clinicalDocument.recordTarget[0].patientRole.id[0].root;
                    demographics.Language = new CultureInfo(clinicalDocument.languageCode.code).DisplayName;
                    demographics.DocumentID = clinicalDocument.id.extension + " " + clinicalDocument.id.root;
                    demographics.DocumentCreated = CommonServices.FormatDate(clinicalDocument.effectiveTime.value, "MMMM dd, yyyy");
                    demographics.CareProvision = careProvision;
                    demographics.PrimaryCarePhysician = "";
                    demographics.Performer = "";

                    if (clinicalDocument.author[0].assignedAuthor.Item.GetType().Name == "POCD_MT000040AuthoringDevice")
                    {
                        Author = ((POCD_MT000040AuthoringDevice)clinicalDocument.author[0].assignedAuthor.Item).softwareName.Text[0];
                    }
                    demographics.Author = Author;
                    demographics.EncounterId = clinicalDocument.componentOf.encompassingEncounter.id[0].extension + " " + clinicalDocument.componentOf.encompassingEncounter.id[0].root;
                    demographics.EncounterDate = "From " + CommonServices.FormatDate(((TS)clinicalDocument.componentOf.encompassingEncounter.effectiveTime.Items[0]).value, "MMMM dd, yyyy") + " To " + CommonServices.FormatDate(((TS)clinicalDocument.componentOf.encompassingEncounter.effectiveTime.Items[1]).value, "MMMM dd, yyyy");
                    demographics.PersonalRelationship = personalRelationship;
                    //demographics.EnteredBy = clinicalDocument.dataEnterer.assignedEntity.assignedPerson.name[0].Items[0].Text[0]; ;
                    //demographics.EnteredBy = clinicalDocument.dataEnterer.assignedEntity.assignedPerson.name[0].Items.AsEnumerable().Select(a => Convert.ToString(a.Text[0])).Aggregate((i, j) => i + " " + j);
                    demographics.EnteredBy = "";
                    demographics.Signed = "";
                    if (clinicalDocument.informant != null)
                    {
                        foreach (var informantItem in clinicalDocument.informant)
                        {
                            if (informantItem.Item.GetType().Name == "POCD_MT000040AssignedEntity")
                            {
                                informant = ((POCD_MT000040AssignedEntity)clinicalDocument.informant[0].Item).assignedPerson.name[0].Items.AsEnumerable().Select(a => Convert.ToString(a.Text[0])).Aggregate((i, j) => i + " " + j);
                            }
                        }
                    }

                    demographics.Informant = informant;
                    demographics.InformationRecipient = "";
                    demographics.LegalAuthenticator = "";
                    demographics.DocumentMaintainedBy = "";
                }
            }
            catch (Exception ex)
            {
                demographics.Message = "Exception Occurred";

            }

            return demographics;
        }

        private void LoadImmunizations(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<ImmunizationsList> immunizationsList = new List<ImmunizationsList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Immunizations immunizations = new Immunizations(title);

            string vaccine = string.Empty;
            string date = string.Empty;
            string status = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    immunizations.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (((POCD_MT000040Material)((POCD_MT000040SubstanceAdministration)entry.Item).consumable.manufacturedProduct.Item).code.nullFlavor == null)
                        {
                            vaccine = ((POCD_MT000040Material)((POCD_MT000040SubstanceAdministration)entry.Item).consumable.manufacturedProduct.Item).code.displayName;
                            date = ((POCD_MT000040SubstanceAdministration)entry.Item).effectiveTime[0].value;
                            date = CommonServices.FormatDate(date, "MMMM dd, yyyy");
                            status = ((CD)(((POCD_MT000040SubstanceAdministration)(entry.Item)).statusCode)).code;

                            immunizationsList.Add(new ImmunizationsList
                            {
                                Vaccine = vaccine,
                                Date = date,
                                Status = status
                            });
                        }
                        else
                        {
                            immunizations.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                        }
                    }
                }

                immunizations.immunizationsList = immunizationsList;
            }
            catch (Exception ex)
            {
                immunizations.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.immunizations = immunizations;

            #endregion
        }
        private void LoadFunctionalStatus(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<FunctionalStatusList> functionalStatusList = new List<FunctionalStatusList>();

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            FunctionalStatus functionalStatus = new FunctionalStatus(title);
            string functionalCategory = string.Empty;
            string effectiveDates = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    functionalStatus.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (entry.Item.GetType().Name == "POCD_MT000040Observation")
                        {
                            functionalCategory = ((CD)(((POCD_MT000040Observation)(entry.Item)).value[0])).displayName;

                            if (((TS)(((POCD_MT000040Observation)(entry.Item)).effectiveTime)).value != null)
                            {
                                effectiveDates = ((TS)(((POCD_MT000040Observation)(entry.Item)).effectiveTime)).value;
                            }
                            else
                            {
                                effectiveDates = ((TS)((POCD_MT000040Observation)entry.Item).effectiveTime.Items[0]).value;
                            }

                            effectiveDates = CommonServices.FormatDate(effectiveDates, "dd MMMM yyyy");

                            functionalStatusList.Add(new FunctionalStatusList
                            {
                                FunctionalCategory = functionalCategory,
                                EffectiveDates = effectiveDates
                            });
                        }
                        else
                        {
                            foreach (var component in ((POCD_MT000040Organizer)entry.Item).component)
                            {
                                functionalCategory = ((CD)(((POCD_MT000040Observation)(component.Item)).value[0])).displayName;
                                effectiveDates = ((TS)(((POCD_MT000040Observation)(component.Item)).effectiveTime)).value;

                                effectiveDates = CommonServices.FormatDate(effectiveDates, "dd MMMM yyyy");

                                functionalStatusList.Add(new FunctionalStatusList
                                {
                                    FunctionalCategory = functionalCategory,
                                    EffectiveDates = effectiveDates
                                });
                            }
                        }
                       
                    }
                }

                functionalStatus.functionalStatusList = functionalStatusList;
            }
            catch (Exception ex)
            {
                functionalStatus.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.functionalStatus = functionalStatus;

            #endregion
        }
        private void LoadEncounters(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<EncountersList> encountersList = new List<EncountersList>();

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Encounters encounters = new Encounters(title);

            string EncounterDiagnosis = string.Empty;
            string Encounter = string.Empty;
            string Location = string.Empty;
            string Date = string.Empty;

            #endregion

            #region Section Extraction
            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    encounters.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (((POCD_MT000040Encounter)entry.Item).entryRelationship != null)
                        {
                            EncounterDiagnosis = ((CD)(((POCD_MT000040Observation)(((POCD_MT000040Act)(((POCD_MT000040Encounter)entry.Item).entryRelationship[0].Item)).entryRelationship[0].Item)).value[0])).displayName;
                        }

                        Encounter = ((POCD_MT000040Encounter)entry.Item).code.displayName;
                        Location = (((POCD_MT000040PlayingEntity)((POCD_MT000040Encounter)entry.Item).participant[0].participantRole.Item).name)[0].Text[0];
                        Date = ((POCD_MT000040Encounter)entry.Item).effectiveTime.value;

                        encountersList.Add(new EncountersList
                        {
                            Encounter = Encounter,
                            EncounterDiagnosis = EncounterDiagnosis,
                            Location = Location,
                            Date = Date
                        });
                    }
                }

                encounters.encountersList = encountersList;
            }
            catch (Exception ex)
            {
                encounters.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.encounters = encounters;

            #endregion
        }
        private void LoadAllergies(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<AllergiesList> allergiesList = new List<AllergiesList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Allergies allergies = new Allergies(title);

            string Substance = string.Empty;
            string Reaction = string.Empty;
            string Severity = string.Empty;
            string Status = string.Empty;

            #endregion

            #region Section Extraction
            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    allergies.Message = new Reponse() { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        POCD_MT000040Act act = (POCD_MT000040Act)entry.Item;
                        if (((CD)(((POCD_MT000040PlayingEntity)(((POCD_MT000040Observation)(((POCD_MT000040Act)(entry.Item)).entryRelationship[0].Item)).participant[0].participantRole.Item)).code)).nullFlavor == null)
                        {
                            Substance = ((CD)(((POCD_MT000040PlayingEntity)(((POCD_MT000040Observation)(((POCD_MT000040Act)(entry.Item)).entryRelationship[0].Item)).participant[0].participantRole.Item)).code)).displayName;
                            Reaction = ((CD)(((POCD_MT000040Observation)(((POCD_MT000040Observation)(((POCD_MT000040Act)(entry.Item)).entryRelationship[0].Item)).entryRelationship[0].Item)).value[0])).displayName;
                            Severity = ((CD)(((POCD_MT000040Observation)(((POCD_MT000040Observation)(((POCD_MT000040Observation)(((POCD_MT000040Act)(entry.Item)).entryRelationship[0].Item)).entryRelationship[0].Item)).entryRelationship[0].Item)).value[0])).displayName;
                            Status = act.statusCode.code.ToString();

                            allergiesList.Add(new AllergiesList
                            {
                                Substance = Substance,
                                Reaction = Reaction,
                                Severity = Severity,
                                Status = Status
                            });
                        }
                        else
                        {
                            allergies.Message = new Reponse() { Message = "No Data Available for this section", ResponseStatus = "success" };
                        }
                    }
                }

                allergies.allergiesList = allergiesList;
            }
            catch (Exception ex)
            {
                allergies.Message = new Reponse() { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.allergies = allergies;

            #endregion

        }
        private void LoadMentalStatus(POCD_MT000040Section section)
        {
            #region Variable Initialization

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;
            List<MentalStatusList> mentalStatusList = new List<MentalStatusList>();

            string title = string.Join("", section.title.Text);
            MentalStatus mentalStatus = new MentalStatus(title);

            string Date = string.Empty;
            string Status = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    mentalStatus.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        Status = ((CD)((POCD_MT000040Observation)entry.Item).value[0]).displayName;
                        Date = ((TS)((POCD_MT000040Observation)entry.Item).effectiveTime.Items[0]).value;
                        Date = CommonServices.FormatDate(Date, "MMMM dd, yyyy");
                        mentalStatusList.Add(new MentalStatusList
                        {
                            Date = Date,
                            Status = Status,
                        });
                    }
                }

                mentalStatus.mentalStatusList = mentalStatusList;
            }
            catch (Exception ex)
            {
                mentalStatus.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.mentalStatus = mentalStatus;

            #endregion
        }
        private void LoadGoals(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<GoalsList> goalSectionsList = new List<GoalsList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Goals goalSections = new Goals(title);

            string Goal = string.Empty;
            string Value = string.Empty;
            string Date = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    goalSections.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        Goal = ((BIN)((POCD_MT000040Observation)entry.Item).value[0]).Text[0];
                        Date = ((POCD_MT000040Observation)entry.Item).effectiveTime.value;
                        Date = CommonServices.FormatDate(Date, "MMMM dd, yyyy");
                        goalSectionsList.Add(new GoalsList
                        {
                            Goal = Goal,
                            Value = Value,
                            Date = Date,
                        });
                    }
                }
                goalSections.goalsList = goalSectionsList;
            }
            catch (Exception ex)
            {
                goalSections.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.goals = goalSections;

            #endregion
        }
        private void LoadImplants(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<ImplantsList> implantsList = new List<ImplantsList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Implants implants = new Implants(title);

            string Implanted = string.Empty;
            string Area = string.Empty;
            string UDI = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    implants.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (((POCD_MT000040Device)((POCD_MT000040Procedure)entry.Item).participant[0].participantRole.Item).code.nullFlavor == null)
                        {
                            Implanted = ((POCD_MT000040Device)((POCD_MT000040Procedure)entry.Item).participant[0].participantRole.Item).code.displayName;
                            Area = ((POCD_MT000040Procedure)entry.Item).targetSiteCode[0].displayName;
                            UDI = ((POCD_MT000040Procedure)entry.Item).participant[0].participantRole.id[0].extension;

                            implantsList.Add(new ImplantsList
                            {
                                Implanted = Implanted,
                                Area = Area,
                                UDI = UDI,
                            });
                        }
                        else
                        {
                            implants.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                        }
                    }
                }

                implants.implantsList = implantsList;
            }
            catch (Exception ex)
            {
                implants.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.implants = implants;

            #endregion
        }
        private void LoadProcedures(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<ProceduresList> proceduresList = new List<ProceduresList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Procedures procedures = new Procedures(title);

            string Procedure = string.Empty;
            string Date = string.Empty;

            #endregion

            #region Section Extraction
            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    procedures.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        Procedure = ((POCD_MT000040Procedure)(entry.Item)).code.displayName;
                        Date = ((TS)(((POCD_MT000040Procedure)(entry.Item)).effectiveTime)).value;
                        Date = CommonServices.FormatDate(Date, "MMMM dd, yyyy");

                        proceduresList.Add(new ProceduresList
                        {
                            Procedure = Procedure,
                            Date = Date
                        });
                    }
                }
                procedures.proceduresList = proceduresList;
            }
            catch (Exception ex)
            {
                procedures.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }
            _patientInformation.procedures = procedures;

            #endregion
        }
        private void LoadSocialHistory(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<SocialHistoryList> socialHistoryList = new List<SocialHistoryList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            SocialHistory socialHistory = new SocialHistory(title);

            string SocialHistoryObservation = string.Empty;
            string DescriptionTypeName = string.Empty;
            string Description = string.Empty;
            string DatesObserved = string.Empty;

            #endregion

            #region Section Extraction
            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    socialHistory.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        SocialHistoryObservation = ((POCD_MT000040Observation)(entry.Item)).code.displayName;
                        DescriptionTypeName = (((POCD_MT000040Observation)(entry.Item)).value[0]).GetType().Name;
                        Description = string.Empty;

                        if (DescriptionTypeName == "CD")
                        {
                            Description = ((CD)(((POCD_MT000040Observation)(entry.Item)).value[0])).displayName;
                        }
                        else if (DescriptionTypeName == "ST")
                        {
                            Description = ((ST)((POCD_MT000040Observation)entry.Item).value[0]).Text[0];
                        }

                        if (((POCD_MT000040Observation)(entry.Item)).effectiveTime.Items == null)
                        {
                            DatesObserved = ((TS)(((POCD_MT000040Observation)(entry.Item)).effectiveTime)).value;
                            DatesObserved = CommonServices.FormatDate(DatesObserved, "MMMM dd, yyyy");
                        }
                        else if (((POCD_MT000040Observation)(entry.Item)).effectiveTime.Items.Count() > 1)
                        {
                            DatesObserved = CommonServices.FormatDate(((TS)(((POCD_MT000040Observation)(entry.Item)).effectiveTime.Items[0])).value, "MMMM dd, yyyy") + " - " + CommonServices.FormatDate(((TS)(((POCD_MT000040Observation)(entry.Item)).effectiveTime.Items[1])).value, "MMMM dd, yyyy");
                        }

                        socialHistoryList.Add(new SocialHistoryList
                        {
                            SocialHistoryObservation = SocialHistoryObservation,
                            Description = Description,
                            DatesObserved = DatesObserved
                        });
                    }
                }

                socialHistory.socialHistoryList = socialHistoryList;
            }
            catch (Exception ex)
            {
                socialHistory.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.socialHistory = socialHistory;

            #endregion

        }
        private void LoadVitalSigns(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<VitalsSignsKV> vitalSignsKV = new List<VitalsSignsKV>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            VitalSigns vitalSigns = new VitalSigns(title);

            string DateTime = string.Empty;
            string Key = string.Empty;
            string Value = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    vitalSigns.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "Succes" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (((POCD_MT000040Organizer)(entry.Item)).effectiveTime.Items == null)
                        {
                            DateTime = ((TS)(((POCD_MT000040Organizer)(entry.Item)).effectiveTime)).value;
                        }
                        else
                        {
                            DateTime = ((TS)(((POCD_MT000040Organizer)(entry.Item)).effectiveTime.Items[0])).value;
                        }
                        DateTime = CommonServices.FormatDate(DateTime, "MMMM dd, yyyy");
                        foreach (var component in ((POCD_MT000040Organizer)entry.Item).component)
                        {
                            Key = ((POCD_MT000040Observation)(component.Item)).code.displayName;
                            Value = ((PQ)(((POCD_MT000040Observation)(component.Item)).value[0])).value + " " + ((PQ)(((POCD_MT000040Observation)(component.Item)).value[0])).unit;

                            vitalSignsKV.Add(new VitalsSignsKV { Key = Key, Value = Value });
                        }

                        vitalSigns.DateTime = DateTime;
                        vitalSigns.vitalsSignsKV = vitalSignsKV;
                    }
                }
            }
            catch (Exception ex)
            {
                vitalSigns.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "Failed" };
            }

            _patientInformation.vitalSigns = vitalSigns;

            #endregion

        }
        private void LoadMedications(POCD_MT000040Section section)
        {
            #region Variable Initialization

            string title = string.Join("", section.title.Text);
            Medications medications = new Medications(title);

            List<MedicationList> medicationsList = new List<MedicationList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string Medication = string.Empty;
            string Directions = string.Empty;
            string StartDate = string.Empty;
            string Status = string.Empty;
            string Indications = string.Empty;
            string FillInstructions = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    medications.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        POCD_MT000040SubstanceAdministration substanceAdministration = (POCD_MT000040SubstanceAdministration)entry.Item;

                        if (((POCD_MT000040Material)substanceAdministration.consumable.manufacturedProduct.Item).code.nullFlavor == null)
                        {
                            Medication = ((POCD_MT000040Material)substanceAdministration.consumable.manufacturedProduct.Item).code.displayName;

                            if (((POCD_MT000040SubstanceAdministration)entry.Item).effectiveTime[0].value == null)
                            {
                                StartDate = ((IVXB_TS)((IVL_TS)substanceAdministration.effectiveTime[0]).Items[0]).value;
                            }
                            else
                            {
                                StartDate = ((POCD_MT000040SubstanceAdministration)entry.Item).effectiveTime[0].value;
                            }

                            StartDate = CommonServices.FormatDate(StartDate, "MMMM dd, yyyy");

                            if (((POCD_MT000040SubstanceAdministration)entry.Item).entryRelationship != null)
                            {
                                foreach (var ER in ((POCD_MT000040SubstanceAdministration)entry.Item).entryRelationship)
                                {
                                    if (ER.Item.GetType().Name == "POCD_MT000040Observation")
                                    {
                                        Indications = ((CD)((POCD_MT000040Observation)((POCD_MT000040SubstanceAdministration)entry.Item).entryRelationship[0].Item).value[0]).displayName;
                                    }
                                }
                            }

                            medicationsList.Add(new MedicationList()
                            {
                                Medication = Medication,
                                Directions = Directions,
                                StartDate = StartDate,
                                Status = Status,
                                FillInstructions = FillInstructions,
                                Indications = Indications
                            });

                        }
                        else
                        {
                            medications.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                        }
                    }
                }

                medications.medicationList = medicationsList;
            }
            catch (Exception ex)
            {
                medications.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.medications = medications;

            #endregion
        }
        private void LoadResults(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<ResultsList> resultsList = new List<ResultsList>();
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Results results = new Results(title);

            string resultType = string.Empty;
            string resultValueTypeName = string.Empty;
            string resultValue = string.Empty;
            string relevantReferenceRangeTypeName = string.Empty;
            string relevantReferenceRange = string.Empty;
            string interpretation = string.Empty;
            string date = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    results.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        foreach (var component in ((POCD_MT000040Organizer)entry.Item).component)
                        {
                            resultType = ((POCD_MT000040Observation)(component.Item)).code.displayName;
                            resultValueTypeName = (((POCD_MT000040Observation)(component.Item)).value[0]).GetType().Name;
                            resultValue = string.Empty;

                            if (resultValueTypeName == "ST")
                            {
                                resultValue = ((ST)(((POCD_MT000040Observation)(component.Item)).value[0])).Text[0];
                            }
                            else if (resultValueTypeName == "PQ")
                            {
                                resultValue = ((PQ)(((POCD_MT000040Observation)(component.Item)).value[0])).value + " " + (((PQ)(((POCD_MT000040Observation)(component.Item)).value[0])).unit == "1" ? "" : ((PQ)(((POCD_MT000040Observation)(component.Item)).value[0])).unit);
                            }
                            else if (resultValueTypeName == "CO")
                            {
                                resultValue = ((CO)(((POCD_MT000040Observation)(component.Item)).value[0])).displayName;
                            }
                            relevantReferenceRangeTypeName = string.Empty;
                            relevantReferenceRange = string.Empty;

                            if (((POCD_MT000040Observation)(component.Item)).referenceRange[0].observationRange.value != null)
                            {
                                relevantReferenceRangeTypeName = (((POCD_MT000040Observation)(component.Item)).referenceRange[0].observationRange.value).GetType().Name;
                            }
                            if (relevantReferenceRangeTypeName != null)
                            {
                                if (relevantReferenceRangeTypeName == "ST")
                                {
                                    relevantReferenceRange = ((ST)(((POCD_MT000040Observation)(component.Item)).referenceRange[0].observationRange.value)).Text[0];
                                }
                                else if (relevantReferenceRangeTypeName == "IVL_PQ")
                                {
                                    relevantReferenceRange = ((IVL_PQ)(((POCD_MT000040Observation)(component.Item)).referenceRange[0].observationRange.value)).Items.
                                         AsEnumerable().Select(a => Convert.ToString(a.value)).Aggregate((i, j) => i + " - " + j);
                                }
                            }
                            else
                            {
                                relevantReferenceRange = ((POCD_MT000040Observation)component.Item).referenceRange[0].observationRange.text.Text[0];
                            }

                            date = ((TS)(((POCD_MT000040Observation)(component.Item)).effectiveTime)).value;
                            date = CommonServices.FormatDate(date, "MMMM dd, yyyy");
                            resultsList.Add(new ResultsList
                            {
                                ResultType = resultType,
                                ResultValue = resultValue,
                                RelevantReferenceRange = relevantReferenceRange,
                                Interpretation = interpretation,
                                Date = date
                            });
                        }
                        results.resultsList = resultsList;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Message = new Reponse { Message = "Unable to load the section ", ResponseStatus = "failed" };
            }

            _patientInformation.results = results;

            #endregion
        }
        private void LoadTreatmentPlan(POCD_MT000040Section section)
        {
            #region Variable Initialization            
            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;
            List<HandOffCommunication> handOffCommunication = new List<HandOffCommunication>();

            string title = string.Join("", section.title.Text);
            TreatmentPlan treatmentPlan = new TreatmentPlan(title);

            string plannedCare = string.Empty;
            string startDate = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    treatmentPlan.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        if (entry.Item.GetType().Name == "POCD_MT000040Observation")
                        {
                            plannedCare = ((POCD_MT000040Observation)entry.Item).code.displayName;
                            startDate = ((POCD_MT000040Observation)entry.Item).effectiveTime.value.Length > 8 ? ((POCD_MT000040Observation)entry.Item).effectiveTime.value.Substring(0, 8) : ((POCD_MT000040Observation)entry.Item).effectiveTime.value;
                            startDate = CommonServices.FormatDate(startDate, "MMMM dd, yyyy");
                        }
                        else if (entry.Item.GetType().Name == "POCD_MT000040SubstanceAdministration")
                        {
                            plannedCare = ((POCD_MT000040SubstanceAdministration)entry.Item).text.Text[0];
                            startDate = ((POCD_MT000040SubstanceAdministration)entry.Item).effectiveTime[0].value;
                            startDate = CommonServices.FormatDate(startDate, "MMMM dd, yyyy");
                        }
                        else if (entry.Item.GetType().Name == "POCD_MT000040Encounter")
                        {
                            plannedCare = ((POCD_MT000040Observation)((POCD_MT000040Encounter)entry.Item).entryRelationship[0].Item).text.Text[0];
                            startDate = ((TS)((POCD_MT000040Observation)((POCD_MT000040Encounter)entry.Item).entryRelationship[0].Item).effectiveTime.Items[0]).value;
                            startDate = CommonServices.FormatDate(startDate, "MMMM dd, yyyy");
                        }
                        handOffCommunication.Add(new HandOffCommunication
                        {
                            PlannedCare = plannedCare,
                            StartDate = startDate
                        });
                    }
                }

                treatmentPlan.handOffCommunication = handOffCommunication;
            }
            catch (Exception ex)
            {
                treatmentPlan.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.treatmentPlan = treatmentPlan;

            #endregion
        }
        private void LoadReasonForReferral(POCD_MT000040Section section)
        {
            #region Variable Initialization

            string title = string.Join("", section.title.Text);
            ReasonForReferral reasonForReferral = new ReasonForReferral(title);

            string value = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null)
                {
                    reasonForReferral.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    if (section.text.Text != null)
                    {
                        value = section.text.Text[0];
                    }
                    else
                    {
                        value = ((StrucDocParagraph)section.text.Items[0]).Text[0];
                    }

                    reasonForReferral.Value = value;
                }
            }
            catch (Exception ex)
            {
                reasonForReferral.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.reasonForReferral = reasonForReferral;

            #endregion

        }
        private void LoadProblems(POCD_MT000040Section section)
        {
            #region Variable Initialization


            List<ActiveConcern> activeConcern = new List<ActiveConcern>();
            List<ResolvedConcern> resolvedConcern = new List<ResolvedConcern>();

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            Problems problems = new Problems(title);

            string problem = string.Empty;
            string concernType = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    problems.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        problem = ((CD)(((POCD_MT000040Observation)(((POCD_MT000040Act)(entry.Item)).entryRelationship[0].Item)).value[0])).displayName;
                        concernType = ((CD)(((POCD_MT000040Act)(entry.Item)).statusCode)).code;

                        if (concernType == "active")
                        {
                            activeConcern.Add(new ActiveConcern { Problem = problem });
                        }
                        else if (concernType == "completed")
                        {
                            resolvedConcern.Add(new ResolvedConcern { Problem = problem });
                        }
                    }
                }

                problems.activeConcern = activeConcern;
                problems.resolvedConcern = resolvedConcern;
            }
            catch (Exception ex)
            {
                problems.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.problems = problems;

            #endregion
        }
        private void LoadAssessments(POCD_MT000040Section section)
        {
            #region Variable Initialization

            string title = string.Join("", section.title.Text);
            string displayName = CommonServices.UppercaseWords(string.Join("", section.title.Text).ToLower());
            string value = string.Empty;

            Assessments assessment = new Assessments(title);

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null)
                {
                    assessment.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    value = section.text.Text[0];

                    assessment.Value = value;
                }
            }
            catch (Exception ex)
            {
                assessment.Message = new Reponse { Message = "Unable to load the data", ResponseStatus = "success" };
            }

            _patientInformation.assessments = assessment;

            #endregion
        }
        private void LoadHealthConcern(POCD_MT000040Section section)
        {
            #region Variable Initialization

            List<Observations> observationsList = new List<Observations>();
            List<Concerns> concernsList = new List<Concerns>();

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;

            string title = string.Join("", section.title.Text);
            HealthConcern healthConcernObj = new HealthConcern(title);

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    healthConcernObj.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    //Mapping is not present
                }
            }
            catch (Exception ex)
            {
                healthConcernObj.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            #endregion
        }
        private void LoadFamilyHistory(POCD_MT000040Section section)
        {
            #region Variable Initialization

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;
            List<FamilyHistoryList> familyHistoryList = new List<FamilyHistoryList>();

            string title = string.Join("", section.title.Text);
            FamilyHistory familyHistory = new FamilyHistory(title);

            string condition = string.Empty;
            string familyRelationship = string.Empty;
            string ageOfOnset = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    familyHistory.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        familyRelationship = ((POCD_MT000040Organizer)entry.Item).subject.relatedSubject.code.displayName;
                        if (((POCD_MT000040Organizer)entry.Item).component.Count() > 1)
                        {
                            foreach (var component in ((POCD_MT000040Organizer)entry.Item).component)
                            {
                                condition = ((CD)((POCD_MT000040Observation)component.Item).value[0]).displayName;

                                ((POCD_MT000040Observation)component.Item).entryRelationship.ToList()
                                    .ForEach(a =>
                                    {
                                        if (a.typeCode.ToString() == "SUBJ")
                                        {
                                            ageOfOnset = ((PQ)((POCD_MT000040Observation)a.Item).value[0]).value;
                                        }
                                    });

                                familyHistoryList.Add(new FamilyHistoryList()
                                {
                                    Condition = condition,
                                    FamilyRelationship = familyRelationship,
                                    AgeOfOnset = ageOfOnset,
                                });
                            }
                        }
                        else
                        {
                            condition = ((CD)((POCD_MT000040Observation)((POCD_MT000040Organizer)entry.Item).component[0].Item).value[0]).displayName;
                            ageOfOnset = ((PQ)((POCD_MT000040Observation)((POCD_MT000040Observation)((POCD_MT000040Organizer)entry.Item).component[0].Item).entryRelationship[1].Item).value[0]).value;

                            familyHistoryList.Add(new FamilyHistoryList()
                            {
                                Condition = condition,
                                FamilyRelationship = familyRelationship,
                                AgeOfOnset = ageOfOnset,
                            });
                        }
                    }
                    familyHistory.familyHistoryList = familyHistoryList;
                }
            }
            catch (Exception ex)
            {
                familyHistory.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.familyHistory = familyHistory;

            #endregion
        }
        private void LoadPayers(POCD_MT000040Section section)
        {
            #region Variable Initialization

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;
            List<PayersList> payersList = new List<PayersList>();

            string title = string.Join("", section.title.Text);
            Payers payers = new Payers(title);

            string payerName = string.Empty;
            string policyType = string.Empty;
            string policyID = string.Empty;
            string coveredPartyID = string.Empty;
            string policyHolder = string.Empty;

            #endregion

            #region Section Extraction

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    payers.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        payersList.Add(new PayersList()
                        {
                            CoveredPartyID = coveredPartyID,
                            PayerName = payerName,
                            PolicyHolder = policyHolder,
                            PolicyID = policyID,
                            PolicyType = policyType
                        });
                    }
                    payers.payersList = payersList;
                }
            }
            catch (Exception ex)
            {
                payers.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }
            _patientInformation.payers = payers;
            #endregion
        }
        private void LoadAdvanceDirectives(POCD_MT000040Section section)
        {
            #region Variable Initialization

            POCD_MT000040Entry[] pOCD_MT000040Entry = section.entry;
            List<AdvanceDirectivesList> advanceDirectivesList = new List<AdvanceDirectivesList>();

            string title = string.Join("", section.title.Text);
            AdvanceDirectives advanceDirectives = new AdvanceDirectives(title);

            string directive = string.Empty;
            string description = string.Empty;
            string verification = string.Empty;
            string supportingDocument = string.Empty;

            #endregion

            try
            {
                if (section.nullFlavor != null || pOCD_MT000040Entry == null)
                {
                    advanceDirectives.Message = new Reponse { Message = "No Data Available for this section", ResponseStatus = "success" };
                }
                else
                {
                    foreach (var entry in pOCD_MT000040Entry)
                    {
                        directive = ((POCD_MT000040Observation)entry.Item).code.displayName;
                        description = ((CD)((POCD_MT000040Observation)entry.Item).value[0]).displayName;
                        supportingDocument = ((POCD_MT000040ExternalDocument)((POCD_MT000040Observation)entry.Item).reference[0].Item).text.reference.value;

                        ((POCD_MT000040Observation)entry.Item).participant.ToList()
                            .ForEach(vrf =>
                            {
                                if (vrf.typeCode.ToString() == "VRF")
                                {
                                    verification = "";
                                    ((POCD_MT000040PlayingEntity)vrf.participantRole.Item).name[0].Items.ToList()
                                    .ForEach(
                                        txt =>
                                        {
                                            verification += txt.Text[0] + " ";
                                        });
                                }
                            });

                        advanceDirectivesList.Add(new AdvanceDirectivesList()
                        {
                            Directive = directive,
                            Description = description,
                            Verification = verification,
                            SupportingDocument = supportingDocument,
                        });
                    }
                    advanceDirectives.advanceDirectivesList = advanceDirectivesList;
                }
            }
            catch (Exception ex)
            {
                advanceDirectives.Message = new Reponse { Message = "Unable to load the section", ResponseStatus = "failed" };
            }

            _patientInformation.advanceDirectives = advanceDirectives;
        }

        internal PatientInformation LoadPatientData(StreamReader sr)
        {
            try
            {
                POCD_MT000040ClinicalDocument  clinicalDocument = CommonServices.Deserialize<POCD_MT000040ClinicalDocument>(sr.BaseStream);
                if (clinicalDocument != null)
                {
                    POCD_MT000040StructuredBody structuredBody = (POCD_MT000040StructuredBody)clinicalDocument.component.Item;

                    #region Fetching Patient Information and Sections

                    _patientInformation.demographics = LoadDemographics(clinicalDocument);

                    if (structuredBody != null && structuredBody.component != null)
                    {
                        int SeqNo = 1;

                        foreach (var item in structuredBody.component)
                        {
                            string code = item.section.code.code;

                            _patientInformation.toc.Add(new TOC { Title = string.Join("", item.section.title.Text), DisplayName = CommonServices.UppercaseWords(string.Join("", item.section.title.Text).ToLower()), Code = code });

                            LoadSections(code, item.section);

                            SeqNo++;
                        }
                    }

                    #endregion
                }
                else
                {
                    _patientInformation.Message = "Clinical Document not found";
                }
            }
            catch (Exception ex)
            {
                _patientInformation.Message = "Exception Occurred";
            }

            return _patientInformation;
        }
        private void LoadSections(string code, POCD_MT000040Section section)
        {
            if (_sectionFunctions.ContainsKey(code))
            {
                _sectionFunctions[code].Invoke(section);
            }
        }
    }
}