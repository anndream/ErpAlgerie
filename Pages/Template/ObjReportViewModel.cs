using ErpAlgerie.Modules.Core.Module;
using SimpleWPFReporting;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErpAlgerie.Pages.Template
{
    class ObjReportViewModel : Screen, IDisposable
    {

        public StackPanel panel { get; set; } = new StackPanel();
        public ExtendedDocument Doc { get; set; }

        public ObjReportViewModel(ExtendedDocument doc)
        {
            this.Doc = doc;
            SetView();
        }

        public void SetView()
        {

            WrapPanel wrap = new WrapPanel();
            wrap.Orientation = Orientation.Horizontal;


            panel.Background = Brushes.White;
            panel.Orientation = Orientation.Vertical;
            panel.Children.Add(new Label()
            {
                Content = Doc.CollectionName,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center
            ,
                FontSize = 40
            });

          
            //StackPanel sp = new StackPanel();
            //sp.Orientation = Orientation.Vertical;

            //// var box = new StackPanel() { Orientation = Orientation.vert };
            //sp.Children.Add(new Label() { Content = $"#{Doc.Name}" , FontSize = 20 });
            //sp.Children.Add(new Label() { Content = $"Créer le : {Doc.AddedAtUtc.ToShortDateString()}" });
            //sp.Children.Add(new Label() { Content = $"Status : {Doc.Status}" });

            var data = Doc.GetType().GetProperties();
            foreach (var item in data)
            {
                var name = item.Name;
                var values = item.GetValue(Doc)?.ToString();

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;

                // var box = new StackPanel() { Orientation = Orientation.vert };
                sp.Children.Add(new Label() { Content = $"{name}", FontSize = 20 });
                sp.Children.Add(new Label() { Content = $"{values}" });

                wrap.Children.Add(sp);

            }
            
            panel.Children.Add(wrap);

        }

        public void print()
        {
        
            Report.ExportReportAsPdf(panel,null,ReportOrientation.Portrait);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
