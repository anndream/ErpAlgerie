using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Pages.Template;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Modules.CRM
{




    class TypeVariante : ModelBase<TypeVariante>
    {

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "Paramétres";
        public override string CollectionName { get; } = "Type variante";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "ContentCut";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "Libelle";




        [IsBold]
        [DisplayName("Libellé")]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [Required]
        public string Libelle { get; set; }

        //==========================================



    }

}