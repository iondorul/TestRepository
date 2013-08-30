using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Core.Utils
{
    public class ListItem
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public static IList<ListItem> FromEnum<T>()
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(typeof(T).ToString() + " is not an enum");

            Array values = Enum.GetValues(typeof(T));
            List<ListItem> items = new List<ListItem>(values.Length);

            foreach (var i in values) {
                items.Add(new ListItem {
                    Name = Enum.GetName(typeof(T), i),
                    Value = i.ToString()
                });
            }

            return items;
        }

        public static IList<ListItem> FromEnum(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException(type.ToString() + " is not an enum");

            Array values = Enum.GetValues(type);
            List<ListItem> items = new List<ListItem>(values.Length);

            foreach (var i in values) {
                items.Add(new ListItem {
                    Name = Enum.GetName(type, i),
                    Value = i.ToString()
                });
            }

            return items;
        }
    }
}
