using FinShark.Dtos.Stock;
using FinShark.Filters;
using FinShark.Helpers;
using FinShark.Interfaces;
using FinShark.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    // Session 3: الـ Controller = الكلاس اللي بيستقبل الـ HTTP requests ويرد عليها.
    // [Route]: كل الـ endpoints هنا بتبدأ بـ /api/stock
    // [ApiController]: بيقرا الـ JSON، وبيعمل validation تلقائي ويرجّع 400 لو الـ DataAnnotations اتكسرت
    // ControllerBase: بيدّينا Ok() و NotFound() و CreatedAtAction()...
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        // Session 5: الـ Controller بيكلّم الـ Repository بس — مفيش DbContext هنا خالص
        private readonly IStockRepository _stockRepo;
        // Session 8: الـ Logger (Serilog ورا الكواليس)
        private readonly ILogger<StockController> _logger;

        // Dependency Injection: الحاجات دي بتيجي جاهزة من الـ DI Container (متسجّلة في Program.cs)
        public StockController(IStockRepository stockRepo, ILogger<StockController> logger)
        {
            _stockRepo = stockRepo;
            _logger = logger;
        }

        // GET /api/stock?symbol=AA&sortBy=Purchase&isDescending=true&pageNumber=1&pageSize=10
        // Session 7: [FromQuery] بيملا الـ QueryObject من الـ query parameters
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StockDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            var stocks = await _stockRepo.GetAllAsync(query);

            // Session 4: بنرجّع DTOs مش Entities
            var stockDtos = stocks.Select(s => s.ToStockDto()).ToList();

            return Ok(stockDtos);   // 200 (حتى لو القائمة فاضية)
        }

        // GET /api/stock/1
        // {id:int} = route constraint: الـ id لازم يكون رقم، و [FromRoute] = اقرأه من الـ URL
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                // Session 8: Structured Logging — {StockId} حقل نقدر نبحث بيه في السجلّات
                _logger.LogWarning("Stock {StockId} not found", id);
                return NotFound();   // 404 — مش Ok(null)
            }

            return Ok(stock.ToStockDto());
        }

        // GET /api/stock/symbol/AAPL — بحث بالرمز
        [HttpGet("symbol/{symbol}")]
        [ProducesResponseType(typeof(StockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySymbol([FromRoute] string symbol)
        {
            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        // POST /api/stock
        // [FromBody]: البيانات جاية JSON في جسم الـ request
        // ValidationFilter: بيشغّل FluentValidation قبل الـ action (Session 4)
        [HttpPost]
        [TypeFilter(typeof(ValidationFilter<CreateStockRequestDto>))]
        [ProducesResponseType(typeof(StockDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            // Session 4: ModelState.IsValid — [ApiController] بيعملها لوحده، بس كاتبينها صريح للتوضيح
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 409 Conflict: البيانات سليمة شكلاً، بس فيه سهم بنفس الرمز موجود فعلاً
            var symbol = stockDto.Symbol.NormalizeSymbol();
            if (await _stockRepo.GetBySymbolAsync(symbol) is not null)
                return Conflict($"Stock with symbol '{symbol}' already exists.");

            var stockModel = stockDto.ToStockFromCreateDto();
            await _stockRepo.CreateAsync(stockModel);

            _logger.LogInformation("Created stock {StockId} ({Symbol})", stockModel.Id, stockModel.Symbol);

            // 201 Created + Location header فيه لينك السهم الجديد (/api/stock/{id})
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        // PUT /api/stock/1 — الـ id من الـ URL، والقيم الجديدة من الـ body
        [HttpPut("{id:int}")]
        [TypeFilter(typeof(ValidationFilter<UpdateStockRequestDto>))]
        [ProducesResponseType(typeof(StockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // لو الرمز الجديد بتاع سهم تاني → 409. لو نفس السهم، مسموح
            var symbol = updateDto.Symbol.NormalizeSymbol();
            var stockWithSameSymbol = await _stockRepo.GetBySymbolAsync(symbol);
            if (stockWithSameSymbol is not null && stockWithSameSymbol.Id != id)
                return Conflict($"Stock with symbol '{symbol}' already exists.");

            var updatedStock = await _stockRepo.UpdateAsync(id, updateDto.ToStockFromUpdateDto());

            if (updatedStock == null)
            {
                return NotFound();
            }

            return Ok(updatedStock.ToStockDto());   // 200 + السهم بعد التعديل
        }

        // DELETE /api/stock/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Deleted stock {StockId}", id);

            return NoContent();   // 204 — الحذف نجح ومفيش حاجة نرجّعها
        }
    }
}
