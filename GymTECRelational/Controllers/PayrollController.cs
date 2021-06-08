using GymTECRelational.EntityFramework;
using GymTECRelational.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GymTECRelational.Controllers
{
    public class PayrollController : ApiController
    {

        GymTECEntities context = new GymTECEntities();
        Tools tools = new Tools();

        /*Metodo para obtener todas las planillas registradas.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de planillas.
        */
        [Route("api/Payroll/getAllPayrolls/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllPayrolls().ToList<Planilla>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para obtener una planilla registrada.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre de la planilla a obtener.
        * Salida: Planilla solicitada.
        */
        [Route("api/Payroll/getPayroll/{payrollName}/{token}")]
        public HttpResponseMessage Get(string payRollName, string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getPayroll(payRollName).ToList<Planilla>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para registrar una nueva planilla.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos de la planilla a registrar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        [Route("api/Payroll/createPayRoll/{token}")]
        public HttpResponseMessage Post([FromBody] Planilla payRoll, string token)
        {
            return tools.createPayroll(payRoll, token);
        }

        /*Metodo para modificar una planilla ya registrada.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos de la planilla a modificar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Payroll/updatePayroll/{currentName}/{token}")]
        public HttpResponseMessage Put(string currentName, string token, [FromBody] Planilla payRoll)
        {
            return tools.updatePayroll(payRoll, token, currentName);
        }

        /*Metodo para eliminar una planilla ya registrada.
       * 
       * Entrada: Token del administrador que realiza la solicitud,nombre de la planilla a eliminar.
       * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        [Route("api/Payroll/deletePayroll/{payRollName}/{token}")]
        public HttpResponseMessage Delete(string token,string payRollName)
        {
            return tools.deleteFromDatabaseOneKey(token, "Planilla", payRollName);
        }
    }
}
