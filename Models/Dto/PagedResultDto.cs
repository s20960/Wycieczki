namespace Wycieczki.Models.Dto
{
    public class PagedResultDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int AllPages { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}
