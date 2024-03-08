using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Excambo.Data;
using Excambo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/games")]
public class GamesController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any,Duration = 10)]
    public async Task<ActionResult<MyGame>> GetGames([FromServices] DataContext context)
    {
        var games = await context.Mygame.Include(x => x.User).AsNoTracking().ToListAsync();
        return Ok(games);
    }


    [HttpGet]
    [Route("{id:int}")]
    [Authorize]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any,Duration = 10)]
    public async Task<ActionResult<List<MyGame>>> GetGame(int id, [FromServices] DataContext context)
    {
        var games = await context.Mygame.Include(x => x.User).AsNoTracking().FirstOrDefaultAsync(x => x.IdUser == id);
        return Ok(games);
    }

    [HttpGet]
    [Route("user/{id:int}")]
    [Authorize]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any,Duration = 10)]
    public async Task<ActionResult<List<MyGame>>> GetGameUser(int id, [FromServices] DataContext context)
    {
        var games = await context.Mygame
            .Include(x => x.User)
            // .ThenInclude(name => name.FirstName)
            .AsNoTracking()
            .Where(x => x.IdUser == id)
            .ToListAsync();
        return Ok(games);
    }

    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<List<MyGame>>> InsertGame([FromBody] MyGame model, [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Mygame.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível cadastrar o jogo" });
        }
    }

}

