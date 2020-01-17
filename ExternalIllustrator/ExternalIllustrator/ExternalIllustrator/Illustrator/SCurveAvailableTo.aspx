<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SCurveAvailableTo.aspx.cs" Inherits="Insignis.Asset.Management.External.Illustrator.Illustrator.SCurveAvailableTo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentHead" runat="server">
    <link href="../Styles/floop.css" rel="stylesheet" />
    <link href="../Styles/floop2.css" rel="stylesheet" />
    <link href="../Styles/OGFixedTable.css" rel="stylesheet" />
    <script src="../Scripts/Nabu/nabu-qti-client.js"></script>
    <script src="../Scripts/jquery-ui.min.js"></script>
    <link href="../Styles/jquery-ui.css" rel="stylesheet" />
    <link href="../../Styles/tablesort.css" rel="stylesheet" />
    <script src="../../Scripts/tablesort.js"></script>
    <script src="../../Scripts/tablesort.number.js"></script>

    <script type="text/javascript">
        jQuery( document ).ready(function() {
            if (document.getElementById("ContentBody__hiddenShowLiquidityWarningDialog") != null) {
                if (document.getElementById("ContentBody__hiddenShowLiquidityWarningDialog").value === 'true') {
                    jAlert("Warning: the proposed portfolio total is less than the total of the required liquidity critera.");
                }
            }
            if (document.getElementById("ContentBody__hiddenScrollIntoView") != null) {
                if (document.getElementById("ContentBody__hiddenScrollIntoView").value.length > 0) {
                    var element = document.getElementById(document.getElementById("ContentBody__hiddenScrollIntoView").value);
                    if (element != null) {
                        element.scrollIntoView();
                        element.value = "";
                    }
                }
            }
        });

        function RecalculateLiquidityTotal() {
            if (document.getElementById("ContentPreBody__hiddenNumberOfLiquidityRequirements") != null) {
                var numberOfLiquidityRequirements = parseInt(document.getElementById("ContentPreBody__hiddenNumberOfLiquidityRequirements").value);
                var totalLiquidityAmount = 0.00;
                if (numberOfLiquidityRequirements > 0) {
                    for (i = 1; i <= numberOfLiquidityRequirements; i++) {
                        if (document.getElementById("ContentPreBody__liquidityAmount_" + i) != null) {
                            if (document.getElementById("ContentPreBody__liquidityAmount_" + i).value.length > 0) {
                                var liquidityAmount = parseFloat(document.getElementById("ContentPreBody__liquidityAmount_" + i).value);
                                totalLiquidityAmount += liquidityAmount;
                            }
                        }
                    }
                }

                document.getElementById("_totalLiquidity").innerText = totalLiquidityAmount.toLocaleString(undefined, {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });

                document.getElementById("ContentBody__hiddenCalculatedTotalDeposit").value = document.getElementById("_totalLiquidity").innerText;
            }
        }

        function SetID(pID) {
            if (document.getElementById("ContentBody__hiddenID") != null) {
                document.getElementById("ContentBody__hiddenID").value = pID;
            }
        }
        function CurrencyChanged() {
            if (document.getElementById("ContentPreBody__availableTo") != null) {
                if (document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution") != null) {
                    if (document.getElementById("ContentPreBody__currencyCode") != null) {
                        var comboCurrency = document.getElementById("ContentPreBody__currencyCode");
                        var comboAvailableTo = document.getElementById("ContentPreBody__availableTo");

                        var selectedCurrency = comboCurrency.options[comboCurrency.selectedIndex].innerText;
                        var selectedAvailableTo = comboAvailableTo.options[comboAvailableTo.selectedIndex].innerText;

                        if (selectedCurrency == "GBP") {
                            if (selectedAvailableTo == "Joint Hub Account") {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "170000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "170000.00";
                            }
                            else {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "85000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "85000.00";
                            }
                        }
                        else if (selectedCurrency == "USD") {
                            if (selectedAvailableTo == "Joint Hub Account") {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "200000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "200000.00";
                            }
                            else {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "100000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "100000.00";
                            }
                        }
                        else if (selectedCurrency == 'EUR') {
                            if (selectedAvailableTo == "Joint Hub Account") {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "170000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "170000.00";
                            }
                            else {
                                document.getElementById("ContentPreBody__maximumDepositInAnyOneInstitution").value = "85000.00";
                                document.getElementById("ContentPreBody__fscsValue").value = "85000.00";
                            }
                        }
                    }
                }
            }
        }

        function AvailableToChanged() {
            CurrencyChanged();
        }
        function disableEnterKey(e) {
            var key;
            key = e.keyCode || e.which;
            if (key == 13)
                return false;
            else
                return true;
        }
        function applyClicked() {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "_buttonApply";
            document.forms[0].submit();
        }
        function editRemoveDeposit() {
            if (document.getElementById("ContentBody_" + "_dialogEditDepositGUID") != null)
                removeDeposit(document.getElementById("ContentBody_" + "_dialogEditDepositGUID").value);
        }
        function removeDeposit(depositGUID) {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "removeDeposit";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = depositGUID;
            document.forms[0].submit();
        }
        function submitDeposit() {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "addDeposit";
            document.forms[0].submit();
        }
        function updateDeposit() {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "updateDeposit";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = document.getElementById("ContentBody_" + "_dialogEditDepositGUID").value;
            document.forms[0].submit();
        }
        function openEditDepositDialog(pInstitutionName, pInvestmentTerm, pAER50K, pAER100K, pAER250K, pAmount, pDepositGUID) {
            if (document.getElementById("ContentBody_" + "_dialogEditDepositGUID") != null)
                document.getElementById("ContentBody_" + "_dialogEditDepositGUID").value = pDepositGUID;

            if (document.getElementById("ContentBody_" + "_dialogEditInstitutionName") != null)
                document.getElementById("ContentBody_" + "_dialogEditInstitutionName").innerHTML = pInstitutionName;
            if (document.getElementById("ContentBody_" + "_dialogEditLiquidity") != null)
                document.getElementById("ContentBody_" + "_dialogEditLiquidity").innerHTML = pInvestmentTerm;

            if (document.getElementById("ContentBody_" + "_dialogEditDepositAmount") != null)
                document.getElementById("ContentBody_" + "_dialogEditDepositAmount").value = pAmount;
            if (document.getElementById("_editRateGuageAER50K") != null)
                document.getElementById("_editRateGuageAER50K").innerHTML = pAER50K+'%';
            if (document.getElementById("_editRateGuageAER100K") != null)
                document.getElementById("_editRateGuageAER100K").innerHTML = pAER100K+'%';
            if (document.getElementById("_editRateGuageAER250K") != null)
                document.getElementById("_editRateGuageAER250K").innerHTML = pAER250K+'%';
            editAmountChanged();

            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = "";
            jQuery('#ContentBody_' + 'editDepositDialog').dialog('open');
        }
        function editAmountChanged(){
	        if (document.getElementById("ContentBody__dialogEditDepositAmount") != null) {
	            if (document.getElementById("_paneleditColourFill") != null) {
	                var panelColourFill = document.getElementById("_paneleditColourFill");
				
			        var textAmount = document.getElementById("ContentBody__dialogEditDepositAmount").value;
				
				    panelColourFill.style.width = '0%';
				    if(textAmount.length > 0){			
					    var floatAmount = parseFloat(textAmount);
					    if(floatAmount > 0){
						    if(floatAmount <= 50000){
							    if(floatAmount == 50000){
								    panelColourFill.style.width = '50px';
							    }
							    else{
								    var percentageOfTotal = (floatAmount / 50000);
								    var widthInPixels = 50 * percentageOfTotal;
								    panelColourFill.style.width = Math.round(widthInPixels) + 'px';
							    }
						    }
						    else if(floatAmount <= 100000){
							    if(floatAmount == 100000){
								    panelColourFill.style.width = '100px';
							    }
							    else{
								    var difference = floatAmount - 50000;
								    var percentageOfTotal = (difference / 50000);
								    var widthInPixels = 50 * percentageOfTotal;
								    widthInPixels = 50 + Math.round(widthInPixels)
								    panelColourFill.style.width =  widthInPixels + 'px';
							    }
						    }
						    else if(floatAmount <= 250000){
							    if(floatAmount == 250000){
								    panelColourFill.style.width = '150px';
							    }
							    else{
								    var difference = floatAmount - 100000;
								    var percentageOfTotal = (difference / 150000);
								    var widthInPixels = 50 * percentageOfTotal;
								    widthInPixels = 100 + Math.round(widthInPixels)
								    panelColourFill.style.width =  widthInPixels + 'px';
							    }
						    }
						    else{
							    panelColourFill.style.width = '100%';
						    }
					    }
				    }
			    }
		    }
	    }
        function openAddDepositDialog(pInstitutionID, pInstitutionName, pInvestmentTerm, pAER50K, pAER100K, pAER250K, termCounter) {
            if (document.getElementById("ContentBody_" + "_dialogInstitutionName") != null)
                document.getElementById("ContentBody_" + "_dialogInstitutionName").innerHTML = pInstitutionName;
            if (document.getElementById("ContentBody_" + "_dialogLiquidity") != null)
                document.getElementById("ContentBody_" + "_dialogLiquidity").innerHTML = pInvestmentTerm;

            if (document.getElementById("ContentBody_" + "_dialogDepositAmount") != null)
                document.getElementById("ContentBody_" + "_dialogDepositAmount").value = document.getElementById("ContentPreBody__fscsValue").value;
            if (document.getElementById("_addRateGuageAER50K") != null)
                document.getElementById("_addRateGuageAER50K").innerHTML = pAER50K+'%';
            if (document.getElementById("_addRateGuageAER100K") != null)
                document.getElementById("_addRateGuageAER100K").innerHTML = pAER100K+'%';
            if (document.getElementById("_addRateGuageAER250K") != null)
                document.getElementById("_addRateGuageAER250K").innerHTML = pAER250K+'%';
            amountChanged();

            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = "";
            if (document.getElementById("ContentPreBody_" + "_addDepositForInstitutionID") != null)
                document.getElementById("ContentPreBody_" + "_addDepositForInstitutionID").value = pInstitutionID.toString();
            if (document.getElementById("ContentPreBody_" + "_addDepositForTermIndex") != null)
                document.getElementById("ContentPreBody_" + "_addDepositForTermIndex").value = termCounter.toString();
            jQuery('#ContentBody_' + 'addDepositDialog').dialog('open');
        }
        function amountChanged(){
	        if (document.getElementById("ContentBody__dialogDepositAmount") != null) {
	            if (document.getElementById("_paneladdColourFill") != null) {
	                var panelColourFill = document.getElementById("_paneladdColourFill");
				
			        var textAmount = document.getElementById("ContentBody__dialogDepositAmount").value;
				
				    panelColourFill.style.width = '0%';
				    if(textAmount.length > 0){			
					    var floatAmount = parseFloat(textAmount);
					    if(floatAmount > 0){
						    if(floatAmount <= 50000){
							    if(floatAmount == 50000){
								    panelColourFill.style.width = '50px';
							    }
							    else{
								    var percentageOfTotal = (floatAmount / 50000);
								    var widthInPixels = 50 * percentageOfTotal;
								    panelColourFill.style.width = Math.round(widthInPixels) + 'px';
							    }
						    }
						    else if(floatAmount <= 100000){
							    if(floatAmount == 100000){
								    panelColourFill.style.width = '100px';
							    }
							    else{
								    var difference = floatAmount - 50000;
								    var percentageOfTotal = (difference / 50000);
								    var widthInPixels = 50 * percentageOfTotal;
								    widthInPixels = 50 + Math.round(widthInPixels)
								    panelColourFill.style.width =  widthInPixels + 'px';
							    }
						    }
						    else if(floatAmount <= 250000){
							    if(floatAmount == 250000){
								    panelColourFill.style.width = '150px';
							    }
							    else{
								    var difference = floatAmount - 100000;
								    var percentageOfTotal = (difference / 150000);
								    var widthInPixels = 50 * percentageOfTotal;
								    widthInPixels = 100 + Math.round(widthInPixels)
								    panelColourFill.style.width =  widthInPixels + 'px';
							    }
						    }
						    else{
							    panelColourFill.style.width = '100%';
						    }
					    }
				    }
			    }
		    }
        }
        function Anonymise() {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "Anonymise";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = "";
            document.forms[0].submit();
        }
        function UnAnonymise() {
            if (document.getElementById("ContentPreBody_" + "_userAction") != null)
                document.getElementById("ContentPreBody_" + "_userAction").value = "UnAnonymise";
            if (document.getElementById("ContentPreBody_" + "_userValue") != null)
                document.getElementById("ContentPreBody_" + "_userValue").value = "";
            document.forms[0].submit();
        }
        function jsAnonymise(pTotalDepositRows) {
            var rowIndex = 0;
            var bankIndex = 1;
            for (; rowIndex < pTotalDepositRows; rowIndex++) {
                if (document.getElementById("ContentBody_" + "_depositTable_InstitutionName_" + rowIndex) != null) {
                    document.getElementById("ContentBody_" + "_depositTable_InstitutionName_" + rowIndex).innerText = "Bank " + bankIndex.toString();
                    bankIndex++;
                }
            }
        }
        function generateTemplatedIllustration(pElement) {
            var selection = pElement.options[pElement.selectedIndex].value;
            if(selection != '-1' && selection != '-2')
                window.location = "GenerateTemplatedIllustration.aspx?T=" + selection;
        }
    </script>
    <style>
        .sort-link:link {
            text-decoration: none;
            color:white;
        }

        .sort-link:visited {
            text-decoration: none;
            color:white;
        }

        .sort-link:hover {
            text-decoration: none;
            color:white;
        }

        .sort-link:active {
            text-decoration: none;
            color:white;
        }
        .ui-state-default .ui-icon {
            background-image: url("../../Images/ui-icons_888888_256x240.png");
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentHeaderTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentHeader" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPreBody" runat="server">
<div id="top-header-banner" class="et_pb_section et_pb_section_1 et_pb_with_background et_section_regular" style="background-image:url(https://partner.insigniscash.com/wp-content/uploads/PRC-Top-Banner-Image-projects-page.jpg)!important;">
    <div class="et_pb_row et_pb_row_0">
        <div class="et_pb_column et_pb_column_4_4 et_pb_column_0 et_pb_css_mix_blend_mode_passthrough et-last-child">
            <div class="et_pb_module et_pb_text et_pb_text_0 et_pb_bg_layout_light  et_pb_text_align_left">
                <div class="et_pb_text_inner">
                    <div class="prc-heading">
                        <h1>Cash <strong>Management&nbsp;</strong></h1>
                        <p class="prc-block">Partner Resources</p>
                    </div>
                </div>
            </div> <!-- .et_pb_text -->
        </div> <!-- .et_pb_column -->
    </div> <!-- .et_pb_row -->
</div>
<div class="container">
    <div class="row">
        <div class="span12">
            <div id="_panelNotify" runat="server" style="display:none;"></div>
        </div>
    </div>
    <div class="row">
        <div class="span12" id="_panelTop" runat="server">
        </div>
    </div>
    <div class="row">
        <div class="span6" id="_panelTopLeft" runat="server">
        </div>
        <div class="span6" id="_panelTopRight" runat="server">
        </div>
    </div>
</div>
<div class="row">
    <div class="span12" id="_panelHeatmap" runat="server">
    </div>
</div>
<input type="hidden" id="_userAction" value="" runat="server" />
<input type="hidden" id="_userValue" value="" runat="server" />
<input type="hidden" id="_addDepositForInstitutionID" value="" runat="server" />
<input type="hidden" id="_addDepositForTermIndex" value="" runat="server" />
<input type="hidden" id="_fscsValue" value="85000.00" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentBody" runat="server">
    <div class="row">
        <div class="span7" id="_panelBottomLeft" runat="server">
        </div>
        <div class="span5" id="_panelBottomRight" runat="server">
        </div>
    </div>
    <input type="hidden" id="_hiddenID" value="" runat="server" />
    <input type="hidden" id="_hiddenCalculatedTotalDeposit" value="" runat="server"/>
    <input type="hidden" id="_hiddenShowLiquidityWarningDialog" value="false" runat="server"/>
    <input type="hidden" id="_hiddenScrollIntoView" value="" runat="server"/>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentFooterBottom" runat="server">
    <script type="text/javascript">
        if (document.getElementById("ContentBody__proposedPortfolioTable") != null) {
            new Tablesort(document.getElementById("ContentBody__proposedPortfolioTable"));
        }
    </script>
</asp:Content>
