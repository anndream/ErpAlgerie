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

        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

 

        public override bool Submitable { get; set; } = false;
        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }

        [DisplayName("Adresse")]
        public override string Name
        {
            get
            {
                return AdresseName;
            }
            set => base.Name = value;
        }



        public Adresse()
        {

        }
        public override string CollectionName { get; } = "Adresses";

        
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Adresse")]
        public string AdresseName { get; set; }

         

    }
}
