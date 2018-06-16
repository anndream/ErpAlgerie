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

namespace ErpAlgerie.Modules.CRM
{
    class LigneFacture : ModelBase<LigneFacture>, NoModule
    {
        [BsonIgnore]
        public override string ModuleName { get; set; } = "Stock";
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";
        public override string CollectionName { get; } = "Lignes Factures";
         
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string NameField { get; set; } = "Code";


        public override void Validate()
        {
            //base.Validate();
            //base.ValidateUnique();

            //   Nom
        
            if (Qts <= 0)
                throw new Exception("Vérifier la qts");


        }
        



        [ColumnAttribute(ModelFieldType.LienFetch, "lArticle>Code")]
        [DisplayName("Code")]
        [myType(typeof(Article))]
        public string Code { get; set; }


        [ShowInTableAttribute(false)]
        [DisplayName("Produit")]
        [ColumnAttribute(ModelFieldType.Lien, "Article")]
        public ObjectId? lArticle { get; set; } = ObjectId.Empty;



        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Désigniation")]
        public string Designiation { get; set; } 

        [ColumnAttribute(ModelFieldType.Numero, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [DisplayName("Qts")]
        public decimal Qts { get; set; }

        [ShowInTableAttribute(true)]
        [DisplayName("Unité de mesure")]
        [ColumnAttribute(ModelFieldType.Text, "UniteMesure")]
        public string UniteMesure { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [DisplayName("P.U.")]
        public decimal PrixUnitaire { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "true")]
        [BsonIgnore]
        [DisplayName("Détails")]
        public string sepStock { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [DisplayName("Remise sur Total")]
        public decimal RemiseSurTotal { get; set; }

        [ColumnAttribute(ModelFieldType.Numero, "")]
        [DisplayName("Remise % sur Total")]
        public decimal RemisePourcentageSurTotal { get; set; }

        [ShowInTableAttribute(false)]
        [DisplayName("Entrepot")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? lStock { get; set; } = ObjectId.Empty;



        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Résultats")]
        public string sepRes { get; set; }

        [ColumnAttribute(ModelFieldType.ReadOnly, "{0:C}")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("TOTAL H.T.")]
        public decimal TotalHT { get
            {
                var total = PrixUnitaire * Qts;
                var remise = (RemisePourcentageSurTotal / 100) * total;
                return total - (remise + RemiseSurTotal);
            }
        }


        [ColumnAttribute(ModelFieldType.Text, "")] 
        [ShowInTable(true)]
        [DisplayName("Détails")]
        public string Details { get; set; }

    }
}
