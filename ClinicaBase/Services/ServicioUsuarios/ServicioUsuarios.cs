using Azure.Core;
using ClinicaBase.Data;
using ClinicaBase.Models.DTOs;
using ClinicaBase.Models.Entities;
using ClinicaBase.Models.ViewModels;
using ClinicaBase.Responses;
using ClinicaBase.Services.ServicioHash;
using Microsoft.EntityFrameworkCore;

namespace ClinicaBase.Services.ServicioUsuarios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly ClinicaBase1Context _context;
        private readonly IServicioHash _servicioHash;
        private readonly IServicioToken _servicioToken;

        private const string GeneralError = "Se ha generado un error inesperado";
        private const string UserOrPasswordNotFound = "El usuario o contraseña no coinciden";

        public ServicioUsuarios(ClinicaBase1Context context, IServicioHash servicioHash,
            IServicioToken servicioToken)
        {
            _context = context;
            _servicioHash = servicioHash;
            _servicioToken = servicioToken;
        }

        public async Task<GeneralResponse> AddUsuario(RegisterViewModel request)
        {
            GeneralResponse response = new();
            BuscarExistenciaDocumento(request.Documento, out bool existeDocumento);
            if (existeDocumento)
            {
                response.Succeed = 0;
                response.Message = "Este usuario ya se encuentra registrado.";
                return response;
            }

            NewRegisterModel(request, out User? newRequestModel);
            if (newRequestModel == null){
                response.Succeed = 0;
                response.Message = GeneralError;
                return response;
            }

            _servicioHash.CreateHashPassword(newRequestModel.Contrasena, out string? hashPasswordSalt, out string? salt);
            if (hashPasswordSalt == null || salt == null)
            {
                response.Succeed = 0;
                response.Message = GeneralError;
                return response;
            }
            newRequestModel.Contrasena = hashPasswordSalt;
            newRequestModel.Sal = salt;
            response = await CreateUser(newRequestModel); 
            return response;
        }


        public async Task<GeneralResponse> Auth(UsuarioAuthViewModel request)
        {
            GeneralResponse response = new();
            User? user = await _context.Users.Where(u => u.Documento == request.Documento).SingleOrDefaultAsync();
            if (user == null)
            {
                response.Message = UserOrPasswordNotFound;
                return response;
            }

            bool usuarioAuthCorrecto = _servicioHash.VerifyHashPassword(request.Contrasena, user.Sal, user.Contrasena);
            if (!usuarioAuthCorrecto)
            {
                response.Message = UserOrPasswordNotFound;
                return response;
            }

            UserTokenClaimsDTO userClaims = new()
            {
                Documento = user.Documento,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                Rol = user.Rol
            };

            string? jwt = _servicioToken.CreateToken(userClaims);

            if (jwt == null)
            {
                response.Message = GeneralError;
            }
            else
            {
                response.Succeed = 1;
                response.Data = jwt;
            }
            return response;
        }


        private void BuscarExistenciaDocumento(int doucmento, out bool response)
        {
            try
            {
                var documentoId = _context.Users.Where(u => u.Documento == doucmento).Select(u => u.Documento).FirstOrDefault();
                if (documentoId == 0)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
                
            }
            catch (Exception)
            {
                response = false;
            }
        }


        private void NewRegisterModel(RegisterViewModel request, out User? newRequestModel)
        {
            try
            {
                User usuario = new()
                {
                    Documento = request.Documento,
                    Nombres = request.Nombres,
                    Apellidos = request.Apellidos,
                    Correo = request.Correo,
                    Telefono = request.Telefono,
                    Contrasena = request.Documento.ToString(),
                    CambioContrasena = 0,
                    Sal = DateTime.Now.ToString(),
                    Rol = request.Rol,
                    Activo = 1
                };

                newRequestModel = usuario;
            }
            catch (Exception)
            {
                newRequestModel = null;
            }
        }


        private async Task<GeneralResponse> CreateUser(User newRequestModel)
        {
            GeneralResponse response = new();
            try
            {
                await _context.Users.AddAsync(newRequestModel);
                await _context.SaveChangesAsync();
                response.Succeed = 1;
                response.Message = "Usuario registrado exitosamente.";
            }
            catch (Exception)
            {
                response.Succeed = 0;
                response.Message = "Se ha generado un error inesperado al momento de guardar la información de usuario.";
            }
            return response;
        }
    }
}
