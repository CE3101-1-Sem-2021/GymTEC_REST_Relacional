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
    public class ClassController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();



        /*Metodo para obtener todas las clases registradas.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de clases.
        */
        [Route("api/Class/getAllClasses/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllClasses().ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }


        /*Metodo para obtener una clase particular por su identificador.
         * 
         * Entrada:Identificador de la clase a solicitar,token del administrador que hace la solicitud.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Class/getClass/{id}/{token}")]
        public HttpResponseMessage Get(int id, string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getClass(id).ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para obtener las clases que cumplan con ciertos parametros de busqueda
         * 
         * Entrada:Parametros de busqueda,token del usuario que realiza la consulta.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Class/searchClasses/{token}")]
        public HttpResponseMessage Get([FromBody]Clase classInfo, string token)
        {
            
            if (tools.tokenVerifier(token, "Admin"))
            {
                if (classInfo.Hora_Final == new TimeSpan(0, 0, 0))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, context.searchClasses(classInfo.Hora_Inicio, classInfo.Fecha, classInfo.Tipo_Servicio,null, classInfo.Sucursal).ToList());
                }
                return Request.CreateResponse(HttpStatusCode.OK,context.searchClasses(classInfo.Hora_Inicio,classInfo.Fecha,classInfo.Tipo_Servicio,classInfo.Hora_Final,classInfo.Sucursal));
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para crear una clase nueva.
         * 
         * Entrada:Datos de la clase,token del empleado que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Class/createClass/{token}")]
        public HttpResponseMessage Post([FromBody] Clase classInfo, string token)
        {
            return tools.createClass(classInfo, token);
        }

        /*Metodo para actualizar la informacion de una clase.
         * 
         * Entrada:Token del administrador que realiza la solicitud, nueva informacion de la clase
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Class/updateClass/{token}")]
        public HttpResponseMessage Put([FromBody] Clase classInfo, string token)
        {
            return tools.updateClass(classInfo,token);
        }

        /*Metodo para eliminar una clase.
         * 
         * Entrada:Identificador de la clase a eliminar,token del administrador que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Class/deleteClass/{id}/{token}")]
        public HttpResponseMessage Delete(string token, string id)
        {
            return tools.deleteFromDatabase(token, "Clase", id,null);
        }
    }
}
