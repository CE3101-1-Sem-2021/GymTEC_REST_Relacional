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
    public class PoductController : ApiController
    {
        Tools tools = new Tools();
        GymTECEntities context = new GymTECEntities();



        /*Metodo para obtener todos los productos registrados.
        * 
        * Entrada: Token del administrador que realiza la solicitud
        * Salida: Lista de productos.
        */
        [Route("api/Product/getAllProducts/{token}")]
        public HttpResponseMessage Get(string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getAllProducts().ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }


        /*Metodo para obtener un producto particular por su codigo de barras.
         * 
         * Entrada:Codigo de barras del producto a solicitar,token del administrador que hace la solicitud.
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Product/getProduct/{id}/{token}")]
        public HttpResponseMessage Get(string id, string token)
        {
            if (tools.tokenVerifier(token, "Admin"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, context.getProduct(id).ToList());
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Token invalido");
        }

        /*Metodo para crear un nuevo producto.
         * 
         * Entrada:Datos del nuevo producto,token del empleado que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Product/createProduct/{token}")]
        public HttpResponseMessage Post([FromBody] Producto product, string token)
        {
            return tools.createProduct(product, token);
        }

        /*Metodo para asignar un producto a una sucursal.
         * 
         * Entrada:Codigo de barras del producto,nombre de la sucursal a la que se va a afiliar el producto,token del empleado que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Product/{type}/{barcode}/{gymName}/{token}")]
        public HttpResponseMessage Post(string type,string barcode,string gymName,string token)
        {
            if (type.Equals("assignProduct"))
            {
                return tools.assignProduct(barcode, gymName, token);
            }
            else if(type.Equals("unsignProduct"))
            {
                return tools.unsignProduct(barcode, gymName, token);
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, "Operacion desconocida");
        }

        /*Metodo para actualizar la informacion de un producto.
         * 
         * Entrada:Codigo de barras del producto que se quiere modificar,token del administrador que realiza la solicitud, nueva informacion del producto
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Product/updateProduct/{currentCode}/{token}")]
        public HttpResponseMessage Put([FromBody] Producto product, string currentCode, string token)
        {
            return tools.updateProduct(currentCode,product, token);
        }

        /*Metodo para eliminar un producto.
         * 
         * Entrada:Codigo de barrras del producto que se quiere eliminar,token del administrador que realiza la solicitud
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        [Route("api/Product/deleteProduct/{id}/{token}")]
        public HttpResponseMessage Delete(string token, string id)
        {
            return tools.deleteFromDatabase(token, "Producto", id,null);
        }
    }
}
