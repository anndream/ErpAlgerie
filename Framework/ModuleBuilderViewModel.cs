using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErpAlgerie.Modules.Core.Module;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.IO;

namespace ErpAlgerie.Framework
{

    class Field
    {
        public string FullProperty { get; set; }
        public StackPanel contentOptions { get; set; } = new System.Windows.Controls.StackPanel();

        StackPanel stack = new StackPanel();
        private ObservableCollection<Field> original;

        public Field(ObservableCollection<Field> original)
        {
            this.original = original;
            stack.Orientation = Orientation.Horizontal;
            var btnDelete = new Button();
            btnDelete.Content = "X";
            btnDelete.Click += BtnDelete_Click;
            btnDelete.TouchDown += BtnDelete_Click;
            stack.Children.Add(btnDelete);


            var btnTop = new Button();
            btnTop.Content = "TOP";
            btnTop.Click += BtnTop_Click; ;
            btnTop.TouchDown += BtnTop_Click; ;
            stack.Children.Add(btnTop);


            var btnDown = new Button();
            btnDown.Content = "DOWN";
            btnDown.Click += BtnDown_Click;
            btnDown.TouchDown += BtnDown_Click;
            stack.Children.Add(btnDown);

            contentOptions.Children.Add(stack);
        }

        private void BtnDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var index = original.IndexOf(this);
                original.Move(index, ++index);
            }
            catch 
            {}
            
        }

        private void BtnTop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                var index = original.IndexOf(this);
                original.Move(index, --index);

            }
            catch 
            {}
        }

        private void BtnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            original.Remove(this);
        }
    }

    class ModuleBuilderViewModel : Screen, IDisposable
    {

        public string ClassName { get; set; } = "Nom de la class";
        public string ModuleName { get; set; }
        public string ColletionName { get; set; }
        public string Identifier { get; set; }

        public bool IsSubmite { get; set; }
        public bool IsDetach { get; set; }
        public bool IsValidateUnique { get; set; }

        // Property info
        public string PropertyName { get; set; }
        public string DispalyNameAtt { get; set; }
        public ModelFieldType ColumnAttrib { get; set; }
        public string TypeVaraible { get; set; }
        public string ColumnAttribOpt { get; set; } = "";
        public string MyTypeAttrib { get; set; } = "";
        public string MyInitValue { get; set; }


        public bool IsBold { get; set; }
        public bool showInTable { get; set; }
        public bool IsShowDetail { get; set; }
        public bool BsonIgnore { get; set; }

        public ObservableCollection<Field> Fields { get; set; } = new ObservableCollection<Field>();

        public IEnumerable<ModelFieldType> ListDataTypes { get
            {
                return Enum.GetValues(typeof(ModelFieldType)).Cast<ModelFieldType>();
            }
        }

        public List<string> TypeVariables { get
            {
                return new List<string>()
                {
                    "string",
                    "bool",
                    "decimal",
                    "ObjectId",
                    "DateTime",
                    "List<"
                };
            }
        }


        public string GetIdentifier()
        {
            if(IsEmpty(Identifier) == false)
            {
                return $@" public override string Name
        {{
            get
           {{
                return {Identifier};
            }}
        }}
";
            }
            else
            {
                return "";
            }
        }
        public string _(bool value)
        {
           return value ? "true" : "false";
        }
        public void Generate()
        {

            /*
             *   [ShowInTableAttribute(false)]
        [ColumnAttribute(ModelFieldType.Table, "LigneFacture")]
        [DisplayName("Produits")]
        [myTypeAttribute(typeof(Article))]
        public List<LigneFacture> ArticleFacture { get; set; } = new List<LigneFacture>();
*/

            var property_init = MyInitValue;
            var property = $@"public {TypeVaraible} {PropertyName}"+ "{ get; set; } "+ property_init;

            var dispalyname = $"[DisplayName(\"{DispalyNameAtt}\")]";
            var column = $"[ColumnAttribute(ModelFieldType.{ColumnAttrib}, \"{ColumnAttribOpt}\")]";
            var isBold = $"[IsBoldAttribute({_(IsBold)})]";
            var showInTabel = $"[ShowInTable({_(showInTable)})]";
            var typeatrrib = "";
            if (!string.IsNullOrWhiteSpace(MyTypeAttrib))
                typeatrrib = $"[myTypeAttribute(typeof({MyTypeAttrib}))]";
            var dontshowdetail = IsShowDetail ? $"[DontShowInDetailAttribute]" : "";
            var field = new Field(Fields);

            field.FullProperty = $@"
{typeatrrib}
{dontshowdetail}
{isBold}
{showInTabel}
{column}
{dispalyname}
{property}
";

            Fields.Add(field);
            NotifyOfPropertyChange("Fields");
            NotifyOfPropertyChange("Fields");

        }


        public bool IsEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        private string GetIsDetach()
        {
            return IsDetach ? "OpenMode.Detach" : "OpenMode.Attach";
        }

        public string GetValidateUnqiue()
        {
            return (IsValidateUnique ? "  base.ValidateUnique();" : "") ;

        }

        public void ExportClass()
        {

            var baseClass = $@"
// Auto generated class

using ErpAlgerie.Modules.Core.Data;
using ErpAlgerie.Modules.Core.Helpers;
using ErpAlgerie.Modules.Core.Module;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace ErpAlgerie.Modules.CRM
{{
    class {ClassName} : ModelBase<{ClassName}>
    {{

        public override OpenMode DocOpenMod {{ get; set; }} = { GetIsDetach() };


         [BsonIgnore]
        public override string ModuleName {{ get; set; }} = ""{ModuleName}"";
        public override bool Submitable {{ get; set; }} = {_(IsSubmite)};
        public override void Validate()
        {{
            base.Validate();
            {GetValidateUnqiue()}

        }}

         {GetIdentifier()}
       

        public {ClassName}()
        {{

        }}
        public override string CollectionName {{ get; set; }} = ""{ ClassName }"";


        {
            string.Join("\n", Fields.Select(a => a.FullProperty))
        }

    }}

        
    }}";



            var s = File.Create(ClassName + ".cs");
            File.WriteAllText($"../{ClassName}.cs", baseClass);


        }







        public void Dispose()
        {
        }
    }
}
