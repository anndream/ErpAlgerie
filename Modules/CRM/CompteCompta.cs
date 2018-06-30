using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.CRM
{
    class CompteCompta : ModelBase<CompteCompta>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "COMPTES";
        public override string CollectionName { get; } = "Comptes";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Attach;
        public override string IconName { get; set; } = "Sitemap";

        #endregion

        #region NAMING

        public override string NameField { get; set; } = "NomCompte";
          
        #endregion

        #region CONSTRUCTOR

        public CompteCompta()
        { 
        }

        #endregion

        #region VALIDATION

        public override void Validate()
        {
            base.Validate();
            base.ValidateUnique();
        }

        #endregion

        #region PROPERTIES

        [Required]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable]
        [IsBold]
        [DisplayName("Nom de compte")]
        public string NomCompte { get; set; }


        [Required]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable] 
        [DisplayName("Code de compte")]
        public string CodeCompte { get; set; }
         

        [DisplayName("Compte parent")]
        [ColumnAttribute(ModelFieldType.Lien, "CompteCompta")]
        public ObjectId? CompteParent { get; set; } = ObjectId.Empty;

        internal static ObjectId? GetCompte(string typeCompte)
        {

           return DS.db.GetOne<CompteCompta>(a => a.Name == typeCompte)?.Id;

        }
 



        #endregion

        #region ACTIONS



        #endregion

        #region LINKS


        #endregion

    }
}

