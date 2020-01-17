function IAMCustomRadioClick(group, subGroup, item, selectedValue) {
    if (document.getElementById("ContentBody_" + group) != null) {
        if (document.getElementById("ContentBody_" + group + "Error") != null) {
            document.getElementById("ContentBody_" + group + "Error").innerHTML = "";
        }
        if (document.getElementById("ContentBody_" + group + subGroup) != null) {
            if (document.getElementById(group + subGroup + item) != null) {
                if (item == 'H' || item == 'M' || item == 'L') {
                    // this is a High, Medium, Low choice
                    document.getElementById(group + subGroup + "H").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "M").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "L").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + item).className = "iam-check-box-selected";
                    document.getElementById("ContentBody_" + group + subGroup).value = selectedValue;

                    if (group == '_priority') {
                        if (document.getElementById("ContentBody_" + group + 'Security') != null) {
                            if (document.getElementById("ContentBody_" + group + 'Rate') != null) {
                                if (document.getElementById("ContentBody_" + group + 'Ease') != null) {
                                    var selectedSecurity = parseInt(document.getElementById("ContentBody_" + group + 'Security').value);
                                    var selectedRate = parseInt(document.getElementById("ContentBody_" + group + 'Rate').value);
                                    var selectedEase = parseInt(document.getElementById("ContentBody_" + group + 'Ease').value);

                                    if (selectedSecurity > 0 && (selectedSecurity == selectedRate || selectedSecurity == selectedEase)) {
                                        document.getElementById("ContentBody_" + group + "Error").innerHTML = "Multiple selection of the same priority are not allowed.<br />";
                                    }
                                    if (selectedRate > 0 && (selectedRate == selectedSecurity || selectedRate == selectedEase)) {
                                        document.getElementById("ContentBody_" + group + "Error").innerHTML = "Multiple selection of the same priority are not allowed.<br />";
                                    }
                                    if (selectedEase > 0 && (selectedEase == selectedSecurity || selectedEase == selectedRate)) {
                                        document.getElementById("ContentBody_" + group + "Error").innerHTML = "Multiple selection of the same priority are not allowed.<br />";
                                    }
                                }
                            }
                        }
                    }
                }
                else if (item == "Y" || item == "N") {
                    // this is a Yes, No choice
                    if (group == '_paperwork') {
                        if (document.getElementById("ContentBody_" + group + 'LikeToSign') != null) {
                            if (document.getElementById("ContentBody_" + group + 'HappyToSign') != null) {
                                if (document.getElementById("ContentBody_" + group + 'SignOnce') != null) {
                                    document.getElementById("ContentBody_" + group + 'LikeToSign').value = "false";
                                    document.getElementById("ContentBody_" + group + 'HappyToSign').value = "false";
                                    document.getElementById("ContentBody_" + group + 'SignOnce').value = "false";
                                }
                            }
                        }
                    } else if (group == '_strength') {
                        if (document.getElementById("ContentBody_" + group + 'OurJudgement') != null) {
                            if (document.getElementById("ContentBody_" + group + 'IncreasedLevel') != null) {
                                if (document.getElementById("ContentBody_" + group + 'OverlyCautious') != null) {
                                    document.getElementById("ContentBody_" + group + 'OurJudgement').value = "false";
                                    document.getElementById("ContentBody_" + group + 'IncreasedLevel').value = "false";
                                    document.getElementById("ContentBody_" + group + 'OverlyCautious').value = "false";
                                }
                            }
                        }
                    } else if (group == '_selectOwn') {
                        if (document.getElementById("ContentBody_" + group + 'WillSelect') != null) {
                            if (document.getElementById("ContentBody_" + group + 'WillNotSelect') != null) {
                                document.getElementById("ContentBody_" + group + 'WillSelect').value = "false";
                                document.getElementById("ContentBody_" + group + 'WillNotSelect').value = "false";
                            }
                        }
                    }

                    document.getElementById(group + subGroup + "Y").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "N").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + item).className = "iam-check-box-selected";
                    document.getElementById("ContentBody_" + group + subGroup).value = selectedValue;

                    if (group == '_paperwork') {
                        if (document.getElementById("ContentBody_" + group + 'LikeToSign') != null) {
                            if (document.getElementById("ContentBody_" + group + 'HappyToSign') != null) {
                                if (document.getElementById("ContentBody_" + group + 'SignOnce') != null) {
                                    var selectedLikeToSign = document.getElementById("ContentBody_" + group + 'LikeToSign').value;
                                    var selectedHappyToSign = document.getElementById("ContentBody_" + group + 'HappyToSign').value;
                                    var selectedSignOnce = document.getElementById("ContentBody_" + group + 'SignOnce').value;

                                    if (selectedLikeToSign == "true") {
                                        document.getElementById("ContentBody_" + group + 'HappyToSign').value = 'false';
                                        document.getElementById(group + 'HappyToSign' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'HappyToSign' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'SignOnce').value = 'false';
                                        document.getElementById(group + 'SignOnce' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'SignOnce' + "N").className = "iam-check-box-selected";
                                    } else if (selectedHappyToSign == "true") {
                                        document.getElementById("ContentBody_" + group + 'LikeToSign').value = 'false';
                                        document.getElementById(group + 'LikeToSign' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'LikeToSign' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'SignOnce').value = 'false';
                                        document.getElementById(group + 'SignOnce' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'SignOnce' + "N").className = "iam-check-box-selected";
                                    } else if (selectedSignOnce == "true") {
                                        document.getElementById("ContentBody_" + group + 'LikeToSign').value = 'false';
                                        document.getElementById(group + 'LikeToSign' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'LikeToSign' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'HappyToSign').value = 'false';
                                        document.getElementById(group + 'HappyToSign' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'HappyToSign' + "N").className = "iam-check-box-selected";
                                    }
                                }
                            }
                        }
                    } else if (group == '_strength') {
                        if (document.getElementById("ContentBody_" + group + 'OurJudgement') != null) {
                            if (document.getElementById("ContentBody_" + group + 'IncreasedLevel') != null) {
                                if (document.getElementById("ContentBody_" + group + 'OverlyCautious') != null) {
                                    var selectedOurJudgement = document.getElementById("ContentBody_" + group + 'OurJudgement').value;
                                    var selectedIncreasedLevel = document.getElementById("ContentBody_" + group + 'IncreasedLevel').value;
                                    var selectedOverlyCautious = document.getElementById("ContentBody_" + group + 'OverlyCautious').value;

                                    if (selectedOurJudgement == "true") {
                                        document.getElementById("ContentBody_" + group + 'IncreasedLevel').value = 'false';
                                        document.getElementById(group + 'IncreasedLevel' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'IncreasedLevel' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'OverlyCautious').value = 'false';
                                        document.getElementById(group + 'OverlyCautious' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'OverlyCautious' + "N").className = "iam-check-box-selected";
                                    } else if (selectedIncreasedLevel == "true") {
                                        document.getElementById("ContentBody_" + group + 'OurJudgement').value = 'false';
                                        document.getElementById(group + 'OurJudgement' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'OurJudgement' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'OverlyCautious').value = 'false';
                                        document.getElementById(group + 'OverlyCautious' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'OverlyCautious' + "N").className = "iam-check-box-selected";
                                    } else if (selectedOverlyCautious == "true") {
                                        document.getElementById("ContentBody_" + group + 'OurJudgement').value = 'false';
                                        document.getElementById(group + 'OurJudgement' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'OurJudgement' + "N").className = "iam-check-box-selected";

                                        document.getElementById("ContentBody_" + group + 'IncreasedLevel').value = 'false';
                                        document.getElementById(group + 'IncreasedLevel' + "Y").className = "iam-check-box-unselected";
                                        document.getElementById(group + 'IncreasedLevel' + "N").className = "iam-check-box-selected";
                                    }
                                }
                            }
                        }
                    } else if (group == '_selectOwn') {
                        if (document.getElementById("ContentBody_" + group + 'WillSelect') != null) {
                            if (document.getElementById("ContentBody_" + group + 'WillNotSelect') != null) {
                                var selectedWillSelect = document.getElementById("ContentBody_" + group + 'WillSelect').value;
                                var selectedWillNotSelect = document.getElementById("ContentBody_" + group + 'WillNotSelect').value;

                                if (selectedWillSelect == "true") {
                                    document.getElementById("ContentBody_" + group + 'WillNotSelect').value = 'false';
                                    document.getElementById(group + 'WillNotSelect' + "Y").className = "iam-check-box-unselected";
                                    document.getElementById(group + 'WillNotSelect' + "N").className = "iam-check-box-selected";
                                } else if (selectedWillNotSelect == "true") {
                                    document.getElementById("ContentBody_" + group + 'WillSelect').value = 'false';
                                    document.getElementById(group + 'WillSelect' + "Y").className = "iam-check-box-unselected";
                                    document.getElementById(group + 'WillSelect' + "N").className = "iam-check-box-selected";
                                }
                            }
                        }
                    }
                } else if (item == 'Existing' || item == 'SaleOfProperty' || item == 'SaleOfBusiness' || item == 'Inheritence' || item == 'Gift' || item == 'Other') {
                    document.getElementById(group + subGroup + "Existing").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "SaleOfProperty").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "SaleOfBusiness").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "Inheritence").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "Gift").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + "Other").className = "iam-check-box-unselected";
                    document.getElementById(group + subGroup + item).className = "iam-check-box-selected";
                    document.getElementById("ContentBody_" + group + subGroup).value = selectedValue;
                }
            }
        }
    }
    if (document.getElementById("ContentBody__hiddenStep") != null) {
        if (document.getElementById("ContentBody__hiddenStep").value == "1") {
            UpdateTriangle();
        }
    }
}

function UpdateTriangle() {
    var calculatedSecurity = 0;
    var calculatedRate = 0;
    var calculatedEase = 0;

    // priority
    if (document.getElementById("ContentBody__priority") != null) {
        if (document.getElementById("ContentBody__priorityError") != null) {
            if (document.getElementById("ContentBody__priorityError").innerHTML == "") {

                var selectedSecurity = parseInt(document.getElementById("ContentBody_" + "_priority" + 'Security').value);
                var selectedRate = parseInt(document.getElementById("ContentBody_" + "_priority" + 'Rate').value);
                var selectedEase = parseInt(document.getElementById("ContentBody_" + "_priority" + 'Ease').value);

                if (selectedSecurity > 0) {
                    calculatedSecurity += selectedSecurity;
                }
                if (selectedRate > 0) {
                    calculatedRate += selectedRate;
                }
                if (selectedEase > 0) {
                    calculatedEase += selectedEase;
                }
            }
        }
    }

    // paperwork
    if (document.getElementById("ContentBody__paperwork") != null) {
        if (document.getElementById("ContentBody__paperworkError") != null) {
            if (document.getElementById("ContentBody__paperworkError").innerHTML == "") {
                var selectedLikeToSign = document.getElementById("ContentBody_" + "_paperwork" + 'LikeToSign').value;
                var selectedHappyToSign = document.getElementById("ContentBody_" + "_paperwork" + 'HappyToSign').value;
                var selectedSignOnce = document.getElementById("ContentBody_" + "_paperwork" + 'SignOnce').value;

                if (selectedLikeToSign == 'true') {
                    calculatedSecurity = calculatedSecurity + 1;
                    calculatedRate = calculatedRate + 1;
                    calculatedEase = calculatedEase + 0;
                } else if (selectedHappyToSign == 'true') {
                    calculatedSecurity = calculatedSecurity + 1;
                    calculatedRate = calculatedRate + 1;
                    calculatedEase = calculatedEase + 1;
                } else if (selectedSignOnce == 'true') {
                    calculatedSecurity = calculatedSecurity + 0;
                    calculatedRate = calculatedRate + 0;
                    calculatedEase = calculatedEase + 2;
                }
            }
        }
    }
    // strength
    if (document.getElementById("ContentBody__strength") != null) {
        if (document.getElementById("ContentBody__strengthError") != null) {
            if (document.getElementById("ContentBody__strengthError").innerHTML == "") {

                var selectedOurJudgement = document.getElementById("ContentBody_" + "_strength" + 'OurJudgement').value;
                var selectedIncreasedLevel = document.getElementById("ContentBody_" + "_strength" + 'IncreasedLevel').value;
                var selectedOverlyCautious = document.getElementById("ContentBody_" + "_strength" + 'OverlyCautious').value;

                if (selectedOurJudgement == 'true') {
                    calculatedSecurity = calculatedSecurity + 0;
                    calculatedRate = calculatedRate + 0;
                    calculatedEase = calculatedEase + 0;
                } else if (selectedIncreasedLevel == 'true') {
                    calculatedSecurity = calculatedSecurity + 1;
                    calculatedRate = calculatedRate + 1;
                    calculatedEase = calculatedEase + 1;
                } else if (selectedOverlyCautious == 'true') {
                    calculatedSecurity = calculatedSecurity + 1;
                    calculatedRate = calculatedRate + 0;
                    calculatedEase = calculatedEase + 0;
                }
            }
        }
    }

    famTriangleInit(calculatedRate, calculatedSecurity, calculatedEase);

    if (document.getElementById("ContentBody__panelYourPreference") != null) {
        document.getElementById("ContentBody__panelYourPreference").innerHTML = "";
        if (calculatedRate > calculatedEase && calculatedRate > calculatedSecurity) {
            // rate is favourite
            document.getElementById("ContentBody__panelYourPreference").innerHTML = "From the answers you have given you may prefer to :<br /><a href='ManagePortfolio.aspx?Ac=" + getParameterByName("Ac") + "'>Build your own bespoke portfolio</a></p>";
        }
        if (calculatedEase > calculatedRate && calculatedEase > calculatedSecurity) {
            // ease of use is favourite
            document.getElementById("ContentBody__panelYourPreference").innerHTML = "From the answers you have given you may prefer to :<br /><a href='ManageGemProducts.aspx?Ac=" + getParameterByName("Ac") + "'>Select one of our gem products</a></p>";
        }
        if (calculatedSecurity > calculatedRate && calculatedSecurity > calculatedEase) {
            // security is favourite
            document.getElementById("ContentBody__panelYourPreference").innerHTML = "From the answers you have given you may prefer to discuss with us how to optimise the security of your investments.</p>";
        }
    }
}

function getParameterByName(key) {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars[key];
}

function IAMLinkedAccountRadioButtonClick(selectedIndex, selectedValue) {
    if(document.getElementById("ContentBody__totalLinkedAccountChoices") != null){
        var totalLinkedAccountChoices = parseInt(document.getElementById("ContentBody__totalLinkedAccountChoices").value);
        if (totalLinkedAccountChoices > 1) {
            var index = 1;
            for (i = 0; i < totalLinkedAccountChoices; i++) {
                if (document.getElementById("ContentBody__linkedAccount" + (index+i)) != null) {
                    if ((index+i) == selectedIndex) {
                        document.getElementById("ContentBody__linkedAccount" + (index + i)).className = "iam-radio-selected";
                        document.getElementById("ContentBody__selectedLinkedAccountID").value = selectedValue;
                    }
                    else {
                        document.getElementById("ContentBody__linkedAccount" + (index+i)).className = "iam-radio-unselected";
                    }
                }
            }
        }
    }
}
