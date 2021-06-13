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
    public class PhoneController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();

        [Route("api/Phone/getPhones/{gymName}/{token}")]
        public List<Sucursal_Telefono> Get(string gymName)
        {
            return context.getAllPhoneNumbByGym(gymName).ToList<Sucursal_Telefono>();
        }

        [Route("api/Phone/addPhone/{token}")]
        public HttpResponseMessage Post([FromBody]Sucursal_Telefono phoneNumb,string token)
        {
            return tools.addPhoneNumber(phoneNumb,token);
        }

        [Route("api/Phone/updatePhone/{currentNumber}/{token}")]
        public HttpResponseMessage Put(string currentNumber,string token,[FromBody]Sucursal_Telefono phoneNumb)
        {
            return tools.updatePhoneNumber(currentNumber, token, phoneNumb);
        }

        [Route("api/Phone/deletePhone/{phoneNumber}/{token}")]
        public HttpResponseMessage Delete(string token,string phoneNumber)
        {
            return tools.deleteFromDatabase(token, "SucursalTelefono", phoneNumber,null);
        }
    }
}
