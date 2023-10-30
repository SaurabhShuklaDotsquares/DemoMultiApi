using EC.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace EC.Core.LIBS
{

    public static class Extensions
    {

        public static void AddOrReplace(this IDictionary<string, object> DICT, string key, object value)
        {
            if (DICT.ContainsKey(key))
                DICT[key] = value;
            else
                DICT.Add(key, value);
        }
        public static Nullable<T> ToNullable<T>(this object input)
           where T : struct
        {
            if (input == null)
                return null;
            if (input is Nullable<T> || input is T)
                return (Nullable<T>)input;
            else if (input is string)
                return (T)Convert.ChangeType(input, typeof(T));

            throw new InvalidCastException();
        }
        public static DateTime? ToDateTime(this string inputDateTime, string inputDateTimeFormat)
        {
            if (string.IsNullOrWhiteSpace(inputDateTime) || string.IsNullOrWhiteSpace(inputDateTimeFormat))
            {
                return null;
            }
            try
            {
                return DateTime.ParseExact(inputDateTime.Trim(), inputDateTimeFormat.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch { return null; }
        }

        public static DateTime? ToDateTime(this string str, bool isWithTime = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (DateTime?)null;

            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd h:mm:ss tt", "yyyy-M-dd h:mm:ss tt", "yyyy-MM-d h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/MM/dd  h:mm:ss tt", "yyyy/M/dd  h:mm:ss tt", "yyyy/MM/d  h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "yyyyMMdd", "d/M/yy" };
            //CultureInfo enGB = new CultureInfo("en-GB");

            if (isWithTime)
            {
                return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static DateTime ToEndDate(this DateTime date)
        {
            return date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public static string ToJson<T>(this T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }
                        );
            }
            catch { }
            return string.Empty;
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool IsMenuActive(this IHtmlHelper htmlHelper, string menuItemUrl)
        {
            var viewContext = htmlHelper.ViewContext;
            var currentPageUrl = viewContext.ViewData["ActiveMenu"] as string ?? viewContext.HttpContext.Request.Path;
            var currentSplitUrl = currentPageUrl.Split("/");
            var controllerCurrentUrl = currentSplitUrl[1];
            var menuItemSplitUrl = menuItemUrl.Split("/");
            var controllerMenuItemUrl = menuItemSplitUrl[0];
            var currentUrl = string.Empty;
            if (controllerMenuItemUrl == controllerCurrentUrl)
            {
                currentUrl = controllerMenuItemUrl;
                return currentPageUrl.StartsWith(controllerMenuItemUrl, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
            }
            // return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
            //return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
        }

        public static string IsActive(this IHtmlHelper html,
                                  string menuItemUrl)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            var menuItemSplitUrl = menuItemUrl.Split("/");
            var controllerMenuItemUrl = menuItemSplitUrl[0];
            // both must match
            var returnActive = controllerMenuItemUrl == routeControl.ToLower();
            return returnActive ? "active" : "";
        }

        public static string GetEnumDescription<TEnum>(this TEnum enumerationValue)
         where TEnum : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static string GetDescription(this String value, Type t)
        {
            if (t.IsEnum)
            {
                var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
                var values = from f
                    in fields
                             let attribute = Attribute.GetCustomAttribute(f, typeof(DisplayAttribute)) as DisplayAttribute
                             where attribute != null && attribute.ShortName == value
                             select f.GetValue(null);
                return values.First().ToString();
            }
            return "";
        }
        public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>(bool getDescription = false, List<TEnum> skipEnums = null)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException();

            var enumList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            if (skipEnums != null && skipEnums.Any())
            {
                enumList = enumList.Except(skipEnums);
            }

            return enumList.Select(c => new SelectListItem()
            {
                Text = getDescription ? c.GetEnumDescription() : c.ToString(),
                Value = Convert.ToInt32(c).ToString()
            });

        }

        public static string GetName(this Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.Name == value.ToString()).Select(fi => fi.Name).FirstOrDefault();
        }
        public static string ToUnique(this string fileName)
        {
            return $"{DateTime.Now.Ticks}{Path.GetExtension(fileName.ToLower())}";
        }

        public static string TrimLength(this string input, int length, bool Incomplete = true)
        {
            if (string.IsNullOrEmpty(input)) { return string.Empty; }
            return input.Length > length ? string.Concat(input.Substring(0, length), Incomplete ? "..." : "") : input;
        }

        #region enum Method

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        //In short this is generic method to get any type of attribute.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes.FirstOrDefault();//attributes.Length > 0 ? (T)attributes[0] : null;
        }

        // This method creates a specific call to the above method, requesting the
        // Display MetaData attribute.
        //e.g. [Display(Name = "Sunday")]
        public static string ToDisplayName(this Enum value)
        {
            var attribute = value.GetAttribute<DisplayAttribute>();

            return attribute == null ? value.ToString() : attribute.Name;
        }

        //public static string GetDisplayAttrValue(this Enum value, EnumDisplayAttribute enumDisplayAttribute)
        //{
        //    string returnValue = string.Empty;
        //    var attribute = value.GetAttribute<DisplayAttribute>();
        //    if (attribute != null)
        //    {
        //        switch (enumDisplayAttribute)
        //        {
        //            case EnumDisplayAttribute.GroupName:
        //                returnValue = attribute.GroupName;
        //                break;
        //            case EnumDisplayAttribute.Prompt:
        //                returnValue = attribute.Prompt;
        //                break;
        //            case EnumDisplayAttribute.Description:
        //                returnValue = attribute.Description;
        //                break;
        //            default:
        //                returnValue = attribute.Name;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        returnValue = value.ToString();
        //    }
        //    return returnValue;
        //}

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        //e.g. [Description("Day of week. Sunday")]
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }


        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host
               .AddressList
               .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        #endregion

        public static DateTime UnixSecondsToDateTime(this long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        public static DateTime ConvertUtcToAEST(DateTime dateTime)
        {
            TimeZoneInfo objTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, objTimeZoneInfo);
        }

        public static DateTime ConvertAESTToUTC(this DateTime AESTdateTime)
        {
            TimeZoneInfo objTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(AESTdateTime, objTimeZoneInfo);
        }

        public static bool IsValidEmail(string emailAddress)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }
        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        public static string ToTitleCase(this string title)
        {
            if (title == null)
                return null;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
