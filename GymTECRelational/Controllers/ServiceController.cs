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
    public class ServiceController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();



        /*Metodo para obtener todos los tipos de servicio registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de tipos.
        */
        [Route("api/Service/getAllServices/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllServices().ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }


        /*Metodo para obtener un tipo de servicio particular por su nombre.
         * 
         * Entrada:Nombre del sevicio a solicitar,token del administrador que hace la solicitud.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Service/getService/{id}/{token}")]
        public HttpResponseMessage Get(string id, string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getService(id).ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para crear un nuevo tipo de servicio.
         * 
         * Entrada:Datos del nuevo servicio,token del empleado que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Service/createService/{token}")]
        public HttpResponseMessage Post([FromBody] Tipo_Servicio service, string token)
        {
            return tools.createService(service, token);
        }

        /*Metodo para actualizar la informacion de un tipo de servicio.
         * 
         * Entrada:Nombre del servicio que se quiere modificar,token del administrador que realiza la solicitud, nueva informacion del tipo de servicio
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Sevice/updateService/{currentName}/{token}")]
        public HttpResponseMessage Put([FromBody] Tipo_Servicio service, string currentName, string token)
        {
            return tools.updateService(currentName,service, token);
        }

        /*Metodo para eliminar un tipo de servicio.
         * 
         * Entrada:Nombre del servicio que se quiere eliminar,token del administrador que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Service/deleteService/{id}/{token}")]
        public HttpResponseMessage Delete(string token, string id)
        {
            return tools.deleteFromDatabase(token, "Tipo_Servicio", id,null);
        }
    }
}
