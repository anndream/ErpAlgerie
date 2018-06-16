using ErpAlgerie.Modules.Core;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{
    public class ModuleErp : ModelBase<ModuleErp>
    {

        public override bool Submitable { get; set; } = false;
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public ModuleErp()
        {
        }
        public override string CollectionName { get; } = "Modules";
        public override string ModuleName { get; set; } = "Configuration";
        public override string Name { get { return Libelle; } set => base.Name = value; }

        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Libellé")]
        public string Libelle { get; set; }

        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [DisplayName("Lien Classe")]
        public string ClassName { get; set; }


        [DisplayName("Icon d'affichage")]
        [Column(ModelFieldType.Text, "")]
        public string ModuleIcon { get; set; } = "ChevronRight";


        [Column(ModelFieldType.Check, "Singltone?")]
        [DisplayName("Une instance")]
        public bool IsInstanceModule { get; set; }


        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        [DisplayName("Instance function")]
        public string InstanceFunction { get; set; } = "getInstance";

        [ColumnAttribute(ModelFieldType.Check, "Afficher sur le bureau")]
        [IsBoldAttribute(false)]
        [ShowInTable(false)]
        [DisplayName("Accée rapide")]
        public bool EstAcceRapide { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Groupe module")]
        public string Grp { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable(true)]
        [DisplayName("Libellé de groupe module")]
        public string GroupeModule { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Champ d'identification")]
        public string NameFieldEntity { get; set; }

        [SetColumn(2)]
        [DisplayName("Mettre à jour")]
        [Column(ModelFieldType.Button, "UpdateDataRefs")]
        public string UpdateDataRefsBtn { get; set; }

        public void UpdateDataRefs()
        {
            var collection = Type.GetType(ClassName);
            var items = DataHelpers.GetMongoDataSync(collection.Name) as IEnumerable<ExtendedDocument>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.ForceIgniorValidatUnique = true;
                    if (!(item as IModel).Save())
                        return;
                }

                MessageBox.Show("Documents à jour!");
            }
        }
        //[ShowInTableAttribute(false)]
        //[DisplayName("Série par default")]
        //[ColumnAttribute(ModelFieldType.Lien, "SeriesName")]
        //public ObjectId? SeriesDefault { get; set; } = ObjectId.Empty;

        [DisplayName("Tempaltes d'impression")]
        [Column(ModelFieldType.Button, "OpenPrint")]
        public string OpenPrintBtn { get; set; }

        public void OpenPrint()
        {
            var type = Type.GetType(ClassName);
            var instance = Activator.CreateInstance(type);
            (instance as ExtendedDocument).ExportWORD(type);
        }

        [DisplayName("Série par default")]
        [ColumnAttribute(ModelFieldType.LienField, "SeriesNames")]
        public ObjectId? SeriesDefault { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Modéle d'impression par Default")]
        public string DefaultTemplateName { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Séries")]
        public string sepSeries { get; set; }

        [ColumnAttribute(ModelFieldType.Table, "SeriesName")]
        [ShowInTable(false)]
        [DisplayName("Séries disponible")]
        [myTypeAttribute(typeof(SeriesName))]
        public List<SeriesName> SeriesNames { get; set; } = new List<SeriesName>();




        [Column(ModelFieldType.BaseButton, "ReloadModules")]
        [DisplayName("Recharger modules")]
        public string ReloadModulesAction { get; set; }



        public void ReloadModules()
        {
            FrameworkManager.UpdateModules(true);
            DataHelpers.Shell.SetupSideMenu().Wait();
        }



        public override bool Save()
        {
            var result = base.Save();
            if (result)
            {

                // update loaded modules in DataHelpers
                var modules = DataHelpers.GetMongoDataSync("ModuleErp");
                DataHelpers.Modules = modules as IEnumerable<ModuleErp>;


            }
            return result;

        }
    }
}