using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDbGenericRepository.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Pages.Template
{ 
    public class FiltreViewModel<T> : Screen, IDisposable where T:IDocument,new()
    {
        public object Model { get; set; }
        public Dictionary<string,PropertyInfo> Properties { get; set; } = new Dictionary<string, PropertyInfo>();
        private PropertyInfo _SelectedProeprty;
        private Thread t;
        private List<object> allData;
       
        public PropertyInfo SelectedProeprty { get {

                return _SelectedProeprty;
            } set
            {
                _SelectedProeprty = value;
                t = new Thread(new ThreadStart(PopulatePossibleValues));
                t.Start();
            }
        }

        public string StatusLabel { get; set; }


        public void PopulatePossibleValues()
        {
            // Get possible values
            PossibleValues.Clear();
            var type = GetTypeName(SelectedProeprty.PropertyType);
            HashSet<string> collections = new HashSet<string>();
            foreach (var item in Inputs)
            {
                var val = _SelectedProeprty.GetValue(item, null)?.ToString();
                if (val != null)
                {
                    if (type == "ObjectId")
                    {
                        try
                        {
                            var attributes = _SelectedProeprty.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                            var option = attributes.Options;
                            if (string.IsNullOrWhiteSpace(option) || option.Contains(">") || string.IsNullOrWhiteSpace(val))
                                continue;
                            var link = DataHelpers.GetMongoData(option, "Id", ObjectId.Parse(val), true) as IEnumerable<ExtendedDocument>;
                            if (link == null)
                                continue;
                            collections.Add(link.FirstOrDefault()?.Name);
                            StatusLabel = $"Récupération des données - {link.FirstOrDefault()?.Name}";
                            NotifyOfPropertyChange("StatusLabel");
                        }
                        catch (Exception s)
                        {
                            MessageBox.Show(s.Message);
                        }
                    }
                    else
                    {
                        collections.Add(val as string);
                    }
                }
            }
            PossibleValues.AddRange(collections);
            StatusLabel = $"Términé";
            NotifyOfPropertyChange("StatusLabel");
            NotifyOfPropertyChange("PossibleValues");
        }

        public List<T> Inputs { get; set; } 
        public List<T> Result { get; set; } = new List<T>();
        public BindableCollection<string> PossibleValues { get; set; } = new BindableCollection<string>();
        public string Valeur { get; set; }

        // Conditions
        public string ConditionsSelected { get; set; } = "ressemble";
        public IEnumerable<string> Conditions { get
            {
                return new List<string>()
                {
                    "égale",
                    "ressemble",
                    "déffirent de",
                    "inférieur à",
                    "supérieur à"
                };
            }
        }


        public void SetInputs(List<T> items)
        {
             Inputs = new List<T>();
             Inputs = items;
            if (Inputs != null && Inputs.Count > 0)
            {
                var atype = Inputs[0];

                Type t = atype.GetType();
                PropertyInfo[] props = t.GetProperties();
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (PropertyInfo prp in props)
                {
                    //
                    //|| ((ColumnAttribute)a).FieldType == ModelFieldType.Lien

                    if(prp.Name == "Name")
                    {
                        Properties.Add("Réf", prp);
                    }

                    var forbiden = new string[] { ">","<","()","_"," " };
                    Object[] attrs = prp.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    Object[] sepa = prp.GetCustomAttributes(typeof(ColumnAttribute), true);
                    var attValide = sepa.Where(a => ((ColumnAttribute)a).FieldType == ModelFieldType.Separation
                    || ((ColumnAttribute)a).FieldType == ModelFieldType.Button
                    || ((ColumnAttribute)a).FieldType == ModelFieldType.OpsButton
                    || ((ColumnAttribute)a).FieldType == ModelFieldType.Image
                    || ((ColumnAttribute)a).FieldType == ModelFieldType.Table
                    || ((ColumnAttribute)a).FieldType == ModelFieldType.LienButton
                    || forbiden.Contains(((ColumnAttribute)a).Options));
                    if (attValide.Any())
                        continue;

                    foreach (var att in attrs)
                    {
                        DisplayNameAttribute attribute = att as DisplayNameAttribute;
                        if ((attribute != null) && (attribute != DisplayNameAttribute.Default))
                        {
                            try
                            {
                                Properties.Add(attribute.DisplayName, prp);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
        public FiltreViewModel()
        {
               
        }

        public FiltreViewModel(List<object> allData)
        {
            this.allData = allData;
        }

        public void doValider()
        {
            FilterData();
            this.RequestClose(true);
        }


        public void doValiderAndNext()
        {
            FilterData(); 
            ConditionsSelected = String.Empty;
            Valeur = String.Empty;

            NotifyOfPropertyChange("SelectedProeprty");
            NotifyOfPropertyChange("ConditionsSelected");
            NotifyOfPropertyChange("Valeur");

        }

        public void FilterData()
        {

            if (string.IsNullOrWhiteSpace(ConditionsSelected))
            {
                DataHelpers.windowManager.ShowMessageBox("Vérifier les conditions");
                return;
            }
            if (string.IsNullOrWhiteSpace(Valeur))
            {
                DataHelpers.windowManager.ShowMessageBox("Vérifier les valeurs");
                return;
            }

            var query = Inputs ;
            //var query = new List<IDocument>();
             //query =   ((System.Collections.Generic.List<IDocument>) Inputs) ;
            if (GetTypeName(SelectedProeprty.PropertyType) == "DateTime")
            {
                var _Valeur = DateTime.Parse(Valeur);
                switch (ConditionsSelected)
                {

                
                    case "égale":
                        var result = query.Where( a => (SelectedProeprty.GetValue(a, null) as DateTime?)?.Date == (_Valeur.Date));
                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(a => !((SelectedProeprty.GetValue(a, null) as DateTime?)?.Date == _Valeur.Date));
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;
                    case "inférieur à":
                        var resultInf = query.Where(a => (SelectedProeprty.GetValue(a, null) as DateTime?)?.Ticks <= (_Valeur.Ticks));
                        Result = resultInf.ToList();
                        Inputs = Result;
                        break;
                    case "supérieur à":
                        var resultSup = query.Where(a => (SelectedProeprty.GetValue(a, null) as DateTime?)?.Ticks >= (_Valeur.Ticks));
                        Result = resultSup.ToList();
                        Inputs = Result;
                        break;

                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "Int32")
            {

                var _Valeur = int.Parse(Valeur);
                switch (ConditionsSelected)
                {
                    case "ressemble":
                        try
                        {
                            var resultd = query.Where(a => SelectedProeprty.GetValue(a, null).ToString().Contains(Valeur));
                            Result = resultd.ToList();
                            Inputs = Result;
                        }
                        catch
                        { }


                        break;
                    case "égale":
                        var result = query.Where(a => (int)SelectedProeprty.GetValue(a, null) == (_Valeur));
                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(a => (int)SelectedProeprty.GetValue(a, null) != _Valeur);
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;
                    case "inférieur à":
                        var resultInf = query.Where(a => (int)SelectedProeprty.GetValue(a, null) <= (_Valeur));
                        Result = resultInf.ToList();
                        Inputs = Result;
                        break;
                    case "supérieur à":
                        var resultSup = query.Where(a => (int)SelectedProeprty.GetValue(a, null) >= (_Valeur));
                        Result = resultSup.ToList();
                        Inputs = Result;
                        break;

                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "Decimal")
            {

                var _Valeur = decimal.Parse(Valeur);
                switch (ConditionsSelected)
                {
                    case "ressemble":
                        try
                        {
                            var resultd = query.Where(a => SelectedProeprty.GetValue(a, null).ToString().Contains(Valeur));
                            Result = resultd.ToList();
                            Inputs = Result;
                        }
                        catch 
                        {}

                        break;
                    case "égale":
                        var result = query.Where(a => (decimal)SelectedProeprty.GetValue(a, null) == (_Valeur));
                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(a => (decimal)SelectedProeprty.GetValue(a, null) != _Valeur);
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;
                    case "inférieur à":
                        var resultInf = query.Where(a => (decimal)SelectedProeprty.GetValue(a, null) <= (_Valeur));
                        Result = resultInf.ToList();
                        Inputs = Result;
                        break;
                    case "supérieur à":
                        var resultSup = query.Where(a => (decimal)SelectedProeprty.GetValue(a, null) >= (_Valeur));
                        Result = resultSup.ToList();
                        Inputs = Result;
                        break;

                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "Double")
            {

                var _Valeur = double.Parse(Valeur);
                switch (ConditionsSelected)
                {
                    case "ressemble":
                        try
                        {
                            var resultd = query.Where(a => SelectedProeprty.GetValue(a, null).ToString().Contains(Valeur));
                            Result = resultd.ToList();
                            Inputs = Result;
                        }
                        catch
                        { }
                        break;
                    case "égale":
                        var result = query.Where(a => (double) SelectedProeprty.GetValue(a, null) == (_Valeur));
                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(a => (double)SelectedProeprty.GetValue(a, null) != _Valeur);
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;
                    case "inférieur à":
                        var resultInf = query.Where(a => (double)SelectedProeprty.GetValue(a, null) <= (_Valeur));
                        Result = resultInf.ToList();
                        Inputs = Result;
                        break;
                    case "supérieur à":
                        var resultSup = query.Where(a => (double)SelectedProeprty.GetValue(a, null) >= (_Valeur));
                        Result = resultSup.ToList();
                        Inputs = Result;
                        break;

                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "String")
            {
                switch (ConditionsSelected)
                {

                    case "ressemble":
                        try
                        {
                            var results = query.Where(s =>
                            {
                                var value = SelectedProeprty.GetValue(s, null);
                                if (value == null) return false;
                                return value.ToString().ToLower().Contains(Valeur.ToLower());
                            });

                           
                            Result = results.ToList();
                            Inputs = Result;

                        }
                        catch 
                        {}

                        break;
                    case "égale":
                        var result = query.Where(s =>
                        {
                            var value = SelectedProeprty.GetValue(s, null);
                            if (value == null) return false;
                            return value.ToString() == (Valeur);
                        });

                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(s =>
                        {
                            var value = SelectedProeprty.GetValue(s, null);
                            if (value == null) return false;
                            return !(value.ToString() == (Valeur));
                        });
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;
                  

                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "ObjectId")
            {
                var attributes = SelectedProeprty.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                var option = attributes.Options;
                var link = DataHelpers.GetMongoData(option, "Name", Valeur, true).FirstOrDefault();
                if (link == null && ConditionsSelected != "ressemble")
                    return;
                switch (ConditionsSelected)
                {
                  

                    case "égale":
                        var result = query.Where(a => SelectedProeprty.GetValue(a, null).ToString().Equals(link.Id.ToString()));
                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(a => !SelectedProeprty.GetValue(a, null).ToString().Contains(link.Id.ToString()));
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;


                    default:
                        break;
                }
            }
            if (GetTypeName(SelectedProeprty.PropertyType) == "Boolean")
            {
                switch (ConditionsSelected)
                {

                    case "ressemble":
                        try
                        {
                            var results = query.Where(s =>
                            {
                                var value = SelectedProeprty.GetValue(s, null);
                                if (value == null) return false;
                                return value.ToString().Contains(Valeur);
                            });


                            Result = results.ToList();
                            Inputs = Result;

                        }
                        catch
                        { }

                        break;
                    case "égale":
                        var result = query.Where(s =>
                        {
                            var value = SelectedProeprty.GetValue(s, null);
                            if (value == null) return false;
                            return value.ToString() == (Valeur);
                        });

                        Result = result.ToList();
                        Inputs = Result;

                        break;
                    case "déffirent de":
                        var resultDef = query.Where(s =>
                        {
                            var value = SelectedProeprty.GetValue(s, null);
                            if (value == null) return false;
                            return !(value.ToString() == (Valeur));
                        });
                        Result = resultDef.ToList();
                        Inputs = Result;
                        break;


                    default:
                        break;
                }
            }

        }

        public static string GetTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            bool isNullableType = nullableType != null;

            if (isNullableType)
                return nullableType.Name;
            else
                return type.Name;
        }

        public void doAnnuler()
        {
            this.RequestClose();
        }

        public void Dispose()
        {
         }
    }
}
