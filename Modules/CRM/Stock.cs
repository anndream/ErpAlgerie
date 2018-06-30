using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class Stock : ModelBase<Stock>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Stocks";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Plus";
        public override bool ShowInDesktop { get; set; } = false;

        public override string NameField { get; set; } = "Designiation";

        #endregion

        




        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

            //   Nom
            if (string.IsNullOrWhiteSpace(Designiation))
                throw new Exception("Designiation est obligatoire");

        }
       

        public Stock()
        {

        } 

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }


        [ColumnAttribute(ModelFieldType.TextLarge, "")]
        [ShowInTable(true)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Adresse")]
        public string SepAdrr { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]   
        [DisplayName("Adresse")]
        public string Adresse { get; set; }



        #region LINKS
        [BsonIgnore]
        [DisplayName("Articles & produits")]
        [ColumnAttribute(ModelFieldType.LienButton, "Article")]
        public string lStock { get; set; }


        #endregion

    }
}
