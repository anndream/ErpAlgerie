using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{
    class ModuleBuilder : ModelBase<ModuleBuilder>
    {


        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "Paramétrages";
        public override string CollectionName { get; } = "Classes";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Settings";
        public override bool ShowInDesktop { get; set; } = false;

        public override string NameField { get; set; } = "_ClassName";

        #endregion

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Nom class")]
        public string _ClassName { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("_ModuleName")]
        public string _ModuleName { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("_IconName")]
        public string _IconName { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("_NameField")]
        public string _NameField { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("_CollectionName")]
        public string _CollectionName { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_ValidateUnique")]
        public bool _ValidateUnique { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_Submitable")]
        public bool _Submitable { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_Detach")]
        public bool _Detach { get; set; }

        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_ShowInDesktop")]
        public bool _ShowInDesktop { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Properties")]
        public string sepProps { get; set; }

        [ColumnAttribute(ModelFieldType.Table, "ModuleProperty")]
        [ShowInTable(false)]
        [DisplayName("Properties")]
        [myTypeAttribute(typeof(ModuleProperty))]
        public List<ModuleProperty> _ModuleProperty { get; set; } = new List<ModuleProperty>();


        [BsonIgnore]
        [ColumnAttribute(ModelFieldType.OpsButton, "Cree")]
        [DisplayName("Créer class")]
        public string CreeBtn { get; set; }


        private string BuildProperty(ModuleProperty property)
        {
            string result = $@"";

            result = addBold(property._IsBold, result);
            result = addDisplayName(property._DisplayName, result);
            result = addColumnAttribute(property._TypeComlumn,property._DisplayOption, result);
            result = addShowInTable(property._ShowInTable, result);
            result = addIsRequired(property._IsRequred, result);
            result = addIsIgnio(property._IsIgnior, result);
            result = addDontShowDetal(property._DontShowDetail, result);
            result = addLongDescription(!string.IsNullOrWhiteSpace(property._LongDescrption),property._LongDescrption, result);
            result = addmyType(!string.IsNullOrWhiteSpace(property._TypeProperty), property._TypeProperty, result);

            var initialValue = property._InitValue;

            switch (property._TypeComlumn)
            {

                case "Lien":
                    result += $"public ObjectId? {property._ProeprtyName} {{ get; set; }} = ObjectId.Empty;";
                    break;

                case "Text":
                    result += $" public string {property._ProeprtyName} {{ get; set; }}";
                    break;

                case "Table":
                    result += $"public List<{property._TypeProperty}> {property._ProeprtyName} {{ get; set; }} = new List<{property._TypeProperty}>();";
                    break;

                case "Devise":
                    result += $"public decimal {property._ProeprtyName} {{ get; set; }}";
                    break;

                case "Numero":
                    result += $"public decimal {property._ProeprtyName} {{ get; set; }}";
                    break;

                case "Separation":
                    result += $" public string sep__{property._ProeprtyName} {{ get; set; }}";
                    break;

                case "LienField":
                    result += $"public ObjectId? {property._ProeprtyName} {{ get; set; }} = ObjectId.Empty;";
                    break;

                case "Date":
                    result += $" public DateTime {property._ProeprtyName}  {{ get; set; }} ";
                    break;

                case "OpsButton":
                    result += $@" public string {property._ProeprtyName}  {{ get; set; }} 

private async void {property._DisplayOption}()
{{
                        
}}

";
                    break;

                default:
                    result += $" public string {property._ProeprtyName}  {{ get; set; }} ";
                    break;
            }

            return result + initialValue;
        }

        private string addBold(bool isBold,string entry)
        {
            if (isBold)
                entry += "[IsBold]\n";

            return entry;
        }
        private string addDisplayName(string dispaly, string entry)
        { 
            entry += $"[DisplayName(\"{dispaly}\")]\n";
            return entry;
        }
        private string addColumnAttribute(string fiels,string option, string entry)
        {
            entry += $"[ColumnAttribute(ModelFieldType.{fiels}, \"{option}\")]\n";
            return entry;
        }
        private string addShowInTable(bool showInTable, string entry)
        {
            if (showInTable)
                entry += $"[ShowInTable]\n";
            return entry;
        }
        private string addDontShowDetal(bool dont, string entry)
        {
            if (dont)
                entry += $"[DontShowInDetail]\n";
            return entry;
        }
        private string addIsRequired(bool isRequired, string entry)
        {
            if (isRequired)
                entry += $"[Required]\n";
            return entry;
        }
        private string addIsIgnio(bool isIgnior, string entry)
        {
            if (isIgnior)
                entry += $"[BsonIgnore]\n";
            return entry;
        }

        private string addLongDescription(bool add,string description, string entry)
        {
            if(add)
            entry += $"[LongDescription(@\"{description}\")]\n";
            return entry;
        }

        private string addmyType(bool addType,string type, string entry)
        {
            if(addType)
                entry += $"[myType(typeof({type}))]\n";
            return entry;
        }
        
        public async void Cree()
        {
            


            var currentPth = Directory.GetCurrentDirectory();
            var fileName = $"dbs/CsTemplate.cs";
            var txtLines = File.ReadAllText(fileName);   //Fill a list with the lines from the txt file.
            string atach = _Detach ? "Detach" : "Attach";


            string allProps = "";

            if(_ModuleProperty.Any())
            {

                foreach (var prop in _ModuleProperty)
                {

                    allProps+= $@"{BuildProperty(prop)}

//==========================================

";
                }

            }




            string classBase = $@"

  class {_ClassName} : ModelBase<{_ClassName}>
    {{

        public override bool Submitable {{ get; set; }} = {_Submitable.ToString().ToLower()};
        public override string ModuleName {{get; set;}} = ""{_ModuleName}"";
        public override string CollectionName {{get;}} = ""{_CollectionName}"";
        public override OpenMode DocOpenMod {{get; set;}} = OpenMode.{atach};
        public override string IconName {{get; set;}} = ""{_IconName}"";
        public override bool ShowInDesktop {{get; set;}} = {_ShowInDesktop.ToString().ToLower()};
        public override string NameField {{ get; set; }} = ""{_NameField}"";
   

";

            txtLines = txtLines.Replace("__data__", classBase);
            txtLines = txtLines.Replace("__props__", allProps);
            
            
            File.WriteAllText($"dbs/{ _ClassName}.cs", txtLines);




        }



    }
}
