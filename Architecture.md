##Projects
Drey need a few things to work appropriately:

###Drey
Imported within a console application, this is the hosting environment that will manage the instantiation and discarding of applications.  It's a lean library, and will probably never be updated in the next 10 years.

When creating a project with Drey, you'll create this project within the first 1-2 days, build an installer for it, and possibly never touch it again for the life of your application.

Within your installation msi file, you should install the following items:

* your windows service *.exe, which is your implementation of the Drey.dll
* nuget.exe - our tool of choice to unpack nuget packages
* the Drey.Configuration.nupkg

As a post-processing step to the msi installer, the drey.configuration package should be installed as a component.

###Drey.Nut
This is the framework to your application.  You'll add a `CrackingPointAttribute` that tells Drey where your startup class is at, and you'll write the equivalent of a `Program.cs` file or `Startup.cs` file (for the Owin folks) that has a single method `Configure()`.  We'll pass you an IDreyConfiguration object that will provide:

* Application Settings - a key/value pair of items to be used throughout your application.
* Connection Strings - any connection strings that have been configured for your application.
* Horde Directory - Where is the base directory the nuts will be installed within?

For ease of deployment, all nuts will be packaged using the nuget.exe packaging system.  

###Drey.Configuration
This nut needs to be installed with every.  Its responsibilities will include:

* "hording" additional nuts
* managing the "cracking" of the nut within the horde
* configuration of the nut
* bringing the nut online
* messaging `Drey` to shut-down the current version of the nut, and to launch the new version of the same.
* updating itself

The application itself will be a Nancy web application, to allow deployment on Windows, MacOS-X, and \*-IX environments.  It will utilize a SQLite database as a backing store, which can be uploaded to a server as a backup and restore mechanism.

###Drey.Server
`Drey.Server` will initially be a set of `Presentation Model Objects (Pmo)` that will be implemented outside of this project/solution.  Eventually, the hope is to provide a drop-in server implementation, but that will not be until after we've done some successful deployments, and see what the general consensus is that the server should have within it.