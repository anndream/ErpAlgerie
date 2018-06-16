using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Pages.Events;
using ErpAlgerie.Pages.Helpers;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using OvImport;
using PagedList;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ErpAlgerie.Pages.Template
{
    public class BaseViewModel<T> : Screen, IDisposable, IHandle<ModelChangeEvent> where T : ExtendedDocument, new()
    {
        public IWindowManager windowManager;
        public IShell shell { get; set; }
        public ExtendedDocument selected { get; set; }

        public IEnumerable<ExtendedDocument> selectedList { get; set; }
        private IEventAggregator aggre;
        public int PageNumber { get; set; } = 1;
        public int PageCount { get; set; } = 20;
        public BindableCollection<T> Items { get; set; } = new BindableCollection<T>();
        public DataHelpers datahelper { get; set; } = new DataHelpers();
        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
        private bool fromFiltre = false;

        public StackPanel opeartionButtons { get; set; }

        

        public Type type
        {
            get
            {
                return typeof(T);
            }
        }
        public List<int> ShowCounts
        {
            get
            {
                return new List<int>()
                {   20,
                    50,
                    100,
                    200,
                    500
                };
            }
        }

        IEnumerable<T> _list;

        public bool ForSelectOnly { get; set; } = false;

        public BaseViewModel()
        {
        }

        public BaseViewModel(IEventAggregator _aggre, bool ForSelectOnly)
        {
            this.ForSelectOnly = ForSelectOnly;
            shell = DataHelpers.Shell;
            windowManager = DataHelpers.windowManager;
            aggre = _aggre;
            Task.Run(() => NextPage().Wait());

            SetupOperationButtons();
        }

        public BaseViewModel(IEventAggregator _aggre, bool ForSelectOnly, IEnumerable<T> _list)
        {
            this._list = _list;
            PAGE_MODE = PAGE_MODES.LIST;
            fromFiltre = true;
            this.ForSelectOnly = ForSelectOnly;
            shell = DataHelpers.Shell;
            windowManager = DataHelpers.windowManager;
            aggre = _aggre;
            Task.Run(() => NextPage().Wait());

            SetupOperationButtons();

        }


        public void SetupOperationButtons()
        {
            opeartionButtons = new StackPanel();

            var sourceType = type;
            var baseAction = type.GetProperties().Where(a => (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute) != null
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute).FieldType == ModelFieldType.BaseButton);

            foreach (var action in baseAction)
            {
                var attrib = (action.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute);
                var attribDisplay = (action.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute);
                Button newOps = new Button();
                newOps.Content = attribDisplay.DisplayName;
                newOps.Click += NewOps_Click;
                newOps.Style = App.Current.FindResource("SideToolButton") as Style;
                newOps.Tag = attrib.Options;  // <= the name of the function
                opeartionButtons.Children.Add(newOps);
               
            } 
           
            opeartionButtons.Orientation = Orientation.Horizontal;             
          

        }

        private void NewOps_Click(object sender, RoutedEventArgs e)
        {
            var method = (sender as Button).Tag.ToString();
            var instance = new T();
            
            (instance as ExtendedDocument).DoFunction(method);
            Actualiser();
        }

        public static void Outside()
        {

        }

        

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
         
        }
        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            InitContextMenu();
            aggre.Subscribe(this);
        }
        public bool IsRunning { get; set; }
        public async Task NextPage()
        {
            if (IsRunning)
                return;

            IsRunning = true;


            switch (PAGE_MODE)
            {
                case PAGE_MODES.ALL:
                    SearchResul = new List<T>();
                    TotalCount = await datahelper.GetMongoDataCount<T>();
                    Items.Clear();
                    Items.AddRange(await datahelper.GetMongoDataPaged<T>(PageNumber, PageCount));
                    break;
                case PAGE_MODES.FILTER_TEXT:
                    TotalCount = await datahelper.GetMongoDataCount<T>(a => a.Name.Contains(NameSearch.ToLower()));                 
                    Items.Clear();
                    Items.AddRange(await datahelper.GetMongoDataFilterPaged<T>(NameSearch.ToLower(), PageNumber, PageCount));
                    break;
                case PAGE_MODES.FILTER_BOX:
                    TotalCount = SearchResul.Count;
                    Items.Clear();
                    Items.AddRange(SearchResul.Skip((PageNumber - 1) * PageCount).Take(PageCount));
                    break;
                case PAGE_MODES.LIST:
                    TotalCount = _list.Count();
                    Items.Clear();
                    Items.AddRange(_list.Skip((PageNumber - 1) * PageCount).Take(PageCount));
                    break;
                default:
                    break;
            }
           
           
            Items.Refresh();
            NotifyOfPropertyChange("Items");
            NotifyOfPropertyChange("ElementsCount");
            IsRunning = false;
        }
         
        public  void GridKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ouvrirItem(); 
            }
        }
        public async void ouvrirItem()
        {
            try
            {
                if (selected != null)
                {
                    // Show for select item  only
                    if (ForSelectOnly)
                    {
                        selectedList = Items.Where(a => a.IsSelectedd);
                        ForSelectOnly = false;
                        CloseWindows();
                    }
                    else
                    {

                        if (selected.DocOpenMod == OpenMode.Attach)
                        {
                           // selected.dis = displayName;
                            shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"{selected.CollectionName} - {selected.Name}");
                        }
                        else
                        {
                            var ioc = DataHelpers.container;
                            var vm = ioc.Get<ViewManager>();
                            var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, shell);
                            c.DisplayName = selected.CollectionName;
                            var content = vm.CreateAndBindViewForModelIfNecessary(c);

                            var cc = new ContentControl();
                            cc.HorizontalAlignment = HorizontalAlignment.Stretch;
                            cc.VerticalAlignment = VerticalAlignment.Stretch;
                            cc.Content = content;

                            GenericWindowViewModel gw = new GenericWindowViewModel(cc, displayName, selected.Name);
                            windowManager.ShowWindow(gw);
                        }
                    }
                }
            }
            catch (Exception s)
            {
                MessageQueue.Enqueue(s.Message);
                return;
            }
        }

        public async Task Actualiser()
        {
            if(!fromFiltre)
                PAGE_MODE = PAGE_MODES.ALL;
            NameSearch = "";
            await NextPage();
            NotifyOfPropertyChange("NameSearch");
            NotifyOfPropertyChange("PageNumber");
            MessageQueue.Enqueue("Données actualisées");
        }

       

        public async void Add()
        {
            try
            {
                selected = new T();


                if (selected.DocOpenMod == OpenMode.Attach && ForSelectOnly == false)
                {
                    //selected.CollectionName = displayName;
                    shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"{this.DisplayName} | {selected.Name}");
                }
                else
                {
                    var ioc = DataHelpers.container;
                    var vm = ioc.Get<ViewManager>();
                    var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, shell);

                    await shell.OpenScreenDetach(selected, selected.Name);
                    //var content = vm.CreateAndBindViewForModelIfNecessary(c);

                    //var cc = new ContentControl();
                    //cc.HorizontalAlignment = HorizontalAlignment.Stretch;
                    //cc.VerticalAlignment = VerticalAlignment.Stretch;
                    //cc.Content = content;

                    //GenericWindowViewModel gw = new GenericWindowViewModel(cc, displayName, selected.Name);
                    //windowManager.ShowWindow(gw);
                }

            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return;
            }
        }

        public async void ModifierModule()
        {
            try
            {
                var instance = Activator.CreateInstance<T>();
                ModuleErp module = this.type.GetMethod("MyModule")?.Invoke(instance,null) as ModuleErp; // 
                if (module != null)
                {
                    await DataHelpers.Shell.OpenScreenDetach(module, module.CollectionName);
                }
            }
            catch (Exception s)
            {
                DataHelpers.Logger.LogError(s);
                throw;
            }
        }

        public void AjouterAuBureau()
        {
            var instance = Activator.CreateInstance<T>();
            var module = this.type.GetMethod("MyModule")?.Invoke(instance,null); // 
            if(module != null)
            {
                // Activate show
                (module as ModuleErp).EstAcceRapide = true;
                (module as ModuleErp).Save();
                MessageBox.Show($"Icon {instance.CollectionName}, ajoutée au bureau");
                return;
            }
        }


        public async void nextPage()
        {
            PageNumber++;
            
            await NextPage();
            NotifyOfPropertyChange("PageNumber");
            NotifyOfPropertyChange("CurrentPage");
        }

        public async void prevPage()
        {
            PageNumber--;
            if (PageNumber <= 0)
            {
                PageNumber = 1;
                return;
            }
            // await GetPages();
            await NextPage();
            NotifyOfPropertyChange("PageNumber");
        }


        public void ExportTemplate()
        {

            var dialog = new SaveFileDialog();
            var file = dialog.ShowDialog();
            if (file == true)
            {
                var result = dialog.FileName;
                var dp = new DynamicPath(result);
                var ovimport = new ExcelImport(dp);

                ovimport.ExportTemplate(result, type);
                Process.Start(result);
            }
        }

        public void ImportData()
        {
            try
            {

                var dialog = new OpenFileDialog();
                var file = dialog.ShowDialog();
                if (file == true)
                {
                    var result = dialog.FileName;
                    var ovimport = new ExcelImport(new DynamicPath(result));

                    var data = ovimport.ImportDataFromType(result, type);

                    if (data != null)
                    {
                        var count = data.Count;
                        var confirmation = MessageBox.Show($"{count} documents trouvés, voulez-vous continuer!", "Confirmation", MessageBoxButton.YesNo);
                        if (confirmation == MessageBoxResult.Yes)
                        {

                            foreach (var item in data)
                            {
                                item.AddedAtUtc = DateTime.Now;
                                item.isLocal = false;
                                // item.Save();
                            }
                            var mi = typeof(BaseMongoRepository).GetMethod("AddMany");
                            var gen = mi.MakeGenericMethod(type);
                            gen.Invoke(DS.db, new object[] { data });
                            //  DS.db.AddMany<t>(data);
                            MessageBox.Show("Terminé");
                            Actualiser();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Aucun document trouvé");
                        return;
                    }


                }

            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return;
            }
        }

        public void ExporterPDF()
        {
            windowManager.ShowWindow(new PrintWindowViewModel(Items));

        }

        public async void ValidateAll()
        {
            var selected = Items.Where(a => a.IsSelectedd);
            if (selected != null && selected.Any())
            {
                foreach (IModel item in selected)
                {
                    try
                    {
                        item.Submit();
                    }
                    catch (Exception s)
                    {
                        MessageBox.Show($"{s.Message}\n{((ExtendedDocument)item).Name}");
                        continue;
                    }

                }
                Actualiser();
            }
        }

        public async void DeleteAll()
        {
           
            var selected = Items.Where(a => a.IsSelectedd).ToList();
            var confirm = MessageBox.Show($"Voulez-vous supprimer ces {selected.Count} documents?", "Confirmation!",MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.No)
                return;


            if (selected != null && selected.Any())
            {
                foreach (IModel item in selected)
                {
                    if (!item.Delete(false))
                        break;
                }
                Actualiser();
            }
        }

















































        #region TEMP



        //public IWindowManager windowManager;
        private string _NameSearch;
        //private static int _SelectedCOunt = 5;
        //private IEventAggregator aggre;
        //private ComboBox filtreDropDown;
        //private List<string> filtreDropDownItems;
        //private bool fromFiltre = false;
        //private ICollectionView Itemlist;
        //private IEnumerable<IDocument> list;
        public int fontSize { get; set; } = 12;
        //private Thread tGetPages;
        //public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));

        public string StatusLabel { get; set; } = "...";


        public async void DoSearchKey()
        {
            await FilterThread();
        }


        public void BigFont()
        {
            fontSize++;
            NotifyOfPropertyChange("fontSize");
        }
        public void SmallFont()
        {
            fontSize--;
            NotifyOfPropertyChange("fontSize");
        }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public void InitContextMenu()
        {
            var menuOpen = new MenuItem();
            menuOpen.Header = "Ouvrir";
            menuOpen.Click += MenuOpen_Click;

            var menuDelete = new MenuItem();
            menuDelete.Header = "Supprimer";
            menuDelete.Click += MenuDelete_Click; ;


            MenuItems.Add(menuOpen);
            MenuItems.Add(menuDelete);

            NotifyOfPropertyChange("MenuItems");


        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                selected.IsSelectedd = true;
                DeleteAll();
            }
            else
            {
                DataHelpers.windowManager.ShowMessageBox("Selectionner une ligne!");
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            ouvrirItem();
        }

        //protected BaseViewModel(IEnumerable<IDocument> _list, Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowManager)
        //{
        //    this.windowManager = windowManager;

        //    fromFiltre = true;
        //    list = _list;

        //    aggre = _aggre;
        //    aggre.Subscribe(this);
        //    type = t;
        //    this.shell = shell;
        //    InitContextMenu();
        //}

        //protected BaseViewModel(Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowManager)
        //{
        //    this.windowManager = windowManager;
        //    fromFiltre = false;
        //    type = t;
        //    aggre = _aggre;
        //    aggre.Subscribe(this);
        //    this.shell = shell;
        //    InitContextMenu();
        //}

        //public string CurrentPage
        //{
        //    get
        //    {
        //        return $"{PageNumber * SelectedCOunt} /";
        //    }
        //}

        public string displayName
        {
            get
            {
                return this.DisplayName;
            }
        }
        public long TotalCount { get; set; }
        public string ElementsCount
        {
            get
            {
                return $"{Items.Count * PageNumber} sur {TotalCount} eléments ";
            }
        }
        //public BindableCollection<dynamic> FinalResults { get; set; } = new BindableCollection<dynamic>();
        //public List<IDocument> Items { get; set; } = new List<IDocument>();
        //public PagingCollectionView ItemsSource { get; set; }

        public string NameSearch
        {
            get { return _NameSearch; }
            set
            {
                _NameSearch = value;


            }
        }

        //public List<MenuItem> opeartionButtons { get; set; }

        //public static int PageNumber { get; set; } = 1;
        //public ExtendedDocument selected { get; set; }

        //public static int SelectedCOunt
        //{
        //    get { return _SelectedCOunt; }
        //    set
        //    {
        //        _SelectedCOunt = value;
        //        //ItemsSource = new PagingCollectionView(Items, SelectedCOunt);
        //      // Task.Run(async () => await LoadData());
        //    }
        //}

        ////  public List<dynamic> Original { get; set; } = new List<dynamic>();
        //public IShell shell { get; set; }

        //public Visibility ShowButtonAjouter { get; set; } = Visibility.Visible;

        //public List<int> ShowCounts
        //{
        //    get
        //    {
        //        return new List<int>()
        //        {   5,
        //            30,
        //            50,
        //            100,
        //            200,
        //            500
        //        };
        //    }
        //}

        //public Type type { get; set; }

        ////private async void loadFromType(Type type)
        ////{
        ////    list = await Task.Run(() => DataHelpers.GetMongoData(type.Name) as IEnumerable<IDocument>);
        ////    this.type = type;

        ////}

        //public static async Task<BaseViewModel> Create(IEnumerable<IDocument> _list, Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowsManager)
        //{
        //    BaseViewModel model = new BaseViewModel(_list, t, shell, _aggre, windowsManager);
        //    await model.LoadData(_list);

        //    return model;
        //}

        //public static async Task<BaseViewModel> Create(Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowsManager)
        //{
        //     methodInfo = typeof(BaseViewModel).GetMethod("CreateAsync");

        //      genericCreat = methodInfo.MakeGenericMethod(t);
        //     genericparam = new  object[] { t, shell, _aggre, windowsManager };
        //     return await ((Task < BaseViewModel > )genericCreat.Invoke(null, genericparam));

        //    //BaseViewModel model = new BaseViewModel(t, shell, _aggre, windowsManager);
        //    //await model.LoadData();
        //    //return model; 
        //}

        //public async Task Next()
        //{
        //    await((Task<BaseViewModel>)genericCreat.Invoke(null, genericparam));
        //}

        //public static async Task<BaseViewModel> CreateAsync<T>(Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowsManager) where T:ExtendedDocument, new()
        //{
        //    BaseViewModel model = new BaseViewModel(t, shell, _aggre, windowsManager);
        //    await model.LoadData<T>(PageNumber, SelectedCOunt);
        //    return model;

        //}

        //public static BaseViewModel CreateSyncSelect(Type t, IShell shell, IEventAggregator _aggre, IWindowManager windowsManager)
        //{
        //    ForSelectOnly = true;
        //    BaseViewModel model = new BaseViewModel(t, shell, _aggre, windowsManager);
        //    model.LoadDataSync();
        //    return model;
        //}




        //public async Task Actualiser()
        //{
        //    //t = new Thread(new ThreadStart(FilterThread));
        //    // SelectedCOunt = 20;
        //    NameSearch = "";
        //    PageNumber = 1;  
        //        await LoadData();
        //       //  NotifyOfPropertyChange("SelectedCOunt");
        //        NotifyOfPropertyChange("PageNumber");
        //    MessageQueue.Enqueue("Données actualisées");
        //}

        //public async void Add()
        //{
        //    try
        //    {
        //        selected = (ExtendedDocument)Activator.CreateInstance(type);


        //        if (selected.DocOpenMod == OpenMode.Attach)
        //        {
        //            selected.CollectionName = displayName;
        //            shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"{this.DisplayName} | {selected.Name}");
        //        }
        //        else
        //        {
        //            var ioc = DataHelpers.container;
        //            var vm = ioc.Get<ViewManager>();
        //            var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, shell);

        //            var content = vm.CreateAndBindViewForModelIfNecessary(c);

        //            var cc = new ContentControl();
        //            cc.HorizontalAlignment = HorizontalAlignment.Stretch;
        //            cc.VerticalAlignment = VerticalAlignment.Stretch;
        //            cc.Content = content;

        //            GenericWindowViewModel gw = new GenericWindowViewModel(cc, displayName, selected.Name);
        //            windowManager.ShowWindow(gw);
        //        }

        //    }
        //    catch (Exception s)
        //    {
        //        MessageBox.Show(s.Message);
        //        return;
        //    }
        //}

        public void CloseWindows()
        {
            try
            {
                this.RequestClose();
            }
            catch
            {
                //MessageBox.Show(this.View.GetParentObject().TryFindParent<Window>().GetType().ToString());
                this.View.GetParentObject().TryFindParent<Window>().Close();
            }
        }





        //public void Dispose()
        //{

        //}


        public   void SelectAll()
        { 
                foreach (ExtendedDocument item in Items)
                {
                    item.IsSelectedd = !item.IsSelectedd;
                }
                NotifyOfPropertyChange("Items");
           

        }
        public async void doFiltrer()
        {
            var allData = await datahelper.GetMongoDataAll<T>();
            var filtre = new FiltreViewModel<T>();
            DataHelpers.container.Get<ViewManager>().BindViewToModel(new FiltreView(), filtre);
            filtre.SetInputs(allData);
            var restulDialog = windowManager.ShowDialog(filtre);
            if (restulDialog == true)
            {
                PAGE_MODE = PAGE_MODES.FILTER_BOX;
                SearchResul = filtre.Result.ConvertAll<T>(a => (T)a);
               
                //   SelectedCOunt = Items.Count;
                //  ItemsSource = new PagingCollectionView(Items, SelectedCOunt);

                await NextPage();
            }
        }

        public List<T> SearchResul { get; set; } = new List<T>();
        //public void ExporterPDF()
        //{
        //    /*
        //     * /*/
        //    windowManager.ShowWindow(new PrintWindowViewModel(Items));

        //}


        //public void ExportTemplate()
        //{

        //    var dialog = new SaveFileDialog();
        //    var file = dialog.ShowDialog();
        //    if (file == true)
        //    {
        //        var result = dialog.FileName;
        //        var dp = new DynamicPath(result);
        //        var ovimport = new ExcelImport(dp);

        //        ovimport.ExportTemplate(result, type);
        //        Process.Start(result);
        //    }
        //}

        //public async void ImportData()
        //{
        //    try
        //    {

        //        var dialog = new OpenFileDialog();
        //        var file = dialog.ShowDialog();
        //        if (file == true)
        //        {
        //            var result = dialog.FileName;
        //            var ovimport = new ExcelImport(new DynamicPath(result));

        //            var data = ovimport.ImportDataFromType(result, type);

        //            if (data != null)
        //            {
        //                var count = data.Count;
        //                var confirmation = MessageBox.Show($"{count} documents trouvés, voulez-vous continuer!", "Confirmation", MessageBoxButton.YesNo);
        //                if (confirmation == MessageBoxResult.Yes)
        //                {

        //                    foreach (var item in data)
        //                    {
        //                        item.AddedAtUtc = DateTime.Now;
        //                        item.isLocal = false;
        //                        // item.Save();
        //                    }
        //                    var mi = typeof(BaseMongoRepository).GetMethod("AddMany");
        //                    var gen = mi.MakeGenericMethod(type);
        //                    gen.Invoke(DS.db, new object[] { data });
        //                    //  DS.db.AddMany<t>(data);
        //                    MessageBox.Show("Terminé");
        //                    await Actualiser();
        //                }

        //            }
        //            else
        //            {
        //                MessageBox.Show("Aucun document trouvé");
        //                return;
        //            }


        //        }

        //    }
        //    catch (Exception s)
        //    {
        //        MessageBox.Show(s.Message);
        //        return;
        //    }
        //}

        //public async void Handle(ModelChangeEvent message)
        //{
        //    await Actualiser();
        //    //new Thread(() =>
        //    //{
        //    //    Thread.CurrentThread.IsBackground = true;
        //    //     Actualiser().Wait();
        //    //}).Start(  ) ;
        //}

        //private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    Console.WriteLine("Finish");
        //}

        //private  async void Bg_DoWork(object sender, DoWorkEventArgs e)
        //{
        //     await Actualiser();
        //}
        //public DataHelpers datahelper { get; set; } = new DataHelpers();


        //public BindableCollection<dynamic> TABLE_DATA { get; set; }
        //private async Task LoadData<T>(int _PageNumber, int _SelectedCOunt) where T : ExtendedDocument, new()
        //{

        //    // if(!fromFiltre)
        //    // list = await DataHelpers.GetMongoData(type.Name);
        //    // datahelper
        //    var result =  await datahelper.GetMongoDataPaged<T>(type, _PageNumber, _SelectedCOunt);
        //    TABLE_DATA = new BindableCollection<dynamic>();
        //    TABLE_DATA.AddRange(result.ConvertAll<ExtendedDocument>(a => a));
        //    TABLE_DATA.Refresh();
        //    // FinalResults = new BindableCollection<dynamic>(t);
        //    //   TABLE_DATA = new ObservableCollection<dynamic>(r.Cast<ExtendedDocument>());
        //    NotifyOfPropertyChange("TABLE_DATA");
        //    //Items = new List<IDocument>();
        //    //await  LoadDataThread(list); 



        //}
        //public async Task LoadData()
        //{

        //        if(!fromFiltre)
        //         list = await DataHelpers.GetMongoData(type.Name);
        //       // datahelper
        //       // list = await DataHelpers.GetMongoDataSync(type, PageNumber, SelectedCOunt);
        //        FinalResults = new BindableCollection<dynamic>(list);
        //        NotifyOfPropertyChange("FinalResults");
        //    Items = new List<IDocument>();
        //    await LoadDataThread(list);



        //}

        //public void LoadDataSync()
        //{

        //        //if (!fromFiltre)
        //        //    list = DataHelpers.GetMongoDataSync(type.Name);
        //        //Items = new List<IDocument>();


        //        //    var tLoadData = new Thread(() => LoadDataThread(list).Wait());
        //        //tLoadData.Start();

        //}

        //public async Task LoadData(IEnumerable<IDocument> _list)
        //{

        //    if (list != null)
        //    {
        //        await Task.Run(() => list = _list.OrderByDescending(a => a?.AddedAtUtc));
        //    }
        //    await LoadDataThread(list);
        //    //var tLoadData = new Thread(() => LoadDataThread(list));
        //    //tLoadData.Start();

        //}

        //public async Task LoadDataThread(IEnumerable<IDocument> _list)
        //{
        //    StatusLabel = "Collection des données..."; NotifyOfPropertyChange("StatusLabel");
        //    Items = _list.ToList();
        //    // Items = _list.OrderByDescending(a => a.AddedAtUtc).ToList();
        //    //  Original = Items;

        //    //ItemsSource = new PagingCollectionView(Items, SelectedCOunt);
        //    await Setup();

        //    // NotifyOfPropertyChange("Original");
        //    await GetPages();
        //    StatusLabel = "Terminé"; NotifyOfPropertyChange("StatusLabel");

        //}


        //public async void nextPage()
        //{
        //    PageNumber++;
        //    // await GetPages();
        //    await Next();
        //    NotifyOfPropertyChange("PageNumber");
        //    NotifyOfPropertyChange("CurrentPage");

        //    //pageNumber++;
        //    //if ((pageNumber * SelectedCOunt) > Items.Count)
        //    //{
        //    //    pageNumber--;
        //    //}

        //    // NotifyOfPropertyChange("Results");
        //}

        //public async void ouvrirItem()
        //{
        //    try
        //    {
        //        if (selected != null)
        //        {
        //            // Show for select item  only
        //            if (ForSelectOnly)
        //            {
        //                ForSelectOnly = false;
        //                CloseWindows();
        //            }
        //            else
        //            {

        //                if (selected.DocOpenMod == OpenMode.Attach)
        //                {
        //                    selected.CollectionName = displayName;
        //                    shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"{selected.CollectionName} - {selected.Name}");
        //                }
        //                else
        //                {
        //                    var ioc = DataHelpers.container;
        //                    var vm = ioc.Get<ViewManager>();
        //                    var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, shell);
        //                    c.DisplayName = selected.CollectionName;
        //                    var content = vm.CreateAndBindViewForModelIfNecessary(c);

        //                    var cc = new ContentControl();
        //                    cc.HorizontalAlignment = HorizontalAlignment.Stretch;
        //                    cc.VerticalAlignment = VerticalAlignment.Stretch;
        //                    cc.Content = content;

        //                    GenericWindowViewModel gw = new GenericWindowViewModel(cc, displayName, selected.Name);
        //                    windowManager.ShowWindow(gw);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception s)
        //    {
        //        MessageQueue.Enqueue(s.Message);
        //        return;
        //    }
        //}

        //public async void prevPage()
        //{
        //    PageNumber--;
        //    if (PageNumber <= 0)
        //    {
        //        PageNumber = 1;
        //        return;
        //    }
        //    // await GetPages();
        //    await Next();
        //    NotifyOfPropertyChange("PageNumber");
        //    NotifyOfPropertyChange("CurrentPage");
        //}
        //Thread t ;
        public async void SearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try { await FilterThread(); }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                    return;
                }
            }
        }

        ////private async Task doFindByName()
        ////{
        ////     if(t == null)
        ////        t = new Thread(new ThreadStart(FilterThread));

        ////    var lengh = NameSearch.Length;
        ////    if (lengh == 0)
        ////    {
        ////        Actualiser();
        ////        return;
        ////    }

        ////    if(lengh < originalSearchLengh)
        ////    {
        ////         Actualiser();
        ////    }
        ////    if (t.ThreadState == ThreadState.Stopped || t.ThreadState == ThreadState.Unstarted)
        ////    {
        ////        t = new Thread(new ThreadStart(FilterThread));
        ////        t.Start();
        ////    }
        ////    else
        ////    {
        ////        //Actualiser();
        ////        StatusLabel = "Recherche encours...";
        ////        NotifyOfPropertyChange("StatusLabel");
        ////    }
        ////    originalSearchLengh = lengh;

        ////}

        //int originalSearchLengh = 0;
        //private static MethodInfo methodInfo;
        //private static MethodInfo genericCreat;
        //private static object[] genericparam;


        public enum PAGE_MODES
        {
            ALL,
            LIST,
            FILTER_TEXT,
            FILTER_BOX
        }
        public PAGE_MODES PAGE_MODE { get; set; } = PAGE_MODES.ALL;

        public async Task FilterThread()
        { 

            StatusLabel = "Recherche en cours...";
            NotifyOfPropertyChange("StatusLabel");
            PAGE_MODE = PAGE_MODES.FILTER_TEXT;
            PageNumber = 1;

            await NextPage();
            StatusLabel = "Terminé";
            NotifyOfPropertyChange("StatusLabel");
        }

        //private async Task GetPages()
        //{
        //   // FinalResults.Clear();
        //    await LoadPageItems();
        //    // tGetPages = new Thread(new ThreadStart(GetPagesThread));
        //    //tGetPages.Start();
        //    //       this.ClearAllPropertyErrors();

        //    // FinalResults.Refresh();
        //}





        //public async void DeleteAll()
        //{
        //    var selected = FinalResults.Where(a => a.IsSelectedd).ToList();
        //    if(selected != null && selected.Any())
        //    {
        //        foreach (IModel item in selected)
        //        {
        //            if (!item.Delete())
        //                break;
        //        }
        //       await Actualiser();
        //    }


        //}

        //public async void ValidateAll()
        //{
        //    var selected = FinalResults.Where(a => a.IsSelectedd);
        //    if (selected != null && selected.Any())
        //    {
        //        foreach (IModel item in selected)
        //        {
        //            try
        //            {
        //                item.Submit();
        //            }
        //            catch (Exception s)
        //            {
        //                MessageBox.Show($"{s.Message}\n{((ExtendedDocument)item).Name}");
        //                continue;
        //            }

        //        }
        //        await Actualiser();
        //    }
        //}

        //private async Task LoadPageItems()
        //{

        //    StatusLabel = "Patientez svp..."; NotifyOfPropertyChange("StatusLabel");
        //    var values = Items.ToPagedList<IDocument>(PageNumber, SelectedCOunt);

        //    FinalResults = new BindableCollection<dynamic>(values);
        //    NotifyOfPropertyChange("FinalResults");
        //    // FinalResults.AddRange(values);
        //    var ecahntient = FinalResults.FirstOrDefault();
        //    if(ecahntient != null)
        //    {
        //        try
        //        {

        //            //Type t = (ecahntient.GetType());
        //            //if (t.Equals(typeof(Prestation)))
        //            //{
        //            //    var list = FinalResults.ToList();
        //            //    var toList = list.ToList();
        //            //    double total = (double)list.Sum(a => (double)a.MontantTotal);
        //            //    double totalpaye = (double)list.Sum(a => (double)a.MontantPaye);
        //            //    FinalResults.Add(new Prestation()
        //            //    {
        //            //        Name = "TOTAL",
        //            //        MontantTotal = total,
        //            //        MontantPaye = totalpaye
        //            //    });
        //            //}
        //            //if (t.Equals(typeof(EcritureCompte)))
        //            //{
        //            //    var list = FinalResults.ToList();
        //            //    var toList = list.ToList();
        //            //    double nDebit = (double)list.Sum(a => (double)a.nDebit);
        //            //    double nCredit = (double)list.Sum(a => (double)a.nCredit);
        //            //    FinalResults.Add(new EcritureCompte()
        //            //    {
        //            //        Name = "TOTAL",
        //            //        nDebit = nDebit,
        //            //        nCredit = nCredit
        //            //    });
        //            //}
        //            //if (t.Equals(typeof(CompteResultatReport)))
        //            //{
        //            //    var list = FinalResults.ToList();
        //            //    var toList = list.ToList();
        //            //    double nDebit = (double)list.Sum(a => (double)a.Montant);
        //            //    FinalResults.Add(new CompteResultatReport()
        //            //    {
        //            //        Name = "TOTAL",
        //            //        Montant = nDebit,

        //            //    });
        //            //}

        //        }
        //        catch (Exception s)
        //        {
        //            StatusLabel = s.Message;
        //        }
        //    }

        //    // FinalResults.Refresh();
        //    // NotifyOfPropertyChange("FinalResults");

        //    ElementsCount = $"{Items.Count} élement(s)";
        //    NotifyOfPropertyChange("ElementsCount");
        //    StatusLabel = "Terminé"; NotifyOfPropertyChange("StatusLabel");
        //}

        //private async Task Setup()
        //{
        //    var typeName = type.Name.ToLower();
        //    if (typeName.ToLower().Contains("report"))
        //    {
        //        ShowButtonAjouter = Visibility.Hidden;
        //        NotifyOfPropertyChange("ShowButtonAjouter");
        //    }
        //}

        //private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //  NotifyOfPropertyChange("Search");
        //}

        public void UserControl_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.F1)
            {
                Add();
            }
            else if (args.Key == Key.F2)
            {
                CloseWindows();
            }
        }
        #endregion


        public void Dispose()
        {

        }

        public void Handle(ModelChangeEvent message)
        {
            if (message.type == this.type)
            {
                Action t = async () => 
                {
                    await Actualiser();
                };

                t();
            }
        }
    }
}