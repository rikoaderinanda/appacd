using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAccountRepository _repo;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory,IAccountRepository repo)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _repo = repo;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest("ID Token tidak boleh kosong.");
            }

            try
            {
                var googleClientId = _configuration["Authentication:Google:ClientId"];

                // Memverifikasi ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });
                
                //validasi email
                if (payload.EmailVerified != true)
                {
                    return Unauthorized("Email Google belum diverifikasi.");
                }

                var validIssuers = new[] { "https://accounts.google.com", "accounts.google.com" };
                if (!validIssuers.Contains(payload.Issuer))
                {
                    return Unauthorized("Issuer Google tidak valid.");
                }

                // payload sekarang berisi data user dari Google
                var userId = payload.Subject;
                var userEmail = payload.Email;
                var userName = payload.GivenName;
                var userPicture = payload.Picture;

                var data = await _repo.CheckUser(userEmail);
                if(data == null){
                    var mo = new GoogleNewUser();
                    mo.UserIdGoogle = userId;
                    mo.Name = userName;
                    mo.Email = userEmail;
                    mo.Picture = userPicture;
                    var id = await _repo.CreateNewUserGoogle(mo);
                    userId = id.ToString();
                }
                else{
                    userId = data.id;
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, userEmail),
                    new Claim("picture", userPicture ?? "")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"])),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new
                {
                    token = jwt,
                    user = new
                    {
                        id = userId,
                        name = userName,
                        email = userEmail,
                        picture = userPicture
                    }
                });
            }
            catch (InvalidJwtException ex)
            {
                // Token tidak valid (kadaluwarsa, tanda tangan tidak cocok, dll.)
                return Unauthorized("ID Token tidak valid, " + ex.Message);
            }
            catch (Exception ex)
            {
                // Tangani error lainnya
                return StatusCode(500, "Terjadi kesalahan pada server, " + ex.Message);
            }
        }
    
        
    }
}