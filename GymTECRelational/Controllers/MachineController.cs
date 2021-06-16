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
    public class MachineController : ApiController
    {
        private GymTECEntities context = new GymTECEntities();
        private Tools tools = new Tools();


        /*Metodo para obtener todos las maquinas registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de maquinas.
        */
        [Route("api/Maquina/getAllMachines/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token,"Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllMachines().ToList<Maquina>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");

        }

        /*Metodo para obtener una maquina registrada.
        * 
        * Entrada: Token del administrador que realiza la solicitud,serial de la maquina a obtener.
        * Salida: Maquina solicitada.
        */
        [Route("api/Maquina/getMachine/{type}/{column}/{token}")]
        public HttpResponseMessage Get(string column,string token,string type)
        {
            if(tools.tokenVerifier(token,"Administrador"))
            {
                if (type.Equals("Single"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, context.getMachine(column).ToList<Maquina>());
                }
                else if(type.Equals("gymMachines"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, context.getMachinesByGym(column).ToList<Maquina>());
                }
            }
            return Request.CreateResponse(HttpStatusCode.Conflict,"Token invalido");
        }

        /*Metodo para registrar una nueva maquina.
       * 
       * Entrada: Token del administrador que realiza la solicitud,datos de la maquina a registrar.
       * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
       */
        [Route("api/Maquina/addMachine/{token}")]
        public HttpResponseMessage Post([FromBody]Maquina machine,string token)
        {
            return tools.createMachine(machine, token);
        }

        /*Metodo para modificar una maquina ya registrada.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos de la maquina a modificar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Maquina/updateMachine/{currentSerial}/{token}")]
        public HttpResponseMessage Put(string token,string currentSerial,[FromBody]Maquina machine)
        {
            return tools.updateMachine(machine, token,currentSerial);
        }

        /*Metodo para eliminar una maquina ya registrada.
        * 
        * Entrada: Token del administrador que realiza la solicitud,serial de la maquina a eliminar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Maquina/updateMachine/{serial}/{token}")]
        public HttpResponseMessage Delete(string serial,string token)
        {
            return tools.deleteFromDatabase(token, "Maquina", serial,null);
        }
    }
}
