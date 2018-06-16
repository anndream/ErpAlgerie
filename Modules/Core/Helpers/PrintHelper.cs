
using DoddleReport;
using DoddleReport.Writers;
using ErpAlgerie.Modules.Core.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ErpAlgerie.Modules.Core.Helpers
{
   public static class  PrintHelper
    {
        static  BackgroundWorker bw;
        private static Report report;

        public static void GenerateExcel(IEnumerable<dynamic> list,string titre, out string StatusText)
        {
            StatusText = "...";
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            try
            {
                // Get the data for the report (any IEnumerable will work) 
                var query = list.ToList();

            // Create the report and turn our query into a ReportSource 
            report = new DoddleReport.Report(query.ToReportSource());

            // Customize the Text Fields report.TextFields.Title = "Products Report";
            report.TextFields.SubTitle = titre;
            report.TextFields.Footer = "Copyright 2018 © www.ovresko.com - Bouziane Khaled (0665 97 76 79)";

            // Render hints allow you to pass additional hints to the reports as they are being rendered 
            report.RenderHints.BooleanCheckboxes = false;
            //report.RenderHints.Orientation = ReportOrientation.Landscape;

            // Customize the data fields report.DataFields["Id"].Hidden = true;
            var sample = list.ElementAt(0);
            if(sample != null)
            {
                var properties = sample.GetType().GetProperties();
                foreach (PropertyInfo item in properties)
                {

                    try
                    {
                        //report.DataFields[item.Name].Hidden = true;

                        //var showInTable = item.GetCustomAttribute(typeof(ShowInTableAttribute)) as ShowInTableAttribute;
                        //var display = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                        //if (display != null && display.DisplayName != "Crée le" && display.DisplayName != "Status" && showInTable?.IsShow == true)
                        //{
                        //    report.DataFields[item.Name].Hidden = false;
                        //}

                          //  report.DataFields[item.Name].Hidden = false;

                        }
                    catch 
                    {
                        continue;
                    }

                }
            }


                // bw.RunWorkerAsync();
                try
                {
                    using (FileStream fs = new FileStream("export.xls", FileMode.Create))
                    {
                        var writer = new ExcelReportWriter();
                        writer.WriteReport(report, fs);

                    }

                    Process.Start("export.xls");
                }
                catch (Exception s)
                {
                    System.Windows.MessageBox.Show(s.Message);
                }

            }
            catch (Exception s)
            {
                System.Windows.MessageBox.Show(s.Message);
            }
        }

        public static void GenerateExcel(IEnumerable<dynamic> items, List<PropertyInfo> choosedPrperties, string titre, out string statusText)
        {
            statusText = "...";
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            try
            {
                // Get the data for the report (any IEnumerable will work) 
                var query = items.ToList();

                // Create the report and turn our query into a ReportSource 
                report = new DoddleReport.Report(query.ToReportSource());

                // Customize the Text Fields report.TextFields.Title = "Products Report";
                report.TextFields.SubTitle = titre;
                report.TextFields.Footer = "Copyright 2018 © www.ovresko.com - Bouziane Khaled (0665 97 76 79)";

                // Render hints allow you to pass additional hints to the reports as they are being rendered 
                report.RenderHints.BooleanCheckboxes = false;
                //report.RenderHints.Orientation = ReportOrientation.Landscape;

                // Customize the data fields report.DataFields["Id"].Hidden = true;
                var sample = items.ElementAt(0);
                if (sample != null)
                {
                    var properties = sample.GetType().GetProperties();
                    foreach (PropertyInfo item in properties)
                    {

                        try
                        {
                            report.DataFields[item.Name].Hidden = true;


                            if (choosedPrperties.Contains(item))
                                report.DataFields[item.Name].Hidden = false;
                            //var showInTable = item.GetCustomAttribute(typeof(ShowInTableAttribute)) as ShowInTableAttribute;
                            //var display = item.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                            //if (display != null && display.DisplayName != "Crée le" && display.DisplayName != "Status" && showInTable?.IsShow == true)
                            //{
                            //    report.DataFields[item.Name].Hidden = false;
                            //}

                            //  report.DataFields[item.Name].Hidden = false;

                        }
                        catch
                        {
                            continue;
                        }

                    }
                }


                // bw.RunWorkerAsync();
                try
                {
                    using (FileStream fs = new FileStream("export.xls", FileMode.Create))
                    {
                        var writer = new ExcelReportWriter();
                        writer.WriteReport(report, fs);

                    }

                    Process.Start("export.xls");
                }
                catch (Exception s)
                {
                    System.Windows.MessageBox.Show(s.Message);
                }

            }
            catch (Exception s)
            {
                System.Windows.MessageBox.Show(s.Message);
            }
        }

        private static void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream("export.xls", FileMode.Create))
                {
                    var writer = new ExcelReportWriter();
                    writer.WriteReport(report, fs);

                }

                Process.Start("export.xls");
            }
            catch (Exception s)
            {
                System.Windows.MessageBox.Show(s.Message);
            }
        }
    }
}
