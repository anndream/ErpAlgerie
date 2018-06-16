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
    public class Role : ModelBase<Role>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "Configuration";
        public override string CollectionName { get; } = "Rôles";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "AccountMultiple";
        public override bool ShowInDesktop { get; set; } = true;

        #endregion

        public override string NameField { get; set; } = "Libelle";


        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public Role()
        {
        } 
      



        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Libellé")]
        public string Libelle { get; set; }


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Réles d'autorisations")]
        public string sepRegles { get; set; }

        [ColumnAttribute(ModelFieldType.Table, "AccesRule")]
        [ShowInTable(false)]
        [DisplayName("Régles d'autorisatios")]
        [myTypeAttribute(typeof(AccesRule))]
        public List<AccesRule> AccesRules { get; set; } = new List<AccesRule>();


    }
}