using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ErpAlgerie.Pages.Template
{
    class GenericWindowViewModel : Screen, IDisposable
    {
        public GenericWindowViewModel(ContentControl main, string Title, string docName)
        {
            this.Title = Title;
            this.main = main;
            main.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            main.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            NotifyOfPropertyChange("main");
        }

        public ContentControl main { get; set; }
        public string Title { get; set; }
        public void Dispose()
        {
        }
    }
}
