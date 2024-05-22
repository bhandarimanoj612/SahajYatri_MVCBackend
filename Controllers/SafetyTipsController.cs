using Microsoft.AspNetCore.Mvc;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sahaj_Yatri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SafetyTipsController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public SafetyTipsController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/SafetyTips
        [HttpGet]
        public ActionResult<IEnumerable<SafetyTip>> Get()
        {
            var safetyTips = _db.SafetyTips.ToList();
            return Ok(safetyTips);
        }

        // GET: api/SafetyTips/5
        [HttpGet("{id}")]
        public ActionResult<SafetyTip> Get(int id)
        {
            var safetyTip = _db.SafetyTips.FirstOrDefault(s => s.Id == id);
            if (safetyTip == null)
            {
                return NotFound();
            }
            return safetyTip;
        }

        // POST: api/SafetyTips
        [HttpPost]
        public IActionResult Post([FromBody] SafetyTip safetyTip)
        {
            _db.SafetyTips.Add(safetyTip);
            _db.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = safetyTip.Id }, safetyTip);
        }

        // PUT: api/SafetyTips/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] SafetyTip safetyTip)
        {
            var existingSafetyTip = _db.SafetyTips.FirstOrDefault(s => s.Id == id);
            if (existingSafetyTip == null)
            {
                return NotFound();
            }
            existingSafetyTip.Text = safetyTip.Text;
            _db.SaveChanges();
            return NoContent();
        }

        // DELETE: api/SafetyTips/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var safetyTip = _db.SafetyTips.FirstOrDefault(s => s.Id == id);
            if (safetyTip == null)
            {
                return NotFound();
            }
            _db.SafetyTips.Remove(safetyTip);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
