using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Pages.LicenceManage;
using MongoDbGenericRepository.Models;
using Portable.Licensing;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

    
namespace ErpAlgerie.Modules.Core
{
    public static class FrameworkManager
    {

        public static void ReloadModules()
        {
            var modules = DataHelpers.GetMongoDataSync("ModuleErp") as IEnumerable<ModuleErp>;
            DataHelpers.Modules = modules as IEnumerable<ModuleErp>;
            var collections = modules.Select(z => z.Libelle).ToList();
            string nspace = "ErpAlgerie.Modules.CRM";

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == nspace
                    && (t.IsSubclassOf(typeof(ExtendedDocument))
                    && !typeof(NoModule).IsAssignableFrom(t))
                    select t;
            q.ToList().ForEach(t =>
            {
                try
                {
                    Console.Write(t.Name);
                    var instance = Activator.CreateInstance(t);
                    var collection = t.GetProperty("CollectionName").GetValue(instance)?.ToString();
                    if (!collections.Contains(collection))
                    {
                       
                    var moduleName = t.GetProperty("ModuleName").GetValue(instance)?.ToString();
                    var iconName = t.GetProperty("IconName").GetValue(instance)?.ToString();
                    var showInDesktop = t.GetProperty("ShowInDesktop").GetValue(instance) as bool?;
                    var isInstance = bool.Parse(t.GetProperty("IsInstance").GetValue(instance)?.ToString());
                    if (!string.IsNullOrWhiteSpace(moduleName))
                    {
                        Console.Write("EXIT");
                    }
                    var moduleErp = new ModuleErp();
                    moduleErp.Libelle = collection;
                    moduleErp.EstAcceRapide = showInDesktop.Value;
                    moduleErp.ClassName = $"{nspace}.{t.Name}";
                    moduleErp.GroupeModule = moduleName;
                    moduleErp.ModuleIcon = iconName;
                    moduleErp.ModuleSubmitable = (instance as ExtendedDocument).Submitable;
                    moduleErp.IsInstanceModule = isInstance;
                    moduleErp.Save();

                    }
                }
                catch (Exception s)
                {
                    Console.WriteLine("ERROR\n");
                    Console.WriteLine(s.Message);
                }

            }
            );

            modules = DataHelpers.GetMongoDataSync("ModuleErp") as IEnumerable<ModuleErp>;
            DataHelpers.Modules = modules ;
        }

        public static void UpdateModules(bool DoUpdate = true)
        {
            var modules = DataHelpers.GetMongoDataSync("ModuleErp");
            DataHelpers.Modules = modules as IEnumerable<ModuleErp>;
            if (DoUpdate == false)
            {
                //check in db
                if (modules?.Count() < 1)
                    DoUpdate = true;
            }
            // deleteAll Modules 

            if (!DoUpdate)
                return;

            foreach (var module in modules)
            {
                (module as IModel).Delete(false);
            }

            //ErpAlgerie.Modules.CRM
            string nspace = "ErpAlgerie.Modules.CRM";
            
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == nspace
                    && (t.IsSubclassOf(typeof(ExtendedDocument))
                    && !typeof(NoModule).IsAssignableFrom(t))
                    select t;
            q.ToList().ForEach(t =>
                {
                    try
                    {
                        Console.Write(t.Name);
                        var instance = Activator.CreateInstance(t);
                        var collection = t.GetProperty("CollectionName").GetValue(instance)?.ToString();
                        var moduleName = t.GetProperty("ModuleName").GetValue(instance)?.ToString();
                        var iconName = t.GetProperty("IconName").GetValue(instance)?.ToString();
                        var showInDesktop =  t.GetProperty("ShowInDesktop").GetValue(instance) as bool?;
                        var isInstance = bool.Parse(t.GetProperty("IsInstance").GetValue(instance)?.ToString());
                        if (!string.IsNullOrWhiteSpace(moduleName))
                        {
                            Console.Write("EXIT");
                        }
                        var moduleErp = new ModuleErp();
                        moduleErp.Libelle = collection;
                        moduleErp.EstAcceRapide = showInDesktop.Value;
                        moduleErp.ClassName = $"{nspace}.{t.Name}";
                        moduleErp.GroupeModule = moduleName;
                        moduleErp.ModuleIcon = iconName;
                        moduleErp.ModuleSubmitable = (instance as ExtendedDocument).Submitable;
                        moduleErp.IsInstanceModule = isInstance;
                        moduleErp.Save();
                    }
                    catch (Exception s)
                    {
                        Console.WriteLine("ERROR\n");
                        Console.WriteLine(s.Message);
                    }

                }
            );

             modules = DataHelpers.GetMongoDataSync("ModuleErp");
            DataHelpers.Modules = modules as IEnumerable<ModuleErp>;

        }

      

        public static void SetupApp()
        {
            // Do all the nessesary job for initial setup;
        }

        // Check if app has minimum Rôles
        public static bool RolesExists(bool AddIfNot = false)
        {
            return true;
        }

        // Check if app has instance of ElvaSettings
        public static bool SettingsExists(bool AddIfNot = false)
        {
            return true;
        }

        // Check if app has taxes
        public static bool TaxesExists(bool AddIfNot = false)
        {
            return true;
        }


        // Check if class modules exist in the database
        public static bool ModulesExists(bool AddIfNot = false)
        {
            return true;
        }




        // Check if series in database
        public static void CreateSeries(bool DeleteOldSeries = true)
        {
            if (DeleteOldSeries)
            {
                var oldseries = DS.db.GetAll<SeriesName>(a => true) as IEnumerable<SeriesName>;
                DS.db.DeleteMany(oldseries);
            }

            var modulesWithSeries = DS.db.GetAll<ModuleErp>(a => true) as IEnumerable<ModuleErp>;
            if(modulesWithSeries != null)
            {

                foreach (var module in modulesWithSeries)
                {
                    try
                    {
                        var serie = new SeriesName();
                        serie.Libelle = module.Name;
                        serie.Sufix = new string(module.Libelle.Take(2).ToArray()).ToUpper() + "-########";
                        serie.Indexe = 1;
                        serie.ForceIgniorValidatUnique = true;
                        serie.Save();
                        module.SeriesNames = new List<SeriesName>();
                        module.SeriesNames.Add(serie);
                        module.SeriesDefault = serie.Id;
                        module.Save();
                    }
                    catch (Exception s)
                    {
                        DataHelpers.Logger.LogError(s);
                        throw s;
                    }
                }
            }

        }



        // Check if admin user exist in database
        public static bool AdminExists(bool AddIfNot = false)
        {
            var users = DataHelpers.GetMongoDataSync("User");
            if(users?.Count() < 1)
            {
                if (AddIfNot)
                {
                    User admin = new User()
                    {
                        Libelle = "Admin",
                        Password = "admin",
                        Fonction = "Administrateur systéme",
                        IsAdmin = true
                    };

                    admin.Save();
                }
                return false;
            }
            return true;
        }

        // Check if plan comptable exist
        public static bool ComptesExists(bool AddIfNot = false)
        {
            return true;
        }

        // Check internet for feedback send
        public static bool InternetExists()
        {
            return true;
        }

        // Check if app licence exist
        public static bool LicenceExists()
        {




            return true;
        }

        public static void GenerateLicence()
        {
            var keyGenerator = Portable.Licensing.Security.Cryptography.KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            var privateKey = keyPair.ToEncryptedPrivateKeyString("15021991");
            var publicKey = keyPair.ToPublicKeyString();

            using (var licence = new StreamWriter("public-lcs"))
            {
                licence.Write(publicKey);
            }

            using (var licence = new StreamWriter("private-lcs"))
            {
                licence.Write(privateKey);
            }

        }

        public static void CreateLicenceStandard(string username, string email)
        {

            var old = File.Exists("License.lic");
            if (old)
            {
                File.Delete("License.lic");
            }
           
                    var license = License.New()
            .As(LicenseType.Standard)
            .LicensedTo(username, email)
            .CreateAndSignWithPrivateKey(PrivateKey, "15021991");

            license.Customer.Email = email;
            license.Customer.Name = username;
            license.Sign(PrivateKey, "15021991");
            license.Type = LicenseType.Standard;


            var settings = new ElvaSettings().getInstance();
            settings.UserName = username;
            settings.Email = email;
            settings.Save();    
            File.WriteAllText("License.lic", license.ToString(), Encoding.UTF8);
        }

        internal static void CleanFiles()
        {
            var files = Directory.EnumerateFiles("temp");
            foreach (var item in files)
            {
                try { File.Delete(item); } catch { Console.WriteLine("Can't delete...!"); }
            
            }
        }

        public static string PrivateKey = "MIICITAjBgoqhkiG9w0BDAEDMBUEEFTGdTEj3djRVchSDv2vrBoCAQoEggH4ow+DkQNlHuB0whG0h51pCMcLuwTrjArybmVZM+PiTRbEnF65d9D4l8t276OEEOmUd07O30RTEEQnm+1+fv7eNmJ8xfzRXqS0/oCm2WjtGxlxQY7DlxctpSlJVLTgI27TuMN6tky6C8EGais35bZAJsbMCCcw6I1BCMUV6ZlCYgvTPSAOIg+Vh8LZUrRbxE/CJ6Ly+BPNlTCHzeqKIkBCoruxMXQEJuLqbbsP0HgiRKeDnowL2Xq/QzbXTi4BjUs26E5Rm2gOoZvf/BxnnPMtpxaRmL0QJ4aQKO1Jk/WkmO6YjG0pTi2XXn4u9cOmh/8q54CFfr7Y2Y3zDbBLeVYPrI8V1l6zcD0pi7S8VDgSpY1RkrAqgvTSlW7bERSAgW69kBRuG4SDZPesGj9AgRcqU8SKq8VSGXwTypv8yyHAkXKbEUdhFueHyRZhltqgjGMc90AYRjD4sjZ+zC6oM5fEGG/T0NB8/VodGyUwAbxttIwI820JoJBwhzQYMyqeICpIQb4O6DljCSXDuKxTHuG/0uaWnjAdT6GSydDvejkA9DVZMMyN0+Wl8l8+ywokwKsSm38WQ29PyAwXN8wBnLm8AYqxBi8fotj+b0wSLSXo6w5NCuKljgzBPXhHM4Ri4P2xa/9GJo/dOkLtbMoLPAUqFnMKkdO7HNuu";
        public static void CreateLicenceTrial(string username,string email)
        {

            var old = File.Exists("License.lic");
            if (old)
            {
                File.Delete("License.lic");
            }
            
                    var license = License.New()
            .As(LicenseType.Trial)
            .ExpiresAt(DateTime.Now.AddDays(14))
            .LicensedTo(username, email)
            .CreateAndSignWithPrivateKey(PrivateKey, "15021991");

            license.Expiration = DateTime.Now.AddDays(14);
            license.Customer.Email = email;
            license.Customer.Name = username;
            license.Sign(PrivateKey, "15021991");
            license.Type = LicenseType.Trial;
            var settings = new ElvaSettings().getInstance();
            settings.UserName = username;
            settings.Email = email;

            settings.DemoUsed = true;
            settings.Save();

            File.WriteAllText("License.lic", license.ToString(), Encoding.UTF8);
        }

        internal static void CreateCulture()
        {

            


        }

        internal static void CheckValidation()
        {
            if (File.Exists("public-lcs"))
            {
                var user = DataHelpers.Settings.UserName;
                var email = DataHelpers.Settings.Email;
                try
                {
                    using (var publicK = new StreamReader("public-lcs"))
                    {
                        var result = ValidateLicence(publicK.ReadLine(), user, email);
                        if (result)
                        {
                            return;
                        }
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                    try
                    {
                        File.Delete("License.lic");
                    }
                    catch
                    { }
                    try
                    {
                        File.Delete("public-lcs");
                    }
                    catch
                    { }
                }
              
            }
            var lm = new LicenceManagerViewModel();
            DataHelpers.windowManager.ShowWindow(lm);
        }

        public static bool ValidateLicence(string publicKey, string username, string email)
        {
            if(string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vérifier les données insérées");
                return false;
            }

            var old = File.Exists("License.lic");
            if (old)
            {
                try
                {
                    using (var sr = (new StreamReader("License.lic")))
                    {

                        var license = License.Load(sr);
                        if (license.VerifySignature(publicKey))
                        {
                            if (license.Customer.Name != username || license.Customer.Email != email)
                            {
                                MessageBox.Show("Licence invalide! contactez votre fournisseur <0665 97 76 79> <contact@ovresko.com> <ovresko@gmail.com>");
                                return false;
                            }

                            // Licence existe
                            var licenceType = license.Type;
                            if (licenceType == LicenseType.Trial)
                            {


                                var date = DateTime.Now;
                                var started = license.Expiration;
                                if (date >= started)
                                {
                                    // Licence Dead
                                    MessageBox.Show("Période d'éssai terminé, contactez votre fournisseur <0665 97 76 79> <contact@ovresko.com> <ovresko@gmail.com>");
                                    return false;
                                }
                                else
                                {
                                    DataHelpers.Settings.DemoUsed = true;
                                    DataHelpers.Settings.Save();
                                }
                            }
                            DataHelpers.LicenceType = licenceType.ToString();
                            DataHelpers.Expiration = license.Expiration;
                            DataHelpers.Customer = license.Customer.Name;

                            return true;
                        }
                        else
                        {
                            // Licence doesn't exist 
                            MessageBox.Show("Licence invalide! contactez votre fournisseur <0665 97 76 79> <contact@ovresko.com> <ovresko@gmail.com>");


                            return false;
                        }


                    }

                }
                catch (Exception s)
                {
                    try
                    {
                        File.Delete("License.lic");
                    }
                    catch 
                    {}
                    try
                    {
                        File.Delete("public-lcs");
                    }
                    catch 
                    {}
                }
            }
            

            return false;
        }

        internal static bool CanSave(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;


            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.CanSave == false);
                return result;
            }
            return true;
        }


        private static IEnumerable<AccesRule> GetRulesForType(Type type)
        {
            var user = DataHelpers.ConnectedUser;
            
            var roles = user.Roles; // Manager CRM ...
            List<Role> Allroles = new List<Role>();
            roles.ForEach(a => Allroles.Add(a.GetObject("Role")));
            var Module = $"{type.Namespace}.{type.Name}";
            return Allroles.SelectMany(z => z.AccesRules).Where(a => (a.Module.GetObject("ModuleErp") as ModuleErp)?.ClassName == Module);
        }

        internal static bool CanAdd(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;


            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.Creer == false);
                return result;
            }
            return true;
        }

        internal static bool CanView(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;

            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.Voir == false);
                return result;
            }
            return true;
        }

        internal static bool CanDelete(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;


            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.Supprimer == false);
                return result;
            }
            return true;
        }

        internal static bool CanValidate(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;


            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.Valider == false);
                return result;
            }
            return true;
        }

        internal static bool CanCancel(Type type)
        {
            if (type.Name == "ModuleErp")
                return true;

            // TODO: Remove if no user accept all
            if (DataHelpers.ConnectedUser == null)
                return true;

            var RulesForThisType = GetRulesForType(type);
            if (RulesForThisType.Any() && RulesForThisType != null)
            {
                var result = !RulesForThisType.Any(a => a.CancelSubmit == false);
                return result;
            }
            return true;
        }
    }
}
