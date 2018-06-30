using ErpAlgerie.Modules.CRM;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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


    public class OvTreeItem
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
        private IEnumerable<OvTreeItem> lines;

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

        public void PrintReport()
        {

            var posPrinter = PosSettings.getInstance().POSPrinter;
            if (string.IsNullOrEmpty(posPrinter))
            {
                MessageBox.Show("Imprimante introuvable, vérifier les paramétres");
                return;
            }

            File.WriteAllText("report.txt", $"{this.Report.ReportName}\n");
            
            using (var file = new StreamWriter("report.txt", true))
            {
                var typeRepor = MessageBox.Show("Inclure les quantités vendues", "Type rapport", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(typeRepor == MessageBoxResult.Yes)
                {
                     lines = Report.GetReport().SelectMany(z => z.Children);
                }
                else
                {
                    lines = Report.GetReport().Where(z => !z.CL1.Contains("Détails")).SelectMany(z => z.Children);
                }
              
                foreach (var item in lines)
                {
                    file.WriteLine($"{item.CL1}\n{item.CL2} {item.CL3}\n___________________");

                }
                file.Close();
                Process p = null;
                try
                {
                    
                    p = new Process();
                    p.StartInfo.FileName = Path.GetFullPath("report.txt");

                    var verbs = p.StartInfo.Verbs;
                    foreach (var v in verbs)
                    {
                        Console.WriteLine(v);
                    }
                    p.StartInfo.Verb = "Print";
                    p.StartInfo.Arguments = "\"" + posPrinter + "\"";
                    p.StartInfo.Verb = "Printto";
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                    p.WaitForExit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }


        }


        public void Dispose()
        {

        }
    }
}
