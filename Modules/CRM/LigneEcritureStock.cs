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
    class LigneEcritureStock : ModelBase<LigneEcritureStock>, NoModule
    { 
        [BsonIgnore]
        public override string ModuleName { get; set; } = "STOCK";
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";
        public override string CollectionName { get; } = "Lignes écriture stock";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach; 

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
         
         

        [ShowInTableAttribute(false)]
        [DisplayName("Entrepôt source")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? lStockSource { get; set; } = ObjectId.Empty;



        [ShowInTableAttribute(false)]
        [DisplayName("Entrepôt destination")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? lStockDestiation { get; set; } = ObjectId.Empty;


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Résultats")]
        public string sepRes { get; set; }

        


        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable(true)]
        [DisplayName("Détails")]
        public string Details { get; set; }

    }
}