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
    public class ScheduleController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();

        [Route("api/Schedule/getSchedules/{gymName}/{token}")]
        public HttpResponseMessage Get(string gymName,string token)
        {
            if (tools.tokenVerifier(token, "Administrador"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllSchedulesByGym(gymName).ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        [Route("api/Schedule/addSchedule/{token}")]
        public HttpResponseMessage Post([FromBody] Sucursal_Horario schedule, string token)
        {
            return tools.addSchedule(schedule, token);
        }

        [Route("api/Schedule/updateSchedule/{currentDay}/{token}")]
        public HttpResponseMessage Put(string currentDay, string token, [FromBody] Sucursal_Horario schedule)
        {
            return tools.updateSchedule(currentDay,schedule,token);
        }

        [Route("api/Schedule/deleteSchedule/{day}/{gymName}/{token}")]
        public HttpResponseMessage Delete(string token, string day,string gymName)
        {
            return tools.deleteFromDatabase(token, "SucursalHorario",day,gymName);
        }
    }
}
