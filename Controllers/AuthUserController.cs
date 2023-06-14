using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FlowLabourApi.Options;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;

namespace FlowLabourApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthUserController : ControllerBase
    {
        private readonly XiangxpContext _context;
        private readonly UserManager<AuthUser> _userManager;
        private readonly SignInManager<AuthUser> _signInManager;

        public AuthUserController(XiangxpContext context, SignInManager<AuthUser> signInManager,
            UserManager<AuthUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: api/AuthUser
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthUser>>> GetAuthUsers()
        {
            return await _context.AuthUsers.ToListAsync();
        }

        // GET: api/AuthUser/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AuthUser>> GetAuthUser(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return Unauthorized();
            }
            var authUser = await _context.AuthUsers.FindAsync(id);

            if (authUser == null)
            {
                return NotFound();
            }

            return authUser;
        }

        // PUT: api/AuthUser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthUser(int id, AuthUserView authUserView)
        {
            if (id != authUserView.Id)
            {
                return BadRequest();
            }
            AuthUser authUser = new AuthUser
            {
                Id = id,
                Username = authUserView.Username,
                Passwordhash = authUserView.Passwordhash,
                PhoneNo = authUserView.PhoneNo,
                Email = authUserView.Email,
                DateJoined = authUserView.DateJoined,
                IsActive = authUserView.IsActive,
                LastLogin = authUserView.LastLogin
            };
            _context.Entry(authUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthUserExists(id))
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

        // POST: api/AuthUser
        //also use to register
        [HttpPost]
        public async Task<ActionResult<AuthUser>> PostAuthUser(AuthUserView authUserView)
        {
            AuthUser authUser = new AuthUser
            {
                Id = authUserView.Id,
                Username = authUserView.Username,
                Passwordhash = authUserView.Passwordhash,
                PhoneNo = authUserView.PhoneNo,
                Email = authUserView.Email,
                DateJoined = authUserView.DateJoined,
                IsActive = authUserView.IsActive,
                LastLogin = authUserView.LastLogin
            };
            _context.AuthUsers.Add(authUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthUser), new { id = authUser.Id }, authUser);
        }

        // DELETE: api/AuthUser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthUser(int id)
        {
            var authUser = await _context.AuthUsers.FindAsync(id);
            if (authUser == null)
            {
                return NotFound();
            }

            _context.AuthUsers.Remove(authUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/AuthUser/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginView loginView)
        {
            var authUser = await _context.AuthUsers.SingleOrDefaultAsync(u => u.Username == loginView.Username && u.Passwordhash == loginView.Passwordhash);

            if (authUser == null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JWTOptions.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, authUser.Username),
                    new Claim(ClaimTypes.Role, "User"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var Token = tokenHandler.WriteToken(token);

            return Ok(new
            {
                authUser.Id,
                authUser.Username,
                authUser.Email,
                authUser.PhoneNo,
                authUser.IsActive,
                authUser.DateJoined,
                authUser.LastLogin,
                Token,
            });
        }

        // POST: api/AuthUser/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Obtain the id of the authenticated user
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            // Invalidate the authentication token of the user (if any)
            //var user = await _context.AuthUsers.FindAsync(int.Parse(userId));
            //user.Token = null;
            //await _context.SaveChangesAsync();

            return Ok();
        }


        private bool AuthUserExists(int id)
        {
            return _context.AuthUsers.Any(e => e.Id == id);
        }


    }
}
