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
    class PosSettings : ModelBase<PosSettings>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "PDV";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string CollectionName { get; } = "Paramétres POS";
        public override string IconName { get; set; } = "Settings";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "Titre";
        #endregion

        public override bool IsInstance { get; set; } = true;

        public PosSettings()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                Printers.Add(printer);
            }
        }
        //public override string Name
        //{
        //    get
        //    {
        //        return "Paramétres POS";
        //    }
        //    set => base.Name = value;
        //}
        public string Titre { get; set; } = "Paramétres POS";

        public static PosSettings getInstance()
        {
            var instance = DS.db.Count<PosSettings>(a => true);
            if (instance > 0)
            {
                return DS.db.GetOne<PosSettings>(t => true);
            }
            else
            {
                DS.db.AddOne<PosSettings>(new PosSettings() { isLocal = false });
                return DS.db.GetOne<PosSettings>(a => true);
            }
        }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Identifiant d'article")]
        public string NameProperty { get; set; } = "Designiation";

        [DisplayName("Client anonyme par default")]
        [Column(ModelFieldType.Lien, "Client")]
        public ObjectId DefaultClient { get; set; }


        [DisplayName("Utiliser prépayé par default")]
        [ColumnAttribute(ModelFieldType.Check, "Toujours prépayé?")]
        public bool EstPrepayeOnly { get; set; }

        [DisplayName("Générer la facture toujours!")]
        [ColumnAttribute(ModelFieldType.Check, "Facturer")]
        public bool EstFacturer { get; set; }
         

        [DisplayName("Utilisateur peut modifier les prix?")]
        [ColumnAttribute(ModelFieldType.Check, "Changer Prix de vente")]
        public bool PeutModifierPrix { get; set; }

        [Required]
        [DisplayName("Séries facture par défault")]
        [LongDescription("Choisir une Série présente dans la liste des séries facture")]
        [ColumnAttribute(ModelFieldType.Lien, "SeriesName")]
        public ObjectId? SeriesFacture { get; set; }


        [DisplayName("Vente")]
        [Column(ModelFieldType.Separation, "")]
        public string sepVente { get; set; }


        [Column(ModelFieldType.Lien,"ListePrix")]
        [DisplayName("Liste des prix par default")]
        public ObjectId? ListPrixParDefault { get; set; } = ObjectId.Empty;


        [DisplayName("Impression")]
        [Column(ModelFieldType.Separation, "")]
        public string sepPrinting { get; set; }

        [DisplayName("Imprimer sans en-tête!")]
        [ColumnAttribute(ModelFieldType.Check, "SANS EN-TÊTE")]
        public bool DontUseHeader { get; set; } = true;


        [DisplayName("Cacher image repas!")]
        [ColumnAttribute(ModelFieldType.Check, "_")]
        public bool HideImageRepas { get; set; } = true;



        [DisplayName("Ticket cuisine obligatoire")]
        [ColumnAttribute(ModelFieldType.Check, "TICKET CUISINE")]
        public bool PrintCuisineAlways { get; set; } = true;

        [DisplayName("Imprimer le ticket de vente ?")]
        [ColumnAttribute(ModelFieldType.Check, "Imprimer le ticket")]
        public bool EstImprimer { get; set; }

        [DisplayName("Imprimer commande cuisine ?")]
        [ColumnAttribute(ModelFieldType.Check, "Imprimer cuisine")]
        public bool EstImprimerCuisine { get;  set; }


        [Required]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Template d'impression")]
        public string NomTemplate { get; set; }


        [Required]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Template d'impression CUISINE")]
        public string NomTemplateCuisine { get; set; }


        [ColumnAttribute(ModelFieldType.Select, "this>Printers")]
        [DisplayName("Imprimantes POS")]
        public string POSPrinter { get; set; }

        [ColumnAttribute(ModelFieldType.Select, "this>Printers")]
        [DisplayName("Imprimantes cuisine")]
        public string POSCuisine { get; set; }

        [BsonIgnore]
        public List<string> Printers { get; set; } = new List<string>();


        #region SAVE

        public override bool Save()
        {
            var result = base.Save();
            DataHelpers.PosSettings = getInstance();
            return result;
        }
        #endregion
    }
}
