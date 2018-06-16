using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Pages.Helpers;
using ErpAlgerie.Pages.Reports;
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
    class Article : ModelBase<Article>
    {


        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Articles";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Plus";
        public override bool ShowInDesktop { get; set; } = true;

        public override string NameField { get; set; } = "Designiation";

        #endregion

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }


        public Article()
        {
            Series = MyModule()?.SeriesDefault;
        }


        [DisplayName("Séries")]
        [ColumnAttribute(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Code  identifiant")]
        public string Code { get; set; }

        [ShowInTableAttribute(false)]
        [DisplayName("Famille repas")]
        [ColumnAttribute(ModelFieldType.Lien, "GroupeArticle")]
        public ObjectId? lGroupeArticle { get; set; } = ObjectId.Empty;

        [BsonIgnore]
        [ShowInTableAttribute(true)]
        [DontShowInDetailAttribute]
        [DisplayName("Famille repas")]
        public string nGroupeArticle
        {
            get
            {
                return lGroupeArticle.GetObject("GroupeArticle")?.Name;
            }
        }

        [DisplayName("Catégorie")]
        [ColumnAttribute(ModelFieldType.Select, "CatégorieArticle")]
        public string CategorieArticle { get; set; }


        [DisplayName("Unité")]
        [ColumnAttribute(ModelFieldType.Lien, "UniteMesure")]
        public ObjectId? UniteMesure { get; set; }

        [ShowInTableAttribute(true)]
        [BsonIgnore]
        [DisplayName("Unité")]
        [ColumnAttribute(ModelFieldType.ReadOnly, "")]
        public string nUniteMesure
        {
            get
            {
                return UniteMesure.GetObject("UniteMesure")?.Name;
            }
        }



        #region VENTE

        //
        [ColumnAttribute(ModelFieldType.Table, "Variante")]
        [ShowInTable(false)]
        [DisplayName("Variante")]
        [myTypeAttribute(typeof(Variante))]
        public List<Variante> Variantes { get; set; } = new List<Variante>();


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Vente")]
        public string sepVente { get; set; }


        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Prix de vente HT.")]
        public decimal PrixVente { get; set; }

        #endregion


        [ShowInTableAttribute(false)]
        [DisplayName("Entrepot standard")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? lStock { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Stockage")]
        public string sepStcok { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [DisplayName("N° Lot")]
        public string NumLot { get; set; }

        [ColumnAttribute(ModelFieldType.ImageSide, "Image")]
        [ShowInTableAttribute(false)]
        [DisplayName("Image")]
        public string ImageRepas { get; set; }



        internal override ExtendedDocument Map(string mappedClass)
        {
            switch (mappedClass)
            {
                case "LigneFacture":
                    LigneFacture la = new LigneFacture()
                    {
                        lArticle = this.Id,
                        Qts = 1,
                        Designiation = this.Designiation,
                        PrixUnitaire = PrixVente,
                    };
                    return la;
                    break;

                case "LigneEcritureStock":

                    var ecritureStock = new LigneEcritureStock();
                    ecritureStock = DataHelpers.MapProperties(ecritureStock,this ) as LigneEcritureStock;

                    ecritureStock.Code = this.Code;
                    ecritureStock.Designiation = this.Designiation;
                    ecritureStock.lArticle = this.Id;
                    ecritureStock.Qts = 1;
                    ecritureStock.UniteMesure = this.nUniteMesure;

                    return ecritureStock;

                default:
                    return null;
                    break;
            }
        }





        #region LINKS

        [BsonIgnore]
        [DisplayName("Prix de l'article")]
        [ColumnAttribute(ModelFieldType.LienButton, "PrixArticle>lArticle")]
        public string PrixArticles { get; set; }




        #endregion


        #region ACTIONS


        [BsonIgnore]
        [ColumnAttribute(ModelFieldType.OpsButton, "EcritureStock")]
        [DisplayName("Écriture stock")]
        public string EcritureJournaleBtn { get; set; }

        public void EcritureStock()
        {
            var es = new EcritureStock();
            es = DataHelpers.MapProperties(es, this) as EcritureStock;

            es.DateEcriture = DateTime.Now;
            es.LigneEcritureStocks.Add(new LigneEcritureStock()
            {
                Code = this.Code,
                Designiation = this.Designiation,
                lArticle = this.Id,
                Qts = 1
            });

            DataHelpers.Shell.OpenScreenAttach(es, es.Name);
        }

        #endregion


        #region CARDS


        [BsonIgnore]
        decimal? _TauxStock = null;
        public decimal? TauxStock()
        {
            if (!isLocal && _TauxStock == null)
            {
                
                var ecritureStock = DS.db.GetAll<EcritureStock>(a =>a.DocStatus == 1 && a.LigneEcritureStocks.Any(z => z.lArticle == this.Id));
                var sorties = ecritureStock.Where(f=>f.ObjetEcriture == "Sortie de Matériel").SelectMany(e => e.LigneEcritureStocks).Where(e => e.lArticle == this.Id).Sum(s => s.Qts);
                 var entree = ecritureStock.Where(f => f.ObjetEcriture == "Réception Matériel").SelectMany(e => e.LigneEcritureStocks).Where(e => e.lArticle == this.Id).Sum(s => s.Qts);
                _TauxStock= entree - sorties;
            }
            return _TauxStock;
        }

        public override DocCard DocCardOne
        {
            get
            {
                return new DocCard()
                {

                    BulletIcon = "AccountCardDetails",
                    BulletTitle = "Solde Stock",
                    BulletValue = $"{TauxStock()}"
                };
            }
        }

        //public override DocCard DocCardTow
        //{
        //    get
        //    {

        //        return new DocCard()
        //        {
        //            BulletIcon = "AccountCardDetails",
        //            BulletTitle = "Nom client",
        //            BulletValue = this.NomComplet
        //        };
        //    }
        //}

        #endregion


    }
}
