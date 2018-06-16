using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Helpers
{
    public class ModelTemplate : ModelBase<ModelTemplate>
    {
        
        [BsonIgnore]
        public override string SubModule { get; set; } = "Articles et prix";
        [BsonIgnore]
        public override string IconName { get; set; } = "Gears";

        [BsonIgnore]
        public override string ModuleName { get; set; } = "Produits";
        public ModelTemplate()
        {

        }
        public override string CollectionName { get; } = "ModelTemplate";



        #region PROPERTIES
        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTableAttribute(true)]
        [DisplayName("Nom")]
        public string Nom { get; set; }
        public override string Name
        {
            get
            {
                return Nom;
            }
        }

       #endregion



    }
}
