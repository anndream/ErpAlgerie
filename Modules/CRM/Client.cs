using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Pages.Helpers;
using ErpAlgerie.Pages.Template;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{
    class Client : ModelBase<Client>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "CRM";
        public override string CollectionName { get; } = "Clients";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "AccountMultiple";
        public override bool ShowInDesktop { get; set; } = true;

        #endregion

        #region NAMING

        public override string NameField { get; set; } = "NomComplet";

        [DisplayName("Série")]
        [Column(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;

        #endregion

        #region CONSTRUCTOR

        public Client()
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

        #region PROPERTIES

        [Required]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable(true)]
        [IsBold]
        [DisplayName("Nom complet")]
        public string NomComplet { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable]
        [DisplayName("Identifiant")]
        public string Identifiant { get; set; }

        [ColumnAttribute(ModelFieldType.Select, "TypeClient")]
        [DisplayName("Type")]
        public string TypeClient { get; set; }

        
        [DisplayName("Groupe client")]
        [ColumnAttribute(ModelFieldType.Lien, "GroupeClient")]
        public ObjectId? GroupeClient { get; set; } = ObjectId.Empty;
        

        //[ShowInTableAttribute(false)]
        //[DisplayName("Domaine d'activité")]
        //[ColumnAttribute(ModelFieldType.Lien, "DomaineActivite")]
        //public ObjectId? lDomaineActivite { get; set; } = ObjectId.Empty;


        [Column(ModelFieldType.Lien,"ListePrix")]
        [DisplayName("Liste des prix")]
        public ObjectId? ListPrix { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Coordonées")]
        public string sepCoord { get; set; }


        [DisplayName("Contact principal du client")]
        [ColumnAttribute(ModelFieldType.Lien, "Contact")]
        public ObjectId? ContactPrincipal { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Région")]
        public string Region { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Mobile")]
        public string TelMob { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("N° Téléphone")]
        public string TelFix { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("N° Fax")]
        public string TelFax { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Site web")]
        public string Siteweb { get; set; }

        [DisplayName("Adresse de livraison par default")]
        [ColumnAttribute(ModelFieldType.LienField, "Adresses")] 
        public ObjectId? AdresseLivraison { get; set; } = ObjectId.Empty;

        //------------------------------------

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Adresses")]
        public string sepAderss { get; set; }

        [ShowInTableAttribute(false)]
        [ColumnAttribute(ModelFieldType.Table, "Adresse")]
        [DisplayName("Adresses")]
        [myTypeAttribute(typeof(Adresse))]
        public List<Adresse> Adresses { get; set; } = new List<Adresse>();


        // --------------------------------


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Infos compte")]
        public string sepcompte { get; set; }
         

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("NIF")]
        public string NIF { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("N° RC")]
        public string NRC { get; set; }


        // -------------------------------


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Contacts")]
        public string sepContacts { get; set; }


        [ShowInTableAttribute(false)]
        [ColumnAttribute(ModelFieldType.Table, "Contact")]
        [DisplayName("Contacts")]
        [myTypeAttribute(typeof(Contact))]
        public List<Contact> ClientContact { get; set; } = new List<Contact>();

        [ColumnAttribute(ModelFieldType.TextLarge, "")]
        [ShowInTable(false)]
        [DisplayName("Détails du client")]
        public string Details { get; set; }

        [ColumnAttribute(ModelFieldType.ImageSide, "Logo")]
        [ShowInTableAttribute(false)]
        [DisplayName("Logo")]
        public string Logo { get; set; }


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("COMPTABILITÉ")]
        public string sepCompte { get; set; }
         
        [DisplayName("Compte débiteur par default")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? CompteComptaDefault { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [ShowInTableAttribute(false)]
        [DisplayName("Limite de crédit")]
        public decimal MontantCreditMax { get; set; }


        [ColumnAttribute(ModelFieldType.Table, "TaxeLigne")]
        [ShowInTable(false)]
        [DisplayName("Taxes appliquées par default")]
        [myTypeAttribute(typeof(Taxe))]
        public List<TaxeLigne> TaxeLigne { get; set; } = new List<TaxeLigne>();

        #endregion

        #region ACTIONS

        


        #endregion

        #region LINKS
        //[BsonIgnore]
        //[DisplayName("Devis")]
        //[ColumnAttribute(ModelFieldType.LienButton, "Devis>Client")]
        //public string ClientDevis { get; set; }

        [BsonIgnore]
        [DisplayName("Factures de ventes")]
        [ColumnAttribute(ModelFieldType.LienButton, "Facture>Client")]
        public string lClient { get; set; }

        //[BsonIgnore]
        //[DisplayName("Commandes de ventes")]
        //[ColumnAttribute(ModelFieldType.LienButton, "CommandeVente>Client")]
        //public string lClientCommandeVente { get; set; }


        //[BsonIgnore]
        //[DisplayName("Bon de livraison")]
        //[ColumnAttribute(ModelFieldType.LienButton, "BonLaivraion>Client")]
        //public string BonLaivraionClient { get; set; }


        [BsonIgnore]
        [DisplayName("Écritures paiement")]
        [ColumnAttribute(ModelFieldType.LienButton, "EcriturePaiment>Client")]
        public string TierEcriturePaiment { get; set; }
        

        #endregion

        #region CARDS


        [BsonIgnore]
        decimal? Taux = null;
        public decimal? TauxSolde()
        {
            if (!isLocal && Taux == null)
            {
                var compteDebiteur = CompteSettings.getInstance().CompteDebiteur;
                var ecritureClient = DS.db.GetAll<EcritureJournal>(a => a.CompteJournal == compteDebiteur && a.Tier == this.Id);
                Taux = ecritureClient.Sum(z => z.MontantCredit) - ecritureClient.Sum(e => e.MontantDebit);
            }
            return Taux;
        }

        public override DocCard DocCardOne
        {
            get
            {
                return new DocCard()
                {

                    BulletIcon = "AccountCardDetails",
                    BulletTitle = "Solde client",
                    BulletValue = $"{TauxSolde()} DA"
                };
            }
        }

        public override DocCard DocCardTow
        {
            get
            {

                return new DocCard()
                {
                    BulletIcon = "AccountCardDetails",
                    BulletTitle = "Nom client",
                    BulletValue = this.NomComplet
                };
            }
        }

        #endregion
    }
}
