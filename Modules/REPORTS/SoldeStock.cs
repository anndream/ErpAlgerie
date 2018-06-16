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
    class SoldeStock : IOvReport
    {
        public override string ReportName { get; set; } = "Qts de stock";

        public override string[] GetHeaders()
        {
            return new string[] { "ARTICLE","QTS","DES","",""};
        }

        public override List<OvTreeItem> GetReport()
        {
            var articles = DataHelpers.GetMongoDataSync("Article") as IEnumerable<Article>;
            var result = new List<OvTreeItem>();

            var withm = articles.Where(a => a.Designiation.Contains("m"));
            var withp = articles.Where(a => a.Designiation.Contains("p"));
            var child = new List<OvTreeItem>();
            var childP = new List<OvTreeItem>();
            var node = new OvTreeItem();

           

            foreach (var item in withp)
            {
                childP.Add(new OvTreeItem() { CL1 = item.Designiation, CL2 = item.PrixVente + "" });
            }

            foreach (var item in withm)
            {
                child.Add(new OvTreeItem() { CL1 = item.Designiation, CL2 = item.PrixVente + "" });
            }



            child.Add(new OvTreeItem()
            {
                CL1 = "P",
                Children = childP
            });

            result.Add(new OvTreeItem()
                {
                CL1 = "M",
                Children = child
            });

            
            return result;

        }
    }
}
