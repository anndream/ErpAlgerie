using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class ModuleProperty : ModelBase<ModuleProperty>
    {


        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "Paramétrages";
        public override string CollectionName { get; } = "Properties";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "Settings";
        public override bool ShowInDesktop { get; set; } = false;

        public override string NameField { get; set; } = "_ProeprtyName";

        #endregion

        public override void Validate()
        {
            base.Validate();
     
        }




        [ColumnAttribute(ModelFieldType.Select, "TypeColumn")]
        [DisplayName("Type Column")]
        public string _TypeComlumn { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Property name")]
        public string _ProeprtyName { get; set; }
        
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Display option")]
        public string _DisplayOption { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Valeur initial")]
        public string _InitValue { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Description")]
        public string _LongDescrption { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Display name")]
        public string _DisplayName { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Type (string, decimal...)")]
        public string _ThisPropertyType { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("My Type")]
        public string _TypeProperty { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_IsBold")]
        public bool _IsBold { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_ShowInTable")]
        public bool _ShowInTable { get; set; }

        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_DontShowDetail")]
        public bool _DontShowDetail { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_IsRequred")]
        public bool _IsRequred { get; set; }



        [ColumnAttribute(ModelFieldType.Check, "")]
        [DisplayName("_IsIgnior")]
        public bool _IsIgnior { get; set; }

    }
}
