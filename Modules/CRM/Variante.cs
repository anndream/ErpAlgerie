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




    class Variante : ModelBase<Variante>
    {

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "STOCK";
        public override string CollectionName { get; } = "Variante";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "ContentCut";
        public override bool ShowInDesktop { get; set; } = false;
        public override string NameField { get; set; } = "Valeur";




        [IsBold]
        [DisplayName("Valeur")]
        [ColumnAttribute(ModelFieldType.Text, "")]
        [ShowInTable]
        [Required]
        public string Valeur { get; set; }

        //==========================================

        [DisplayName("Type de variante")]
        [ColumnAttribute(ModelFieldType.Lien, "TypeVariante")]
        [Required]
        public ObjectId? TypeVariante { get; set; } = ObjectId.Empty;


        //==========================================

        [ColumnAttribute(ModelFieldType.ImageSide, "Image")]
        [ShowInTableAttribute(false)]
        [DisplayName("Image")]
        public string ImageRepas { get; set; }

        [ShowInTable]
            [DontShowInDetail]
            [BsonIgnore]
            [DisplayName("Variante")]
            public string NomVariante { get {
                return TypeVariante.GetObject("TypeVariante")?.Name;
            } }

    }

}