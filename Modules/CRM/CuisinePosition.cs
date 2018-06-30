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
    class CuisinePosition : ModelBase<CuisinePosition>
    {


        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "PDV";
        public override string CollectionName { get; } = "Position cuisine";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "PrinterAlert";
        public override bool ShowInDesktop { get; set; } = true;

        public override string NameField { get; set; } = "Designiation";

        #endregion

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }



    }
}
