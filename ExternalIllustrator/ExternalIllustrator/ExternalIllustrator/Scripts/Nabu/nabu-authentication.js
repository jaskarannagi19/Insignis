var nabuAuthenticationUserAccountSession = null;

var nabuAuthenticationRedirectOnLogin = "";
var nabuAuthenticationRedirectOnPasswordExpired = "";

function nabuAuthenticationLogin(pUserAccountName, pUserAccountPassword, pUIPanel, pUIErrorMessage) {
    uiFrameworkPanel = pUIPanel;
    uiFrameworkErrorMessage = pUIErrorMessage;

    nabuFrameworkErrorsDetected = false;
    nabuFrameworkErrorDetails = "";

    var parameters = [];
    parameters.push({ Key: "accountname", Value: pUserAccountName });
    parameters.push({ Key: "password", Value: pUserAccountPassword });
    parameters.push({ Key: "languageid", Value: nabuGlobalisationSelectedLanguage.LanguageID.toString() });

    $.ajax({
        cache: false,
        type: "GET",
        async: false,
        url: nabuFrameworkGenerateCallUrl("Authentication", "Login", parameters),
        contentType: "application/json; charset=utf-8",
        dataType: nabuApiResponseFormat.toLowerCase(),
        processData: true,
        error: function (result) {
            if (result.statusText == 'success') {
                if (nabuAuthenticationUserAccountSession.ErrorCode != null && nabuAuthenticationUserAccountSession.ErrorCode.Detail != null && nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias != null && nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias.length > 0) {
                    if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_PASSWORD_EXPIRED") {
                        // need to store the user session in a cookie
                        window.location = nabuAuthenticationRedirectOnPasswordExpired;
                    } else {

                        if (document.getElementById(uiFrameworkPanel) != null)
                            document.getElementById(uiFrameworkPanel).style.display = 'inline';

                        nabuFrameworkErrorsDetected = true;
                        if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_LICENSE_EXPIRED")
                            nabuFrameworkErrorDetails = 'The license associated with this user account has expired.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_LICENSE_INACTIVE")
                            nabuFrameworkErrorDetails = 'The license associated with this user account is inactive.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNTSESSION_CREATE")
                            nabuFrameworkErrorDetails = 'Unable to create user account session.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_LICENSE_INVALID")
                            nabuFrameworkErrorDetails = 'The license associated with this user account is invalid.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_LOGIN_ATTEMPTS_EXCEEDED")
                            nabuFrameworkErrorDetails = 'Your account is locked because the number of invalid attempts has exceeded tolerance.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_INACTIVE")
                            nabuFrameworkErrorDetails = 'This user account is inactive, please contact support to reactivate.';
                        else if (nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_PASSWORD_INVALID" || nabuAuthenticationUserAccountSession.ErrorCode.Detail.Alias == "ERR_USERACCOUNT_NAME_INVALID")
                            nabuFrameworkErrorDetails = 'Invalid username or password entered, are you a registered or perhaps you have forgotten your credentials.';
                        else
                            nabuFrameworkErrorDetails = 'Unknown error occured when logging in.';

                        if (document.getElementById(uiFrameworkErrorMessage) != null)
                            document.getElementById(uiFrameworkErrorMessage).innerHTML = nabuFrameworkErrorDetails;
                    }
                }
                else {
                    // need to store the user session in a cookie
                    window.location = nabuAuthenticationRedirectOnLogin;
                }
            } else {
                nabuFrameworkErrorsDetected = true;
                nabuFrameworkErrorDetails = 'AJAX error - ' + result.status + ' ' + result.statusText;

                if (document.getElementById(uiFrameworkErrorMessage) != null)
                    document.getElementById(uiFrameworkErrorMessage).innerHTML = nabuFrameworkErrorDetails;

                if (document.getElementById(uiFrameworkPanel) != null)
                    document.getElementById(uiFrameworkPanel).style.display = 'inline';
            }
        }
    });
}

