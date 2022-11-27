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
        public async Task<Response<IEnumerable<Player>>> GetPlayers()
        {
            return new(StatusCodes.Status200OK, "OK", await _context.Players.ToListAsync());
        }

        // GET: api/Players/id/[playerid]
        [HttpGet("id/{id}")]
        public async Task<Response<Player>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return PlayerNotFound();
            }
            
            return new(StatusCodes.Status200OK, "OK", player);
        }

        // GET: api/Players/name/[playername]
        [HttpGet("name/{name}")]
        public Response<Player> GetPlayerByName(string name)
        {
            IQueryable<Player> query = (from p in _context.Players where p.PlayerName == name select p);
            Player player = query.Count() == 0 ? null : query.First<Player>();

            if (player == null)
            {
                return PlayerNotFound();
            }

            return new(StatusCodes.Status200OK, "OK", player);
        }

        // GET: api/Players/onlinecount
        [HttpGet("onlinecount")]
        public Response<int> GetOnlineCount()
        {
            int OnlineCount = (from p in _context.Players where p.IsOnline select p).Count();
            return new(StatusCodes.Status200OK, "OK", OnlineCount);
        }

        // GET: api/Players/onlinecount/[region]
        [HttpGet("onlinecount/{region}")]
        public Response<int> GetOnlineCountForRegion(string region)
        {
            int OnlineCount = (from p in _context.Players where (p.IsOnline && p.PlayerRegion == region) select p).Count();
            return new(StatusCodes.Status200OK, "OK", OnlineCount);
        }

        // GET: api/Players/matches
        [HttpGet("matches")]
        public async Task<Response<IEnumerable<Match>>> GetMatches()
        {
            return new(StatusCodes.Status200OK, "OK", await _context.Matches.ToListAsync());
        }

        // GET: api/Players/match/[playerid]
        [HttpGet("match/{id}")]
        public async Task<Response<Match>> GetMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return MatchNotFound();
            }

            return new(StatusCodes.Status200OK, "OK", match);
        }

        // GET: api/Players/matchcount/[playerid]
        [HttpGet("matchcount/{id}")]
        public async Task<Response<int>> GetPlayerMatchCount(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return new(404, "Player not found", 0);
            }

            int numMatchesPlayed = (from m in _context.Matches where (m.Player1 == id || m.Player2 == id) select m).Count();

            return new(StatusCodes.Status200OK, "OK", numMatchesPlayed);
        }

        // GET: api/Players/winrate/[playerid]
        [HttpGet("winrate/{id}")]
        public async Task<Response<float>> GetPlayerWinrate(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return new(StatusCodes.Status404NotFound, "Player not found", 0);
            }

            int numMatchesPlayed = (from m in _context.Matches where (m.Player1 == id || m.Player2 == id) select m).Count();
            int numMatchesWon = (from m in _context.Matches where (m.Winner == id) select m).Count();

            float winrate = numMatchesPlayed == 0 ? 0.0f : (float)numMatchesWon / (float)numMatchesPlayed;

            return new(StatusCodes.Status200OK, "OK", winrate);
        }

        // PATCH: api/Players/onlinestatus
        // params: id, status
        [HttpPatch("onlinestatus")]
        public async Task<Response<Player>> SetOnlineStatus([FromQuery]int id, [FromQuery]bool online)
        {
            if (!PlayerExists(id))
            {
                return PlayerNotFound();
            }

            var player = await _context.Players.FindAsync(id);
            player.IsOnline = online;
            _context.Entry(player).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new(StatusCodes.Status202Accepted, "Online status updated", player);
        }

        // POST: api/Players/register
        //params: name, region
        [HttpPost("register")]
        public async Task<Response<Player>> PostPlayer([FromQuery]string name, [FromQuery]string region)
        {
            if (name.Length > 20)
            {
                return new(StatusCodes.Status400BadRequest, "Name must be at most 20 characters", null);
            }
            if (region != "NA" && region != "EU")
            {
                return new(StatusCodes.Status400BadRequest, "Region must be NA or EU", null);
            }
            if (GetPlayerByName(name).Data != null)
            {
                return new(StatusCodes.Status400BadRequest, "Name already in use", null);
            }
            Player player = new Player();
            player.PlayerName = name;
            player.PlayerRegion = region;
            player.IsOnline = false; 
            
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return new(StatusCodes.Status201Created, "New player created", player);
        }

        // POST: api/Players/matchresult
        //params: player1, player2, winner
        [HttpPost("matchresult")]
        public async Task<Response<Match>> PostMatch([FromQuery] int player1, [FromQuery] int player2, [FromQuery] int winner)
        {

            if (winner != player1 && winner != player2)
            {
                return new(StatusCodes.Status400BadRequest, "Winner must be one of the participating players", null);
            }

            if (!PlayerExists(player1) || !PlayerExists(player2))
            {
                if (!PlayerExists(player1))
                    return new(StatusCodes.Status404NotFound, "Player 1 not found", null);
                else
                    return new(StatusCodes.Status404NotFound, "Player 2 not found", null);
            }

            Match match = new Match();
            match.Player1 = player1;
            match.Player2 = player2;
            match.Winner = winner;
            match.MatchTimestamp = DateTime.Now;

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return new(StatusCodes.Status201Created, "Match data created", match); 
        }


        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.PlayerId == id);
        }

        private Response<Player> PlayerNotFound()
        {
            Response<Player> r = new(StatusCodes.Status404NotFound, "Player not found", null);
            return r;
        }

        private Response<Match> MatchNotFound()
        {
            Response<Match> r = new(StatusCodes.Status404NotFound, "Match not found", null);
            return r;
        }
    }
}
