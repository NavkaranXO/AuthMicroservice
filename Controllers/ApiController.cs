
using MyMicroservice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMicroservice.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyMicroservice.Controllers
{
    [ApiController]
    [Route("cashcards")]
    [Authorize]
    public class CashCardController : ControllerBase
    {
        private readonly CashCardDbContext _context;

        public CashCardController(CashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCashCards()
        {
            var cashcards = await _context.CashCards.ToListAsync();
            return Ok(cashcards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashCardById(int id)
        {
            var card = await _context.CashCards.FindAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CashCard), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCashCard([FromBody] CashCard card)
        {
            await _context.CashCards.AddAsync(card);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCashCardById), new { id = card.Id }, card);
        }

    }
}