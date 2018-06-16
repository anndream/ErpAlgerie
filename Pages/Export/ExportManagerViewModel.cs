using ErpAlgerie.Modules.Core.Module;
using MahApps.Metro.Controls;
using OvExport;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ErpAlgerie.Pages.Export
{
    class ExportManagerViewModel : Screen, IDisposable
    {
        public ExportManagerViewModel(Type type, object item,string TYPE, bool UseDefault = false,bool UseHeader = true)
        {
            this.UseHeader = UseHeader;
            this.DefaultTemplate = UseDefault;
            this.type = type;
            this.Item = item;
            this.TYPE = TYPE;
            GetModels();
            NotifyOfPropertyChange("Model");
            NotifyOfPropertyChange("DefaultTemplate");


            if (!string.IsNullOrWhiteSpace((Item).MyModule()?.DefaultTemplateName))
            {
                Model = (Item).MyModule()?.DefaultTemplateName;
                DefaultTemplate = true;
                NotifyOfPropertyChange("Model");
                NotifyOfPropertyChange("DefaultTemplate");
                CurrentText = Model;
            }

            NotifyOfPropertyChange("Model");
            NotifyOfPropertyChange("DefaultTemplate");
        }

       





        public bool DefaultTemplate { get; set; } = true;
        public string TYPE { get; set; }
        public dynamic Item { get; set; }
        public Type type { get; set; }
        public HashSet<string> Models { get; set; } = new HashSet<string>();
        public string Model { get; set; }
        public OvPdfModelExport ovExport { get; set; }
        public string CurrentText { get; set; }
        public bool UseHeader { get; set; } = true;


        public void Create()
        {
            if(string.IsNullOrWhiteSpace(Model))
            {
                MessageBox.Show("Choisir un modéle");
                return;
            }

            if (DefaultTemplate)
            {
                var module = (Item).MyModule();
                module.DefaultTemplateName = Model;
                module.Save();
            }

           

            ovExport = new OvPdfModelExport(type,Model,Item);
            ovExport.UseHeader = this.UseHeader;
            string file = "";
            if(TYPE == "PDF")
                file= ovExport.GeneratePdf();

            if (TYPE == "WORD")
                file= ovExport.GenerateOffice();

            
            if (!string.IsNullOrWhiteSpace(file))
            {
                Thread.Sleep(2000);
                try
                {
                    Process.Start(file);
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                    throw;
                }
                try { this.RequestClose(); } catch { }
            }
            
           

        }

        private void GetModels()
        {
            var temp = (Item as ExtendedDocument).DefaultTemplate;
            var ModelName = type.Name;
            if (DefaultTemplate )
            {
                // use default
                Model = ModelName;
                Create();
                 
            }


           
            if (string.IsNullOrWhiteSpace(ModelName))
                return;

            var folder = $"templates/{ModelName }";
            if (!Directory.Exists(folder))
            {
                MessageBox.Show("Modéle introuvable!, Créer nouveau");
                OvPdfModelExport ov = new OvPdfModelExport(type);
                ov.EditTemplate(ModelName);
                return;
            }

            var models = Directory.EnumerateFiles(folder);
            foreach (var item in models)
            {
                var file = Path.GetFileNameWithoutExtension(item);
                Models.Add(file);
            }
          //  Models = new HashSet<string>(models.Select();
            NotifyOfPropertyChange("Models");
        }


        public void DeleteModel()
        {
            var folder = $"templates/{type.Name }";
            var path = Path.GetFullPath($"{folder}/{Model}");
            File.Delete(path);
            GetModels();
        }

        public void OpenModel()
        {

            if (string.IsNullOrWhiteSpace(Model))
            {
                MessageBox.Show("Selectionner template!");
                return;
            }

            ovExport = new OvPdfModelExport(type, Model, Item);
            ovExport.EditTemplate();
        }

        public void CreateModel()
        {
            if (string.IsNullOrWhiteSpace(CurrentText)  )
            {
                MessageBox.Show("Saisir nom template");
                return;
            }


            ovExport = new OvPdfModelExport(type, CurrentText, Item);
            ovExport.EditTemplate();

        }


        public void Dispose()
        {
           

        }
    }
}
