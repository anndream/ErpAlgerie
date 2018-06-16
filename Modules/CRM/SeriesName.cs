
// Auto generated class

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
   public class SeriesName : ModelBase<SeriesName>
    {

        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;


        [BsonIgnore]
        public override string ModuleName { get; set; } = "Stock";
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";


        public override bool Submitable { get; set; } = false;
        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }

        public override string Name
        {
            get
            {
                return Libelle;
            }
            set => base.Name = value;
        }



        public SeriesName()
        {

        }
        public override string CollectionName { get; } = "SeriesName";
        

        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Libellé")]
        public string Libelle { get; set; }
        

        [IsBoldAttribute(false)]
        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [LongDescription("MM: mois - AA: année - JJ:jours")]
        [DisplayName("Suffix")]
        public string Sufix { get; set; }

        [ColumnAttribute(ModelFieldType.Numero, "")]
        [ShowInTable(true)]
        [DisplayName("Indexe")]
        public long Indexe { get; set; }
    }


}