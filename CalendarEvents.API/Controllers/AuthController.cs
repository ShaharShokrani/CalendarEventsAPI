using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CalendarEvents.Controllers
{
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {        
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventsController> _log;
        private readonly IConfiguration _config;

        public AuthController(
            IAuthRepository authRepository, 
            IMapper mapper,
            IConfiguration config,
            ILogger<EventsController> log)
        {
            this._authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            this._mapper = mapper?? throw new ArgumentNullException(nameof(mapper));
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDTO userForRegisterDTO)
        {
            userForRegisterDTO.Email = userForRegisterDTO.Email.ToLower();

            if (await _authRepository.IsUserExists(userForRegisterDTO.Email))
                return BadRequest("Username already exists");

            var userToCreate = _mapper.Map<UserModel>(userForRegisterDTO);

            var createdUser = await _authRepository.Register(userToCreate, userForRegisterDTO.Password);

            var userToReturn = _mapper.Map<UserDetailedDTO>(createdUser);

            return CreatedAtAction(nameof(Register), userToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> IsUserExists(string userName)
        {
            if (await _authRepository.IsUserExists(userName.ToLower()))
                return Ok(true);
            else
                return Ok(false);            
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            var userFromRepo = await _authRepository.Login(userLoginDTO.Email
                .ToLower(), userLoginDTO.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Email)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)); //TODO: Encapsulate it with Config class

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserListDTO>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}