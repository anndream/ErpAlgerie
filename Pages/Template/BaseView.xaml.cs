using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml.Linq;

namespace ErpAlgerie.Pages.Template
{
    /// <summary>
    /// Logique d'interaction pour BaseView.xaml
    /// </summary>
    public partial class BaseView : UserControl
    {
        public BaseView()
        {
            InitializeComponent();
            
        }

        

        public FixedDocumentSequence Document { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            datagrid.SelectAll();
        }
    }
}
