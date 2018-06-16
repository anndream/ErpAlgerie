using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.REPORTS;
using ErpAlgerie.Pages.Reports;
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




    class SessionPos : ModelBase<SessionPos>
    {

        public override bool Submitable { get; set; } = false;
        public override string ModuleName { get; set; } = "Paramétres";
        public override string CollectionName { get; } = "Session POS";
        public override OpenMode DocOpenMod { get; set; } = OpenMode.Detach;
        public override string IconName { get; set; } = "CalendarToday";
        public override bool ShowInDesktop { get; set; } = true;
        public override string NameField { get; set; } = "DateSession";




        [DisplayName("Date de session")]
        [ColumnAttribute(ModelFieldType.Date, "")]
        [ShowInTable]
        [Required]
        public DateTime DateSession { get; set; } = DateTime.Now;

        //==========================================

        [DisplayName("Montant initial")]
        [ColumnAttribute(ModelFieldType.Devise, "")]
        [Required]
        public decimal MontantInit { get; set; }

        [ShowInTable]
        [DisplayName("Montant Cloture")]
        [ColumnAttribute(ModelFieldType.Devise, "")]
        [Required]
        public decimal MontantCloture { get; set; }

        //==========================================

        [DisplayName("Les résultats")]
        [BsonIgnore]
        [Column(ModelFieldType.Separation, "Résultats")]
        public string sepResultat { get; set; }


        [DisplayName("Montant calculé supposé")]
        [Column(ModelFieldType.ReadOnly,"{0:C}")]
        public decimal? Supposed { get {
                var factures = DS.db.GetAll<Facture>(a => a.DocStatus ==1 && a.DateCreation>=this.DateSession && a.DateCreation < this.DateSession.AddDays(1)) as IEnumerable<Facture>;
                return factures?.Sum(a => a.MontantGlobalTTC) + MontantInit;
            } }


        #region ACTIONS

        //OpenReport

        [BsonIgnore]
        [ColumnAttribute(ModelFieldType.OpsButton, "GenererRapport")]
        [DisplayName("Rapport du jour")]
        public string GenererRapportBtn { get; set; }

        public void GenererRapport()
        {

            var repor = new ReportViewModel(new RapportSessionPosJournalier(this));
            repor.DisplayName = $"Rapport {this.DateSession.ToShortDateString()}";
            DataHelpers.Shell.Items.Add(repor);
            DataHelpers.Shell.ActivateItem(repor);
            this.CloseParent();
        }

        #endregion

    }

}