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




    class MessageCommande : ModelBase<MessageCommande>
    {

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "POS";
        public override string CollectionName { get; } = "Messages Commande";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "CommentTextOutline";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "Message";



        [IsBold]
        [DisplayName("Message")]
        [ColumnAttribute(ModelFieldType.Text, "Message")]
        [Required]
        public string Message { get; set; }

        //==========================================



    }

}