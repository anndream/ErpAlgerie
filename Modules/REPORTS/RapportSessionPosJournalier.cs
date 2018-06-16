using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.CRM;
using ErpAlgerie.Pages.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.REPORTS
{
    class RapportSessionPosJournalier : IOvReport
    {
        private SessionPos sessionPos;

        public RapportSessionPosJournalier(SessionPos sessionPos)
        {
            this.sessionPos = sessionPos;
        }

        public override string ReportName { get; set; } = "Rapport journalier";

        public override string[] GetHeaders()
        {
            return new string[] { "Données", "Taux", "DES", "", "" };
        }

        public override List<OvTreeItem> GetReport()
        {

            var factures = DS.db.GetAll<Facture>(a =>a.DocStatus == 1 && a.DateCreation >= sessionPos.DateSession && a.DateCreation < sessionPos.DateSession.AddDays(1)) as IEnumerable<Facture>;

            var result = new List<OvTreeItem>();

            var globalResult = new OvTreeItem();
            globalResult.CL1 = "Résumée";

            var globalResult_montant = new OvTreeItem();
            globalResult_montant.CL1 = "Total des ventes";
            var montantVente = factures.Sum(z => z.MontantGlobalTTC);
            globalResult_montant.CL2 = $"{montantVente.ToString("N")} DA";



            var globalResult_ouvertur = new OvTreeItem();
            globalResult_ouvertur.CL1 = "Montant d'ouverture";
            var montantOuverture = sessionPos.MontantInit;
            globalResult_ouvertur.CL2 = $"{montantOuverture.ToString("N")} DA";


            var globalResult_cloture = new OvTreeItem();
            globalResult_cloture.CL1 = "Montant de clôture déclaré";
            var montantCloture = sessionPos.MontantCloture;
            globalResult_cloture.CL2 = $"{montantCloture.ToString("N")} DA";


            var globalResult_clotureS = new OvTreeItem();
            globalResult_clotureS.CL1 = "Montant de clôture supposé";
            var montantClotureS = montantVente + montantOuverture;
            globalResult_clotureS.CL2 = $"{montantClotureS.ToString("N")} DA";


            var globalResult_benefice = new OvTreeItem();
            globalResult_benefice.CL1 = "Bénéfice Supposé";
            var montantBenefice = montantClotureS - montantOuverture;
            globalResult_benefice.CL2 = $"{montantBenefice.ToString("N")} DA";


            var globalResult_beneficeS = new OvTreeItem();
            globalResult_beneficeS.CL1 = "Bénéfice calculé manuel";
            var montantBeneficeS = montantCloture - montantOuverture;
            globalResult_beneficeS.CL2 = $"{montantBeneficeS.ToString("N")} DA";

            var globalResult_transactions = new OvTreeItem();
            var trans = factures.Count();
            globalResult_transactions.CL1 = "Tickets";
            globalResult_transactions.CL2 = $"{trans} ventes";

         


            var globalResult_repas = new OvTreeItem();
            var items = factures.Sum(z => z.ArticleFacture.Count());
            globalResult_repas.CL1 = "Repas / boissons";
            globalResult_repas.CL2 = $"{items}";


            globalResult.Children.Add(globalResult_ouvertur);
            globalResult.Children.Add(globalResult_montant);
            globalResult.Children.Add(globalResult_cloture);
            globalResult.Children.Add(globalResult_clotureS);
            globalResult.Children.Add(globalResult_benefice);
            globalResult.Children.Add(globalResult_beneficeS);
            
            globalResult.Children.Add(globalResult_transactions);
            globalResult.Children.Add(globalResult_repas);

            result.Add(globalResult);



            var Details = new OvTreeItem();
            Details.CL1 = "Détails";

            var repas = DS.db.GetAll<Article>(a => true) as IEnumerable<Article>;
            var lines = factures.SelectMany(z => z.ArticleFacture);

            var names = lines.Select(e => e.lArticle);
            foreach (var r in repas)
            {
                if (names.Contains(r.Id))
                {
                    var Details_item = new OvTreeItem();
                    Details_item.CL1 = r.Name;
                    Details_item.CL2 = $"{lines.Where(z => z.lArticle == r.Id).Sum(e => e.Qts)} ventes";
                    Details.Children.Add(Details_item);
                }
            }
            result.Add(Details);

            return result;

        }
    }
}
