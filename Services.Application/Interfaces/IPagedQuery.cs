namespace Services.Application.Interfaces;

public interface IPagedQuery
{
    int Page { get; set; }
    int PageSize { get; set; }
}