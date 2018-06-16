using ErpAlgerie.Modules.Core.Data;
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
    class EcriturePaiment : ModelBase<EcriturePaiment>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = true;
        public override string ModuleName { get; set; } = "COMPTE";
        public override string CollectionName { get; } = "Écriture de paiement";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "CurrencyUsd";
        public override bool ShowInDesktop { get; set; } = true;

        #endregion

        #region NAMING


        [DisplayName("Série")]
        [Column(ModelFieldType.LienField, "MyModule()>SeriesNames")]
        [myType(typeof(ModuleErp))]
        public ObjectId? Series { get; set; } = ObjectId.Empty;

        #endregion

        #region CONSTRUCTOR

        public EcriturePaiment()
        {
            Series = MyModule()?.SeriesDefault;
            this.ComtpeCredit = CompteSettings.getInstance().CompteDebiteur;
            this.ComtpeDebit = CompteSettings.getInstance().ComptePaiementClient;
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
        [ColumnAttribute(ModelFieldType.Select, "TypeEcritureCompte")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Objet")]
        public string ObjetEcriture { get; set; }

        [ColumnAttribute(ModelFieldType.Date, "")]
        [DisplayName("Date de comptabilisation")]
        public DateTime DateEcriture { get; set; } = DateTime.Now;

        [DisplayName("Utilisateur")]
        [ColumnAttribute(ModelFieldType.Lien, "User")]
        public ObjectId? Utilisateur { get; set; } = ObjectId.Empty;


        [ColumnAttribute(ModelFieldType.Select, "ModePaiement")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Mode de paiement")]
        public string ModeDePiement { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Source / Destination")]
        public string sepSourceDest { get; set; }


        [DisplayName("Tier")]
        [ColumnAttribute(ModelFieldType.Lien, "Client")]
        public ObjectId? Client { get; set; } = ObjectId.Empty;

        [ShowInTableAttribute(false)]
        [DisplayName("Compte débit")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? ComtpeDebit { get; set; } = ObjectId.Empty;

        [ShowInTableAttribute(false)]
        [DisplayName("Compte crédit")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? ComtpeCredit { get; set; } = ObjectId.Empty;


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Taux")]
        public string sepArticles { get; set; }


        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(true)]
        [Required]
        [ShowInTable(true)]
        [DisplayName("Montant payé")]
        public decimal MontantPaye { get; set; }



        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Informations complémentaires")]
        public string sepInfos { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Cheque/Reference N°")]
        public string RefCheque { get; set; }

        [ColumnAttribute(ModelFieldType.Date, "")]
        [DisplayName("Date cheque")]
        public DateTime DateCheque { get; set; } = DateTime.Now;


        [DisplayName("Facture référence")]
        [Column(ModelFieldType.Lien, "Facture")]
        public ObjectId? RefFacture { get; set; }


        [ColumnAttribute(ModelFieldType.TextLarge, "")]
        [ShowInTable(false)]
        [DisplayName("Remarques")]
        public string Remarques { get; set; } 
        #endregion



        #region REFERENCES



        #endregion

        #region SUBMIT

        public override bool Submit()
        {
            EcritureJournal ej = new EcritureJournal();
            ej.CompteJournal = this.ComtpeDebit;
            ej.MontantDebit = this.MontantPaye;
            ej.MontantCredit = 0;
            ej.ObjetEcriture = "Entrée Paiement";
            ej.RefDate = this.DateEcriture;
            ej.RefEcrtirurePaiement = this.Id;
            ej.RefNumber = this.Name;
            ej.CompteContre = this.Client?.GetObject("Client")?.Name;
            ej.Tier = this.Client;
            ej.Utilisateur = DataHelpers.ConnectedUser?.Id;
            ej.Save();
            ej.Submit();

            EcritureJournal ejVente = new EcritureJournal();
            ejVente.RefEcrtirurePaiement = this.Id;
            ejVente.CompteJournal = this.ComtpeCredit;
            ejVente.MontantDebit = 0;
            ejVente.MontantCredit = this.MontantPaye;
            ejVente.ObjetEcriture = "Entrée Paiement";
            ejVente.RefDate = this.DateEcriture;
            ejVente.RefNumber = this.Name;
            ejVente.Tier = this.Client;
            ejVente.CompteContre = this.ComtpeDebit.GetObject("CompteCompta")?.Name;
            ejVente.Utilisateur = DataHelpers.ConnectedUser?.Id;
            ejVente.Save();
            ejVente.Submit();

            // Change Facture stat EstPaye
            var facture = this.RefFacture.GetObject("Facture") as Facture;
            if(facture != null)
            {
                facture.EstPaye = (this.MontantPaye == facture.MontantGlobalTTC);
                facture.Save();
            }


            return base.Submit();
        }

        public override bool Cancel()
        {
            var ecriturejournal = DS.db.GetAll<EcritureJournal>(a => a.RefEcrtirurePaiement == this.Id) as IEnumerable<EcritureJournal>;
            var result = base.Cancel();
            if (ecriturejournal != null && result == true)
            {
                foreach (var item in ecriturejournal)
                {
                    if (!item.Delete())
                        continue;
                }
            }

            return result;
        }

        public override bool Delete(bool ConfirmFromUser = true)
        {
            var ecriturejournal = DS.db.GetAll<EcritureJournal>(a => a.RefEcrtirurePaiement == this.Id) as IEnumerable<EcritureJournal>;
            var result = base.Delete(ConfirmFromUser);
            if (ecriturejournal != null && result == true)
            {
                foreach (var item in ecriturejournal)
                {
                    if (!item.Delete())
                        continue;
                }
            }

            return result;
           

        }
        #endregion
    }
}
