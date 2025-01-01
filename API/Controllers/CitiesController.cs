using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

[AllowAnonymous]
public class CitiesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCities() => Ok(Seed.SeedCities());
}
