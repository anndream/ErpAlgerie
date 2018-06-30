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
    public class User : ModelBase<User>
    {
        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "APPLICATION";
        public override string CollectionName { get; } = "Utilisateurs";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "AccountKey";
        public override bool ShowInDesktop { get; set; } = true;

        public override string NameField { get; set; } = "Libelle";

        #endregion

         
        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }
        public User()
        {
        }
        
        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(true)]
        [ShowInTable(true)]
        [DisplayName("Nom et prénom")]
        public string Libelle { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")] 
        [DisplayName("Fonction")]
        public string Fonction { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [DisplayName("E-Mail")]
        public string Email { get; set; }

       


        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Rôles")]
        public string sepRoles { get; set; }

        [ColumnAttribute(ModelFieldType.WeakTable, "Role")]
        [DisplayName("Rôles")]
        public List<ObjectId> Roles { get; set; } = new List<ObjectId>();

        [ColumnAttribute(ModelFieldType.Separation, "")]
        [BsonIgnore]
        [DisplayName("Autorisations")]
        public string seppermissions { get; set; }

        [ColumnAttribute(ModelFieldType.Check, "Est administrateur?")]
        [DisplayName("Admin systéme")]
        public bool IsAdmin { get; set; }

        [ColumnAttribute(ModelFieldType.Text, "")]
        [IsBoldAttribute(false)]
        [ShowInTable(false)]
        [DisplayName("Mots de passe")]
        public string Password { get; set; }

    }
}