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
    public class AccesRule : ModelBase<AccesRule>
    {

        #region SETTINGS

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "APPLICATION";
        public override string CollectionName { get; } = "Régles d'autorisations";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "LockOpenOutline";
        public override bool ShowInDesktop { get; set; } = false;

        #endregion

        #region NAMING

        public override string Name { get { return Naming(); } set => base.Name = value; }


        #endregion
         

        public override void Validate()
        {
            base.Validate();
            //base.ValidateUnique();
        }
        public AccesRule()
        {
        }
       
         
        [ShowInTableAttribute(false)]
        [DisplayName("Module")]
        [ColumnAttribute(ModelFieldType.Lien, "ModuleErp")]
        [IsSourceAttribute("ModuleErp")]
        public ObjectId? Module { get; set; } = ObjectId.Empty;

        [SetColumn(2)]
        [ShowInTable()]
        [ColumnAttribute(ModelFieldType.Check, "Peut voir ?")]
        [DisplayName("Voir")]
        public bool Voir { get; set; } = true;

        [SetColumn(2)]
        [ShowInTable()]
        [ColumnAttribute(ModelFieldType.Check, "Peut Créer ?")]
        [DisplayName("Créer")]
        public bool Creer { get; set; } = true;

        [SetColumn(2)]
        [ShowInTable()]
        [ColumnAttribute(ModelFieldType.Check, "Peut modifier ?")]
        [DisplayName("Modifier")]
        public bool CanSave { get; set; } = true;

        [ColumnAttribute(ModelFieldType.Check, "Peut Supprimer ?")]
        [SetColumn(2)]
        [DisplayName("Supprimer")]
        [ShowInTable()]
        public bool Supprimer { get; set; }

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.Check, "Peut valider ?")]
        [DisplayName("Valider")]
        [ShowInTable()]
        public bool Valider { get; set; }

        [SetColumn(2)]
        [ColumnAttribute(ModelFieldType.Check, "Peut Annuler ?")]
        [DisplayName("Annuler")]
        [ShowInTable()]
        public bool CancelSubmit { get; set; }

        private string Naming()
        {
            var voir = Voir ? "Voir " : "";
            var valider = Valider ? "Valider " : "";
            var supp = Supprimer ? "Supp " : "";
            var cree = Creer ? "Créer " : "";
            return $"{Module.GetObject("ModuleErp")?.Name} ({voir}{valider}{supp}{cree})";
        }

    }
}