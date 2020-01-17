var nabuGlobalisationSelectedLanguage = null;

function nabuGlobalisationGetLanguageBySystemName(pSystemName, pUIPanel, pUIErrorMessage) {
    uiFrameworkPanel = pUIPanel;
    uiFrameworkErrorMessage = pUIErrorMessage;
    
    nabuFrameworkErrorsDetected = false;
    nabuFrameworkErrorDetails = "";

    var parameters = [];
    parameters.push({ Key: "systemName", Value: pSystemName });

    $.ajax({
        cache: false,
        type: "GET",
        async: false,
        url: nabuFrameworkGenerateCallUrl("Globalisation", "GetLanguageBySystemName", parameters),
        contentType: "application/json; charset=utf-8",
        dataType: nabuApiResponseFormat.toLowerCase(),
        processData: true,
        error: function (result) {
            if (result.statusText == 'success') {
                if (document.getElementById(uiFrameworkPanel) != null)
                    document.getElementById(uiFrameworkPanel).style.display = 'block';
            } else {
                nabuFrameworkErrorsDetected = true;
                nabuFrameworkErrorDetails = 'AJAX error - ' + result.status + ' ' + result.statusText;

                if (document.getElementById(uiFrameworkErrorMessage) != null)
                    document.getElementById(uiFrameworkErrorMessage).innerHTML = nabuFrameworkErrorDetails;
            }
        }
    });
}
