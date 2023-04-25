using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SupernoteDesktopClient.Extensions
{
    internal static class StringExtensions
    {
        public static string MaskSerialNumber(this string value)
        {
            int unmaskedStartCharacters = 4;
            int unmaskedEdnCharacters = 3;
            int totaUnmaskedCharacters = unmaskedStartCharacters + unmaskedEdnCharacters;

            if (value.Length > totaUnmaskedCharacters)
            {
                return value.Substring(0, unmaskedStartCharacters) + new String('X', value.Length - totaUnmaskedCharacters) + value.Substring(value.Length - unmaskedEdnCharacters);
            }
            else
                return value;
        }

        public static string GetShortSHA1Hash(this string value)
        {
            string returnResult;

            using (SHA1 sha1 = SHA1.Create())
            {
                returnResult = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(value)).Take(10).ToArray());
            }

            return returnResult;
        }

        public static string ReplaceFirstOccurrence(this string source, string search, string replace)
        {
            int position = source.IndexOf(search);
            if (position < 0)
                return source;

            return source.Remove(position, search.Length).Insert(position, replace);
        }
    }
}
