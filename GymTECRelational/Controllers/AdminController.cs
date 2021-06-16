using GymTECRelational.EntityFramework;
using GymTECRelational.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GymTECRelational.Controllers
{
    public class AdminController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();
        /*Metodo para obtener todos los administradores registrados.
         * 
         * Entrada:-
         * Salida: Lista de administradores.
         */
        public List<Empleado> Get()
        {
            
            return context.selectAllAdmins().ToList<Empleado>();
            
        }

        [Route("api/Admin/getAdmin/{id}/{token}")]
        public HttpResponseMessage Get(string id,string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAdminById(id));
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token Invalido");
        }

        /*Metodo para realizar las operaciones de registro y login de los administradores.
         * 
         * Entrada:Datos del administrador,tipo de operacion a realizar
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Admin/{requestType}")]
        public HttpResponseMessage Post([FromBody] Empleado admin, string requestType)
        {
            if (requestType == "login")
            {
                admin.Puesto = "Administrador";
                return tools.loginRequest(admin);
            }
            else if (requestType == "register")
            {
                admin.Puesto = "Administrador";
                return tools.registerRequest(admin);
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Operacion no reconocida");
        }

        // PUT: api/Admin/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Admin/5
        public void Delete(int id)
        {
        }
    }
}
