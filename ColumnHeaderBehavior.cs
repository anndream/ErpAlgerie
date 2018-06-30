using ErpAlgerie;
using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity; 

namespace AttributtedDataColumn
{
    public class ColumnHeaderBehavior : Behavior<DataGrid>
    {
          
        public static string GetPropertyDisplayName(object descriptor)
        {
            PropertyDescriptor pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                DisplayNameAttribute attr = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if ((attr != null) && (attr != DisplayNameAttribute.Default))
                {
                    return attr.DisplayName;
                }
            }
            else
            {
                PropertyInfo pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attrs = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    foreach (var att in attrs)
                    {
                        DisplayNameAttribute attribute = att as DisplayNameAttribute;
                        if ((attribute != null) && (attribute != DisplayNameAttribute.Default))
                        {
                            return attribute.DisplayName;
                        }
                    }
                }
            }
            return null;
        }

        public static bool GetPropertyIsBold(object descriptor)
        {
            PropertyDescriptor pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                IsBoldAttribute attr = pd.Attributes[typeof(IsBoldAttribute)] as IsBoldAttribute;
                if ((attr != null))
                {
                    return attr.IsBod;
                }
            }
            else
            {
                PropertyInfo pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attrs = pi.GetCustomAttributes(typeof(IsBoldAttribute), true);
                    foreach (var att in attrs)
                    {
                        IsBoldAttribute attribute = att as IsBoldAttribute;
                        if ((attribute != null))
                        {
                            return attribute.IsBod;
                        }
                    }
                }
            }
            return false;
        }

        public static bool GetPropertyIsShowTable(object descriptor)
        {
            PropertyDescriptor pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                ShowInTableAttribute attr = pd.Attributes[typeof(ShowInTableAttribute)] as ShowInTableAttribute;
                if ((attr != null))
                {
                    return attr.IsShow;
                }
            }
            else
            {
                PropertyInfo pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attrs = pi.GetCustomAttributes(typeof(ShowInTableAttribute), true);
                    foreach (var att in attrs)
                    {
                        ShowInTableAttribute attribute = att as ShowInTableAttribute;
                        if ((attribute != null))
                        {
                            return attribute.IsShow;
                        }
                    }
                }
            }
            return false;
        }

        static ObjectIdConv objectIdConv = new ObjectIdConv();


        public static void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Style style1 = new Style()
            //{
            //    TargetType = typeof(DataGridCell)
            //};
            //style1.Setters.Add(new Setter(property: DataGridCell.FontSizeProperty, value: 20));
            //e.Column.CellStyle = style1;
           // if (e.Column.DisplayIndex == 0)
              
            string displayName = GetPropertyDisplayName(e.PropertyDescriptor);
            bool isShow = GetPropertyIsShowTable(e.PropertyDescriptor);
            var columnAttr = (e.PropertyDescriptor as PropertyDescriptor)?.Attributes[typeof(ColumnAttribute)] as ColumnAttribute;
            bool estDevise = false;
            if (columnAttr != null) {

                if (columnAttr?.FieldType == ModelFieldType.Devise
                || (columnAttr.FieldType == ModelFieldType.ReadOnly && columnAttr.Options == "{0} DA"))
                estDevise = true;

                
            }

             
           

            if (!string.IsNullOrEmpty(displayName) && isShow)
            {

                // IF IS EDITABLE DETAIL TABLE
                if ((sender as DataGrid).Name != "datagrid")
                {


                  if(  e.PropertyType == typeof(ObjectId?))
                    {
                        e.Cancel = true;
                    }

                    if (e.Column.DependencyObjectType.SystemType == typeof(DataGridCheckBoxColumn)
                     || e.PropertyType == typeof(ObjectId)
                        || e.PropertyType == typeof(ObjectId?)
                        || displayName == "Réf")
                    {
                        e.Column.IsReadOnly = true;
                    }
                    else
                    {
                        e.Column.IsReadOnly = false;
                    }
                }

                IsSourceAttribute attrisSource = (e.PropertyDescriptor as PropertyDescriptor).Attributes[typeof(IsSourceAttribute)] as IsSourceAttribute;

                // Treat column as combobox with source
                if (attrisSource != null)
                {
                    if (!String.IsNullOrWhiteSpace(attrisSource.source))
                    {
                        var cb = new DataGridComboBoxColumn();
                        cb.ItemsSource = DataHelpers.GetMongoDataSync(attrisSource.source);
                        cb.DisplayMemberPath = $"Name";

                        cb.SelectedValuePath = "Id";
                        cb.SelectedValueBinding = new Binding($"l{attrisSource.source}");
                        e.Column = cb;
                    }
                }
                if (estDevise)
                {
                    DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                    if (dataGridTextColumn != null)
                    { 
                        dataGridTextColumn.Binding.StringFormat = "{0} DA";
                    }
                }
                if (e.PropertyType == typeof(DateTime))
                {
                    DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                    if (dataGridTextColumn != null)
                    {
                        dataGridTextColumn.Binding.StringFormat = "d";
                    }
                }
                e.Column.Header = displayName;
                e.Column.Width = DataGridLength.Auto;
                if (displayName == "Crée le" && (sender as DataGrid).Name != "datagrid")
                    e.Cancel = true;
                if (  displayName == "Status") //|| displayName == "Crée le"
                {
                    if ((sender as DataGrid).Name == "datagrid")
                    {
                        Style style = new Style()
                        {
                            TargetType = typeof(DataGridCell)
                        };
                       // style.Setters.Add(new Setter(property: DataGridCell.ForegroundProperty, value: Brushes.Black));
                        style.Setters.Add(new Setter(property: DataGridCell.MarginProperty, value: new Thickness(10, 2, 2, 2)));
                        style.Setters.Add(new Setter(property: DataGridCell.BackgroundProperty, value: new Binding { Converter = new BackColorConv() }));
       
                        e.Column.CellStyle = style;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }

                if ((sender as DataGrid).Name == "datagrid" )
                {
                    if (displayName == "Réf")
                    {
                        e.Column.DisplayIndex = 0;
                        Style style = new Style()
                        {
                            TargetType = typeof(DataGridCell),
                            BasedOn = App.Current.FindResource("MaterialDesignDataGridCell") as Style
                        };
                        // style.Setters.Add(new Setter(property: DataGridCell.MarginProperty, value: new Thickness(0)));
                        style.Setters.Add(new Setter(property: DataGridCell.VerticalContentAlignmentProperty, value: VerticalAlignment.Center));
                        style.Setters.Add(new Setter(property: DataGridCell.MinWidthProperty, value: ((double)150)));

                        // style.Setters.Add(new Setter(property: DataGridCell.PaddingProperty, value: new Thickness(10)));
                        style.Setters.Add(new Setter(property: DataGridCell.FontWeightProperty, value: FontWeights.SemiBold));
                        style.Setters.Add(new Setter(property: DataGridCell.VerticalAlignmentProperty, value: VerticalAlignment.Center));
                        e.Column.CellStyle = style; 
                    }

                    if (columnAttr != null && columnAttr.FieldType == ModelFieldType.Lien)
                    {
                        var c = ((e.Column as DataGridTextColumn).Binding as Binding);

                        c.ConverterParameter = columnAttr.Options;
                        c.Converter = objectIdConv;
                    }

                }

                if (displayName == "Retard")
                {
                    //Style style = new Style()
                    //{
                    //    TargetType = typeof(DataGridCell)
                    //};
                    //style.Setters.Add(new Setter(property: DataGridCell.ForegroundProperty, value: Brushes.Black));
                    //style.Setters.Add(new Setter(property: DataGridCell.MarginProperty, value: new Thickness(10, 2, 2, 2)));
                    //style.Setters.Add(new Setter(property: DataGridCell.BackgroundProperty, value: new Binding { Converter = new BackFicheAnimalColorConv() }));

                    //e.Column.CellStyle = style;
                }

                //var bold = GetPropertyIsBold(e.PropertyDescriptor);
                //if ((sender as DataGrid).Name == "datagrid" && bold)
                //{
                //    Style style = new Style()
                //    {
                //        TargetType = typeof(DataGridCell)
                //    };

                //    style.Setters.Add(new Setter(property: DataGridCell.FontWeightProperty, value: FontWeights.Bold));
                //    style.Setters.Add(new Setter(property: DataGridCell.VerticalAlignmentProperty, value: VerticalAlignment.Center));
                //    //style.Setters.Add(new Setter(property: DataGridCell.MarginProperty, value: new Thickness(10, 2, 2, 2)));
                //    e.Column.CellStyle = style;
                //}


            }
            else
            {
                e.Cancel = true;
            }

           
        }

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }


        private class ObjectIdConv : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    var v = (ObjectId)value;
                    var param = parameter.ToString();
                    return v.GetObject(param)?.Name;
                }
                catch
                {
                    return value;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
                // throw new NotImplementedException();
            }
        }


        private class BackColorConv : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    var v = (ExtendedDocument)value;                    
                    return v.StatusColor;
                }
                catch
                {
                    return Brushes.Black;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
               // throw new NotImplementedException();
            }
        }

        private class BackFicheAnimalColorConv : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    //var v = (Prestation)value;
                    //if (v.Retard.Equals("En Retard"))
                    //{
                    //    return Brushes.Orange;
                    //}
                    //else if (v.Retard == "Payé")
                    //{
                    //    return Brushes.LightGreen;
                    //}
                    //else if (v.Retard == "Programmé")
                    //{
                    //    return Brushes.White;
                    //}
                  
                    
                    return Brushes.WhiteSmoke;
                }
                catch
                {
                    return Brushes.Black;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                //  throw new NotImplementedException();
                return value;
            }
        }
    }
}