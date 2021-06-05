using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using GymTECRelational.EntityFramework;

namespace GymTECRelational.Models
{
    public class Tools
    {
        private GymTECEntities context;
        //Constructor standard de la clase
        public Tools()
        {
            context = new GymTECEntities();
        }

        /*Metodo para realizar la operacion de registro.
         * 
         * Entrada:Datos del nuevo usuario
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage registerRequest(Object user)
        {
            HttpResponseMessage response = null;
            
            if (user is Empleado)
            {
                Empleado employee = (Empleado)user;
                
                if (context.Empleadoes.Any(o => o.Cedula == employee.Cedula))
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("La cedula provista ya esta registrada!");
                    return response;
                }
                if (context.Empleadoes.Any(o => o.Email == employee.Email))
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El correo porvisto ya esta registrado!");
                    return response;
                }
                employee.Token = getToken();
                List<string> cryptoComponents = md5Encryption(employee.Contraseña);

                employee.Contraseña = cryptoComponents[0];
                employee.Salt = cryptoComponents[1];

                try
                {
                    context.Empleadoes.Add(employee);
                    context.SaveChanges();
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Empleado registrado correctamente!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Error inesperado!");

                }
            }
            else if(user is Administrador)
            {
                Administrador admin = (Administrador)user;
                if (context.Administradors.Any(o => o.Email == admin.Email))
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El correo porvisto ya esta registrado!");
                    return response;
                }
                admin.Token = getToken();
                List<string> cryptoComponents = md5Encryption(admin.Contraseña);

                admin.Contraseña = cryptoComponents[0];
                admin.Salt = cryptoComponents[1];

                try
                {
                    context.Administradors.Add(admin);
                    context.SaveChanges();
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Administrador registrado correctamente!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Error inesperado!");

                }

            }
            return response;
            
        }

        /*Metodo para realizar la operacion de login.
         * 
         * Entrada:Credenciales del usuario
         * Salida: Token para la sesion del usuario.
         */
        public HttpResponseMessage loginRequest(Object credentials)
        {
            HttpResponseMessage response = null;
            if (credentials is Empleado)
            {
                Empleado employeeCred = (Empleado)credentials;
                if (context.Empleadoes.Any(o => o.Email == employeeCred.Email))
                {

                    Empleado employee = context.getEmployeeByMail(employeeCred.Email).ToList<Empleado>()[0];

                    if (employee.Puesto == employeeCred.Puesto)
                    {
                        if (employee.Puesto == "Sin Asignar")
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("El empleado no tiene acceso al sistema");
                            return response;
                        }
                        if (passwordVerifier(employeeCred.Contraseña, employee.Contraseña, employee.Salt))
                        {
                            var token = getToken();
                            context.assignTokenEmployee(token, employee.Cedula);
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent(token);
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El empleado no desempeña el puesto indicado");
                    return response;

                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Contraseña o correo incorrecto");
                return response;
            }
            else if (credentials is Administrador)
            {
                Administrador adminCred = (Administrador)credentials;
                if (context.Administradors.Any(o => o.Email == adminCred.Email))
                {
                    Administrador admin = context.getAdminByMail(adminCred.Email).ToList<Administrador>()[0];
                    if(passwordVerifier(adminCred.Contraseña,admin.Contraseña,admin.Salt))
                    {
                        var token = getToken();
                        context.assignTokenAdmin(token, admin.Id);
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent(token);
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Contraseña o correo incorrecto");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Tipo de usuario desconocido");
            return response;
        }
        /*Metodo para crear un nuevo gimnasio.
         * 
         * Entrada:Informacion del nuevo gimnasio,token del administrador que realiza el registro.
         * Salida: Lista que contiene la contraseña encriptada y su sal asociada.
         */
        public HttpResponseMessage createGym(Sucursal gym,string token)
        {
            HttpResponseMessage response = null;
            if (tokenVerifier(token,"Admin"))
            {
                if (!context.Sucursals.Any(o => o.Nombre == gym.Nombre))
                {
                    try
                    {
                        context.Sucursals.Add(gym);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Gimnasio creado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado!");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nombre de la nueva sucursal ya se encuentra registrado!");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("El nombre de la nueva sucursal ya se encuentra registrado!");
            return response;
        }

        /*Metodo para obtener un token unico.
         * 
         * Entradas:-
         * Salidas:Token unico.
         */
        public string getToken()
        {
            string g = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            g = g.Replace("+", "");
            return g.Replace("/", "");
        }
        /*Metodo para encriptar contraseñas.
         * 
         * Entrada:Contraseña a encriptar
         * Salida: Lista que contiene la contraseña encriptada y su sal asociada.
         */
        public List<string> md5Encryption(string input)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[10];
            rng.GetBytes(buff);

            string salt = Convert.ToBase64String(buff);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(input+salt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();
            return new List<string>(new string[]{result,salt});
        }

        /*Metodo para saber si una contraseña si nencriptar corresponde a una contraseña encriptada.
         * 
         * Entrada:Contraseña encriptada,contraseña a comparar
         * Salida: Condicion que indica si las 2 contraseñas son correspondientes
         */
        public bool passwordVerifier(string password,string encrypted,string salt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(password + salt));
            string result = BitConverter.ToString(bytes).Replace("-", String.Empty).ToLower();

            return result.Equals(encrypted);
        }

        /*Metodo para verificar que un token sea valido.
         * 
         * Entrada:Token a verificar,tipo de usuario al que pertenece
         * Salida: Booleano que indica si el token es valido.
         */
        public bool tokenVerifier(string token,string type)
        {
            if(type.Equals("Admin"))
            {
                return context.Administradors.Any(o => o.Token == token);
            }
            return context.Empleadoes.Any(o => o.Token == token);
        }


    }
}