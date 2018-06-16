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
    class ListePrix : ModelBase<ListePrix>
    {
        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Liste des prix";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "CurrencyUsd";
        public override bool ShowInDesktop { get; set; } = true;

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public ListePrix()
        {
        }


        public override string NameField { get; set; } = "Libelle";

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Libellé")]
        public string Libelle { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "Appliquer Pour les achats")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Achat ?")]
        public bool PourAchat { get; set; } = true;

        [ColumnAttribute(ModelFieldType.Check, "Appliquer pour les Ventes")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Ventes ?")]
        public bool PourVente { get; set; }

        #region LINKS

        [BsonIgnore]
        [DisplayName("Prix d'article")]
        [ColumnAttribute(ModelFieldType.LienButton, "PrixArticle>ListePrix_")]
        public string PrixArticle { get; set; }

        #endregion


    }
}