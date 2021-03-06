using System;
using Stylet;
using ErpAlgerie.Modules.Core.Helpers;
using StyletIoC;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Pages.Template;
using System.Collections.Generic;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Pages.Events;
using System.Linq;
using System.Collections;
using MongoDbGenericRepository.Models;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using MongoDB.Bson;
using System.Drawing;
using System.Windows.Media;
using ErpAlgerie.Pages.PopupWait;
using ErpAlgerie.Pages.Helpers;
using ErpAlgerie.Modules.CRM;
using System.Xml;
using FontAwesome.WPF;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using ErpAlgerie.Framework;
using System.IO;
using ErpAlgerie.Modules.Core;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.POS;
using MahApps.Metro.Controls;
using ErpAlgerie.Pages.Home;
using ErpAlgerie.Pages.Reports;
using ErpAlgerie.Modules.REPORTS;
using ErpAlgerie.Pages.LicenceManage;

namespace ErpAlgerie.Pages
{

    public class TreeViewItemEx
    {
        public TreeViewItemEx(string name, string director)
        {
            Name = name;
            Director = director;
        }

        public string Name { get; }
        public string Director { get; }
        public XmlNode NodeXml { get; set; }
    }

    public class TreeViewItemExCategory
    {
        public TreeViewItemExCategory(string name)
        {
            Name = name;
        }

        public TreeViewItemExCategory(string name, params TreeViewItemEx[] items)
        {
            Name = name;
            Items = new ObservableCollection<TreeViewItemEx>(items);
        }

        public string Name { get; }
        public ObservableCollection<TreeViewItemEx> Items { get; set; }
    }

    public class ExpanderMenu : Expander
    {
        public ImageSource iconSource { get; set; }
    }

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell, IHandle<ModelChangeEvent>
    {

        //       
        //}


        public string Applogo
        {
            get
            {
                if (DataHelpers.Settings.AppLogo != null)
                    return Path.GetFullPath(DataHelpers.Settings.AppLogo);
                return "";
            }
        }
        public async Task OpenModuleErp(ModuleErp module)
        {
            var displayname = module.Libelle;
            var className = module.ClassName;
            var iconImg = module.ModuleIcon;
            var instance = module.IsInstanceModule;
            var instanceFunction = module.InstanceFunction;


            var found = Items.FirstOrDefault(a => a.DisplayName == displayname);
            if (found != null)
            {
                ActivateItem(found);
            }
            else
            {
                if (instance == false)
                {
                    var baseView = DataHelpers.GetBaseViewModelScreen(Type.GetType(className), aggre, false);
                    baseView.DisplayName = displayname;
                    Items.Add(baseView);
                    ActivateItem(baseView);
                }
                else
                {
                    try
                    {
                        var one = Activator.CreateInstance(Type.GetType(className));
                        var oneInstance = (ExtendedDocument)one.GetType()
                            .GetMethod(instanceFunction).
                            Invoke(one, null);
                        this.OpenScreen(await DetailViewModel.Create(oneInstance, oneInstance.GetType(), aggre, this), $"{displayname}");
                    }
                    catch (Exception s)
                    {
                        MessageBox.Show(s.Message);
                        return;
                    }
                }
            }
        }

        public void OpenReport()
        {

            var repor = new ReportViewModel(new SoldeStock());
            this.Items.Add(repor);
            ActivateItem(repor);
        }

        public async void MenuItemChange(object sender, EventArgs args)
        {
            try
            {
                var nodeZ = (sender as TextBlock).GetBindingExpression(TextBlock.TextProperty).DataItem as TreeViewItemEx;//.NodeXml;

                var node = nodeZ.NodeXml;
                var modelHeader = node.Attributes["header"].Value;
                var modelClass = node.Attributes["class"].Value;
                var modelIcon = node.Attributes["icon"].Value;
                var ins = node.Attributes["instance"];



                var found = Items.FirstOrDefault(a => a.DisplayName == modelHeader);
                if (found != null)
                {
                    ActivateItem(found);
                }
                else
                {
                    if (ins == null)
                    {

                        //Type d1 = typeof(BaseViewModel<>);
                        //Console.Write(d1.GetGenericTypeDefinition());
                        //Console.WriteLine(d1.GenericTypeArguments);
                        //Type[] typeArgs = { Type.GetType(modelClass) } ;
                        //Type makeme = d1.MakeGenericType(typeArgs);
                        //dynamic baseViewModel = Activator.CreateInstance(makeme,new object[]{ aggre,false});
                        //var control = baseViewModel;
                        //var screnn = control as IScreen;
                        //screnn.AttachView(new BaseView());
                        //// List<string> itsMe = o as List<string>;

                        var baseView = DataHelpers.GetBaseViewModelScreen(Type.GetType(modelClass), aggre, false);
                        // var control = await BaseViewModel.Create(Type.GetType(modelClass), this, aggre, windowManager);
                        baseView.DisplayName = modelHeader;

                        Items.Add(baseView);
                        ActivateItem(baseView);
                    }
                    else
                    {
                        try
                        {
                            var isntance = ins.Value;
                            var one = Activator.CreateInstance(Type.GetType(modelClass));
                            var oneInstance = (ExtendedDocument)one.GetType()
                                .GetMethod(isntance).
                                Invoke(one, null);
                            this.OpenScreen(await DetailViewModel.Create(oneInstance, oneInstance.GetType(), aggre, this), $"{modelHeader}");
                        }
                        catch (Exception s)
                        {
                            MessageBox.Show(s.Message);
                            return;
                        }
                    }

                }
            }
            catch (Exception s)
            {
                MessageBox.Show($@"Erreur de navigation!! contactez le fournisseur
                        {s.Message}", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            collapseMenu();
        }


        public string ConnectedUser
        {
            get
            {
                return DataHelpers.ConnectedUser?.Name;
            }
        }
        protected override void OnInitialActivate()
        {

            #region LOGIN
            var login = new LoginViewModel();
            var res = windowManager.ShowDialog(login);

            if (!res.HasValue || !res.Value)
            {
                App.Current.Shutdown(0);
            }
            NotifyOfPropertyChange("ConnectedUser");
            #endregion
        }

        public void ClosingTab()
        {

        }
        public ScrollViewer sideMenu { get; set; } = new ScrollViewer();
        public int menuWidth { get; set; } = 200;
        public void collapseMenu(bool open = false)
        {
            if (open)
            {
                menuWidth = 200;
                NotifyOfPropertyChange("menuWidth");
                return;
            }
            if (menuWidth == 0)
            {
                menuWidth = 200;
            }
            else
            {
                menuWidth = 0;
            }
            NotifyOfPropertyChange("menuWidth");
        }
        public bool MenuIsExpanded { get; set; } = true;

        public List<TreeViewItemExCategory> MainMenuCategories { get; set; } = new List<TreeViewItemExCategory>();
        public async Task SetupSideMenu()
        {

            // Init actions
            MainMenuCategories.Clear();

            //      sideMenu.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            // Load config models
            var models = new List<ExtendedDocument>();
            var config = new XmlDocument();
            config.Load("Modules/Models.xml");

            // get ModulesERP
            var modules = DataHelpers.GetMongoDataSync("ModuleErp") as IEnumerable<ModuleErp>;
            config.DocumentElement.RemoveAll();

            var TopMenus = modules.Where(z => !string.IsNullOrEmpty(z.GroupeModule)).Select(a => a.GroupeModule).Distinct();
            foreach (var groupe in TopMenus)
            {
                var moduleOfGroupe = modules.Where(a => a.GroupeModule == groupe);

                //(2) string.Empty makes cleaner code
                XmlElement section = config.CreateElement(string.Empty, "section", string.Empty);
                section.SetAttribute("header", groupe);
                section.SetAttribute("icon", "Desktop");
                foreach (var module in moduleOfGroupe)
                {
                    XmlElement document = config.CreateElement(string.Empty, "item", string.Empty);
                    //header ="Clients" icon="Users" description="" class="ErpAlgerie.Modules.CRM.Client"
                    document.SetAttribute("header", module.Libelle);
                    document.SetAttribute("icon", "Users");
                    document.SetAttribute("class", module.ClassName);
                    if (module.IsInstanceModule)
                        document.SetAttribute("instance", "getInstance");
                    section.AppendChild(document);
                }
                config.DocumentElement.AppendChild(section);

            }
            config.Save("Modules/Models.xml");
            config.Load("Modules/Models.xml");
            parentStack = new StackPanel();
            // Iteerate over items

            foreach (XmlNode node in config.DocumentElement.ChildNodes)
            {
                // iterate over sections
                Console.Write(node.ChildNodes);
                var header = node.Attributes["header"].Value;

                // ExpanderMenu expander = new ExpanderMenu();

                //   var nodeTree = new TreeViewItemExCategory(header, new Li);
                var Children = new List<TreeViewItemEx>();
                //
                var items = node.ChildNodes;

                foreach (XmlNode item in items)
                {
                    var modelHeader = item.Attributes["header"].Value;
                    var modelClass = item.Attributes["class"].Value;
                    var modelIcon = item.Attributes["icon"].Value;
                    var modelInstance = item.Attributes["instance"]?.Value;


                    var itemNode = new TreeViewItemEx(modelHeader, modelHeader);
                    itemNode.NodeXml = item;
                    //   itemNode.HeaderContent = new Button() { Content = modelHeader };

                    var i = new FontAwesome.WPF.ImageAwesome();
                    // i.Icon = (FontAwesomeIcon)Enum.Parse(typeof(FontAwesomeIcon), modelIcon);
                    i.Icon = FontAwesomeIcon.Plus;
                    i.Foreground = System.Windows.Media.Brushes.LightGray;
                    // itemNode.HeaderIcon = modelIcon;
                    Children.Add(itemNode);

                }
                var Category = new TreeViewItemExCategory(header, Children.ToArray());
                MainMenuCategories.Add(Category);

            }
            NotifyOfPropertyChange("sideMenu");
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var allexpanders = parentStack.Children;

            foreach (Expander item in allexpanders)
            {
                if (item != (sender as Expander) && item.IsExpanded == true)
                    item.IsExpanded = false;
            }

            //if((sender as Expander).IsExpanded == false)
            //{
            //    (sender as Expander).IsExpanded = true;
            //}

        }



        public async void OpenInstance(ExtendedDocument doc)
        {
            this.OpenScreen(await DetailViewModel.Create(doc, doc.GetType(), aggre, this), $"{doc.CollectionName} - {doc.Name}");
        }

        IContainer container;
        private IWindowManager windowManager;
        IEventAggregator aggre;
        private StackPanel parentStack;

        public List<DateTime> months { get; set; } = new List<DateTime>();
        public DateTime selectedMonth { get; set; } = DateTime.Today;
        public ShellViewModel(IContainer container, IEventAggregator _aggre, IWindowManager windowManager)
        {


            this.windowManager = windowManager;
            DataHelpers.windowManager = this.windowManager;
            DataHelpers.Shell = this;
            DataHelpers.Aggre = _aggre;
            DataHelpers.container = container;
            aggre = _aggre;
            aggre.Subscribe(this);
            this.container = container;


            this.StateChanged += ShellViewModel_StateChanged;

            for (int i = 0; i < 11; i++)
            {
                months.Add(DateTime.Today.AddMonths(1 - i));
            }
            NotifyOfPropertyChange("months");

            var setupside = SetupSideMenu();
            setupside.Wait();
            NotifyOfPropertyChange("MainMenuCategories");


        }
        public string SearchMenuText { get; set; }

        public async void SearchMenuTextChanged()
        {

            collapseMenu(true);
            await SetupSideMenu();

            List<TreeViewItemExCategory> newResult = new List<TreeViewItemExCategory>();

            MainMenuCategories.ForEach(cat =>
            {
                var toleave = cat.Items.Where(a => a.Name.ContainsIgniorCase(SearchMenuText));

                var newCat = new TreeViewItemExCategory(cat.Name);
                newCat.Items = new ObservableCollection<TreeViewItemEx>(toleave);
                if (newCat.Items.Any())
                    newResult.Add(newCat);
            });

            MenuIsExpanded = true;
            NotifyOfPropertyChange("MenuIsExpanded");
            MainMenuCategories = newResult;
            NotifyOfPropertyChange("MainMenuCategories");

        }


        private void ShellViewModel_StateChanged(object sender, ScreenStateChangedEventArgs e)
        {

        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            OpenHome();
            //  SetupNotificationsItems();
        }

        public async void OpenPos()
        {
            var pos = new PointOfSaleViewModel();
            // DataHelpers.windowManager.ShowWindow(pos);
            //OpenScreen(pos, "POS");
            var ioc = DataHelpers.container;

            var view = DataHelpers.container.Get<ViewManager>().CreateAndBindViewForModelIfNecessary(pos);


            var cc = new ContentControl();
            cc.HorizontalAlignment = HorizontalAlignment.Stretch;
            cc.VerticalAlignment = VerticalAlignment.Stretch;
            cc.Content = view;
            // GenericWindowViewModel gw = new GenericWindowViewModel(cc, "POS", "POS");

            Window win = new Window();
            win.WindowState = WindowState.Maximized;
            win.WindowStyle = WindowStyle.None;
            win.Content = cc;
            win.Show();
            //var windo = cc.GetParentObject().TryFindParent<Window>();
            //windo.WindowState = WindowState.Maximized;
            //windo.WindowStyle = WindowStyle.None;

            //windowManager.ShowWindow(gw);

        }
        public void OpenScreen(IScreen screen, string title)
        {
            try
            {
                screen.DisplayName = title;
                Items.Add(screen);
                ActivateItem(screen);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }
        public async Task OpenScreenDetach(IScreen screen, string title)
        {

            var ioc = DataHelpers.container;
            var vm = ioc.Get<ViewManager>();
           // var c = await DetailViewModel.Create(screen, screen.GetType(), aggre, this);

            //c.DisplayName = screen.CollectionName;
            var content = vm.CreateAndBindViewForModelIfNecessary(screen);

            var cc = new ContentControl();
            cc.HorizontalAlignment = HorizontalAlignment.Stretch;
            cc.VerticalAlignment = VerticalAlignment.Stretch;
            cc.Content = content;

            GenericWindowViewModel gw = new GenericWindowViewModel(cc, title, title);
            windowManager.ShowDialog(gw);
        }
        public async Task OpenScreenDetach(ExtendedDocument screen, string title)
        {

            var ioc = DataHelpers.container;
            var vm = ioc.Get<ViewManager>();
            var c = await DetailViewModel.Create(screen, screen.GetType(), aggre, this);

            c.DisplayName = screen.CollectionName;
            var content = vm.CreateAndBindViewForModelIfNecessary(c);

            var cc = new ContentControl();
            cc.HorizontalAlignment = HorizontalAlignment.Stretch;
            cc.VerticalAlignment = VerticalAlignment.Stretch;
            cc.Content = content;

            GenericWindowViewModel gw = new GenericWindowViewModel(cc, c.DisplayName, screen.Name);
            windowManager.ShowDialog(gw);
        }

        public async void OpenScreenAttach(ExtendedDocument screen, string title)
        {

            try
            {
                var c = await DetailViewModel.Create(screen, screen.GetType(), aggre, this);

                c.DisplayName = screen.CollectionName;
                Items.Add(c);
                ActivateItem(c);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return;
            }
        }

        public void CloseScreen(IScreen screen)
        {
            DeactivateItem(screen);
            Items.Remove(screen);
            //Items.(screen);

        }

        /// <summary>
        /// Click pour ouvrir les listes
        /// </summary>
        #region SHOW COMMANDS







        public void showBulkEntryViewModel()
        {
            //var control = new BulkEntryViewModel(typeof(Collect), aggre, windowManager);
            //control.DisplayName = "Entrée en masse";
            //Items.Add(control);
            //ActivateItem(control);
            //MessageBox.Show("Sous développement");
            //return;
        }




        public void showClose()
        {
            System.Windows.Application.Current.Shutdown();
        }


        public void showUpdate()
        {
            //  AutoUpdater.Start("https://www.dl.dropboxusercontent.com/s/pwhbwgwvo6oku0i/Releases.xml");
        }
        #endregion

        public void Handle(ModelChangeEvent message)
        {
            // SetupNotificationsItems();
        }


        public void ActualiserApp()
        {
            this.OpenScreen(new HomeMenuViewModel(), $"Etat réservoir");

        }





        public void showActiontemp()
        {

        }



        public void CloseApp()
        {
            this.RequestClose();
        }


        /////// NOTIFICATIONS 

        public List<MenuItem> NotificationItems { get; set; } = new List<MenuItem>();

        public void OpenModuleBuilder()
        {
            var ModuleBuimder = new ModuleBuilderViewModel();
            OpenScreen(ModuleBuimder, "Editor");
        }


        public void OpenHome()
        {
            var vm = new HomeViewModel();
            var found = Items.FirstOrDefault(a => a.DisplayName == "Home");
            if (found != null)
            {
                ActivateItem(found);
                return;
            }

            OpenScreen(vm, "Home");
        }

        public void OpenScreenFindAttach(Type docType, string displayName)
        {

            try
            { 
                var control = DataHelpers.GetBaseViewModelScreen(docType, aggre, false);
                control.DisplayName = displayName;
                this.ActivateItem(control);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                
            }

        }


        public IEnumerable<ExtendedDocument> OpenScreenFind(Type docType, string displayName)
        {

            try
            {
                //Type d1 = typeof(BaseViewModel<>);
                //Type[] typeArgs = { docType };
                //Type makeme = d1.MakeGenericType(typeArgs);
                //dynamic baseViewModel = Activator.CreateInstance(makeme, new object[] { aggre, true });
                //var control = baseViewModel;

                var control = DataHelpers.GetBaseViewModelScreen(docType, aggre, true);

                // var control = BaseViewModel.CreateSyncSelect(docType, this, aggre, windowManager);
                var ioc = DataHelpers.container;
                var vm = ioc.Get<ViewManager>();
                // var c = await DetailViewModel.Create(screen, screen.GetType(), aggre, this);
                control.DisplayName = displayName;
                var content = vm.CreateAndBindViewForModelIfNecessary(control);

                var cc = new ContentControl();
                cc.HorizontalAlignment = HorizontalAlignment.Stretch;
                cc.VerticalAlignment = VerticalAlignment.Stretch;
                cc.Content = content;

                GenericWindowViewModel gw = new GenericWindowViewModel(cc, displayName, "");
                var res = windowManager.ShowDialog(gw);
                var result = (control as dynamic).selectedList;

                return result;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return null;
            }

        }

        public void OpenLicence()
        {
            var lm = new LicenceManagerViewModel();
            DataHelpers.windowManager.ShowWindow(lm);
        }

        public string Customer { get { return DataHelpers.Customer; } }
    }
}
