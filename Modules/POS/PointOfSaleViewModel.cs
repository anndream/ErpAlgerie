
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.CRM;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ErpAlgerie.Modules.POS
{
    class PointOfSaleViewModel : Screen, IDisposable
    {
        enum OK_ACTIONS
        {
            SAVE,
            QTS,
            PRIX
        }

        public bool CanEditPrix
        {
            get
            {
                return PosSettings.getInstance().PeutModifierPrix;
            }
        }
        public ObservableCollection<UIElement> Products { get; set; }
        public ObservableCollection<Button> Categories { get; set; }
        public ObservableCollection<string> items { get; set; }
        public ObservableCollection<UIElement> CartData { get; set; } = new ObservableCollection<UIElement>();
        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
        public ObservableCollection<CartLine> AllCartLines
        {
            get
            {
                return CurrentTicket?.CarteLines;
            }
        }
        public string SearchMenuText { get; set; } = "";
        public int SesssionIndex { get; set; }
        public ObservableCollection<PosTicket> Tickets { get; set; } = new ObservableCollection<PosTicket>();
        public string Refresh { get; set; } = "Refresh";
        OK_ACTIONS OK_ACTION = OK_ACTIONS.SAVE;
        public string OkStatus
        {
            get
            {
                return OK_ACTION.ToString();
            }
        }

        private object _SelectedCartLine;

        public object SelectedCartLine
        {
            get { return _SelectedCartLine; }
            set
            {
                _SelectedCartLine = value;
                NotifyOfPropertyChange("SelectedCart");
            }
        }

        public void ExpandView()
        {
            var host = this.View.GetParentObject().TryFindParent<Window>();
            if (host != null)
            {
                if (host.WindowStyle == WindowStyle.None)
                {
                    host.WindowStyle = WindowStyle.SingleBorderWindow;
                    host.WindowState = WindowState.Normal;
                }
                else
                {
                    host.WindowStyle = WindowStyle.None;
                    host.WindowState = WindowState.Maximized;
                }
            }
        }


        private PosTicket _CurrentTicket;
        public PosTicket CurrentTicket
        {
            get { return _CurrentTicket; }
            set
            {
                _CurrentTicket = value;
                NotifyOfPropertyChange("CurrentTicket");
            }
        }

        public bool _EstPrepayeOnly { get; set; }
        public PosSettings settings { get; set; } = PosSettings.getInstance();
        public void AddTicket()
        {

            if(CurrentTicket != null && settings.PrintCuisineAlways && CmdStatus == "")
            {
                MessageBox.Show("ENVOYER/IMPRIMER TICKET CUISINE");
                return;
            }



            if (_EstPrepayeOnly)
            {
                CurrentTicket = new PosTicket();
                CurrentTicket.ticketType = TicketType.PREPAYE;
                CurrentTicket.Numero = SesssionIndex++;
                CurrentTicket.Date = DateTime.Now;

                Tickets.Add(CurrentTicket);
                CreateCartLines();
                NotifyOfPropertyChange("Tickets");
                NotifyOfPropertyChange("CurrentTicket");
                NotifyOfPropertyChange("Total");

                MessageQueue.Enqueue("NOUVEAU TICKET");
                return;
            }


           
            var ticketType = new PosSelectViewModel();

            var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(ticketType);
            DataHelpers.windowManager.ShowDialog(ticketType);
           
            if (ticketType.ticketType == TicketType.TABLE)
            {

                if(Tickets.Where(a => a.ticketType == TicketType.TABLE && a.Numero == ticketType.Numero).Count() > 0)
                {

                    MessageBox.Show($"Une table avec le méme numéro <TABLE-{ticketType.Numero}> existe/ouverte!");
                    return;
                }
                else
                {
                    CurrentTicket = new PosTicket();
                    CurrentTicket.ticketType = ticketType.ticketType;                   
                    CurrentTicket.Numero = ticketType.Numero;
                    
                }
              
            }
            else
            {
                CurrentTicket = new PosTicket();
                CurrentTicket.ticketType = ticketType.ticketType;
                CurrentTicket.Numero = SesssionIndex++;
            
            }
            CartData.Add(view);


            CurrentTicket.Date = DateTime.Now;
            Tickets.Add(CurrentTicket);
            CreateCartLines();
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Total");


        }


        public void OpenPaiementDialog()
        {
            try
            {
                var Client = PosSettings.getInstance().DefaultClient.GetObject("Client");
                var paye = new PayeTicketViewModel(CurrentTicket, Client);
                //var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(paye);
                var doDelete = DataHelpers.windowManager.ShowDialog(paye);

                 

                if (doDelete == true)
                {
                    CurrentTicket.Save();
                    var index = Tickets.IndexOf(CurrentTicket); // * * * *
                    Tickets.Remove(CurrentTicket); //  * * |*| *

                    if (Tickets.Count > index) // 3
                    {
                        CurrentTicket = Tickets[index];
                    }
                    else if (Tickets.Count > 0)
                    {
                        CurrentTicket = Tickets[--index];
                    }
                    else
                    {
                        CurrentTicket = null;
                    }





                    // Automatic create new ticket
                    if (!Tickets.Any())
                    {
                        var ticket = new PosTicket()
                        {
                            Date = DateTime.Now,
                            ticketType = TicketType.PREPAYE,
                            Numero = SesssionIndex++
                        };
                        Tickets.Add(ticket);
                        CurrentTicket = ticket;
                    }

                    NotifyOfPropertyChange("Tickets");
                    NotifyOfPropertyChange("CurrentTicket");
                    CreateCartLines();
                    MessageQueue.Enqueue("Vente terminé");
                }
                //else if (paye.MontantRecu < CurrentTicket.Total)
                //{
                //    CurrentTicket.MontantPaye += paye.MontantRecu;
                //    MessageQueue.Enqueue("PAIEMENT PARTIEL ENREGISTRÉ");

                //}

                CreateCartLines();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }


        // Total ttc
        public decimal? Total
        {
            get
            {
                return CurrentTicket?.Total;
            }
        }
        public PointOfSaleViewModel()
        {
            // AddTicket();
            CartData = new ObservableCollection<UIElement>();
            Task.Run(() => Setup());
            NotifyOfPropertyChange("Products");

        }
        public void Dispose()
        {

        }

        protected async override void OnViewLoaded()
        {
            base.OnViewLoaded();
            base.OnInitialActivate();


            // Check Session
            var sessions = DataHelpers.GetMongoData("SessionPos", "DateSession", DateTime.Today, true) as IEnumerable<SessionPos>;

            if (!sessions.Any())
            {
                var newSession = new SessionPos() { DateSession = DateTime.Now };
                await DataHelpers.Shell.OpenScreenDetach(newSession, "Nouvelle session pos");
                CurrentSession = newSession;
            }
            else
            {
                CurrentSession = sessions.FirstOrDefault();
            }
        }

        public async void CloseSession()
        {
            MessageBox.Show("Saisir le montant de cloture");
            await DataHelpers.Shell.OpenScreenDetach(CurrentSession, "Cloturer la session pos");
            this.Close();
        }

        public SessionPos CurrentSession { get; set; }

        public string DispalyNameproperty { get; set; } = "Name";
        public async void FiltreProduit()
        {
            if (string.IsNullOrWhiteSpace(SearchMenuText))
            {
                await Setup();
                return;
            }

            //  var t = new Thread(new ThreadStart( () =>
            //{
            var items = DataHelpers.GetMongoData("Article", DispalyNameproperty, SearchMenuText.ToLower(), false) as IEnumerable<Article>;
            await Setup(items);
            //}));

            //  t.Start();


        }

        public void UserControl_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.F1)
            {
                BTN_SAVE();
            }
            else if (args.Key == Key.F2)
            {
                AddTicket();
            }
            else if(args.Key == Key.F3)
            {
                IsSearchFocus = true;
                NotifyOfPropertyChange("IsSearchFocus");
            }
        }
        public bool IsSearchFocus { get; set; }

        public void ChangeQts()
        {
            int pad = 0;
            int.TryParse(PAD_TEXT, out pad);
            if (CurrentTicket != null && pad > 0)
            {
                OK_ACTION = OK_ACTIONS.QTS;
                BTN_SAVE();
                return;
            }

            MessageQueue.Enqueue("Tapez Qts...");
            PAD_TEXT = "";


            OK_ACTION = OK_ACTIONS.QTS;
            NotifyOfPropertyChange("PAD_TEXT");
            NotifyOfPropertyChange("OkStatus");
        }

        public void ChangePrix()
        {

            int pad = 0;
            int.TryParse(PAD_TEXT, out pad);
            if (CurrentTicket != null && pad > 0)
            {
                OK_ACTION = OK_ACTIONS.PRIX;
                BTN_SAVE();
                return;
            }


            MessageQueue.Enqueue("Tapez Prix de vente...");
            PAD_TEXT = "";

            OK_ACTION = OK_ACTIONS.PRIX;
            NotifyOfPropertyChange("PAD_TEXT");
            NotifyOfPropertyChange("OkStatus");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public string PAD_TEXT { get; set; }

        public void PAD_CLICK(string value)
        {
            PAD_TEXT += value;
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
                // padd mepty
                // reset action to save
                OK_ACTION = OK_ACTIONS.SAVE;

                NotifyOfPropertyChange("PAD_TEXT");
                NotifyOfPropertyChange("OkStatus");
            }
            NotifyOfPropertyChange("PAD_TEXT");
        }

        public CartLine SelectedCart
        {
            get
            {
                if (SelectedCartLine != null)
                    return ((SelectedCartLine as ListViewItem)?.DataContext as CartLineViewModel)?.cartLine;
                return null;
            }
        }

        //  Button save OK
        public void BTN_SAVE()
        {
            decimal pad = 0;
            decimal.TryParse(PAD_TEXT, out pad);
            switch (OK_ACTION)
            {
                case OK_ACTIONS.SAVE:
                    OpenPaiementDialog();
                    MessageQueue.Enqueue("TERMINE");
                    break;
                case OK_ACTIONS.QTS:
                    SetQteOrItem(pad, SelectedCart);
                    MessageQueue.Enqueue("OK");
                    OK_ACTION = OK_ACTIONS.SAVE;
                    break;
                case OK_ACTIONS.PRIX:
                    SetPrice(pad, SelectedCart);
                    MessageQueue.Enqueue("OK");
                    OK_ACTION = OK_ACTIONS.SAVE;
                    break;
                default:
                    break;
            }
            NotifyOfPropertyChange("OkStatus");
            PAD_TEXT = "";
            NotifyOfPropertyChange("PAD_TEXT");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public async void RefreshAll()
        {
            await Setup();
            Refresh = "Refresh";
            NotifyOfPropertyChange("Refresh");
            MessageQueue.Enqueue("Actaliser");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public async Task Setup(IEnumerable<Article> _items = null)
        {
            // AllCartLines = new HashSet<CartLine>();
            var settings = PosSettings.getInstance();
            _EstPrepayeOnly = settings.EstPrepayeOnly;
            DispalyNameproperty = string.IsNullOrWhiteSpace(settings.NameProperty) ? "Name" : settings.NameProperty;

            Products = new ObservableCollection<UIElement>();
            Categories = new ObservableCollection<Button>();
            if (_items == null)
                _items = await DataHelpers.instanc().GetMongoDataPaged<Article>(1,50) as IEnumerable<Article>;

            var cats = await DataHelpers.instanc().GetData<GroupeArticle>(a => true) as IEnumerable<GroupeArticle>;
            foreach (var item in cats)
            {
                var btnCat = new Button();

                btnCat.Content = item.Name;
                btnCat.MinWidth = 100;
                btnCat.Height = 50;
                btnCat.Tag = item;
                btnCat.Click += BtnCat_Click1;
                btnCat.HorizontalAlignment = HorizontalAlignment.Left;
                btnCat.HorizontalContentAlignment = HorizontalAlignment.Left;
                btnCat.Margin = new Thickness(0, 5, 0, 0);
                Categories.Add(btnCat);
            }
            foreach (var item in _items)
            {
                //Products = new ObservableCollection<UIElement>();
                var productBox = new ProductBoxViewModel(item);
                productBox.clickEvent += ProductBox_clickEvent;
                var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(productBox);
                Products.Add(view);
            }

            NotifyOfPropertyChange("Categories");
            NotifyOfPropertyChange("Products");
            NotifyOfPropertyChange("CartData");
            NotifyOfPropertyChange("Total");
        }

        private async void BtnCat_Click1(object sender, RoutedEventArgs e)
        {

            var category = (sender as Button).Tag as GroupeArticle;

            if (category != null)
            {
                var items = DataHelpers.GetMongoData("Article", "lGroupeArticle", category.Id, false) as IEnumerable<Article>;
                await Setup(items);
            }
        }

        private void ProductBox_clickEvent(object sender, EventArgs e)
        {
            var article = (sender as ProductBoxViewModel).product;

            if (article.Variantes.Any())
            {
                // has variante
                Refresh  = "KeyboardBackspace";
                NotifyOfPropertyChange("Refresh");

                var vars = article.Variantes;
                Products.Clear();
                foreach (var item in vars)
                {
                    var productBox = new ProductBoxViewModel(item);
                    productBox.product = article;
                    
                    productBox.clickEvent += ProductBox_clickEvent1;
                    var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(productBox);
                    Products.Add(view);
                }
                NotifyOfPropertyChange("Products");

                NotifyOfPropertyChange("CmdStatus");
            }
            else
            {
                var cartLine = new CartLine(article);
                AddQteOrItem(1, cartLine);
            }


           
        }

        private void ProductBox_clickEvent1(object sender, EventArgs e)
        {
            var article = (sender as ProductBoxViewModel).product;
            var variante = (sender as ProductBoxViewModel).variante;
            var cartLine = new CartLine(article,variante);
            AddQteOrItem(1, cartLine);
           // MessageBox.Show("Variante");
            return;
        }

        private void SetQteOrItem(decimal qte, CartLine cart)
        {
            try
            {

                var exited = AllCartLines?.Where(a => a == cart).FirstOrDefault();
                if (exited != null)
                {// Just add qts
                    exited.Qts = qte;
                }
                else
                {
                    AllCartLines?.Add(cart);
                }

                CreateCartLines();
            }
            catch (Exception s)
            {
                MessageQueue.Enqueue(s.Message);
            }
        }

        private void SetPrice(decimal prix, CartLine cart)
        {
            try
            {

                var exited = AllCartLines?.Where(a => a == cart).FirstOrDefault();
                if (exited != null)
                {// Just add qts
                    exited.PricUnitaire = prix;
                }
                CreateCartLines();
            }
            catch (Exception s)
            {
                MessageQueue.Enqueue(s.Message);
            }
        }

        private void AddQteOrItem(decimal qte, CartLine cart)
        {
            var exited = AllCartLines?.Where(a => a.article.Name == cart.article.Name 
            && (a.variante == cart.variante || a.variante?.Name == cart.variante.Name)
             ).FirstOrDefault();
            if (exited != null)
            {// Just add qts
                exited.Qts++;
            }
            else
            {
                AllCartLines?.Add(cart);
            }


            CurrentTicket.EstEnvoyerCuisine = false;
            
            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");

            CreateCartLines();
        }

        public void CreateCartLines()
        {
            CartData.Clear();
            if (AllCartLines == null)
            {
                NotifyOfPropertyChange("CartData");
                NotifyOfPropertyChange("Total");

                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");
                return;
            }

            foreach (var cartLine in AllCartLines)
            {
                var cartLineView = new CartLineViewModel(cartLine);
                cartLineView.clickEvent += CartLineView_clickEvent;
                cartLineView.DoubleClick += CartLineView_DoubleClick;
                var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(cartLineView);
                CartData.Add(view);
            }

            NotifyOfPropertyChange("CartData");
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("Total");
            NotifyOfPropertyChange("CurrentTicket");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        private void CartLineView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var line = (sender as CartLine);

                var host = new Window();
                host.Width = 500;
                host.Height = 450;
                host.Background = Brushes.LightGray;

                var sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                sp.VerticalAlignment = VerticalAlignment.Stretch;
                sp.HorizontalAlignment = HorizontalAlignment.Stretch;
                sp.Margin = new Thickness(15);

                var text = new TextBox();
                text.VerticalAlignment = VerticalAlignment.Stretch;
                text.HorizontalAlignment = HorizontalAlignment.Stretch;
                text.Background = Brushes.White;
                text.FontSize = 35;
                text.Margin = new Thickness(10);
                var btn = new Button();
                btn.Content = "OK";
                btn.VerticalAlignment = VerticalAlignment.Bottom;

                var msgs = DS.db.GetAll<MessageCommande>(a => true) as IEnumerable<MessageCommande>;
                var list = new ListView();
                list.ItemsSource = msgs.Select(z => z.Message);
                list.MaxHeight = 200;
                list.Margin = new Thickness(10);
                list.Background = Brushes.White;
                list.SelectionChanged += delegate
                {
                    text.Text += list.SelectedItem?.ToString()+" / ";
                };
                btn.Click += delegate
                {
                    var msg = text.Text; 
                     host.Tag = msg; 
                    
                   
                    host.Hide();
                };


                

                sp.Children.Add(text);
                sp.Children.Add(list);
                sp.Children.Add(btn);

                host.Content = sp;
                host.ShowDialog();

                var value = host.Tag?.ToString();
              
                line.Message = value;
                host.Close();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
            CreateCartLines();

        }

        private void CartLineView_clickEvent(object sender, EventArgs e)
        {
            var cartline = sender as CartLine;
            if (cartline != null)
            {
                AllCartLines?.Remove(cartline);
                CreateCartLines();
            }
        }

        private void BtnCat_Click(object sender, RoutedEventArgs e)
        {

        }

        public void TransfertTicket()
        {
            if (CurrentTicket == null)
                return;
            var ticketType = new PosSelectViewModel(CurrentTicket);

            var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(ticketType);
            DataHelpers.windowManager.ShowDialog(ticketType);
            CurrentTicket = ticketType.currentTicket;

            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CurrentTicket");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public void Close()
        {
            try
            {
                this.RequestClose(true);

            }
            catch (Exception s)
            {
                this.View.GetParentObject().TryFindParent<Window>().Close();
                DataHelpers.Logger.LogError(s);
            }
        }

        public void BackTicket()
        {
            var index = Tickets.IndexOf(CurrentTicket) - 1;
            if (index >= 0)
            {
                CurrentTicket = Tickets[index];
            }
            else
            {
                index = 0;
            }
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CmdColor");
            NotifyOfPropertyChange("CmdStatus");
        }

        public void NextTicket()
        {
            var index = Tickets.IndexOf(CurrentTicket) + 1;
            if (index < Tickets.Count)
            {
                CurrentTicket = Tickets[index];
            }
            else
            {
                index--;
            }
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Tickets");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public void DeleteTicket()
        {
            if (CurrentTicket != null)
            {
                Tickets.Remove(CurrentTicket);
                NotifyOfPropertyChange("Tickets");

                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");
                CreateCartLines();

                MessageQueue.Enqueue("Ticket supprimer");
                NextTicket();
            }
        }


        public string CmdStatus
        {
            get
            {

                if (CurrentTicket?.EstEnvoyerCuisine == true && CurrentTicket?.EstDeLivrer == true)
                    return "OK";
                if (CurrentTicket?.EstEnvoyerCuisine == true && CurrentTicket?.EstDeLivrer == false)
                    return "PREP.";
                if (CurrentTicket?.EstEnvoyerCuisine == false && CurrentTicket?.EstDeLivrer == true)
                    return "MOD.";
                return "";

            }
        }


        public string CmdColor
        {
            get
            { 
                return CurrentTicket?.GetStatusColor();
            }
        }

        public void EnvoyerCuisine()
        {
            if (CurrentTicket != null && CurrentTicket.CarteLines.Any())
            {
                CurrentTicket.EnvoyerCuisine();
                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");
            }
        }

        public void LivrerTicket()
        {
            if (CurrentTicket != null && CurrentTicket.CarteLines.Any())
            {
                CurrentTicket.EstDeLivrer = !CurrentTicket.EstDeLivrer;
                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");

                if (CurrentTicket.EstDeLivrer)
                    MessageQueue.Enqueue($"TICKET LIVRÉ");


                if (!CurrentTicket.EstDeLivrer)
                    MessageQueue.Enqueue($"TICKET NON LIVRÉ");
            }
        }

        public void ShowTickets()
        {
            var tickets = this.Tickets;
            if(tickets == null || !tickets.Any())
            {
                MessageBox.Show("Aucun ticket!");
                return;
            }


            Products = new ObservableCollection<UIElement>();

            foreach (var item in tickets)
            {
                
                var ticket = new Button();
                ticket.MinHeight = 80;
                ticket.Height = 80;
                ticket.MinWidth = 80;
                ticket.Width = 100;
                ticket.Background = (SolidColorBrush)new BrushConverter().ConvertFromString( item.GetStatusColor());
                ticket.HorizontalAlignment = HorizontalAlignment.Center;
                ticket.HorizontalContentAlignment = HorizontalAlignment.Center;
                ticket.Tag = item;
                ticket.Margin = new Thickness(10);
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                sp.HorizontalAlignment = HorizontalAlignment.Center;

                PackIcon icon = new PackIcon();
                if(item.ticketType == TicketType.PREPAYE)
                    icon.Kind = PackIconKind.TagOutline;

                if(item.ticketType == TicketType.TABLE)
                    icon.Kind = PackIconKind.Food;

                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.Height = 40;
                icon.Width = 40;
                icon.VerticalAlignment = VerticalAlignment.Center;
                icon.VerticalContentAlignment = VerticalAlignment.Center;
                TextBlock text = new TextBlock();
                text.Text = item.Name;
                text.Margin = new Thickness(0, 10, 0, 0);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                sp.Children.Add(icon);
                sp.Children.Add(text);
                ticket.Content = sp;

                ticket.Click += Ticket_Click;
                
                Products.Add(ticket);
            }

            NotifyOfPropertyChange("Products");
            NotifyOfPropertyChange("Tickets");

        }

        private void Ticket_Click(object sender, RoutedEventArgs e)
        {

            var ticket = (sender as Button).Tag as PosTicket;
            CurrentTicket = ticket;

            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CurrentTicket");

            RefreshAll();
        }

        public void OpenDrawer()
        {
            var local = PosSettings.getInstance().POSPrinter;
            const string ESC = "\u001B";
            const string p = "\u0070";
            const string m = "\u0000";
            const string t1 = "\u0025";
            const string t2 = "\u0250";
            const string openTillCommand = ESC + p + m + t1 + t2;
            RawPrinterHelper.SendStringToPrinter(local, openTillCommand);
            
        }

      
    }
}
