﻿
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.CRM;
using MongoDB.Bson;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ErpAlgerie.Modules.POS
{
    class PayeTicketViewModel : Screen, IDisposable
    {

        public PosTicket ticket { get; set; }



        public PayeTicketViewModel(PosTicket ticket, Client client)
        {
            settings = PosSettings.getInstance();
            if (ticket == null)
            { 
                throw new Exception("Selectionner un TICKET a payé");
            }
          

            this.ticket = ticket; 

            if (this.ticket.MontantPaye > 0)
            {
                MontantRecu = this.ticket.Reste ;
            }
            else
            {
                MontantRecu = this.ticket.Total;
            }
           
            LoadData();

            IsAnonym = true;
            var settingsUser = PosSettings.getInstance();
            if (client != null)
            { 
                SelectedClient = client;
            }
            
            PrintPdf = settingsUser.EstImprimer;
            PrintKitchen = settingsUser.EstImprimerCuisine;
            CreateFacture = settingsUser.EstFacturer;


            NotifyOfPropertyChange("IsAnonym");
            NotifyOfPropertyChange("SelectedClient");
            NotifyOfPropertyChange("MontantTotal");

        }

        public PosSettings settings { get; set; }

        public void LoadData()
        {
            var Clients_ = DataHelpers.GetMongoDataSync("Client") as IEnumerable<Client>;
            Clients = new List<Client>(Clients_);
            NotifyOfPropertyChange("Clients");
            NotifyOfPropertyChange("SelectedClient");

        }
        public async void TransfertTicket()
        {
            ticket.Client = this.SelectedClient;
            ticket.MontantPaye += MontantRecu;

            if (ticket.Reste <= 0)
            {
                ticket.EstPaye = true;
            }


            Facture fac = new Facture();
            fac.EstDelivrer = true;
            fac.Series = settings.SeriesFacture;
            fac.EstPaye = (MontantRetour >= 0);
            fac.Client = SelectedClient?.Id;
            fac.NomClient = SelectedClient?.NomComplet;
            fac.DateCreation = DateTime.Now;
            fac.DateEcheance = DateTime.Now;
            fac.Remarques = $"{ticket.ticketType.ToString()} {ticket.Numero}";
            fac.VenteComptoir = true;
            fac.UpdateStock = true;   
           var repas = ticket.CarteLines ;



           


            foreach (var item in repas)
            {

                LigneFacture line = item.article.Map("LigneFacture") as LigneFacture;
                line.PrixUnitaire = item.PricUnitaire;
                line.Qts = item.Qts; 
                fac.ArticleFacture.Add(line);
            }
            if(DeleteTicket && ticket.Reste > 0)
            {
                var response = MessageBox.Show("etes vous sur de vouloir supprimer le ticket partiellement payée?", "SUPPRIMER TICKET?", MessageBoxButton.YesNo);
                if (response == MessageBoxResult.No)
                    return;
            }
           if(CreateFacture && ticket.Reste > 0)
            {
                
                MessageBox.Show("Vous pouvez pas facturer une commande partiellement payée. Décocher <FACTURER>");
                return;
            }
            if (CreateFacture)
            {
                try
                {
                    //214 214

                  
                    fac.RemiseGlobal = ticket.Remise;
                    ticket.RefFacture = fac.Id;
                    if (settings.SeriesFacture != null && settings.SeriesFacture != ObjectId.Empty)
                        fac.Series = settings.SeriesFacture;
                    
                    fac.Save();
                    fac.Submit();

                    var paiement = fac.SaisirPaiement(false);
                    paiement.Save();
                    paiement.Submit();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
            }
            if (PrintKitchen)
            {
               
            }
            if (PrintPdf)
            {

                var local = PosSettings.getInstance().POSPrinter;
                //PrintDocument print = new PrintDocument();
                //var docz= fac.ExportPDF(fac.GetType(), settings.NomTemplate, !settings.DontUseHeader);
                //print.DocumentName = docz;
                //print.PrinterSettings.PrinterName = local;
                //print.DefaultPageSettings.PaperSize.Width = 297
                //print.Print();

                if (string.IsNullOrWhiteSpace(settings.NomTemplate))
                {
                    MessageBox.Show("Définis le modéle d'impression dans paramétres");
                    return;
                }
                var doc = fac.ExportWORD(fac.GetType(), settings.NomTemplate, !settings.DontUseHeader);
                try
                {
                    const string ESC = "\u001B";
                    const string p = "\u0070";
                    const string m = "\u0000";
                    const string t1 = "\u0025";
                    const string t2 = "\u0250";
                    const string openTillCommand = ESC + p + m + t1 + t2;
                    RawPrinterHelper.SendStringToPrinter(local, openTillCommand);

                    ProcessStartInfo info = new ProcessStartInfo(doc);
                    info.Arguments = "\"" + local + "\"";
                    info.Verb = "Printto";
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(info);

                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
                //if (!string.IsNullOrWhiteSpace(settings.NomTemplate))
                //{
                //    var doc =  fac.ExportWORD(fac.GetType(), settings.NomTemplate,!settings.DontUseHeader);
                //    ProcessStartInfo info = new ProcessStartInfo(doc);
                //    info.Verb = "Print";
                //    info.CreateNoWindow = true;
                //    info.WindowStyle = ProcessWindowStyle.Hidden;
                //    Process.Start(info);

                //}
                //else
                //{
                //  fac.ExportPDF(fac.GetType(), true, !settings.DontUseHeader);
                //}
            }
            if(OpenPdf)
               await DataHelpers.Shell.OpenScreenDetach(fac, fac.Name);

            if (DeleteTicket)
            {
                this.RequestClose(true);
            }
            else
            {
                this.RequestClose(false);
            }


        }
        public void Dispose()
        {
            this.RequestClose(false);
        }

       
        private decimal _MontantTotal;
        public bool DeleteTicket { get; set; } = true;

       

        public bool CreateFacture { get; set; } = false;
        public bool PrintPdf { get; set; } = false;

        public bool PrintKitchen { get; set; } = false;
        public bool OpenPdf { get; set; } = false;
        public decimal MontantRecu { get; set; }

        public decimal MontantRetour { get
            {
                return MontantRecu - ticket.Reste;
            }
        }
        

        public  void SetTotalAuto()
        { 
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantTotal");
            NotifyOfPropertyChange("MontantRetour");
        }

        public Client SelectedClient { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();

        public bool ClientEnabled { get; set; } = false;
        private bool _IsAnonym;
        public bool IsAnonym
        {
            get { return _IsAnonym; }
            set
            {
                _IsAnonym = value;
                ClientEnabled = !value;
                NotifyOfPropertyChange("IsAnonym");
                NotifyOfPropertyChange("ClientEnabled");
            }
        }
        public string PAD_TEXT { get; set; }
        public void PAD_CLICK(string value)
        {
            PAD_TEXT += value;
            NotifyOfPropertyChange("PAD_TEXT");

            updateValues();

            NotifyOfPropertyChange("SelectedClient");



        }

        public void updateValues()
        {
            if (TAP_EDIT_DEST == "MONTANT_TOTAL")
            {
                //decimal montant = 0;
                //decimal.TryParse(PAD_TEXT, out montant);
                //MontantTotal = montant;
                NotifyOfPropertyChange("MontantTotal");
            }

            if (TAP_EDIT_DEST == "MONTANT_RECU")
            {
                decimal montant = 0;
                decimal.TryParse(PAD_TEXT, out montant);
                MontantRecu = montant;
                NotifyOfPropertyChange("MontantRecu");
                NotifyOfPropertyChange("MontantReste");
                NotifyOfPropertyChange("ticket");


            }

            if (TAP_EDIT_DEST == "MONTANT_REMISE")
            {
                decimal montant = 0;
                decimal.TryParse(PAD_TEXT, out montant);
                this.ticket.Remise = montant;
                NotifyOfPropertyChange("MontantRemise");

                NotifyOfPropertyChange("ticket");
            }
            NotifyOfPropertyChange("MontantTotalaPaye");
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantReste");
        }

        public string TAP_EDIT_DEST { get; set; }
        public void SelectAll(object s, EventArgs e)
        {
            Keyboard.ClearFocus();
            PAD_TEXT = "";
            TAP_EDIT_DEST = "MONTANT_TOTAL";
            //NotifyOfPropertyChange("PAD_TEXT");
        }
        public void SelectAllMontantRecu(object s, EventArgs e)
        {
            Keyboard.ClearFocus();
            PAD_TEXT = "";
            TAP_EDIT_DEST = "MONTANT_RECU";
            NotifyOfPropertyChange("PAD_TEXT");
        }
        public void SelectAllMontantRemise(object s, EventArgs e)
        {
            Keyboard.ClearFocus();
            PAD_TEXT = "";
            TAP_EDIT_DEST = "MONTANT_REMISE";
            NotifyOfPropertyChange("PAD_TEXT");
        }
        

        public void PAD_DELETE()
        {
            if (PAD_TEXT?.Length > 0)
            {
                PAD_TEXT = PAD_TEXT.Remove(PAD_TEXT.Length - 1);                
            }
            else
            { 
                NotifyOfPropertyChange("OkStatus");
            }
            Billet = 0;

            NotifyOfPropertyChange("Billet");
            updateValues();
            NotifyOfPropertyChange("PAD_TEXT");
        }

        public decimal Billet { get; set; } = 0;
        public void da100()
        {
            MontantRecu = Billet += 100;
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantTotalaPaye");
            NotifyOfPropertyChange("MontantReste");

        }
        public void da200()
        {
            MontantRecu = Billet += 200;
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantReste");
            NotifyOfPropertyChange("MontantTotalaPaye");

        }
        public void da500()
        {
            MontantRecu = Billet += 500;
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantTotalaPaye");
        }
        public void da1000()
        {
            MontantRecu = Billet += 1000;
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantReste");
            NotifyOfPropertyChange("MontantTotalaPaye");

        }
        public void da2000()
        {
            MontantRecu = Billet += 2000;
            NotifyOfPropertyChange("MontantRetour");
            NotifyOfPropertyChange("MontantRecu");
            NotifyOfPropertyChange("MontantTotalaPaye");
            NotifyOfPropertyChange("MontantReste");
        }

        public void UserControl_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.F1)
            {
                TransfertTicket();
            }

            if (args.Key == Key.F3)
            {
                PrintPdf = !PrintPdf;
                NotifyOfPropertyChange("PrintPdf");
            }

            if (args.Key == Key.F2)
            {
                DeleteTicket = !DeleteTicket;
                NotifyOfPropertyChange("DeleteTicket");
            }
        }


    }
}
