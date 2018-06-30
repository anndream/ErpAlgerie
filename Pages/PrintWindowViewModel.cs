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

        public  void SelectAll()
        {
            foreach (var item in checkBoxes)
            {
                item.IsChecked = true;
                NotifyOfPropertyChange("props");
            }
        }

        public void ImportantSelectAll()
        {

            var unwanted = new List<string>()
            {
                "_etag",
                "DocOpenMod",
                "Submitable",
                "CollectionName",
                "CreatedBy",
                "isLocal",
                "isHandled",
                "DocStatus",
                "Submitable",
                "StatusColor",
                "CloseEvent",
                "DefaultTemplate",
                "IsSelectedd",
                "ModuleName",
                "ShowInDesktop",
                "SubModule",
                "DocCardOne",
                "DocCardTow",
                "IconName",
                "ForceIgniorValidatUnique",
                "Index",
                "NameField",
                "_NameField",
                "IsInstance",
                "PropertyChangedDispatcher",
                "Version",


            };

            foreach (var item in checkBoxes.Where(a => !unwanted .Contains(a.Content)))
            {
                item.IsChecked = true;
                NotifyOfPropertyChange("props");
            }
        }
        public List<CheckBox> checkBoxes { get; set; } = new List<CheckBox>();
        public PrintWindowViewModel(IEnumerable<dynamic> items)
        {
            Items = items;
            props = new WrapPanel();

            ExtendedDocument one = items.FirstOrDefault() ;

            if(one != null)
            {
                checkBoxes = new List<CheckBox>();
                var pr = one.GetType().GetProperties();
                foreach (PropertyInfo item in pr)
                {
                    var name = item.Name;
                    var dispaly = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    var column = item.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;

                    if (column != null && (column.FieldType == ModelFieldType.Separation
                        || column.FieldType == ModelFieldType.BaseButton
                        || column.FieldType == ModelFieldType.Button
                        || column.FieldType == ModelFieldType.LienButton
                        || column.FieldType == ModelFieldType.OpsButton
                        || column.FieldType == ModelFieldType.Table
                        || column.FieldType == ModelFieldType.WeakTable 
                        ))
                        continue;

                    if (dispaly != null)
                        name = dispaly.DisplayName;

                    CheckBox check = new CheckBox();
                    check.Tag = item;
                    check.Content = name;
                    check.Margin = new Thickness(5);
                    check.Checked += Check_Checked;
                    check.Unchecked += Check_Unchecked;
                    checkBoxes.Add(check);
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
