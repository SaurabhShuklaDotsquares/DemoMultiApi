using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Core.Extensions {
    public static class SelectListItemExtension {
        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, dynamic> value, dynamic selectedValue) {
            return items.ToSelectListItems(name, value, x => selectedValue.Contains((string)value(x)));
        }

        public static object GetPropValue(object src, string propName) {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items) {
            return items.ToSelectListItems(x => GetPropValue(x, "Name").ToString(), x => GetPropValue(x, "Id").ToString());

            //var name = typeof(T).GetProperty("Name");
            //var valProperty = typeof(T).GetProperty("Name");
            ////typeof(T).
            //ToSelectListItems()
            //return items.ToSelectListItems(items.Select(x=> x), items.Select(x => x));
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, bool> isSelected) {
            return items.ToSelectListItems(x => GetPropValue(x, "Name").ToString(), x => GetPropValue(x, "Id").ToString(), isSelected);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, bool> isSelected, Func<T, bool> isDisabled) {
            return items.ToSelectListItems(x => GetPropValue(x, "Name").ToString(), x => GetPropValue(x, "Id").ToString(), isSelected, isDisabled);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, dynamic> value) {
            return items.ToSelectListItems(name, value, null);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, dynamic> value, Func<T, bool> isSelected, Func<T, bool> isDisabled = null) {
            if (items == null) {
                return new List<SelectListItem>();
            }

            return items.Select(item => new SelectListItem {
                Text = name(item),
                Value = Convert.ToString(value(item)),
                Selected = isSelected != null && isSelected(item),
                Disabled = isDisabled != null && isDisabled(item)
            });
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, dynamic> value, IEnumerable<dynamic> selectedValues) {
            if (items == null) {
                return new List<SelectListItem>();
            }

            if (selectedValues == null) {
                selectedValues = new List<string>();
            }

            return items.ToMultiSelectListItems(name, value, x => selectedValues.Contains((string)value(x)));
        }

        public static IEnumerable<SelectListItem> ToMultiSelectListItems<T>(this IEnumerable<T> items, Func<T, string> name, Func<T, dynamic> value, Func<T, bool> isSelected) {
            if (items == null) {
                return new List<SelectListItem>();
            }

            return items.Select(item => new SelectListItem {
                Text = name(item),
                Value = (string)value(item),
                Selected = isSelected(item)
            });
        }

    }
}
