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
        [Route("api/Gym/{token}")]
        public List<Sucursal> Get(string token)
        {
            if (tools.tokenVerifier(token,"Admin"))
            {
                var lst=context.selectAllGyms().ToList<Sucursal>();
                return lst;
            }
            return new List<Sucursal>();
        }

        // GET: api/Gym/5
        public string Get(int id)
        {
            return "value";
        }


        /*Metodo para realizar lass operaciones de registro de un nuevo gimnasio.
         * 
         * Entrada:Datos del nuevo gimnasio, Token del administrador que realiza el registro
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Gym/createGym/{token}")]
        public HttpResponseMessage Post([FromBody]Sucursal gym,string token)
        {
            return tools.createGym(gym,token);
        }
        // PUT: api/Gym/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Gym/5
        public void Delete(int id)
        {
        }
    }
}
