using System.Security.Cryptography;
using System.Text;

namespace csharp_ef_webapi.Utilities;

// Helper class that generates the etag from a key (route) and content (response)
public static class ETagGenerator
{
    public static string GenerateETag(string objectJson)
    {
        using (var md5 = MD5.Create())
        {
            var dataBytes = Encoding.UTF8.GetBytes(objectJson);
            var hashBytes = md5.ComputeHash(dataBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}