namespace Utilities.String
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    #endregion

    public static class StringUtils
    {
        /// <summary>
        /// Returns string if it is null or empty, otherwise returns another specified string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="otherStr"></param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string str, string otherStr)
        {
            return string.IsNullOrEmpty(str) ? otherStr : str;
        }

        /// <summary>
        /// Detects if string is null or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Detects if string is null or whitespace
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Converts string to camel-case formatted string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str, Action<CamelCasePropertyNamesContractResolver> config = null)
        {
            var camelCase = new CamelCasePropertyNamesContractResolver();
            config?.Invoke(camelCase);
            return camelCase.GetResolvedPropertyName(str);
        }

        /// <summary>
        /// Returns split by spaces string 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> SplitBySpaces(this string str)
        {
            return string.IsNullOrEmpty(str) ? new List<string>() : str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Returns string that contains digits from specified string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExtractDigits(this string str)
        {
            return new string(str.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Checks if string contains other specified string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Returns split by camel-case string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string input)
        {
            return Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
        }

        static void CompactWhitespaces(StringBuilder sb)
        {
            if (sb.Length == 0)
                return;

            // set [start] to first not-whitespace char or to sb.Length

            var start = 0;

            while (start < sb.Length)
                if (Char.IsWhiteSpace(sb[start]))
                    start++;
                else
                    break;

            // if [sb] has only whitespaces, then return empty string

            if (start == sb.Length)
            {
                sb.Length = 0;
                return;
            }

            // set [end] to last not-whitespace char

            var end = sb.Length - 1;

            while (end >= 0)
                if (Char.IsWhiteSpace(sb[end]))
                    end--;
                else
                    break;

            // compact string

            var dest = 0;
            var previousIsWhitespace = false;

            for (var i = start; i <= end; i++)
                if (Char.IsWhiteSpace(sb[i]))
                {
                    if (!previousIsWhitespace)
                    {
                        previousIsWhitespace = true;
                        sb[dest] = ' ';
                        dest++;
                    }
                }
                else
                {
                    previousIsWhitespace = false;
                    sb[dest] = sb[i];
                    dest++;
                }

            sb.Length = dest;
        }

        static string CompactWhitespaces(this string s)
        {
            var sb = new StringBuilder(s);

            CompactWhitespaces(sb);

            return sb.ToString();
        }

        /// <summary>
        /// Compute the distance between two strings
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Compute(string s, string t)
        {
            s = CompactWhitespaces(s);
            t = t.CompactWhitespaces();

            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;

            if (m == 0)
                return n;

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++) { }

            for (var j = 0; j <= m; d[0, j] = j++) { }

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                                       Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                                       d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m] * 100 / Math.Max(n, m);
        }

        /// <summary>
        /// Detects if JSON is valid
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]")))   //For array
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            return false;
        }

        /// <summary>
        /// Trims nullable string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimOrDefault(this string str)
        {
            return str?.Trim();
        }

        /// <summary>
        /// Returns int list from split by specified separator string 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<int> ToListInt(this string str, char separator)
        {
            foreach (var source in str.Split(separator).ToList())
            {
                int id;
                if (int.TryParse(source, out id))
                    yield return id;
            }
        }

        /// <summary>
        /// Detects if string contains value only once
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static bool ContainsOnce(this string str, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var firstIndexOf = str.IndexOf(value, stringComparison);
            if (firstIndexOf == -1)
                return false;

            var lastIndexOf = str.LastIndexOf(value, stringComparison);
            return firstIndexOf == lastIndexOf;
        }

        /// <summary>
        /// Converts List of strings to List of property-value pairs
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<List<KeyValuePair<PropertyInfo, string>>> ToPropertyValuePairs(this List<string> conditions, Type type)
        {
            const string separator = "==";
            return conditions
                   .Where(x => x.Contains(separator))
                   .Select(x =>
                           {
                               var oneRow = x.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                               return oneRow.Select(c =>
                                                    {
                                                        var condition = c.Split(new[] { separator }, StringSplitOptions.None);

                                                        var propertyName = condition.ElementAtOrDefault(0);
                                                        var property = type.GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
                                                        var value = string.Join(separator, condition.Skip(1));
                                                        return new KeyValuePair<PropertyInfo, string>(property, value);
                                                    })
                                            .Where(c => c.Key != null)
                                            .ToList();
                           })
                   .Where(x => x.Any())
                   .ToList();
        }

        /// <summary>
        /// Converts string to GUID
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid StringToGUID(string value)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }
    }
}