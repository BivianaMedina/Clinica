using ClinicaBase.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClinicaBase.Services.ServicioUsuarios
{
    public class ServicioToken : IServicioToken
    {
        private readonly IConfiguration _configuration;

        public ServicioToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(UserTokenClaimsDTO userClaims)
        {
            try
            {
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Secret").Value!));

                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userClaims.Documento.ToString()),
                new Claim(ClaimTypes.Name, userClaims.Nombres.Trim() + " " + userClaims.Apellidos.Trim()),
                new Claim(ClaimTypes.Role, userClaims.Rol),
            };

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(60),
                    signingCredentials: credentials
                    );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;
            }
            catch (Exception)
            {
                return null!;
            }

        }
    }
}
