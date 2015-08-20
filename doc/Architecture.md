##Projects
Drey need a few things to work appropriately:

###Drey
Imported within a console application, this is the hosting environment that will manage the instantiation and discarding of applications.  It's a lean library, and will probably never be updated in the next 10 years.

When creating a project with Drey, you'll create this project within the first 1-2 days, build an installer for it, and possibly never touch it again for the life of your application.

Within your installation msi file, you should install the following items:

* your windows service \*.exe, which is your implementation of the Drey.dll
* nuget.exe - our tool of choice to unpack nuget packages
* the Drey.Configuration.nupkg

As a post-processing step to the msi installer, the drey.configuration package should be installed as a component.

###Drey.Configuration
This nupkg needs to be installed with every service host.  Its responsibilities include:

* downloading additional apps/packages
* Bringing apps/packages online.
* A configuration UI for each app/package
* updating itself

The application itself will be a Nancy web application, to allow deployment on Windows, MacOS-X, and \*-IX environments.  It will utilize a SQLite database as a backing store, which can be uploaded to a server as a backup and restore mechanism.

###Drey.Server

`Drey Server` has two base endpoints:

**/api/v2/package** - This endpoint is a light response to 
