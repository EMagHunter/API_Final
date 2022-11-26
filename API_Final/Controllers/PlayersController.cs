using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Final.Models;
using Microsoft.Data.SqlClient;

namespace API_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly APIFinalDbContext _context;

        public PlayersController(APIFinalDbContext context)
        {
            _context = context;
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        // GET: api/Players/[playerid]
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/Players/onlinecount
        [HttpGet("onlinecount")]
        public async Task<ActionResult<int>> GetOnlineCount()
        {
            int OnlineCount = (from p in _context.Players where p.IsOnline select p).Count();

            return OnlineCount;
        }

        // GET: api/Players/matches
        [HttpGet("matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches.ToListAsync();
        }

        // GET: api/Players/matchcount/[playerid]
        [HttpGet("matchcount/{id}")]
        public async Task<ActionResult<int>> GetPlayerMatchCount(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            int numMatchesPlayed = (from m in _context.Matches where (m.Player1 == id || m.Player2 == id) select m).Count();

            return numMatchesPlayed;
        }

        // GET: api/Players/winrate/[playerid]
        [HttpGet("winrate/{id}")]
        public async Task<ActionResult<float>> GetPlayerWinrate(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            int numMatchesPlayed = (from m in _context.Matches where (m.Player1 == id || m.Player2 == id) select m).Count();
            int numMatchesWon = (from m in _context.Matches where (m.Winner == id) select m).Count();

            float winrate = numMatchesPlayed == 0 ? 0.0f : (float)numMatchesWon / (float)numMatchesPlayed;

            return winrate;
        }


        // PUT: api/Players/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(int id, Player player)
        {
            if (id != player.PlayerId)
            {
                return BadRequest();
            }

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Players
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.PlayerId }, player);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.PlayerId == id);
        }
    }
}
