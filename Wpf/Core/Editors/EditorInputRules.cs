using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Wpf.Core.Editors;

public static class EditorInputRules
{
    #region Methods

    public static string ToTurkishProperCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        CultureInfo culture = CultureInfo.GetCultureInfo("tr-TR");
        string[] words = value.Split(' ' , StringSplitOptions.None);
        StringBuilder builder = new();

        for (int index = 0 ; index < words.Length ; index++)
        {
            if (index > 0)
            {
                builder.Append(' ');
            }

            string word = words[index];

            if (string.IsNullOrEmpty(word))
            {
                continue;
            }

            string lowerWord = word.ToLower(culture);
            builder.Append(char.ToUpper(lowerWord[0] , culture));

            if (lowerWord.Length > 1)
            {
                builder.Append(lowerWord[1..]);
            }
        }

        return builder.ToString();
    }

    public static string FormatPhone(string value)
    {
        string digits = new([.. value.Where(char.IsDigit)]);

        if (digits.Length == 0)
        {
            return string.Empty;
        }

        if (digits.Length > 11)
        {
            digits = digits[..11];
        }

        if (digits.StartsWith('0'))
        {
            return digits.Length switch
            {
                <= 4 => digits,
                <= 7 => $"{digits[..4]} {digits[4..]}",
                <= 9 => $"{digits[..4]} {digits[4..7]} {digits[7..]}",
                _ => $"{digits[..4]} {digits[4..7]} {digits[7..9]} {digits[9..]}"
            };
        }

        return digits.Length switch
        {
            <= 3 => digits,
            <= 6 => $"{digits[..3]} {digits[3..]}",
            <= 8 => $"{digits[..3]} {digits[3..6]} {digits[6..]}",
            _ => $"{digits[..3]} {digits[3..6]} {digits[6..8]} {digits[8..]}"
        };
    }

    public static string NormalizeEmail(string value)
    {
        return value.Trim().ToLower(CultureInfo.GetCultureInfo("tr-TR"));
    }

    public static bool IsValidEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return Regex.IsMatch(
            value ,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$" ,
            RegexOptions.CultureInvariant);
    }

    public static string NormalizeDecimal(string value)
    {
        return value.Replace('.' , ',');
    }

    public static string KeepDigits(string value)
    {
        return new([.. value.Where(char.IsDigit)]);
    }

    #endregion Methods
}