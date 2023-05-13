using ClinicaBase.Models.ViewModels;
using ClinicaBase.Responses;
using ClinicaBase.Services.ServicioUsuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaBase.Controllers
{
    public class AuthController : Controller
    {
        private readonly IServicioUsuarios _servicioUsuarios;

        public AuthController(IServicioUsuarios servicioUsuarios)
        {
            _servicioUsuarios = servicioUsuarios;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UsuarioAuthViewModel request)
        {
            GeneralResponse response = new();

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            response = await _servicioUsuarios.Auth(request);

            if (response.Succeed == 0)
            {
                request.Succeed = response.Succeed;
                request.Message = response.Message;
                return View(request);
            }

            Console.WriteLine(response.Data);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Register(RegisterViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            GeneralResponse response = new();
            response = await _servicioUsuarios.AddUsuario(request);

            if (response.Succeed == 1)
            {
                return RedirectToAction("Index", "Home", response);
            }            
            request.Succeed = response.Succeed;
            request.Message = response.Message;
            return View(request);
        }
    }
}
