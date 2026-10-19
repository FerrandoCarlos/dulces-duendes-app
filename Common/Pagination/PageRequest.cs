namespace DulcesDuendesApp.Common.Pagination
{
    public class PageRequest
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 20;

        public int Page { get; }
        public int PageSize { get; }
        public int Skip => (int)Math.Min((long)(Page - 1) * PageSize, int.MaxValue);

        public PageRequest(int page = 1, int pageSize = DefaultPageSize)
        {
            Page = Math.Max(1, page);
            PageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        }

    }
}
