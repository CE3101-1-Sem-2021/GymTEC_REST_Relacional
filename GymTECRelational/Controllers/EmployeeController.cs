using GymTECRelational.EntityFramework;
using GymTECRelational.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

namespace GymTECRelational.Controllers
{
    public class EmployeeController : ApiController
    {
        Tools tools = new Tools();

        // GET: api/Employee
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Employee/5
        public string Get(int id)
        {
            return "value";
        }

        /*Metodo para realizar lass operaciones de registro y login de los empleados.
         * 
         * Entrada:Datos del empleado,tipo de operacion a realizar
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Employee/{requestType}")]
        public HttpResponseMessage Post([FromBody]Empleado employee,string requestType)
        {
            if (requestType == "login")
            {
                return tools.loginRequest(employee);
            }
            else if(requestType=="register")
            {
                return tools.registerRequest(employee);
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Operacion no reconocida");
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
        }
    }
}
