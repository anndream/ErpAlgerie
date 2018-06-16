
using ErpAlgerie.Modules.Core.Module;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using ErpAlgerie.Pages;
using MongoDB.Bson;
using ErpAlgerie.Modules.CRM;
using StyletIoC;
using System.Linq.Expressions;
using MongoDbGenericRepository.Models;
using MongoDbGenericRepository;
using ErpAlgerie.Pages.Template;
using ErpAlgerie.Modules.Core.Helpers;
using LLibrary;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace ErpAlgerie.Modules.Core.Data
{
    public class DataHelpers
    {



        public static string LicenceType { get; set; }
        public static DateTime Expiration { get; set; }
        public static string Customer { get; set; }

        public static ModuleErp GetModule(ExtendedDocument type)
        {
            return Modules.FirstOrDefault(a => a.Libelle == type.CollectionName);
        }

        public static BackgroundWorker worker { get; set; } = new BackgroundWorker();


        private static dynamic GetGenericData(Type type)
        {
            try
            {
                Type d1 = typeof(GenericData<>);
                Type[] typeArgs = { type };
                Type makeme = d1.MakeGenericType(typeArgs);
                dynamic generic = Activator.CreateInstance(makeme, null);

                return generic;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return null;

            }
        }

        private static dynamic GetGenericData(string concrete)
        {
            try
            {
                var type = Type.GetType($"ErpAlgerie.Modules.CRM.{concrete}");
                Type d1 = typeof(GenericData<>);
                Type[] typeArgs = { type };
                Type makeme = d1.MakeGenericType(typeArgs);
                dynamic generic = Activator.CreateInstance(makeme, null);

                return generic;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return null;
            }
        }

        public static IScreen GetBaseViewModelScreen(Type type, IEventAggregator aggre, bool ForSelectOnly)
        {
            Type d1 = typeof(BaseViewModel<>);
            Type[] typeArgs = { type };
            Type makeme = d1.MakeGenericType(typeArgs);
            dynamic baseViewModel = Activator.CreateInstance(makeme, new object[] { aggre, ForSelectOnly });

            var control = baseViewModel;
            container.Get<ViewManager>().BindViewToModel(new BaseView(), control);
            //var screnn = control as IScreen;
            //screnn.AttachView(new BaseView());
            return control;
        }

        public static IScreen GetBaseViewModelScreen(Type type, IEventAggregator aggre, bool ForSelectOnly, IEnumerable<IDocument> _list)
        {
            Type d1 = typeof(BaseViewModel<>);
            Type[] typeArgs = { type };
            Type makeme = d1.MakeGenericType(typeArgs);
            dynamic baseViewModel = Activator.CreateInstance(makeme, new object[] { aggre, ForSelectOnly, _list });

            var control = baseViewModel;
            container.Get<ViewManager>().BindViewToModel(new BaseView(), control);
            return control;
        }

        public static User ConnectedUser { get; set; }
        public static ShellViewModel Shell { get; internal set; }
        public static IEventAggregator Aggre { get; internal set; }
        internal static ElvaSettings Settings
        {get;set;}

        public static string DbAdresse { get; internal set; }
        public static string DbName { get; internal set; }

        //public static IEnumerable<T> GetMongoData<T>(T collectionName)
        //{
        //    var client = new MongoClient(Properties.Settings.Default.MongoServerSettings);
        //    var MongoDB = client.GetDatabase("ElvaData");
        //    var collection = MongoDB.GetCollection<T>(collectionName.GetType().Name);

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        internal static IWindowManager windowManager;
        internal static string imgBg;

        private static DataHelpers _instance { get; set; }
        public static IEnumerable<ModuleErp> Modules { get; set; } = new List<ModuleErp>();

        public static L Logger { get; internal set; }
        

        public static DataHelpers instanc()
        {
            if (_instance == null)
                _instance = new DataHelpers();
            return _instance;
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }

        internal static void ClearData(string collection = null)
        {
            try
            {

                if (collection == null)
                {
                    DS.db.MongoDbContext.Database.Client.DropDatabase(DbName);
                    return;
                }

                DS.db.MongoDbContext.Database.DropCollection(collection);
                return;
            }
            catch (Exception s)
            {
                Logger.LogError(s);
                throw s;
            }
        }

        //public static async Task<ExtendedDocument> GetByIdV2(Type type, ObjectId? id)
        //{

        //    MethodInfo method = typeof(DataHelpers).GetMethod("GetByIdAsync");
        //    MethodInfo genericMethod = method.MakeGenericMethod(type);

        //    ExtendedDocument r = null;
        //    try
        //    {
        //        if(id.HasValue && id != ObjectId.Empty)
        //            r = await (genericMethod.Invoke(null, new object[] { id.Value }) as Task<ExtendedDocument>);
        //    }
        //    catch (Exception s)
        //    {
        //        Console.WriteLine(s.Message);
        //    }

        //    return r;
        //}


        //public static async Task<T> GetByIdAsync<T>(ObjectId id) where T:ExtendedDocument
        //{
        //    try
        //    {
        //        var db = DS.db;
        //        var r = await db.GetAllAsync<T>(a => a.Id == id);
        //        return r.FirstOrDefault();
        //    }
        //    catch 
        //    {
        //        return null;    
        //    }
        //}

        internal static int GetDays(DateTime doc)
        {
            try
            {
                return (int)(DateTime.Today - doc).TotalDays;
            }
            catch
            {
                return -1;
            }
        }

        internal static int GetDays(DateTime debut, DateTime fin)
        {
            try
            {
                return (int)(fin - debut).TotalDays;
            }
            catch
            {
                return -1;
            }
        }

        public static string GetPeriodeString(DateTime debut, DateTime end)
        {
            var days = new AgeHelper(debut, end);
            if (days.Years == 0)
            {
                if (days.Months == 0)
                {
                    return $"{days.Days} jours";
                }
                else
                {
                    return $"{days.Months} mois,{days.Days} jours";
                }
            }
            else
            {
                return $"{days.Years} ans et {days.Months} mois";
            }

            return "";
        }

        internal static async Task<IEnumerable<ExtendedDocument>> GetMongoData(string concrete)
        {
            var type = Type.GetType($"ErpAlgerie.Modules.CRM.{concrete}");
            return await GetGenericData(type).FindAllAsync();
            // return await  FilterEngine.GetMongoData(concrete);
        }


        // collect / Vache / Name
        internal static IEnumerable<ExtendedDocument> GetMongoData(string concrete, string field, object value, bool strict)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(concrete))
                    throw new Exception("Collection name is not valide");

                var generic = GetGenericData(concrete);
                Type t = generic.GetExpressionForSearch();

                var result = generic.Find(field, value, strict);
                return result;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return null;
            }
            //return FilterEngine.GetMongoData(concrete, field, value, strict);

        }

        //    return collection.Find(a => true).ToEnumerable();
        //}
        internal static IEnumerable<ExtendedDocument> GetMongoDataSync(string concrete)
        {
            var type = Type.GetType($"ErpAlgerie.Modules.CRM.{concrete}");
            return GetGenericData(type).FindAll();
        }

        internal static StyletIoC.IContainer container;
        internal static Process mongod;

        internal static List<string> GetSelectData(string option)
        {
            switch (option)
            {
                
                 case "StatusFacture":
                    return new List<string>()
                    {"",
                        "Brouillon",
                        "À Délivrée",
                        "À Payée",
                        "Terminée",
                        "Annulée"
                    };

                case "StatusCommande":
                    return new List<string>()
                    {"",
                        "Brouillon",
                        "À Délivrée",
                        "À Facturée",
                        "À Payée",
                        "Terminée",
                        "Annulée"
                    };

                case "TypeEcritureCompte":
                    return new List<string>()
                    {
                        "Recevoir",
                        "Payer",
                        "Transfert interne"
                    };

                case "ModePaiement":
                    return new List<string>()
                    {
                        "Espèces",
                        "Chèque",
                        "Virement bancaire"
                    };


                case "TypeEcritureJournal":
                    return new List<string>()
                    {
                        "Facture de vente",
                        "Facture d'achat",
                        "Entrée Paiement"
                    };
                case "TypeEcritureStock":
                    return new List<string>()
                    {"",
                        "Sortie de Matériel",
                        "Réception Matériel",
                        "Transfert de Matériel",
                        "Transfert de Matériel pour la Fabrication",
                        "Fabrication",
                        "Ré-emballer",
                        "Sous-traiter"
                    };
                case "TypeColumn":
                    return new List<string>()
                    {"",
                        "Text",
        "Date",
        "Devise",
        "Numero",
        "Select",
        "Check",
        "TextLarge",
        "Lien",
        "LienField",
        "LienButton",
        "LienFetch",
        "ReadOnly",
        "Separation",
        "FiltreText",
        "FiltreDate",
        "Image",
        "ImageSide",
        "Table",
        "WeakTable",
        "Button",
        "OpsButton",
        "BaseButton"
                    };

                case "UniteMesure":
                    return new List<string>()
                    {"",
                        "Unité",
                        "Kg",
                        "Paires",
                        "CM",
                        "m",
                        "m²",
                        "Boite",
                        "Carton"
                    };

                case "SerieArticle":
                    return new List<string>()
                            {"ART",
                                "P",
                                "S",
                                "PROD"
                            };

                case "CatégorieArticle":
                    return new List<string>()
                    {"N/A",
                        "Produit",
                        "Service",
                        "Abonnement"
                    };
                case "MethodeAppro":
                    return new List<string>()
                    {"N/A",
                        "Achat",
                        "Stock",
                        "Produire"
                    };

                case "TypeVentilation":
                    return new List<string>()
                    {
                        "Les anciens d'abord",
                        "Les nouveaux d'abord"
                    };
                case "EtatPaiement":
                    return new List<string>()
                    {
                        "Montant non payé",
                        "Payé partiellement",
                        "Montant payé"
                    };

                case "TypeClient":
                    return new List<string>()
                    {"",
                        "Contact",
                        "Société"
                    };

                default:
                    return null;
                    break;
            }
        }

        public static ExtendedDocument GetById(string typename, ObjectId? Id)
        {
            var generic = GetGenericData(typename);
            var result = generic.GetByIdNullable(Id);
            return result; 
        }


        public static ExtendedDocument GetById(Type type, ObjectId? Id)
        {
            var generic = GetGenericData(type);
            var result = generic.GetByIdNullable(Id);
            return result;
        }


        public static bool fun(ExtendedDocument t)
        {
            return true;
        }

        public Task<List<T>> GetMongoDataAll<T>() where T : IDocument
        {
            return DS.db.GetAllAsync<T>(a => true);
        }

        public async Task<List<T>> GetMongoDataPaged<T>(int pageNumber, int selectedCOunt) where T : IDocument
        {
            return await DS.db.GetPaginatedAsyncA<T>(a => true, ((pageNumber - 1) * selectedCOunt), selectedCOunt);

        }

        public async Task<List<T>> GetMongoDataFilterPaged<T>(string lowerName, int pageNumber, int selectedCOunt) where T : IDocument
        {

            return await DS.db.GetPaginatedAsyncA<T>(a => a.Name.ToLower().Contains(lowerName), ((pageNumber - 1) * selectedCOunt), selectedCOunt);

        }

        public async Task<long> GetMongoDataCount<T>(Expression<Func<T, bool>> filter) where T : IDocument
        {
            return await DS.db.CountAsync<T>(filter);

        }

        public async Task<List<T>> GetData<T>(Expression<Func<T, bool>> filter) where T : IDocument
        {
            return await DS.db.GetAllAsync<T>(filter);

        }


        /// <summary>
        /// /////
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>

        public static async Task<List<dynamic>> GetDataStatic(Type type, Expression<Func<dynamic, bool>> filter)
        {

            MethodInfo method = typeof(DataHelpers).GetMethod("GetDataStaticGeneric");
            MethodInfo genericMethod = method.MakeGenericMethod(type);

            var s = (genericMethod.Invoke(null, new object[] { filter }));

            return await (s as Task<List<dynamic>>);

        }

        public static List<T> GetDataStaticGeneric<T>(Expression<Func<T, bool>> filter) where T : IDocument
        {
            var s = DS.db.GetAllAsync<T>(filter).Result;
            return s;
        }

        public List<T> GetDataSync<T>(Expression<Func<T, bool>> filter) where T : IDocument
        {
            return DS.db.GetAll<T>(filter);

        }
        public async Task<T> GetOne<T>(Expression<Func<T, bool>> filter) where T : IDocument
        {
            return await DS.db.GetOneAsync<T>(filter); 
        }


        public Task<long> GetMongoDataCount<T>() where T : IDocument
        {
            return DS.db.CountAsync<T>(a => true);

        }

        internal Task<List<T>> GetMongoDataSearch<T>(string nameSearch) where T : IDocument
        {
            return DS.db.GetAllAsync<T>(a => a.Name.ToLower().Contains(nameSearch.ToLower()));
        }

        internal static ExtendedDocument MapProperties(ExtendedDocument selected, dynamic model)
        {
            var notAlloewd = typeof(ModelBase<>).GetProperties().Select(z => z.Name).ToList();
            notAlloewd.Add("Series");

            var propsSelected = selected.GetType().GetProperties().Where(z => z.GetCustomAttribute(typeof(DisplayNameAttribute)) != null);
            var propsModel = (model.GetType().GetProperties() as PropertyInfo[]).Select(z => z.Name);

            var commun = propsSelected.Where(a => propsModel.Contains(a.Name)
            && !notAlloewd.Contains(a.Name)
            && a.CanWrite);

            if (commun != null)
            {

                foreach (var prop in commun)
                {
                    var propModel = (model as ExtendedDocument).GetType().GetProperty(prop.Name);
                    Type typeModel = propModel?.PropertyType;
                    Type typeSelected = prop.PropertyType;
                    if (typeSelected.Equals(typeModel) == true)
                        prop.SetValue(selected, propModel.GetValue(model));
                }
            }

            return selected;
        }




        #region DATA METHODS



        #endregion




    }
}