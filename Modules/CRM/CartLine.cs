using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
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
    class CartLine : ModelBase<CartLine>
    {


        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Repas";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Plus";
        public override bool ShowInDesktop { get; set; } = false;

       // public override string NameField { get; set; } = "Designiation";

        #endregion

        public override void Validate()
        {
            base.Validate(); 

        }

        public CartLine()
        {

        }

        public CartLine(Article article, ObjectId? listePrix)
        {
            this.article = article;
            this.aCuisinePosition = article.aCuisinePosition;
            ListePrix = listePrix;
            PricUnitaire = article.GetPrixFromList(listePrix);
        }

        public CartLine(Article article, Variante variante, ObjectId? listePrix)
        {
            this.article = article;
            this.aCuisinePosition = article.aCuisinePosition;
            this.variante = variante;
            ListePrix = listePrix;
            PricUnitaire = article.GetPrixFromList(listePrix); 
        }

        //public CartLine(Article article, Variante variante, ObjectId? listePrix)
        //{
        //    this.article = article;
        //    this.variante = variante;
        //    ListePrix = listePrix;
        //    PricUnitaire = article.GetPrixFromList(listePrix);
        //}

        string propInSetting = PosSettings.getInstance().NameProperty;

        public Article  article { get; set; }
        public Variante variante { get; set; }

        public ObjectId? ListePrix { get; set; } = null;


        
        public decimal OldQts { get; set; }

        [ShowInTable]
        [Column(ModelFieldType.ReadOnly, "")]
        [DisplayName("Qte")]
        public decimal Qts { get; set; } = 1;

        [Column(ModelFieldType.ReadOnly, "")]
        [ShowInTable]
        [DisplayName("Prix")]
        public decimal PricUnitaire { get; set; }


        [ShowInTable(false)]
        [DisplayName("Position cuisine")]
        [ColumnAttribute(ModelFieldType.Lien, "CuisinePosition")]
        public ObjectId? aCuisinePosition { get; set; } = ObjectId.Empty;

         
        public override string Name { get
            {
                
                if (string.IsNullOrWhiteSpace(propInSetting))
                {
                    return article.Name;
                }
                else
                {
                    var nameProp = article.GetType().GetProperty(propInSetting);
                    if(nameProp != null)
                    {
                        return article.GetType().GetProperty(propInSetting)?.GetValue(article)?.ToString();
                    }
                    else
                    {
                        return article.Name;
                    }
                   
                }
            } set { }
        }

        public decimal Total
        {
            get
            {
                return PricUnitaire * Qts;
            }
        }

        [Column(ModelFieldType.ReadOnly, "")]
        [DisplayName("Message")]
        [ShowInTable]
        public string Message { get; set; } = "";

    }
}
