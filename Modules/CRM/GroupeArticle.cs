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
    class GroupeArticle : ModelBase<GroupeArticle>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Groupe d'article";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Gears";
        public override bool ShowInDesktop { get; set; } = false;

        public override string NameField { get; set; } = "Designiation";

        #endregion

         

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }
         
        public GroupeArticle()
        {

        } 
        

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }
    }
}
