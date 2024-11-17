using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Shared.ExtensionMethod;

public static class StringExtensions
{
    public static string GetDisplayName(object instance, string propertyName)
    {
        Type type = instance.GetType();

        DisplayAttribute? displayAttribute = type.GetProperty(propertyName)!.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? propertyName;
    }
    public static bool IsValidName(this string obj)
    {
        return obj.Trim() == "test" == false;
    }
    public static string GetCamelCase(this string str) => char.ToLowerInvariant(str[0]) + str[1..]; //str.Substring(1)
    public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
    {
        return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
    }

    public static int ToInt(this string value)
    {
        return Convert.ToInt32(value);
    }

    public static decimal ToDecimal(this string value)
    {
        return Convert.ToDecimal(value);
    }

    public static string ToNumeric(this int value)
    {
        return value.ToString("N0"); //"123,456"
    }

    public static string ToNumeric(this decimal value)
    {
        return value.ToString("N0");
    }

    public static string ToCurrency(this int value)
    {
        //fa-IR => current culture currency symbol => ریال
        //123456 => "123,123ریال"
        return value.ToString("C0");
    }

    public static string ToCurrency(this decimal value)
    {
        return value.ToString("C0");
    }

    public static string En2Fa(this string str)
    {
        return str.Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");
    }

    public static string Fa2En(this string str)
    {
        return str.Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            //iphone numeric
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9");
    }

    public static string FixPersianChars(this string str)
    {
        return str.Replace("ﮎ", "ک")
            .Replace("ﮏ", "ک")
            .Replace("ﮐ", "ک")
            .Replace("ﮑ", "ک")
            .Replace("ك", "ک")
            .Replace("ي", "ی")
            .Replace(" ", " ")
            .Replace("‌", " ")
            .Replace("ھ", "ه");//.Replace("ئ", "ی");
    }

    public static string? CleanString(this string str)
    {
        return str.Trim().FixPersianChars().Fa2En().NullIfEmpty();
    }

    public static string? NullIfEmpty(this string str)
    {
        return str?.Length == 0 ? null : str;
    }

    public static bool CheckSheba(this string str, bool checkLenght)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return false;
        }
        str = str.Replace(" ", "").ToLower();
        //بررسی رشته وارد شده برای اینکه در فرمت شبا باشد
        var isSheba = Regex.IsMatch(str, "^[a-zA-Z]{2}\\d{2} ?\\d{4} ?\\d{4} ?\\d{4} ?\\d{4} ?[\\d]{0,2}",
            RegexOptions.Compiled);

        if (!isSheba)
            return false;
        //طول شماره شبا را چک میکند کمتر نباشد
        if (str.Length < 26)
            return false;
        str = str.ToLower();
        //بررسی اعتبار سنجی اصلی شبا
        ////ابتدا گرفتن چهار رقم اول شبا
        var get4FirstDigit = str.Substring(0, 4);
        ////جایگزین کردن عدد 18 و 27 به جای آی و آر
        var replacedGet4FirstDigit = get4FirstDigit.ToLower().Replace("i", "18").Replace("r", "27");
        ////حذف چهار رقم اول از رشته شبا
        var removedShebaFirst4Digit = str.Replace(get4FirstDigit, "");
        ////کانکت کردن شبای باقیمانده با جایگزین شده چهار رقم اول
        var newSheba = removedShebaFirst4Digit + replacedGet4FirstDigit;
        ////تبدیل کردن شبا به عدد  - دسیمال تا 28 رقم را نگه میدارد
        var finalLongData = Convert.ToDecimal(newSheba);
        ////تقسیم عدد نهایی به مقدار 97 - اگر باقیمانده برابر با عدد یک شود این رشته شبا صحیح خواهد بود
        var finalReminder = finalLongData % 97;
        if (finalReminder == 1)
        {
            return true;
        }
        return false;


    }

    public static bool IsIzbSheba(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return false;
        }
        var codeBank = str.Substring(4, 3);

        if (codeBank == "069")
            return true;
        else
            return false;
    }

    public static bool IsCorrectDateTimeRequest(this string RequestDateTime)
    {
        if (string.IsNullOrWhiteSpace(RequestDateTime))
        {
            return false;
        }

        DateTime requestDateTime = Convert.ToDateTime(RequestDateTime);
        DateTime ServerDateTime = DateTime.Now;

        DateTime TwoMinBefore = ServerDateTime.Subtract(TimeSpan.FromMinutes(2));
        DateTime TwoMinAfter = ServerDateTime.AddMinutes(2);

        if (requestDateTime.Ticks > TwoMinBefore.Ticks && requestDateTime.Ticks < TwoMinAfter.Ticks)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsCorrectDateTimeRequest2(this string RequestDateTime)
    {
        if (string.IsNullOrWhiteSpace(RequestDateTime))
        {
            return false;
        }

        DateTime requestDateTime = Convert.ToDateTime(RequestDateTime);
        DateTime ServerDateTime = DateTime.Now;

        DateTime TwoMinBefore = ServerDateTime.Subtract(TimeSpan.FromMinutes(5));
        DateTime TwoMinAfter = ServerDateTime.AddMinutes(5);

        if (requestDateTime.Ticks > TwoMinBefore.Ticks && requestDateTime.Ticks < TwoMinAfter.Ticks)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string ToMaskedPan(this string value)
    {
        if (value.Length == 16)
        {
            string maskPan = string.Format("{0}{1}{2}", value.Substring(0, 6), "******", value.Substring(12, 4));
            return maskPan;
        }
        else
        {
            return "****";
        }
    }

    public static byte[] ToByteArray(this string base64String)
    {
        return Convert.FromBase64String(base64String);
    }
}