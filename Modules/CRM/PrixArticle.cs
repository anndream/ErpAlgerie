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
    class PrixArticle : ModelBase<PrixArticle>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Prix d'article";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "CurrencyUsd";
        public override bool ShowInDesktop { get; set; } = false;


        public override string NameField { get; set; } = "_NomArticle";

        #endregion

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public PrixArticle()
        {
        }


        [ShowInTableAttribute(false)]
        [DisplayName("Liste des prix")]
        [ColumnAttribute(ModelFieldType.Lien, "ListePrix")]
        public ObjectId? ListePrix_ { get; set; } = ObjectId.Empty;
        
        [ShowInTableAttribute(false)]
        [DisplayName("Article")]
        [ColumnAttribute(ModelFieldType.Lien, "Article")]
        public ObjectId? lArticle { get; set; } = ObjectId.Empty;

        [Column(ModelFieldType.LienFetch, "lArticle>Designiation")]
        [myType(typeof(Article))]
        [DisplayName("Nom de l'article")]
        public string _NomArticle { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Taux")]
        public decimal Taux { get; set; }


    }
}