using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.CRM;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ErpAlgerie.Modules.POS
{
     enum TypeBox
    {
        ARTICLE,
        VARIANTE
    }

    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
                return Path.GetFullPath(value?.ToString());
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

   // public delegate void BoxClicked();
    class ProductBoxViewModel : Screen , IDisposable
    {
        public TypeBox _TypeBox { get; set; }
        public event EventHandler clickEvent;
        public ProductBoxViewModel(Article product)
        {
            _TypeBox = TypeBox.ARTICLE;
            this.product = product;
        }

        public ProductBoxViewModel(Variante item)
        {
            _TypeBox = TypeBox.VARIANTE; 
            this.variante = item;
         }

        public async void BoxClicked()
        {
            EventHandler handler = clickEvent;
            if (handler != null)
            {
                 handler(this,EventArgs.Empty);
            }
        }

        public Visibility   HideImage { get {

                return (DataHelpers.PosSettings.HideImageRepas ? Visibility.Collapsed : Visibility.Visible);

            } }
        public Article product { get; set; }
        public Variante variante { get; set; }

        public string ImageRepas { get
            {

                switch (_TypeBox)
                {
                    case TypeBox.ARTICLE:
                        return this.product.ImageRepas;
                        break;
                    case TypeBox.VARIANTE:
                        return this.variante.ImageRepas;
                        break;
                    default:
                        return "";
                        break;
                }
            }
        }
         public string Name
        {
            get
            { 
                if (_TypeBox == TypeBox.ARTICLE)
                {
                    return product.Name;
                    //if (string.IsNullOrWhiteSpace(propInSetting))
                    //{
                    //    return product.Name;
                    //}
                    //else
                    //{
                    //    var nameProp = product.GetType().GetProperty(propInSetting);
                    //    if (nameProp != null)
                    //    {
                    //        return product.GetType().GetProperty(propInSetting)?.GetValue(product)?.ToString();
                    //    }
                    //    else
                    //    {
                    //        return product.Name;
                    //    }
                    //}
                }
                else if(_TypeBox == TypeBox.VARIANTE)
                {
                    return variante.Name;
                }

                return "NA";
            }
        }
        public async void EditArticle()
        {
            await DataHelpers.Shell.OpenScreenDetach(product, product.Name);
            NotifyOfPropertyChange("product");
        }
        public void Dispose()
        {
             
        }
    }

}
