namespace Cross.SharedKernel.Abstractions;

/// <summary>
/// An object that represents a paginated result useful for pagination.
/// </summary>
/// <param name="Items">The items in the current page.</param>
/// <param name="TotalItems">The total number of items.</param>
/// <param name="Page">The page used to get the items.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <typeparam name="T">The type of the items in the paginated result.</typeparam>
public record PaginatedResult<T>(IEnumerable<T> Items, int TotalItems, int Page, int PageSize);
