
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.CRM;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using MongoDB.Bson;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public BindableCollection<UIElement> Products { get; set; }
        public BindableCollection<Button> Categories { get; set; }
        public BindableCollection<string> items { get; set; }
        public BindableCollection<UIElement> CartData { get; set; } = new BindableCollection<UIElement>();
        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
        //public BindableCollection<CartLine> AllCartLines
        //{
        //    get
        //    {
        //        if(CurrentTicket != null)
        //        return new BindableCollection<CartLine>(CurrentTicket?.CarteLines);
        //        return new BindableCollection<CartLine>();
        //    }
        //}
        public string SearchMenuText { get; set; } = "";
        public int SesssionIndex { get; set; }
        public BindableCollection<PosTicket> Tickets { get; set; } = new BindableCollection<PosTicket>();
        public string Refresh { get; set; } = "Refresh";
        OK_ACTIONS OK_ACTION = OK_ACTIONS.SAVE;
        public int Position { get; set; }
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
        private Timer timer;

        public PosTicket CurrentTicket
        {
            get { return _CurrentTicket; }
            set
            {
                _CurrentTicket = value; 
            }
        }

        public bool _EstPrepayeOnly { get; set; }
        //public PosSettings settings { get; set; } = PosSettings.getInstance();
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
                CurrentTicket.Position = this.Position;
                this.Position++;
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
            bool IsJumler = false;
            if (ticketType.ticketType == TicketType.TABLE)
            {

                if(Tickets.Where(a => a.ticketType == TicketType.TABLE && a.Numero == ticketType.Numero).Count() > 0)
                {
                    MessageBox.Show($"Mode jumeler\nUne table avec le méme numéro <TABLE-{ticketType.Numero}> existe/ouverte! ");


                    var existedTicket = Tickets.First(a => a.ticketType == TicketType.TABLE && a.Numero == ticketType.Numero);
                    CurrentTicket = existedTicket;
                    //CurrentTicket?.Refresh();
                    //Tickets?.Refresh();
                    NotifyOfPropertyChange("CurrentTicket");
                    NotifyOfPropertyChange("Tickets");

                    IsJumler = true;
                }
                else
                {
                    CurrentTicket = new PosTicket();
                    CurrentTicket.Position = this.Position;
                    this.Position++;

                    CurrentTicket.ticketType = ticketType.ticketType;                   
                    CurrentTicket.Numero = ticketType.Numero;
                    
                }
              
            }
            else
            {
                CurrentTicket = new PosTicket();
                CurrentTicket.Position = this.Position;
                this.Position++;
                CurrentTicket.ticketType = ticketType.ticketType;
                CurrentTicket.Numero = SesssionIndex++;
            
            }
            CartData.Add(view);
            //CartData.Refresh();
            NotifyOfPropertyChange("CartData");

            if (settings.ListPrixParDefault != ObjectId.Empty && settings.ListPrixParDefault != null)
            {
                CurrentTicket.ListePrix = settings.ListPrixParDefault;
            }

            CurrentTicket.Date = DateTime.Now;
            if(!IsJumler)
                Tickets.Add(CurrentTicket);
             
            CreateCartLines();
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Total");
            if (ShowTicketsVisible)
                ShowTickets();

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
                    CurrentTicket.isHandled = 1;
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
                    //CurrentTicket?.Refresh();
                    //Tickets?.Refresh();
                    CreateCartLines();
                    MessageQueue.Enqueue("Vente terminé");
                }
                //else if (paye.MontantRecu < CurrentTicket.Total)
                //{
                //    CurrentTicket.MontantPaye += paye.MontantRecu;
                //    MessageQueue.Enqueue("PAIEMENT PARTIEL ENREGISTRÉ");

                //}

                CreateCartLines();
                if (ShowTicketsVisible)
                    ShowTickets();
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
            aViewManager = DataHelpers.container.Get<ViewManager>();
            // AddTicket();
            CartData = new BindableCollection<UIElement>();

            var notHnadled = DS.db.GetAll<PosTicket>(a => a.isHandled == 0) as IEnumerable<PosTicket>;
            Tickets.AddRange(notHnadled);

            BackgroundWorker worker = new BackgroundWorker();
            timer = new Timer(new TimerCallback(SyncData),Tickets,0,5000);



        }

        public void SyncData(object o)
        {

            var _tickets = o;
            var notHnadled = DS.db.GetAll<PosTicket>(a => a.isHandled == 0 && a.FromTablet) as IEnumerable<PosTicket>;

            if (notHnadled == null)
                return;

            foreach (var item in notHnadled)
            {
                item.FromTablet = false;
                var here = Tickets.Where(a => a.NameTicket == item.NameTicket).FirstOrDefault();
                if(here != null)
                {
                     here.CarteLines = item.CarteLines;
                }
                else
                {
                    Tickets.Add(item);
                }

                if (item.DoPrintFromTablet)
                {
                    item.Position = this.Position;
                    this.Position++;
                    item.EnvoyerCuisine();
                    item.DoPrintFromTablet = false;
                    item.isLocal = false;
                    item.Save();
                }
            }
            foreach (var item in Tickets.Reverse())
            {                 
                item.FromTablet = false;
                try{
                    var result = item.Save(); 
                    if (!result)
                    {
                        var isThere = DS.db.GetOne<PosTicket>(a => a.Id == item.Id);
                        if(isThere == null)
                        {
                            Tickets.Remove(item);
                        }
                    }
                }
                catch(Exception s) { Console.WriteLine("=======> "+s.Message ); continue; }
            }


            NotifyOfPropertyChange("CurrentTicket");
            if(SelectedCartLine == null)
                Execute.OnUIThread(CreateCartLines);
            

            if(ShowTicketsVisible)
            {
                Execute.OnUIThread(ShowTickets);
            }
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

            if (!sessions.Any() || sessions.First(  ).CreatedBy != DataHelpers.ConnectedUser?.Id)
            { 
                var newSession = new SessionPos() { DateSession = DateTime.Now };
                await DataHelpers.Shell.OpenScreenDetach(newSession, "Nouvelle session pos");
                CurrentSession = newSession;
            }
            else  
            {
                CurrentSession = sessions.FirstOrDefault();
            }

            RefreshAll();
        }

        public async void CloseSession()
        {
            //MessageBox.Show("Saisir le montant de cloture");
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
            ShowTicketsVisible = false; 
        }


        public async void LoadCategorie(bool reload = false)
        {
            if (reload || Categories == null || Categories?.Any() == false)
            {
                Categories = new BindableCollection<Button>();
                var cats = await DataHelpers.instanc().GetData<GroupeArticle>(a => true) as IEnumerable<GroupeArticle>;
                foreach (var item in cats)
                {
                    var btnCat = new Button();

                    btnCat.Content = item.Name;
                    //btnCat.MinWidth = 200;
                    btnCat.Height = 50;
                    btnCat.Tag = item;
                    btnCat.Click += BtnCat_Click1;
                    btnCat.TouchDown += BtnCat_Click1;
                    btnCat.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btnCat.HorizontalContentAlignment = HorizontalAlignment.Center;
                    btnCat.Margin = new Thickness(3);
                    btnCat.Background = Brushes.Black;

                    Categories.Add(btnCat);
                }
            }

            NotifyOfPropertyChange("Categories");
        }
        public PosSettings settings { get; set; } = DataHelpers.PosSettings;


        public IEnumerable<Article> AllArticles { get; set; } = new List<Article>();

        public async Task Setup(IEnumerable<Article> _items = null)
        {
            // AllCartLines = new HashSet<CartLine>();
           
            _EstPrepayeOnly = settings.EstPrepayeOnly;
           // DispalyNameproperty = string.IsNullOrWhiteSpace(settings.NameProperty) ? "Name" : settings.NameProperty;

            Products = new BindableCollection<UIElement>();
            //Categories = new BindableCollection<Button>();
            LoadCategorie();
            if (_items == null)
            {
                //   _items = await DataHelpers.instanc().GetMongoDataPaged<Article>(1, 50) as IEnumerable<Article>;
                AllArticles = await DataHelpers.instanc().GetData<Article>(a => true) as IEnumerable<Article>;
                _items = AllArticles;
            }

            foreach (var item in _items)
            {
                //Products = new ObservableCollection<UIElement>();
                var productBox = new ProductBoxViewModel(item);
                productBox.clickEvent += ProductBox_clickEvent;
                var view = aViewManager.CreateAndBindViewForModelIfNecessary(productBox);
                Products.Add(view);
             //   
            }
            //Categories.Refresh();
            //Products.Refresh();
            NotifyOfPropertyChange("Products");
            NotifyOfPropertyChange("CartData");
            NotifyOfPropertyChange("Total");

            ShowTicketsVisible = false;
        }

        private ViewManager aViewManager;
        private async void BtnCat_Click1(object sender, RoutedEventArgs e)
        {

            var category = (sender as Button).Tag as GroupeArticle;

            if (category != null)
            {
                //var items = DataHelpers.GetMongoData("Article", "lGroupeArticle", category.Id, false) as IEnumerable<Article>;
                var items = AllArticles.Where(a => a.lGroupeArticle == category.Id);
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
                var cartLine = new CartLine(article,CurrentTicket?.ListePrix);
                AddQteOrItem(1, cartLine);

            }


           
        }

        private void ProductBox_clickEvent1(object sender, EventArgs e)
        {
            var article = (sender as ProductBoxViewModel).product;
            var variante = (sender as ProductBoxViewModel).variante;
            var cartLine = new CartLine(article,variante,CurrentTicket?.ListePrix);
            AddQteOrItem(1, cartLine);
           // MessageBox.Show("Variante");
            return;
        }

        private void SetQteOrItem(decimal qte, CartLine cart)
        {
            try
            {

                var exited = CurrentTicket?.CarteLines?.Where(a => a == cart).FirstOrDefault();
                if (exited != null)
                {// Just add qts
                    exited.Qts = qte;
                }
                else
                {
                    //AllCartLines?.Add(cart);
                }
                if (CurrentTicket != null)
                    CurrentTicket.EstEnvoyerCuisine = false;
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

                var exited = CurrentTicket?.CarteLines?.Where(a => a == cart).FirstOrDefault();
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
            var exited = CurrentTicket?.CarteLines?.Where(a => a.article.Name == cart.article.Name 
            && (a.variante == cart.variante || a.variante?.Name == cart.variante.Name)
             ).FirstOrDefault();
            if (exited != null)
            {// Just add qts
                exited.Qts++;
            }
            else
            {
                CurrentTicket?.CarteLines?.Add(cart);
            }

            if(CurrentTicket!=null)
                CurrentTicket.EstEnvoyerCuisine = false;
            
            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");

            CreateCartLines();
        }

        public void CreateCartLines()
        {
            CartData.Clear();
            if (CurrentTicket?.CarteLines == null)
            {
                NotifyOfPropertyChange("CartData");
                NotifyOfPropertyChange("Total");

                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");
                return;
            }
           
            foreach (var cartLine in CurrentTicket?.CarteLines)
            {
                cartLine.ListePrix = CurrentTicket.ListePrix;
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
                if(line == null)
                {
                    MessageBox.Show("Selectionner un repas!");
                    return;
                }

                var host = new Window();
                host.Width = 750;
                host.Height = 650;
                host.Background = Brushes.LightGray;



                //var scroll = new ScrollViewer();
                //scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                //scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                var sp = new StackPanel();
                sp.CanVerticallyScroll = true;
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
                btn.Margin = new Thickness(10, 20, 10, 10);
                btn.Content = "OK";
                btn.VerticalAlignment = VerticalAlignment.Bottom;
                
                var msgs = DS.db.GetAll<MessageCommande>(a => true) as IEnumerable<MessageCommande>;
                var list = new WrapPanel();
                 
                foreach (var item in msgs)
                {
                    var btnMsg = new Button();
                    btnMsg.Content = item.Message;
                    btnMsg.Tag = item;
                    btnMsg.Background = Brushes.Black;
                    btnMsg.Margin = new Thickness(5);
                    btnMsg.Click += delegate
                    {
                        text.Text += btnMsg.Content?.ToString() + " / ";
                    };

                    list.Children.Add(btnMsg);

                }

               // list.ItemsSource = msgs.Select(z => z.Message);
                //list.FontWeight = FontWeights.Bold;
              
                //list.MaxHeight = 200;
               // list.Margin = new Thickness(10);
               // list.Background = Brushes.White;
                //list.SelectionChanged += delegate
                //{
                //    text.Text += list.SelectedItem?.ToString()+" / ";
                //};
                btn.Click += delegate
                {
                    var msg = text.Text; 
                     host.Tag = msg; 
                    host.Hide();
                };
                btn.TouchDown += delegate
                {
                    var msg = text.Text;
                    host.Tag = msg;
                    host.Hide();
                };

                 

                sp.Children.Add(btn);
                sp.Children.Add(text);
                sp.Children.Add(list);

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
                CurrentTicket?.CarteLines?.Remove(cartline);
                if (CurrentTicket != null)
                    CurrentTicket.EstEnvoyerCuisine = false;
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
            var ticketType = new PosSelectViewModel(CurrentTicket,Tickets);

            var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(ticketType);
            DataHelpers.windowManager.ShowDialog(ticketType);

            CurrentTicket = ticketType.currentTicket;
            //CurrentTicket?.Refresh();
            //Tickets?.Refresh();
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CurrentTicket");

            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
            if (ShowTicketsVisible)
                ShowTickets();
        }

        public void Close()
        {
            try
            {
                timer.Dispose();
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

            //CurrentTicket?.Refresh();
            //Tickets?.Refresh();
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
            //CurrentTicket?.Refresh();
            //Tickets?.Refresh();
            NotifyOfPropertyChange("CurrentTicket");
            NotifyOfPropertyChange("Tickets");
            NotifyOfPropertyChange("CmdStatus");
            NotifyOfPropertyChange("CmdColor");
        }

        public void DeleteTicket()
        {
            if (CurrentTicket != null)
            {
                CurrentTicket.Delete(false);
                Tickets.Remove(CurrentTicket);
                NotifyOfPropertyChange("Tickets");

                NotifyOfPropertyChange("CmdStatus");
                NotifyOfPropertyChange("CmdColor");
                CreateCartLines();

                MessageQueue.Enqueue("Ticket supprimer");
                NextTicket();

                if (ShowTicketsVisible)
                    ShowTickets();

               
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

                //CurrentTicket?.Refresh();
                //Tickets?.Refresh();

                NotifyOfPropertyChange("CurrentTicket");
                NotifyOfPropertyChange("Tickets");

                if (ShowTicketsVisible)
                    ShowTickets();
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

                if (ShowTicketsVisible)
                    ShowTickets();
            }
        }
        public bool ShowTicketsVisible { get; set; } = false;
        public void ShowTickets()
        {
            var tickets = this.Tickets;
            //if(tickets == null || !tickets.Any())
            //{
            //    MessageBox.Show("Aucun ticket!");
            //    return;
            //}

            ShowTicketsVisible = true;
            Products = new BindableCollection<UIElement>();

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
                text.Text = item.NameTicket;
                text.Margin = new Thickness(0, 10, 0, 0);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                sp.Children.Add(icon);
                sp.Children.Add(text);
                ticket.Content = sp;

                ticket.Click += Ticket_Click;
                ticket.TouchDown += Ticket_Click;

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

            //ShowTickets();
           // RefreshAll();
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

      public void OuvrirMsg()
        {
            CartLineView_DoubleClick(SelectedCart, EventArgs.Empty);
        }


        public void ShowTicktsOld()
        {
            this.ExpandView();
            DataHelpers.Shell.OpenScreenFindAttach(typeof(PosTicket), "Historique Tickets");
       
        }

        public int CatScroll { get; set; }

        public void CatUp()
        {

        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            try
            {
                var ticketOld = DS.db.GetAll<PosTicket>(a => a.isHandled == 0 && a.Date < DateTime.Today.AddDays(-1));
                DS.db.DeleteMany(ticketOld);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }
    }
}
