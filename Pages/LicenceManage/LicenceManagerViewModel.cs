using ErpAlgerie.Modules.Core;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.CRM;
using Microsoft.Win32;
using Portable.Licensing;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Pages.LicenceManage
{
    class LicenceManagerViewModel : Screen
    {

        public string licenceFile { get; set; }
        public string clePublic { get; set; }
        public string email { get; set; }
        public string userName { get; set; }

        public string TypeLicence { get; set; }
        public string DateExpiration { get; set; }


        public LicenceManagerViewModel()
        {
            if (File.Exists("public-lcs"))
            {
                using(var str = new StreamReader("public-lcs"))
                {
                    clePublic = str.ReadToEnd();
                   
                }
            }

            if (File.Exists("License.lic"))
            {
                try
                {
                    using (var sr = (new StreamReader("License.lic")))
                    {
                        var lic = License.Load(sr);
                        TypeLicence = lic.Type.ToString();
                        DateExpiration = lic.Expiration.ToLongDateString();
                        IsValide = true;
                    }
                }
                catch 
                {}
            }

            var settings = DataHelpers.Settings;
            email = settings.Email;
            userName = settings.UserName;
            NotifyOfPropertyChange("clePublic");
            NotifyOfPropertyChange("email");
            NotifyOfPropertyChange("userName");
            NotifyOfPropertyChange("TypeLicence");
            NotifyOfPropertyChange("DateExpiration");
        }

        public void LoadLicence()
        {
            var od = new OpenFileDialog();
            if (od.ShowDialog() == true)
            {
                var file = od.FileName;
                if (!file.Contains("lic"))
                {
                    MessageBox.Show("Licence non valide");
                    return;
                }
                licenceFile = file;
                NotifyOfPropertyChange("licenceFile");
            }
        }


        public bool IsValide { get; set; }

        public void VlidateLicence()
        {
            if(string.IsNullOrWhiteSpace( licenceFile))
            {
                MessageBox.Show("Charger votre licence d'abord");
                return;
            }
            if (string.IsNullOrWhiteSpace(clePublic))
            {
                MessageBox.Show("Clé public obligatoire!");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Vérifer nom d'utilsateur ou email");
                return;
            }

            using (var publicK = new StreamWriter("public-lcs"))
            {
                publicK.Write(clePublic);
                publicK.Close();
            }
              

            File.Copy(licenceFile, "License.lic", true);

            try
            {
                if (FrameworkManager.ValidateLicence(clePublic,userName,email))
                {
                    MessageBox.Show("Licence validée");
                    var settings = new ElvaSettings().getInstance();
                    settings.UserName = userName;
                    settings.Email = email;
                    settings.Save();
                    IsValide = true;
                    this.RequestClose();
                }
                else
                {
                    IsValide = false;
                    MessageBox.Show("Licence invalide");

                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }



        }

        protected override void OnClose()
        {
            if (!IsValide)
            {
                Environment.Exit(0);
            }

            base.OnClose();
        }

        public void ActivateDemo()
        {
            if (string.IsNullOrWhiteSpace(clePublic))
            {
                MessageBox.Show("Clé public obligatoire!");
                return;
            }
            using (var publicK = new StreamWriter("public-lcs"))
            {
                publicK.Write(clePublic);
                publicK.Close();
            }
               
            var used = new ElvaSettings().getInstance().DemoUsed;
            if (false)
            {
                MessageBox.Show("Licence demo expirée, Contactez votre fournisseur/ 0665 97 76 79 / ovresko@gmail.com");
                return;
            }
            else
            {
                FrameworkManager.CreateLicenceTrial(userName, email);
                try
                {
                    if (FrameworkManager.ValidateLicence(clePublic,userName,email))
                    {
                        MessageBox.Show("Licence validée");
                        IsValide = true;
                        this.RequestClose();
                    }
                    else
                    {
                        MessageBox.Show("Licence invalide");
                        IsValide = false;
                        
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
            }
        }

    }
}
