using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{
    class ElvaSettings : ModelBase<ElvaSettings>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "APPLICATION";
        public override string CollectionName { get; } = "Paramétrages";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "Gears";
        public override bool ShowInDesktop { get; set; } = true;

        public override string NameField { get; set; } = "Libelle";

        public override bool IsInstance { get; set; } = true;

        #endregion


        public ElvaSettings()
        {
           
        }
         


        #region PROPERTIES

        [IsBoldAttribute(false)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("L'entreprise")]
        public string Societe { get; set; }

        [IsBoldAttribute(false)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Adresse")]
        public string Adresse { get; set; }

        [ColumnAttribute(ModelFieldType.Check, "Mono-poste")]
        [DisplayName("Utiliser BD locale")]
        public bool LocalBd { get; set; }


        [BsonIgnore]
        [DisplayName("Champs")]
        [Column(ModelFieldType.Separation,"")]
        public string sepChamp { get; set; }
         
        [LongDescription(@"les valeur possible sont mentionnée de dans
les valeur possible sont mentionnée de dans")]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Format d'affichage devise")]
        public string FormatDevis { get; set; }




        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Base des données")]
        public string sepVente { get; set; }
        

        [ShowInTableAttribute(false)]
        [DisplayName("Source base des données")]
        [ColumnAttribute(ModelFieldType.Lien, "DbSourceLink")]
        public ObjectId? DbSourceLink { get; set; } = ObjectId.Empty;



        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Logo application")]
        public string sepLogo { get; set; }

        [ColumnAttribute(ModelFieldType.Image, "param")] 
        [DisplayName("Logo sur l'application")]
        public string AppLogo { get; set; }


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Impression")]
        public string sepPrinting { get; set; }



        [DisplayName("Modifier modéle d'impression")]
        [Column(ModelFieldType.Button,"OpenTemplate")]
        public string BtnOpenTempalate { get; set; }

        public void OpenTemplate()
        {
            var file = "template.docx";
            Process.Start(file);
        }

        [SetColumn(1)]
        [IsBoldAttribute(false)]
        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        [DisplayName("Base des données en ligne:")]
        public string ActiveDB { get; set; }

        [ShowInTableAttribute(false)]
        [IsBold(false)]
        [ColumnAttribute(ModelFieldType.OpsButton, "SubmitDB")]
        [DisplayName("Transférer DB")]
        public double AddFactureBtn { get; set; }

      

        private void SubmitDB()
        {

            var db = DbSourceLink.GetObject("DbSourceLink") as DbSourceLink;
            if (db != null)
            {
                Properties.Settings.Default.MongoServerSettings = db.SourceIp;
                Properties.Settings.Default.dbUrl = db.DbName;
                this.ActiveDB = db.DbName;
               
                if (this.Save())
                {
                    Bootstrapper.SetupDb();
                    MessageBox.Show($"Source DB à été changée vers: {db.DbName}");
                }
                return;
            }
            else
            {
                MessageBox.Show("Aucune source DB sélectionnée!");
                return;
            }
        }

        ////////////////// ACTION

        public override bool Save()
        {
            isLocal = false; 
            var adr = Properties.Settings.Default.MongoServerSettings;
            ITestRepository2 defaulDb = new TestRepository2(adr, "Default");
            defaulDb.UpdateOne(this);
            return true;
        }


        [BsonIgnore]
        [ColumnAttribute(ModelFieldType.OpsButton, "ClearDataAll")]
        [DisplayName("Vider la base des données")]
        public string ClearDataAllBtn { get; set; }

        public void ClearDataAll()
        {
            var confirm = MessageBox.Show("Voulez-vous supprimer tout les données enregistrées?", "Confirmation", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {

                DataHelpers.ClearData();
                MessageBox.Show("Base des données éffacés"); 
            }
        }


        public override ElvaSettings getInstance()
        {
            var adr = Properties.Settings.Default.MongoServerSettings;
            ITestRepository2 defaulDb = new TestRepository2(adr, "Default");
            var instance = defaulDb.GetAll<ElvaSettings>(a => true);
            if (instance != null && instance.Count() > 0)
            {
                return instance.FirstOrDefault() as ElvaSettings;
            }
            else
            {
                var setting = new ElvaSettings() { isLocal = false };
                // try find default db
                try
                {
                    var Dbsources = DataHelpers.GetMongoDataSync("DbSourceLink") as IEnumerable<DbSourceLink>;
                    if(Dbsources != null)
                    {
                        var def = Dbsources.Where(a => a.DbName == "Default").FirstOrDefault();
                        if(def != null)
                        {
                            setting.DbSourceLink = def.Id;
                        }
                    }
                }
                catch { }
            

                defaulDb.AddOne<ElvaSettings>(setting);
                return defaulDb.GetOne<ElvaSettings>(a => true);
            }
        }

        //public static ElvaSettings getInstance()
        //{
        //    var instance =  DataHelpers.GetMongoDataSync("ElvaSettings") as IEnumerable<ElvaSettings>;
        //    if (instance != null && instance.Count() > 0)
        //    {
        //        return instance.FirstOrDefault() as ElvaSettings;
        //    }
        //    else
        //    {
        //        DS.db.AddOne<ElvaSettings>(new ElvaSettings() { isLocal = false });
        //        return DS.db.GetOne<ElvaSettings>(a => true);
        //    }
        //}

        public override string Name
        {
            get
            {
                return "Configuration";
            }
            set => base.Name = value;
        }
        #endregion


         
        public bool AppInitialized { get; set; } = false;

        public bool DemoUsed { get;  set; }
        public string UserName { get;  set; }
        public string Email { get; set; }
    }


    
}
