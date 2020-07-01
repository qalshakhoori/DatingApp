namespace DatingApp.API.Helpers
{
  public class PaginationHandler
  {
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public PaginationHandler(int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      CurrentPage = currentPage;
      ItemsPerPage = itemsPerPage;
      TotalItems = totalItems;
      TotalPages = totalPages;
    }
  }
}