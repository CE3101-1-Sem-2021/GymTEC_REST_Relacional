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
    public class JobController : ApiController
    {

        GymTECEntities context = new GymTECEntities();
        Tools tools = new Tools();


        /*Metodo para obtener todos los puestos registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de puestos.
        */
        [Route("api/Job/getAllJobs/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK,context.getAllJobs().ToList<Puesto>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Acceso Denegado");
        }

        /*Metodo para obtener un puesto registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre del puesto a obtener.
        * Salida: Puesto solicitado.
        */
        [Route("api/Job/getJob/{jobName}/{token}")]
        public HttpResponseMessage Get(string jobName,string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getJob(jobName).ToList<Puesto>());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para registrar un nuevo puesto de trabajo.
       * 
       * Entrada: Token del administrador que realiza la solicitud,datos del puesto a registrar.
       * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
       */
        [Route("api/Job/createJob/{token}")]
        public HttpResponseMessage Post([FromBody]Puesto job,string token)
        {
            return tools.createJob(job, token);
        }


        /*Metodo para modificar un puesto de trabajo ya registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,datos del puesto a modificar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Job/updateJob/{currentName}/{token}")]
        public HttpResponseMessage Put(string currentName,string token, [FromBody]Puesto job)
        {
            return tools.updateJob(job, token, currentName);
        }

        /*Metodo para eliminar un puesto de trabajo ya registrado.
        * 
        * Entrada: Token del administrador que realiza la solicitud,nombre del puesto a eliminar.
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Job/deleteJob/{nombre}/{token}")]
        public HttpResponseMessage Delete(string nombre,string token)
        {
            return tools.deleteFromDatabase(token, "Puesto", nombre,null);
        }
    }
}
