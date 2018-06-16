using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ErpAlgerie.Pages.Reports
{


    public abstract class IOvReport
    {
       public abstract List<OvTreeItem> GetReport();
       public abstract string[] GetHeaders();
        public abstract string ReportName { get; set; }
        public virtual string BG { get; set; } = "White";

    }


    public  class  OvTreeItem
    {
        public List<OvTreeItem> Children { get; set; } = new List<OvTreeItem>();

        public string CL1 { get; set; }
        public string CL2 { get; set; }
        public string CL3 { get; set; }
        public string CL4 { get; set; }
        public string CL5 { get; set; }
    }

    class ReportViewModel : Screen, IDisposable
    {
        public ReportViewModel(IOvReport report)
        {
            Report = report;
        }

        public string CL1 { get { return Report.GetHeaders()[0]; } }
        public string CL2 { get { return Report.GetHeaders()[1]; } }
        public string CL3 { get { return Report.GetHeaders()[2]; } }
        public string CL4 { get { return Report.GetHeaders()[3]; } }
        public string CL5 { get { return Report.GetHeaders()[4]; } }
        public string BG { get { return Report.BG; } }

        public List<OvTreeItem> Items
        {
            get
            {
                return Report.GetReport();
            }
        }


        public IOvReport Report { get; set; }

 

        public void refresh()
        {
           
            NotifyOfPropertyChange("Items");
            //Items.Add(new OvTreeItem()
            //{
            //    CL1 = "Hello",
            //    CL2 = "Red",
            //    CL3 = "cl3",
            //    Children = new List<OvTreeItem>()
            //{
            //    new OvTreeItem(){CL1 = "Hello",CL2 ="Red" },
            //    new OvTreeItem(){CL1 = "Hello",CL3 ="Red" }

            // }
            //});
            NotifyOfPropertyChange("Items");
        }
        public void Dispose()
        {
           
        }
    }
}
