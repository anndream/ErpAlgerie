using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Pages.Helpers;
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
    class Facture : ModelBase<Facture>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = true;
        public override string ModuleName { get; set; } = "PDV";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string CollectionName { get; } = "Facture de vente";
        public override string IconName { get; set; } = "Book";
        public override bool ShowInDesktop { get; set; } = true;

        #endregion

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }


        public Facture()
        {
            Series = MyModule()?.SeriesDefault;
            this.Stock = CompteSettings.getInstance().StockSortie;
        }

        
        [DisplayName("Séries")]
        [ColumnAttribute(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;



        [Required]
        [DisplayName("Client")]
        [Column(ModelFieldType.Lien, "Client")]
        public ObjectId? Client { get; set; } = ObjectId.Empty;

        [DisplayName("Adresse de livraison")]
        [ColumnAttribute(ModelFieldType.LienField, "Client>Adresses")]
        [myType(typeof(Client))]
        public ObjectId? AdresseLivraison { get; set; } = ObjectId.Empty;
        

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.Check, "Inclus paiement")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Vente au comptoir?")]
        public bool VenteComptoir { get; set; }

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.Check, "Mettre à jour le stock")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Vente Sans BL?")]
        public bool UpdateStock { get; set; }

        [Column(ModelFieldType.LienFetch, "Client>NomComplet")]
        [myType(typeof(Client))]
        [ShowInTable]
        [DisplayName("Nom de client")]
        public string NomClient { get; set; }

         

        [Required]
        [ColumnAttribute(ModelFieldType.Date, "")]
        [ShowInTable(true)]
        [DisplayName("Date Création")]
        public DateTime DateCreation { get; set; } = DateTime.Now;



        [Required]
        [ColumnAttribute(ModelFieldType.Date, "")]
        [ShowInTable(true)]
        [DisplayName("Date d'échéance")]
        public DateTime DateEcheance { get; set; } = DateTime.Now;


        [DisplayName("Liste de prix")]
        [ColumnAttribute(ModelFieldType.Lien, "ListePrix")]
        public ObjectId? ListPrix { get; set; } = ObjectId.Empty;


        //[ColumnAttribute(ModelFieldType.Separation, "")]
        //[BsonIgnore]
        //[DisplayName("Lignes devis")]
        //public string sepLF { get; set; }

        [Required]
        [ShowInTableAttribute(false)]
        [ColumnAttribute(ModelFieldType.Table, "LigneFacture")]
        [DisplayName("Articles")]
        [myTypeAttribute(typeof(Article))]
        [AfterMapMethod("ApplyListePrix")]
        public List<LigneFacture> ArticleFacture { get; set; } = new List<LigneFacture>();


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Montant HT")]
        public string sepRésultats { get; set; }



        [ColumnAttribute(ModelFieldType.Devise, "")]
        [DisplayName("Remise sur Total HT.")]
        public decimal RemiseSurHt { get; set; }

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Montant HT.")]
        public decimal MontantHT
        {
            get
            {

                return (ArticleFacture.Sum(a => a.TotalHT));
            }
        }


        [ColumnAttribute(ModelFieldType.Numero, "")]
        [DisplayName("Remise % sur Total")]
        public decimal RemisePourcentageSurTotal { get; set; }

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Montant total HT.")]
        public decimal MontantTotalHT
        {
            get
            {
                var remisePerecentage = (RemisePourcentageSurTotal / 100) * MontantHT;
                return MontantHT - (RemiseSurHt + remisePerecentage);
            }
        }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Taxes")]
        public string sepTaxes { get; set; }


        [ColumnAttribute(ModelFieldType.Table, "TaxeLigne")]
        [ShowInTable(false)]
        [DisplayName("Taxes")]
        [myTypeAttribute(typeof(Taxe))]
        public List<TaxeLigne> TaxeLigne { get; set; } = new List<TaxeLigne>();

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Résultats")]
        public string sepMntants { get; set; }


        [DisplayName("Remise sur Montant TTC")]
        [Column(ModelFieldType.Devise, "")]
        public decimal RemiseGlobal { get; set; }


        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Somme taxes")]
        public decimal MontantTaxes
        {
            get
            {
                var montantBase = MontantTotalHT;

                Action<TaxeLigne> d = (tl) =>
                {
                    tl.MontantHT = MontantTotalHT;
                    tl.TauxPrecedent = montantBase;
                    montantBase += tl.MontantTaxe;
                };
                TaxeLigne.ForEach(d);

                return TaxeLigne.Sum(a => a.MontantTaxe);
            }
        }


        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Montant TTC.")]
        public decimal MontantTTC
        {
            get
            {

                return MontantTotalHT + MontantTaxes;
            }
        }

        [SetColumn(2)]
        [IsBold(true)]
        [ShowInTable]
        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Montant Global TTC.")]
        public decimal MontantGlobalTTC
        {
            get
            {

                return MontantTTC - RemiseGlobal;
            }
        }



        [ColumnAttribute(ModelFieldType.Separation, "false")]
        [BsonIgnore]
        [DisplayName("Comptes & paiement")]
        public string sepComptes { get; set; }


        [DisplayName("Compte débit")]
        [ColumnAttribute(ModelFieldType.Lien, typeof(CompteCompta))]
        public ObjectId? CompteDebit { get; set; } = ObjectId.Empty;


        [ColumnAttribute(ModelFieldType.Separation, "false")]
        [BsonIgnore]
        [DisplayName("Commande & Livraison")]
        public string sepPrix { get; set; }



        [DisplayName("N° de Bon de Commande du Client")]
        [Column(ModelFieldType.Text, "")]
        public string NumCommaneClient { get; set; }


        [ColumnAttribute(ModelFieldType.Date, "")]
        [ShowInTable(true)]
        [DisplayName("Date Commande")]
        public DateTime DateCommande { get; set; } = DateTime.Now;


        [DisplayName("Emplacement")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? Stock { get; set; } = ObjectId.Empty;

        [DisplayName("N° Commande")]
        [Column(ModelFieldType.Numero,"")]
        public int Position { get; set; }


        [DisplayName("Remarques")]
        [Column(ModelFieldType.TextLarge, "")]
        public string Remarques { get; set; }


        [ColumnAttribute(ModelFieldType.Separation, "false")]
        [BsonIgnore]
        [DisplayName("Status")]
        public string sepStatus { get; set; }

        [DisplayName("Status facture")]
        [Column(ModelFieldType.ReadOnly, "")]
        public string StatusFacture {

            get
            {
                if (DocStatus == 0)
                    return "Brouillon";

                if (EstAnnuler)
                    return "Annulée";

                if (EstPaye && EstDelivrer)
                    return "Terminée";

                if (EstDelivrer)
                    return "À Payée";

                return "À Délivrée";
            }
        }



         public bool EstPaye { get; set; }
        public bool EstDelivrer { get; set; }
        public bool EstAnnuler { get; set; }

        #region LINKS

         
        #endregion



        #region ACTIONS


        [BsonIgnore]
        [ColumnAttribute(ModelFieldType.OpsButton, "SaisirPaiement")]
        [DisplayName("Saisir paiement")]
        public string SaisirPaiementBtn { get; set; }

        public EcriturePaiment SaisirPaiement(bool open = true)
        {
            if (EnsureIsSavedSubmit() )
            {
                EcriturePaiment ep = new EcriturePaiment();
                ep = DataHelpers.MapProperties(ep, this) as EcriturePaiment;

                ep.Client = this.Client;
                ep.DateEcriture = DateTime.Now;
                ep.ModeDePiement = "Espèces";
                ep.ComtpeCredit = CompteSettings.getInstance().CompteDebiteur;
                ep.ComtpeDebit = CompteSettings.getInstance().ComptePaiementClient;
                ep.MontantPaye = this.MontantGlobalTTC;
                ep.ObjetEcriture = "Recevoir";
                ep.Utilisateur = DataHelpers.ConnectedUser?.Id;
                ep.RefFacture = this.Id;

                this.RefPaiement = ep.Id;
                if (this.Save() && open)
                    DataHelpers.Shell.OpenScreenAttach(ep, ep.Name);

                return ep;
            }
            return null;
        }

       
        #endregion

        #region CARDS

        public override DocCard DocCardOne
        {
            get
            {

                return new DocCard()
                {
                    BulletIcon = "Calculator",
                    BulletTitle = "Montant total",
                    BulletValue = this.MontantGlobalTTC.ToString("C2")
                };
            }
        }

        public override DocCard DocCardTow
        {
            get
            {

                return new DocCard()
                {
                    BulletIcon = "CalendarCheck",
                    BulletTitle = "Montant payé",
                    BulletValue = $"______"
                };
            }
        }

        #endregion


        #region AFTER MAP
        public LigneFacture ApplyListePrix(LigneFacture ligne)
        {
            if (this.ListPrix != null && this.ListPrix != ObjectId.Empty)
            {
                var list = ListPrix.GetObject("ListePrix") as ListePrix;
                var listPrix = DataHelpers.instanc().GetDataSync<PrixArticle>(a => a.ListePrix_ == ListPrix);
                if (listPrix != null)
                {
                    var firstPric = listPrix.FirstOrDefault(a => a.lArticle == ligne.lArticle);
                    if (firstPric != null)
                    {
                        ligne.PrixUnitaire = firstPric.Taux;
                    }
                }

            }

            return ligne;
        }
        #endregion


        #region SUBMIT

        public override string Status
        {
            get
            {
                return this.StatusFacture;
            }
        }

        public override bool Submit()
        {

            // Write stock entry
            if(this.UpdateStock)
            {
                EcritureStock es = new EcritureStock();

                es.DateEcriture = this.DateCreation;
                es.ObjetEcriture = "Sortie de Matériel";
                es.Remarques = $"à partir du facture {this.Name}";
                es.RefFacture = this.Id;
                foreach (var item in this.ArticleFacture)
                {
                    var line = new LigneEcritureStock();
                    line = DataHelpers.MapProperties(line, item) as LigneEcritureStock;

                    es.LigneEcritureStocks.Add(line);
                }
                es.StockSourceDefault = this.Stock;
                es.StockDestinationDefault = CompteSettings.getInstance().StockSortie;
                es.Save();
                es.Submit();
            }

            EcritureJournal ej = new EcritureJournal();
            ej.CompteJournal = CompteSettings.getInstance().CompteDebiteur;
            ej.MontantDebit = this.MontantGlobalTTC;
            ej.MontantCredit = 0;
            
            ej.ObjetEcriture = "Facture de vente";
            ej.RefDate = this.DateCreation;
            ej.RefNumber = this.Name;
            ej.CompteContre = CompteSettings.getInstance().CompteVente?.GetObject("CompteCompta")?.Name;
            ej.Tier = this.Client;
            ej.Utilisateur = DataHelpers.ConnectedUser?.Id;
            ej.Save();
            ej.Submit();

            EcritureJournal ejVente = new EcritureJournal();
            ejVente.CompteJournal = CompteSettings.getInstance().CompteVente;
            ejVente.MontantDebit = 0;
            ejVente.MontantCredit = this.MontantGlobalTTC;
            ejVente.ObjetEcriture = "Facture de vente";
            ejVente.RefDate = this.DateCreation;
            ejVente.RefNumber = this.Name;
            ejVente.Tier = this.Client;
            ejVente.CompteContre = this.NomClient;
            ejVente.Utilisateur = DataHelpers.ConnectedUser?.Id;
            ejVente.Save();
            ejVente.Submit();

            RefEcritureJournal.Add(ej.Id);
            RefEcritureJournal.Add(ejVente.Id);

            if (this.EstPaye)
            {
                var ecritureapiemenrt = new EcriturePaiment();
                ecritureapiemenrt.RefFacture = this.Id;
                ecritureapiemenrt = DataHelpers.MapProperties(ecritureapiemenrt, this) as EcriturePaiment;
                ecritureapiemenrt.Client = this.Client;
                ecritureapiemenrt.DateEcriture = this.DateEcheance;
                ecritureapiemenrt.ModeDePiement = "Espèces";
                ecritureapiemenrt.MontantPaye = this.MontantGlobalTTC;
                ecritureapiemenrt.ObjetEcriture = "Recevoir";
                ecritureapiemenrt.Remarques = "Enregistrer depuis factur " + this.Name;
                ecritureapiemenrt.Save();
                ecritureapiemenrt.Submit();

            }

            return base.Submit();
        }


        public override bool Cancel()
        {
            var ecriturePaiement = DS.db.GetAll<EcriturePaiment>(a => a.RefFacture == this.Id) as IEnumerable<EcriturePaiment>;
            var ecritureStock = DS.db.GetAll<EcritureStock>(a => a.RefFacture == this.Id) as IEnumerable<EcritureStock>;
            var ecriturejournal = DS.db.GetAll<EcritureJournal>(a => a.RefNumber == this.Name) as IEnumerable<EcritureJournal>;
            

            var result = base.Cancel();
            if (!result)
                return false;


            if (ecriturejournal != null && result == true)
            {
                foreach (var item in ecriturejournal)
                {
                    if (!item.Cancel())
                        continue;
                }
            }

            if (ecriturePaiement != null && result == true)
            {
                foreach (var item in ecriturePaiement)
                {
                    if (!item.Cancel())
                        continue;
                }
            }

            if (ecritureStock != null && result == true)
            {
                foreach (var item in ecritureStock)
                {
                    if (!item.Cancel())
                        continue;
                }
            }



            return result;
        }

        public override bool Delete(bool ConfirmFromUser = true)
        {
            
            var ecriturePaiement = DS.db.GetAll<EcriturePaiment>(a => a.RefFacture == this.Id) as IEnumerable<EcriturePaiment>;
            var ecritureStock = DS.db.GetAll<EcritureStock>(a => a.RefFacture == this.Id) as IEnumerable<EcritureStock>;
            var ecriturejournal = DS.db.GetAll<EcritureJournal>(a => a.RefNumber == this.Name) as IEnumerable<EcritureJournal>;



            var result = base.Delete(ConfirmFromUser);
            if (ecriturePaiement != null && result == true)
            {
                foreach (var item in ecriturePaiement)
                {
                    if (!item.Delete(false))
                        continue;
                }
            }

            if (ecriturejournal != null && result == true)
            {
                foreach (var item in ecriturejournal)
                {
                    if (!item.Delete(false))
                        continue;
                }
            }

            if (ecritureStock != null && result == true)
            {
                foreach (var item in ecritureStock)
                {
                    if (!item.Delete(false))
                        continue;
                }
            }

            return result;

            
        }
        #endregion



        #region REFERENCES

        public ObjectId? RefCommande { get; set; } = ObjectId.Empty;
        public ObjectId? RefDevis { get; set; } = ObjectId.Empty;
        public ObjectId? RefBonLivraison { get; set; } = ObjectId.Empty;
        public ObjectId? RefPaiement { get; set; } = ObjectId.Empty;
        public List<ObjectId?> RefEcritureJournal { get; set; } = new List<ObjectId?>(  );



        #endregion


    }


}
