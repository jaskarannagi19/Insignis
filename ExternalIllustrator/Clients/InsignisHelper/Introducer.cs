using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Encryption;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Authentication;
using Octavo.Gate.Nabu.Entities.Core;
using Octavo.Gate.Nabu.Entities.PeopleAndPlaces;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Helper.UI.Controls
{
    public class Introducer : BaseType
    {
        public int? ID = null;
        public int? OrganisationID = null;
        public string Organisation = null;
        public string Name = null;
        public string ContactTelephone = null;
        public string ContactEmail = null;
        public List<string> Names = new List<string>();

        public Introducer()
        {
        }

        public Introducer(int pClientID, BaseAbstraction pBaseAbstraction, int pLanguageID)
        {
            try
            {
                Names.Clear();
                EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
                AuthenticationAbstraction authenticationAbstraction = new AuthenticationAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
                CoreAbstraction coreAbstraction = new CoreAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
                PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);

                PartyRelationship[] relationships = coreAbstraction.ListPartyRelationshipsTo(pClientID, pLanguageID);
                foreach (PartyRelationship relationship in relationships)
                {
                    if (relationship.ErrorsDetected == false)
                    {
                        relationship.FromPartyRole = coreAbstraction.GetPartyRole((int)relationship.FromPartyRole.PartyRoleID, pLanguageID);
                        if (relationship.FromPartyRole.ErrorsDetected == false)
                        {
                            if (relationship.FromPartyRole.PartyRoleType.Detail.Alias.CompareTo("PR_INTRODUCER") == 0 || relationship.FromPartyRole.PartyRoleType.Detail.Alias.CompareTo("PR_INTRODUCER_SUPERVISOR") == 0)
                            {
                                // this is the client's introducer
                                Person introducer = peopleAndPlacesAbstraction.GetPerson((int)relationship.FromPartyID, pLanguageID);
                                if (introducer.ErrorsDetected == false && introducer.PartyID.HasValue)
                                {
                                    ID = introducer.PartyID;
                                    introducer.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)introducer.PartyID, pLanguageID);
                                    if (introducer.PersonNames != null && introducer.PersonNames.Length > 0)
                                    {
                                        if (introducer.PersonNames[0].ErrorsDetected == false && introducer.PersonNames[0].PersonNameID.HasValue)
                                        {
                                            Name = encryptorDecryptor.Decrypt(introducer.PersonNames[0].FullName);
                                            Names.Add(Name);

                                            UserAccount introducerUserAccount = authenticationAbstraction.GetUserAccount((int)introducer.PartyID, pLanguageID);
                                            if (introducerUserAccount.ErrorsDetected == false && introducerUserAccount.PartyID.HasValue)
                                                ContactEmail = encryptorDecryptor.Decrypt(introducerUserAccount.AccountName);

                                            PartyContactMechanism[] contactMechanisms = coreAbstraction.ListPartyContactMechanisms((int)introducer.PartyID);
                                            if (contactMechanisms != null && contactMechanisms.Length > 0)
                                            {
                                                foreach (PartyContactMechanism contactMechanism in contactMechanisms)
                                                {
                                                    if (contactMechanism.ErrorsDetected == false)
                                                    {
                                                        contactMechanism.ContactMechanism.ContactMechanismType = peopleAndPlacesAbstraction.GetContactMechanismType((int)contactMechanism.ContactMechanism.ContactMechanismType.ContactMechanismTypeID, pLanguageID);
                                                        if (contactMechanism.ContactMechanism.ContactMechanismType.Detail.Alias.StartsWith("TELECOM_"))
                                                        {
                                                            TelecommsNumber workNumber = peopleAndPlacesAbstraction.GetTelecommsNumber((int)contactMechanism.ContactMechanism.ContactMechanismID, pLanguageID);
                                                            ContactTelephone = encryptorDecryptor.Decrypt(workNumber.ContactNumber);
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.ErrorsDetected = true;
                                                        this.ErrorDetails.Add(contactMechanism.ErrorDetails[0]);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else if (introducer.PersonNames[0].ErrorsDetected)
                                        {
                                            this.ErrorsDetected = true;
                                            this.ErrorDetails.Add(introducer.PersonNames[0].ErrorDetails[0]);
                                        }
                                    }
                                    // now lets get the introducers organisation
                                    PartyRelationshipType employerEmployeeRelationshipType = coreAbstraction.GetPartyRelationshipTypeByAlias("EMPLOYER_EMPLOYEE", pLanguageID);
                                    PartyRelationship[] introducerRelationships = coreAbstraction.ListPartyRelationshipsTo((int)introducer.PartyID, pLanguageID);
                                    if (introducerRelationships.Length > 0)
                                    {
                                        if (introducerRelationships[0].ErrorsDetected == false)
                                        {
                                            Organisation introducerOrganisation = coreAbstraction.GetOrganisation((int)introducerRelationships[0].FromPartyID,pLanguageID);
                                            if (introducerOrganisation.ErrorsDetected == false && introducerOrganisation.PartyID.HasValue)
                                            {
                                                Organisation = introducerOrganisation.Name;
                                                OrganisationID = introducerOrganisation.PartyID;
                                            }
                                        }
                                    }
                                }
                                else if (introducer.ErrorsDetected)
                                {
                                    this.ErrorsDetected = true;
                                    this.ErrorDetails.Add(introducer.ErrorDetails[0]);
                                }
                                else
                                {
                                    Organisation introducerOrganisation = coreAbstraction.GetOrganisation((int)relationship.FromPartyID, pLanguageID);
                                    if (introducerOrganisation.ErrorsDetected == false && introducerOrganisation.PartyID.HasValue)
                                    {
                                        Organisation = introducerOrganisation.Name;
                                        OrganisationID = introducerOrganisation.PartyID;
                                        Name = null;
                                        ID = introducerOrganisation.PartyID;
                                    }
                                }
                                break;
                            }
                        }
                        else
                        {
                            this.ErrorsDetected = true;
                            this.ErrorDetails.Add(relationship.FromPartyRole.ErrorDetails[0]);
                            break;
                        }
                    }
                    else
                    {
                        this.ErrorsDetected = true;
                        this.ErrorDetails.Add(relationship.ErrorDetails[0]);
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                this.ErrorsDetected = true;
                this.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
                this.StackTrace = exc.StackTrace;
            }
        }

        public void GetInstitutionForIntroducer(int pIntroducerID, BaseAbstraction pBaseAbstraction, int pLanguageID)
        {
            try
            {
                EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
                AuthenticationAbstraction authenticationAbstraction = new AuthenticationAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
                CoreAbstraction coreAbstraction = new CoreAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
                PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);

                Person introducer = peopleAndPlacesAbstraction.GetPerson((int)pIntroducerID, pLanguageID);
                if (introducer.ErrorsDetected == false && introducer.PartyID.HasValue)
                {
                    ID = introducer.PartyID;
                    introducer.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)introducer.PartyID, pLanguageID);
                    if (introducer.PersonNames != null && introducer.PersonNames.Length > 0)
                    {
                        if (introducer.PersonNames[0].ErrorsDetected == false && introducer.PersonNames[0].PersonNameID.HasValue)
                        {
                            Name = encryptorDecryptor.Decrypt(introducer.PersonNames[0].FullName);

                            UserAccount introducerUserAccount = authenticationAbstraction.GetUserAccount((int)introducer.PartyID, pLanguageID);
                            if (introducerUserAccount.ErrorsDetected == false && introducerUserAccount.PartyID.HasValue)
                                ContactEmail = encryptorDecryptor.Decrypt(introducerUserAccount.AccountName);

                            PartyContactMechanism[] contactMechanisms = coreAbstraction.ListPartyContactMechanisms((int)introducer.PartyID);
                            if (contactMechanisms != null && contactMechanisms.Length > 0)
                            {
                                foreach (PartyContactMechanism contactMechanism in contactMechanisms)
                                {
                                    if (contactMechanism.ErrorsDetected == false)
                                    {
                                        contactMechanism.ContactMechanism.ContactMechanismType = peopleAndPlacesAbstraction.GetContactMechanismType((int)contactMechanism.ContactMechanism.ContactMechanismType.ContactMechanismTypeID, pLanguageID);
                                        if (contactMechanism.ContactMechanism.ContactMechanismType.Detail.Alias.StartsWith("TELECOM_"))
                                        {
                                            TelecommsNumber workNumber = peopleAndPlacesAbstraction.GetTelecommsNumber((int)contactMechanism.ContactMechanism.ContactMechanismID, pLanguageID);
                                            ContactTelephone = encryptorDecryptor.Decrypt(workNumber.ContactNumber);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        this.ErrorsDetected = true;
                                        this.ErrorDetails.Add(contactMechanism.ErrorDetails[0]);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (introducer.PersonNames[0].ErrorsDetected)
                        {
                            this.ErrorsDetected = true;
                            this.ErrorDetails.Add(introducer.PersonNames[0].ErrorDetails[0]);
                        }
                    }
                    // now lets get the introducers organisation
                    PartyRelationshipType employerEmployeeRelationshipType = coreAbstraction.GetPartyRelationshipTypeByAlias("EMPLOYER_EMPLOYEE", pLanguageID);
                    PartyRelationship[] introducerRelationships = coreAbstraction.ListPartyRelationshipsTo((int)introducer.PartyID, pLanguageID);
                    if (introducerRelationships.Length > 0)
                    {
                        if (introducerRelationships[0].ErrorsDetected == false)
                        {
                            Organisation introducerOrganisation = coreAbstraction.GetOrganisation((int)introducerRelationships[0].FromPartyID, pLanguageID);
                            if (introducerOrganisation.ErrorsDetected == false && introducerOrganisation.PartyID.HasValue)
                            {
                                OrganisationID = introducerOrganisation.PartyID;
                                Organisation = introducerOrganisation.Name;
                            }
                        }
                    }
                }
                else if (introducer.ErrorsDetected)
                {
                    this.ErrorsDetected = true;
                    this.ErrorDetails.Add(introducer.ErrorDetails[0]);
                }
            }
            catch (Exception exc)
            {
                this.ErrorsDetected = true;
                this.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
                this.StackTrace = exc.StackTrace;
            }
        }
    }
}