using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Excambo.Data;
using Excambo.Models;
using System.Data;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;

[Route("api/oportuny")]

public class OportunyController : ControllerBase
{
    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<List<User>>> SearchForGamesWithoutCondition([FromServices] DataContext context)
    {
        var geometryFactory = NtsGeometryServices
        .Instance
        .CreateGeometryFactory(srid: 4326)
        .CreatePoint(new Coordinate(-22.758400436342708, -43.32376836927987));

        var exchangeOportuny = await context.User
        .OrderBy(c => c.GeographicData.Distance(geometryFactory))
        .Where(c => c.GeographicData != null)
        .Select(c => new
        {
            c.IdUser,
            c.FirstName,
            Distance = geometryFactory.Distance(c.GeographicData)
        })
        .AsNoTracking()
        .ToListAsync();

        return Ok(exchangeOportuny);
    }

}