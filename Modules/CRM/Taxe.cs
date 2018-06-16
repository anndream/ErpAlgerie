using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class Taxe : ModelBase<Taxe>
    {
        [BsonIgnore]
        public override string ModuleName { get; set; } = "Stock";
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";



        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

            //   Nom
            if (string.IsNullOrWhiteSpace(Designiation))
                throw new Exception("Designiation est obligatoire");

        }
        public override string Name
        {
            get
            {
                return Designiation;
            }
            set => base.Name = value;
        }

        public Taxe()
        {

        }
        public override string CollectionName { get; } = "Taxes";

        internal override ExtendedDocument Map(string mappedClass)
        {
            switch (mappedClass)
            {
                case "TaxeLigne":
                    TaxeLigne la = new TaxeLigne()
                    {
                        Taxe = Id,
                        MontantHT = 0,
                        PreviousMnt = PreviousMntApply
                    };
                    return la;
                    break;
                default:
                    return null;
                    break;
            }
        }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }

        [ColumnAttribute(ModelFieldType.Numero, "")]        
        [ShowInTable(true)]
        [DisplayName("Taux")]
        public decimal TauxTva { get; set; }


        [ColumnAttribute(ModelFieldType.Check, "Taux basé sur le taux précédent")]
        [DisplayName("Appliquer sur le taux")]
        public bool PreviousMntApply { get; set; }


    }

}
