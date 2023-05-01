# PadCrafterJsonImporter
UPM package which imports a json string exported from PadCrafter.com into a Unity InputSystem InputActionAsset

# Installing
You can:
- download this repo and put the code in Assets/com.darbotron.padcrafterjsonimporter into your project
- download and put the content of the /com.darbotron.padcrafterjsonimporter folder into your project's "Packages" folder
- add this via Unity Package Manager with a git URL: 
  - to get the latest version: https://github.com/darbotron/PadCrafterJsonImporter.git?path=/com.darbotron.padcrafterjsonimporter
  - to get a version with a specific tag append the tag name prefixed with '#' to the end of the URL e.g.: https://github.com/darbotron/PadCrafterJsonImporter.git?path=/com.darbotron.padcrafterjsonimporter#0.0.2-preview

# Using the importer
Once you've installed the package into your project.
- go to https://www.padcrafter.com/ and either make a control scheme or choose one of the examples from the dropdown
- using the 'Options' dropdown choose 'Export as JSON'
- in the 'Export as JSON' window click the 'Copy to clipboard' button
- in Unity open the importer from the Unity menu Darbotron -> PadCrafter Json Importer
- the Padcrafter Json Importer window should now appear
- click the 'Import Padcrafter Json from Clipboard' button...
- the json you exported should appear in the text area of the importer window (or an error if something went wrong)
- assuming there was no error you can click the 'Generate Input Action Asset' button...
- this will cause a system standard save panel to appear allowing you to choose where to save your asset and what to call it
- (NOTE: if you try to save outside your project's root folder this will currently cause undefined behaviour)
- once you've saved the InputActionAsset you should be able to open it & edit it with the normal Unity InputSystem editor

# Support
This importer was made out of the goodness of my heart for free so I will not be supporting it!
The package is licensed under the MIT license see https://opensource.org/license/mit/

Good luck & have fun!