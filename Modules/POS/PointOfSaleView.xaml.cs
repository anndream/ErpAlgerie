using System;
using System.Collections.Generic;
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

namespace ErpAlgerie.Modules.POS
{
    /// <summary>
    /// Logique d'interaction pour PointOfSaleView.xaml
    /// </summary>
    public partial class PointOfSaleView : UserControl
    {
        public PointOfSaleView()
        {
            InitializeComponent();
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CatScroller.LineUp();
            CatScroller.LineUp();
            CatScroller.LineUp();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CatScroller.LineDown();
            CatScroller.LineDown();
            CatScroller.LineDown();
            CatScroller.LineDown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 25; i++)
            {
                ProductScroller.LineUp();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < 25; i++)
            {

                ProductScroller.LineDown();
            }

            
        }
    }
}
