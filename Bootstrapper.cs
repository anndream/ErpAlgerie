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

namespace ErpAlgerie
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private ThreadStart th;
        private Thread t;

        protected override void OnStart()
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
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-DZ");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-DZ");

          
        }
        protected override void OnExit(ExitEventArgs e)
        {

            try
            {
                DataHelpers.mongod.Kill();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
            base.OnExit(e);

        }
        public static void SetupDb()
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
                    Console.Write(s.Message );
                }
            }

            var dbfolder = Path.GetFullPath("dbs/data");

            //starting the mongod server (when app starts)
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName =   @"dbs/bin/mongod.exe";
            start.WindowStyle = ProcessWindowStyle.Normal;
            // set UseShellExecute to false
            start.UseShellExecute = false;

            //@"" prevents need for backslashes
            //var dbPaht = Path.GetFullPath("dbs/db");
            start.Arguments = $"--dbpath=\"{dbfolder}\" --storageEngine=mmapv1";

            DataHelpers.mongod = Process.Start(start);

            

            #endregion



            // i have one instance settings with one adresse
            // Check if db source exist
            try
            {
                bool addDefault = false;
                DbSourceLink DefaultDbSource = new DbSourceLink();
                // CHeck if DefaultDB exist            
                var dblinks = DataHelpers.GetMongoDataSync("DbSourceLink") as IEnumerable<DbSourceLink>;
                if(dblinks != null)
                {
                    if(!dblinks.Select(a => a.DbName).Contains("Default"))
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
            catch (Exception s )
            {
                Console.Write(s.Message);
                SetDefaultDB();
            }



            // StartLocal Db if local;
           

        }
        public static void SetDefaultDB()
        {
            var adr = Properties.Settings.Default.MongoServerSettings;
            var db = Properties.Settings.Default.dbUrl;

            DataHelpers.DbAdresse = adr;
            DataHelpers.DbName = db;

        }

       

        protected override void Configure()
        {

            

            SetupDb();
            // Check and update modules
            var updateModules = Properties.Settings.Default.UpdateModules;
            FrameworkManager.UpdateModules(updateModules);
            

            // Init admin
            FrameworkManager.AdminExists(true);

            var viewManager = this.Container.Get<ViewManager>();
            var setting = new ElvaSettings().getInstance();
            DataHelpers.Settings = setting;

        }

        protected override void OnLaunch()
        {
            base.OnLaunch();

            // TEMP
            //FrameworkManager.GenerateLicence();
            //FrameworkManager.CreateLicenceTrial("admin", "admin");
            // FrameworkManager.CreateLicenceStandard("khaled", "kimboox44@gmail.com");

            // Check AppInit

            var setting = new ElvaSettings().getInstance();

            FrameworkManager.CheckValidation();

            if (setting.AppInitialized == false)
            {
                MessageBox.Show("Configuration de premiére utilisation");
                 
                // Setup Licence
                //FrameworkManager.GenerateLicence();
            }

        }

    }
}
