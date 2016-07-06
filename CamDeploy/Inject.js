var wasrun = false;
m$.ready(function () {
    var s = 'The first example checks to see if a tag is present indicating that the feature is NOT installed and activated.  ';
    s += "The feature in this project includes a delegate control that replaces 'SmallSearchInputBox', once installed, the 'ctl00_PlaceHolderSearchArea_SmallSearchInputBox1_csr_sboxdiv' id tag no longer exists ";
    s += "because it has been replaced by the contents of the delegate control.  ";
    s += "I like this method best because it doesnt require a call back to SharePoint to determine if the feature is installed.  ";
    s += "Execute this check?";
    wasrun = confirm(s);
    if (wasrun == true)
    {
        var x = m$("#ctl00_PlaceHolderSearchArea_SmallSearchInputBox1_csr_sboxdiv");
        if (x.length != 0) {
            alert("I've found the search box so my ftc feature has been retracted, deactivated or has not been deployed, I can now apply my CAM branding by removing the logo.");
            m$("#DeltaSiteLogo").remove();
        }
        else {
            alert("I can't find the search box, that means my delegate control is currently deployed, I cannot deploy the CAM branding.");
        }
    }
});


var context = null;
var web = null;
var featureCollection;
ExecuteOrDelayUntilScriptLoaded(CheckFeatureIsEnabled, "SP.js");
function CheckFeatureIsEnabled() {
    if (wasrun) { return; }
    var s = 'This second example checks to see if a feature with the id found in our manifest is present in the list of installed and activated features.  ';
    s += "This method does require a call back to SharePoint on each page render so I like option 1 better, but the return value could be cached for a bit so a call would not be required on each render.  ";
    s += "Execute this check?";
    wasrun = confirm(s);
    if (wasrun == true) {
        context = new SP.ClientContext.get_current();
        web = context.get_web();
        featureCollection = web.get_features();
        context.load(featureCollection);
        context.executeQueryAsync(onSuccess, onFailure);
    }
}
function onSuccess() {
    var found = false;
    var listEnumerator = featureCollection.getEnumerator();
    while (listEnumerator.moveNext()) {
        if (listEnumerator.get_current().get_definitionId() == 'fe41cfac-20e9-4d2d-a322-617f1cdf7f02') {
            found = true;
            alert("I've found the custom branding feature, I cannot deploy the CAM branding.");
        }
    }
    if (!found)
    {
        alert("I was unable to find the feature id, so my ftc feature has been retracted, deactivated or has not been deployed.  I can now apply my CAM branding by removing the logo.");
        m$("#DeltaSiteLogo").remove();
    }
}
function onFailure(sender, args) {
    alert('request failed ' + args.get_message() + 'n' + args.get_stackTrace());
}
