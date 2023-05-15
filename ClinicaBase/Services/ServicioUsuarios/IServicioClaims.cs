using ClinicaBase.Models.DTOs;
using ClinicaBase.Responses;

namespace ClinicaBase.Services.ServicioUsuarios
{
    public interface IServicioClaims
    {
        public GeneralResponse GuardarClaims(UserClaimsDTO userClaims);

    }
}
