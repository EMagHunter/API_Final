using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Final.Models;
using Microsoft.Data.SqlClient;
using System.Numerics;

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

        // GET: api/Players/id/[playerid]
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Player>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/Players/[playername]
        [HttpGet("{name}")]
        public async Task<ActionResult<Player>> GetPlayerByName(string name)
        {
            Player player = (from p in _context.Players where p.PlayerName == name select p).First<Player>();

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

        // GET: api/Players/onlinecount/[region]
        [HttpGet("onlinecount/{region}")]
        public async Task<ActionResult<int>> GetOnlineCountForRegion(string region)
        {
            int OnlineCount = (from p in _context.Players where (p.IsOnline && p.PlayerRegion == region) select p).Count();

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

        // POST: api/Players/onlinestatus
        // params: id, status
        [HttpPost("onlinestatus")]
        public async Task<IActionResult> SetOnlineStatus([FromQuery]int id, [FromQuery]bool online)
        {
            if (!PlayerExists(id))
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            player.IsOnline = online;
            _context.Entry(player).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Players/register
        //params: name, region
        [HttpPost("register")]
        public async Task<ActionResult<Player>> PostPlayer([FromQuery]string name, [FromQuery]string region)
        {
            Player player = new Player();
            player.PlayerName = name;
            player.PlayerRegion = region;
            player.IsOnline = false; 
            
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.PlayerId }, player);
        }

        // POST: api/Players/matchresult
        //params: player1, player2, winner
        [HttpPost("matchresult")]
        public async Task<ActionResult<Match>> PostMatch([FromQuery] int player1, [FromQuery] int player2, [FromQuery] int winner)
        {

            if (winner != player1 && winner != player2)
            {
                return BadRequest();
            }

            if (!PlayerExists(player1) || !PlayerExists(player2))
            {
                return NotFound();
            }

            Match match = new Match();
            match.Player1 = player1;
            match.Player2 = player2;
            match.Winner = winner;
            match.MatchTimestamp = DateTime.Now;

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            //for some reason using CreatedAtAction here causes an error saying "No route matches the supplied values."
            //I can't tell what's wrong. I'd really love to know, but the internet wasn't very helpful
            //I figure some information that would otherwise be present in the reponse is missing when I return only the
            //match object, but as it stands this is preferable over an error screen.
            return match; 
        }


        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.PlayerId == id);
        }
    }
}
