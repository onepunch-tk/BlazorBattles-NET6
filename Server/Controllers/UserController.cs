using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorBattles.Server.Data;
using BlazorBattles.Server.Services;
using BlazorBattles.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBattles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityService;

        public UserController(DataContext context, IUtilityService utilityService)
        {
            _context = context;
            _utilityService = utilityService;
        }


        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            var user = await _utilityService.GetUser();
            return Ok(user?.Bananas);
        }

        [HttpPut("addbananas")]
        public async Task<IActionResult> AddBananas([FromBody] int bananas)
        {
            var user = await _utilityService.GetUser();
            user.Bananas += bananas;

            await _context.SaveChangesAsync();
            return Ok(user.Bananas);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var users = await _context.Users.Where(user => !user.IsDelete && user.IsConfirmed)
                .OrderByDescending(user=>user.Victories)
                .ThenBy(user=>user.Defeats)
                .ThenBy(user=>user.DateCreated)
                .ToDictionaryAsync(key => key.Id, user => user);

            int rank = 1;
            var response = users.Select(user => new UserStatistic()
            {
                Rank = rank++,
                UserId = user.Key,
                UserName = user.Value.UserName,
                Battles = user.Value.Battles,
                Victories = user.Value.Victories,
                Defeats = user.Value.Defeats,
            });
            
            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var user = await _utilityService.GetUser();
            var battles = await _context.Battles
                .Where(battle => battle.AttackerId.Equals(user.Id) || battle.OpponentId.Equals(user.Id))
                .Include(battle => battle.Attacker)
                .Include(battle => battle.Opponent)
                .Include(battle => battle.Winner)
                .ToListAsync();

            var history = battles.Select(battle => new BattleHistoryEntry()
            {
                BattleId = battle.Id,
                AttackerId = battle.AttackerId,
                OpponentId = battle.OpponentId,
                YouWon = battle.WinnerId == user.Id,
                AttackerName = battle.Attacker.UserName,
                OpponentName = battle.Opponent.UserName,
                RoundFought = battle.RoundFought,
                WinnerDamageDealt = battle.WinnerDamage,
                BattleDate = battle.BattleTime
            });

            return Ok(history.OrderByDescending(h => h.BattleDate));
        }
    }
}