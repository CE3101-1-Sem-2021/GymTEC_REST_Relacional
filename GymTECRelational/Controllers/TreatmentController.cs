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
    public class TreatmentController : ApiController
    {
        GymTECEntities context = new GymTECEntities();
        Tools tools = new Tools();

        /*Metodo para obtener todos los tratamientos de spa registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de tratamientos.
        */
        [Route("api/Treatment/getAllTreatments/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllTreatments().ToList<Tratamiento_Spa>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para obtener un tratamiento registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre del tratamiento a obtener.
        * Salida: Tratamiento solicitadoo.
        */
        [Route("api/Treatment/getTreatment/{treatmentId}/{token}")]
        public HttpResponseMessage Get(int treatmentId, string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getTreatment(treatmentId).ToList<Tratamiento_Spa>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para registrar una nuevo tratamiento.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos del tratamiento a registrar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        [Route("api/Treatment/createTreatment/{token}")]
        public HttpResponseMessage Post([FromBody] Tratamiento_Spa treatment, string token)
        {
            return tools.createTreatment(treatment, token);
        }


        [Route("api/Treatment/{type}/{treatmentId}/{gymName}/{token}")]
        public HttpResponseMessage Post(string type,int treatmentId,string gymName,string token)
        {
            if (type.Equals("assignTreatment"))
            {
                return tools.assignTreatment(treatmentId,gymName,token);
            }
            else if(type.Equals("unsignTreatment"))
            {
                return tools.unsignTreatment(treatmentId,gymName,token);
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Operacion desconocida");
        }


        /*Metodo para modificar un tratamiento ya registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos del tratamiento a modificar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Treatment/updateTreatment/{token}")]
        public HttpResponseMessage Put(string token, [FromBody] Tratamiento_Spa treatment)
        {
            return tools.updateTreatment(treatment, token);
        }

        /*Metodo para eliminar un tratamiento ya registrado.
       * 
       * Entrada: Token del administrador que realiza la solicitud,nombre del tratamiento a eliminar.
       * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        [Route("api/Treatment/deleteTreatment/{treatmentId}/{token}")]
        public HttpResponseMessage Delete(string token, string treatmentId)
        {
            return tools.deleteFromDatabase(token,"Tratamiento_Spa", treatmentId,null);
        }
    }
}

