using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Pages.Export;
using ErpAlgerie.Pages.Helpers;
using ErpAlgerie.Pages.Reports;
using MaterialDesignThemes.Wpf;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;
using Stylet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.Core.Module
{

    public enum OpenMode
    {
        Attach = 0,
        Detach = 1
    }

    [BsonIgnoreExtraElements(true, Inherited = true)]
    public abstract class ExtendedDocument : Document
    { 


        [DisplayName("Réf")]
        [DontShowInDetail()]
        [ShowInTable(true)]
        public new abstract string Name { get; set; }

        public abstract string CollectionName { get; }


        [ShowInTable]
        [DisplayName("Crée par")]
        [Column(ModelFieldType.Lien,"User")]
        [DontShowInDetail]
        [myType(typeof(User))]
        public ObjectId? CreatedBy { get; set; } = ObjectId.Empty;

        [BsonIgnore]
        public string CreatedByName { get
            {
                return CreatedBy?.GetObject("User")?.Name;
            }
        }

        [BsonIgnore]
        public virtual bool IsInstance { get; set; } = false;

        [BsonIgnore]
        public virtual OpenMode DocOpenMod { get; set; } = OpenMode.Attach;

        public ObjectId _etag { get; set; }
        /// <summary>
        /// 0 not handled - 1 Handled
        /// </summary>
        public int isHandled { get; set; } // 0 no / 1 handled

        /// <summary>
        /// TRUE NEWDOW - FALSE SAVED
        /// </summary>
        public bool isLocal { get; set; } = true;
        /// <summary>
        /// 0: Draft. 1: Submit (valide)
        /// </summary>
        public int DocStatus { get; set; }
        public virtual bool Submitable { get; set; } = false;

        [DisplayName("Crée le")]
        [DontShowInDetail()]
        [ShowInTable(true)]
        public override DateTime AddedAtUtc { get => base.AddedAtUtc; set => base.AddedAtUtc = value; }

        [DisplayName("Éditer le")]
        [DontShowInDetail()]
        public DateTime EditedAtUtc { get; set; }

        [BsonIgnore]
        [DisplayName("Status")]
        [DontShowInDetail()]
        [ShowInTable(true)]
        public virtual string Status { get
            {
                if (isLocal)
                    return "Nouveau";//, "#2196F3"); new Tuple<string, string>( , "#FFC400") new Tuple<string, string>(, "#00E676"), "#00E676")
                if (Submitable)
                {
                    return DocStatus == 0 ? "Brouillon" : "Validé";
                }
                return  "Enregistrer";
            }
        }

        [BsonIgnore] 
        [DontShowInDetail()] 
        public virtual string StatusColor
        {
            get
            {
                if (isLocal)
                    return "#2196F3";//, "#2196F3"); new Tuple<string, string>( , "#FFC400") new Tuple<string, string>(, "#00E676"), "#00E676")
                if (Submitable)
                {
                    return DocStatus == 0 ? "#FFC400" : "#00E676";
                }
                return "#00E676";
            }
        }

        // Method
        public virtual void DoFunction(string methodName)
        {
            try
            {
                var method = this.GetType().GetMethod(methodName);
                if (method != null)
                {
                    method.Invoke(this, null);
                }
            }
            catch (Exception s)
            {
                DataHelpers.Logger.LogError($"Method {methodName} not found in {this.CollectionName}\n{s.Message}");
            }
        }
        public virtual void DoFunction(string methodName, object[] parameters = null)
        {
            try
            {
                var method = this.GetType().GetMethod(methodName);
                if(method != null)
                {
                    method.Invoke(this, parameters);
                }
            }
            catch (Exception s)
            {
                DataHelpers.Logger.LogError($"Method {methodName} not found in {this.CollectionName}\n{s.Message}");
            }
        }

        public event EventHandler CloseEvent;

        public void CloseParent()
        {
            EventHandler handler = CloseEvent;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public string DefaultTemplate { get; set; }


        public virtual void ExportPDF(Type t, bool useDefault = false, bool UseHeader = true)
        {
            var exportPdf = new ExportManagerViewModel(t, this, "PDF",useDefault,UseHeader);
            exportPdf.UseHeader = UseHeader;
            if(useDefault!=true)
                DataHelpers.windowManager.ShowDialog(exportPdf);

        }


        public virtual string ExportPDF(Type t, string template, bool Useheader = true)
        {
            OvExport.OvPdfModelExport ov = new OvExport.OvPdfModelExport(t, template, this);
            ov.UseHeader = Useheader;
           return ov.GeneratePdf();
        }

        public virtual void ExportWORD(Type t, bool useDefault = false, bool UseHeader = true)
        {
            var exportPdf = new ExportManagerViewModel(t, this, "WORD", useDefault);
            exportPdf.UseHeader = UseHeader;
            if (useDefault != true)
               DataHelpers.windowManager.ShowDialog(exportPdf);            
        }

        public virtual string ExportWORD(Type t, string template, bool Useheader = true)
        {
            OvExport.OvPdfModelExport ov = new OvExport.OvPdfModelExport(t, template, this);
            ov.UseHeader = Useheader;
            return ov.GenerateOffice();
        }

        public virtual IEnumerable<dynamic> GetList(string link)
        {
            try
            {  
                try
                { 
                    var result = DataHelpers.GetMongoData(link, $"l{CollectionName}", Id.ToString(), true).OrderByDescending(a => a.AddedAtUtc).ToList<dynamic>();
                    return result;
                }
                catch 
                {
                    return new List<dynamic>();
                }
            }
            catch 
            {
                return new List<dynamic>();
            }
        }

        [BsonIgnore]
        [ShowInTable(true)]
        [DontShowInDetail]  
        public bool IsSelectedd { get; set; }

        internal virtual ExtendedDocument Map(string mappedClass)
        {
            return this;
        }

        [BsonIgnore]
        public virtual string ModuleName { get; set; }

        [BsonIgnore]
        public virtual bool ShowInDesktop { get; set; }
        [BsonIgnore]
        public virtual string SubModule { get; set; }


        [BsonIgnore]
        public virtual DocCard DocCardOne { get; } = null;


        [BsonIgnore]
        public virtual DocCard DocCardTow { get; } = null;


        [BsonIgnore]
        public virtual string IconName { get; set; } = "InformationOutline";
        public bool Has(string value,Type t)
        {
            var attrib = t.GetProperties();

            foreach (var item in attrib)
            {
                if (item.GetValue(this)?.ToString().Contains(value) == true)
                {
                    return true;
                }
            }
            return false;
        }


        public bool EnsureIsSavedSubmit()
        {
            if (this.isLocal || this.DocStatus != 1)
            {
                MessageBox.Show("Document non enregsitré/validé!");
                return false;
            }
            return true;
        }


        [BsonIgnore]
        public bool ForceIgniorValidatUnique = false;

        public string GetLiteralValue(PropertyInfo property)
        {
            var id = property.GetValue(this);
            var value = id?.ToString();
            if (property.PropertyType == typeof(ObjectId) || property.PropertyType == typeof(ObjectId?))
            {
                var isLink = (property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute);
                if (isLink != null && isLink.FieldType == ModelFieldType.Lien)
                {
                    var lienClass = isLink.Options;
                    var name = DataHelpers.GetById(lienClass, id as ObjectId?)?.Name;

                    value = name;
                }
                if (isLink != null && isLink.FieldType == ModelFieldType.LienField)
                {
                    if (isLink.Options.Contains('>'))
                    {
                        try
                        {
                            var lienClass = isLink.Options.Split('>');
                            var type_ = (property.GetCustomAttribute(typeof(myTypeAttribute)) as myTypeAttribute);

                            var className = lienClass[0].ToString(); // CLient
                            var field = lienClass[1].ToString(); // Adresses list ids

                            var inner = $"ErpAlgerie.Modules.CRM.{className}";
                            var pro = (Type.GetType(inner).GetProperty(field).GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute);
                            if (pro.FieldType == ModelFieldType.Table)
                            {
                                var classLien = pro.Options; // Adresse
                                var concrete = DataHelpers.GetById(classLien, id as ObjectId?)?.Name; // Client object
                                value = concrete;
                            }
                        }
                        catch (Exception s)
                        {
                            Console.WriteLine(s.Message );
                        }
                    }
                   
                }

            }

            return value;
        }
    }


    [BsonIgnoreExtraElements(true, Inherited = true)]
    public abstract class ModelBase<T> : ExtendedDocument, IModel where T : ExtendedDocument, new()
    {
        public override string Name { get; set; }
         
        private Type type; 

        public   virtual  T getInstance()
        {
            return new T();
        }

        public ModelBase()
        {
            Name = Id.Increment.ToString();
            type = typeof(T);

            //var module = this.GetModule()?.SeriesName?.GetObject("SeriesName");
            //if (module != null)
            //{
            //    this.Index = module.Indexe++;
                
            //}
        }

        public long Index { get; set; }



        

         
        public ModuleErp MyModule() {  
                return DataHelpers.GetModule(this) ;
            
        }
           
       

        [BsonIgnore]
        private string _NameField;
        [BsonIgnore]
        public virtual string NameField
        {
            get {
                var v = MyModule()?.NameFieldEntity;
                return (string.IsNullOrWhiteSpace(v)) ? "Id" : MyModule()?.NameFieldEntity;
            }
            set { _NameField = value; }
        }

       
        //public async static Task<T> GetByIdBase(ObjectId id)
        //{
        //    return await DS.db.GetOneAsync<T>(a => a.Id == id); 
        //}

        public void SetSeries()
        {
            var index = MyModule();
            bool named = false;
            if (index != null)
            {
                var usedSeries = (ObjectId?)this.GetType().GetProperty("Series")?.GetValue(this);
                if (usedSeries != null && usedSeries != ObjectId.Empty)
                {
                    var serie = index.SeriesNames.FirstOrDefault(a => a.Id == usedSeries);
                    if (serie != null)
                    {
                        string finaluffix = serie?.Sufix;
                        int? trailingZeroz = 5;
                        if (finaluffix.Contains("#") == true)
                        {
                            trailingZeroz = finaluffix.Count(a => a == '#');
                            finaluffix= finaluffix.Replace("#", "");
                        }
                        if (finaluffix.Contains("MM") == true)
                        {
                            finaluffix = finaluffix.Replace("MM", this.AddedAtUtc.ToString("MM"));
                        }
                        if (finaluffix.Contains("AA") == true)
                        {
                            finaluffix = finaluffix.Replace("AA", this.AddedAtUtc.ToString("yy"));
                        }
                        if (finaluffix.Contains("JJ") == true)
                        {
                            finaluffix = finaluffix.Replace("JJ", this.AddedAtUtc.ToString("dd"));
                        }
                       
                        if (isLocal)
                        {
                           
                            this.Index = serie.Indexe++;                           
                            this.Name = $"{finaluffix}{this.Index.ToString($"D{trailingZeroz}")}";
                            (index as IModel).Save();
                            named = true;
                        }
                        else
                        {
                            // index==0 means first update 
                            if (this.Index > 0)
                            {
                                
                                this.Name = $"{finaluffix}{this.Index.ToString($"D{trailingZeroz}")}";
                                (index as IModel).Save();
                                named = true;
                            }
                            else
                            {
                                this.Index = serie.Indexe++;
                                this.Name = $"{finaluffix}{this.Index.ToString($"D{trailingZeroz}")}";
                                (index as IModel).Save();
                                named = true;
                            }
                        }
                      
                    }
                }

            }
            if (!named)
            {
                var propert = GetType().GetProperty(this.NameField);
                var name = propert?.GetValue(this);
                this.Name = name?.ToString();
            }
        }
        public virtual bool Save()
        {

            if (this.isLocal)
            {
                if (FrameworkManager.CanAdd(type))
                {
                    SetSeries();
                    BeforeSave();
                    this.isLocal = false;
                    DS.db.AddOne<T>(this as T);
                    AfterSave();
                    return true;
                }
                MessageBox.Show("Vous n'avez pas l'autorisation pour créer!", "Action non autorisée");
                return false;
            }
            else
            {
                if (FrameworkManager.CanSave(type))
                {
                    SetSeries();
                    BeforeEdit();
                    DS.db.UpdateOne<T>(this as T);
                    AfterEdit();
                    return true;
                }
                MessageBox.Show("Vous n'avez pas l'autorisation pour modifier!", "Action non autorisée");
                return false;
            }
        }
        
        public virtual bool Delete(bool ConfirmFromUser = true)
        {
            if (FrameworkManager.CanDelete(type))
            {
                if (BeforeDelete(ConfirmFromUser))
                {
                    DS.db.DeleteOne<T>(this as T);                  
                    AfterDelete();
                    return true;
                }               
                return false;
            }
             
            MessageBox.Show("Vous n'avez pas l'autorisation de supprimer!", "Action non autorisée");
            return false;
        }

        public virtual bool Cancel()
        {
            if (FrameworkManager.CanCancel(type))
            {
              //  BeforeEdit();
                this.DocStatus = 0;
                DS.db.UpdateOne<T>(this as T);
                AfterEdit();
                return true;
            }
            else
            {
                MessageBox.Show("Vous n'avez pas l'autorisation pour annuler!", "Action non autorisée");
                return false;
            }
        }

        public virtual bool Submit()
        {
            if (FrameworkManager.CanValidate(type))
            {
                BeforeEdit();
                this.DocStatus = 1;
                DS.db.UpdateOne<T>(this as T);
                AfterEdit();
                return true;
            }
            MessageBox.Show("Vous n'avez pas l'autorisation pour valider!", "Action non autorisée");
            return false;
        }

        public virtual void AfterSave()
        {

        }

        public virtual void BeforeSave()
        {

            this.CreatedBy = DataHelpers.ConnectedUser?.Id;
            Validate();
        }

        public virtual void AfterDelete()
        {

        }

        public virtual bool BeforeDelete(bool Confirm = true)
        {
            if (Confirm)
            {
                var result = DataHelpers.windowManager.ShowMessageBox("Voulez-vous supprimer le document?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return (result == MessageBoxResult.Yes); 
            }
            return true;
        }

        public virtual void AfterEdit()
        {

        }

        public virtual void BeforeEdit()
        {
            Validate();
            this.EditedAtUtc = DateTime.Now;
        }

        //[BsonIgnore]


        //public virtual string Name { get
        //    {
        //        return CollectionName + " " + Id.ToString();
        //    }
        //}
        //public Guid Id { get ; set ; }
        // public int Version { get ; set ; }

        //public T GetById(Guid id)
        //{
        //    return DS.db.GetById<T>(id);
        //}

        public virtual void ValidateUnique()
        {
            if (ForceIgniorValidatUnique)
                return;

            var names = DS.db.Count<T>(a => a.Name == this.Name);
            if (names > 0 && isLocal)
            {
                var confirmation = MessageBox.Show($"Un document avec le méme identifiant existe déja '{this.Name}'\nVoullez-vous continuer", "Identifiant existe!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (confirmation == MessageBoxResult.No)
                {

                   throw new Exception($"Opération annuler");
                  
                }
            }
            else if (names > 1 && !isLocal)
            {
                var confirmation = MessageBox.Show($"Un document avec le méme identifiant existe déja '{this.Name} / {this.CollectionName}'\nVoullez-vous continuer", "Identifiant existe!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (confirmation == MessageBoxResult.No)
                {
                    throw new Exception($"Opération annuler");
                }

            }
        }

        /// <summary>
        /// Default validation method for the model
        /// </summary>
        public virtual void Validate()
        {
            var props = this.GetType().GetProperties();
            var required = props.Where(a => (a.GetCustomAttribute(typeof(RequiredAttribute)) as RequiredAttribute) != null);

            if (required != null)
            {
                foreach (var item in required)
                {
                    var display = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    var value = item.GetValue(this);
                    if (display != null)
                    {
                        if (value == null
                                       || (value as ObjectId?) == ObjectId.Empty
                                       || (value as IList)?.Count < 1)
                        {
                            throw new Exception($"Le champ <{display?.DisplayName}> est requis!");
                        }

                    }
                }
            }

           

        }
        /// <summary>
        /// Generate one instance of the class T with parameters (ID,Docstatus)
        /// </summary>
        /// <param name="args">1st param ID, 2nd docstatus (0 draft and 1 submit)</param>
        /// <returns></returns>
        public static T GetOneStatic(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// Generate one instance of the class T
        /// </summary>
        /// <returns></returns>
        public static T GetOneStatic()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public virtual IEnumerable<IDocument> GetList()
        {
            return DataHelpers.GetMongoData(type.Name) as IEnumerable<IDocument>;
        }


        
       
    }
}
