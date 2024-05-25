using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Attributes
{
    public static class UIInputMetadataReader
    {
        public static IEnumerable<(string PropertyName, string PropertyType, UIInputType Type, string Label)> GetUIInputElements(Type type)
        {
            return type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(UIInputAttribute)))
                .Select(prop =>
                {
                    UIInputAttribute? attr = (UIInputAttribute?)prop.GetCustomAttribute(typeof(UIInputAttribute));
                    UIInputType type = attr?.Type == UIInputType.Null ? InferType(prop) : UIInputType.Null;
                    string label = attr?.LabelKey ?? prop.Name;
                    return (prop.Name, prop.PropertyType.Name, type, label);
                });
        }
        private static UIInputType InferType(PropertyInfo prop)
        {
            switch (Type.GetTypeCode(prop.PropertyType))
            {
                case TypeCode.String:
                    return UIInputType.TextBox;
                case TypeCode.Int32:
                    return UIInputType.Integer;
                case TypeCode.DateTime:
                    return UIInputType.Date;
                case TypeCode.Double:
                    return UIInputType.Double;
                default:
                    return UIInputType.Null;
            }
        }
    }
}
