using FinShark.Data;
using FinShark.Models;
using FinShark.Models.Dtos;
using FinShark.Models.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        //DTO Data Transfer Object
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }
        //Url/api/Stock/GetAllStock
        [HttpGet]
        public async Task<IActionResult> GetAllStock()
        {
            //SQL = Select * From Stock
            var Stocks = await _context.Stocks
                .Select(s => s.ToStockDto())
                .ToListAsync();

            //List<StockDto> stockDtos = new List<StockDto>();
            //foreach (var Stock in Stocks)
            //{
            //      s.ToStockDto() = StockMappers.ToStockDto(Stock)
            //    stockDtos.Add(StockMappers.ToStockDto(Stock));
            //}

            return Ok(Stocks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockById(int id)
        {
            //SQL = Select * From Stock as s Where s.Id = {id} and s.IsActive = 1
            //Stock stock = _context.Stocks.Where(s => (s.Id == id && s.IsActive == true)).First();
            Stock stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return BadRequest($"Not Found Stock With Id = {id}");//404
            }
            return Ok(stock);
        }
        [HttpPost]//  POST api/Stock 
        public async Task<IActionResult> AddNewStock(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();
            return Created();
        }
        [HttpPost("CreateNewStock")]//  POST api/Stock/CreateNewStock
        public async Task<IActionResult> CreateNewStock([FromBody] StockRequestDto stockRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //await _context.Stocks.AddAsync(stock);
            //await _context.SaveChangesAsync();
            return Created();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id , Stock newstock)
        {
            //Stock oldstock = _context.Stocks.Find(id);
            //if (oldstock == null)
            //{
            //    return BadRequest($"Not Found Stock With Id = {id}");//404
            //}
            //oldstock.Symbol = newstock.Symbol;
            //oldstock.CompanyName = newstock.CompanyName;
            //oldstock.Purchase = newstock.Purchase;
            //oldstock.MarketCap = newstock.MarketCap;
            //oldstock.Industry = newstock.Industry;
            //oldstock.LastDiv = newstock.LastDiv;
            //_context.SaveChanges();
            //return Ok(oldstock);
            _context.Stocks.Update(newstock);
            await _context.SaveChangesAsync();
            return Ok(newstock);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockById(int id)
        {
            //Hard Delete 
            //Stock stock = _context.Stocks.Find(id);
            //_context.Stocks.Remove(stock);
            //_context.SaveChanges();

            //Soft Delete
            Stock stock =await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return BadRequest($"Not Found Stock With Id = {id}");//404
            }
            stock.IsDelete = true;
            await _context.SaveChangesAsync();
            return Ok(stock);
        }
        //RESTFULL API CURD
        /*
        GET    api/Stock    =>GetAllStock()   Done
        GET    api/Stock/1  =>GetStockById(int id) Done
        POST   api/Stock    =>AddNewStock(Stock stock) Done
        PUT    api/Stock/2  =>UpdateStock(int id , Stock newstock) Done
        Delete api/Stock/6  =>DeleteStockById(int id) Done
         */
    }
}
