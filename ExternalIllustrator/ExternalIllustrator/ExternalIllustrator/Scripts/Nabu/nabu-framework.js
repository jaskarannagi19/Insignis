var nabuApiRoot = "https://localhost:8071";
var nabuApiAccessKey = "b5add981-6d54-4745-95eb-4baeed0959a9";
var nabuApiLicensedTo = "Octavo%20Gate";
var nabuApiResponseFormat = "JSONP";

var uiFrameworkPanel = "";
var uiFrameworkErrorMessage = "";

var nabuFrameworkErrorsDetected = false;
var nabuFrameworkErrorDetails = "";
var nabuFrameworkCurrentMethod = "";

function nabuFrameworkInitialise(apiRoot, apiAccessKey, apiLicensedTo, isCrossDomain) {
    nabuApiRoot = apiRoot;

    nabuApiAccessKey = apiAccessKey;

    nabuApiLicensedTo = apiLicensedTo.replace(" ","%20");

    if (isCrossDomain == true)
        nabuApiResponseFormat = "JSONP";
    else
        nabuApiResponseFormat = "JSON";
}

function nabuFrameworkGenerateCallUrl(apiService, apiMethod, apiParameters) {
    var url = nabuApiRoot;

    if (endsWith(url,"/") == false)
        url += "/";

    url += apiService;

    if (endsWith(url,"/") == false)
        url += "/";

    url += apiMethod;
    nabuFrameworkCurrentMethod = apiMethod;

    var parameterCounter = 0;
    if (apiParameters.length > 0)
    {
        url += "?";
        for (i = 0; i < apiParameters.length; i++)
        {
            if (parameterCounter > 0)
                url += "&";

            url += apiParameters[i].Key;

            url += "=";

            url += apiParameters[i].Value;
            parameterCounter++;
        }
    }

    if (parameterCounter == 0)
        url += "?";

    if (parameterCounter > 0)
        url += "&";

    url += "apikey=" + nabuApiAccessKey;
    url += "&apilicensedto=" + nabuApiLicensedTo;
    url += "&format=" + nabuApiResponseFormat;
    return url;
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function jsonCallback(pObject) {
    if (pObject.ErrorsDetected == false) {
        if (nabuFrameworkCurrentMethod == "GetLanguageBySystemName") {
            nabuGlobalisationSelectedLanguage = pObject;
        } else if (nabuFrameworkCurrentMethod == "Login") {
            nabuAuthenticationUserAccountSession = pObject;
        }
    } else {
        if (nabuFrameworkCurrentMethod == "Login")
            nabuAuthenticationUserAccountSession = pObject;

        nabuFrameworkErrorsDetected = true;
        nabuFrameworkErrorDetails = pObject.ErrorDetails[0].ErrorMessage;

        if (document.getElementById(uiFrameworkErrorMessage) != null)
            document.getElementById(uiFrameworkErrorMessage).innerHTML = nabuFrameworkErrorDetails;
    }
}
