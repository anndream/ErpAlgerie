using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Modules.POS;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
        public override string ModuleName { get; set; } = "PDV";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string CollectionName { get; } = "Ticket de vente";
        public override string IconName { get; set; } = "TagOutline";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "NameTicket";

        #endregion

        public PosTicket()
        {

        }

        public PosTicket(Client client, DateTime date, int numero, List<CartLine> carteLines)
        {
            Client = client;
            Date = date;
            Numero = numero;
            CarteLines = carteLines;
          }

        [ShowInTable]
        [Column(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Total à payé")]
        public decimal Total
        {
            get
            {
                return CarteLines.Where(z => z != null).Sum(a => a.Total) - Remise;
            }
        }

        [Column(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Total")]
        public decimal TotalNet
        {
            get
            {
                return CarteLines.Where(z => z != null).Sum(a => a.Total);
            }
        }

        public int Position { get; set; }

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

        [Column(ModelFieldType.ReadOnly, "{0} DA")]
        [DisplayName("Reste à payé")]
        public decimal Reste     { get
            {
                if (MontantPaye > Total)
                    return 0;
                return Total - MontantPaye;
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


        [ColumnAttribute(ModelFieldType.Table, "CartLine")]
        [ShowInTable(false)]
        [DisplayName("Variante")]
        [myTypeAttribute(typeof(Article))]
        public List<CartLine> CarteLines { get; set; } = new List<CartLine>();

        [DisplayName("Résultats")]
        [BsonIgnore]
        [Column(ModelFieldType.Separation,"")]
        public string sepTable { get; set; }

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

        [Column(ModelFieldType.Check, "")]
        [DisplayName("Envoyé au cuisine")]
        public bool EstEnvoyerCuisine { get; set; } = false;


        [Column(ModelFieldType.Check, "")]
        [DisplayName("Est délivré")]
        public bool EstDeLivrer { get; set; }


        [DisplayName("A partir du terminal mobile")]
        [Column(ModelFieldType.Check, "Mobile")]
        public bool FromTablet { get; set; }


        [Column(ModelFieldType.Check, "")]
        [DisplayName("Est payé")]
        public bool EstPaye { get; set; }


        [Column(ModelFieldType.Lien,"ListePrix")]
        [DisplayName("Liste des prix")]
        public ObjectId? ListePrix { get; set; } = ObjectId.Empty;

        public bool EnvoyerCuisineDeja { get; set; }

        public void EnvoyerCuisine()
        {
            var settings = PosSettings.getInstance();
            var imprimanteCuisine = settings.POSCuisine;

            var positions = CarteLines.GroupBy(a => a.aCuisinePosition);
            var edited = EnvoyerCuisineDeja ? "**MODIFIÉ**" : "";
            foreach (var position in positions)
            {
                Facture fac = new Facture();
                fac.EstDelivrer = true;
                fac.Position = this.Position;
                fac.Client = this.Client?.Id;
                fac.NomClient = this.Client?.NomComplet;
                fac.DateCreation = DateTime.Now;
               
                fac.Remarques = $"{this.ticketType.ToString()} {this.Numero} {position.Key?.GetObject("CuisinePosition")?.Name} {edited} ";
                var repas = this.CarteLines.Where(a => a.aCuisinePosition == position.Key);
                foreach (var item in repas)
                {
                    var deffirence = item.Qts - item.OldQts;
                    if (deffirence > 0 && item.OldQts > 0)
                    {
                        item.Message += $" QTS +{deffirence}";
                    }
                    if (deffirence < 0 && item.OldQts > 0)
                    {
                        item.Message += $" QTS {deffirence}";
                    }

                    LigneFacture line = item.article.Map("LigneFacture") as LigneFacture;
                    line.PrixUnitaire = item.PricUnitaire;
                    line.Qts = item.Qts;
                    line.Details = item.variante?.Name + " " + item.Message;
                    fac.ArticleFacture.Add(line);

                    if (item.Message?.Contains('L') == false)                        
                        item.Message += new string('L', 1);

                    item.OldQts = item.Qts;
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


        #region OVERRIDE

        public override bool Cancel()
        {
            var factures = DS.db.GetAll<Facture>(a => a.Id == this.RefFacture) as IEnumerable<Facture>;

            var result = base.Cancel();
            if (!result)
                return false;


            if (factures != null && result == true)
            {
                foreach (var item in factures)
                {
                    if (!item.Cancel())
                        continue;
                }
            }

            return result;

        }


        public override bool Delete(bool ConfirmFromUser = true)
        {

            var factures = DS.db.GetAll<Facture>(a => a.Id == this.RefFacture) as IEnumerable<Facture>;

            var result = base.Delete(ConfirmFromUser);

            if (factures != null && result == true)
            {
                foreach (var item in factures)
                {
                    if (!item.Delete(false))
                        continue;
                }
            }

            return result;
        }



        #endregion


        #region REFERENCES

        public ObjectId? RefFacture { get; set; } = ObjectId.Empty;
        public bool DoPrintFromTablet { get; set;}

        #endregion
    }
}
