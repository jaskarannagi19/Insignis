using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Encryption;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Core;
using Octavo.Gate.Nabu.Entities.PeopleAndPlaces;
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.Helper.UI.Controls
{
    public class IntroducerDialog
    {
        public List<Literal> animatedDialogScripts = new List<Literal>();
        public List<HtmlGenericControl> animatedDialogBodies = new List<HtmlGenericControl>();

        private string selectedClientIntroducerIDs = "";
        private string selectedClientIntroducer = "";

        public void Initialise(int pClientID, CoreAbstraction pCoreAbstraction, int pLanguageID)
        {
            EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
            PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(pCoreAbstraction.ConnectionString, pCoreAbstraction.DBType, pCoreAbstraction.ErrorLogFile);

            string scriptBody = "<script type=\"text/javascript\">";
            scriptBody += "var _introducerDialogData = [";

            // create a default one - i.e. not selected
            scriptBody += "new IntroducerOrganisation(-1,'No Organisation', [new Introducer(-1,'No Client Introducer',false)])";

            BaseString[] myClientIntroducers = pCoreAbstraction.CustomQuery("SELECT pa.PartyID FROM [SchCore].[Party] pa INNER JOIN [SchCore].[PartyRole] pr ON pr.PartyID = pa.PartyID WHERE pr.PartyRoleID IN (SELECT pre.FromPartyRoleID FROM [SchFinancial].[Client] c INNER JOIN [SchCore].[Party] p ON p.PartyID = c.ClientID INNER JOIN [SchCore].[PartyRole] pro ON pro.PartyID = p.PartyID INNER JOIN [SchCore].[PartyRelationship] pre ON pre.ToPartyRoleID = pro.PartyRoleID INNER JOIN [SchCore].[PartyRelationshipType] prt ON prt.PartyRelationshipTypeID = pre.PartyRelationshipTypeID INNER JOIN [SchGlobalisation].[Translation] t on t.TranslationID = prt.TranslationID WHERE c.ClientID = " + pClientID + " AND t.Alias = 'CLIENT_INTRODUCER')");
            PartyRelationshipType employerEmployeeRelationshipType = pCoreAbstraction.GetPartyRelationshipTypeByAlias("EMPLOYER_EMPLOYEE", pLanguageID);
            Organisation[] introducerOrganisations = pCoreAbstraction.ListOrganisationsByPartyTypeAlias("INTRODUCER_ORGANISATION", pLanguageID);
            foreach (Organisation introducerOrganisation in introducerOrganisations)
            {
                if (introducerOrganisation.ErrorsDetected == false)
                {
                    scriptBody += ", ";
                    scriptBody += "new IntroducerOrganisation(" + introducerOrganisation.PartyID + ",'" + introducerOrganisation.Name.Replace("'", "`") + "', [";

                    bool hasNoSpecificIntroducer = false;
                    int introducerCounter = 0;
                    PartyRelationship[] introducerRelationships = pCoreAbstraction.ListPartyRelationshipsFrom((int)introducerOrganisation.PartyID, pLanguageID);
                    if (introducerRelationships != null && introducerRelationships.Length > 0)
                    {
                        foreach (PartyRelationship introducerRelationship in introducerRelationships)
                        {
                            if (introducerRelationship.ErrorsDetected == false)
                            {
                                if (introducerRelationship.PartyRelationshipType.PartyRelationshipTypeID == employerEmployeeRelationshipType.PartyRelationshipTypeID)
                                {
                                    Person introducerPerson = peopleAndPlacesAbstraction.GetPerson(introducerRelationship.ToPartyID, pLanguageID);
                                    if (introducerPerson != null && introducerPerson.ErrorsDetected == false && introducerPerson.PartyID.HasValue)
                                    {
                                        introducerPerson.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)introducerPerson.PartyID, pLanguageID);
                                        if (introducerPerson.PersonNames != null && introducerPerson.PersonNames.Length > 0)
                                        {
                                            BaseString[] clientIntroducers = pCoreAbstraction.CustomQuery("SELECT p.PartyID, pn.FullName FROM [SchCore].[Party] p INNER JOIN [SchCore].[PartyRole] pr ON pr.PartyID = p.PartyID INNER JOIN [SchCore].[PartyRoleType] prt ON prt.PartyRoleTypeID = pr.PartyRoleTypeID INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = prt.TranslationID INNER JOIN [SchPeopleAndPlaces].[Person] pe ON pe.PersonID = p.PartyID INNER JOIN [SchPeopleAndPlaces].[PersonName] pn ON pn.PersonID = pe.PersonID WHERE t.Alias IN ('PR_INTRODUCER','PR_INTRODUCER_SUPERVISOR')");

                                            if(introducerCounter > 0)
                                            scriptBody += ", ";
                                            bool isSelected = false;

                                            if (myClientIntroducers.Length > 0)
                                            {
                                                try
                                                {
                                                    foreach (BaseString myClientIntroducer in myClientIntroducers)
                                                    {
                                                        if (myClientIntroducer.ErrorsDetected == false)
                                                        {
                                                            if (Convert.ToInt32(myClientIntroducer.Value) == (int)introducerPerson.PartyID)
                                                            {
                                                                bool found = false;
                                                                if (selectedClientIntroducerIDs.Trim().Length > 0)
                                                                {
                                                                    if (selectedClientIntroducerIDs.Contains(","))
                                                                    {
                                                                        string commaSeparator = ",";
                                                                        string[] IDs = selectedClientIntroducerIDs.Split(commaSeparator.ToCharArray());
                                                                        foreach (string ID in IDs)
                                                                        {
                                                                            if (ID.CompareTo(introducerPerson.PartyID.Value.ToString()) == 0)
                                                                            {
                                                                                found = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (selectedClientIntroducerIDs.CompareTo(introducerPerson.PartyID.Value.ToString()) == 0)
                                                                            found = true;
                                                                    }
                                                                }

                                                                if (found == false)
                                                                {
                                                                    if (selectedClientIntroducer.Trim().Length > 0)
                                                                    {
                                                                        selectedClientIntroducer += ", ";
                                                                        selectedClientIntroducerIDs += ",";
                                                                    }
                                                                    selectedClientIntroducer += encryptorDecryptor.Decrypt(introducerPerson.PersonNames[0].FullName);
                                                                    selectedClientIntroducerIDs += introducerPerson.PartyID.Value;
                                                                    isSelected = true;
                                                                }
                                                            }
                                                            else if (Convert.ToInt32(myClientIntroducer.Value) == (int)introducerOrganisation.PartyID)
                                                            {
                                                                bool found = false;
                                                                if (selectedClientIntroducerIDs.Trim().Length > 0)
                                                                {
                                                                    if (selectedClientIntroducerIDs.Contains(","))
                                                                    {
                                                                        string commaSeparator = ",";
                                                                        string[] IDs = selectedClientIntroducerIDs.Split(commaSeparator.ToCharArray());
                                                                        foreach (string ID in IDs)
                                                                        {
                                                                            if (ID.CompareTo(introducerOrganisation.PartyID.Value.ToString()) == 0)
                                                                            {
                                                                                found = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (selectedClientIntroducerIDs.CompareTo(introducerOrganisation.PartyID.Value.ToString()) == 0)
                                                                            found = true;
                                                                    }
                                                                }
                                                                if (found == false)
                                                                {
                                                                    if (selectedClientIntroducer.Trim().Length > 0)
                                                                    {
                                                                        selectedClientIntroducer += ", ";
                                                                        selectedClientIntroducerIDs += ",";
                                                                    }
                                                                    selectedClientIntroducer += introducerOrganisation.Name;
                                                                    selectedClientIntroducerIDs += introducerOrganisation.PartyID.Value;
                                                                    hasNoSpecificIntroducer = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            scriptBody += "new Introducer(" + introducerPerson.PartyID + ",'" + encryptorDecryptor.Decrypt(introducerPerson.PersonNames[0].FullName).Replace("'", "`") + "'," + ((isSelected == true) ? "true" : "false") + ")";
                                            introducerCounter++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (introducerCounter > 0)
                        scriptBody += ", ";
                    scriptBody += "new Introducer(" + introducerOrganisation.PartyID + ",'No specific introducer'," + ((hasNoSpecificIntroducer == true) ? "true" : "false") + ")";
                    scriptBody += "])";
                }
            }
            scriptBody += "];";
            scriptBody += "</script>";

            Literal script = new Literal();
            script.Text = scriptBody;
            animatedDialogScripts.Add(script);

            if(selectedClientIntroducer.Trim().Length==0)
                selectedClientIntroducer = "No Client Introducer";
        }

        public HtmlInputHidden GetInputControl()
        {
            HtmlInputHidden hiddenClientIntroducerID = new HtmlInputHidden();
            hiddenClientIntroducerID.ID = "_clientIntroducer";
            hiddenClientIntroducerID.Value = selectedClientIntroducerIDs;
            return hiddenClientIntroducerID;
        }

        public HtmlGenericControl GetViewControl()
        {
            HtmlGenericControl pClientIntroducerView = new HtmlGenericControl("span");
            pClientIntroducerView.ID = "_clientIntroducerView";
            pClientIntroducerView.InnerText = selectedClientIntroducer.ToString();
            return pClientIntroducerView;
        }

        public HtmlAnchor GetOpenDialogControl()
        {
            HtmlAnchor buttonOpenDialog = new HtmlAnchor();
            buttonOpenDialog.HRef = "#";
            buttonOpenDialog.Attributes.Add("class", "btn");
            buttonOpenDialog.Attributes.Add("onclick", "IntroducerDialogRefresh();" + Helper.UI.Dialog.OpenDialog("ContentBody_" + "_selectIntroducerDialog"));
            buttonOpenDialog.Style.Add("margin-left", "10px");
            buttonOpenDialog.InnerHtml = "<i style=\"margin:0px; padding:0px;\" class=\"fa fa-edit\"></i>";
            return buttonOpenDialog;
        }

        public bool HasIntroducerDefined()
        {
            if (selectedClientIntroducerIDs.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public HtmlAnchor GetRemoveIntroducerControl()
        {
            HtmlAnchor buttonRemoveIntroducer = new HtmlAnchor();
            buttonRemoveIntroducer.Attributes.Add("class", "btn");
            buttonRemoveIntroducer.Attributes.Add("onclick", "jConfirmNavigation('Are you sure you want to remove the introducer(s) for the selected client?','" + "RemoveIntroducer.aspx" + "');");
            buttonRemoveIntroducer.Style.Add("margin-left", "5px");
            buttonRemoveIntroducer.InnerHtml = "<i style=\"margin:0px; padding:0px;\" class=\"fa fa-trash\"></i>";
            return buttonRemoveIntroducer;
        }

        public void Generate()
        {
            animatedDialogScripts.Add(Helper.UI.Dialog.DrawAnimatedDialogScript("ContentBody_" + "_selectIntroducerDialog", false, "blind", "explode", true, 670, 525));

            HtmlGenericControl selectIntroducerDialogContainer = new HtmlGenericControl("div");
            selectIntroducerDialogContainer.ID = "_selectIntroducerDialog";
            selectIntroducerDialogContainer.Attributes.Add("title", "Select Introducer(s)");

            Table tableIntroducerDetails = new Table();

            TableRow rowIntroducerDetails = new TableRow();

            TableCell cellLeftColumn = new TableCell();
            cellLeftColumn.Style.Add("text-align", "left");
            cellLeftColumn.Controls.Add(Helper.UI.Theme.DrawHeadLine2("Organisation"));
            cellLeftColumn.Controls.Add(Helper.UI.Theme.DrawLiteral("<div id=\"_introducerDialogOrganisationListView\" style=\"border:1px solid black; overflow: auto; padding: 0px; margin: 0px; width:295px; height:295px;\"></div>"));
            rowIntroducerDetails.Cells.Add(cellLeftColumn);

            TableCell cellRightColumn = new TableCell();
            cellRightColumn.Style.Add("text-align", "left");
            cellRightColumn.Controls.Add(Helper.UI.Theme.DrawHeadLine2("Introducer"));
            cellRightColumn.Controls.Add(Helper.UI.Theme.DrawLiteral("<div id=\"_introducerDialogIntroducersListView\" style=\"border:1px solid black; overflow: auto; padding: 0px; margin: 0px; width:295px; height:295px;\"></div>"));
            rowIntroducerDetails.Cells.Add(cellRightColumn);

            tableIntroducerDetails.Rows.Add(rowIntroducerDetails);
            selectIntroducerDialogContainer.Controls.Add(tableIntroducerDetails);

            HtmlAnchor buttonSubmitRequest = new HtmlAnchor();
            buttonSubmitRequest.Attributes.Add("class", "btn");
            buttonSubmitRequest.Style.Add("float", "right");
            buttonSubmitRequest.Style.Add("color", "white");
            buttonSubmitRequest.Style.Add("margin-right", "5px");
            buttonSubmitRequest.Controls.Add(Helper.UI.Theme.DrawLiteral("Select"));
            buttonSubmitRequest.Attributes.Add("onclick", "IntroducerDialogChoose();");
            selectIntroducerDialogContainer.Controls.Add(buttonSubmitRequest);

            animatedDialogBodies.Add(selectIntroducerDialogContainer);
        }
    }
}
