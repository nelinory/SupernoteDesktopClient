using System;

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
    }
}
