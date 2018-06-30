using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class TaxeLigne : ModelBase<TaxeLigne>
    {
        public override bool Submitable { get; set; } = false;
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public TaxeLigne()
        {
        }
        public override string CollectionName { get; } = "Lignes taxes";
        public override string Name { get { return Taxe.GetObject("Taxe")?.Name; } set => base.Name = value; }

        [ShowInTableAttribute(false)]
        [DisplayName("Taxe")]
        [ColumnAttribute(ModelFieldType.Lien, "Taxe")]
        public ObjectId? Taxe { get; set; } = ObjectId.Empty;

        [ColumnAttribute(ModelFieldType.Check, "Appliquer sur taux précédent")]
        [ShowInTable(true)]
        [DisplayName("Taux de ligne précédente")]
        public bool PreviousMnt { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "{0} DA")]
        [ShowInTable(true)] 
        [DisplayName("Taux précédent")]
        public decimal TauxPrecedent { get; set; }

        [ColumnAttribute(ModelFieldType.Devise, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [DisplayName("Montant HT")]
        public decimal MontantHT { get; set; }

        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [ShowInTable(true)]
        [IsBoldAttribute(true)]
        [DisplayName("Montant taxe")]
        public decimal MontantTaxe
        {
            get
            {
                if(PreviousMnt)
                {
                    return ((Taxe.GetObject("Taxe")?.TauxTva * TauxPrecedent) / 100);
                }
                else
                {
                    return ((Taxe.GetObject("Taxe")?.TauxTva * MontantHT) / 100);
                }
               
            }
        }

        [ColumnAttribute(ModelFieldType.ReadOnly, "{0} DA")]
        [ShowInTable(true)]
        [IsBoldAttribute(false)]
        [DisplayName("Montant total")]
        public decimal MontantTotal
        {
            get
            {
                if (PreviousMnt)
                    return (TauxPrecedent + MontantTaxe);
                return (MontantHT + MontantTaxe);
            }
        }




    }
}