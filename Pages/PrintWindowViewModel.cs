using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ICSharpCode.AvalonEdit.Document;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ErpAlgerie.Pages
{
    class PrintWindowViewModel : Screen, IDisposable
    {
        public string StatusText = "Titre du document";

        public WrapPanel props { get; set; }

        public List<PropertyInfo> ChoosedPrperties { get; set; } = new List<PropertyInfo>();

        public PrintWindowViewModel(IEnumerable<dynamic> items)
        {
            Items = items;
            props = new WrapPanel();

            ExtendedDocument one = items.FirstOrDefault() ;

            if(one != null)
            {
                var pr = one.GetType().GetProperties();
                foreach (PropertyInfo item in pr)
                {
                    var name = item.Name;
                    var dispaly = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    if (dispaly != null)
                        name = dispaly.DisplayName;

                    CheckBox check = new CheckBox();
                    check.Tag = item;
                    check.Content = name;
                    check.Checked += Check_Checked;
                    check.Unchecked += Check_Unchecked;
                    props.Children.Add(check);
                }

            }

            NotifyOfPropertyChange("props");




        }

        private void Check_Unchecked(object sender, RoutedEventArgs e)
        {
            var property = (sender as CheckBox).Tag as PropertyInfo;
            ChoosedPrperties.Remove(property);
        }

        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            var property = (sender as CheckBox).Tag as PropertyInfo;
            ChoosedPrperties.Add(property);
        }

        public IEnumerable<dynamic> Items { get; set; }
        public string Titre { get; set; } = "Rapport : ";

        public void Ok()
        {
            if (string.IsNullOrWhiteSpace(Titre))
            {
                MessageBox.Show("Insérer un titre");
                return;
            }
            PrintHelper.GenerateExcel(Items, ChoosedPrperties, Titre, out StatusText);
            Annuler();
        }


        public void Annuler()
        {
            RequestClose();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
