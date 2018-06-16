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
        [BsonIgnore]
        public override string ModuleName { get; set; } = "Stock";
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();

        }

        public override string Name { get => Designiation; set => base.Name = value; }
        public GroupeArticle()
        {

        }
        public override string CollectionName { get; } = "Groupe d'article";

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Designiation")]
        public string Designiation { get; set; }
    }
}
