using ClinicaBase.Data;
using ClinicaBase.Models.DTOs;
using ClinicaBase.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ClinicaBase.Services.ServicioUsuarios
{
    public class ServicioCookies : IServicioClaims
    {
		private readonly ClinicaBase1Context _context;

		public ServicioCookies(ClinicaBase1Context context)
		{
			_context = context;
		}

        public GeneralResponse GuardarClaims(UserClaimsDTO userClaims)
        {
            GeneralResponse response = new();

			try
			{
				var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userClaims.Documento.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, userClaims.Nombres.Trim() + " " + userClaims.Apellidos.Trim()));
				identity.AddClaim(new Claim(ClaimTypes.Role, userClaims.Rol));

                var principal = new ClaimsPrincipal(identity);

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                //    principal, new AuthenticationProperties
                //    {
                //        ExpiresUtc = DateTime.Now.AddHours(1),
                //        IsPersistent = true
                //    });

                response.Succeed = 1;
                return response;
			}
			catch (Exception)
			{
				return response;
			}
        }

        
    }
}
