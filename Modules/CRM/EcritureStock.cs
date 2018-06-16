using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class EcritureStock : ModelBase<EcritureStock>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = true;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Écriture de Stock";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Transfer";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "ObjetEcriture";
        #endregion

        #region NAMING


        [DisplayName("Série")]
        [Column(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;

        #endregion

        #region CONSTRUCTOR

        public EcritureStock()
        {
            Series = MyModule()?.SeriesDefault;
        }

        #endregion

        #region VALIDATION

        public override void Validate()
        {
            base.Validate();
           // base.ValidateUnique();
        }

        #endregion


        #region PROPERTIES
        [Required]
        [ColumnAttribute(ModelFieldType.Select, "TypeEcritureStock")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Objet")]
        public string ObjetEcriture { get; set; }

        [ShowInTable]
        [ColumnAttribute(ModelFieldType.Date, "")]
        [DisplayName("Date de comptabilisation")]
        public DateTime DateEcriture { get; set; } = DateTime.Now;

        [DisplayName("Utilisateur")]
        [ColumnAttribute(ModelFieldType.Lien, "User")]
        public ObjectId? Utilisateur { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Lignes écriture")]
        public string sepname { get; set; }

        [ShowInTableAttribute(false)]
        [DisplayName("Entrepôt Source par default")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? StockSourceDefault { get; set; } = ObjectId.Empty;

        [DisplayName("Entrepôt Destination par default")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? StockDestinationDefault { get; set; } = ObjectId.Empty;

        [ShowInTable]
        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        [DisplayName("Quantité total")]
        public decimal QtsTotal
        {
            get
            {
                return LigneEcritureStocks.Sum(z => z.Qts);
            }
        }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Articles")]
        public string sepArticles { get; set; }

        [ColumnAttribute(ModelFieldType.Table, "LigneEcritureStock")]
        [ShowInTable(false)]
        [DisplayName("Lignes d'écritures")]
        [myTypeAttribute(typeof(Article))]
        public List<LigneEcritureStock> LigneEcritureStocks { get; set; } = new List<LigneEcritureStock>();

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Frais")]
        public string sepFrais { get; set; }




        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Informations complémentaires")]
        public string sepInfos { get; set; }

        [ShowInTable]
        [ColumnAttribute(ModelFieldType.TextLarge, "")]
        [DisplayName("Remarques")]
        public string Remarques { get; set; }

        #endregion


        #region SUBMIT

        public override bool Submit()
        {
            return base.Submit();
        }
        #endregion


        #region REFERENCES

        public ObjectId RefFacture { get; set; } = ObjectId.Empty;
        #endregion
    }
}