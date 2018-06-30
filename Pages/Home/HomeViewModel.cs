using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.CRM;
using MaterialDesignThemes.Wpf;
using Stylet;
using Stylet.Logging;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErpAlgerie.Pages.Home
{
    class HomeViewModel : Screen, IDisposable
    {

        ///public ScrollViewer MasterPanel { get; set; } = new ScrollViewer();
        public StackPanel MasterPanelStack { get; set; } = new StackPanel();
        public WrapPanel MasterPanel { get; set; }


        protected override async void OnActivate()
        {
            base.OnActivate();
            await Setup();
        }

        public HomeViewModel()
        {
        }

        private async Task Setup()
        {
            MasterPanel = new WrapPanel();
            MasterPanel.Margin = new Thickness(15);
             
            // Init master panel
            MasterPanelStack = new StackPanel();
            MasterPanelStack.Orientation = Orientation.Vertical;
            MasterPanelStack.VerticalAlignment = VerticalAlignment.Stretch;
            MasterPanelStack.HorizontalAlignment = HorizontalAlignment.Stretch;

            // get Groupe names
            var groupes = DataHelpers.Modules.Where(z => !string.IsNullOrWhiteSpace(z.GroupeModule)).Select(a => a.GroupeModule).Distinct();

            foreach (var group in groupes)
            {
                //HomeIcons = new WrapPanel();
                //HomeIcons.HorizontalAlignment = HorizontalAlignment.Stretch;
                var modules = DataHelpers.Modules.Where(a => a.EstAcceRapide && a.GroupeModule == group);

                //Add groupe name to stack
                if(modules.Any())
                    MasterPanelStack.Children.Add(new TextBlock() { Text = group, Foreground=Brushes.White });
                
                foreach (var item in modules)
                {

                    Button btn = new Button();
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                    //btn.Content = item.Libelle;
                    btn.Style = App.Current.FindResource("HomeButton") as Style;
                    btn.Tag = item;
                    btn.Click += Btn_Click;
                    btn.TouchDown += Btn_Click;
                    var ic = item.ModuleIcon;
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Vertical };
                    sp.HorizontalAlignment = HorizontalAlignment.Center;

                    if (!string.IsNullOrWhiteSpace(ic))
                    {
                        // add icon
                        PackIcon pi = new PackIcon();
                        try
                        {
                            pi.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), ic);
                        }
                        catch (Exception s)
                        {
                            DataHelpers.Logger.LogError(s.Message);
                            pi.Kind = PackIconKind.Exclamation;
                            DataHelpers.Logger.LogInfo($"Setting default Icon for {item.Libelle }");
                        }
                        pi.Width = 50;
                        pi.HorizontalAlignment = HorizontalAlignment.Center;
                        pi.Height = 50;
                        btn.Content = (pi);
                    }

                    sp.Children.Add(btn);
                    sp.Children.Add(new TextBlock() { Text = item.Libelle,Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center });
                    // btn.Content = ic;
                    MasterPanel.Children.Add(sp);
                   // MasterPanelStack.Children.Add(HomeIcons);
                }

               
            }
           


            NotifyOfPropertyChange("MasterPanel");
        }

        private async void Btn_Click(object sender, EventArgs e)
        {
            var modulerp = (sender as Button).Tag as ModuleErp;
             await DataHelpers.Shell.OpenModuleErp(modulerp); 
        }

        public void Dispose()
        {

        }
    }
}
