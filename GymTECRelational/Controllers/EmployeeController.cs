using GymTECRelational.EntityFramework;
using GymTECRelational.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

namespace GymTECRelational.Controllers
{
    public class EmployeeController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();



        /*Metodo para obtener todos los empleados registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de empleados.
        */
        [Route("api/Employee/getAllEmployees/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if(tools.tokenVerifier(token,"Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.selectAllAdmins().ToList<Empleado>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }


        /*Metodo para obtener un empleado particular por su cedula.
         * 
         * Entrada:Cedula del empleado a solicitar,token del administrador que hace la solicitud.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Employee/getEmployee/{id}/{token}")]
        public HttpResponseMessage Get(string id,string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getEmployeeById(id).ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para realizar las operaciones de registro y login de los empleados.
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

        /*Metodo para actualizar la informacion de un empleado.
         * 
         * Entrada:Cedula del empleado que se quiere modificar,token del administrador que realiza la solicitud, nueva informacion del empleado
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Employee/updateEmployee/{currentId}/{token}")]
        public HttpResponseMessage Put([FromBody]Empleado employee,string currentId,string token)
        {
            return tools.updateEmployee(currentId,employee,token);
        }

        /*Metodo para eliminar un empleado.
         * 
         * Entrada:Cedula del empleado que se quiere eliminar,token del administrador que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Employee/deleteEmployee/{id}/{token}")]
        public HttpResponseMessage Delete(string token,string id)
        {
            return tools.deleteFromDatabase(token, "Empleado", id,null);
        }
    }
}
