using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenOrm.Extensions
{
    public static partial class Extensions
    {
        #region Case Convert
        /// <summary>
        /// http://csharphelper.com/blog/2014/10/convert-between-pascal-case-camel-case-and-proper-case-in-c/
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string str)
        {
            // If there are 0 or 1 characters, just return the string.
            if (str == null) return str;
            if (str.Length < 2) return str.ToUpper();

            // Split the string into words.
            string[] words = str.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// http://csharphelper.com/blog/2014/10/convert-between-pascal-case-camel-case-and-proper-case-in-c/
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            // If there are 0 or 1 characters, just return the string.
            if (str == null || str.Length < 2)
                return str;

            // Split the string into words.
            string[] words = str.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        /// <summary>
        /// http://csharphelper.com/blog/2014/10/convert-between-pascal-case-camel-case-and-proper-case-in-c/
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToProperCase(this string str)
        {
            // If there are 0 or 1 characters, just return the string.
            if (str == null) return str;
            if (str.Length < 2) return str.ToUpper();

            // Start with the first character.
            string result = str.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i])) result += " ";
                result += str[i];
            }

            return result;
        }

        public static string LowercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);

            return new string(a);
        }

        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
        #endregion

        //public static string PadNums(this string str, int padLength = 10)
        //{
        //    return Regex.Replace(str, "[0-9]+", match => match.Value.PadLeft(padLength, '0'));
        //}
        public static string PadNumbers(this string str, int padLength = 10)
        {
            return Regex.Replace(str, "[0-9]+", match => match.Value.PadLeft(padLength, '0'));
        }

        public static List<string> SplitOnLineBreaks(this string x, bool RemoveEmptyEntries = true)
        {
            if (RemoveEmptyEntries) return x.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            else return x.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        }

        public static string Ellipsis(this string text, int length)
        {
            if (text.Length <= length) return text;
            int pos = text.IndexOf(" ", length);
            if (pos >= 0)
                return text.Substring(0, pos) + "...";
            return text;
        }

        public static bool ContainsOr(this string s, params string[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (s.Contains(p[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EqualsOr(this string s, params string[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (s == p[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAnd(this string s, params string[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (!s.Contains(p[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool StartsWithOr(this string s, params string[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (s.StartsWith(p[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EndsWithOr(this string s, params string[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (s.EndsWith(p[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static double ToDouble(this string x)
        {
            double result;
            if (double.TryParse(x, out result))
            {
                return result;
            }
            return 0;
        }

        public static decimal ToDecimal(this string x)
        {
            decimal result;
            if (decimal.TryParse(x, out result))
            {
                return result;
            }
            return 0;
        }

        public static int ToInt(this string x)
        {
            int result;
            if (int.TryParse(x, out result))
            {
                return result;
            }
            return 0;
        }

        public static long ToInt64(this string x)
        {
            long result;
            if (long.TryParse(x, out result))
            {
                return result;
            }
            return 0;
        }

        public static bool ToBool(this string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            if (s.ToLower().EqualsOr("1", "true", "on", "checked")) return true;
            return false;
        }

        public static bool IsNoE(this string x)
        {
            return string.IsNullOrEmpty(x.Replace(" ", ""));
        }

        /// <summary>
        /// Retire les accents d'une chaine
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string x)
        {
            if (string.IsNullOrWhiteSpace(x))
                return x;

            x = x.Normalize(NormalizationForm.FormD);
            var chars = x.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        public static string HashPassword(this string x)
        {
            var data = Encoding.UTF8.GetBytes(x);
            string hash = "";

            using (SHA256 shaM = new SHA256Managed())
            {
                var hashedInputBytes = shaM.ComputeHash(data);
                hash = string.Join("", hashedInputBytes.Select(y => y.ToString("X2")).ToList());
            }

            data = Encoding.UTF8.GetBytes(hash);

            using (SHA512 shaM = new SHA512Managed())
            {
                var hashedInputBytes = shaM.ComputeHash(data);
                hash = string.Join("", hashedInputBytes.Select(y => y.ToString("X2")).ToList());
            }
            return hash;
        }

        public static string Encrypt(this string text, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(this string crypted, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(crypted);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        //https://stackoverflow.com/questions/25134897/gzip-compression-and-decompression-in-c-sharp
        public static string Compress(this string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    gs.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        //https://stackoverflow.com/questions/25134897/gzip-compression-and-decompression-in-c-sharp
        public static string Decompress(this string text)
        {
            byte[] compressed = Convert.FromBase64String(text);
            // Read the last 4 bytes to get the length
            byte[] lengthBuffer = new byte[4];
            Array.Copy(compressed, compressed.Length - 4, lengthBuffer, 0, 4);
            int uncompressedSize = BitConverter.ToInt32(lengthBuffer, 0);

            var buffer = new byte[uncompressedSize];
            using (var ms = new MemoryStream(compressed))
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    gzip.Read(buffer, 0, uncompressedSize);
                }
            }
            return Encoding.Unicode.GetString(buffer);
        }

        public static bool IsNumeric(this string x)
        {
            x = x.Replace("$", "").Replace("€", "").Replace("£", "").Replace(" ", "");
            if (string.IsNullOrEmpty(x))
                return false;
            double number;
            return double.TryParse(Convert.ToString(x, CultureInfo.InvariantCulture), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }

        public static bool IsValidEmail(this string str)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            }
            catch
            {
                return false;
            }
        }

        public static int CompareVersions(this string current, string other)
        {
            if (string.IsNullOrEmpty(current)) return 0;
            if (current == other) return 0;
            if (!string.IsNullOrEmpty(current) && string.IsNullOrEmpty(other)) return 1;
            if (string.IsNullOrEmpty(current) && !string.IsNullOrEmpty(other)) return -1;
            List<string> versions = new List<string>(2) { current, other };
            versions = versions.OrderBy(x => x.Trim().PadNumbers()).ToList();
            if (versions[0] == current) return -1;
            return 1;
        }

        public static string GetStringBetween(this string current, string firstPart, string secondPart)
        {
            if (string.IsNullOrEmpty(current)) return "";
            if (!current.Contains(firstPart)) return "";
            string tmp = current.Substring(current.IndexOf(firstPart) + firstPart.Length);
            if (!tmp.Contains(secondPart)) return tmp;
            return tmp.Substring(0, tmp.IndexOf(secondPart));
        }

        public static string SetStringBetween(this string current, string before, string after, string value)
        {
            if (string.IsNullOrEmpty(current)) return "";
            if (!current.Contains(before)) return current;
            int indexBefore = current.IndexOf(before) + before.Length;
            string tmp = current.Substring(indexBefore);
            if (!tmp.Contains(after)) return current;

            int indexAfter = tmp.IndexOf(after);
            //string between = tmp.Substring(0, indexSecondPart);
            string firstPart = current.Substring(0, indexBefore);
            string secondPart = current.Substring(indexAfter);

            return firstPart + value + secondPart;
        }

        public static List<string> Chunk(this string str, int maxChunkSize)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < str.Length; i += maxChunkSize)
                result.Add(str.Substring(i, Math.Min(maxChunkSize, str.Length - i)));

            return result;
        }

        #region Path
        public static string GetRoot(this string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            string root = "";
            string tmp = path;
            if (path.Contains(@":\"))
            {
                string[] splitted = tmp.Split(new[] { @":\" }, System.StringSplitOptions.RemoveEmptyEntries);
                root = splitted[0];
                tmp = splitted[1];
            }
            if (tmp.Contains(@"\"))
            {
                string[] splitted = tmp.Split(new[] { @"\" }, System.StringSplitOptions.RemoveEmptyEntries);
                root += splitted[0];
                tmp = tmp.Substring(tmp.IndexOf(@"\", 1) + 1);
            }

            return root;
        }

        public static string RemoveRoot(this string path)
        {
            string root = "";
            string tmp = path;
            if (path.Contains(@":\"))
            {
                string[] splitted = tmp.Split(new[] { @":\" }, System.StringSplitOptions.RemoveEmptyEntries);
                root = splitted[0] + @":\";
                tmp = splitted[1];
            }
            if (tmp.Contains(@"\"))
            {
                string[] splitted = tmp.Split(new[] { @"\" }, System.StringSplitOptions.RemoveEmptyEntries);
                root += splitted[0];
                tmp = tmp.Substring(tmp.IndexOf(@"\", 1) + 1);
            }

            return tmp;
        }
        #endregion


































        //public static IEnumerable<Type> GetGenericIEnumerables(this object o)
        //{
        //    return o.GetType()
        //            .GetInterfaces()
        //            .Where(t => t.IsGenericType
        //                && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        //            .Select(t => t.GetGenericArguments()[0]);
        //}


        //public static List<T2> ChangeType<T2>(this List<object> copyFromList)
        //{
        //    if (copyFromList != null)
        //    {
        //        Type listType = typeof(List<>).MakeGenericType(new[] { typeof(T2) });
        //        IList list = (IList)Activator.CreateInstance(listType);

        //        if (OpenOrmTools.IsListOrArray(copyFromList.GetType()))
        //        {
        //            foreach (var o in copyFromList)
        //            {
        //                T2 o2 = (T2)Activator.CreateInstance(typeof(T2));
        //                o.CopyTo(o2);
        //                list.Add(o2);
        //            }

        //            return (List<T2>)Convert.ChangeType(list, typeof(List<T2>));
        //        }
        //        else
        //        {
        //            T2 o2 = (T2)Activator.CreateInstance(typeof(T2));
        //            copyFromList.CopyTo(o2);
        //            list.Add(o2);
        //        }

        //        return (List<T2>)list;
        //    }

        //    return null;
        //}

        //public static void CopyTo<T, T2>(this T copyFromObject, T2 copyToObject)
        //{
        //    //BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        //    BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        //    FieldInfo[] fieldsFromObject = copyFromObject.GetType().GetFields(bindFlags);
        //    FieldInfo[] fieldsToObject = copyToObject.GetType().GetFields(bindFlags);

        //    for (int i = 0; i < fieldsFromObject.Length; i++)
        //    {
        //        FieldInfo fieldFrom = copyFromObject.GetType().GetField(fieldsFromObject[i].Name, bindFlags);
        //        if (fieldFrom != null)
        //        {
        //            for (int j = 0; j < fieldsToObject.Length; j++)
        //            {
        //                FieldInfo fieldTo = copyToObject.GetType().GetField(fieldsToObject[j].Name, bindFlags);
        //                if (fieldTo != null)
        //                {
        //                    if (fieldFrom.Name == fieldTo.Name)
        //                    {
        //                        if(fieldFrom.FieldType != fieldTo.FieldType)
        //                        {
        //                            object fromObjectValue = fieldFrom.GetValue(copyFromObject);
        //                            object toObjectValue = Activator.CreateInstance(fieldTo.FieldType);
        //                            fromObjectValue.CopyTo(toObjectValue);
        //                            fieldTo.SetValue(copyToObject, toObjectValue);
        //                        }
        //                        else
        //                        {
        //                            fieldTo.SetValue(copyToObject, fieldFrom.GetValue(copyFromObject));
        //                        }


        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}







        //public static string GetStringBetween(this string current, string firstPart, string secondPart)
        //{
        //    if (!current.Contains(firstPart)) return "";
        //    string tmp = current.Substring(current.IndexOf(firstPart) + firstPart.Length);
        //    if (!tmp.Contains(secondPart)) return tmp;
        //    return tmp.Substring(0, tmp.IndexOf(secondPart) + 1);
        //}
    }
}
