using FinShark.Data;
using FinShark.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }
        //Url/api/Stock/GetAllStock
        [HttpGet]
        public IActionResult GetAllStock()
        {
            //SQL = Select * From Stock
            List<Stock> Stocks = _context.Stocks.ToList();
            return Ok(Stocks);
        }
        [HttpGet("{id}")]
        public IActionResult GetStockById(int id)
        {
            //SQL = Select * From Stock as s Where s.Id = {id} and s.IsActive = 1
            //Stock stock = _context.Stocks.Where(s => (s.Id == id && s.IsActive == true)).First();
            Stock stock = _context.Stocks.Find(id);
            if (stock == null)
            {
                return BadRequest($"Not Found Stock With Id = {id}");//404
            }
            return Ok(stock);
        }
        [HttpPost]
        public IActionResult AddNewStock(Stock stock)
        {
            _context.Stocks.Add(stock);
            _context.SaveChanges();
            return Created();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateStock(int id , Stock newstock)
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
            _context.SaveChanges();
            return Ok(newstock);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStockById(int id)
        {
            //Hard Delete 
            //Stock stock = _context.Stocks.Find(id);
            //_context.Stocks.Remove(stock);
            //_context.SaveChanges();

            //Soft Delete
            Stock stock = _context.Stocks.Find(id);
            if (stock == null)
            {
                return BadRequest($"Not Found Stock With Id = {id}");//404
            }
            stock.IsDelete = true;
            _context.SaveChanges();
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
