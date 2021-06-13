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
    public class GymController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();


        /*Metodo para obtener la informacion de todos los gimnasios.
         * 
         * Entrada:Token del administrador que hace la solicitud
         * Salida: Lista de los gimnasios registrados.
         */
        [Route("api/Gym/getAllGyms/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token,"Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.selectAllGyms().ToList<Sucursal>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict,"Token invalido");
        }

        /*Metodo para obtener un gimnasioo ne particular por su nombre.
         * 
         * Entrada:Nombre del gimnsaio a solicitar,token del administrador que hacela solicitud.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/getGym/{gymName}/{token}")]
        public HttpResponseMessage Get(string gymName,string token)
        {
            if(tools.tokenVerifier(token,"Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK,context.selectGym(gymName).ToList<Sucursal>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict,"Token invalido");
        }


        /*Metodo para realizar las operaciones de registro de un nuevo gimnasio.
         * 
         * Entrada:Datos del nuevo gimnasio, Token del administrador que realiza el registro
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/createGym/{token}")]
        public HttpResponseMessage Post([FromBody]Sucursal gym,string token)
        {
            return tools.createGym(gym,token);
        }

        /*Metodo para cambiar el estado de el spa o la tienda de un gimnasio en particular.
         * 
         * Entrada:Operacion a realizar,estado a colocar,nombre del gimnasio,token del administrador que realiza la operacion
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/{operation}/{state}/{gymName}/{token}")]
        public HttpResponseMessage Post(string token,string operation,int state,string gymName)
        {
            return tools.gymSpecialsActivation(token, operation,state,gymName);
        }

        /*Metodo para copiar un gimnasio
         * 
         * Entrada:Token del administrador que realiza la solicitud,nombre del gimnasio que se quiere copiar
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa
         */
        [Route("api/Gym/copyGym/{gymName}/{token}")]
        public HttpResponseMessage Post(string gymName,string token)
        {
            return tools.copyGym(gymName, token);
        }
        /*Metodo para actualizar la informacion de un gimnasio.
         * 
         * Entrada:Nombre del gimnasio que se quiere modificar,token de ladministrador que relaiza la solicitud,nueva informacion del gimnasio
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/updateGym/{gymName}/{token}")]
        public HttpResponseMessage Put(string gymName,string token,[FromBody]Sucursal gym)
        {
            return tools.updateGym(gymName, token, gym);
        }

        /*Metodo para eliminar un gimnasio.
         * 
         * Entrada:Nombre del gimnasio que se quiere eliminar,token de ladministrador que relaiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/deleteGym/{gymName}/{token}")]
        public HttpResponseMessage Delete(string token,string gymName)
        {
            return tools.deleteFromDatabase(token, "Sucursal", gymName,null);
        }
    }
}
