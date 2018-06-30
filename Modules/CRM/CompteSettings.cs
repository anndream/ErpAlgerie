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
    class CompteSettings : ModelBase<CompteSettings>
    {
        [BsonIgnore]
        public override string ModuleName { get; set; } = "COMPTES";
        public override bool IsInstance { get; set; } = true;

        public CompteSettings()
        {

        }
        public override string CollectionName { get; } = "Paramétres Comptes";
        public override string Name
        {
            get
            {
                return "Paramétres Comptes";
            }
            set => base.Name = value;
        }
        public static CompteSettings getInstance()
        {
            var instance = DS.db.Count<CompteSettings>(a => true);
            if (instance > 0)
            {
                return DS.db.GetOne<CompteSettings>(t => true);
            }
            else
            {
                DS.db.AddOne<CompteSettings>(new CompteSettings() { isLocal = false });
                return DS.db.GetOne<CompteSettings>(a => true);
            }
        }

        [DisplayName("Compte vente")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? CompteVente { get; set; } = ObjectId.Empty;

        [DisplayName("Compte débiteur")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? CompteDebiteur { get; set; } = ObjectId.Empty;

        
        [DisplayName("Compte paiement client")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? ComptePaiementClient { get; set; } = ObjectId.Empty;


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Entrepôts par default ")]
        public string sepEntrepot { get; set; }


        [ShowInTableAttribute(false)]
        [DisplayName("Stock sortie par default")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? StockSortie { get; set; } = ObjectId.Empty;

        [ShowInTableAttribute(false)]
        [DisplayName("Stock entrées par default")]
        [ColumnAttribute(ModelFieldType.Lien, "Stock")]
        public ObjectId? StockEntree { get; set; } = ObjectId.Empty;
    }
}
