using AutoMapper ;
using api_rest.Data;
using api_rest.Data.Dtos;
using api_rest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace api_rest.Controller
{
    [ApiController]
    [Route("v1")]
    public class UserController : ControllerBase
    {
        private AppDbContext _context;
        private IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("users")]
        public async Task<List<ReadUserDto>> GetAsync(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadUserDto>>(
                await _context.Users.Skip(skip).Take(take).ToListAsync());
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] string id
         )
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            try {
                var user = _mapper.Map<ReadUserDto>(await _context
                    .Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(userData => userData.Id == userId));

                return user != null ? Ok(user) : BadRequest("Usuário não encontrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.InnerException);
            }
        }

        [HttpPost("users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync(
            [FromBody] CreateUserDto userDto
         )
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = _mapper.Map<User>(userDto);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    null, user);
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

        [HttpPatch("users/{id}")]
        public async Task<IActionResult> PatchAsync (
            [FromBody] JsonPatchDocument<UpdateUserDto> patch,
            [FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            try
            {
                var user = await _context
                  .Users
                  .FirstOrDefaultAsync(userData => userData.Id == userId);

                if (user == null)
                    return NotFound("Usuário não encontrado.");

                var userToUpdate = _mapper.Map<UpdateUserDto>(user);

                patch.ApplyTo(userToUpdate, ModelState);

                if (!TryValidateModel(userToUpdate))
                {
                    return ValidationProblem(ModelState);
                }

                _mapper.Map(userToUpdate, user);
                await _context.SaveChangesAsync();

                return NoContent();
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
            [FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest("ID de usuário inválido.");

            var user = await _context
              .Users
              .FirstOrDefaultAsync(userData => userData.Id == userId);

            if (user == null)
            {
                return BadRequest("Usuário não encontrado.");
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
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
