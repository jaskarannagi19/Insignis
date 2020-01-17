using Insignis.Asset.Management.Clients.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Encryption;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Core;
using Octavo.Gate.Nabu.Entities.Financial;
using Octavo.Gate.Nabu.Entities.PeopleAndPlaces;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Standard.Portfolio
{
    public class Generator : BaseType
    {
        private Client selectedClient = null;
        private BaseAbstraction baseAbstraction = null;
        private int languageID;

        public Generator(Client pSelectedClient, BaseAbstraction pBaseAbstraction, int pLanguage)
        {
            selectedClient = pSelectedClient;
            baseAbstraction = pBaseAbstraction;
            languageID = pLanguage;
        }

        public PortfolioSummaryParameter GenerateSpecificPortfolio(int pAccountID, int pMiniHubAccountID)
        {
            PortfolioSummaryParameter portfolioSummaryParameter = new PortfolioSummaryParameter();
            // Reference
            portfolioSummaryParameter.ReferenceNumber = selectedClient.ClientReference;

            EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
            CoreAbstraction coreAbstraction = new CoreAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            GlobalisationAbstraction globalisationAbstraction = new GlobalisationAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            FinancialAbstraction financialAbstraction = new FinancialAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);

            Party party = coreAbstraction.GetParty((int)selectedClient.PartyID, languageID);

            if (selectedClient.Category.IsIndividual)
            {
                Person person = peopleAndPlacesAbstraction.GetPerson((int)selectedClient.PartyID, languageID);
                if (person != null && person.ErrorsDetected == false && person.PartyID.HasValue)
                {
                    person.PartyType = party.PartyType;
                    // Personal Information
                    person.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)person.PartyID, languageID);
                    if (person.PersonNames != null && person.PersonNames.Length > 0 && person.PersonNames[0].ErrorsDetected == false)
                    {
                        portfolioSummaryParameter.FullName = encryptorDecryptor.Decrypt(person.PersonNames[0].FullName);
                    }
                }

                List<Account> allAccounts = new List<Account>();
                if (pAccountID != -1)
                    allAccounts.Add(financialAbstraction.GetAccount(pAccountID));
                if (pMiniHubAccountID != -1)
                    allAccounts.Add(financialAbstraction.GetAccount(pMiniHubAccountID));
                selectedClient.Accounts = null;
                selectedClient.Accounts = allAccounts.ToArray();
            }
            else
            {
                Organisation organisation = coreAbstraction.GetOrganisation((int)selectedClient.PartyID, languageID);
                if (organisation != null && organisation.ErrorsDetected == false && organisation.PartyID.HasValue)
                    portfolioSummaryParameter.FullName = organisation.Name;
                selectedClient.Accounts = financialAbstraction.ListAccounts((int)selectedClient.PartyID);
            }

            AccountType[] allAccountTypes = financialAbstraction.ListAccountTypes(languageID);
            AccountStatus[] accountStatusList = financialAbstraction.ListAccountStatuses(languageID);

            Institution[] institutions = financialAbstraction.ListInstitutions(languageID);

            foreach (Account account in selectedClient.Accounts)
            {
                if (account.ErrorsDetected == false)
                {
                    foreach (AccountType accountType in allAccountTypes)
                    {
                        if (accountType.ErrorsDetected == false)
                        {
                            if (account.Type.AccountTypeID == accountType.AccountTypeID)
                            {
                                account.Type = accountType;
                                break;
                            }
                        }
                    }
                    foreach (AccountStatus accountStatus in accountStatusList)
                    {
                        if (accountStatus.ErrorsDetected == false)
                        {
                            if (account.Status.AccountStatusID == accountStatus.AccountStatusID)
                            {
                                account.Status = accountStatus;
                                break;
                            }
                        }
                    }
                }
            }

            bool first = true;
            Currency currencyHelper = null;
            if (selectedClient.Accounts.Length > 0)
            {
                foreach (AccountType rootAccountType in allAccountTypes)
                {
                    if (rootAccountType.ErrorsDetected == false)
                    {
                        if (rootAccountType.ParentID.HasValue == false)
                        {
                            if (rootAccountType.Detail.Alias.CompareTo("ACT_PORTFOLIO") == 0)
                            {
                                foreach (AccountType hubAccountType in allAccountTypes)
                                {
                                    if (hubAccountType.ErrorsDetected == false)
                                    {
                                        if (hubAccountType.ParentID.HasValue && hubAccountType.ParentID == rootAccountType.AccountTypeID)
                                        {
                                            foreach (Account hubAccount in selectedClient.Accounts)
                                            {
                                                if (hubAccount.ErrorsDetected == false)
                                                {
                                                    if (hubAccount.Type.AccountTypeID == hubAccountType.AccountTypeID)
                                                    {
                                                        if (hubAccount.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                                                        {
                                                            if (first)
                                                            {
                                                                first = false;
                                                                currencyHelper = new Currency(hubAccount.Currency, financialAbstraction);
                                                            }

                                                            if (currencyHelper.GetCurrentCurrency().CurrencyID != hubAccount.Currency.CurrencyID)
                                                                currencyHelper = new Currency(hubAccount.Currency, financialAbstraction);

                                                            AccountType copyHubAccountType = hubAccountType;
                                                            if (hubAccount.Branch != null && hubAccount.Branch.PartyID.HasValue)
                                                            {
                                                                Institution hubAccountBank = HubAccountHelper.GetHubAccountInstitutionForBranch(hubAccount.Branch, coreAbstraction, financialAbstraction, languageID);
                                                                if (hubAccountBank != null && hubAccountBank.ErrorsDetected == false && hubAccountBank.PartyID.HasValue)
                                                                {
                                                                    if (hubAccountBank.Name.ToUpper().StartsWith("CATER ALLEN") == false)
                                                                    {
                                                                        Account miniHubAccount = null;
                                                                        foreach (Account relatedAccount in selectedClient.Accounts)
                                                                        {
                                                                            if (relatedAccount.ErrorsDetected == false)
                                                                            {
                                                                                if (relatedAccount.AccountID != hubAccount.AccountID && relatedAccount.Type.AccountTypeID == hubAccount.Type.AccountTypeID)
                                                                                {
                                                                                    if (relatedAccount.Status.AccountStatusID == hubAccount.Status.AccountStatusID)
                                                                                    {
                                                                                        if (relatedAccount.Currency.CurrencyID == hubAccount.Currency.CurrencyID)
                                                                                        {
                                                                                            miniHubAccount = relatedAccount;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        HubAccount hubAccountReport = new HubAccount();
                                                                        hubAccountReport.Type = copyHubAccountType.Detail.Name + "(" + currencyHelper.GetCurrentCurrency().CurrencyCode + ")";

                                                                        // if this is a joint account
                                                                        if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                        {
                                                                            PartyRelationship[] partyRelationships = coreAbstraction.ListPartyRelationshipsFrom((int)selectedClient.PartyID, languageID);
                                                                            if (partyRelationships.Length > 0)
                                                                            {
                                                                                PartyRelationshipType prtSpouseOrPartner = coreAbstraction.GetPartyRelationshipTypeByAlias("SPOUSEORPARTNER", languageID);
                                                                                if (prtSpouseOrPartner.ErrorsDetected == false && prtSpouseOrPartner.PartyRelationshipTypeID.HasValue)
                                                                                {
                                                                                    foreach (PartyRelationship prt in partyRelationships)
                                                                                    {
                                                                                        if (prt.ErrorsDetected == false)
                                                                                        {
                                                                                            if (prt.PartyRelationshipType.PartyRelationshipTypeID == prtSpouseOrPartner.PartyRelationshipTypeID)
                                                                                            {
                                                                                                Person partnerPerson = peopleAndPlacesAbstraction.GetPerson(prt.ToPartyID, languageID);
                                                                                                if (partnerPerson != null && partnerPerson.ErrorsDetected == false && partnerPerson.PartyID.HasValue)
                                                                                                {
                                                                                                    partnerPerson.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)partnerPerson.PartyID, languageID);
                                                                                                    if (partnerPerson.PersonNames != null && partnerPerson.PersonNames.Length > 0 && partnerPerson.PersonNames[0].ErrorsDetected == false)
                                                                                                    {
                                                                                                        hubAccountReport.AccountHeldWith = encryptorDecryptor.Decrypt(partnerPerson.PersonNames[0].FullName);
                                                                                                    }
                                                                                                }
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                            break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        hubAccountReport.Status = hubAccount.Status.Detail.Name;

                                                                        decimal amountAvailableToInvest = Calculations.AmountAvailableToInvest(hubAccount, miniHubAccount, financialAbstraction);
                                                                        hubAccountReport.AvailableToInvest = currencyHelper.DisplayValue(amountAvailableToInvest);

                                                                        decimal amountWeightedAverageRate = Calculations.CalculateConfirmedWeightedAverageRate(hubAccount, miniHubAccount);
                                                                        hubAccountReport.WeightedAverageOfConfirmedAccounts = amountWeightedAverageRate.ToString("0.00") + "%";

                                                                        decimal pendingInvestments = Calculations.CalculatePendingInvestments(hubAccount, miniHubAccount);
                                                                        hubAccountReport.TotalPendingInvestments = currencyHelper.DisplayValue(pendingInvestments);

                                                                        decimal? pendingWithdrawalRequests = Calculations.CalculatePendingWithdrawalsRequestedAgainstHubAccount(hubAccount, miniHubAccount, financialAbstraction);
                                                                        hubAccountReport.TotalPendingWithdrawalRequests = currencyHelper.DisplayValue(((pendingWithdrawalRequests.HasValue) ? pendingWithdrawalRequests.Value : 0));

                                                                        decimal amountFundsInvested = Calculations.CalculateFundsInvested(selectedClient.Accounts, hubAccount, miniHubAccount);
                                                                        hubAccountReport.FundsInvested = currencyHelper.DisplayValue(amountFundsInvested);

                                                                        decimal amountMinimumHubBalance = 0;
                                                                        BaseInteger hubAccountClientID = GetClientIDForAccount(hubAccount, financialAbstraction);
                                                                        if (hubAccountClientID.ErrorsDetected == false)
                                                                        {
                                                                            FeeAccount feeAccount = new FeeAccount(new Client(hubAccountClientID.Value), hubAccount, financialAbstraction, languageID);
                                                                            amountMinimumHubBalance = feeAccount.account.Balance.Value; //Helper.Calculations.CalculateMinimumHubBalance(hubAccount);
                                                                            hubAccountReport.MinimumHubBalance = currencyHelper.DisplayValue(amountMinimumHubBalance);
                                                                        }

                                                                        decimal amountAmountAvailableForWithdrawal = Calculations.CalculateAmountAvailableForWithdrawal(selectedClient.Accounts, hubAccount, miniHubAccount);
                                                                        hubAccountReport.AvailableToWithdraw = currencyHelper.DisplayValue(amountAmountAvailableForWithdrawal);

                                                                        decimal amountTotal = Calculations.CalculateTotal(selectedClient.Accounts, hubAccount, miniHubAccount, amountMinimumHubBalance, pendingWithdrawalRequests);
                                                                        hubAccountReport.Total = currencyHelper.DisplayValue(amountTotal);

                                                                        Account[] childAccounts = GetAllChildren(hubAccount, miniHubAccount, financialAbstraction);
                                                                        foreach (Account childAccount in childAccounts)
                                                                        {
                                                                            if (childAccount.ErrorsDetected == false)
                                                                            {
                                                                                foreach (AccountType accountType in allAccountTypes)
                                                                                {
                                                                                    if (accountType.ErrorsDetected == false)
                                                                                    {
                                                                                        if (childAccount.Type.AccountTypeID == accountType.AccountTypeID)
                                                                                        {
                                                                                            childAccount.Type = accountType;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                foreach (AccountStatus accountStatus in accountStatusList)
                                                                                {
                                                                                    if (accountStatus.ErrorsDetected == false)
                                                                                    {
                                                                                        if (childAccount.Status.AccountStatusID == accountStatus.AccountStatusID)
                                                                                        {
                                                                                            childAccount.Status = accountStatus;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (childAccount.Type.Detail.Alias.EndsWith("SAPPHIRE") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("RUBY") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("EMERALD") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("AMETHYST") == false)
                                                                                {
                                                                                    childAccount.Attributes = financialAbstraction.ListAccountAttributes((int)childAccount.AccountID, languageID);
                                                                                    AccountProductKey accountProductKey = null;
                                                                                    int jointAccountOpenedInNameOfPartyID = -1;
                                                                                    foreach (AccountAttribute accountAttribute in childAccount.Attributes)
                                                                                    {
                                                                                        if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                                                                                            accountProductKey = new AccountProductKey(accountAttribute.Value);
                                                                                        else
                                                                                        {
                                                                                            if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                                            {
                                                                                                if (accountAttribute.Detail.Alias.CompareTo("AAT_JOINT_ACCOUNT_OPENED_IN_NAME_OF_PARTY_ID") == 0)
                                                                                                {
                                                                                                    if (accountAttribute.Value != null && accountAttribute.Value.Trim().Length > 0)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            jointAccountOpenedInNameOfPartyID = Convert.ToInt32(accountAttribute.Value);
                                                                                                        }
                                                                                                        catch
                                                                                                        {
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    string institutionName = "";
                                                                                    foreach (Institution institution in institutions)
                                                                                    {
                                                                                        if (institution.ErrorsDetected == false)
                                                                                        {
                                                                                            if (accountProductKey != null && accountProductKey.InstitutionID == institution.PartyID)
                                                                                            {
                                                                                                institutionName = TextFormatter.RemoveNonASCIICharacters(institution.Name);
                                                                                                if (institutionName.Contains("&amp;"))
                                                                                                    institutionName = institutionName.Replace("&amp;", "&");
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    if (accountProductKey != null)
                                                                                    {
                                                                                        if (childAccount.Status.Detail.Alias.CompareTo("ACT_PENDING") == 0)
                                                                                        {
                                                                                            BespokeAccount pendingInvestment = new BespokeAccount();
                                                                                            pendingInvestment.Bank = institutionName;
                                                                                            pendingInvestment.InvestmentAmount.Add(currencyHelper.DisplayValue(childAccount.Balance));
                                                                                            if (childAccount.Rate.HasValue)
                                                                                                pendingInvestment.Rate = childAccount.Rate.Value.ToString("0.00") + "%";
                                                                                            else if (accountProductKey != null && accountProductKey.Rate.HasValue)
                                                                                                pendingInvestment.Rate = accountProductKey.Rate.Value.ToString("0.00") + "%";
                                                                                            pendingInvestment.Liquidity.Add(accountProductKey.investmentTerm.GetText());
                                                                                            pendingInvestment.LiquidityDays = ((accountProductKey.investmentTerm.GetLiquidityDays().HasValue) ? accountProductKey.investmentTerm.GetLiquidityDays().Value : 0);
                                                                                            hubAccountReport.pendingInvestments.Add(pendingInvestment);
                                                                                        }
                                                                                        else if (childAccount.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                                                                                        {
                                                                                            if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                                            {
                                                                                                if (jointAccountOpenedInNameOfPartyID != -1)
                                                                                                {
                                                                                                    if (jointAccountOpenedInNameOfPartyID == 0)
                                                                                                        institutionName += " [Opened jointly]";
                                                                                                    else
                                                                                                    {
                                                                                                        PersonName[] openedBy = peopleAndPlacesAbstraction.ListPersonNames(jointAccountOpenedInNameOfPartyID, languageID);
                                                                                                        if (openedBy.Length > 0 && openedBy[0].ErrorsDetected == false && openedBy[0].PersonNameID.HasValue)
                                                                                                            institutionName += " [Opened By " + encryptorDecryptor.Decrypt(openedBy[0].FullName) + "]";
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            BespokeAccount liveInvestment = new BespokeAccount();
                                                                                            liveInvestment.Bank = institutionName;
                                                                                            liveInvestment.InvestmentAmount.Add(currencyHelper.DisplayValue(childAccount.Balance));
                                                                                            if (childAccount.Rate.HasValue)
                                                                                                liveInvestment.Rate = childAccount.Rate.Value.ToString("0.00") + "%";
                                                                                            else if (accountProductKey != null && accountProductKey.Rate.HasValue)
                                                                                                liveInvestment.Rate = accountProductKey.Rate.Value.ToString("0.00") + "%";
                                                                                            liveInvestment.Liquidity.Add(accountProductKey.investmentTerm.GetText());
                                                                                            liveInvestment.LiquidityDays = ((accountProductKey.investmentTerm.GetLiquidityDays().HasValue) ? accountProductKey.investmentTerm.GetLiquidityDays().Value : 0);

                                                                                            int linesAdded = 0;
                                                                                            if (accountProductKey != null && accountProductKey.investmentTerm != null)
                                                                                            {
                                                                                                if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                                                                                                {
                                                                                                    // do nothing
                                                                                                }
                                                                                                else if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                                {
                                                                                                    // do nothing

                                                                                                }
                                                                                                else if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.TermAccount)
                                                                                                {
                                                                                                    DateTime? termMaturesOn = accountProductKey.investmentTerm.GetTermExpiryDate(childAccount.DateOpened, accountProductKey.investedOn);
                                                                                                    if (termMaturesOn.HasValue)
                                                                                                    {
                                                                                                        if (DateTime.Now.CompareTo(termMaturesOn.Value) < 0)
                                                                                                        {
                                                                                                            int differenceInDays = Convert.ToInt32(termMaturesOn.Value.Subtract(DateTime.Now).TotalDays);
                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.Liquidity.Add("Matures: " + termMaturesOn.Value.ToString("dd-MMM-yyyy"));
                                                                                                            liveInvestment.Liquidity.Add("[Available in " + differenceInDays.ToString() + " day(s)]");
                                                                                                            linesAdded = 3;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.Liquidity.Add("Matured");
                                                                                                            linesAdded = 2;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            hubAccountReport.liveInvestments.Add(liveInvestment);
                                                                                            for (int i = 0; i < linesAdded; i++)
                                                                                                liveInvestment.InvestmentAmount.Add("");

                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount ||
                                                                                                accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                            {
                                                                                                string SQL = "";
                                                                                                SQL += "SELECT";
                                                                                                SQL += " DebitValue,";
                                                                                                SQL += " TransactionDate";
                                                                                                SQL += " FROM [SchFinancial].[AccountTransaction] act";
                                                                                                SQL += " INNER JOIN [SchFinancial].[TransactionType] trt ON trt.TransactionTypeID = act.TransactionTypeID";
                                                                                                SQL += " INNER JOIN [SchFinancial].[TransactionStatus] trs ON trs.TransactionStatusID = act.TransactionStatusID";
                                                                                                SQL += " INNER JOIN [SchGlobalisation].[Translation] ttt ON ttt.TranslationID = trt.TranslationID";
                                                                                                SQL += " INNER JOIN [SchGlobalisation].[Translation] tts ON tts.TranslationID = trs.TranslationID";
                                                                                                SQL += " WHERE AccountID=" + childAccount.AccountID;
                                                                                                SQL += " AND ttt.Alias IN ('TT_WITHDRAWALREQUEST','TT_WITHDRAWALREQUESTANDCLOSE','TT_NOTICEGIVEN''TT_NOTICEGIVENANDCLOSE')";
                                                                                                SQL += " AND tts.Alias NOT IN ('TS_CANCELLED');";
                                                                                                BaseString[] pendingChildAccountWithdrawals = financialAbstraction.CustomQuery(SQL);
                                                                                                if (pendingChildAccountWithdrawals != null && pendingChildAccountWithdrawals.Length > 0)
                                                                                                {
                                                                                                    string separator = "|";
                                                                                                    foreach (BaseString pendingChildAccountWithdrawal in pendingChildAccountWithdrawals)
                                                                                                    {
                                                                                                        if (pendingChildAccountWithdrawal.ErrorsDetected == false)
                                                                                                        {
                                                                                                            string[] fields = pendingChildAccountWithdrawal.Value.Split(separator.ToCharArray());

                                                                                                            DateTime transactionDate = DateTime.ParseExact(fields[1].Replace("'", ""), "dd-MMM-yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.InvestmentAmount.Add("");

                                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                                                                                                                liveInvestment.Liquidity.Add("Withdrawal Requested");
                                                                                                            else
                                                                                                                liveInvestment.Liquidity.Add("Notice Given");

                                                                                                            liveInvestment.InvestmentAmount.Add("(" + currencyHelper.DisplayValue(Convert.ToDecimal(fields[0])) + ")");

                                                                                                            liveInvestment.Liquidity.Add(transactionDate.ToString("dd-MMM-yyyy"));
                                                                                                            liveInvestment.InvestmentAmount.Add("");

                                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                                            {
                                                                                                                if (accountProductKey.investmentTerm.NoticeDays.HasValue)
                                                                                                                {
                                                                                                                    string comment = "[";
                                                                                                                    comment += "Available in ";
                                                                                                                    DateTime availableDate = transactionDate.AddDays(accountProductKey.investmentTerm.NoticeDays.Value);
                                                                                                                    double daysUntilAvailable = availableDate.Subtract(DateTime.Now).TotalDays;
                                                                                                                    comment += Math.Round(daysUntilAvailable);
                                                                                                                    comment += " day";
                                                                                                                    if (daysUntilAvailable > 1)
                                                                                                                        comment += "s";
                                                                                                                    comment += "]";
                                                                                                                    liveInvestment.Liquidity.Add(comment);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                                liveInvestment.Liquidity.Add("[Pending]");
                                                                                                            liveInvestment.InvestmentAmount.Add("");
                                                                                                        }
                                                                                                        else
                                                                                                            break;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    GemProduct gemProduct = new GemProduct();

                                                                                    gemProduct.Product = childAccount.Type.Detail.Name;
                                                                                    gemProduct.Status = childAccount.Status.Detail.Name;
                                                                                    gemProduct.InvestmentAmount = currencyHelper.DisplayValue(childAccount.Balance);

                                                                                    hubAccountReport.gemProducts.Add(gemProduct);
                                                                                }
                                                                            }
                                                                        }

                                                                        // order the child accounts by liquidity days (shortest to longest time)
                                                                        hubAccountReport.pendingInvestments.Sort((a, b) => a.LiquidityDays.CompareTo(b.LiquidityDays));
                                                                        hubAccountReport.liveInvestments.Sort((a, b) => a.LiquidityDays.CompareTo(b.LiquidityDays));

                                                                        portfolioSummaryParameter.hubAccounts.Add(hubAccountReport);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return portfolioSummaryParameter;
        }

        public PortfolioSummaryParameter GenerateClientPortfolio()
        {
            PortfolioSummaryParameter portfolioSummaryParameter = new PortfolioSummaryParameter();
            // Reference
            portfolioSummaryParameter.ReferenceNumber = selectedClient.ClientReference;

            EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
            CoreAbstraction coreAbstraction = new CoreAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            GlobalisationAbstraction globalisationAbstraction = new GlobalisationAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            FinancialAbstraction financialAbstraction = new FinancialAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);
            PeopleAndPlacesAbstraction peopleAndPlacesAbstraction = new PeopleAndPlacesAbstraction(baseAbstraction.ConnectionString, baseAbstraction.DBType, baseAbstraction.ErrorLogFile);

            Party party = coreAbstraction.GetParty((int)selectedClient.PartyID, languageID);

            List<Reports.Custom.Report.HubAccount> relatedReadOnlyHubAccounts = new List<Reports.Custom.Report.HubAccount>();
            if (selectedClient.Category.IsIndividual)
            {
                Person person = peopleAndPlacesAbstraction.GetPerson((int)selectedClient.PartyID, languageID);
                if (person != null && person.ErrorsDetected == false && person.PartyID.HasValue)
                {
                    person.PartyType = party.PartyType;
                    // Personal Information
                    person.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)person.PartyID, languageID);
                    if (person.PersonNames != null && person.PersonNames.Length > 0 && person.PersonNames[0].ErrorsDetected == false)
                    {
                        portfolioSummaryParameter.FullName = encryptorDecryptor.Decrypt(person.PersonNames[0].FullName);
                    }
                }

                List<Account> signatoryHubAccounts = RelatedAccounts.ListRelatedHubAccounts(selectedClient.PartyID.Value, "SIGNATORY", coreAbstraction, languageID);
                if (signatoryHubAccounts.Count > 0)
                {
                    foreach (Account signatoryHubAccount in signatoryHubAccounts)
                    {
                        if (signatoryHubAccount.ErrorsDetected == false)
                            relatedReadOnlyHubAccounts.Add(new Reports.Custom.Report.HubAccount((int)signatoryHubAccount.AccountID, financialAbstraction, languageID));
                    }
                    signatoryHubAccounts.Clear();
                }
                signatoryHubAccounts = null;
                List<Account> jointAccounts = RelatedAccounts.ListRelatedHubAccounts(selectedClient.PartyID.Value, "SPOUSEORPARTNER", coreAbstraction, languageID);
                if (jointAccounts.Count > 0)
                {
                    foreach (Account jointHubAccount in jointAccounts)
                    {
                        if (jointHubAccount.ErrorsDetected == false)
                        {
                            Institution hubAccountBank = HubAccountHelper.GetHubAccountInstitutionForBranch(jointHubAccount.Branch, coreAbstraction, financialAbstraction, languageID);
                            if (hubAccountBank != null && hubAccountBank.ErrorsDetected == false && hubAccountBank.PartyID.HasValue)
                            {
                                //if (hubAccountBank.Name.ToUpper().StartsWith("BARCLAY"))
                                relatedReadOnlyHubAccounts.Add(new Reports.Custom.Report.HubAccount((int)jointHubAccount.AccountID, financialAbstraction, languageID));
                                //else
                                //{
                                //}
                            }
                        }
                    }
                    jointAccounts.Clear();
                }
                jointAccounts = null;

                List<Account> allAccounts = new List<Account>();
                selectedClient.Accounts = financialAbstraction.ListAccounts((int)selectedClient.PartyID);
                if (selectedClient.Accounts != null && selectedClient.Accounts.Length > 0)
                {
                    foreach (Account account in selectedClient.Accounts)
                        allAccounts.Add(account);
                }
                if (relatedReadOnlyHubAccounts != null && relatedReadOnlyHubAccounts.Count > 0)
                {
                    foreach (Reports.Custom.Report.HubAccount hubAccount in relatedReadOnlyHubAccounts)
                        allAccounts.Add(hubAccount.GetAccount());
                }
                selectedClient.Accounts = null;
                selectedClient.Accounts = allAccounts.ToArray();
            }
            else
            {
                Organisation organisation = coreAbstraction.GetOrganisation((int)selectedClient.PartyID, languageID);
                if (organisation != null && organisation.ErrorsDetected == false && organisation.PartyID.HasValue)
                {
                    portfolioSummaryParameter.FullName = organisation.Name;
                }
                selectedClient.Accounts = financialAbstraction.ListAccounts((int)selectedClient.PartyID);
            }

            AccountType[] allAccountTypes = financialAbstraction.ListAccountTypes(languageID);
            AccountStatus[] accountStatusList = financialAbstraction.ListAccountStatuses(languageID);

            Institution[] institutions = financialAbstraction.ListInstitutions(languageID);

            foreach (Account account in selectedClient.Accounts)
            {
                if (account.ErrorsDetected == false)
                {
                    foreach (AccountType accountType in allAccountTypes)
                    {
                        if (accountType.ErrorsDetected == false)
                        {
                            if (account.Type.AccountTypeID == accountType.AccountTypeID)
                            {
                                account.Type = accountType;
                                break;
                            }
                        }
                    }
                    foreach (AccountStatus accountStatus in accountStatusList)
                    {
                        if (accountStatus.ErrorsDetected == false)
                        {
                            if (account.Status.AccountStatusID == accountStatus.AccountStatusID)
                            {
                                account.Status = accountStatus;
                                break;
                            }
                        }
                    }
                }
            }

            bool first = true;
            Currency currencyHelper = null;
            if (selectedClient.Accounts.Length > 0)
            {
                foreach (AccountType rootAccountType in allAccountTypes)
                {
                    if (rootAccountType.ErrorsDetected == false)
                    {
                        if (rootAccountType.ParentID.HasValue == false)
                        {
                            if (rootAccountType.Detail.Alias.CompareTo("ACT_PORTFOLIO") == 0)
                            {
                                foreach (AccountType hubAccountType in allAccountTypes)
                                {
                                    if (hubAccountType.ErrorsDetected == false)
                                    {
                                        if (hubAccountType.ParentID.HasValue && hubAccountType.ParentID == rootAccountType.AccountTypeID)
                                        {
                                            foreach (Account hubAccount in selectedClient.Accounts)
                                            {
                                                if (hubAccount.ErrorsDetected == false)
                                                {
                                                    if (hubAccount.Type.AccountTypeID == hubAccountType.AccountTypeID)
                                                    {
                                                        if (hubAccount.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                                                        {
                                                            if (first)
                                                            {
                                                                first = false;
                                                                currencyHelper = new Currency(hubAccount.Currency, financialAbstraction);
                                                            }

                                                            if (currencyHelper.GetCurrentCurrency().CurrencyID != hubAccount.Currency.CurrencyID)
                                                                currencyHelper = new Currency(hubAccount.Currency, financialAbstraction);

                                                            AccountType copyHubAccountType = hubAccountType;
                                                            if (hubAccount.Branch != null && hubAccount.Branch.PartyID.HasValue)
                                                            {
                                                                Institution hubAccountBank = HubAccountHelper.GetHubAccountInstitutionForBranch(hubAccount.Branch, coreAbstraction, financialAbstraction, languageID);
                                                                if (hubAccountBank != null && hubAccountBank.ErrorsDetected == false && hubAccountBank.PartyID.HasValue)
                                                                {
                                                                    if (hubAccountBank.Name.ToUpper().StartsWith("CATER ALLEN") == false)
                                                                    {
                                                                        Account miniHubAccount = null;
                                                                        foreach (Account relatedAccount in selectedClient.Accounts)
                                                                        {
                                                                            if (relatedAccount.ErrorsDetected == false)
                                                                            {
                                                                                if (relatedAccount.AccountID != hubAccount.AccountID && relatedAccount.Type.AccountTypeID == hubAccount.Type.AccountTypeID)
                                                                                {
                                                                                    if (relatedAccount.Status.AccountStatusID == hubAccount.Status.AccountStatusID)
                                                                                    {
                                                                                        if (relatedAccount.Currency.CurrencyID == hubAccount.Currency.CurrencyID)
                                                                                        {
                                                                                            Institution minihubAccountBank = HubAccountHelper.GetHubAccountInstitutionForBranch(relatedAccount.Branch, coreAbstraction, financialAbstraction, languageID);
                                                                                            if (minihubAccountBank != null && minihubAccountBank.ErrorsDetected == false && minihubAccountBank.PartyID.HasValue)
                                                                                            {
                                                                                                if (minihubAccountBank.Name.ToUpper().StartsWith("CATER ALLEN"))
                                                                                                {
                                                                                                    miniHubAccount = relatedAccount;
                                                                                                    break;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        HubAccount hubAccountReport = new HubAccount();
                                                                        hubAccountReport.Type = copyHubAccountType.Detail.Name;
                                                                        if (relatedReadOnlyHubAccounts != null && relatedReadOnlyHubAccounts.Count > 0)
                                                                        {
                                                                            foreach (Reports.Custom.Report.HubAccount relatedHubAccount in relatedReadOnlyHubAccounts)
                                                                            {
                                                                                if (relatedHubAccount.GetAccount().AccountID == hubAccount.AccountID)
                                                                                {
                                                                                    if (relatedHubAccount.GetClient().PartyID != selectedClient.PartyID)
                                                                                    {
                                                                                        hubAccountReport.Type += " [" + relatedHubAccount.GetClient().ClientReference + "]";
                                                                                    }
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                        hubAccountReport.Type += " (" + currencyHelper.GetCurrentCurrency().CurrencyCode + ")";

                                                                        // if this is a joint account
                                                                        if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                        {
                                                                            PartyRelationship[] partyRelationships = coreAbstraction.ListPartyRelationshipsFrom((int)selectedClient.PartyID, languageID);
                                                                            if (partyRelationships.Length > 0)
                                                                            {
                                                                                PartyRelationshipType prtSpouseOrPartner = coreAbstraction.GetPartyRelationshipTypeByAlias("SPOUSEORPARTNER", languageID);
                                                                                if (prtSpouseOrPartner.ErrorsDetected == false && prtSpouseOrPartner.PartyRelationshipTypeID.HasValue)
                                                                                {
                                                                                    foreach (PartyRelationship prt in partyRelationships)
                                                                                    {
                                                                                        if (prt.ErrorsDetected == false)
                                                                                        {
                                                                                            if (prt.PartyRelationshipType.PartyRelationshipTypeID == prtSpouseOrPartner.PartyRelationshipTypeID)
                                                                                            {
                                                                                                Person partnerPerson = peopleAndPlacesAbstraction.GetPerson(prt.ToPartyID, languageID);
                                                                                                if (partnerPerson != null && partnerPerson.ErrorsDetected == false && partnerPerson.PartyID.HasValue)
                                                                                                {
                                                                                                    partnerPerson.PersonNames = peopleAndPlacesAbstraction.ListPersonNames((int)partnerPerson.PartyID, languageID);
                                                                                                    if (partnerPerson.PersonNames != null && partnerPerson.PersonNames.Length > 0 && partnerPerson.PersonNames[0].ErrorsDetected == false)
                                                                                                    {
                                                                                                        hubAccountReport.AccountHeldWith = encryptorDecryptor.Decrypt(partnerPerson.PersonNames[0].FullName);
                                                                                                    }
                                                                                                }
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                            break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        hubAccountReport.Status = hubAccount.Status.Detail.Name;

                                                                        decimal amountAvailableToInvest = Calculations.AmountAvailableToInvest(hubAccount, miniHubAccount, financialAbstraction);
                                                                        hubAccountReport.AvailableToInvest = currencyHelper.DisplayValue(amountAvailableToInvest);

                                                                        decimal amountWeightedAverageRate = Calculations.CalculateConfirmedWeightedAverageRate(hubAccount, miniHubAccount);
                                                                        hubAccountReport.WeightedAverageOfConfirmedAccounts = amountWeightedAverageRate.ToString("0.00") + "%";

                                                                        decimal pendingInvestments = Calculations.CalculatePendingInvestments(hubAccount, miniHubAccount);
                                                                        hubAccountReport.TotalPendingInvestments = currencyHelper.DisplayValue(pendingInvestments);

                                                                        decimal? pendingWithdrawalRequests = Calculations.CalculatePendingWithdrawalsRequestedAgainstHubAccount(hubAccount, miniHubAccount, financialAbstraction);
                                                                        hubAccountReport.TotalPendingWithdrawalRequests = currencyHelper.DisplayValue(((pendingWithdrawalRequests.HasValue) ? pendingWithdrawalRequests.Value : 0));

                                                                        decimal amountFundsInvested = Calculations.CalculateFundsInvested(selectedClient.Accounts, hubAccount, miniHubAccount);
                                                                        hubAccountReport.FundsInvested = currencyHelper.DisplayValue(amountFundsInvested);

                                                                        Client hubAccountOwner = HubAccountHelper.GetClientForHubAccount(hubAccount, financialAbstraction, languageID);
                                                                        FeeAccount feeAccount = new FeeAccount(hubAccountOwner, hubAccount, financialAbstraction, languageID);
                                                                        decimal amountMinimumHubBalance = feeAccount.account.Balance.Value; //Helper.Calculations.CalculateMinimumHubBalance(hubAccount);
                                                                        hubAccountReport.MinimumHubBalance = currencyHelper.DisplayValue(amountMinimumHubBalance);

                                                                        decimal amountAmountAvailableForWithdrawal = Calculations.CalculateAmountAvailableForWithdrawal(selectedClient.Accounts, hubAccount, miniHubAccount);
                                                                        hubAccountReport.AvailableToWithdraw = currencyHelper.DisplayValue(amountAmountAvailableForWithdrawal);

                                                                        decimal amountTotal = Calculations.CalculateTotal(selectedClient.Accounts, hubAccount, miniHubAccount, amountMinimumHubBalance, pendingWithdrawalRequests);
                                                                        hubAccountReport.Total = currencyHelper.DisplayValue(amountTotal);

                                                                        Account[] childAccounts = GetAllChildren(hubAccount, miniHubAccount, financialAbstraction);
                                                                        foreach (Account childAccount in childAccounts)
                                                                        {
                                                                            if (childAccount.ErrorsDetected == false)
                                                                            {
                                                                                foreach (AccountType accountType in allAccountTypes)
                                                                                {
                                                                                    if (accountType.ErrorsDetected == false)
                                                                                    {
                                                                                        if (childAccount.Type.AccountTypeID == accountType.AccountTypeID)
                                                                                        {
                                                                                            childAccount.Type = accountType;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                foreach (AccountStatus accountStatus in accountStatusList)
                                                                                {
                                                                                    if (accountStatus.ErrorsDetected == false)
                                                                                    {
                                                                                        if (childAccount.Status.AccountStatusID == accountStatus.AccountStatusID)
                                                                                        {
                                                                                            childAccount.Status = accountStatus;
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (childAccount.Type.Detail.Alias.EndsWith("SAPPHIRE") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("RUBY") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("EMERALD") == false &&
                                                                                    childAccount.Type.Detail.Alias.EndsWith("AMETHYST") == false)
                                                                                {
                                                                                    childAccount.Attributes = financialAbstraction.ListAccountAttributes((int)childAccount.AccountID, languageID);
                                                                                    AccountProductKey accountProductKey = null;
                                                                                    int jointAccountOpenedInNameOfPartyID = -1;
                                                                                    foreach (AccountAttribute accountAttribute in childAccount.Attributes)
                                                                                    {
                                                                                        if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                                                                                            accountProductKey = new AccountProductKey(accountAttribute.Value);
                                                                                        else
                                                                                        {
                                                                                            if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                                            {
                                                                                                if (accountAttribute.Detail.Alias.CompareTo("AAT_JOINT_ACCOUNT_OPENED_IN_NAME_OF_PARTY_ID") == 0)
                                                                                                {
                                                                                                    if (accountAttribute.Value != null && accountAttribute.Value.Trim().Length > 0)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            jointAccountOpenedInNameOfPartyID = Convert.ToInt32(accountAttribute.Value);
                                                                                                        }
                                                                                                        catch
                                                                                                        {
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    string institutionName = "";
                                                                                    foreach (Institution institution in institutions)
                                                                                    {
                                                                                        if (institution.ErrorsDetected == false)
                                                                                        {
                                                                                            if (accountProductKey != null && accountProductKey.InstitutionID == institution.PartyID)
                                                                                            {
                                                                                                institutionName = TextFormatter.RemoveNonASCIICharacters(institution.Name);
                                                                                                if (institutionName.Contains("&amp;"))
                                                                                                    institutionName = institutionName.Replace("&amp;", "&");
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    if (accountProductKey != null)
                                                                                    {
                                                                                        if (childAccount.Status.Detail.Alias.CompareTo("ACT_PENDING") == 0)
                                                                                        {
                                                                                            BespokeAccount pendingInvestment = new BespokeAccount();
                                                                                            pendingInvestment.Bank = institutionName;
                                                                                            pendingInvestment.InvestmentAmount.Add(currencyHelper.DisplayValue(childAccount.Balance));
                                                                                            if (childAccount.Rate.HasValue)
                                                                                                pendingInvestment.Rate = childAccount.Rate.Value.ToString("0.00") + "%";
                                                                                            else if (accountProductKey != null && accountProductKey.Rate.HasValue)
                                                                                                pendingInvestment.Rate = accountProductKey.Rate.Value.ToString("0.00") + "%";
                                                                                            pendingInvestment.Liquidity.Add(accountProductKey.investmentTerm.GetText());
                                                                                            pendingInvestment.LiquidityDays = ((accountProductKey.investmentTerm.GetLiquidityDays().HasValue) ? accountProductKey.investmentTerm.GetLiquidityDays().Value : 0);
                                                                                            hubAccountReport.pendingInvestments.Add(pendingInvestment);
                                                                                        }
                                                                                        else if (childAccount.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                                                                                        {
                                                                                            if (hubAccountType.Detail.Alias.CompareTo("ACT_JOINTHUBACCOUNT") == 0)
                                                                                            {
                                                                                                if (jointAccountOpenedInNameOfPartyID != -1)
                                                                                                {
                                                                                                    if (jointAccountOpenedInNameOfPartyID == 0)
                                                                                                        institutionName += " [Opened jointly]";
                                                                                                    else
                                                                                                    {
                                                                                                        PersonName[] openedBy = peopleAndPlacesAbstraction.ListPersonNames(jointAccountOpenedInNameOfPartyID, languageID);
                                                                                                        if (openedBy.Length > 0 && openedBy[0].ErrorsDetected == false && openedBy[0].PersonNameID.HasValue)
                                                                                                            institutionName += " [Opened By " + encryptorDecryptor.Decrypt(openedBy[0].FullName) + "]";
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            BespokeAccount liveInvestment = new BespokeAccount();
                                                                                            liveInvestment.Bank = institutionName;
                                                                                            liveInvestment.InvestmentAmount.Add(currencyHelper.DisplayValue(childAccount.Balance));
                                                                                            if (childAccount.Rate.HasValue)
                                                                                                liveInvestment.Rate = childAccount.Rate.Value.ToString("0.00") + "%";
                                                                                            else if (accountProductKey != null && accountProductKey.Rate.HasValue)
                                                                                                liveInvestment.Rate = accountProductKey.Rate.Value.ToString("0.00") + "%";
                                                                                            liveInvestment.Liquidity.Add(accountProductKey.investmentTerm.GetText());
                                                                                            liveInvestment.LiquidityDays = ((accountProductKey.investmentTerm.GetLiquidityDays().HasValue) ? accountProductKey.investmentTerm.GetLiquidityDays().Value : 0);
                                                                                            int linesAdded = 0;
                                                                                            if (accountProductKey != null && accountProductKey.investmentTerm != null)
                                                                                            {
                                                                                                if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                                                                                                {
                                                                                                    // do nothing
                                                                                                }
                                                                                                else if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                                {
                                                                                                    // do nothing

                                                                                                }
                                                                                                else if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.TermAccount)
                                                                                                {
                                                                                                    DateTime? termMaturesOn = accountProductKey.investmentTerm.GetTermExpiryDate(childAccount.DateOpened, accountProductKey.investedOn);
                                                                                                    if (termMaturesOn.HasValue)
                                                                                                    {
                                                                                                        if (DateTime.Now.CompareTo(termMaturesOn.Value) < 0)
                                                                                                        {
                                                                                                            int differenceInDays = Convert.ToInt32(termMaturesOn.Value.Subtract(DateTime.Now).TotalDays);
                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.Liquidity.Add("Matures: " + termMaturesOn.Value.ToString("dd-MMM-yyyy"));
                                                                                                            liveInvestment.Liquidity.Add("[Available in " + differenceInDays.ToString() + " day(s)]");
                                                                                                            linesAdded = 3;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.Liquidity.Add("Matured");
                                                                                                            linesAdded = 2;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            hubAccountReport.liveInvestments.Add(liveInvestment);
                                                                                            for (int i = 0; i < linesAdded; i++)
                                                                                                liveInvestment.InvestmentAmount.Add("");

                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount ||
                                                                                                accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                            {
                                                                                                string SQL = "";
                                                                                                SQL += "SELECT";
                                                                                                SQL += " DebitValue,";
                                                                                                SQL += " TransactionDate";
                                                                                                SQL += " FROM [SchFinancial].[AccountTransaction] act";
                                                                                                SQL += " INNER JOIN [SchFinancial].[TransactionType] trt ON trt.TransactionTypeID = act.TransactionTypeID";
                                                                                                SQL += " INNER JOIN [SchFinancial].[TransactionStatus] trs ON trs.TransactionStatusID = act.TransactionStatusID";
                                                                                                SQL += " INNER JOIN [SchGlobalisation].[Translation] ttt ON ttt.TranslationID = trt.TranslationID";
                                                                                                SQL += " INNER JOIN [SchGlobalisation].[Translation] tts ON tts.TranslationID = trs.TranslationID";
                                                                                                SQL += " WHERE AccountID=" + childAccount.AccountID;
                                                                                                SQL += " AND ttt.Alias IN ('TT_WITHDRAWALREQUEST','TT_WITHDRAWALREQUESTANDCLOSE','TT_NOTICEGIVEN''TT_NOTICEGIVENANDCLOSE')";
                                                                                                SQL += " AND tts.Alias NOT IN ('TS_CANCELLED');";
                                                                                                BaseString[] pendingChildAccountWithdrawals = financialAbstraction.CustomQuery(SQL);
                                                                                                if (pendingChildAccountWithdrawals != null && pendingChildAccountWithdrawals.Length > 0)
                                                                                                {
                                                                                                    string separator = "|";
                                                                                                    foreach (BaseString pendingChildAccountWithdrawal in pendingChildAccountWithdrawals)
                                                                                                    {
                                                                                                        if (pendingChildAccountWithdrawal.ErrorsDetected == false)
                                                                                                        {
                                                                                                            string[] fields = pendingChildAccountWithdrawal.Value.Split(separator.ToCharArray());

                                                                                                            DateTime transactionDate = DateTime.ParseExact(fields[1].Replace("'", ""), "dd-MMM-yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

                                                                                                            liveInvestment.Liquidity.Add("");
                                                                                                            liveInvestment.InvestmentAmount.Add("");

                                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                                                                                                                liveInvestment.Liquidity.Add("Withdrawal Requested");
                                                                                                            else
                                                                                                                liveInvestment.Liquidity.Add("Notice Given");

                                                                                                            liveInvestment.InvestmentAmount.Add("(" + currencyHelper.DisplayValue(Convert.ToDecimal(fields[0])) + ")");

                                                                                                            liveInvestment.Liquidity.Add(transactionDate.ToString("dd-MMM-yyyy"));
                                                                                                            liveInvestment.InvestmentAmount.Add("");

                                                                                                            if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                                                                                            {
                                                                                                                if (accountProductKey.investmentTerm.NoticeDays.HasValue)
                                                                                                                {
                                                                                                                    string comment = "[";
                                                                                                                    comment += "Available in ";
                                                                                                                    DateTime availableDate = transactionDate.AddDays(accountProductKey.investmentTerm.NoticeDays.Value);
                                                                                                                    double daysUntilAvailable = availableDate.Subtract(DateTime.Now).TotalDays;
                                                                                                                    comment += Math.Round(daysUntilAvailable);
                                                                                                                    comment += " day";
                                                                                                                    if (daysUntilAvailable > 1)
                                                                                                                        comment += "s";
                                                                                                                    comment += "]";
                                                                                                                    liveInvestment.Liquidity.Add(comment);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                                liveInvestment.Liquidity.Add("[Pending]");
                                                                                                            liveInvestment.InvestmentAmount.Add("");
                                                                                                        }
                                                                                                        else
                                                                                                            break;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    GemProduct gemProduct = new GemProduct();

                                                                                    gemProduct.Product = childAccount.Type.Detail.Name;
                                                                                    gemProduct.Status = childAccount.Status.Detail.Name;
                                                                                    gemProduct.InvestmentAmount = currencyHelper.DisplayValue(childAccount.Balance);

                                                                                    hubAccountReport.gemProducts.Add(gemProduct);
                                                                                }
                                                                            }
                                                                        }

                                                                        // order the child accounts by liquidity days (shortest to longest time)
                                                                        hubAccountReport.pendingInvestments.Sort((a, b) => a.LiquidityDays.CompareTo(b.LiquidityDays));
                                                                        hubAccountReport.liveInvestments.Sort((a, b) => a.LiquidityDays.CompareTo(b.LiquidityDays));

                                                                        portfolioSummaryParameter.hubAccounts.Add(hubAccountReport);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return portfolioSummaryParameter;
        }

        private Account[] GetAllChildren(Account pHubAccount, Account pMiniHubAccount, FinancialAbstraction pFinancialAbstraction)
        {
            List<Account> allChildren = new List<Account>();
            if (pHubAccount.Children == null)
                pHubAccount.Children = pFinancialAbstraction.ListAccountChildren((int)pHubAccount.AccountID);
            foreach (Account child in pHubAccount.Children)
                allChildren.Add(child);
            if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
            {
                if (pMiniHubAccount.Children == null)
                    pMiniHubAccount.Children = pFinancialAbstraction.ListAccountChildren((int)pMiniHubAccount.AccountID);
                foreach (Account child in pMiniHubAccount.Children)
                    allChildren.Add(child);
            }
            return allChildren.ToArray();
        }

        private BaseInteger GetClientIDForAccount(Account pAccount, FinancialAbstraction pFinancialAbstraction)
        {
            BaseInteger result = new BaseInteger(-1);
            try
            {
                result = pFinancialAbstraction.CustomQueryAsInteger("SELECT ClientID FROM [SchFinancial].[Account] WHERE AccountID = " + pAccount.AccountID);
            }
            catch (Exception exc)
            {
                result.ErrorsDetected = true;
                result.StackTrace = exc.StackTrace;
                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
            }
            return result;
        }
    }
}
