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
    class EcritureJournal : ModelBase<EcritureJournal>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = true;
        public override string ModuleName { get; set; } = "COMPTES";
        public override string CollectionName { get; } = "Écriture de journal";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "TableEdit";
        public override bool ShowInDesktop { get; set; } = false;

        #endregion

        #region NAMING


        [DisplayName("Série")]
        [Column(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;

        #endregion

        #region CONSTRUCTOR

        public EcritureJournal()
        {
            Series = MyModule()?.SeriesDefault;
        }

        #endregion

        #region VALIDATION

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }

        #endregion



        [ColumnAttribute(ModelFieldType.Select, "TypeEcritureJournal")]
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
        [DisplayName("Données")]
        public string sepDonnes { get; set; }

        [ShowInTable]
        [Required]
        [DisplayName("Tier")]
        [ColumnAttribute(ModelFieldType.Lien, "Client")]
        public ObjectId? Tier { get; set; } = ObjectId.Empty;


        [Required]
        [ShowInTable]
        [DisplayName("Compte")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? CompteJournal { get; set; } = ObjectId.Empty;

        [ShowInTable]
        [ColumnAttribute(ModelFieldType.Text, "")]       
        [DisplayName("Contre compte")]
        public string CompteContre { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(true)]
        [Required]
        [ShowInTable(true)]
        [DisplayName("Montant débit")]
        public decimal MontantDebit { get; set; }


        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(true)]
        [Required]
        [ShowInTable(true)]
        [DisplayName("Montant crédit")]
        public decimal MontantCredit { get; set; }



        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Informations complémentaires")]
        public string sepInfos { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Réf N°")]
        public string RefNumber { get; set; }

        [ColumnAttribute(ModelFieldType.Date, "")]
        [DisplayName("Date Réf")]
        public DateTime RefDate { get; set; } = DateTime.Now;

        [ColumnAttribute(ModelFieldType.TextLarge, "")]
        [ShowInTable(false)]
        [DisplayName("Remarques")]
        public string Remarques { get; set; }



        #region REFERENCES

        public ObjectId? RefEcrtirurePaiement { get; set; } = ObjectId.Empty;
        #endregion
    }
}
