using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItem([FromQuery]SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();
        
        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }
        
        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(i => i.Make, Order.Ascending),
            "new" => query.Sort(i => i.CreatedAt, Order.Descending),
            "model" => query.Sort(i => i.Model, Order.Ascending),
            "year" => query.Sort(i => i.Year, Order.Ascending),
            _ => query.Sort(i => i.AuctionEnd, Order.Ascending)
        };

        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(i => i.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(i => i.AuctionEnd < DateTime.UtcNow.AddDays(1) && i.AuctionEnd > DateTime.UtcNow),  
            _ => query.Match(i => i.AuctionEnd > DateTime.UtcNow)
        };
        
        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(i => i.Seller == searchParams.Seller);
        }
        
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(i => i.Winner == searchParams.Winner);
        }

        query.PageNumber(searchParams.PageNumber).PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync();
        
        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
    
}
