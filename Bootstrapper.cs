using System;
using Stylet;
using StyletIoC;
using ErpAlgerie.Pages;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Module;
using System.Threading;
using ErpAlgerie.Modules.Core.Hooks;
using System.Windows;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FluentScheduler;
using MongoDB.Bson;
using System.Drawing;
using Bogus;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Modules.Core;
using MongoDbGenericRepository;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Pages.Template;
using LLibrary;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Threading;

namespace ErpAlgerie
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private ThreadStart th;
        private Thread t;

        protected override void OnStart()
        {

            try
            {

                base.OnStart();
                Stylet.Logging.LogManager.Enabled = true;


                // Configure L Logging

                var myLogger = new L(new LConfiguration
                {
                    DeleteOldFiles = TimeSpan.FromDays(5),
                });

                DataHelpers.Logger = myLogger;

                // Change culture
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-DZ");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-DZ");

            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
          
        }

        protected override void OnExit(ExitEventArgs e)
        {

            try
            {
                DataHelpers.mongod.Kill();
                DataHelpers.restheart.Kill();
            }
            catch (Exception s)
            {
                //MessageBox.Show(s.Message);
            }
            base.OnExit(e);

        }

        public static void SetupDb()
        {
            try
            {
                // Prepare database
                var adr = Properties.Settings.Default.MongoServerSettings;
                var db = Properties.Settings.Default.dbUrl;

                #region START LOCAL DB

                // Check Connection
                //var cmd = $"dbs/MongoDBPortable.exe";
                //Process.Start(Path.GetFullPath(cmd));

                var lockFiles = "dbs/data";
                var fls = Directory.EnumerateFiles(lockFiles);

                var locks = fls.Where(a => a.Contains("lock"));
                foreach (var item in locks)
                {
                    try
                    {

                        File.Delete(Path.GetFullPath(item));
                    }
                    catch (Exception s)
                    {
                        Console.Write(s.Message);
                    }
                }

                var dbfolder = Path.GetFullPath("dbs/data");

                //starting the mongod server (when app starts)
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"dbs/bin/mongod.exe";
                start.WindowStyle = ProcessWindowStyle.Hidden;
                // set UseShellExecute to false
                start.UseShellExecute = false;

                //@"" prevents need for backslashes
                //var dbPaht = Path.GetFullPath("dbs/db");
                start.Arguments = $"--dbpath=\"{dbfolder}\" --storageEngine=mmapv1 --bind_ip=0.0.0.0";

                DataHelpers.mongod = Process.Start(start);



                // RESTHEART

                //starting the mongod server (when app starts)
                ProcessStartInfo startRestHeart = new ProcessStartInfo();
                startRestHeart.FileName = @"java";
                startRestHeart.WindowStyle = ProcessWindowStyle.Hidden;
                // set UseShellExecute to false
                startRestHeart.UseShellExecute = false;

                //@"" prevents need for backslashes
                //var dbPaht = Path.GetFullPath("dbs/db");
                startRestHeart.Arguments = $"-jar restheart/restheart.jar";

                DataHelpers.restheart = Process.Start(startRestHeart);




                #endregion



                // i have one instance settings with one adresse
                // Check if db source exist
                try
                {
                    bool addDefault = false;
                    DbSourceLink DefaultDbSource = new DbSourceLink();
                    // CHeck if DefaultDB exist            
                    var dblinks = DataHelpers.GetMongoDataSync("DbSourceLink") as IEnumerable<DbSourceLink>;
                    if (dblinks != null)
                    {
                        if (!dblinks.Select(a => a.DbName).Contains("Default"))
                        {
                            DefaultDbSource = new DbSourceLink();
                            DefaultDbSource.DbName = "Default";
                            DefaultDbSource.SourceIp = adr;
                            addDefault = true;
                        }
                    }
                    var setting = (new ElvaSettings()).getInstance();
                    if (setting != null)
                    {
                        if (addDefault)
                            DefaultDbSource.Save();

                        if (setting.DbSourceLink != null && setting.DbSourceLink != ObjectId.Empty)
                        {

                            // i have db set
                            var DbSource = setting.DbSourceLink.GetObject("DbSourceLink") as DbSourceLink;
                            if (DbSource != null)
                            {
                                Properties.Settings.Default.MongoServerSettings = DbSource.SourceIp;
                                Properties.Settings.Default.dbUrl = DbSource.DbName;
                                DataHelpers.DbAdresse = DbSource.SourceIp;
                                DataHelpers.DbName = DbSource.DbName;
                            }
                            else
                            {
                                SetDefaultDB();
                            }
                        }
                        else
                        {
                            SetDefaultDB();
                        }
                    }
                    else
                    {
                        SetDefaultDB();
                    }
                }
                catch (Exception s)
                {
                    Console.Write(s.Message);
                    SetDefaultDB();
                }



                // StartLocal Db if local;

            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }

        }

        public static void SetDefaultDB()
        {
            try
            {
                var adr = Properties.Settings.Default.MongoServerSettings;
                var db = Properties.Settings.Default.dbUrl;

                DataHelpers.DbAdresse = adr;
                DataHelpers.DbName = db;

            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }
        
        protected override void Configure()
        {
            try
            {

                SetupDb();
                // Check and update modules
                var updateModules = Properties.Settings.Default.UpdateModules;
                FrameworkManager.UpdateModules(updateModules);
                
                // Init admin
                FrameworkManager.AdminExists(true);

                // Clean data and files
                FrameworkManager.CleanFiles();

                // Reload new modules
                FrameworkManager.ReloadModules();
                
                // Load Settings in memory
                DataHelpers.PosSettings = PosSettings.getInstance();

                var viewManager = this.Container.Get<ViewManager>();
                var setting = new ElvaSettings().getInstance();
                DataHelpers.Settings = setting;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }

        }

        protected override void OnLaunch()
        {
            try
            {
                base.OnLaunch();
                FrameworkManager.CreateCulture();

                // TEMP
                // FrameworkManager.GenerateLicence();
                // FrameworkManager.CreateLicenceTrial("admin", "admin");
                // FrameworkManager.CreateLicenceStandard("pos", "pos@pos.com");

                // Check AppInit

                var setting = new ElvaSettings().getInstance();

                FrameworkManager.CheckValidation();

                if (setting.AppInitialized == false)
                {
                     MessageBox.Show("Configuration de premiére utilisation");

                    // Setup modules
                    FrameworkManager.UpdateModules();

                    // setup series
                    FrameworkManager.CreateSeries();
                    
                    setting.AppInitialized = true;
                    setting.Save();

                    MessageBox.Show("Configuration terminée");


                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }

        }


        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            base.OnUnhandledException(e);
        }
    }
}
