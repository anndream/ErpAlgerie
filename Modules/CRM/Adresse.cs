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
    class Adresse : ModelBase<Adresse>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "CRM";
        public override string CollectionName { get; } = "Adresses";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "MapMarker";
        public override bool ShowInDesktop { get; set; } = false;

        public override string NameField { get; set; } = "AdresseName";

        #endregion
          
        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }

       


        public Adresse()
        {

        } 

        
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Adresse")]
        public string AdresseName { get; set; }

         

    }
}
