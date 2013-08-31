using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using HDS.FileHandler;

namespace HDS.Shapes.Service
{
    partial class ShapeService : ServiceBase
    {
        // Create a logger for use in this class
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Component Designer generated code
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "Service1";
        }
        #endregion

        public ShapeService()
        {
            InitializeComponent();
            ServiceName = "HDS Shape Service";
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            if (Log.IsInfoEnabled)
                Log.InfoFormat("Application {0} Started", ServiceName);

            try
            {
                var w = FileUtil.WatchFolder(ConfigurationManager.AppSettings[Constants.keyShapesWatchFolder],
                    ConfigurationManager.AppSettings[Constants.keyShapesWatchExtension]);
                w.Created += OnCreated;
                w.Deleted += OnDeleted;
            }
            catch (Exception ex)
            {
                Log.Error("WGBMspToFiserv Service failed to start! {1}", ex);
                Stop();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (Log.IsInfoEnabled)
                Log.InfoFormat("Application {0} Stopped", ServiceName);
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (Log.IsInfoEnabled)
                Log.InfoFormat("Application {0} Paused", ServiceName);
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            if (Log.IsInfoEnabled)
                Log.InfoFormat("Application {0} Continued", ServiceName);

        }

        protected void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (Log.IsInfoEnabled)
                Log.InfoFormat("File Deleted in input folder: '{0}'", e.Name);
        }

        protected void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (Log.IsInfoEnabled)
                Log.InfoFormat("File Created in input folder: '{0}'", e.Name);

            ProcessFile.ProcessInput(e.FullPath);
        }
    }
}
