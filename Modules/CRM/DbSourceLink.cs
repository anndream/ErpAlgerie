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
    class DbSourceLink : ModelBase<DbSourceLink>
    {

        public override string ModuleName { get; set; } = "Configuration";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }

        public override string Name
        {
            get
            {
                return DbName;
            }
            set => base.Name = value;
        }

        public DbSourceLink()
        {

        }
        public override string CollectionName { get; } = "Sources DB";

        [IsBoldAttribute(false)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Libellé")]
        public string Libelle { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Lien (IP & N° Port)")]
        public string SourceIp { get; set; }


        [IsBoldAttribute(false)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Nom base des données")]
        public string DbName { get; set; }
    }
}
