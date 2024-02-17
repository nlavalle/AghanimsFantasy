using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Utilities;
public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> items, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize);

        this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        // With the repository setup I don't think I'm ever going to call this with IQueryable, but idk
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(Task<IEnumerable<T>> source, int pageIndex, int pageSize)
    {
        // This might be the repository approach but tbd
        IEnumerable<T> sourceResult = await source;

        var items = sourceResult.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, pageIndex, pageSize);
    }
}