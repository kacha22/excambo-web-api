using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Excambo.Data;
using Excambo.Models;
using ExcamboWebApi.Services;
using NetTopologySuite.Geometries;
using NetTopologySuite;

[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [Authorize]
    public async Task<ActionResult<List<User>>> GetUser([FromServices] DataContext context)
    {
        var listUsers = await context.User.AsNoTracking().ToListAsync();

        var readerUsers = from readUsers in listUsers
                          select new
                          {
                              readUsers.IdUser,
                              readUsers.FirstName,
                              readUsers.UserEmail
                          };
        return Ok(readerUsers);
    }

    [HttpGet]
    [Authorize]
    [Route("{id:int}")]
    public async Task<ActionResult<User>> GetUserId(int id, [FromServices] DataContext context)
    {
        var getUser = await context.User.AsNoTracking().Select(p => new
        {
            IdUser = p.IdUser,
            FirstName = p.FirstName,
            userEmail = p.UserEmail
        })
        .FirstOrDefaultAsync(x => x.IdUser == id);

        if (getUser == null)
        {
            return BadRequest(new { message = "Usuário não encontrado" });
        }
        else
        {
            return Ok(getUser);
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<List<User>>> InsertUser([FromBody] User model, [FromServices] DataContext context)
    {
        var geometryFactory = NtsGeometryServices
        .Instance
        .CreateGeometryFactory(srid: 4326)
        .CreatePoint(new Coordinate(-22.758400436342708, -43.32376836927987));
        
        var user = await context.User.AsNoTracking().FirstOrDefaultAsync(x => x.UserEmail == model.UserEmail);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (user == null)
            {
                context.User.Add(model);
                await context.SaveChangesAsync();
                model.PasswordHash = "";
                model.GeographicData = geometryFactory;
                return Ok(model);
            }
            else
            {
                return BadRequest(new { message = "E-mail já cadastrado" });
            }
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível criar o usuário" });
        }
    }

    [HttpPut]
    [Authorize]
    [Route("{id:int}")]
    public async Task<ActionResult<User>> PutUser(int id, [FromBody] User model, [FromServices] DataContext context)
    {
        if (model.IdUser != id)
            return NotFound(new { message = "Usuário não encontrado" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<User>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado" });
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível atualizar o usuário" });
        }
    }

    [HttpPatch]
    [Route("{id:int}")]
    [Authorize]
    public async Task<ActionResult<User>> Patch(int id, [FromBody] JsonPatchDocument<User> patchUser, [FromServices] DataContext context)
    {

        if (patchUser == null)
        {
            return BadRequest();
        }

        var user = await context.User.FirstOrDefaultAsync(x => x.IdUser == id);

        if (user == null)
        {
            return NotFound();
        }
        patchUser.ApplyTo(user, ModelState);
        var isValid = TryValidateModel(ModelState);
        if (!isValid)
        {
            return BadRequest(ModelState);
        }
        await context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete]
    [Route("delete/{id:int}")]
    [Authorize]
    public async Task<ActionResult<User>> DeleteUser(int id, [FromServices] DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await context.User.FirstOrDefaultAsync(x => x.IdUser == id);
        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }
        try
        {
            context.User.Remove(user);
            await context.SaveChangesAsync();
            return Ok(new { message = "Usuário excluído" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Usuário não encontrado" });
        }
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<dynamic>> AuthenticateUser([FromServices] DataContext context, [FromBody] User model)
    {
        var user = await context.User.AsNoTracking().Where(x => x.UserEmail == model.UserEmail && x.PasswordHash == model.PasswordHash).FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new { message = "Usuário ou senha inválidos" });
        }
        var token = TokenService.GenerateToken(user);
        return new
        {
            token = token
        };
    }

}