namespace csharp_ef_data_loader.Services;

using System.Text;

public static class DotaServiceExtensions
{
    public static void SetQuery(this UriBuilder uriBuilder, IEnumerable<KeyValuePair<string, string>> query)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var kvp in query)
        {
            if (sb.Length != 0)
                sb.Append('&');
            sb.AppendJoin('=', kvp.Key, kvp.Value);
        }
        uriBuilder.Query = sb.ToString();
    }

    public static void AppendPath(this UriBuilder uriBuilder, string path)
    {
        uriBuilder.Path = Path.Join(uriBuilder.Path, path);
    }
}
