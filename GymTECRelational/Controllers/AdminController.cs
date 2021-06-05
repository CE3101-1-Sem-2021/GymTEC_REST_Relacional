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

        /*Metodo para obtener todos los administradores registrados.
         * 
         * Entrada:-
         * Salida: Lista de administradores.
         */
        public List<Administrador> Get()
        {
            using (GymTECEntities context =new GymTECEntities())
            {
                return context.selectAllAdmins().ToList<Administrador>();
            }
        }

        // GET: api/Admin/5
        public string Get(int id)
        {
            return "value";
        }

        /*Metodo para realizar lass operaciones de registro y login de los administradores.
         * 
         * Entrada:Datos del administrador,tipo de operacion a realizar
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Admin/{requestType}")]
        public HttpResponseMessage Post([FromBody] Administrador admin, string requestType)
        {
            if (requestType == "login")
            {
                return tools.loginRequest(admin);
            }
            else if (requestType == "register")
            {
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
