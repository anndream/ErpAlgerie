using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Module;
using ErpAlgerie.Modules.CRM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateEngine.Docx;

namespace ErpAlgerie.Pages.Helpers
{
    class DocEngine
    {


        public static void Generate(ExtendedDocument model)
        {
            switch (model.CollectionName)
            {


                default:
                    DefaultSet(model);
                    break;
            }
        }


        //private static List<IContentItem> setHeader(List<IContentItem> fields)
        // {
        //     //var creche = ElvaSettings.getInstance();
        //     //var logo = creche.PhotoIndice;
        //     //var pathLogo = "";
        //     //if (!string.IsNullOrWhiteSpace(logo))
        //     //{
        //     //    pathLogo = Path.GetFullPath(logo);
        //     //}


        //     //try { fields.Add(new FieldContent("Date", $"{DateTime.Today.ToLongDateString()}")); } catch { }
        //     //    try
        //     //    { fields.Add(new FieldContent("Creche", $"{creche?.Societe}"));
        //     //}
        //     //catch { }
        //     //try
        //     //        { fields.Add(new FieldContent("AdresseCreche", $"{creche?.Adresse}"));
        //     //}
        //     //catch { }
        //     //try
        //     //            { fields.Add(new FieldContent("ContactCreche", $"{creche?.Contact}"));
        //     //}
        //     //catch { }
        //     //try
        //     //                { fields.Add(new ImageContent("Logo", File.ReadAllBytes(pathLogo))); }
        //     //catch { }

        //     //return fields;
        // }


        public static void DefaultSet(ExtendedDocument model)
        {

            //    var doc = $"templates/{model.Name}.docx";
            //    if (File.Exists($"templates/{model.CollectionName}.docx"))
            //    {
            //        File.Copy($"templates/{model.CollectionName}.docx", doc, true);

            //        var type = model.GetType();
            //        var properties = type.GetProperties();

            //        var fields = new List<IContentItem>();
            //        fields = setHeader(fields);

            //        PropertyInfo[] props = type.GetProperties();

            //        foreach (var item in props)
            //        { 
            //            fields.Add(new FieldContent(item.Name.ToLower(), item.GetValue(model)?.ToString() ?? "..."));
            //        }

            //        var valuesToFill = new Content(fields.ToArray());
            //        using (var outputDocument = new TemplateProcessor(doc)
            //          .SetRemoveContentControls(true).SetNoticeAboutErrors(false))
            //        {
            //            outputDocument.FillContent(valuesToFill);
            //            outputDocument.SaveChanges();
            //            StartDoc(doc); 

            //        }
            //    }
            //    else
            //    {
            //        var resul = MessageBox.Show($"Le document {model.Name} n'a pas un template, créer un template word avec le nom '{model.CollectionName}', Voulez-vous ouvrir les repertoir en question","Aucune template!",MessageBoxButton.YesNo);

            //       if(resul == MessageBoxResult.Yes)
            //        {
            //            Process.Start(@"templates");
            //        }
            //        return;
            //    }


            //}


            //private static void StartDoc(string apth)
            //{
            //    try
            //    {
            //        var p = Path.GetFullPath(apth);
            //        Process.Start(p);
            //    }
            //    catch (Exception s)
            //    {
            //        MessageBox.Show(s.Message);
            //    }
            //}
        }
    }
}
