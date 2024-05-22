using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetExpensesController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public BudgetExpensesController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/BudgetExpenses
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            // Filter out soft deleted expenses
            var expenses = await _db.Expenses.Where(e => !e.IsDeleted).ToListAsync();
            return expenses;
        }

        // GET: api/BudgetExpenses/5
        [HttpGet("{id:int}", Name = "GetExpenseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Expense>> GetExpenseById(int id)
        {
            // Retrieve expense only if it's not soft deleted
            var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (expense == null)
            {
                return NotFound();
            }
            return Ok(expense);
        }

        // GET: api/BudgetExpenses/user/username
        [HttpGet("user/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByUserName(string userName)
        {
            // Filter out soft deleted expenses
            var expenses = await _db.Expenses.Where(e => e.UserName == userName && !e.IsDeleted).ToListAsync();
            if (expenses == null || !expenses.Any())
            {
                return NotFound("No expenses found for the specified user.");
            }
            return expenses;
        }

        // GET: api/BudgetExpenses/destination/username
        [HttpGet("destination/{destination}/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByDestinationAndUserName(string destination, string userName)
        {
            // Filter out soft deleted expenses
            var expenses = await _db.Expenses.Where(e => e.Destination == destination && e.UserName == userName && !e.IsDeleted).ToListAsync();
            if (expenses == null || !expenses.Any())
            {
                return NotFound("No expenses found for the specified destination and user.");
            }
            return expenses;
        }

        // GET: api/BudgetExpenses/total/destination/username
        [HttpGet("total/{destination}/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<double>> GetTotalExpensesForDestinationAndUser(string destination, string userName)
        {
            // Calculate total expenses excluding soft deleted expenses
            var totalExpenses = await _db.Expenses.Where(e => e.Destination == destination && e.UserName == userName && !e.IsDeleted).SumAsync(e => e.Amount);
            return Ok(totalExpenses);
        }

        // POST: api/BudgetExpenses
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Expense>> AddExpense(Expense expense)
        {
            if (expense == null)
            {

            }
            // Set the date to the current date
            expense.Date = DateTime.Now;
            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(AddExpense), new { id = expense.Id }, expense);
        }

        // PUT: api/BudgetExpenses/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExpense(Expense expense, int id)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            _db.Entry(expense).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/BudgetExpenses/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Expense>> DeleteExpense(int id)
        {
            var expense = await _db.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            expense.IsDeleted = true; // Mark expense as deleted
            _db.Expenses.Update(expense);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
