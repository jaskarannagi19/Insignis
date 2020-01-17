class IntroducerOrganisation {
    constructor(pPartyID, pName, pIntroducers) {
        this.PartyID = pPartyID;
        this.Name = pName;
        this.Introducers = pIntroducers;
    }
}

class Introducer{
    constructor(pPartyID, pName) {
        this.PartyID = pPartyID;
        this.Name = pName;
    }
}

var _introducerDialogCurrentSelectedIntroducerID = -1;
var _introducerDialogCurrentSelectedIntroducerName = "";

function IntroducerDialogRefresh(selectedIntroducerID) {
    if (document.getElementById("_introducerDialogOrganisationListView") != null) {
        if (document.getElementById("_introducerDialogIntroducersListView") != null) {
            document.getElementById("_introducerDialogOrganisationListView").innerHTML = "";
            document.getElementById("_introducerDialogIntroducersListView").innerHTML = "";

            var selectedOrganisationID = 0;
            for (organisationIndex = 0; organisationIndex < _introducerDialogData.length; organisationIndex++) {
                var introducerOrganisation = _introducerDialogData[organisationIndex];
                for (introducerIndex = 0; introducerIndex < introducerOrganisation.Introducers.length; introducerIndex++) {
                    var introducer = introducerOrganisation.Introducers[introducerIndex];
                    if (introducer.PartyID == selectedIntroducerID) {
                        selectedOrganisationID = introducerOrganisation.PartyID;
                        break;
                    }
                }
                if (selectedOrganisationID != 0) {
                    break;
                }
            }

            for (organisationIndex = 0; organisationIndex < _introducerDialogData.length; organisationIndex++) {
                var introducerOrganisation = _introducerDialogData[organisationIndex];
                var organisationRowHTML = "<p style='width:100%; margin:0px; padding: 0px; cursor: pointer;";
                if (introducerOrganisation.PartyID == selectedOrganisationID) {
                    organisationRowHTML += "background-color: #ffa64d; color:white;";

                    for (introducerIndex = 0; introducerIndex < introducerOrganisation.Introducers.length; introducerIndex++) {
                        var introducer = introducerOrganisation.Introducers[introducerIndex];
                        var introducerRowHTML = "<p style='width: 100%; margin:0px; padding: 0px; cursor: pointer;";

                        if (introducer.PartyID == selectedIntroducerID) {
                            introducerRowHTML += "background-color: #ffa64d; color:white;";
                            _introducerDialogCurrentSelectedIntroducerID = selectedIntroducerID;
                            _introducerDialogCurrentSelectedIntroducerName = introducer.Name;
                            if (_introducerDialogCurrentSelectedIntroducerName == 'No specific introducer') {
                                _introducerDialogCurrentSelectedIntroducerName = introducerOrganisation.Name;
                            }
                        }
                        introducerRowHTML += "' onclick='IntroducerDialogRefresh(" + introducer.PartyID + ");'>" + introducer.Name + "</p>";
                        document.getElementById("_introducerDialogIntroducersListView").innerHTML += introducerRowHTML;
                    }
                }
                organisationRowHTML += "' onclick='IntroducerDialogRefresh(" + introducerOrganisation.PartyID + ");'>" + introducerOrganisation.Name + "</p>";

                document.getElementById("_introducerDialogOrganisationListView").innerHTML += organisationRowHTML;
            }
        }
    }
}

function IntroducerDialogChoose() {
    jQuery("#ContentBody__selectIntroducerDialog").dialog("close");

    if (document.getElementById("ContentBody__clientIntroducer") != null) {
        if (document.getElementById("ContentBody__clientIntroducerView") != null) {
            document.getElementById("ContentBody__clientIntroducer").value = _introducerDialogCurrentSelectedIntroducerID.toString();
            document.getElementById("ContentBody__clientIntroducerView").innerText = _introducerDialogCurrentSelectedIntroducerName;
        }
    }
}
