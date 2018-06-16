using ErpAlgerie.Modules.Core.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpAlgerie.Modules.Core.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {

        public ModelFieldType FieldType { get; set; }
        public string Options { get; set; }


        public Type OptionsType { get; set; }
        public ColumnAttribute(ModelFieldType fieldType, string options)
        {
            FieldType = fieldType;
            Options = options;
        }

        public ColumnAttribute(ModelFieldType fieldType, Type _OptionsType)
        {
            FieldType = fieldType;
            OptionsType = _OptionsType;
            Options = OptionsType.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AfterMapMethodAttribute : Attribute
    {
        public AfterMapMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class myTypeAttribute : Attribute
    {
        public myTypeAttribute(Type type)
        {
            this.type = type;
        }

        public Type type { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class IsSourceAttribute : Attribute
    {
        public IsSourceAttribute(string source)
        {
            this.source = source;
        }

        public string source { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IsBoldAttribute : Attribute
    {
        public IsBoldAttribute(bool isBod = true)
        {
            IsBod = isBod;
        }

        public bool IsBod { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LongDescriptionAttribute : Attribute
    {
        public LongDescriptionAttribute(string text)
        {
            this.text = text;
        }

        public string text { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ShowInTableAttribute : Attribute
    {
        public ShowInTableAttribute(bool isShow = true)
        {
            IsShow = isShow;
        }

        public bool IsShow { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SetColumnAttribute : Attribute
    {
        public SetColumnAttribute(int column)
        {
            this.column = column;
        }

        public int column { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DontShowInDetailAttribute : Attribute
    {
        public DontShowInDetailAttribute()
        {
        }
        
    }
}
