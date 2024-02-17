using Magicube.Data.Abstractions.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Magicube.Data.Abstractions.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class DataItemsAttribute : Attribute {
        public Type         DataProvider        { get; set; }
        public string       DataProviderField   { get; set; }
        public bool         IsStaticDatProvider { get; set; }
        public ChoicesTypes ChoicesType         { get; set; } = ChoicesTypes.SELECT;

        public IEnumerable<DataItem> GetItems(PropertyComponentContext property) {
            if (property.Property.PropertyType.IsEnum) {
                var items = new List<DataItem>();
                MemberInfo[] enumItems = property.Property.PropertyType.GetMembers(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < enumItems.Length; i++) {
                    var text = enumItems[i].GetCustomAttribute<DisplayAttribute>()?.Name ?? enumItems[i].Name;
                    items.Add(new DataItem() { Value = i.ToString(), Text = text });
                }

                return items;
            } else {
                //var properties = explorer.Properties.Where(p => p.Metadata.PropertyName.Equals(DataSource));
                //if (properties.Count() == 1) {
                //    return properties.First().Model as IEnumerable<SelectListItem>;
                //}
                return Enumerable.Empty<DataItem>();
            }
        }
    }

    public class DataItem {
        public string Value { get; set; }
        public string Text  { get; set; }
    }

    public enum ChoicesTypes {
        RADIO,
        CHECKBOX,
        SELECT
    }
}
