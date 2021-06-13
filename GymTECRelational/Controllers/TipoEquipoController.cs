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
    public class TipoEquipoController : ApiController
    {
        GymTECEntities context = new GymTECEntities();
        Tools tools = new Tools();

        /*Metodo para obtener todos los tipos de equiporegistrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de tipos de equipo.
        */
        [Route("api/TipoEquipo/getAllMachineTypes/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK,context.getAllMachineTypes().ToList<Tipo_Equipo>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para obtener un tipo de equipo en particular.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre del tipo de equipo
        * Salida: Tipo de equipo solicitado.
        */
        [Route("api/TipoEquipo/getMachineType/{type}/{token}")]
        public HttpResponseMessage Get(string type, string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getMachineType(type).ToList<Tipo_Equipo>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para crear un nuevo tipo de equipo.
        * 
        * Entrada: Token del administrador que realiza la solicitud,informacion del tipo de equipo
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        [Route("api/TipoEquipo/addMachineType/{token}")]
        public HttpResponseMessage Post([FromBody]Tipo_Equipo machineType,string token)
        {
            return tools.createMachineType(machineType, token);
        }

        /*Metodo para modificar un tipo de equipo.
         * 
         * Entrada: Token del administrador que realiza la solicitud,informacion actualizada del tipo de equipo,nombre actual del tipo de dispositivo
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/TipoEquipo/updateMachineType/{currentName}/{token}")]
        public HttpResponseMessage Put(string currentName,string token,[FromBody]Tipo_Equipo machineType)
        {
            return tools.updateMachineType(machineType, token, currentName);
        }

        /*Metodo para eliminar un tipo de equipo ya registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre del tipo de equipo a eliminar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/TipoEquipo/deleteMachineType/{typeName}/{token}")]
        public HttpResponseMessage Delete(string token,string typeName)
        {
            return tools.deleteFromDatabase(token, "Tipo_Equipo", typeName,null);
        }
    }
}
