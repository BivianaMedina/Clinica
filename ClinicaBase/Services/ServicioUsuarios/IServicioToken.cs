using ClinicaBase.Models.DTOs;

namespace ClinicaBase.Services.ServicioUsuarios
{
    public interface IServicioToken
    {
        public string CreateToken(UserTokenClaimsDTO userClaims);
    }
}
