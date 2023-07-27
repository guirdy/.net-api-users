using api_rest.Data;
using api_rest.Model;
using api_rest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_rest.Controller
{
    [ApiController]
    [Route("v1")]
    public class UserController : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            try
            {
                var users = await context
                    .Users
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.Message);
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] string id
         )
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            try {
                var user = await context
                    .Users
                    .Where(userData => userData.Id == Guid.Parse(id))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return user != null ? Ok(user) : BadRequest("Usuário não encontrado."); ;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.InnerException);
            }
        }

        [HttpPost("users")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateUserViewModel model
         )
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = new User
            {
                Date = DateTime.Now,
                Name = model.Name,
                Age = model.Age,
                Gender = model.Gender,
            };

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Created($"v1/users/{user.Id}", user);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Erro de atualização no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.Message);
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> PutAsync (
            [FromServices] AppDbContext context,
            [FromBody] UpdateUserViewModel model,
            [FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            try
            {
                var user = await context
                  .Users
                  .Where(userData => userData.Id == Guid.Parse(id))
                  .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound("Usuário não encontrado.");

                if (model.Name != null && !model.Name.Equals(user.Name))
                {
                    user.Name = model.Name;
                }
                if (model.Gender != null && !model.Gender.Equals(user.Gender))
                {
                    user.Gender = model.Gender;
                }
                if (model.Age > 0 && model.Age != user.Age)
                {
                    user.Age = model.Age;
                }

                await context.SaveChangesAsync();

                return Ok(user);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Erro de atualização no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.Message);
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            var user = await context
              .Users
              .Where(userData => userData.Id == Guid.Parse(id))
              .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Usuário não encontrado.");
            }

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Erro de atualização no banco de dados: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.Message);
            }
        }
    }
}
