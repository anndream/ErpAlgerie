using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Modules.POS;
using MongoDB.Bson;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{
    public enum TicketType
    {
        PREPAYE,
        TABLE
    }
    class PosTicket: ModelBase<PosTicket>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "VENTE";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string CollectionName { get; } = "Ticket de vente";
        public override string IconName { get; set; } = "TagOutline";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "NameTicket";

        #endregion

        public PosTicket()
        {

        }

        public PosTicket(Client client, DateTime date, int numero, ObservableCollection<CartLine> carteLines)
        {
            Client = client;
            Date = date;
            Numero = numero;
            CarteLines = carteLines;
          }


        
        public Client Client { get; set; }
        [ShowInTable]
        [Column(ModelFieldType.ReadOnly,"")]
        [DisplayName("Client")]
        public string ClientName { get
            {
                return Client?.Name;
            }
        }

        [ShowInTable]
        [Column(ModelFieldType.Date,"")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [ShowInTable]
        [Column(ModelFieldType.Devise, "")]
        [DisplayName("Montant payé")]
        public decimal MontantPaye { get; set; }

        [ShowInTable]
        [Column(ModelFieldType.Devise, "")]
        [DisplayName("Remise")]
        public decimal Remise { get; set; }

        [Column(ModelFieldType.ReadOnly, "{0:C}")]
        [DisplayName("Rest à payé")]
        public decimal Reste     { get
            {
                if (MontantPaye > Total)
                    MontantPaye = Total;
                return Total - MontantPaye;
            }
        }


        [ShowInTable]
        [Column(ModelFieldType.ReadOnly, "{0:C}")]
        [DisplayName("Total")]
        public decimal Total
        {
            get
            {
                return CarteLines.Sum(a => a.Total) - Remise;
            }
        }


        private int _Numero;

        [Column(ModelFieldType.Numero, "")]
        [DisplayName("Indexe")]
        public int Numero
        {
            get { return _Numero; }
            set { _Numero = value;
                NotifyOfPropertyChange("Name");
            }
        }


         public ObservableCollection<CartLine> CarteLines { get; set; } = new ObservableCollection<CartLine>();
 


        public TicketType ticketType { get; set; }

        
        

        [Column(ModelFieldType.ReadOnly, "")]
        [DisplayName("Numéro")]
        public string NameTicket { get
            {
                return $"{getTicketType()} {Numero}";
            }
        }

        private string getTicketType()
        {
            switch (ticketType)
            {
                case TicketType.PREPAYE:
                    return "Prépaye";

                case TicketType.TABLE:
                    return "Table";
                default:
                    return "";
                    break;
            }
        }


        // PRINT CUISINE

        [Column(ModelFieldType.Check, "Envoyé au cuisine")]
        [DisplayName("")]
        public bool EstEnvoyerCuisine { get; set; } = false;


        [Column(ModelFieldType.Check, "Est délivré")]
        [DisplayName("")]
        public bool EstDeLivrer { get; set; }


        [Column(ModelFieldType.Check, "Est payé")]
        [DisplayName("")]
        public bool EstPaye { get; set; }

        public bool EnvoyerCuisineDeja { get; set; }

        public void EnvoyerCuisine()
        {
            var settings = PosSettings.getInstance();
            var imprimanteCuisine = settings.POSCuisine;

            Facture fac = new Facture();
            fac.EstDelivrer = true;

            fac.Client =this.Client?.Id;
            fac.NomClient =  this.Client?.NomComplet;
            fac.DateCreation = DateTime.Now;
            var edited = EnvoyerCuisineDeja ? "**MODIFIÉ**" : "";
            fac.Remarques = $"{this.ticketType.ToString()} {this.Numero} {edited}";
            var repas = this.CarteLines;
            foreach (var item in repas)
            {

                LigneFacture line = item.article.Map("LigneFacture") as LigneFacture;
                line.PrixUnitaire = item.PricUnitaire;
                line.Qts = item.Qts;
                line.Details = item.variante?.Name + " "+item.Message;
                fac.ArticleFacture.Add(line);
            }

            if (string.IsNullOrWhiteSpace(settings.NomTemplateCuisine))
            {
                MessageBox.Show("Définis le modéle d'impression dans paramétres");
                return;
            }
            var doc = fac.ExportWORD(fac.GetType(), settings.NomTemplateCuisine, !settings.DontUseHeader);
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(doc);

                foreach (String verb in info.Verbs)
                {
                    System.Diagnostics.Debug.WriteLine(verb);
                }



                info.Arguments = "\"" + imprimanteCuisine + "\"";
                info.Verb = "Printto";
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);

                EstEnvoyerCuisine = true;
                EnvoyerCuisineDeja = true;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }

        public string GetStatusColor()
        {
            if (this.EstDeLivrer && this.EstEnvoyerCuisine)
                return "Red";

            if (this.EstEnvoyerCuisine)
                return "Orange";

            if (this.EstPaye)
                return "Green";

            return "Blue";
        }

        [DisplayName("Facture de vente")]
        [Column(ModelFieldType.Button,"OpenFacture")]
        public string OpenFactureBtn { get; set; }

        public void OpenFacture()
        {
            var facture = DataHelpers.GetById("Facture", this.RefFacture) as Facture;
            if(facture != null)
            {
                DataHelpers.Shell.OpenScreenAttach(facture, facture.Name);
            }
        }
        #region REFERENCES

        public ObjectId? RefFacture { get; set; } = ObjectId.Empty;

        #endregion
    }
}
