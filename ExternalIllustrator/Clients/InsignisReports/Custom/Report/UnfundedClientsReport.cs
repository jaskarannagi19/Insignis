using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Encryption;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Core;
using Octavo.Gate.Nabu.Entities.Financial;
using Octavo.Gate.Nabu.Entities.PeopleAndPlaces;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Custom.Report
{
    public class UnfundedClientsReport
    {
        private EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();

        public UnfundedClientsReport()
        {
        }

        public GenericReport Generate(FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            CoreAbstraction coreAbstraction = new CoreAbstraction(pFinancialAbstraction.ConnectionString, pFinancialAbstraction.DBType, pFinancialAbstraction.ErrorLogFile);
            PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(pFinancialAbstraction.ConnectionString, pFinancialAbstraction.DBType, pFinancialAbstraction.ErrorLogFile);

            GenericReport genericReport = new GenericReport("Unfunded Clients");
            genericReport.Elements.Add(new SubHeading("Client accounts opened but unfunded"));
            try
            {
                PartyRelationshipType prtSpouseOrPartner = coreAbstraction.GetPartyRelationshipTypeByAlias("SPOUSEORPARTNER", pLanguageID);

                AccountType[] accountTypes = pFinancialAbstraction.ListAccountTypes(pLanguageID);
                ClientCategory[] clientCategories = pFinancialAbstraction.ListClientCategories(pLanguageID);
                ClientStatus[] clientStatuses = pFinancialAbstraction.ListClientStatuses(pLanguageID);
                Currency[] currencies = pFinancialAbstraction.ListCurrencies(pLanguageID);

                List<int> BarclaysBranchIDs = ListBarclaysBranchIDs(pFinancialAbstraction, "SELECT bra.BranchID FROM [SchFinancial].[Branch] bra INNER JOIN[SchCore].[PartyRole] prot ON prot.PartyID = bra.BranchID INNER JOIN[SchCore].[PartyRelationship] pr ON pr.ToPartyRoleID = prot.PartyRoleID INNER JOIN[SchCore].[PartyRole] prof ON prof.PartyRoleID = pr.FromPartyRoleID INNER JOIN[SchCore].[Organisation] org ON org.OrganisationID = prof.PartyID WHERE org.Name LIKE 'Barclays%'");

                string SQL = "SELECT acc.AccountID, acc.ClientID, acc.AccountTypeID, acc.CurrencyID, acc.BranchID, acc.CreatedDate";
                SQL += " FROM [SchFinancial].[Account] acc";
                SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                SQL += " WHERE acc.Balance IS NULL";
                SQL += " AND actt.Alias LIKE '%HUBACCOUNT'";
                SQL += " ORDER BY actt.Alias, acc.ClientID";
                AccountType currentAccountType = null;
                BaseString[] recordSet = pFinancialAbstraction.CustomQuery(SQL);
                Table table = null;
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields();

                        bool isAccountHeldAtBarclaysBranch = false;

                        foreach (int barclaysBranchID in BarclaysBranchIDs)
                        {
                            try
                            {
                                if (Convert.ToInt32(fields[4]) == barclaysBranchID)
                                {
                                    isAccountHeldAtBarclaysBranch = true;
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (isAccountHeldAtBarclaysBranch)
                        {
                            if (currentAccountType == null || currentAccountType.AccountTypeID != Convert.ToInt32(fields[2]))
                            {
                                currentAccountType = pFinancialAbstraction.GetAccountType(Convert.ToInt32(fields[2]), pLanguageID);
                                if (currentAccountType.ErrorsDetected == false && currentAccountType.AccountTypeID.HasValue)
                                {
                                    if (table != null)
                                    {
                                        genericReport.Elements.Add(table);
                                        table = null;
                                    }
                                    genericReport.Elements.Add(new LineBreak());
                                    genericReport.Elements.Add(new SubHeading(currentAccountType.Detail.Name));
                                    table = new Table();
                                    Row headerRow = new Row();
                                    headerRow.Cells.Add(new TextCell("Reference"));
                                    headerRow.Cells.Add(new TextCell("Category"));
                                    headerRow.Cells.Add(new TextCell("Currency"));
                                    headerRow.Cells.Add(new TextCell("Name"));
                                    headerRow.Cells.Add(new TextCell("Sales Person"));
                                    headerRow.Cells.Add(new TextCell("Introducer"));
                                    headerRow.Cells.Add(new TextCell("Status"));
                                    headerRow.Cells.Add(new TextCell("Joined"));
                                    table.Rows.Add(headerRow);
                                }
                            }
                            Client client = pFinancialAbstraction.GetClient(Convert.ToInt32(fields[1]), pLanguageID);
                            if (client.ErrorsDetected == false && client.PartyID.HasValue)
                            {
                                client.SetCategory(clientCategories);
                                client.SetStatus(clientStatuses);

                                if (client.Status.Detail.Alias.CompareTo("ACTIVE") == 0)
                                {
                                    Row bodyRow = new Row();
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));

                                    // reference
                                    if (client.ClientReference != null && client.ClientReference.Trim().Length > 0)
                                        bodyRow.Cells[0].Value = client.ClientReference;
                                    else
                                        bodyRow.Cells[0].Value = "ID: [" + client.PartyID + "]";

                                    // category
                                    if (client.Category != null)
                                    {
                                        if (client.Category.ErrorsDetected == false)
                                        {
                                            if (client.Category.ClientCategoryID.HasValue && client.Category.Detail != null && client.Category.Detail.Name != null && client.Category.Detail.Name.Trim().Length > 0)
                                                bodyRow.Cells[1].Value = client.Category.Detail.Name;
                                            else
                                                bodyRow.Cells[1].Value = "[unspecified]";
                                        }
                                        else
                                            bodyRow.Cells[1].Value = "[error]";
                                    }
                                    else
                                        bodyRow.Cells[1].Value = "[unspecified]";

                                    // currency
                                    foreach (Currency currency in currencies)
                                    {
                                        if (currency.ErrorsDetected == false)
                                        {
                                            if (currency.CurrencyID == Convert.ToInt32(fields[3]))
                                                bodyRow.Cells[2].Value = currency.CurrencyCode;
                                        }
                                    }

                                    // name
                                    if (client.Category != null && client.Category.ErrorsDetected == false && client.Category.ClientCategoryID.HasValue)
                                    {
                                        if (client.Category.IsIndividual)
                                        {
                                            Person personClient = peopleAndPlacesAbstraction.GetPerson((int)client.PartyID, pLanguageID);
                                            if (personClient.ErrorsDetected == false && personClient.PartyID.HasValue)
                                            {
                                                personClient.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)personClient.PartyID, pLanguageID);
                                                if (personClient.PersonNames.Length > 0)
                                                {
                                                    if (personClient.PersonNames[0].ErrorsDetected == false)
                                                    {
                                                        bodyRow.Cells[3].Value = encryptorDecryptor.Decrypt(personClient.PersonNames[0].FullName);

                                                        if (currentAccountType.Detail.Alias.Contains("JOINT"))
                                                        {
                                                            // load the spouse
                                                            string innerSQL = "SELECT pre.PartyRelationshipID, pre.FromPartyRoleID, pre.ToPartyRoleID, pre.FromDate, pro.PartyRoleID, pro.PartyID";
                                                            innerSQL += " FROM [SchCore].[PartyRelationship] pre";
                                                            innerSQL += " INNER JOIN[SchCore].[PartyRole] pro ON(pro.PartyRoleID = pre.FromPartyRoleID OR pro.PartyRoleID = pre.ToPartyRoleID)";
                                                            innerSQL += " WHERE pro.PartyID = " + client.PartyID;
                                                            innerSQL += " AND pre.PartyRelationshipTypeID = " + prtSpouseOrPartner.PartyRelationshipTypeID;

                                                            BaseString[] relationshipRecordSet = coreAbstraction.CustomQuery(innerSQL);
                                                            if (relationshipRecordSet.Length > 0)
                                                            {
                                                                if (relationshipRecordSet[0].ErrorsDetected == false)
                                                                {
                                                                    Person spousePerson = null;
                                                                    string[] relationshipFields = relationshipRecordSet[0].GetFields();
                                                                    if (Convert.ToInt32(relationshipFields[1]) == Convert.ToInt32(relationshipFields[4]))
                                                                        spousePerson = peopleAndPlacesAbstraction.GetPerson(Convert.ToInt32(coreAbstraction.CustomQuery("SELECT PartyID FROM [SchCore].[PartyRole] WHERE PartyRoleID = " + Convert.ToInt32(relationshipFields[2]) + ";")[0].Value), pLanguageID);
                                                                    else if (Convert.ToInt32(relationshipFields[2]) == Convert.ToInt32(relationshipFields[4]))
                                                                        spousePerson = peopleAndPlacesAbstraction.GetPerson(Convert.ToInt32(relationshipFields[5]), pLanguageID);

                                                                    if (spousePerson != null && spousePerson.ErrorsDetected == false && spousePerson.PartyID.HasValue)
                                                                    {
                                                                        spousePerson.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)spousePerson.PartyID, pLanguageID);
                                                                        if (spousePerson.PersonNames.Length > 0)
                                                                        {
                                                                            if (spousePerson.PersonNames[0].ErrorsDetected == false)
                                                                            {
                                                                                bodyRow.Cells[3].Value += " and ";
                                                                                bodyRow.Cells[3].Value += encryptorDecryptor.Decrypt(spousePerson.PersonNames[0].FullName);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                        bodyRow.Cells[3].Value = "[error]";
                                                }
                                                else
                                                    bodyRow.Cells[3].Value = "[unspecified]";
                                            }
                                            else if (personClient.PartyID.HasValue == false)
                                                bodyRow.Cells[3].Value = "[unspecified]";
                                            else
                                                bodyRow.Cells[3].Value = "[error]";
                                        }
                                        else
                                        {
                                            Organisation organisationClient = coreAbstraction.GetOrganisation((int)client.PartyID, (int)pLanguageID);
                                            if (organisationClient.ErrorsDetected == false)
                                            {
                                                if (organisationClient.PartyID.HasValue)
                                                    bodyRow.Cells[3].Value = organisationClient.Name;
                                                else
                                                    bodyRow.Cells[3].Value = "[unspecified]";
                                            }
                                            else
                                                bodyRow.Cells[3].Value = "[error]";
                                        }
                                    }
                                    // sales person
                                    bodyRow.Cells[4].Value = GetSalesPerson(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // introducer
                                    bodyRow.Cells[5].Value = GetIntroducer(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // status
                                    bodyRow.Cells[6].Value = client.Status.Detail.Name;

                                    // joined
                                    bodyRow.Cells[7].Value = fields[5].Substring(1,11);
                                    table.Rows.Add(bodyRow);
                                }
                            }
                        }
                    }
                }
                if (table.Rows.Count > 1)
                    genericReport.Elements.Add(table);

                // now generate the statistics
                table = new Table();
                Row statisticsHeaderRow = new Row();
                statisticsHeaderRow.Cells.Add(new TextCell("Client Category"));
                statisticsHeaderRow.Cells.Add(new RightAlignedCell("Active"));
                statisticsHeaderRow.Cells.Add(new RightAlignedCell("Inactive"));
                statisticsHeaderRow.Cells.Add(new RightAlignedCell("Deactivated"));
                statisticsHeaderRow.Cells.Add(new RightAlignedCell("Total"));
                table.Rows.Add(statisticsHeaderRow);
                SQL = "SELECT Count(*), cct.Alias, cst.Alias";
                SQL += " FROM [SchFinancial].[Client] cl";
                SQL += " INNER JOIN [SchFinancial].[ClientCategory] cc ON cc.ClientCategoryID = cl.ClientCategoryID";
                SQL += " INNER JOIN [SchFinancial].[ClientStatus] cs ON cs.ClientStatusID = cl.ClientStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] cct ON cct.TranslationID = cc.TranslationID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] cst ON cst.TranslationID = cs.TranslationID";
                SQL += " GROUP BY cct.Alias, cst.Alias";
                SQL += " ORDER BY cct.Alias, cst.Alias";
                string currentCategoryAlias = "";
                int rowCount = 0;
                int totalActiveCount = 0;
                int totalInactiveCount = 0;
                int totalDeactivatedCount = 0;
                Row statisticsRow = null;
                BaseString[] statisticsRecordSet = pFinancialAbstraction.CustomQuery(SQL);
                foreach (BaseString statisticsRecord in statisticsRecordSet)
                {
                    if (statisticsRecord.ErrorsDetected == false)
                    {
                        string[] fields = statisticsRecord.GetFields();
                        if (currentCategoryAlias.Length == 0)
                            currentCategoryAlias = fields[1].Replace("'", "");
                        else if (currentCategoryAlias.CompareTo(fields[1].Replace("'", "")) != 0)
                        {
                            table.Rows.Add(statisticsRow);
                            statisticsRow = null;
                            rowCount = 0;
                            currentCategoryAlias = fields[1].Replace("'", "");
                        }

                        if (statisticsRow == null)
                        {
                            statisticsRow = new Row();
                            statisticsRow.Cells.Add(new TextCell(currentCategoryAlias));
                            foreach (ClientCategory clientCategory in clientCategories)
                            {
                                if (clientCategory.ErrorsDetected == false)
                                {
                                    if (clientCategory.Detail.Alias.CompareTo(currentCategoryAlias) == 0)
                                    {
                                        statisticsRow.Cells[0].Value = clientCategory.Detail.Name;
                                        break;
                                    }
                                }
                            }
                            statisticsRow.Cells.Add(new RightAlignedCell(""));
                            statisticsRow.Cells.Add(new RightAlignedCell(""));
                            statisticsRow.Cells.Add(new RightAlignedCell(""));
                            statisticsRow.Cells.Add(new RightAlignedCell(""));
                        }
                        int count = Convert.ToInt32(fields[0]);
                        if (fields[2].Replace("'", "").CompareTo("ACTIVE") == 0)
                        {
                            statisticsRow.Cells[1].Value = count.ToString();
                            totalActiveCount += count;
                        }
                        else if (fields[2].Replace("'", "").CompareTo("INACTIVE") == 0)
                        {
                            statisticsRow.Cells[2].Value = count.ToString();
                            totalInactiveCount += count;
                        }
                        if (fields[2].Replace("'", "").CompareTo("DEACTIVATED") == 0)
                        {
                            statisticsRow.Cells[3].Value = count.ToString();
                            totalDeactivatedCount += count;
                        }
                        rowCount += count;
                        statisticsRow.Cells[4].Value = rowCount.ToString();
                    }
                }
                if (statisticsRow != null)
                {
                    table.Rows.Add(statisticsRow);
                    statisticsRow = null;
                }
                statisticsRow = new Row();
                statisticsRow.Cells.Add(new TextCell(""));
                statisticsRow.Cells.Add(new RightAlignedCell(totalActiveCount.ToString()));
                statisticsRow.Cells.Add(new RightAlignedCell(totalInactiveCount.ToString()));
                statisticsRow.Cells.Add(new RightAlignedCell(totalDeactivatedCount.ToString()));
                statisticsRow.Cells.Add(new RightAlignedCell(Convert.ToString(totalActiveCount + totalInactiveCount + totalDeactivatedCount)));
                table.Rows.Add(statisticsRow);

                genericReport.Elements.Add(new LineBreak());
                genericReport.Elements.Add(new SubHeading("Client statistics"));
                genericReport.Elements.Add(table);
            }
            catch (Exception exc)
            {
                genericReport.Elements.Add(new Paragraph("Caught Error : " + exc.Message));
            }
            /*try
            {
                Table table = new Table();
                Row headerRow = new Row();
                headerRow.Cells.Add(new TextCell("Reference"));
                headerRow.Cells.Add(new TextCell("Category"));
                headerRow.Cells.Add(new TextCell("Name"));
                headerRow.Cells.Add(new TextCell("Sales Person"));
                headerRow.Cells.Add(new TextCell("Introducer"));
                headerRow.Cells.Add(new TextCell("Status"));
                table.Rows.Add(headerRow);

                foreach (Client client in clients)
                {
                    if (client.ErrorsDetected == false)
                    {
                        client.SetCategory(clientCategories);
                        client.SetStatus(clientStatuses);

                        if (client.Status.Detail.Alias.CompareTo("ACTIVE") == 0)
                        {
                            totalActiveClients++;
                            if (client.Category.Detail.Alias.StartsWith("DEMO_") == false)
                            {
                                if (client.Category.IsIndividual)
                                    totalActivePersonalClients++;
                                else
                                    totalActiveCorporateClients++;

                                if (IncludeInOutput(client, pFinancialAbstraction))
                                {
                                    Row bodyRow = new Row();
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));

                                    // reference
                                    if (client.ClientReference != null && client.ClientReference.Trim().Length > 0)
                                        bodyRow.Cells[0].Value = client.ClientReference;
                                    else
                                        bodyRow.Cells[0].Value = "ID: [" + client.PartyID + "]";

                                    // category
                                    if (client.Category != null)
                                    {
                                        if (client.Category.ErrorsDetected == false)
                                        {
                                            if (client.Category.ClientCategoryID.HasValue && client.Category.Detail != null && client.Category.Detail.Name != null && client.Category.Detail.Name.Trim().Length > 0)
                                                bodyRow.Cells[1].Value = client.Category.Detail.Name;
                                            else
                                                bodyRow.Cells[1].Value = "[unspecified]";
                                        }
                                        else
                                            bodyRow.Cells[1].Value = "[error]";
                                    }
                                    else
                                        bodyRow.Cells[1].Value = "[unspecified]";

                                    // name
                                    if (client.Category != null && client.Category.ErrorsDetected == false && client.Category.ClientCategoryID.HasValue)
                                    {
                                        if (client.Category.IsIndividual)
                                        {
                                            Person personClient = peopleAndPlacesAbstraction.GetPerson((int)client.PartyID, pLanguageID);
                                            if (personClient.ErrorsDetected == false && personClient.PartyID.HasValue)
                                            {
                                                personClient.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)personClient.PartyID, pLanguageID);
                                                if (personClient.PersonNames.Length > 0)
                                                {
                                                    if (personClient.ErrorsDetected == false)
                                                        bodyRow.Cells[2].Value = encryptorDecryptor.Decrypt(personClient.PersonNames[0].FullName);
                                                    else
                                                        bodyRow.Cells[2].Value = "[error]";
                                                }
                                                else
                                                    bodyRow.Cells[2].Value = "[unspecified]";
                                            }
                                            else if (personClient.PartyID.HasValue == false)
                                                bodyRow.Cells[2].Value = "[unspecified]";
                                            else
                                                bodyRow.Cells[2].Value = "[error]";
                                        }
                                        else
                                        {
                                            Organisation organisationClient = coreAbstraction.GetOrganisation((int)client.PartyID, (int)pLanguageID);
                                            if (organisationClient.ErrorsDetected == false)
                                            {
                                                if (organisationClient.PartyID.HasValue)
                                                    bodyRow.Cells[2].Value = organisationClient.Name;
                                                else
                                                    bodyRow.Cells[2].Value = "[unspecified]";
                                            }
                                            else
                                                bodyRow.Cells[2].Value = "[error]";
                                        }
                                    }
                                    // sales person
                                    bodyRow.Cells[3].Value = GetSalesPerson(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // introducer
                                    bodyRow.Cells[4].Value = GetIntroducer(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // status

                                    table.Rows.Add(bodyRow);
                                    totalReported++;
                                }
                            }
                            else
                                totalActiveDemoClients++;
                        }
                        else if (client.Status.Detail.Alias.CompareTo("INACTIVE") == 0)
                            totalInactiveClients++;
                        else if (client.Status.Detail.Alias.CompareTo("DEACTIVATED") == 0)
                            totalDeactivatedClients++;
                    }
                    else
                    {
                        genericReport.Elements.Add(new Paragraph("Unable to read client : " + client.ErrorDetails[0].ErrorMessage));
                        break;
                    }
                }
                genericReport.Elements.Add(table);

                genericReport.Elements.Add(new LineBreak());

                Table statisticTable = new Table();
                statisticTable.Rows.Add(MetricRow("Total Clients", totalClients));
                statisticTable.Rows.Add(MetricRow("Total Clients Reported", totalReported));
                statisticTable.Rows.Add(MetricRow("Total Active Demo Clients", totalActiveDemoClients));
                statisticTable.Rows.Add(MetricRow("Total Active Clients", totalActiveClients));
                statisticTable.Rows.Add(MetricRow("Total Inactive Clients", totalInactiveClients));
                statisticTable.Rows.Add(MetricRow("Total Deactivated Clients", totalDeactivatedClients));
                statisticTable.Rows.Add(MetricRow("Total Active Personal Clients", totalActivePersonalClients));
                statisticTable.Rows.Add(MetricRow("Total Active Organisation Clients", totalActiveCorporateClients));
                genericReport.Elements.Add(statisticTable);
            }
            catch (Exception exc)
            {
                genericReport.Elements.Add(new Paragraph("Caught Error : " + exc.Message));
            }*/
            return genericReport;
        }

        private List<int> ListBarclaysBranchIDs(FinancialAbstraction pFinancialAbstraction, string pSQL)
        {
            List<int> branchIDs = new List<int>();
            try
            {
                BaseString[] recordSet = pFinancialAbstraction.CustomQuery(pSQL);
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields();
                        branchIDs.Add(Convert.ToInt32(fields[0].Replace("'", "")));
                    }
                }
            }
            catch
            {
            }
            return branchIDs;
        }

        public GenericReport OldGenerate(FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            CoreAbstraction coreAbstraction = new CoreAbstraction(pFinancialAbstraction.ConnectionString, pFinancialAbstraction.DBType, pFinancialAbstraction.ErrorLogFile);
            PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(pFinancialAbstraction.ConnectionString, pFinancialAbstraction.DBType, pFinancialAbstraction.ErrorLogFile);

            GenericReport genericReport = new GenericReport("Unfunded Clients");
            genericReport.Elements.Add(new SubHeading("Client accounts opened but unfunded"));

            ClientCategory[] clientCategories = pFinancialAbstraction.ListClientCategories(pLanguageID);
            ClientStatus[] clientStatuses = pFinancialAbstraction.ListClientStatuses(pLanguageID);
            Client[] clients = pFinancialAbstraction.ListClients();

            int totalClients = clients.Length;
            int totalReported = 0;
            int totalActiveDemoClients = 0;

            int totalActiveClients = 0;
            int totalInactiveClients = 0;
            int totalDeactivatedClients = 0;

            int totalActivePersonalClients = 0;
            int totalActiveCorporateClients = 0;
            try
            {
                Table table = new Table();
                Row headerRow = new Row();
                headerRow.Cells.Add(new TextCell("Reference"));
                headerRow.Cells.Add(new TextCell("Category"));
                headerRow.Cells.Add(new TextCell("Name"));
                headerRow.Cells.Add(new TextCell("Sales Person"));
                headerRow.Cells.Add(new TextCell("Introducer"));
                headerRow.Cells.Add(new TextCell("Status"));
                table.Rows.Add(headerRow);

                foreach (Client client in clients)
                {
                    if (client.ErrorsDetected == false)
                    {
                        client.SetCategory(clientCategories);
                        client.SetStatus(clientStatuses);

                        if (client.Status.Detail.Alias.CompareTo("ACTIVE") == 0)
                        {
                            totalActiveClients++;
                            if (client.Category.Detail.Alias.StartsWith("DEMO_") == false)
                            {
                                if (client.Category.IsIndividual)
                                    totalActivePersonalClients++;
                                else
                                    totalActiveCorporateClients++;

                                if (IncludeInOutput(client, pFinancialAbstraction))
                                {
                                    Row bodyRow = new Row();
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));
                                    bodyRow.Cells.Add(new TextCell(""));

                                    // reference
                                    if (client.ClientReference != null && client.ClientReference.Trim().Length > 0)
                                        bodyRow.Cells[0].Value = client.ClientReference;
                                    else
                                        bodyRow.Cells[0].Value = "ID: [" + client.PartyID + "]";

                                    // category
                                    if (client.Category != null)
                                    {
                                        if (client.Category.ErrorsDetected == false)
                                        {
                                            if (client.Category.ClientCategoryID.HasValue && client.Category.Detail != null && client.Category.Detail.Name != null && client.Category.Detail.Name.Trim().Length > 0)
                                                bodyRow.Cells[1].Value = client.Category.Detail.Name;
                                            else
                                                bodyRow.Cells[1].Value = "[unspecified]";
                                        }
                                        else
                                            bodyRow.Cells[1].Value = "[error]";
                                    }
                                    else
                                        bodyRow.Cells[1].Value = "[unspecified]";

                                    // name
                                    if (client.Category != null && client.Category.ErrorsDetected == false && client.Category.ClientCategoryID.HasValue)
                                    {
                                        if (client.Category.IsIndividual)
                                        {
                                            Person personClient = peopleAndPlacesAbstraction.GetPerson((int)client.PartyID, pLanguageID);
                                            if (personClient.ErrorsDetected == false && personClient.PartyID.HasValue)
                                            {
                                                personClient.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)personClient.PartyID, pLanguageID);
                                                if (personClient.PersonNames.Length > 0)
                                                {
                                                    if (personClient.ErrorsDetected == false)
                                                        bodyRow.Cells[2].Value = encryptorDecryptor.Decrypt(personClient.PersonNames[0].FullName);
                                                    else
                                                        bodyRow.Cells[2].Value = "[error]";
                                                }
                                                else
                                                    bodyRow.Cells[2].Value = "[unspecified]";
                                            }
                                            else if (personClient.PartyID.HasValue == false)
                                                bodyRow.Cells[2].Value = "[unspecified]";
                                            else
                                                bodyRow.Cells[2].Value = "[error]";
                                        }
                                        else
                                        {
                                            Organisation organisationClient = coreAbstraction.GetOrganisation((int)client.PartyID, (int)pLanguageID);
                                            if (organisationClient.ErrorsDetected == false)
                                            {
                                                if (organisationClient.PartyID.HasValue)
                                                    bodyRow.Cells[2].Value = organisationClient.Name;
                                                else
                                                    bodyRow.Cells[2].Value = "[unspecified]";
                                            }
                                            else
                                                bodyRow.Cells[2].Value = "[error]";
                                        }
                                    }
                                    // sales person
                                    bodyRow.Cells[3].Value = GetSalesPerson(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // introducer
                                    bodyRow.Cells[4].Value = GetIntroducer(client, peopleAndPlacesAbstraction, pLanguageID);

                                    // status

                                    table.Rows.Add(bodyRow);
                                    totalReported++;
                                }
                            }
                            else
                                totalActiveDemoClients++;
                        }
                        else if (client.Status.Detail.Alias.CompareTo("INACTIVE") == 0)
                            totalInactiveClients++;
                        else if (client.Status.Detail.Alias.CompareTo("DEACTIVATED") == 0)
                            totalDeactivatedClients++;
                    }
                    else
                    {
                        genericReport.Elements.Add(new Paragraph("Unable to read client : " + client.ErrorDetails[0].ErrorMessage));
                        break;
                    }
                }
                genericReport.Elements.Add(table);

                genericReport.Elements.Add(new LineBreak());

                Table statisticTable = new Table();
                statisticTable.Rows.Add(MetricRow("Total Clients", totalClients));
                statisticTable.Rows.Add(MetricRow("Total Clients Reported", totalReported));
                statisticTable.Rows.Add(MetricRow("Total Active Demo Clients", totalActiveDemoClients));
                statisticTable.Rows.Add(MetricRow("Total Active Clients", totalActiveClients));
                statisticTable.Rows.Add(MetricRow("Total Inactive Clients", totalInactiveClients));
                statisticTable.Rows.Add(MetricRow("Total Deactivated Clients", totalDeactivatedClients));
                statisticTable.Rows.Add(MetricRow("Total Active Personal Clients", totalActivePersonalClients));
                statisticTable.Rows.Add(MetricRow("Total Active Organisation Clients", totalActiveCorporateClients));
                genericReport.Elements.Add(statisticTable);
            }
            catch (Exception exc)
            {
                genericReport.Elements.Add(new Paragraph("Caught Error : " + exc.Message));
            }
            return genericReport;
        }

        private Row MetricRow(string pLabel, int pMetric)
        {
            Row row = new Row();
            row.Cells.Add(new TextCell(pLabel));
            row.Cells.Add(new RightAlignedCell(pMetric.ToString()));
            return row;
        }

        private string GetSalesPerson(Client pClient, PeopleAndPlacesAbstraction pPeopleAndPlacesAbstraction, int pLanguageID)
        {
            string salesPersonName = "";
            BaseString[] salesPersons = pPeopleAndPlacesAbstraction.CustomQuery("SELECT pa.PartyID FROM [SchCore].[Party] pa INNER JOIN [SchCore].[PartyRole] pr ON pr.PartyID = pa.PartyID WHERE pr.PartyRoleID = (SELECT pre.FromPartyRoleID FROM [SchFinancial].[Client] c INNER JOIN [SchCore].[Party] p ON p.PartyID = c.ClientID INNER JOIN [SchCore].[PartyRole] pro ON pro.PartyID = p.PartyID INNER JOIN [SchCore].[PartyRelationship] pre ON pre.ToPartyRoleID = pro.PartyRoleID INNER JOIN [SchCore].[PartyRelationshipType] prt ON prt.PartyRelationshipTypeID = pre.PartyRelationshipTypeID INNER JOIN [SchGlobalisation].[Translation] t on t.TranslationID = prt.TranslationID WHERE c.ClientID = " + pClient.PartyID + " AND t.Alias = 'CLIENT_SALES_PERSON')");
            if (salesPersons.Length > 0)
            {
                if (salesPersons[0].ErrorsDetected == false)
                {
                    Person salesPerson = pPeopleAndPlacesAbstraction.GetPerson(Convert.ToInt32(salesPersons[0].Value), pLanguageID);
                    if (salesPerson.ErrorsDetected == false)
                    {
                        if (salesPerson.PartyID.HasValue)
                        {
                            salesPerson.PersonNames = pPeopleAndPlacesAbstraction.ListPersonNames((int)salesPerson.PartyID, pLanguageID);
                            if (salesPerson.PersonNames.Length > 0)
                            {
                                if (salesPerson.PersonNames[0].ErrorsDetected == false)
                                    salesPersonName = encryptorDecryptor.Decrypt(salesPerson.PersonNames[0].FullName);
                            }
                        }
                    }
                    else
                        salesPersonName = "[error]";
                }
                else
                    salesPersonName = "[error]";
            }
            return salesPersonName;
        }

        private string GetIntroducer(Client pClient, PeopleAndPlacesAbstraction pPeopleAndPlacesAbstraction, int pLanguageID)
        {
            string introducerName = "";
            BaseString[] clientIntroducers = pPeopleAndPlacesAbstraction.CustomQuery("SELECT pa.PartyID FROM [SchCore].[Party] pa INNER JOIN [SchCore].[PartyRole] pr ON pr.PartyID = pa.PartyID WHERE pr.PartyRoleID IN (SELECT pre.FromPartyRoleID FROM [SchFinancial].[Client] c INNER JOIN [SchCore].[Party] p ON p.PartyID = c.ClientID INNER JOIN [SchCore].[PartyRole] pro ON pro.PartyID = p.PartyID INNER JOIN [SchCore].[PartyRelationship] pre ON pre.ToPartyRoleID = pro.PartyRoleID INNER JOIN [SchCore].[PartyRelationshipType] prt ON prt.PartyRelationshipTypeID = pre.PartyRelationshipTypeID INNER JOIN [SchGlobalisation].[Translation] t on t.TranslationID = prt.TranslationID WHERE c.ClientID = " + pClient.PartyID + " AND t.Alias = 'CLIENT_INTRODUCER')");
            if (clientIntroducers.Length > 0)
            {
                if (clientIntroducers[0].ErrorsDetected == false)
                {
                    Person introducerPerson = pPeopleAndPlacesAbstraction.GetPerson(Convert.ToInt32(clientIntroducers[0].Value), pLanguageID);
                    if (introducerPerson.ErrorsDetected == false)
                    {
                        if (introducerPerson.PartyID.HasValue)
                        {
                            introducerPerson.PersonNames = pPeopleAndPlacesAbstraction.ListPersonNames((int)introducerPerson.PartyID, pLanguageID);
                            if (introducerPerson.PersonNames.Length > 0)
                            {
                                if (introducerPerson.PersonNames[0].ErrorsDetected == false)
                                    introducerName = encryptorDecryptor.Decrypt(introducerPerson.PersonNames[0].FullName);
                            }
                        }
                    }
                    else
                        introducerName = "[error]";
                }
                else
                    introducerName = "[error]";
            }
            return introducerName;
        }

        private bool IncludeInOutput(Client pClient, FinancialAbstraction pFinancialAbstraction)
        {
            bool includeInOutput = false;
            Account[] hubAccounts = pFinancialAbstraction.ListAccounts((int)pClient.PartyID, "%HUBACCOUNT");
            if (hubAccounts.Length > 0)
            {
                if (hubAccounts.Length == 1)
                {
                    if (hubAccounts[0].ErrorsDetected == false)
                    {
                        if (hubAccounts[0].Balance.HasValue == false)
                            includeInOutput = true;
                    }
                }
            }
            else
            {
                if (pClient.Category.Detail.Alias.CompareTo("INDIVIDUAL") != 0)
                    includeInOutput = true;
            }
            return includeInOutput;
        }
    }
}
