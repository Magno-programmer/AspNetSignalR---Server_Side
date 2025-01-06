using System.Text.RegularExpressions;

namespace AspNetSignalIR.ImageConfigurations;

public class CheckBase64
{
    internal static bool IsBase64String(string base64)
    {
        // Check if a string is Base64 encoded
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None);
    }
}
