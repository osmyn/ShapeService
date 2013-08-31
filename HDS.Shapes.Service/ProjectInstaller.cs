using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace HDS.Shapes.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller serviceProcessInstaller;

        public ProjectInstaller()
        {
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "HDSShapesService";
            serviceInstaller.DisplayName = "HDS Shapes Service";
            serviceInstaller.Description = "Transforms input files into shape output files";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            Installers.Add(serviceInstaller);

            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            Installers.Add(serviceProcessInstaller);
        }

        public string GetContextParameter(string key)
        {
            string sValue = "";
            try
            {
                sValue = this.Context.Parameters[key].ToString();
            }
            catch
            {
                sValue = "";
            }
            return sValue;
        }


        // Override the 'OnBeforeInstall' method.
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            string username = GetContextParameter("user").Trim();
            string password = GetContextParameter("password").Trim();

            if (username != "")
                serviceProcessInstaller.Username = username;
            if (password != "")
                serviceProcessInstaller.Password = password;
        }
    }
}
