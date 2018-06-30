using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Pages.MassEdit
{

    class CustomProperty
    {
        public CustomProperty(PropertyInfo aProperty, string name)
        {
            this.aProperty = aProperty;
            Name = name;
        }

        public PropertyInfo aProperty { get; set; }
        public string Name { get; set; }


    }
    class MassEditViewModel : Screen, IDisposable
    {
        
        public BindableCollection<CustomProperty> Properties { get; set; }
        public bool ReplaceText { get; set; }
        public string ReplaceValue { get; set; }

        private CustomProperty _SelectedProp;
        public CustomProperty SelectedProp
        {
            get { return _SelectedProp; }
            set { _SelectedProp = value;
                if(value != null)
                {
                    if (value.aProperty.PropertyType.IsGenericType &&
                       value.aProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        HintTextBox = $"{value.aProperty.Name} - {value.aProperty.PropertyType.GetGenericArguments()[0]}";
                    }
                    else
                    {

                        HintTextBox = $"{value.aProperty.Name} - {value.aProperty.PropertyType.Name}";
                    }
                    NotifyOfPropertyChange("HintTextBox");
                }
            }
        }

        public string HintTextBox { get; set; } = "Valeur";
        public string SelectedValue { get; set; }
        public Type aType { get; set; }

        public List<object>  aDocuments { get; set; }
        public string DocCount { get {
                return $"{aDocuments?.Count}";
            } }

        public MassEditViewModel(Type aType, IEnumerable<object> aDocuments)
        {
            this.aDocuments = aDocuments.ToList();
            this.aType = aType; 
            var properties = aType.GetProperties();

            // find displayname
            List<CustomProperty> aCustomProp = new List<CustomProperty>();
            foreach (var item in properties)
            {
                var column = item.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                if (column == null || column.FieldType == ModelFieldType.Separation
                    || column.FieldType == ModelFieldType.BaseButton
                    || column.FieldType == ModelFieldType.Button
                    || column.FieldType == ModelFieldType.Image
                    || column.FieldType == ModelFieldType.ImageSide
                    || column.FieldType == ModelFieldType.OpsButton
                    || column.FieldType == ModelFieldType.Table
                    || column.FieldType == ModelFieldType.WeakTable
                    || column.FieldType == ModelFieldType.ReadOnly
                    || column.FieldType == ModelFieldType.LienButton)
                    continue;


                string name = item.Name; 
                var display = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                if(display != null)
                {
                    name = display.DisplayName;
                }
                aCustomProp.Add(new CustomProperty(item, name));
            }
            Properties = new BindableCollection<CustomProperty>();
            Properties.AddRange(aCustomProp);
            Properties.Refresh();
        }
         

        public void OkClick()
        {
            try
            {
                if(ReplaceText)
                {
                    if (string.IsNullOrEmpty(ReplaceValue))
                    {
                        MessageBox.Show("Saisir la valeur à remplacer");
                        return;
                    }



                }

                foreach (IModel item in aDocuments)
                {
                    var aType = SelectedProp.aProperty.PropertyType;
                    if (aType.IsGenericType &&
                        aType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        aType = aType.GetGenericArguments()[0];
                    }
                    TypeConverter typeConverter;
                    object propValue = SelectedValue;

                   


                    if (aType == typeof(ObjectId))
                    {
                        try { propValue = ObjectId.Parse(SelectedValue); } catch { }
                    }
                    else
                    {
                        try
                        {
                            typeConverter = TypeDescriptor.GetConverter(aType);
                            if (ReplaceText)
                            {
                                var origin = SelectedProp.aProperty.GetValue(item).ToString();
                                try
                                {
                                   var  edited = origin.Replace(ReplaceValue, SelectedValue);
                                   propValue = typeConverter.ConvertFromString(edited);

                                }
                                catch { throw; }
                            }
                            else
                            {
                                propValue = typeConverter.ConvertFromString(SelectedValue);
                            }
                        }
                        catch
                        {
                            propValue = SelectedValue;
                        }
                    }
                   
                        
                     
                        SelectedProp.aProperty.SetValue(item, propValue);
                        (item as ExtendedDocument).ForceIgniorValidatUnique = true;
                        item.Save();
                    
                }

                MessageBox.Show("Modifications enregistrées");
                return;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
                return;
            }
        }


        public void Dispose()
        {
        }
    }
}
