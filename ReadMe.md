# SharePoint FTC Branding Recognition #

### Summary ###
Check to see if an FTC feature for branding is installed before applying branding through Javascript customization. 

### Applies to ###
- Office 365 Dedicated (D)
- SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
FTCtoCAMrecognition | Daniel Budimir

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 6th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
You may have and FTC branding solution/feature in place that must be retracted prior to the move to o365 MT/vNext.  During the retraction your sites would be unbranded, once retraction is complete you could then deploy your Provider Hosted Add-in (PHA) to re-establish branding.  This is kind of messy because it relies on you to manually handle recognizing that the FTC has been retracted prior to deploying your PHA so the 2 don't collide.

I've provided 2 solutions to the issue.

1) Simply check to see if an html id tag exists or not.  If you've branded your site using delegate controls, then some of the ootb html will be replaced with the markup from your controls. It's easy enough to check if an id exists on the page that would be there if the page were ootb, conversely, you may have added some id's in your delegate controls that can be checked for as well.

2) Check for the FTC feature id when the page is loaded and add the JavaScript customizations if the feature is not present.
The feature will not return in the features list if it has been deactivated or retracted.   

## Caveats ##

I've not written this example as a PHA, the CAM Deploy project copies 1 js file to the "Style Library" of your site, then registers that file as a JSLink custom action, the same logic would apply in a PHA except that the js file would live in your site so no need to copy it to SharePoint.

In the interest of simplicity, I did not add jQuery, SharePoint comes with a skinnied down version of jQuery in mQuery.js, so you'll notice some functions beginning with "m$", the syntax and usage is the same as jQuery.  

You will need to run this on-prem most likely, In o365D many customer still have FTC because the hardware is dedicated to the customer, so you may be able to test in a PPE environment if you have one.  For testing in a o365 Tenant you could use and existing feature id or possibly upload a solution with a feature that has no FTC.

# To use this sample #
1. Open the .sln file for the sample in **Visual Studio**.
2. Update the user name, password and url fields in the CamDeploy program.cs file line 22-24
3. Update the feature Id to match your FTC feature in the CamDeploy inject.js file line 45
4. Run the CamDeploy project to copy the inject.js file to the "Style Library" and add the JSLink custom action.
5. Update the url for the SPSolution project and deploy, this project simply replaces the search box with some text.
6. Browse the site and step through the prompts to see the results.
7. retract the SPSolution and browse the site and step through the prompts to see the results.


Once deployment is complete across the board and you're sure the CAM compliant branding is in place this logic can be removed completely.  

