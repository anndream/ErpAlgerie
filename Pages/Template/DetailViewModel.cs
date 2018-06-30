using AttributtedDataColumn;
using ErpAlgerie.Modules.Core;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Pages.Events;
using ErpAlgerie.Pages.Helpers;
using ErpAlgerie.Pages.PopupWait;
using FontAwesome.WPF;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDbGenericRepository.Models;
using Stylet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ErpAlgerie.Pages.Template
{
    public class DetailViewModel : Screen, IDisposable
    {
        public StackPanel DocImageContent { get; set; }

        private IEventAggregator aggre;
        public Visibility LinksVisible { get; set; } = Visibility.Collapsed;

            private Button btnAddModel;
        private bool isFreezed;
        private UniformGrid masterWrap;
       // private Binding myBinding; 
        private Type type;
        protected DetailViewModel(IDocument model, Type t, IEventAggregator _aggre, IShell shell)
        {

            (model as ExtendedDocument).CloseEvent += DetailViewModel_CloseEvent;
            // Change culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-DZ");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-DZ");

            if (!FrameworkManager.CanView(t))
            {
                throw new Exception("Vous n'etes pas autorise a visualiser ce document");
            }

          

            this.shell = shell;
            aggre = _aggre;
            this.model = model; type = t;
            try { imgBg = System.IO.Path.GetFullPath(DataHelpers.imgBg); } catch { }

            //this.PropertyChanged += DetailViewModel_PropertyChanged;
            //StateChanged += DetailViewModel_StateChanged;


            borderSeparation = new Border();
            borderSeparation.Background = App.Current.FindResource("MaterialDesignDivider") as Brush;
            borderSeparation.Height = 1;
            borderSeparation.HorizontalAlignment = HorizontalAlignment.Stretch;
            borderSeparation.SnapsToDevicePixels = true;

            SetupDocCards();
        }

        private void DetailViewModel_CloseEvent(object sender, EventArgs e)
        {
            Close();
        }

        private void SetupDocCards()
        {

            //SETUP CARD ONE

            if ((model as ExtendedDocument)?.DocCardOne != null)
            {
                DocCardOne = new Card();

                var carOneContent = new StackPanel();
                carOneContent.Orientation = Orientation.Horizontal;

                // icon
                PackIcon pi = new PackIcon();
                try
                {
                    pi.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), (model as ExtendedDocument).DocCardOne.BulletIcon);
                }
                catch (Exception s)
                {
                    DataHelpers.Logger.LogError(s.Message);
                    pi.Kind = PackIconKind.Exclamation;
                }
                pi.Width = 28;
                pi.HorizontalAlignment = HorizontalAlignment.Center;
                pi.Height = 28;

                pi.HorizontalContentAlignment = HorizontalAlignment.Center;
                pi.VerticalAlignment = VerticalAlignment.Center;
                pi.VerticalContentAlignment = VerticalAlignment.Center;
                carOneContent.HorizontalAlignment = HorizontalAlignment.Center;

                carOneContent.Children.Add(pi);

                var st = new StackPanel();
                carOneContent.VerticalAlignment = VerticalAlignment.Center;
                st.Orientation = Orientation.Vertical;
                st.Children.Add(new TextBlock() { Text = (model as ExtendedDocument).DocCardOne.BulletValue, FontSize = 14 });
                st.Children.Add(new TextBlock() { Text = (model as ExtendedDocument).DocCardOne.BulletTitle, FontSize = 10 });
                st.Margin = new Thickness(8, 2, 8, 2);
                carOneContent.Children.Add(st);
                carOneContent.Background = Brushes.White;
                DocCardOne.Content = carOneContent;
                 ShadowAssist.SetShadowDepth(DocCardOne, ShadowDepth.Depth0);
                ShadowAssist.SetShadowEdges(DocCardOne, ShadowEdges.Left);
                DocCardOne.Padding = new Thickness(5);
                NotifyOfPropertyChange("DocCardOne");
            }


            // Card Tow
            //SETUP CARD ONE

            if ((model as ExtendedDocument)?.DocCardTow != null)
            {
                DocCardTow = new Card();
                var carTowContent = new StackPanel();
                carTowContent.Orientation = Orientation.Horizontal;

                // icon
                PackIcon pi = new PackIcon();
                try
                {
                    pi.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), (model as ExtendedDocument).DocCardTow.BulletIcon);
                }
                catch (Exception s)
                {
                    DataHelpers.Logger.LogError(s.Message);
                    pi.Kind = PackIconKind.Exclamation;
                }
                pi.Width = 28;
                pi.HorizontalAlignment = HorizontalAlignment.Center;
                pi.Height = 28;

                pi.HorizontalContentAlignment = HorizontalAlignment.Center;
                pi.VerticalAlignment = VerticalAlignment.Center;
                pi.VerticalContentAlignment = VerticalAlignment.Center;
                carTowContent.HorizontalAlignment = HorizontalAlignment.Center;

                carTowContent.Children.Add(pi);

                var st = new StackPanel();
                st.Margin = new Thickness(8, 2, 8, 2);
                carTowContent.VerticalAlignment = VerticalAlignment.Center;
                st.Orientation = Orientation.Vertical;
                st.Children.Add(new TextBlock() { Text = (model as ExtendedDocument).DocCardTow.BulletValue, FontSize = 14 });
                st.Children.Add(new TextBlock() { Text = (model as ExtendedDocument).DocCardTow.BulletTitle, FontSize = 10 });
                carTowContent.Children.Add(st);
                carTowContent.Background = Brushes.White;
                DocCardTow.Content = carTowContent;
                ShadowAssist.SetShadowDepth(DocCardTow, ShadowDepth.Depth0);
                ShadowAssist.SetShadowEdges(DocCardTow, ShadowEdges.Left);
                DocCardTow.Padding = new Thickness(5);
                NotifyOfPropertyChange("DocCardTow");
            }

        }
        Border borderSeparation;
        private Thread notifyBaseThread;

        private bool _CollapseAll = true;

        public bool CollapseAll
        {
            get { return _CollapseAll; }
            set { _CollapseAll = value;

                Action reload = async () =>
                {
                    await Setup();
                };

                reload();
        }
        }


        public string btnColor { get; set; } = "#2196F3";
        public string btnName { get; set; } = "Enregistrer";
        public bool CloseAfter { get; set; } = false;
        public string CollectionTitle
        {
            get
            {
                return (model as ExtendedDocument).CollectionName;
            }
        }

        public double CurrentScrollPosition { get; private set; }
        // "#27a8f7";
        public string DocStatus
        {
            get
            {
                return model.isLocal == true ? "Nouveau" : "Enregistré";
            }
        }

        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
        public double elementWidth { get; set; } = 350;
        public bool fermerVisible { get; set; } = true;
        public string GlobalMsg { get; set; } = "Nouveau(elle)";
        public string imgBg { get; set; }
        public WrapPanel linkButtons { get; set; }
        public StackPanel linkButtonsOps { get; set; }
        public Card DocCardOne { get; set; }
        public Card DocCardTow { get; set; }

        //  public ScrollViewer masterStackContent { get; set; }
        public dynamic model { get; set; }
        public WrapPanel opeartionButtons { get; set; }
        public List<string> propertiesOfDoc
        {
            get
            {
                return (model as ExtendedDocument).GetType().GetProperties().Select(z => z.Name).ToList();
            }
        }

        public int ScrollPosition { get; set; } = 0;
        public IShell shell { get; set; }
        public StackPanel stackContent { get; set; }
        public ComboBox Tablebox { get; private set; }
        public ObjectId? tableModel { get; set; } = ObjectId.Empty;

        public ObjectId? tableModelWeak { get; set; } = ObjectId.Empty;
        
        public static async Task<DetailViewModel> Create(IDocument model, Type t, IEventAggregator _aggre, IShell shell)
        {
            DetailViewModel modelBase = new DetailViewModel(model, t, _aggre, shell);
            await modelBase.Setup();

            return modelBase;
        }

        public static DetailViewModel CreateSync(IDocument model, Type t, IEventAggregator _aggre, IShell shell)
        {
            DetailViewModel modelBase = new DetailViewModel(model, t, _aggre, shell);
            var s = modelBase.Setup();
            s.Wait();
            return modelBase;
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            Console.WriteLine();
            base.RequestClose(dialogResult);
        }
        public void Close()
        {
            // Cancel new
            if((model as ExtendedDocument).isLocal)
            {
                var response = MessageBox.Show("Ignorer les modifications?", "Confirmation", MessageBoxButton.YesNo);
                if (response == MessageBoxResult.No)
                {
                    return;
                }
                
            }


            var original = DataHelpers.GetById(type,model.Id) ;
            if(original != null && (model as ExtendedDocument).DocStatus != 1)
            {
                var ins = (original as IModel);
                var fields = (ins.GetType().GetProperties());
                foreach (var item in fields)
                {
                    if (item.Name == "AddedAtUtc"
                        || item.Name == "EditedAtUtc"
                         || item.Name == "MyModule"
                        || item.Name == "CollectionName" || item.Name == "Index")
                        continue;

                    if (item.GetValue(model)?.ToString() != item.GetValue(original)?.ToString())
                    {
                        var response = MessageBox.Show("Ignorer les modifications?","Confirmation",MessageBoxButton.YesNo);
                        if(response == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
 
                }
            }


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
        public async void CreerNouveau()
        {
            var selected = (ExtendedDocument)Activator.CreateInstance(type);
            shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"Nouveau(elle) {selected.CollectionName}");
        }

        public async void Dupliquer()
        {
            var selected = (ExtendedDocument)Activator.CreateInstance(type);

            var parentProperties = model.GetType().GetProperties();
           // var childProperties = child.GetType().GetProperties();

            foreach (PropertyInfo parentProperty in parentProperties)
            {
                if (parentProperty.CanWrite)
                {
                    try
                    {
                        parentProperty.SetValue(selected, parentProperty.GetValue(model));
                        selected.isLocal = true;
                        selected.DocStatus = 0;
                        selected.Id = ObjectId.Empty;
                    }
                    catch (Exception s)
                    {
                        DataHelpers.Logger.LogError(s);
                        MessageBox.Show(s.Message);                        
                    }
                }
            }

            
            shell.OpenScreen(await DetailViewModel.Create(selected, selected.GetType(), aggre, shell), $"Nouveau(elle) {selected.CollectionName}");

        }

        public void Delete()
        {
            try
            {
                var origin = model as ExtendedDocument;
                if (origin.Submitable && origin.DocStatus == 0 && origin.isLocal == false)
                {
                   if( (model as IModel).Delete())
                        shell.CloseScreen(this);
                }
                else if (origin.isLocal == true)
                {
                    throw new Exception("Vous ne pouvez pas supprimer un document non enregistré");
                }
                else if (origin.Submitable && origin.DocStatus == 1 && origin.isLocal == false)
                {
                    throw new Exception("Vous devez annuler le document avant de le supprimer");
                }
                else
                {
                   if( (model as IModel).Delete())
                        shell.CloseScreen(this);
                }

                
                var filtre = new PopupWaitViewModel("Veuillez patienter...");
                var restulDialog = DataHelpers.windowManager.ShowDialog(filtre);

                this.aggre.Publish(new ModelChangeEvent(this.model.GetType()));
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }

        public void Dispose()
        {
        }

        public async void Notify(Object source)
        {
            if (FinishLoaded)
            { 
                await Setup();
               
            }
        }

        public async void Save()
        {
            try
            { 
                var origin = model as ExtendedDocument;
                if (origin.Submitable && origin.DocStatus == 0 && origin.isLocal == false)
                {
                    (model as IModel).Submit();
                    //await Setup();
                }
                else if (origin.Submitable && origin.DocStatus == 1 && origin.isLocal == false)
                {
                    var confirmation = MessageBox.Show("Si vous annuler, vous devez modifier les changements effectués", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (confirmation == MessageBoxResult.OK)
                    {
                        (model as IModel).Cancel();
                        await Setup();
                        return;
                    }
                }
                else
                {
                    (model as IModel).Save();
                   // await Setup();
                }

                //notifyBaseThread = new Thread(new ThreadStart(() =>
                //{
                    this.aggre.Publish(new ModelChangeEvent(this.model.GetType()));
                //}));
                //notifyBaseThread.Priority = ThreadPriority.Lowest;

                //notifyBaseThread.Start();


                await Actualiser();
                //  MessageBox.Show("Modifications enregistré");
                MessageQueue.Enqueue("Document enregistré");
                //var filtred = new PopupWaitViewModel("Veuillez patienter...");
                //var restulDialog = DataHelpers.windowManager.ShowDialog(filtred);
                NotifyOfPropertyChange("btnName");
                NotifyOfPropertyChange("btnColor");

                if (CloseAfter)
                {
                    Close();
                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
        }

        public void UserControl_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.F1)
            {
                Save();
            }
            else if(args.Key == Key.F2)
            {
                Close();
            }
        }

        protected override void OnClose()
        {
            //var _model = (model as ExtendedDocument);
            //if (_model.Id != ObjectId.Empty && !_model.isLocal  && ((_model.Submitable && _model.DocStatus == 0) || !_model.Submitable))
            //{
            //        // saved
            //        var e = DataHelpers.GetById(_model.GetType().Name, _model.Id);
            //        if (!e.Equals(_model))
            //        {
            //            var res = MessageBox.Show("Vos modifications ne sont enregistrées, voulez-vous quitter?", "Modifications non enregistrées", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //            if (res == MessageBoxResult.No)
            //                return;

            //        }

            //}

            base.OnClose();
        }

        private async void Actualiser_Click(object sender, RoutedEventArgs e)
        {
            await Actualiser();
        }

        public async Task Actualiser()
        {
                //FinishLoaded = true;
             
                NotifyOfPropertyChange("model");
                await Setup();
                SetupDocCards();


        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            // add iamge
            var Nmodel = (ExtendedDocument)model;
            var propName = (sender as Button).Tag.ToString();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Selectionner une image";
            ofd.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphics (*.png)|*.png";
            BitmapImage img;
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    var file = System.IO.Path.GetFullPath(ofd.FileName);
                    var imageLink = $"img_data/{propName}_{DateTime.Now.Millisecond}{System.IO.Path.GetExtension(ofd.FileName)}";
                    File.Copy(file, imageLink, true);
                    img = new BitmapImage(new Uri(System.IO.Path.GetFullPath(imageLink)));

                    Nmodel.GetType().GetProperty(propName).SetValue(model, imageLink);
                    NotifyOfPropertyChange("model");
                    Setup();
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
            }
        }

        // Table add button
        private void AddBtnTable_Click(object sender, RoutedEventArgs e)
        {
            var Nmodel = (ExtendedDocument)model;

            // Model name (RepasSimple)
            var modelType = (sender as Button).Tag as Type;

            var s = Activator.CreateInstance(modelType) as IModel;
            var propertyName = modelType.Name;
            var initValues = Nmodel.GetType().GetProperty(propertyName).GetValue(model);
            // initValues.Add(s);

            Type genericListType = typeof(List<>).MakeGenericType(type);
            var newValues = (IList)Activator.CreateInstance(genericListType);
            newValues = initValues;

            newValues.Add(s);
            Nmodel.GetType().GetProperty(propertyName).SetValue(model, newValues);
            NotifyOfPropertyChange($"model");
        }

        private async void Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Setup();   
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (sender as Button);
                if (btn.Tag == null)
                    return;
                var data = (btn.Tag as ArrayList);
                var type = Type.GetType(data[0].ToString());
                var selected = (ExtendedDocument)Activator.CreateInstance(type);                 
                await DataHelpers.Shell.OpenScreenDetach(selected, selected.Name);
                (model as ExtendedDocument).GetType().GetProperty(data[1].ToString()).SetValue(model, selected.Id);
                NotifyOfPropertyChange("model"); 
                await Setup();
            }
            catch (Exception s)
            {


                MessageQueue.Enqueue(s.Message);
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
                return;
            }
        }

        private async void BtnAddLien_Click(object sender, RoutedEventArgs e)
        {
            var Nmodel = (ExtendedDocument)model;
            if (Nmodel != null)
            {
                try
                { // header[0] -> lClient
                    // header[1] -> Facture
                    // header = Facture
                    // property name is the link

                    var item = (Button)sender;
                    var header = (ArrayList)item.Tag;

                    var property = header[0].ToString(); // lClient
                    var link = header[1].ToString(); // Facture

                    var type = Type.GetType($"ErpAlgerie.Modules.CRM.{link}");
                    var selected = (ExtendedDocument)Activator.CreateInstance(type);
                    
                    // selected is facture in our exampel

                    selected.GetType().GetProperty(property).SetValue(selected, model.Id);

                    selected = DataHelpers.MapProperties(selected, model);

                  

                    if (selected.DocOpenMod == OpenMode.Detach)
                    {
                        await DataHelpers.Shell.OpenScreenDetach(selected, selected.CollectionName);
                    }
                    else
                    {
                        DataHelpers.Shell.OpenScreenAttach(selected, selected.CollectionName);
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                    return;
                }
            }
        }

        private async void Btnaddline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = (sender as Button).Tag as DataGrid;
                var selected = s.GetValue(DataGrid.ItemsSourceProperty);
                (selected as IList).Clear();
            }
            catch (Exception s)
            {

                MessageQueue.Enqueue(s.Message);
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
            }
            await Setup();
        }

        private async void BtnAddline_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    var s = (sender as Button).Tag as DataGrid;
            //    var selected = s.GetValue(DataGrid.ItemsSourceProperty);
            //    s.GetValue(DataGrid.);
            //    var clone = (selected as IList)[0].;
            //    (selected as IList).Add(clone);
            //}
            //catch (Exception s)
            //{
            //    MessageBox.Show(s.Message, "Erreur");
            //}
            //await Setup();
        }

        private async void BtnAddModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dic = (ArrayList)(sender as Button).Tag;
                if (tableModel == ObjectId.Empty || dic.Count < 3)
                {
                    MessageBox.Show("Selectionner une ligne à ajoutée, ou vérifier la déclaration");
                    return;
                }

                var data = dic;
                var s = data[0] as DataGrid;

               await AddItemToTable(s, data[1].ToString(), data[2].ToString());
               // var selected = s.GetValue(DataGrid.ItemsSourceProperty);

               // var doc = DataHelpers.GetById(data[1].ToString(), tableModel);
               // var mapped = doc.Map(data[2].ToString());

               // (selected as IList).Add(mapped);

               //// s.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
                await Setup();
            }
            catch (Exception s)
            {

                MessageQueue.Enqueue(s.Message);
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
                return;
            }
        }


        private async Task AddItemToTable(DataGrid table,string source,string mapFunction)
        {
            
                var afterMap = (table.Tag as AfterMapMethodAttribute)?.MethodName;
                var selected = table.GetValue(DataGrid.ItemsSourceProperty);

            var doc = DataHelpers.GetById(source,tableModel);
           // DataHelpers.GetById(source, tableModel);
                var mapped = doc.Map(mapFunction);

                if (afterMap != null)
                    mapped = (model as ExtendedDocument).GetType().GetMethod(afterMap)?.Invoke(model, new[] { mapped });


                (selected as IList).Add(mapped);
           
        }


        private async Task AddItemToTable(DataGrid table, ExtendedDocument item, string mapFunction)
        {

            var afterMap = (table.Tag as AfterMapMethodAttribute)?.MethodName;
            var selected = table.GetValue(DataGrid.ItemsSourceProperty);

            var doc = item;
            var mapped = doc.Map(mapFunction);

            if (afterMap != null)
            {
                try
                {
                    mapped = (model as ExtendedDocument).GetType().GetMethod(afterMap).Invoke(model, new[] { mapped });

                }
                catch (Exception s)
                {
                    MessageQueue.Enqueue(s.Message);
                    DataHelpers.Logger.LogError(s)  ;
                }
            }

            (selected as IList).Add(mapped);

        }
        private async void BtnNewModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (sender as Button);
                if (btn.Tag == null)
                    return;
                var data = (btn.Tag as ArrayList);
                var type = Type.GetType(data[0].ToString());
                var selected = (ExtendedDocument)Activator.CreateInstance(type);

                await DataHelpers.Shell.OpenScreenDetach(selected, selected.Name);


                //var ioc = DataHelpers.container;
                //var vm = ioc.Get<ViewManager>();
                //var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, DataHelpers.Shell);

                //c.DisplayName = selected.CollectionName;
                //var content = vm.CreateAndBindViewForModelIfNecessary(c);

                //var cc = new ContentControl();
                //cc.HorizontalAlignment = HorizontalAlignment.Stretch;
                //cc.VerticalAlignment = VerticalAlignment.Stretch;
                //cc.Content = content;

                //GenericWindowViewModel gw = new GenericWindowViewModel(cc, c.DisplayName, selected.Name);
                //DataHelpers.windowManager.ShowDialog(gw);
                
                tableModel = selected.Id;
                await Setup();



                //


                //var btn = (sender as Button);
                //if (btn.Tag == null)
                //    return;
                //var data = (btn.Tag as ArrayList);
                //var type = Type.GetType(data[0].ToString());
                //var selected = (ExtendedDocument)Activator.CreateInstance(type);
                //await DataHelpers.Shell.OpenScreenDetach(selected, selected.Name);
                //(model as ExtendedDocument).GetType().GetProperty(data[1].ToString()).SetValue(model, selected.Id);
                //NotifyOfPropertyChange("model");
                //await Setup();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
                return;
            }
        }

        private async void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dic = (ArrayList)(sender as Button).Tag;
                if (dic.Count < 3)
                {
                    MessageBox.Show("Selectionner une ligne à ajoutée, ou vérifier la déclaration");
                    return;
                }

                var data = dic;
                var s = data[0] as DataGrid;
                var selected = s.GetValue(DataGrid.ItemsSourceProperty);

                var type = Type.GetType(data[1].ToString());
                var doc = DataHelpers.Shell.OpenScreenFind(type, $"Selectioner...");

                if(doc != null)
                {
                    var list = doc as IEnumerable<ExtendedDocument>;
                    foreach (var item in list)
                    {
                    //    var mapped = item.Map(data[2].ToString());
                    //    (selected as IList).Add(mapped);
                        await AddItemToTable(s, item, data[2].ToString());
                        
                    }


                    await Setup();
                }
               
            }
            catch (Exception s)
            {
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
                return;
            }

            //

            //
        }

        private async void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (btn.Tag == null)
                return;

            var combo = (ComboBox)btn.Tag;
            var selected = (ExtendedDocument)combo.SelectedItem;

            if (selected != null)
            {
                shell.OpenScreenDetach(selected, $"{selected.Name}");
                await Setup();
            }
        }

        private void BtnViewDate_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if (btn.Tag == null)
                return;

            var combo = (DatePicker)btn.Tag;

            if (combo != null)
            {
                combo.SelectedDate = DateTime.Now;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private async void DatePicker_SelectedDateChanged1(object sender, TimePickerBaseSelectionChangedEventArgs<DateTime?> e)
        {
            await Setup();
        }

        private void DatePicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //var result = MessageBox.Show("Voulez-vous supprimer le document?", "Voulez-vous supprimer le document?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if(result == MessageBoxResult.Yes)
            Delete();
        }

        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = (sender as MenuItem).Tag as DataGrid;
                var selected = s.GetValue(DataGrid.ItemsSourceProperty);
                (selected as IList).Clear();
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message, "Erreur");
            }
            await Setup();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = (sender as MenuItem).Tag as DataGrid;
                var selected = s.GetValue(DataGrid.ItemsSourceProperty);
                (selected as IList).Remove(s.SelectedItem);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message, "Erreur");
            }
            await Setup();
        }

        private void DetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "select" || e.PropertyName == "date")
            //{
            //    NotifyOfPropertyChange("model");
            //}
            //Console.WriteLine(e.PropertyName);
        }

        private async void DetailViewModel_StateChanged(object sender, ScreenStateChangedEventArgs e)
        {
           // try { await Setup(); } catch { }
        }
        private void Exporter_Click(object sender, RoutedEventArgs e)
        {
            //DocEngine.Generate(model as ExtendedDocument);
            var dest = (sender as Button).Tag.ToString();


            try
            {
               
                if (dest == "PDF")
                {
                    (model as ExtendedDocument).ExportPDF(type);
                    //OvExport.OvPdfModelExport pdf = new OvExport.OvPdfModelExport(model.GetType(), model.GetType().Name, model);
                    //var file = pdf.GeneratePdf();
                    //if (!string.IsNullOrWhiteSpace(file))
                    //{
                    //    Thread.Sleep(2000);
                    //    Process.Start(file);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Go Create template! By!!");
                    //}
                }else if (dest == "OFFICE")
                {

                    (model as ExtendedDocument).ExportWORD(type);
                    //OvExport.OvPdfModelExport pdf = new OvExport.OvPdfModelExport(model.GetType(), model.GetType().Name, model);
                    //var file = pdf.GenerateOffice();
                    //if (!string.IsNullOrWhiteSpace(file))
                    //{
                    //    Thread.Sleep(2000);
                    //    Process.Start(file);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Go Create template! By!!");
                    //}
                }
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return;
            }



            //var rep = new ObjReportViewModel(model);
            //shell.OpenScreen(rep, "Report");
        }

        private async void Listview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Table double click
            if (isFreezed)
                return;
            var table = (sender as DataGrid);
            table.CancelEdit();
            table.CancelEdit(DataGridEditingUnit.Row);
            table.CancelEdit(DataGridEditingUnit.Cell);
            var dg = table.SelectedItem as ExtendedDocument;
            try
            {
                var selected = (sender as DataGrid).SelectedItem as ExtendedDocument;

                var ioc = DataHelpers.container;
                var vm = ioc.Get<ViewManager>();
                var c = await DetailViewModel.Create(selected, selected.GetType(), aggre, shell);
                c.CloseAfter = true;
                var content = vm.CreateAndBindViewForModelIfNecessary(c);

                var cc = new ContentControl();
                cc.HorizontalAlignment = HorizontalAlignment.Stretch;
                cc.VerticalAlignment = VerticalAlignment.Stretch;
                cc.Content = content;

                GenericWindowViewModel gw = new GenericWindowViewModel(cc, $"Modifier {selected.CollectionName}", selected.Name);

                DataHelpers.windowManager.ShowDialog(gw);
                await Setup();
            }
            catch (Exception s)
            {
                DataHelpers.windowManager.ShowMessageBox(s.Message, "Erreur");
                return;
            }
        }

        private async void NewMenu_Click(object sender, RoutedEventArgs e)
        {
            var Nmodel = (ExtendedDocument)model;
            if (Nmodel != null)
            {
                try
                {
                    // header[0] -> lClient
                    // header[1] -> Facture
                    // header = Facture
                    // property name is the link

                    var item = (Button)sender;
                    var header = (ArrayList)item.Tag;

                    var link = header[0].ToString();

                    //var relatedItems = DataHelpers.GetMongoData(header[1].ToString(), link, Nmodel.Id.ToString(), true);
                    var relatedItems = DataHelpers.GetMongoData(header[1].ToString(), link, Nmodel.Id, true);


                    var linkType = relatedItems.First().GetType();

                    //Type d1 = typeof(BaseViewModel<>);
                    //Type[] typeArgs = { linkType };
                    //Type makeme = d1.MakeGenericType(typeArgs);
                    //dynamic baseViewModel = Activator.CreateInstance(makeme, new object[] { aggre, false, relatedItems });
                    //var control = baseViewModel;
                    var view = DataHelpers.GetBaseViewModelScreen(linkType,aggre,false,relatedItems);
                    // var control = await BaseViewModel.Create(relatedItems, linkType, shell, aggre, DataHelpers.windowManager);
                    view.DisplayName = $"{item.Content.ToString()} / {model.Name}";

                    shell.OpenScreen(view, view.DisplayName);
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                    return;
                }
            }
        }

        // CUSTOM BUTTON
        private async void NewMenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string methodName = (sender as Button).Tag.ToString();
                var Nmodel = (ExtendedDocument)model;
                Nmodel.DoFunction(methodName);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
            await Setup();
        }

        // CUSTOM BUTTON OPS
        private async void NewOps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string methodName = (sender as Button).Tag.ToString();
                var Nmodel = (ExtendedDocument)model;
                Nmodel.DoFunction(methodName);
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }
            await Setup();
        }

        private void RefreshAll()
        {
            var props = (model as ExtendedDocument).GetType().GetProperties();

            foreach (var prop in props)
            {
                NotifyOfPropertyChange(prop.Name);
            }
        }

        private void Rtb_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Xceed.Wpf.Toolkit.MultiLineTextEditor).IsOpen = true;
        }

        private void Rtb_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Xceed.Wpf.Toolkit.MultiLineTextEditor).IsOpen = false;
        }
        public bool FinishLoaded { get; set; } = true;


        
        private async Task Setup(int delay = 0)
        {

            if (delay > 0)
                Thread.Sleep(delay);

            (model as ExtendedDocument).IsSelectedd = false;
            if(!FinishLoaded)
            { 
                return;
            }
        
            FinishLoaded = false;
             
            if (model == null)
                throw new Exception("Model vide");

            // try to relocate current scrol position

            isFreezed = false;
          //  MessageQueue.Enqueue(model.Status);
            GlobalMsg = model.Status;
            NotifyOfPropertyChange("GlobalMsg");
            linkButtons = new WrapPanel();
            linkButtons.Orientation = Orientation.Vertical;
            linkButtons.MaxHeight = 130;
            linkButtons.Margin = new Thickness(10);
            if (model.DocOpenMod == OpenMode.Detach)
            {
                //    fermerVisible = false;
            }

            // Opération BUtton
            opeartionButtons = new WrapPanel();
            opeartionButtons.HorizontalAlignment = HorizontalAlignment.Left;
           // opeartionButtons.Width = 820;
            //opeartionButtons.Background = Brushes.Gray;
            opeartionButtons.Orientation = Orientation.Horizontal;


            linkButtonsOps = new StackPanel();

            linkButtonsOps.Orientation = Orientation.Horizontal;
            // linkButtonsOps.MaxWidth = 430;

            // DOC CARD ONE

          


            Button actualiser = new Button();
            actualiser.Content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children = {
                    new PackIcon() { Kind = PackIconKind.Refresh, Width = 20, Height = 20 }
                    //,
                    //new TextBlock(){Text = "Actualiser", VerticalAlignment = VerticalAlignment.Center,}
                },
                VerticalAlignment = VerticalAlignment.Center,


            };
            actualiser.Style = App.Current.FindResource("SideToolButton") as Style;
            actualiser.Click += Actualiser_Click;
            actualiser.TouchDown += Actualiser_Click;


            Button Exporter = new Button();
            Exporter.Tag = "PDF";
            Exporter.Click += Exporter_Click; ;
            Exporter.TouchDown += Exporter_Click; ;
            Exporter.Content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children = {
                    new PackIcon() { Kind = PackIconKind.FilePdf, Width = 20, Height = 20 }
                    //,
                    //new TextBlock(){Text = "Exporter pdf", VerticalAlignment = VerticalAlignment.Center,}
                },
                VerticalAlignment = VerticalAlignment.Center,


            };
            Exporter.Style = App.Current.FindResource("SideToolButton") as Style;

            Button ExporterOffice = new Button();
            ExporterOffice.Tag = "OFFICE";
            ExporterOffice.Click += Exporter_Click; ;
            ExporterOffice.TouchDown += Exporter_Click; ;
            ExporterOffice.Content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children = {
                    new PackIcon() { Kind = PackIconKind.FileWordBox, Width = 20, Height = 20 }
                    //,
                    //new TextBlock(){Text = "Exporter word", VerticalAlignment = VerticalAlignment.Center,}
                },
                VerticalAlignment = VerticalAlignment.Center,


            };
            ExporterOffice.Style = App.Current.FindResource("SideToolButton") as Style;


            Button delete = new Button();
            delete.Click += Delete_Click; ;
            delete.TouchDown += Delete_Click; ;
            delete.Content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,                
                Children = {
                    new PackIcon() { Kind = PackIconKind.DeleteForever, Width = 20, Height = 20 }
                    //,
                    //new TextBlock(){Text = "Supprimer", VerticalAlignment = VerticalAlignment.Center,}
                },
                VerticalAlignment = VerticalAlignment.Center,
                

            };
            delete.Style = App.Current.FindResource("SideToolButton") as Style;



            //delete.Background = Brushes.WhiteSmoke;
            //actualiser.Background = Brushes.WhiteSmoke;
            //Exporter.Background = Brushes.WhiteSmoke;

            Separator sep = new Separator();
            sep.BorderThickness = new Thickness(1);
            sep.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");

            linkButtonsOps.Children.Add(delete);
            linkButtonsOps.Children.Add(actualiser);
            linkButtonsOps.Children.Add(Exporter);
            linkButtonsOps.Children.Add(ExporterOffice);

            
            linkButtonsOps.Children.Add(sep);

            masterWrap = new UniformGrid();
            masterWrap.Margin = new Thickness(10, 5, 10, 5);
            masterWrap.Columns = 2;
            // masterWrap.MaxHeight = 750;
            masterWrap.VerticalAlignment = VerticalAlignment.Stretch;
            masterWrap.HorizontalAlignment = HorizontalAlignment.Left;
            masterWrap.Width = 800;
            masterWrap.Height = Double.NaN;

            //Grid grid = new Grid();
            //var column1 = new ColumnDefinition()
            //{
            //    Width = new GridLength(450)
            //};
            //var column2 = new ColumnDefinition()
            //{
            //    Width = new GridLength(450)
            //};
            //var column3 = new ColumnDefinition()
            //{
            //    Width = new GridLength(200, GridUnitType.Star)
            //};

            //grid.ColumnDefinitions.Add(column1);
            //grid.ColumnDefinitions.Add(column2);
            //grid.ColumnDefinitions.Add(column3);

            //grid.ShowGridLines = false;

            stackContent = new StackPanel();
            stackContent.Margin = new Thickness(0);
            stackContent.Orientation = Orientation.Vertical;
                //  stackContent.MaxHeight = 750;
            stackContent.VerticalAlignment = VerticalAlignment.Stretch;
            stackContent.HorizontalAlignment = HorizontalAlignment.Stretch;

            var properties = type.GetProperties();

            var submitable = (model as ExtendedDocument);
            if (submitable.Submitable)
            {
                if (submitable.isLocal == false && submitable.DocStatus == 0)
                {
                    btnName = "Valider";
                    btnColor = "#009688";
                    NotifyOfPropertyChange("btnName");
                    NotifyOfPropertyChange("btnColor");
                }
                else if (submitable.isLocal == false && submitable.DocStatus == 1)
                {
                    btnName = "Annuler";
                    btnColor = "#FF3D00";
                    NotifyOfPropertyChange("btnName");
                    NotifyOfPropertyChange("btnColor");
                    isFreezed = true;
                }
            }

            //MethodInfo method = typeof(DataHelpers).GetMethod("GetMongoData");
            //MethodInfo generic = method.MakeGenericMethod(type.Assembly.GetType());
            //generic.Invoke(null, null);

            dynamic concrete = model;
            bool addToPanel = true;

            int indexRox = 0;

            Expander expander = new Expander();
            expander.Header = "Général";
            expander.IsExpanded = true;
            expander.HorizontalAlignment = HorizontalAlignment.Stretch;
           // expander.Width = 1100;
            expander.Padding = new Thickness(0, 0, 0, 20);
            expander.BorderThickness = new Thickness(0, 0, 1, 0);
            expander.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");


            foreach (var prop in properties)
            {
                addToPanel = true;
                string name = "";
                var dd = prop.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                if (dd != null)
                {
                    name = dd.DisplayName;
                }
                var dontShow = prop.GetCustomAttribute(typeof(DontShowInDetailAttribute)) as DontShowInDetailAttribute;
                var isBold = prop.GetCustomAttribute(typeof(IsBoldAttribute)) as IsBoldAttribute;

                //var s = prop.GetAttribute<DisplayNameAttribute>(false);
                if (!String.IsNullOrWhiteSpace(name) && dontShow == null)
                {
                    Label label = new Label();
                    label.Content = name;
                    // label.Width = 150;
                    //label.Height = 28;
                    label.Margin = new Thickness(3);
                    label.Padding = new Thickness(2);
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.FontSize = 13;
                    // label.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#686868");
                    label.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#535864");
                    // label.FontWeight = FontWeights.Light;
                    Grid.SetColumn(label, 0);
                    //if (isBold != null)
                    //{
                    //    if (isBold.IsBod)
                    //        label.FontWeight = FontWeights.Bold;
                    //}
                    Border border = new Border()
                    {
                        CornerRadius = new CornerRadius(2),
                        BorderThickness = new Thickness(1),
                        BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D1D8DD")
                    };
                    border.Width = elementWidth;
                    border.Height = 28;
                    border.Padding = new Thickness(0);
                    border.Margin = new Thickness(3);
                    border.HorizontalAlignment = HorizontalAlignment.Left;

                    StackPanel sp = new StackPanel();

                    //sp.Width = 450;
                    //sp.Height = 35;
                    //sp.Margin = new Thickness(3, 3, 3, 0);
                    //sp.Orientation = Orientation.Horizontal;

                    sp.Width = 350;
                    sp.Height = 70;
                    sp.VerticalAlignment = VerticalAlignment.Bottom;
                    sp.Margin = new Thickness(3, 3, 3, 0);
                    sp.Orientation = Orientation.Vertical;

                    var attributes = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                    var mytypeAttr = prop.GetCustomAttribute(typeof(myTypeAttribute)) as myTypeAttribute;
                    var setColumn = prop.GetCustomAttribute<SetColumnAttribute>();
                    if (attributes != null)
                    {
                        //if (indexRox == 17  )
                        //{
                        //    indexColumn ++;
                        //    indexRox = 0;
                        //}
                        
                        if (setColumn?.column == 1)
                        {
                            if(masterWrap.Children.Count % 2 != 0)
                                masterWrap.Children.Add(new StackPanel());
                        }
                        if (setColumn?.column == 2)
                        {
                            if (masterWrap.Children.Count % 2 == 0)
                                masterWrap.Children.Add(new StackPanel());
                        }

                        switch (attributes.FieldType)
                        {
                            case ModelFieldType.Text:
                                
                                var longDescription = prop.GetCustomAttribute(typeof(LongDescriptionAttribute)) as LongDescriptionAttribute;
                                TextBox tb = new TextBox();
                                HintAssist.SetHint(tb, name); 
                                //HintAssist.SetFloatingScale(tb, 1); 
                                tb.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                tb.Width = elementWidth; 
                               var myBinding = new Binding(prop.Name);
                                myBinding.Source = model;
                                myBinding.Mode = BindingMode.TwoWay;
                                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                tb.SetBinding(TextBox.TextProperty, myBinding); 
                                tb.KeyUp += Tb_KeyUp;
                                if (isBold?.IsBod == true)                               
                                    tb.FontWeight = FontWeights.Bold;
                                
                                if(longDescription != null)
                                {
                                    var text = longDescription.text;
                                    TextBlock tbLong = new TextBlock();
                                    tbLong.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                    tbLong.TextWrapping = TextWrapping.Wrap;
                                    tbLong.Width = elementWidth;
                                    tbLong.Text = text;
                                    sp.Children.Add(tbLong);
                                    sp.Height = double.NaN;
                                }
                                sp.Children.Add(tb);
                               
                                masterWrap.Children.Add(sp);
                                expander.IsExpanded = CollapseAll;
                                indexRox++;

                                break;

                            case ModelFieldType.Date:
                                var optionDate = attributes.Options;
 
                                    var datePicker = new DatePicker();
                                datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged2;
                                Border brDate = new Border();
                                brDate.BorderThickness = new Thickness(1);
                                brDate.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                brDate.CornerRadius = new CornerRadius(5);
                                brDate.Padding = new Thickness(1);
                                brDate.Background = Brushes.White;
                                brDate.Width = elementWidth;
                                  
                                    datePicker.Style = App.Current.FindResource("MaterialDesignFloatingHintDatePickerEx") as Style; 
                                   
                                   var myBindingDate = new Binding(prop.Name);
                                myBindingDate.Source = model;
                                myBindingDate.Mode = BindingMode.TwoWay;
                                myBindingDate.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                    datePicker.SetBinding(DateTimePicker.SelectedDateProperty, myBindingDate);
                                
                                    HintAssist.SetHint(datePicker, name); 
                                if (isBold?.IsBod == true)
                                    datePicker.FontWeight = FontWeights.Bold;

                                Button btnViewDate = new Button();
                                btnViewDate.Content = new PackIcon() { Kind = PackIconKind.Update };
                                //btnView.Style = App.Current.FindResource("MaterialDesignFloatingActionMiniDarkButton") as Style;
                                btnViewDate.Style = App.Current.FindResource("ToolButton") as Style;

                                btnViewDate.Margin = new Thickness(2);
                                btnViewDate.Padding = new Thickness(2);
                                btnViewDate.HorizontalAlignment = HorizontalAlignment.Right;

                                 
                                    btnViewDate.Tag = datePicker;
                                btnViewDate.Click += BtnViewDate_Click;
                                btnViewDate.TouchDown += BtnViewDate_Click;


                                datePicker.Width = elementWidth - 55;
                                datePicker.VerticalAlignment = VerticalAlignment.Bottom;
                                datePicker.VerticalContentAlignment = VerticalAlignment.Bottom;
                                StackPanel sp1date = new StackPanel();
                              //  sp1date.Margin = new Thickness(25,15,10,0);
                                sp1date.Orientation = Orientation.Horizontal;
                                sp1date.Children.Add(datePicker);
                                sp1date.Children.Add(btnViewDate);
                                brDate.Child = sp1date;


                                if (datePicker.Text == "")
                                    label.Content = "";
                                sp.Children.Add(label);
                                sp.Children.Add(brDate);
                                sp.Width = elementWidth;

                                masterWrap.Children.Add(sp);

                                break;

                            case ModelFieldType.Devise:

                                TextBox tbDevise = new TextBox();
                                // tbDevise.MouseDown += TbDevise_MouseDown;
                                // tbDevise.MouseLeftButtonUp += TbDevise_MouseDown;
                                HintAssist.SetHint(tbDevise, name);
                                // HintAssist.SetFloatingScale(tbDevise, 0.8);
                                if (isBold?.IsBod == true)
                                    tbDevise.FontWeight = FontWeights.Bold;
                                // tb.Style = App.Current.FindResource("MaterialDesignTextFieldBoxTextBoxExtra") as Style;
                                tbDevise.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                tbDevise.MouseLeftButtonUp += TbDevise_MouseDown1;
                                tbDevise.StylusButtonUp += TbDevise_StylusButtonUp; ;
                               
                                tbDevise.Width = elementWidth;
                              //  tbDevise.Height = 20;
                               var myBindingDevise = new Binding(prop.Name);
                                myBindingDevise.Source = model;
                                myBindingDevise.ConverterCulture = new System.Globalization.CultureInfo("ar-DZ");
                                myBindingDevise.StringFormat = !string.IsNullOrWhiteSpace(DataHelpers.Settings.FormatDevis) ? DataHelpers.Settings.FormatDevis : "{0:C}" ;
                                myBindingDevise.Mode = BindingMode.TwoWay;
                                myBindingDevise.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                tbDevise.SetBinding(TextBox.TextProperty, myBindingDevise);
                                //tbDevise.Margin = new Thickness(0, 0, 3, 0);
                                //tbDevise.Padding = new Thickness(7, -1, 0, 0);
                                //tbDevise.BorderThickness = new Thickness(0);
                                tbDevise.KeyUp += Tb_KeyUp;
                              //  border.Child = tbDevise;

                               // sp.Children.Add(label);
                                sp.Children.Add(tbDevise);
                                //if (indexRox >= grid.RowDefinitions.Count)
                                //{
                                //    RowDefinition gridRowDevise = new RowDefinition();
                                //    gridRowDevise.Height = new GridLength(35);
                                //    grid.RowDefinitions.Add(gridRowDevise);
                                //}
                                //Grid.SetColumn(sp, indexColumn);
                                //Grid.SetRow(sp, indexRox);
                                // grid.Children.Add(sp);
                                // indexRox++;

                                masterWrap.Children.Add(sp);
                                break;

                            case ModelFieldType.Numero:

                                TextBox tbNumero = new TextBox();
                                HintAssist.SetHint(tbNumero, name);
                               // HintAssist.SetFloatingScale(tbNumero, 0.8);
                                tbNumero.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                tbNumero.Width = elementWidth;

                               var myBindingNumero = new Binding(prop.Name);
                                myBindingNumero.Source = model;
                                myBindingNumero.StringFormat = attributes.Options;
                                myBindingNumero.Mode = BindingMode.TwoWay;
                                myBindingNumero.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                tbNumero.SetBinding(TextBox.TextProperty, myBindingNumero);
                                if (isBold?.IsBod == true)
                                    tbNumero.FontWeight = FontWeights.Bold;
                                //border.Child = tbNumero;
                                tbNumero.KeyUp += Tb_KeyUp;
                               // sp.Children.Add(label);
                                sp.Children.Add(tbNumero);

                                masterWrap.Children.Add(sp);
                                break;

                            case ModelFieldType.Separation:
                                bool expanded = true;
                                try { expanded = bool.Parse(attributes.Options); } catch { }
                                if (sp.Children.Count > 0)
                                    masterWrap.Children.Add(sp);
                                expander.Content = masterWrap;
                                stackContent.Children.Add(expander);
                                masterWrap = new UniformGrid();
                                masterWrap.Margin = new Thickness(10, 5, 10, 5);
                                masterWrap.Columns = 2;
                                masterWrap.MaxHeight = 750;
                                masterWrap.VerticalAlignment = VerticalAlignment.Stretch;
                                masterWrap.HorizontalAlignment = HorizontalAlignment.Left;
                              //  masterWrap.Width = Double.NaN;
                                masterWrap.Height = Double.NaN;
                                masterWrap.Width = 800;

                                borderSeparation = new Border();
                                borderSeparation.Background = App.Current.FindResource("MaterialDesignDivider") as Brush;
                                borderSeparation.Height = 1;
                                borderSeparation.HorizontalAlignment = HorizontalAlignment.Stretch;
                                borderSeparation.SnapsToDevicePixels = true;
                                stackContent.Children.Add(borderSeparation);

                             expander = new Expander();
                                expander.Header = name.ToUpper();
                                expander.IsExpanded = CollapseAll;
                                expander.Padding = new Thickness(0, 0, 0, 20);
                                expander.BorderThickness = new Thickness(0, 0, 1, 0);
                                expander.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");

                                break;

                            case ModelFieldType.Select:

                                var optionSelect = attributes.Options;

                                var dataSelect = DataHelpers.GetSelectData(optionSelect);
                                if (optionSelect.Contains("this"))
                                {
                                    var secondParam = optionSelect.Split('>');
                                    var options = model.GetType().GetProperty(secondParam[1].ToString()).GetValue(model) as List<string>;
                                    dataSelect = options.ToList();
                                }

                                Border brSelect = new Border();
                                brSelect.BorderThickness = new Thickness(1);
                                brSelect.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                brSelect.CornerRadius = new CornerRadius(5);
                                brSelect.Padding = new Thickness(1);
                                brSelect.Background = Brushes.White;

                                ComboBox boxSelect = new ComboBox();
                                HintAssist.SetHint(boxSelect, name);
                                //  HintAssist.SetFloatingScale(box, 0.8);
                                boxSelect.Style = App.Current.FindResource("MaterialDesignFloatingHintComboBoxWhite") as Style;
                                // box.Margin = new Thickness(0);
                                boxSelect.SelectionChanged += Box_SelectionChanged;
                                if (isBold?.IsBod == true)
                                    boxSelect.FontWeight = FontWeights.Bold;
                                boxSelect.Width = elementWidth - 20;

                          



                              
                                var pathSelect = $"{optionSelect}";
                               var myBindingSelect = new Binding($"{prop.Name}");
                                myBindingSelect.Source = model;
                                myBindingSelect.Mode = BindingMode.TwoWay;
                                myBindingSelect.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                boxSelect.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = dataSelect, Mode = BindingMode.OneTime });
                                boxSelect.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = model, Path = new PropertyPath(prop.Name), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });

                                if (boxSelect.Text == "")
                                    label.Content = "";

                                brSelect.Child = boxSelect;
                                sp.Width = elementWidth;
                                sp.Children.Add(label);
                                sp.Children.Add(brSelect);
                                masterWrap.Children.Add(sp);

                                // box.SetBinding(ComboBox.DisplayMemberPathProperty, new Binding { Source = $"Name" });
                                // box.SelectedValuePath = "Id";
                                // border.Height = 40; 
                                // X
                                // box.Padding = new Thickness(7, -1, 0, 0);
                                // border.Child = box;
                                //// sp.Children.Add(label);
                                // sp.Children.Add(border);

                                // masterWrap.Children.Add(sp);
                                break;

                            case ModelFieldType.Check:

                                CheckBox checkbox = new CheckBox();
                                var optioncheck = attributes.Options;
                                checkbox.Content = optioncheck;
                                checkbox.Width = elementWidth;
                                checkbox.Height = 20;
                              var myBindingCheck = new Binding(prop.Name);
                                myBindingCheck.Source = model;
                                myBindingCheck.Mode = BindingMode.TwoWay;
                                myBindingCheck.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                checkbox.SetBinding(CheckBox.IsCheckedProperty, myBindingCheck);
                                checkbox.Margin = new Thickness(0, 25, 0, 0);
                                //label.Margin = new Thickness(50, 0, 10, 0);
                                // checkbox.Padding = new Thickness(7, -1, 0, 0);
                                checkbox.BorderThickness = new Thickness(1);
                                //border.BorderThickness = new Thickness(0);
                                //border.Child = checkbox;
                                checkbox.Click += Checkbox_Click;
                                checkbox.TouchDown += Checkbox_Click;
                                label.FontSize = 12;
                                label.FontWeight = FontWeights.Normal;
                                sp.Children.Add(checkbox);
                                sp.Children.Add(label);
                                //if (indexRox >= grid.RowDefinitions.Count)
                                //{
                                //    RowDefinition gridRowCheck = new RowDefinition();
                                //    gridRowCheck.Height = new GridLength(35);
                                //    grid.RowDefinitions.Add(gridRowCheck);
                                //}
                                //Grid.SetColumn(sp, indexColumn);
                                //Grid.SetRow(sp, indexRox);
                                // grid.Children.Add(sp);
                                // indexRox++;

                                masterWrap.Children.Add(sp);
                                break;

                            case ModelFieldType.TextLarge:

                                if (sp.Children.Count > 0)
                                    masterWrap.Children.Add(sp);
                                expander.Content = masterWrap;
                                stackContent.Children.Add(expander);
                                masterWrap = new UniformGrid();
                                masterWrap.Margin = new Thickness(10, 5, 10, 5);
                                masterWrap.Columns = 1;
                                masterWrap.MaxHeight = 750;
                                masterWrap.VerticalAlignment = VerticalAlignment.Top;
                                masterWrap.HorizontalAlignment = HorizontalAlignment.Left;
                               // masterWrap.Width = Double.NaN;
                                masterWrap.Height = 100;
                               masterWrap.Width = 800;

                                expander = new Expander();
                                expander.Header = name.ToUpper();
                                expander.IsExpanded = CollapseAll;
                                expander.Padding = new Thickness(0, 0, 0, 20);
                                expander.BorderThickness = new Thickness(0, 0, 1, 0);
                                expander.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");

                                borderSeparation = new Border();
                                borderSeparation.Background = App.Current.FindResource("MaterialDesignDivider") as Brush;
                                borderSeparation.Height = 1;
                                borderSeparation.HorizontalAlignment = HorizontalAlignment.Stretch;
                                borderSeparation.SnapsToDevicePixels = true;
                                stackContent.Children.Add(borderSeparation);

                                label.VerticalAlignment = VerticalAlignment.Top;
                                TextBox rtb = new TextBox();
                                //  rtb.Margin = new Thickness(0);
                                //  tb.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                rtb.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxLarge") as Style;
                                rtb.Height = 100;
                                 rtb.Width = 750; ;

                                rtb.VerticalAlignment = VerticalAlignment.Top;
                                 HintAssist.SetHint(rtb, $"{name}..."); 

                                rtb.AcceptsReturn = true;
                                rtb.TextWrapping = TextWrapping.Wrap;
                                rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                                rtb.Margin = new Thickness(0);
                                rtb.Padding = new Thickness(0);
                               var myBindingTextLarge = new Binding(prop.Name);
                                myBindingTextLarge.Source = model;
                                myBindingTextLarge.Mode = BindingMode.TwoWay;
                                myBindingTextLarge.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                rtb.SetBinding(TextBox.TextProperty, myBindingTextLarge);
                                rtb.Background = Brushes.White;

                                masterWrap.Children.Add(rtb);

                                break;
                           

                            case ModelFieldType.Lien:
                                #region Champ lien
                                var optionLien = attributes.Options;
                                ComboBox box = new ComboBox();



                                Border br = new Border();
                                br.BorderThickness = new Thickness(1);
                                br.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                br.CornerRadius = new CornerRadius(5);
                                br.Padding = new Thickness(1);
                                br.Background = Brushes.White;
                                HintAssist.SetHint(box, name);
                                //HintAssist.SetFloatingScale(box, 1);
                                //HintAssist.SetFloatingOffset(box, new Point(0,-10));

                                box.Style = App.Current.FindResource("MaterialDesignFloatingHintComboBoxWhite") as Style;
                               // box.Style = App.Current.FindResource("MaterialDesignFloatingHintComboBox") as Style;
                                box.ItemsPanel = (ItemsPanelTemplate)Application.Current.FindResource("VSP");
                                box.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
                                box.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
                                if (isBold?.IsBod == true)
                                    box.FontWeight = FontWeights.Bold;
                                //  box.Margin = new Thickness(0);
                                box.KeyUp += Tb_KeyUp;
                                box.SelectionChanged += Box_SelectionChanged1; ;
                                box.DropDownClosed += Box_DropDownClosed;
                                box.MouseLeftButtonUp += Box_LostFocus1;
                                
                                //box.Padding = new Thickness(5, 2, 0, 0);
                                // box.BorderThickness = new Thickness(0.5);
                                // box.SelectionChanged += Box_LostFocus;
                                box.IsEditable = true;
                                var dataLien = await DataHelpers.GetMongoData(optionLien);
                                var property = prop.Name;
                                var pathLien = property;// $"l{optionLien}";
                               //var myBindingLien = new Binding($"{pathLien}");
                               // myBindingLien.Source = model;
                               // myBindingLien.Mode = BindingMode.TwoWay;
                               // myBindingLien.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                box.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = dataLien, Mode = BindingMode.OneTime });
                                box.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = model, Path = new PropertyPath(pathLien), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                                box.SetBinding(ComboBox.DisplayMemberPathProperty, new Binding { Source = $"Name" });
                                box.SelectedValuePath = "Id";
                                //  box.HorizontalAlignment = HorizontalAlignment.Stretch;
                                box.Width = elementWidth - 62;
                               
                               
                                Button btnView = new Button();
                                btnView.Content = new PackIcon() { Kind = PackIconKind.DatabaseSearch };

                                //btnView.Style = App.Current.FindResource("MaterialDesignFloatingActionMiniDarkButton") as Style;
                                btnView.Style = App.Current.FindResource("ToolButton") as Style;
                                var clsLien = new ArrayList() { box, $"ErpAlgerie.Modules.CRM.{optionLien}", prop.Name };



                                var clsBox = new ArrayList() { optionLien, prop.Name };
                                box.Tag = clsBox;
                                box.MouseDoubleClick += Box_MouseDoubleClick;
                                



                                btnView.Tag = clsLien;
                                btnView.Click += BtnView_Click1;
                                btnView.TouchDown += BtnView_Click1;

                                btnView.Margin = new Thickness(2);
                                btnView.Padding = new Thickness(2);
                                btnView.HorizontalAlignment = HorizontalAlignment.Right;

                                Button btnAdd = new Button();
                                btnAdd.Content = new PackIcon() { Kind = PackIconKind.Plus };
                                btnAdd.Style = App.Current.FindResource("ToolButton") as Style;
                                //  btnAdd.Style = App.Current.FindResource("MaterialDesignFloatingActionMiniDarkButton") as Style;
                                btnAdd.Tag = new ArrayList() { $"ErpAlgerie.Modules.CRM.{optionLien}", prop.Name };
                                btnAdd.Click += BtnAdd_Click;
                                btnAdd.TouchDown += BtnAdd_Click;
                                btnAdd.Margin = new Thickness(2);
                                btnAdd.Padding = new Thickness(2);
                                btnAdd.HorizontalAlignment = HorizontalAlignment.Right;

                                StackPanel sp1 = new StackPanel();
                                sp1.Margin = new Thickness(0);
                                sp1.Background = Brushes.White;
                                sp1.Orientation = Orientation.Horizontal;
                                sp1.Children.Add(box);
                                sp1.Children.Add(btnAdd);
                                sp1.Children.Add(btnView); 
                                // sp.Children.Add(label);
                                br.Child = sp1;
                                if (box.Text == "")
                                    label.Content = "";

                                sp.Children.Add(label);
                                sp.Children.Add(br);
                                sp.Width = elementWidth;
                                //if (indexRox >= grid.RowDefinitions.Count)
                                //{
                                //    RowDefinition gridRowLien = new RowDefinition();
                                //    gridRowLien.Height = new GridLength(35);
                                //    grid.RowDefinitions.Add(gridRowLien);
                                //}
                                //Grid.SetColumn(sp, indexColumn);
                                //Grid.SetRow(sp, indexRox);
                                // grid.Children.Add(sp);
                                // indexRox++;

                                masterWrap.Children.Add(sp);
                                break;

                            #endregion

                            case ModelFieldType.ReadOnly:
                                #region Champ read only
                                var format = attributes.Options;
                                TextBox tbReadOnly = new TextBox();                                
                                tbReadOnly.Width = elementWidth;
                                HintAssist.SetHint(tbReadOnly, name);
                               // HintAssist.SetFloatingScale(tbReadOnly, 0.8);
                                tbReadOnly.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                if (isBold?.IsBod == true)
                                    tbReadOnly.FontWeight = FontWeights.Bold;

                                tbReadOnly.Background = Brushes.WhiteSmoke;
                                //  tbReadOnly.Height = 20;
                                myBinding = new Binding(prop.Name);
                                myBinding.Source = model;
                                myBinding.Mode = BindingMode.OneWay;
                                myBinding.ConverterCulture = new System.Globalization.CultureInfo("ar-DZ");
                                myBinding.StringFormat = format;
                                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                tbReadOnly.SetBinding(TextBox.TextProperty, myBinding);
                                //tbReadOnly.Margin = new Thickness(0);
                                //tbReadOnly.Padding = new Thickness(7, -1, 0, 0);
                                //tbReadOnly.BorderThickness = new Thickness(0);
                                tbReadOnly.IsReadOnly = true;
                                //  border.Child = tbReadOnly;

                                // sp.Children.Add(label);
                                sp.Children.Add(tbReadOnly);
                                //if (indexRox >= grid.RowDefinitions.Count)
                                //{
                                //    RowDefinition gridRowReadOnly = new RowDefinition();
                                //    gridRowReadOnly.Height = new GridLength(35);
                                //    grid.RowDefinitions.Add(gridRowReadOnly);
                                //}
                                //Grid.SetColumn(sp, indexColumn);
                                //Grid.SetRow(sp, indexRox);
                                // grid.Children.Add(sp);
                                // indexRox++;

                                masterWrap.Children.Add(sp);
                                break;

                            #endregion

                            case ModelFieldType.LienButton:
                                #region Champ Lien Button
                                Button newMenu = new Button();
                                var optionLienButton = attributes.Options; // model name
                                var propName = "";
                                if (optionLienButton.Contains(">"))
                                {
                                    // property name in options
                                    var spilts = optionLienButton.Split('>');
                                    propName = spilts[1].ToString();
                                    optionLienButton = spilts[0].ToString();
                                }
                                else
                                {
                                    propName = prop.Name;
                                }

                                newMenu.Content = name;
                                newMenu.Style = App.Current.FindResource("LinkButton") as Style;
                              
                                var tagLien = new ArrayList() { propName, optionLienButton };
                                newMenu.Tag = tagLien;
                                newMenu.Click += NewMenu_Click;
                                newMenu.TouchDown += NewMenu_Click;
                                newMenu.HorizontalAlignment = HorizontalAlignment.Left;
                                newMenu.HorizontalContentAlignment = HorizontalAlignment.Left;

                                var stackLienContent = new StackPanel();
                                stackLienContent.Orientation = Orientation.Horizontal;
                                stackLienContent.HorizontalAlignment = HorizontalAlignment.Left;
                                stackLienContent.Margin = new Thickness(0);

                                stackLienContent.Children.Add(newMenu);

                                if (!prop.Name.Contains("NOBTN"))
                                {
                                    Button btnAddLien = new Button();
                                    btnAddLien.Content = new PackIcon() { Kind = PackIconKind.Plus, Width = 16 };
                                    btnAddLien.Style = App.Current.FindResource("LinkButtonPlus") as Style;
                                    btnAddLien.Tag = tagLien;
                                    btnAddLien.Click += BtnAddLien_Click;
                                    btnAddLien.TouchDown += BtnAddLien_Click;

                                    stackLienContent.Children.Add(btnAddLien);
                                }

                                linkButtons.Children.Add(stackLienContent);
                                if(!(model as ExtendedDocument).isLocal)
                                    LinksVisible = Visibility.Visible;
                                NotifyOfPropertyChange("LinksVisible");
                                NotifyOfPropertyChange("linkButtons");
                                //addToPanel = false;
                                break;

                            #endregion

                            case ModelFieldType.Image:
                                #region Champ image
                                var optionImage = attributes.Options;
                                sp.Height = 220;
                                Image img = new Image();
                                img.Width = elementWidth-60;
                                img.Height = 170;
                                var value = model.GetType().GetProperty(prop.Name).GetValue(model, null);
                                var imgPath = "";
                                if (value != null)
                                {
                                    imgPath = System.IO.Path.GetFullPath(value);
                                }

                                img.SetBinding(Image.SourceProperty, new Binding() { Source = imgPath, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                               // img.Margin = new Thickness(25,0,0,10);
                                Border b = new Border();
                                b.BorderThickness = new Thickness(1);
                                b.CornerRadius = new CornerRadius(5);
                                b.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                Button addBtn = new Button();
                                addBtn.Tag = prop.Name;
                                addBtn.Content = "Changer";
                                addBtn.Click += AddBtn_Click;
                                addBtn.TouchDown += AddBtn_Click;
                                addBtn.VerticalAlignment = VerticalAlignment.Bottom;
                                addBtn.Height = 26;
                                addBtn.Width = elementWidth;
                                StackPanel sps = new StackPanel();
                                sps.Orientation = Orientation.Vertical;
                                sps.HorizontalAlignment = HorizontalAlignment.Stretch;
                                sps.VerticalAlignment = VerticalAlignment.Stretch;
                                b.Child = img;
                                sps.Children.Add(b);
                                sps.Children.Add(addBtn);
                                sps.Margin = new Thickness(25, 10, 0, 10);
                                
                                //border.Height = 200;
                                //border.Child = sps;

                                //sp.Children.Add(border);

                                masterWrap.Children.Add(sps);

                                break;

                            case ModelFieldType.ImageSide:
                              
                                var optionImageSide = attributes.Options;
                              //  sp.Height = 220;
                                Image imgSide = new Image();
                                imgSide.Width = 180;
                                //imgSide.Height = 170;
                                var valueSide = model.GetType().GetProperty(prop.Name).GetValue(model, null);
                                var imgPathSide = "";
                                if (valueSide != null)
                                {
                                    imgPathSide = System.IO.Path.GetFullPath(valueSide);
                                }

                                imgSide.SetBinding(Image.SourceProperty, new Binding() { Source = imgPathSide,
                                    Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                imgSide.Margin = new Thickness(0);

                                Button addBtnSide = new Button();
                                addBtnSide.Style = App.Current.FindResource("ImportButtonStyle") as Style;
                                addBtnSide.Tag = prop.Name;
                                addBtnSide.Content = optionImageSide;
                                addBtnSide.Click += AddBtn_Click;
                                addBtnSide.TouchDown += AddBtn_Click;
                                addBtnSide.VerticalAlignment = VerticalAlignment.Bottom;
                                addBtnSide.Height = 26;
                                addBtnSide.Width = 180;
                                DocImageContent = new StackPanel();
                                DocImageContent.Orientation = Orientation.Vertical;
                                DocImageContent.HorizontalAlignment = HorizontalAlignment.Stretch;
                                DocImageContent.VerticalAlignment = VerticalAlignment.Stretch;
                                DocImageContent.Children.Add(imgSide);
                                DocImageContent.Children.Add(addBtnSide);
                                NotifyOfPropertyChange("DocImageContent");
                                //border.Height = 200;
                                //border.Child = spsSide;

                                //sp.Children.Add(border);

                               // masterWrap.Children.Add(sp);

                                break;

                            #endregion

                            case ModelFieldType.WeakTable:

                                // AccessRule
                                var optionWeak = attributes.Options;
                               
                                WrapPanel wrap = new WrapPanel();
                                // List<AccessRule>
                                var sourceData = DataHelpers.GetMongoDataSync(optionWeak) as IEnumerable<ExtendedDocument>;
                                foreach (var item in sourceData)
                                {
                                    CheckBox itemBox = new CheckBox();
                                    itemBox.Margin = new Thickness(20, 5, 5, 5);
                                    itemBox.Content = item.Name;
                                    if ((model as ExtendedDocument).GetType().GetProperty(prop.Name).GetValue(model)?.Contains(item.Id))
                                        itemBox.IsChecked = true;
                                    itemBox.Tag =new ArrayList() { prop.Name, item.Id };
                                    itemBox.Checked += ItemBox_Checked;
                                    itemBox.Unchecked += ItemBox_Checked;

                                    wrap.Children.Add(itemBox);
                                }
                                masterWrap.Children.Add(wrap);
                                break;


                            case ModelFieldType.Table:
                                #region Champs table

                                if(masterWrap.Children.Count > 0)
                                {
                                    // NO SEPARATION
                                    if (sp.Children.Count > 0)
                                        masterWrap.Children.Add(sp);
                                    expander.Content = masterWrap;
                                    stackContent.Children.Add(expander);
                                    masterWrap = new UniformGrid();
                                    masterWrap.Margin = new Thickness(10, 5, 10, 5);
                                    masterWrap.Columns = 1;

                                    masterWrap.VerticalAlignment = VerticalAlignment.Top;
                                    masterWrap.HorizontalAlignment = HorizontalAlignment.Left;
                                    // masterWrap.Width = Double.NaN;
                                   // masterWrap.Height = 100;
                                    masterWrap.Width = 800;

                                    expander = new Expander();
                                    expander.Header = name.ToUpper();
                                    expander.IsExpanded = CollapseAll;
                                    expander.Padding = new Thickness(0, 0, 0, 20);
                                    expander.BorderThickness = new Thickness(0, 0, 1, 0);
                                    expander.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");

                                    borderSeparation = new Border();
                                    borderSeparation.Background = App.Current.FindResource("MaterialDesignDivider") as Brush;
                                    borderSeparation.Height = 1;
                                    borderSeparation.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    borderSeparation.SnapsToDevicePixels = true;
                                    stackContent.Children.Add(borderSeparation);

                                }

                                var optionTable = attributes.Options;
                                var type = mytypeAttr.type;
                                var afterMapMethod = prop.GetCustomAttribute(typeof(AfterMapMethodAttribute)) as AfterMapMethodAttribute;

                                DataGrid listview = new DataGrid();
                                listview.SelectionUnit = DataGridSelectionUnit.FullRow;


                                ContextMenu cm = new ContextMenu();
                                MenuItem deleteItem = new MenuItem();
                                deleteItem.Header = "Supprimer";
                                deleteItem.Click += DeleteItem_Click;
                                deleteItem.TouchDown += DeleteItem_Click;
                                deleteItem.IsEnabled = !isFreezed;
                                deleteItem.Tag = listview;
                                cm.Items.Add(deleteItem); 

                                MenuItem deleteAll = new MenuItem();
                                deleteAll.Header = "Supprimer tout";
                                deleteAll.Click += DeleteAll_Click; ;
                                deleteAll.TouchDown += DeleteAll_Click; ;
                                deleteAll.Tag = listview;
                                deleteAll.IsEnabled = !isFreezed;
                                cm.Items.Add(deleteAll);
                                
                                listview.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0"); ;
                                listview.BorderThickness = new Thickness(1);
                                listview.FontSize = 14;
                                listview.ContextMenu = cm;
                                listview.HorizontalAlignment = HorizontalAlignment.Stretch;
                                listview.VerticalAlignment = VerticalAlignment.Stretch;
                                listview.AutoGenerateColumns = true;
                                listview.Background = Brushes.White;
                                listview.Tag = afterMapMethod;
                                // listview.CanUserAddRows = !isFreezed;
                                listview.CanUserAddRows = false;
                                listview.CanUserDeleteRows = !isFreezed;
                               // listview.IsReadOnly = true;
                                listview.RowEditEnding += Listview_RowEditEnding;
                                 
                                listview.Unloaded += Listview_Unloaded;
                                listview.MouseDoubleClick += Listview_MouseDoubleClick;
                                var myBindingTable = new Binding(prop.Name);
                                myBindingTable.Source = model;
                                myBindingTable.IsAsync = true;
                                myBindingTable.Mode = BindingMode.TwoWay;

                                myBindingTable.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                listview.AutoGeneratingColumn += ColumnHeaderBehavior.OnAutoGeneratingColumn;
                                listview.MinHeight = 150;
                                listview.MinWidth = 655;
                                listview.Margin = new Thickness(25, 0, 20, 20);
                                label.Visibility = Visibility.Hidden;
                                listview.SetBinding(DataGrid.ItemsSourceProperty, myBindingTable);
                                DataGridAssist.SetCellPadding(listview, new Thickness(4, 2, 2, 2));
                                DataGridAssist.SetColumnHeaderPadding(listview, new Thickness(4, 2, 2, 2));
                                label.Width = 0;
                                Grid spsTable = new Grid();
                                var clmTable = new ColumnDefinition()
                                {
                                    Width = new GridLength(1, GridUnitType.Star)
                                };
                                var rowTable = new RowDefinition()
                                {
                                    Height = new GridLength(1, GridUnitType.Star),
                                };
                                var rowTable2 = new RowDefinition()
                                {
                                    Height = new GridLength(1, GridUnitType.Star),
                                };
                                spsTable.ColumnDefinitions.Add(clmTable);
                                spsTable.RowDefinitions.Add(rowTable);
                                spsTable.RowDefinitions.Add(rowTable2);

                                Grid.SetColumn(listview, 0);
                                Grid.SetRow(listview, 1);

                                // Add buttons in stackpanel
                                var stackForButtons = new StackPanel();
                                stackForButtons.Orientation = Orientation.Horizontal;
                                stackForButtons.Margin = new Thickness(25, 10, 10, 10);
                                // COmbo select
                                Tablebox = new ComboBox();
                                HintAssist.SetHint(Tablebox, $"Checher {name}");
                                //HintAssist.SetFloatingScale(Tablebox, 0.8);

                                Tablebox.Style = App.Current.FindResource("MaterialDesignFloatingHintComboBoxWhiteTable") as Style;
                                Tablebox.ItemsPanel = (ItemsPanelTemplate)Application.Current.FindResource("VSP");
                                Tablebox.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
                                Tablebox.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
                                Tablebox.Margin = new Thickness(0, 0, 10, 0); 
                                Tablebox.IsEditable = true;
                                var tagTable = new ArrayList() { listview, type.Name, optionTable };
                                Tablebox.Tag = tagTable;
                                Tablebox.KeyUp += Tablebox_KeyUp;

                                var boxdataLien = await DataHelpers.GetMongoData(type.Name);

                                Tablebox.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = boxdataLien, Mode = BindingMode.OneTime ,IsAsync=true});
                                Tablebox.SetBinding(ComboBox.SelectedValueProperty, new Binding { Source = this, Path = new PropertyPath("tableModel"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                                Tablebox.SetBinding(ComboBox.DisplayMemberPathProperty, new Binding { Source = $"Name" });
                                Tablebox.SelectedValuePath = "Id";
                                Tablebox.HorizontalAlignment = HorizontalAlignment.Stretch;
                                Tablebox.Width = 400;
                                Tablebox.SelectionChanged += Tablebox_SelectionChanged;
                                Tablebox.IsTextSearchEnabled = false;            
                                // btn Find or select
                                Button btnSelect = new Button();
                                btnSelect.Style = App.Current.FindResource("ToolButton") as Style;
                                btnSelect.Content = new PackIcon() { Kind = PackIconKind.DatabaseSearch };
                                btnSelect.Margin = new Thickness(2);
                                btnSelect.Padding = new Thickness(2);
                                btnSelect.HorizontalAlignment = HorizontalAlignment.Right;
                                var cls = new ArrayList() { listview, $"ErpAlgerie.Modules.CRM.{type.Name}", optionTable };
                                btnSelect.Tag = cls;
                                btnSelect.IsEnabled = !isFreezed;
                                btnSelect.Click += BtnSelect_Click; ;
                                btnSelect.TouchDown += BtnSelect_Click; ;

                                // btn créer
                                Button btnNewModel = new Button();
                                btnNewModel.Style = App.Current.FindResource("ToolButton") as Style;
                                btnNewModel.Content = new PackIcon() { Kind = PackIconKind.Plus };
                                btnNewModel.Margin = new Thickness(2);
                                btnNewModel.Padding = new Thickness(2);
                                btnNewModel.HorizontalAlignment = HorizontalAlignment.Right;
                                btnNewModel.IsEnabled = !isFreezed;
                                btnNewModel.Tag = new ArrayList() { $"ErpAlgerie.Modules.CRM.{type.Name}", prop.Name };
                                btnNewModel.Click += BtnNewModel_Click; ;
                                btnNewModel.TouchDown += BtnNewModel_Click; ;

                                // Button add
                                btnAddModel = new Button();
                                btnAddModel.Style = App.Current.FindResource("ToolButton") as Style;
                                btnAddModel.Content = new PackIcon() { Kind = PackIconKind.CheckCircle };
                                btnAddModel.Margin = new Thickness(2);
                                btnAddModel.Padding = new Thickness(2);
                                btnAddModel.HorizontalAlignment = HorizontalAlignment.Right;
                                var tag2 = new ArrayList() { listview, type.Name, optionTable };
                                btnAddModel.IsEnabled = !isFreezed;
                                btnAddModel.Tag = tag2;
                                btnAddModel.Click += BtnAddModel_Click;
                                btnAddModel.TouchDown += BtnAddModel_Click;

                                // Button delete all
                                Button btnDeleteall = new Button();
                                btnDeleteall.Content = new PackIcon() { Kind = PackIconKind.DeleteForever };
                                btnDeleteall.Style = App.Current.FindResource("ToolButton") as Style;
                                btnDeleteall.Margin = new Thickness(2);
                                btnDeleteall.Padding = new Thickness(2);
                                btnDeleteall.HorizontalAlignment = HorizontalAlignment.Right;
                                btnDeleteall.Tag = listview;
                                btnDeleteall.IsEnabled = !isFreezed;

                                // Button UP
                                Button btnUP = new Button();
                                btnUP.Content = new PackIcon() { Kind = PackIconKind.ArrowUpBoldCircle };
                                btnUP.Style = App.Current.FindResource("ToolButton") as Style;
                                btnUP.Margin = new Thickness(2);
                                btnUP.Padding = new Thickness(2);
                                btnUP.HorizontalAlignment = HorizontalAlignment.Right;
                                btnUP.Tag = listview;
                                btnUP.IsEnabled = !isFreezed;
                                btnUP.Click += BtnDOWN_Click;
                                btnUP.TouchDown += BtnDOWN_Click;

                                // Button DOWN
                                Button btnDOWN = new Button();
                                btnDOWN.Content = new PackIcon() { Kind = PackIconKind.ArrowDownBoldCircle };
                                btnDOWN.Style = App.Current.FindResource("ToolButton") as Style;
                                btnDOWN.Margin = new Thickness(2);
                                btnDOWN.Padding = new Thickness(2);
                                btnDOWN.HorizontalAlignment = HorizontalAlignment.Right;
                                btnDOWN.Tag = listview;
                                btnDOWN.IsEnabled = !isFreezed;
                                btnDOWN.Click += BtnUP_Click;
                                btnDOWN.TouchDown += BtnUP_Click;


                                stackForButtons.Children.Add(Tablebox);

                                stackForButtons.Children.Add(btnAddModel);
                                stackForButtons.Children.Add(btnSelect);
                                stackForButtons.Children.Add(btnNewModel);
                                stackForButtons.Children.Add(btnDeleteall);
                                stackForButtons.Children.Add(btnUP);
                                stackForButtons.Children.Add(btnDOWN);


                                Grid.SetColumn(stackForButtons, 0);
                                Grid.SetRow(stackForButtons, 0);
                                btnDeleteall.Click += Btnaddline_Click;
                                btnDeleteall.TouchDown += Btnaddline_Click;
                                spsTable.Children.Add(listview);
                                spsTable.Children.Add(stackForButtons);
                                spsTable.Margin = new Thickness(0);
                                masterWrap.Children.Clear();
                                masterWrap.Columns = 1;

                                masterWrap.Children.Add(spsTable);
                                //masterWrap.Children.Add(btnaddline);

                                if (isFreezed)
                                {
                                    listview.IsReadOnly = true;
                                }
                                break;

                            #endregion
                            case ModelFieldType.LienFetch:
                                #region Champ read only

                                
                                var allattLienFetch = attributes.Options.Split('>');
                                var propertyLienFetch = allattLienFetch[0];
                                var valueLienFetch = allattLienFetch[1];

                                

                                TextBox tbLienFetch = new TextBox();

                                tbLienFetch.IsReadOnly = true;
                                tbLienFetch.Width = elementWidth;
                                HintAssist.SetHint(tbLienFetch, name); 
                                tbLienFetch.Style = App.Current.FindResource("MaterialDesignFloatingHintTextBoxWhite") as Style;
                                if (isBold?.IsBod == true)
                                    tbLienFetch.FontWeight = FontWeights.Bold;

                                tbLienFetch.Background = Brushes.WhiteSmoke;  
                                myBinding = new Binding(prop.Name);
                                myBinding.Source = model; 
                                 myBinding.Mode = BindingMode.OneWayToSource;
                                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                                tbLienFetch.SetBinding(TextBox.TextProperty, myBinding);


                                var id = model.GetType().GetProperty(propertyLienFetch)?.GetValue(model); // <= ObjecId
                                var sources = DataHelpers.GetById(mytypeAttr.type.Name, id);
                                var valueFetched = (sources as ExtendedDocument)?.GetType()?.GetProperty(valueLienFetch)?.GetValue(sources);
                                if(valueFetched != null)
                                {
                                    tbLienFetch.Text = valueFetched?.ToString();
                                    
                                }
                                else
                                {
                                    tbLienFetch.Text ="";
                                }
                                sp.Children.Add(tbLienFetch); 
                                masterWrap.Children.Add(sp);
                                break;

                                #endregion
                                break;
                            case ModelFieldType.LienField:

                                #region Champs lien Field

                                //optionLienField == lClientCommandeVente
                                // lClientCommandeVente =model.lClientCommandeVente => Client

                                Border brField = new Border();
                                brField.BorderThickness = new Thickness(1);
                                brField.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#AFBFC0");
                                brField.CornerRadius = new CornerRadius(5);
                                brField.Padding = new Thickness(1);
                                brField.Background = Brushes.White;


                                var allatt = attributes.Options.Split('>');
                                var optionLienField = allatt[0];
                               
                                box = new ComboBox();
                                 
                                HintAssist.SetHint(box, name);
                               // HintAssist.SetFloatingScale(box, 0.8);

                                box.Style = App.Current.FindResource("MaterialDesignFloatingHintComboBoxWhite") as Style;
                                box.ItemsPanel = (ItemsPanelTemplate)Application.Current.FindResource("VSP");
                                box.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
                                box.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
                                if (isBold?.IsBod == true)
                                    box.FontWeight = FontWeights.Bold;
                                //  box.Margin = new Thickness(0);
                                box.KeyUp += Tb_KeyUp;
                                //box.Padding = new Thickness(5, 2, 0, 0);
                                // box.BorderThickness = new Thickness(0.5);
                                // box.SelectionChanged += Box_LostFocus;
                                box.IsEditable = true;
                                dynamic modelValuesId = null; 
                                if (optionLienField.Contains("()"))
                                {
                                    modelValuesId = model.GetType().
                                  GetMethod(optionLienField.Replace("()",""))?.Invoke(model,null); 
                                }
                                else
                                {
                                    modelValuesId = model.GetType().
                                  GetProperty(optionLienField)?.
                                  GetValue(model);
                                }
                              

                                if (mytypeAttr != null && modelValuesId != null)
                                {
                                    var sourcProp = allatt[1];

                                    dynamic sourceEntity = null;
                                    if (modelValuesId.GetType() == typeof(ObjectId))
                                    {
                                      //  var instance = Activator.CreateInstance(mytypeAttr.type);
                                        //sourceEntity = await (instance as ModelBase<>).GetById( Getd();
                                       // sourceEntity = DataHelpers.GetById(mytypeAttr.type.Name, modelValuesId);
                                        ObjectId _id = modelValuesId;
                                        //sourceEntity = DataHelpers.GetDataStatic(mytypeAttr.type, a => (a as ExtendedDocument).Id == _id);
                                        //IGenericData ds = DataHelpers.GetGenericData(mytypeAttr.type);

                                        sourceEntity = DataHelpers.GetById(mytypeAttr.type,_id);
                                    }
                                    else
                                    {
                                        sourceEntity = modelValuesId;
                                    }
                                   // DataHelpers.GetById(mytypeAttr.type.Name, modelValuesId);
                                    if (sourceEntity != null)
                                    {
                                        var modelValues = sourceEntity.GetType().GetProperty(sourcProp)
                                          .GetValue(sourceEntity);
                                        if (modelValues != null)
                                        {
                                            box.SetBinding(ComboBox.ItemsSourceProperty,
                                                new Binding
                                                {
                                                    Source = modelValues,
                                                    Mode = BindingMode.OneWay
                                                });

                                            box.SetBinding(ComboBox.SelectedValueProperty, new Binding
                                            {
                                                Source = model,
                                                Path = new PropertyPath(prop.Name),
                                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                                Mode = BindingMode.TwoWay
                                            });

                                            box.SetBinding(ComboBox.DisplayMemberPathProperty,
                                                new Binding { Source = $"Name" });
                                            box.SelectedValuePath = "Id"; 
                                        }
                                    }
                                }





                                else if(modelValuesId != null)
                                {
                                    var sourceEntity = modelValuesId;


                                    if (sourceEntity != null)
                                    {
                                        
                                        box.SetBinding(ComboBox.ItemsSourceProperty,
                                            new Binding
                                            {
                                                Source = sourceEntity,
                                                Mode = BindingMode.OneWay
                                            });

                                        box.SetBinding(ComboBox.SelectedValueProperty, new Binding
                                        {
                                            Source = model,
                                            Path = new PropertyPath(prop.Name),
                                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                            Mode = BindingMode.TwoWay
                                        });

                                        box.SetBinding(ComboBox.DisplayMemberPathProperty,
                                            new Binding { Source = $"Name" });
                                        box.SelectedValuePath = "Id";
                                    }
                                }

                                 
                                brField.Width = elementWidth;
                                box.Width = elementWidth - 20;

                                StackPanel sp1Field = new StackPanel();
                                sp1Field.Orientation = Orientation.Horizontal;
                                
                                sp1Field.Children.Add(box);

                                brField.Child = sp1Field;
                                if (box.SelectedItem == null)
                                    label.Content = "";
                                sp.Children.Add(label);
                                sp.Children.Add(brField);
                                sp.Width = elementWidth;
                                masterWrap.Children.Add(sp);

                                #endregion  

                                break;

                            case ModelFieldType.OpsButton:

                                Button newOps = new Button();
                                var optionnewOps = attributes.Options; // model name
                                newOps.Content = name; 
                                newOps.Style = App.Current.FindResource("ToolBarAction") as Style;

                                
                                newOps.Tag = optionnewOps;
                                newOps.Click += NewOps_Click;
                                newOps.TouchDown += NewOps_Click;
                                opeartionButtons.Orientation = Orientation.Horizontal;                                 
                                opeartionButtons.Children.Add(newOps); 
                                addToPanel = false;
                                break;

                            case ModelFieldType.Button:

                                Button newMenuButton = new Button();

                                var optionButtonn = attributes.Options; // model name
                                newMenuButton.Content = name;
                                newMenuButton.Style = App.Current.FindResource("DetailButton") as Style;
                                newMenuButton.Tag = optionButtonn;
                                newMenuButton.Click += NewMenuButton_Click; ;
                                newMenuButton.TouchDown += NewMenuButton_Click; ;
                                border.Child = newMenuButton;
                                sp.Children.Add(label);
                                sp.Children.Add(border);
                                //if (indexRox >= grid.RowDefinitions.Count)
                                //{
                                //    RowDefinition gridRowReadOnly = new RowDefinition();
                                //    gridRowReadOnly.Height = new GridLength(35);
                                //    grid.RowDefinitions.Add(gridRowReadOnly);
                                //}
                                //Grid.SetColumn(sp, indexColumn);
                                //Grid.SetRow(sp, indexRox);
                                //grid.Children.Add(sp);
                                //indexRox++;
                                // opeartionButtons.Add(newMenuButton);
                                // addToPanel = false;

                                masterWrap.Children.Add(sp);

                                break;

                            default:
                                break;
                        }
                    }

                    if (addToPanel && (dontShow == null))
                    {
                        //if (isFreezed)
                        //{
                        //    foreach (UIElement item in masterWrap.Children)
                        //    {
                        //       var texts = item.FindChildren<TextBox>();
                        //        foreach (var t in texts)
                        //        {
                        //            var propertyElement = item.GetType().GetProperty("IsReadOnlyProperty");
                        //            if (propertyElement != null)
                        //            {
                        //                propertyElement.SetValue(item, true);
                        //            }
                        //        }
                        //    }
                        //}

                        //sp.Children.Add(label);

                        //sp.Children.Add(border);
                        //stackContent.Children.Add(sp);
                    }
                }
            }

            //  stackContent.Children.Add(grid);
            // stackContent.Children.Add(masterWrap);
            
            expander.Content = masterWrap;
            stackContent.Children.Add(expander);
            masterWrap = new UniformGrid();
            masterWrap.Margin = new Thickness(10, 5, 10, 5);
            masterWrap.Columns = 2;
            masterWrap.MaxHeight = 750;
            masterWrap.VerticalAlignment = VerticalAlignment.Stretch;
            masterWrap.HorizontalAlignment = HorizontalAlignment.Stretch;
           // masterWrap.Width = Double.NaN;
            masterWrap.Height = Double.NaN;
            masterWrap.Width = 800;

            if (isFreezed)
            {
                var elements = stackContent.FindChildren<Expander>(); 

                foreach (var item in elements)
                { 
                    item.IsEnabled = false; 
                }
                 

            }


            expander = new Expander();
            expander.Padding = new Thickness(0, 0, 0, 20);
            expander.HorizontalAlignment = HorizontalAlignment.Stretch;
            expander.BorderThickness = new Thickness(0, 0, 1, 0);
            expander.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#D3D3D3");
            expander.IsExpanded = CollapseAll;
            //expander.Header = "es";
            //expander.IsExpanded = true;
            //masterStackContent.CanContentScroll = true;

            //masterStackContent.Content = (stackContent);

            //masterStackContent.ScrollToVerticalOffset(CurrentScrollPosition);

            NotifyOfPropertyChange("stackContent");
            NotifyOfPropertyChange("opeartionButtons");
            NotifyOfPropertyChange("DocStatus");
            NotifyOfPropertyChange("linkButtonsOps");
            NotifyOfPropertyChange("linkButtons");

            NotifyOfPropertyChange("model");
            FinishLoaded = true;
              
        }

        private async void DatePicker_SelectedDateChanged2(object sender, SelectionChangedEventArgs e)
        {
            await Setup();
        }

        private async void Box_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // new ArrayList() { optionLien, prop.Name };
            var data = (sender as ComboBox).Tag as ArrayList;

            var link = data[0]?.ToString(); 
            var prop = data[1]?.ToString();
             
            ObjectId? valueId = (model as ExtendedDocument).GetType().GetProperty(prop).GetValue(model); 

            if(valueId != null && valueId != ObjectId.Empty)
            {
                var Concrete = valueId.GetObject(link) as ExtendedDocument;
                if (Concrete != null)
                {
                    await DataHelpers.Shell.OpenScreenDetach(Concrete, Concrete.Name);
                    await Setup();
                }

            }

        }

        private void Listview_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
             
        }

        private async void Listview_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit)
                { 
                   // await Setup(500);
                }
            }
            catch 
            {
                Console.WriteLine("Listview_RowEditEnding");
            }
        }

        private void Listview_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, false);
            }
            catch (Exception s)
            {
                DataHelpers.Logger.LogError(s);
            }
        }

     

        private async void BtnDOWN_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            var table = but.Tag as DataGrid;

            if (table != null)
            {
                var selected = table.SelectedItem;
                if (selected != null)
                {

                    var source = table.GetValue(DataGrid.ItemsSourceProperty) as IList;

                    var index = source.IndexOf(selected);
                    var removedItem = source[index];


                    if (index -1 >= 0 && (index - 1) <= source.Count)
                    {
                        source.RemoveAt(index);
                        source.Insert((index - 1), removedItem);
                    }

                    await Setup();
                }
            }
        }

        private async void BtnUP_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            var table = but.Tag as DataGrid;

            if(table != null)
            {
                var selected = table.SelectedItem;
                if (selected != null)
                {

                    var source = table.GetValue(DataGrid.ItemsSourceProperty) as IList;

                    var index = source.IndexOf(selected);
                    var removedItem = source[index];


                    if (index+1 > 0 && (index+1) < source.Count )
                    {
                        source.RemoveAt(index);
                        source.Insert((index + 1), removedItem);
                    }

                    await Setup(); 
                }
            }
        }

        public  void ShowProperties()
        {
            UserControl control = new UserControl();
            Window win = new Window();
            var scroll = new ScrollViewer();
            scroll.HorizontalAlignment = HorizontalAlignment.Stretch;
            scroll.VerticalAlignment = VerticalAlignment.Stretch;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            control.Background = Brushes.WhiteSmoke;
            // props with displayname
            var props = (model as ExtendedDocument).GetType().GetProperties().Where(a => 
            a.GetCustomAttribute(typeof(DisplayNameAttribute)) != null
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.Separation
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.BaseButton
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.Button
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.OpsButton
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.LienButton
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.ImageSide
            && (a.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute)?.FieldType != ModelFieldType.Image);

            var stack = new StackPanel() { Orientation = Orientation.Vertical };

            var stackLin = new StackPanel() { Orientation = Orientation.Horizontal };
            var line = new TextBlock()
            {
                Text = $"Nom de champ",
                Width = 200,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            var value = new TextBlock()
            {
                Text = $"Codes",
                Width = 200,
                
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Margin = new Thickness(5)
            };
            stackLin.Children.Add(line);
            stackLin.Children.Add(value);
             
            stack.Children.Add(stackLin);


            foreach (var item in props)
            {
                var dispaly = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

                 stackLin = new StackPanel() { Orientation = Orientation.Horizontal };

                 line = new TextBlock()
                {
                    Text = $"{dispaly.DisplayName}",
                    Width = 200,
                    FontSize = 14,
                    Margin = new Thickness(5)
                };

                var valueTb = new TextBox()
                {
                    Text = $"{item.Name}",
                    Width = 200,
                    FontSize = 14,
                    Margin = new Thickness(5)
                };
                stackLin.Children.Add(line);
                stackLin.Children.Add(valueTb);

                stack.Children.Add(stackLin);
            }
            stack.Margin = new Thickness(20);
            stack.HorizontalAlignment = HorizontalAlignment.Stretch;
            stack.VerticalAlignment = VerticalAlignment.Stretch;
            scroll.Content = stack;
            control.Content = scroll;
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.VerticalAlignment = VerticalAlignment.Stretch; 
                win.WindowState = WindowState.Normal;
            win.WindowStyle = WindowStyle.ToolWindow;
            win.Content = control;
            win.ShowDialog();
        }


        private void TbDevise_StylusButtonUp(object sender, StylusButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();

        }

        private void TbDevise_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();

        }

        private void TbDevise_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
           
        }

        private void TbDevise_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private async void Box_DropDownClosed(object sender, EventArgs e)
        {
            await Setup();
        }

        private async void Box_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
           // await Actualiser();
        }

        private void ItemBox_Checked(object sender, RoutedEventArgs e)
        {
            var box = (sender as CheckBox); 
            var data = box.Tag as  ArrayList;
            // { prop.Name, item 
            var propName = data[0].ToString();
            // item is ID of accesrule
            var item = data[1];

            if (box.IsChecked == true)
            {
                if (item != null)
                {
                    ((model as ExtendedDocument).GetType().GetProperty(propName).GetValue(model) as IList).Add(item);
                }
            }
            else
            {
                if (item != null)
                {
                    ((model as ExtendedDocument).GetType().GetProperty(propName).GetValue(model) as IList).Remove(item);
                }
            }
            

        }

        private async void Box_LostFocus1(object sender, RoutedEventArgs e)
        {
            // throw new NotImplementedException();
            //await Actualiser();
        }

        private async void BtnView_Click1(object sender, RoutedEventArgs e)
        {

            try
            {
                var dic = (ArrayList)(sender as Button).Tag;
                if (dic.Count < 3)
                {
                    MessageBox.Show("Selectionner une ligne à ajoutée, ou vérifier la déclaration");
                    return;
                }

                var data = dic;
                var s = data[0] as ComboBox;
                
                var type = Type.GetType(data[1].ToString());
                var docList = DataHelpers.Shell.OpenScreenFind(type, "Selectioner document");
                var doc = docList?.FirstOrDefault();
                (this.model as ExtendedDocument).GetType().GetProperty(dic[2].ToString()).SetValue(this.model, doc.Id);
                s.SelectedItem = doc.Id;
                await Setup();

            }
            catch (Exception s)
            {

                MessageQueue.Enqueue(s.Message);
                GlobalMsg = $"Erreur: {s.Message}";
                NotifyOfPropertyChange("GlobalMsg");
                return;
            }

        }

        private async void Checkbox_Click(object sender, RoutedEventArgs e)
        {
           // await Task.Delay(1000);
            await Setup();
        }

        private async void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                //TODO replace with actualiser
                 await Setup();
               
            }
           
        }

        private async void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            await Setup ();
        }

        private async void Tablebox_KeyUp(object sender, KeyEventArgs e)
        {
            if (isFreezed)
                return;

            if (e.Key == Key.Enter)
            {
                try
                {
                    var dic = (ArrayList)(sender as ComboBox).Tag;
                    if (tableModel == ObjectId.Empty || dic.Count < 3)
                    {
                        MessageBox.Show("Selectionner une ligne à ajoutée, ou vérifier la déclaration");
                        return;
                    }

                    var data = dic;
                    var s = data[0] as DataGrid;
                    await AddItemToTable(s, data[1].ToString(), data[2].ToString());
                    //var selected = s.GetValue(DataGrid.ItemsSourceProperty);

                    //var doc = DataHelpers.GetById(data[1].ToString(), tableModel);
                    //var mapped = doc.Map(data[2].ToString());


                    // Do AfterMap Function if exist

                    //(selected as IList).Add(mapped);

                    await Setup();
                }
                catch (Exception s)
                {

                    MessageQueue.Enqueue(s.Message);
                    GlobalMsg = $"Erreur: {s.Message}";
                    NotifyOfPropertyChange("GlobalMsg");
                    return;
                }
            }
            var text = (sender as ComboBox).Text;
            //(sender as ComboBox).IsDropDownOpen = true;

            (sender as ComboBox).FindChild<TextBox>("PART_EditableTextBox").CaretIndex = text.Length;
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView((sender as ComboBox).ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(text)) return true;
                else
                {
                    if (((ExtendedDocument)o).Name.ToLower().Contains(text.ToLower())) return true;
                    else return false;
                }
            });

            itemsViewOriginal.Refresh();
        }

        private void Tablebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Valeur : "+tableModel);
        }

     

        private void TbDevise_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
       
       
        public async void EditTemplate()
        {
            OvExport.OvPdfModelExport pdf = new OvExport.OvPdfModelExport(model.GetType(), model.GetType().Name, model);
            pdf.EditTemplate();
        }
    }
}