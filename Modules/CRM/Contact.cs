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
    class Contact : ModelBase<Contact>
    {
      



        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;

        public override void Validate()
        {
            base.Validate();
          //  base.ValidateUnique();

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

        public Contact()
        {

        }
        public override string CollectionName { get; } = "Contacts";

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Nom et prénom")]
        public string Designiation { get; set; }


        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Adresse")]
        public string Adresse { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Région")]
        public string Region { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Mobile")]
        public string TelMob { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("N° Téléphone")]
        public string TelFix { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("N° Fax")]
        public string TelFax { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [ShowInTable(true)]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("Site web")]
        public string Siteweb { get; set; }

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Infos compte")]
        public string sepcompte { get; set; }

        //[ShowInTableAttribute(true)]
        //[IsSourceAttribute("DomaineActivite")]
        //[DisplayName("Domaine d'activité")]
        //[ColumnAttribute(ModelFieldType.Lien, "DomaineActivite")]
        //public ObjectId? lDomaineActivite { get; set; } = ObjectId.Empty;

    }
}
