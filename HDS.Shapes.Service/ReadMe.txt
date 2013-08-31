To install this service:

Make any changes you need to the app.config file - once you install and run the service, changes to the app.config will require a re-start.

Run As Administrator a command prompt (rt-click the cmd tool and choose run as administrator)

run the following from the directory where the project files have been unzipped:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil HDS.Shapes.Service.exe

Open up the services by running (win-R) services.msc. Find "HDS Shapes Service" and start it. It will start automatically after this.

to uninstall:
Make sure the services.msc window is closed first, then run the following:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil HDS.Shapes.Service.exe /u