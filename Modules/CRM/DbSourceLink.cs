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
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "APPLICATION";
        public override string CollectionName { get; } = "Sources DB";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "Plus";
        public override bool ShowInDesktop { get; set; } = true;

        public override string NameField { get; set; } = "DbName";
         

        #endregion
         

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }

       

        public DbSourceLink()
        {

        } 

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
