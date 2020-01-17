function dcfConditionalDisplayRadioTrigger(pTriggerElementID) {
    if (document.getElementsByName("ctl00$ContentBody$" + pTriggerElementID) != null) {
        var triggerElements = document.getElementsByName("ctl00$ContentBody$" + pTriggerElementID);

        var triggerElementValue = "";
        for (var i = 0; i < triggerElements.length; i++) {
            if (triggerElements[i].checked)
                triggerElementValue = triggerElements[i].value;
        }

        if (triggerElementValue != null) {
            var formElements = document.getElementById('form1').elements;
            for (var i = 0; i < formElements.length; i++) {
                //if (elem[i].localName == 'input') {

                var currentElement = formElements[i];

                    // does this node have our special display where variable attribute set
                    // does this node have our special display where variable value attribute
                    if (currentElement.hasAttribute("conditionalDisplayWhereVariable") && currentElement.hasAttribute("conditionalDisplayWhereValue")) {
                        var attributeConditionalDisplayWhereVariable = currentElement.getAttribute("conditionalDisplayWhereVariable");
                        var attributeConditionalDisplayWhereValue = currentElement.getAttribute("conditionalDisplayWhereValue");
                        // if this attribute variable name matches the one we have triggered from
                        if (attributeConditionalDisplayWhereVariable == pTriggerElementID) {

                            // if this value is the same as the trigger value
                            if (attributeConditionalDisplayWhereValue == triggerElementValue) {
                                // ensure the node is visible
                                currentElement.style.visibility = 'visible';

                                if (document.getElementById(currentElement.id + "_span") != null) {
                                    document.getElementById(currentElement.id + "_span").style.visibility = 'visible';
                                }
                                if (document.getElementById(currentElement.id + "_Month") != null) {
                                    document.getElementById(currentElement.id + "_Month").style.visibility = 'visible';
                                }
                                if (document.getElementById(currentElement.id + "_Year") != null) {
                                    document.getElementById(currentElement.id + "_Year").style.visibility = 'visible';
                                }
                            }
                            else {
                                // hide the node
                                currentElement.style.visibility = 'hidden';

                                if (document.getElementById(currentElement.id + "_span") != null) {
                                    document.getElementById(currentElement.id + "_span").style.visibility = 'hidden';
                                }
                                if (document.getElementById(currentElement.id + "_Month") != null) {
                                    document.getElementById(currentElement.id + "_Month").style.visibility = 'hidden';
                                }
                                if (document.getElementById(currentElement.id + "_Year") != null) {
                                    document.getElementById(currentElement.id + "_Year").style.visibility = 'hidden';
                                }
                            }
                        }
                    }
                //}
            } 
        }
    }
    return true;
}
function dcfConditionalDisplayTrigger(pTriggerElementID) {
    if (document.getElementById("ContentBody_" + pTriggerElementID) != null) {
        var triggerElement = document.getElementById("ContentBody_" + pTriggerElementID);

        var triggerElementValue = triggerElement.value;
        if (triggerElementValue != null) {
            var elem = document.getElementById('form1').elements;
            for (var i = 0; i < elem.length; i++) {
                //if (elem[i].localName == 'input') {

                    var currentElement = elem[i];

                    // does this node have our special display where variable attribute set
                    // does this node have our special display where variable value attribute
                    if (currentElement.hasAttribute("conditionalDisplayWhereVariable") && currentElement.hasAttribute("conditionalDisplayWhereValue")) {
                        var attributeConditionalDisplayWhereVariable = currentElement.getAttribute("conditionalDisplayWhereVariable");
                        var attributeConditionalDisplayWhereValue = currentElement.getAttribute("conditionalDisplayWhereValue");
                        // if this attribute variable name matches the one we have triggered from
                        if (attributeConditionalDisplayWhereVariable == pTriggerElementID) {

                            // if this value is the same as the trigger value
                            if (attributeConditionalDisplayWhereValue == triggerElementValue) {
                                // ensure the node is visible
                                currentElement.style.visibility = 'visible';

                                if (document.getElementById(currentElement.id + "_span") != null) {
                                    document.getElementById(currentElement.id + "_span").style.visibility = 'visible';
                                }
                                if (document.getElementById(currentElement.id + "_Month") != null) {
                                    document.getElementById(currentElement.id + "_Month").style.visibility = 'visible';
                                }
                                if (document.getElementById(currentElement.id + "_Year") != null) {
                                    document.getElementById(currentElement.id + "_Year").style.visibility = 'visible';
                                }
                            }
                            else {
                                // hide the node
                                currentElement.style.visibility = 'hidden';

                                if (document.getElementById(currentElement.id + "_span") != null) {
                                    document.getElementById(currentElement.id + "_span").style.visibility = 'hidden';
                                }
                                if (document.getElementById(currentElement.id + "_Month") != null) {
                                    document.getElementById(currentElement.id + "_Month").style.visibility = 'hidden';
                                }
                                if (document.getElementById(currentElement.id + "_Year") != null) {
                                    document.getElementById(currentElement.id + "_Year").style.visibility = 'hidden';
                                }
                            }
                        }
                    }
                //}
            }
        }
    }
    return true;
}

function dcfDateRangeSelectionChanged(pHiddenElementID) {
    if (document.getElementById("ContentBody_" + pHiddenElementID) != null) {
        if (document.getElementById("ContentBody_" + pHiddenElementID + "_Month") != null) {
            if (document.getElementById("ContentBody_" + pHiddenElementID + "_Year") != null) {
                hiddenElement = document.getElementById("ContentBody_" + pHiddenElementID);
                comboMonth = document.getElementById("ContentBody_" + pHiddenElementID + "_Month");
                comboYear = document.getElementById("ContentBody_" + pHiddenElementID + "_Year");

                hiddenElement.value = comboMonth.value + "-" + comboYear.value;
            }
        }
    }
    return true;
}
function dcfDateRangeSelectionChangedWithCondition(pHiddenElementID, pTriggerElementID, pMinimumYears) {
    if (document.getElementById("ContentBody_" + pHiddenElementID) != null) {
        if (document.getElementById("ContentBody_" + pHiddenElementID + "_Month") != null) {
            if (document.getElementById("ContentBody_" + pHiddenElementID + "_Year") != null) {
                hiddenElement = document.getElementById("ContentBody_" + pHiddenElementID);
                comboMonth = document.getElementById("ContentBody_" + pHiddenElementID + "_Month");
                comboYear = document.getElementById("ContentBody_" + pHiddenElementID + "_Year");

                hiddenElement.value = comboMonth.value + "-" + comboYear.value;

                var today = new Date();
                if ((today.getFullYear() - parseInt(comboYear.value)) < pMinimumYears){
                    if (document.getElementById("ContentBody_" + pTriggerElementID + "_panel") != null) {
                        document.getElementById("ContentBody_" + pTriggerElementID + "_panel").style.visibility = "visible";
                    }
                    if (document.getElementById("ContentBody_" + pTriggerElementID) != null) {
                        var numberRegistered = 0;
                        if (document.getElementById("ContentBody_" + pTriggerElementID).value.length > 0) {
                            numberRegistered = parseInt(document.getElementById("ContentBody_" + pTriggerElementID).value);
                        }
                        if (numberRegistered == 0) {
                        }
                    }
                }
                else {
                    if (document.getElementById("ContentBody_" + pTriggerElementID + "_panel") != null) {
                        document.getElementById("ContentBody_" + pTriggerElementID + "_panel").style.visibility = "collapse";
                    }
                }
            }
        }
    }
    return true;
}

function dcfTextDateDDMMYYYYValidate(pElementID, pMinimumLength) {
    if (document.getElementById("ContentBody_" + pElementID) != null) {
        var hasErrors = false;

        // dd/mm/yyyy
        var textValue = document.getElementById("ContentBody_" + pElementID).value;
        if (pMinimumLength !== -1 && textValue.length == pMinimumLength) {
            var today = new Date();

            if (textValue.substring(2, 3) === '/' && textValue.substring(5, 6) === '/') {
                var parts = textValue.split('/');
                if (parts.length === 3) {
                    var nYear = parseInt(parts[2]);
                    if (nYear > 0 && nYear <= today.getFullYear()) {
                        var isLeapYear = (nYear % 100 === 0) ? (nYear % 400 === 0) : (nYear % 4 === 0);

                        var nMonth = parseInt(parts[1]);
                        if (nMonth > 0 && nMonth < 13) {
                            //                 J,  F,  M,  A,  M,  J,  J,  A,  S,  O,  N,  D
                            var daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

                            var maxDaysInMonth = daysInMonth[nMonth - 1];
                            if (isLeapYear && nMonth === 2)
                                maxDaysInMonth = maxDaysInMonth + 1;

                            var nDay = parseInt(parts[0]);

                            if (nDay > 0 && nDay <= maxDaysInMonth) {
                                // we are all good!!!
                            }
                            else {
                                hasErrors = true;
                            }
                        }
                        else {
                            hasErrors = true;
                        }
                    }
                    else {
                        hasErrors = true;
                    }
                }
                else {
                    hasErrors = true;
                }
            }
            else {
                hasErrors = true;
            }
        }
        else {
            hasErrors = true;
        }

        if (hasErrors) {
            document.getElementById("ContentBody_" + pElementID).style.border = '1px solid red';
        }
        else {
            document.getElementById("ContentBody_" + pElementID).style.border = '1px solid #e0e0e0';
        }
    }
}

function dcfMaskedTextValidate(pElementID, pMask, pMinLength, pMaxLength) {
    if (document.getElementById("ContentBody_" + pElementID) != null) {
        var hasErrors = false;

        var fieldData = document.getElementById("ContentBody_" + pElementID).value;

        if (pMinLength !== -1 && fieldData.length < pMinLength) {
            hasErrors = true;
        }
        else {
            if (pMaxLength !== -1 && fieldData.length > pMaxLength) {
                hasErrors = true;
            }
            else {
                if (pMask.length > 0) {
                    if (pMinLength === -1 && pMaxLength === -1) {
                        if (fieldData.length != pMask.length)
                            hasErrors = true;
                    }
                }
            }
        }

        if (hasErrors) {
            document.getElementById("ContentBody_" + pElementID).style.border = '1px solid red';
        }
        else {
            document.getElementById("ContentBody_" + pElementID).style.border = '1px solid #e0e0e0';
        }
    }
}

function dcfAlertIfSame(pElementOneID, pElementTwoID,pAlertBoxMessage) {
    if (document.getElementById("ContentFooter__hasValidationErrors") != null) {
        document.getElementById("ContentFooter__hasValidationErrors").value = 'false';
    }
    if (document.getElementById("ContentBody_" + pElementOneID) != null) {
        if (document.getElementById("ContentBody_" + pElementTwoID) != null) {
            var elementOne = document.getElementById("ContentBody_" + pElementOneID);
            var elementTwo = document.getElementById("ContentBody_" + pElementTwoID);

            elementOne.style.border = '1px solid #e0e0e0';
            elementTwo.style.border = '1px solid #e0e0e0';

            if (elementOne.style.visibility === undefined || elementOne.style.visibility === '')
                elementOne.style.visibility = 'visible';
            if (elementTwo.style.visibility === undefined || elementTwo.style.visibility === '')
                elementTwo.style.visibility = 'visible';

            if (elementOne.style.visibility === 'visible' &&
                elementTwo.style.visibility === 'visible') {

                var elementOneValue = document.getElementById("ContentBody_" + pElementOneID).value;
                var elementTwoValue = document.getElementById("ContentBody_" + pElementTwoID).value;

                if (elementOneValue === elementTwoValue) {
                    document.getElementById("ContentBody_" + pElementOneID).style.border = '1px solid red';
                    document.getElementById("ContentBody_" + pElementTwoID).style.border = '1px solid red';
                    if (pAlertBoxMessage != null && pAlertBoxMessage.length > 0) {
                        if (document.getElementById("ContentFooter__hasValidationErrors") != null) {
                            document.getElementById("ContentFooter__hasValidationErrors").value = 'true';
                        }
                        alert(pAlertBoxMessage);
                    }
                }
            }
        }
    }
    return true;
}
function dcfAlertIfDifferent(pElementOneID, pElementTwoID, pAlertBoxMessage) {
    if (document.getElementById("ContentFooter__hasValidationErrors") != null) {
        document.getElementById("ContentFooter__hasValidationErrors").value = 'false';
    }
    if (document.getElementById("ContentBody_" + pElementOneID) != null) {
        if (document.getElementById("ContentBody_" + pElementTwoID) != null) {
            if (document.getElementById("ContentBody_" + pElementOneID).style.visibility === 'visible' &&
                document.getElementById("ContentBody_" + pElementTwoID).style.visibility === 'visible') {

                var elementOneValue = document.getElementById("ContentBody_" + pElementOneID).value;
                var elementTwoValue = document.getElementById("ContentBody_" + pElementTwoID).value;

                if (elementOneValue !== elementTwoValue) {
                    document.getElementById("ContentBody_" + pElementOneID).style.border = '1px solid red';
                    document.getElementById("ContentBody_" + pElementTwoID).style.border = '1px solid red';
                    if (pAlertBoxMessage != null && pAlertBoxMessage.length > 0) {
                        if (document.getElementById("ContentFooter__hasValidationErrors") != null) {
                            document.getElementById("ContentFooter__hasValidationErrors").value = 'true';
                        }
                        alert(pAlertBoxMessage);
                    }
                }
                else {
                    document.getElementById("ContentBody_" + pElementOneID).style.border = '1px solid #e0e0e0';
                    document.getElementById("ContentBody_" + pElementTwoID).style.border = '1px solid #e0e0e0';
                }
            }
        }
    }
    return true;
}

function dcfMandatoryFieldValidation() {
    var elem = document.getElementById('form1').elements;
    var alertUser = false;

    // check mandatory elements
    for (var i = 0; i < elem.length; i++) {
        var currentElement = elem[i];
        if (currentElement.hasAttribute("mandatory")){
            if (currentElement.style.visibility !== 'hidden') {
                if (currentElement.value.length > 0) {
                    document.getElementById("ContentBody_" + pElementOneID).style.border = '1px solid #e0e0e0';
                }
                else {
                    document.getElementById("ContentBody_" + pElementTwoID).style.border = '1px solid red';
                    alertUser = true;
                }
            }
        }
    }

    // check mandatory groups elements
    for (var i = 0; i < elem.length; i++) {
        var currentElement = elem[i];
        if (currentElement.hasAttribute("mandatorygroup")) {
            if (currentElement.style.visibility !== 'hidden') {
                var groupName = currentElement.getAttribute("mandatorygroup");
                var groupValue = "";
                for (var j = 0; j < elem.length; j++) {
                    var loopElement = elem[j];
                    if (loopElement.hasAttribute("mandatorygroup")) {
                        if (loopElement.style.visibility !== 'hidden') {
                            if (loopElement.getAttribute("mandatorygroup") === groupName) {
                                if (loopElement.checked === true) {       // check boxes
                                    groupValue = groupValue + loopElement.value;
                                }
                            }
                        }
                    }
                }
                if (groupValue.length > 0) {
                    for (var j = 0; j < elem.length; j++) {
                        var loopElement = elem[j];
                        if (loopElement.hasAttribute("mandatorygroup")) {
                            if (loopElement.style.visibility !== 'hidden') {
                                if (loopElement.getAttribute("mandatorygroup") === groupName) {
                                    loopElement.parentElement.style.border = 'none';
                                }
                            }
                        }
                    }
                }
                else {
                    for (var j = 0; j < elem.length; j++) {
                        var loopElement = elem[j];
                        if (loopElement.hasAttribute("mandatorygroup")) {
                            if (loopElement.style.visibility !== 'hidden') {
                                if (loopElement.getAttribute("mandatorygroup") === groupName) {
                                    loopElement.parentElement.style.border = '1px solid red';
                                }
                            }
                        }
                    }
                    alertUser = true;
                }
            }
        }
    }

    if (alertUser == true) {
        alert("Mandatory fields have not been completed, those fields are highlighted, please correct before continuing.");
        return false;
    }
    else
        return true;
}

function dcfValidateForm() {
    var alertUser = false;

    if (document.getElementById("ContentFooter__hasValidationErrors") != null) {
        if (document.getElementById("ContentFooter__hasValidationErrors").value === 'true') {
            alertUser = true;
        }
    }

    if (alertUser == true) {
        alert("There outstanding validation errors on this page, please correct.");
        return false;
    }
    else {
        if (document.getElementById("ContentFooter__performMandatoryChecks") != null) {
            if (document.getElementById("ContentFooter__performMandatoryChecks").value === 'true') {
                return dcfMandatoryFieldValidation();
            }
            else
                return true;
        }
        else
            return true;
    }
}

function dcfCopyAddress(pCheckBoxElement, pFromAddressPrefix, pToAddressPrefix) {
    if (pCheckBoxElement.checked == true) {
        if (document.getElementById("ContentBody_" + pFromAddressPrefix + "Line1") != null && document.getElementById("ContentBody_" + pToAddressPrefix + "Line1") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line1").value = document.getElementById("ContentBody_" + pFromAddressPrefix + "Line1").value;
        }
        if (document.getElementById("ContentBody_" + pFromAddressPrefix + "Line2") != null && document.getElementById("ContentBody_" + pToAddressPrefix + "Line2") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line2").value = document.getElementById("ContentBody_" + pFromAddressPrefix + "Line2").value;
        }
        if (document.getElementById("ContentBody_" + pFromAddressPrefix + "Line3") != null && document.getElementById("ContentBody_" + pToAddressPrefix + "Line3") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line3").value = document.getElementById("ContentBody_" + pFromAddressPrefix + "Line3").value;
        }
        if (document.getElementById("ContentBody_" + pFromAddressPrefix + "PostCode") != null && document.getElementById("ContentBody_" + pToAddressPrefix + "PostCode") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "PostCode").value = document.getElementById("ContentBody_" + pFromAddressPrefix + "PostCode").value;
        }
    } else {
        if (document.getElementById("ContentBody_" + pToAddressPrefix + "Line1") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line1").value = '';
        }
        if (document.getElementById("ContentBody_" + pToAddressPrefix + "Line2") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line2").value = '';
        }
        if (document.getElementById("ContentBody_" + pToAddressPrefix + "Line3") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "Line3").value = '';
        }
        if (document.getElementById("ContentBody_" + pToAddressPrefix + "PostCode") != null) {
            document.getElementById("ContentBody_" + pToAddressPrefix + "PostCode").value = '';
        }
    }
    return true;
}

function dcfInputTableAddRowClicked(pInputTableID) {
    if (document.getElementById("ContentBody_" + pInputTableID + "Table") != null) {
        if (document.getElementById("ContentBody_" + pInputTableID) != null) {
            // get a reference to the hidden row counter
            var hiddenRowCounter = document.getElementById("ContentBody_" + pInputTableID);

            // get a reference to the table element
            var tableReference = document.getElementById("ContentBody_" + pInputTableID + "Table");

            // does the table have a TBODY defined?
            var hasTBody = false
            if (tableReference.tBodies.length > 0)
                hasTBody = true;

            // if no TBODY defined, add on
            if (hasTBody == false)
                tableReference.appendChild(document.createElement("tbody"));

            // get a reference to the table body
            var tBodyReference = tableReference.getElementsByTagName('tbody')[0];

            // add an empty row the table body
            var newRow = tBodyReference.insertRow(tBodyReference.rows.length);
            tBodyReference.rows[tBodyReference.rows.length - 1].id = guid();

            // increment the hidden row counter variable
            var newCounter = parseInt(hiddenRowCounter.value) + 1
            hiddenRowCounter.value = newCounter.toString();

            // get a reference to the table header
            var tHeadReference = tableReference.getElementsByTagName('thead')[0];

            // loop through each cell in the header row
            for (cellCount = 0; cellCount < tHeadReference.rows[0].cells.length; cellCount++) {
                // Insert a cell into the new row
                var newCell = newRow.insertCell(cellCount);

                if (tHeadReference.rows[0].cells[cellCount].hasAttribute("variable-suffix")) {
                    // Append a text node to the cell
                    var inputVariable = document.createElement("input");
                    inputVariable.type = "text";
                    inputVariable.id = "ContentBody_" + pInputTableID + tHeadReference.rows[0].cells[cellCount].getAttribute("variable-suffix").replace("#", tBodyReference.rows[tBodyReference.rows.length - 1].id);
                    inputVariable.name = "ctl00$ContentBody$" + pInputTableID + tHeadReference.rows[0].cells[cellCount].getAttribute("variable-suffix").replace("#", tBodyReference.rows[tBodyReference.rows.length - 1].id);
                    inputVariable.style.width = "100%";
                    newCell.appendChild(inputVariable);
                }
                else {
                    var buttonDeleteRow = document.createElement("div");
                    buttonDeleteRow.className = "btn";
                    buttonDeleteRow.innerHTML = "<i class='fa fa-trash'></i>";
                    buttonDeleteRow.style.cursor = "pointer";
                    buttonDeleteRow.style.margin = "0px";
                    buttonDeleteRow.title = "Delete Row";
                    buttonDeleteRow.ID = "button" + tBodyReference.rows[tBodyReference.rows.length - 1].id;
                    buttonDeleteRow.addEventListener("click", function () { dcfInputTableDeleteRowClicked(this, pInputTableID); });
                    newCell.appendChild(buttonDeleteRow);
                }
            }
        }
    }
    return true;
}

function dcfInputTableDeleteRowClicked(evt, pInputTableID) {
    if (document.getElementById("ContentBody_" + pInputTableID + "Table") != null) {
        if (document.getElementById("ContentBody_" + pInputTableID) != null) {
            // get a reference to the hidden row counter
            var hiddenRowCounter = document.getElementById("ContentBody_" + pInputTableID);

            // get a reference to the table element
            var tableReference = document.getElementById("ContentBody_" + pInputTableID + "Table");

            // get a reference to the table body
            var tBodyReference = tableReference.getElementsByTagName('tbody')[0];

            // iterate through each row until we find the one in question
            for (i = 0; i < tBodyReference.rows.length; i++) {
                // substring 6 because we have the 'button' prefix on the ID
                if (tBodyReference.rows[i].id === evt.ID.substring(6)) {
                    tBodyReference.deleteRow(i);
                    break;
                }
            }

            // decrement the hidden row counter variable
            var newCounter = parseInt(hiddenRowCounter.value) - 1
            hiddenRowCounter.value = newCounter.toString();
        }
    }
    return true;
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}