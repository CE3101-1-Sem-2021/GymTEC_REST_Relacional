using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        public HttpResponseMessage registerRequest(Empleado employee)
        {
            HttpResponseMessage response = null;
            
                
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
            
            return response;
            
        }

        /*Metodo para realizar la operacion de login.
         * 
         * Entrada:Credenciales del usuario
         * Salida: Token para la sesion del usuario.
         */
        public HttpResponseMessage loginRequest(Empleado credentials)
        {
            HttpResponseMessage response = null;
            

            if (context.Empleadoes.Any(o => o.Email == credentials.Email))
            {

                Empleado employee = context.getEmployeeByMail(credentials.Email).ToList<Empleado>()[0];

                if (employee.Puesto == credentials.Puesto)
                {
                    if (employee.Puesto == "Sin Asignar")
                    {
                        response = new HttpResponseMessage(HttpStatusCode.Conflict);
                        response.Content = new StringContent("El empleado no tiene acceso al sistema");
                        return response;
                    }
                    if (passwordVerifier(credentials.Contraseña, employee.Contraseña, employee.Salt))
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
        

        public HttpResponseMessage updateEmployee(string currentId,Empleado employee,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador")||selfTokenVerifier(token,currentId))
            {
                if(currentId==employee.Cedula||!context.Empleadoes.Any(o=>o.Cedula==employee.Cedula))
                {
                    Empleado temp = context.getEmployeeById(currentId).ToList()[0];
                    if(temp.Email==employee.Email||!context.Empleadoes.Any(o=>employee.Email==employee.Email))
                    {
                        try
                        {
                            context.updateEmployee(currentId,employee.Cedula,employee.Puesto,employee.Planilla,employee.Distrito,employee.Canton,employee.Provincia,employee.Sucursal,employee.Nombre,employee.Apellidos,employee.Salario,employee.Email);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Empleado actualizado correctamente!");
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
                    response.Content = new StringContent("el nuevo correo ya se encuentra registrado por otro empleado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La nueva cedula ya se encuentra registrada por otro empleado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
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
            if (tokenVerifier(token,"Administrador"))
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
            response.Content = new StringContent("Token invalido");
            return response;
        }
        public HttpResponseMessage updateGym(string gymName,string token, Sucursal gym)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.Sucursals.Any(o => o.Nombre == gymName))
                {
                    if(gymName==gym.Nombre||!context.Sucursals.Any(o=>o.Nombre==gym.Nombre))
                    {
                        
                        try
                        {
                            context.updateGym(gymName,gym.Nombre,gym.Distrito,gym.Canton,gym.Provincia,gym.Fecha_Apertura,gym.Capacidad_Max,gym.Gerente);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Gimnasio actualizado correctamente!");
                            return response;
                        }
                        catch(Exception e)
                        {
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado");
                            return response;
                        }
                        
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El nuevo nombre que se le quiere asignar a la sucursal ya se encuentra registrado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La sucursal que se quiere modificar no esta registrada");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }

        /*Metodo para agregar un numero de telefono a un gimnasio.
         * 
         * Entradas:Numero de telefono a agregar,token del administrador que realiza la operacion
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage addPhoneNumber(Sucursal_Telefono phoneNumb,string token)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.Sucursal_Telefono.Any(o=> o.Telefono==phoneNumb.Telefono))
                {
                    try
                    {
                        context.Sucursal_Telefono.Add(phoneNumb);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Numero de telefono agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Ese numero ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para actualizar un numero de telefono de un gimnasio.
         * 
         * Entradas:Numero de telefono actual,token de quien realiza la solicitud,informacion actualizada del numero de telefono
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage updatePhoneNumber(string currentNumber,string token,Sucursal_Telefono phoneNumb)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador"))
            {
                if(phoneNumb.Telefono==currentNumber||!context.Sucursal_Telefono.Any(o=>o.Telefono==phoneNumb.Telefono))
                {
                    if (context.Sucursals.Any(o => o.Nombre == phoneNumb.Sucursal))
                    {
                        try
                        {
                            context.updatePhoneNumb(currentNumber, phoneNumb.Sucursal, phoneNumb.Telefono);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Numero actualizado correctamente!");
                            return response;
                        }
                        catch (Exception e)
                        {
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado");
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("La sucursal a la que se le quiere asignar el nuevo numero no esta registrada");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Ese nuevo numero ya se encuentra registrado");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para agregar un horario a un gimnasio.
         * 
         * Entradas:Horario a agregar,token del administrador que realiza la operacion
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage addSchedule(Sucursal_Horario schedule,string token)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.Sucursal_Horario.Any(o=>o.Dia==schedule.Dia&& o.Sucursal==schedule.Sucursal))
                {
                    try
                    {
                        context.Sucursal_Horario.Add(schedule);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Horario agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException.InnerException.Message.Split('\r')[0].Equals("wrongSchedule"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("El horario de apertura no puede ser mayor al de cierre");
                            return response;
                        }
                        Console.WriteLine(e.ToString());
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado!");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El horario para este dia ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para actualizar un numero de telefono de un gimnasio.
         * 
         * Entradas:Numero de telefono actual,token de quien realiza la solicitud,informacion actualizada del numero de telefono
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage updateSchedule(string currentDay, Sucursal_Horario schedule,string token)
        {
            HttpResponseMessage response = null;
            if (tokenVerifier(token, "Administrador"))
            {
                if (schedule.Dia == currentDay || !context.Sucursal_Horario.Any(o => o.Dia ==schedule.Dia && o.Sucursal==schedule.Sucursal))
                {
                    if (context.Sucursals.Any(o => o.Nombre == schedule.Sucursal))
                    {
                        try
                        {
                            context.updateSchedule(currentDay,schedule.Dia,schedule.Sucursal,schedule.Hora_Apertura,schedule.Hora_Cierre);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Horario actualizado correctamente!");
                            return response;
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException.Message.Split('\r')[0].Equals("wrongSchedule"))
                            {
                                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                                response.Content = new StringContent("El horario de apertura no puede ser mayor al de cierre");
                                return response;
                            }
                            Console.WriteLine(e.ToString());
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado!");
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("La sucursal a la que se le quiere asignar el nuevo horario no esta registrada");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Ya existe un horario asignado para este dia");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para activar y desactivar la tienda y spa de un gimnasio en particular.
         * 
         * Entradas:Token del administrador que realiza la operacion,operacion a realizar,nombre del gimnasio
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage gymSpecialsActivation(string token,string operation,int state,string gymName)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {
                if(context.Sucursals.Any(o=>o.Nombre==gymName))
                {
                    if(operation.Equals("activateSpa"))
                    {
                        context.activateGymSpa(Convert.ToBoolean(state), gymName);
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Se ha cambiado el estado del Spa correctamente");
                        return response;
                    }
                    else if(operation.Equals("activateStore"))
                    {
                        context.activateGymStore(Convert.ToBoolean(state), gymName);
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Se ha cambiado el estado de la tienda correctamente");
                        return response;
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Operacion desconocida");
                    return response;

                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La sucursal indicada no se encuentra registrada");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para copiar un gimnasio
         * 
         * Entrada:Token del administrador que realiza la solicitud,nombre del gimnasio que se quiere copiar
         * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa
         */
        public HttpResponseMessage copyGym(string gymName, string token)
        {
            HttpResponseMessage response = null;
            if (tokenVerifier(token, "Administrador"))
            {
                if (context.Sucursals.Any(o => o.Nombre == gymName))
                {
                    string newName = gymName;
                    int ind = 1;
                    while (context.Sucursals.Any(o => o.Nombre == newName))
                    {
                        newName = gymName + "_copia_" + ind.ToString();
                        ind++;
                    }
                    Sucursal gymCopy = context.Sucursals.AsNoTracking().FirstOrDefault(m => m.Nombre == gymName);
                    gymCopy.Nombre = newName;
                    HttpResponseMessage temp = createGym(gymCopy, token);
                    if (temp.StatusCode.Equals(HttpStatusCode.Conflict))
                    {
                        return temp;
                    }
                    List<Tratamiento_Spa> treatments = context.getTreatmentsByGym(gymName).ToList();
                    foreach (Tratamiento_Spa treatment in treatments)
                    {
                        context.assignTreatment(treatment.Id, newName);
                    }

                    List<Producto> products = context.getProductsByGym(gymName).ToList();
                    foreach (Producto product in products)
                    {
                        context.assignProduct(product.Codigo_Barras, newName);
                    }

                    List<Sucursal_Horario> schedules = context.Sucursal_Horario.AsNoTracking().Where(m=>m.Sucursal==gymName).ToList();
                    foreach(Sucursal_Horario schedule in schedules)
                    {
                        schedule.Sucursal = newName;
                        context.Sucursal_Horario.Add(schedule);
                    }

                    List<Clase> classes = context.getClassesByGym(gymName).ToList();
                    foreach (Clase classInfo in classes)
                    {
                        classInfo.Instructor = null;
                        classInfo.Sucursal = newName;
                        createClass(classInfo, token);
                    }

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Gimnasio copiado correctamente");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El gimnasio que se quiere copiar no se encuentra registrado en el sistema");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para agregar una clase a la base de datos.
         * 
         * Entradas:Token del administrador que realiza la operacion,informacion de la classe a agregar
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage createClass(Clase classInfo,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador")||tokenVerifier(token,"Instructor"))
            {
                if(classInfo.Hora_Inicio==classInfo.Hora_Final)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Las horas de inicio y finalizacion no pueden ser las mismas");
                    return response;
                }
                if (classDayScheduleVerifier(classInfo))
                {
                    try
                    {
                        context.Clases.Add(classInfo);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Clase creada correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException.InnerException.Message.Split('\r')[0].Equals("wrongSchedule"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("El horario de inicio de la clase debe ser menor al horario de finalizacion");
                            return response;
                        }
                        Console.WriteLine(e.ToString());
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado!");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La fecha de la clase no calza en el horario del gimnasio");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para agregar una clase a la base de datos.
         * 
         * Entradas:Token del administrador que realiza la operacion,informacion de la classe a agregar
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage updateClass(Clase classInfo, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador")||tokenVerifier(token,"Instructor"))
            {
                if (classDayScheduleVerifier(classInfo))
                {
                    try
                    {
                        context.updateClass(classInfo.Id, classInfo.Hora_Inicio, classInfo.Fecha, classInfo.Tipo_Servicio, classInfo.Hora_Final, classInfo.Sucursal, classInfo.Instructor, classInfo.Modalidad, classInfo.Capacidad);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Clase actualizada correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException.Message.Split('\r')[0].Equals("wrongSchedule"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("El horario de inicio de la clase no puede ser nas tarde que el horario de finalizacion");
                            return response;
                        }
                        Console.WriteLine(e.ToString());
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado!");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La fecha de la clase no calza en el horario del gimnasio");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para inscribirse en uan clase.
         * 
         * Entradas:Identificador de la clase, identificador del cliente
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage registerClass(int classId,string clientId)
        {
            HttpResponseMessage response = null;
            Clase classInfo = context.getClass(classId).ToList()[0];
            if(!context.Cliente_Clase.Any(o=>o.Id==classId&&o.Cliente==clientId))
            {
                context.registerClass(clientId, classId);

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("Inscripcion realizada correctamente");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("El cliente ya se encuentra registrado en esta clase");
            return response;
        }


        /*Metodo para copiar el calendario de la semana actual a una semana en el futuro.
         * 
         * Entradas:Token del administrador que realiza la operacion,nombre de la sucursal,cantidad de semanas que se adelantara el calendario
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage copyWeekSchedule(int weekadvantage,string gymName,string token)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador")||tokenVerifier(token,"Instructor"))
            {
                DayOfWeek weekBeginning = DayOfWeek.Monday;
                DateTime initialDate = DateTime.Now.AddDays(-1 * ((7 + (DateTime.Now.DayOfWeek - weekBeginning)) % 7)).Date;
                DateTime finalDate = initialDate.AddDays(6).Date;
                List<Clase> clases=context.Clases.Where(m=>m.Sucursal==gymName && (m.Fecha>=initialDate && m.Fecha<=finalDate)).ToList();
                foreach(Clase classInfo in clases)
                {
                    classInfo.Fecha = classInfo.Fecha.AddDays(7*weekadvantage).Date;
                    createClass(classInfo,token);
                }
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("Calendario semanal copiado correctamente");
                return response;


            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para agregar una maquina a la base de datos.
         * 
         * Entradas:Token del administrador que realiza la operacion,informacion de la maquina a agregar
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage createMachine(Maquina machine,string token)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.Maquinas.Any(o=>o.Serial==machine.Serial))
                {
                    if(context.Tipo_Equipo.Any(o=>o.Nombre==machine.Tipo_Equipo))
                    {
                        if(machine.Sucursal==null||context.Sucursals.Any(o=>o.Nombre==machine.Sucursal))
                        {
                            try
                            {
                                context.Maquinas.Add(machine);
                                context.SaveChanges();
                                response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Content = new StringContent("Maquina agregada correctamente!");
                                return response;
                            }
                            catch (Exception e)
                            {
                                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                                response.Content = new StringContent("Error inesperado");
                                return response;
                            }
                        }
                        response = new HttpResponseMessage(HttpStatusCode.Conflict);
                        response.Content = new StringContent("La sucursal especificada no se encuentra registrada");
                        return response;
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El tipo de maquina especificado no se encuentra registrado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El serial de la nueva maquina ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para modificar los datos de una maquina registrada en la base de datos.
         * 
         * Entradas:Token del administrador que realiza la operacion,informacion actualizada de la maquina,serial actual de la maquina 
         * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
         */
        public HttpResponseMessage updateMachine(Maquina machine,string token,string currentSerial)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador"))
            {
                if (context.Maquinas.Any(o => o.Serial == currentSerial))
                {
                    if (machine.Serial==currentSerial||!context.Maquinas.Any(o => o.Serial == machine.Serial))
                    {
                        if (context.Tipo_Equipo.Any(o => o.Nombre == machine.Tipo_Equipo))
                        {
                            if (machine.Sucursal == null || context.Sucursals.Any(o => o.Nombre == machine.Sucursal))
                            {
                                try
                                {
                                    context.updateMachine(currentSerial,machine.Serial,machine.Tipo_Equipo,machine.Sucursal,machine.Marca,machine.Costo);
                                    context.SaveChanges();
                                    response = new HttpResponseMessage(HttpStatusCode.OK);
                                    response.Content = new StringContent("Maquina modificada correctamente!");
                                    return response;
                                }
                                catch (Exception e)
                                {
                                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                                    response.Content = new StringContent("Error inesperado");
                                    return response;
                                }
                            }
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("La nueva sucursal especificada no se encuentra registrada");
                            return response;
                        }
                        response = new HttpResponseMessage(HttpStatusCode.Conflict);
                        response.Content = new StringContent("El nuevo tipo de maquina especificado no se encuentra registrado");
                        return response;
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El nuevo serial de la maquina ya se encuentra registrado en otra maquina");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La maquina que se quiere modificar no se encuentra registrada");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }


        /*Metodo para añadir un nuevo tipo de equipo a la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion del tipo de equipo a agregar 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage createMachineType(Tipo_Equipo machineType,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.Tipo_Equipo.Any(o=> o.Nombre==machineType.Nombre))
                {
                    try
                    {
                        context.Tipo_Equipo.Add(machineType);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Tipo agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nombre del nuevo tipo ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para modificar un tipo de equipo en la base de datos.
       * 
       * Entradas:Token del administrador que realiza la operacion,informacion actualizada del tipo de equipo a agregar,nombre actual del tipo de equipo 
       * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
       */
        public HttpResponseMessage updateMachineType(Tipo_Equipo machineType, string token,string currentName)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.Tipo_Equipo.Any(o => o.Nombre == currentName))
                {
                    if (machineType.Nombre == currentName || !context.Tipo_Equipo.Any(o => o.Nombre == machineType.Nombre))
                    {
                        try
                        {
                            context.updateMachineType(currentName,machineType.Nombre,machineType.Descripcion);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Tipo modificado correctamente!");
                            return response;
                        }
                        catch (Exception e)
                        {
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado");
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El nuevo nombre del tipo ya se encuentra registrado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El tipo que se quiere modificar no se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para añadir un nuevo puesto de trabajo a la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion del puesto de trabajo a agregar 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage createJob(Puesto job,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {

                if(!context.Puestoes.Any(o=> o.Nombre==job.Nombre))
                {
                    try
                    {
                        context.Puestoes.Add(job);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Puesto agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nombre del puesto que se quiere agregar ya se encuentra registrado");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }

        /*Metodo para modificar un puesto de trabajo en la base de datos.
      * 
      * Entradas:Token del administrador que realiza la operacion,informacion actualizada del puesto de trabajo a moodificar,nombre actual del puesto de trabajo 
      * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
      */
        public HttpResponseMessage updateJob(Puesto job, string token, string currentName)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.Puestoes.Any(o => o.Nombre == currentName))
                {
                    if (job.Nombre == currentName || !context.Puestoes.Any(o => o.Nombre == job.Nombre))
                    {
                        try
                        {
                            context.updateJob(currentName, job.Nombre, job.Descripcion);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Puesto modificado correctamente!");
                            return response;
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException.Message.Split('\r')[0].Equals("defaultJobModification"))
                            {
                                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                                response.Content = new StringContent("No es posible modificar los puestos por default");
                                return response;
                            }
                            Console.WriteLine(e.ToString());
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado!");
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El nuevo nombre del puesto ya se encuentra registrado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El puesto que se quiere modificar no se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para añadir una nueva planilla a la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion de la planilla a agregar 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage createPayroll(Planilla payRoll, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {

                if (!context.Planillas.Any(o => o.Nombre == payRoll.Nombre))
                {
                    try
                    {
                        context.Planillas.Add(payRoll);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Planilla agregada correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nombre de la planilla que se quiere agregar ya se encuentra registrado");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }

        /*Metodo para modificar una planilla en la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion actualizada de la planilla a modificar,nombre actual de la planilla 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage updatePayroll(Planilla payRoll, string token, string currentName)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.Planillas.Any(o => o.Nombre == currentName))
                {
                    if (payRoll.Nombre == currentName || !context.Planillas.Any(o => o.Nombre == payRoll.Nombre))
                    {
                        try
                        {
                            context.updatePayRoll(currentName, payRoll.Nombre, payRoll.Descripcion);
                            context.SaveChanges();
                            response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Content = new StringContent("Planilla modificada correctamente!");
                            return response;
                        }
                        catch (Exception e)
                        {
                            response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            response.Content = new StringContent("Error inesperado");
                            return response;
                        }
                    }
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("El nuevo nombre de la planilla ya se encuentra registrado");
                    return response;
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("La planilla que se quiere modificar no se encuentra registrada");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para añadir un nuevo tratamiento a la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion del tratamiento a agregar 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage createTreatment(Tratamiento_Spa treatment, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador")||tokenVerifier(token,"Dependiente Spa"))
            {

                try
                {
                    context.Tratamiento_Spa.Add(treatment);
                    context.SaveChanges();
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Tratamiento agregado correctamente!");
                    return response;
                }
                catch (Exception e)
                {
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Error inesperado");
                    return response;
                }

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }

        /*Metodo para modificar un tratamiento en la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion actualizada del tratamiento a modificar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage updateTreatment(Tratamiento_Spa treatment, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador")||tokenVerifier(token,"Dependiente Spa"))
            {
                try
                {
                    context.updateTreatment(treatment.Id,treatment.Nombre);
                    context.SaveChanges();
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("Tratamiento modificado correctamente!");
                    return response;
                }
                catch (Exception e)
                {
                    if(e.InnerException.Message.Split('\r')[0].Equals("defaultTreatmentModification"))
                    {
                        response = new HttpResponseMessage(HttpStatusCode.Conflict);
                        response.Content = new StringContent("No es posible modificar los tratamientos por default");
                        return response;
                    }

                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("Error inesperado");
                    return response;
                }
                 
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }


        /*Metodo para asignar un tratamiento a una sede en especifico
       * 
       * Entradas:Token del administrador que realiza la operacion,identificador del tratamiento a asignar,sede a la que se le va a asignar
       * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
       */
        public HttpResponseMessage assignTreatment(int treatmentId,string gymName,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.getTreatment(treatmentId).ToList()[0].Sucursal.Any(o=>o.Nombre==gymName))
                {
                    try
                    {
                        context.assignTreatment(treatmentId, gymName);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Tratamiento asignado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Este tratamiento ya se encuentra registrado en esta sede");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para desasignar un tratamiento de una sede en especifico
        * 
        * Entradas:Token del administrador que realiza la operacion,identificador del tratamiento a desasignar,sede a la que se le va a desasignar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage unsignTreatment(int treatmentId, string gymName, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.getTreatment(treatmentId).ToList()[0].Sucursal.Any(o => o.Nombre == gymName))
                {
                    try
                    {
                        context.unsignTreatment(treatmentId, gymName);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Tratamiento desasignado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Este tratamiento no se encuentra registrado en esta sede");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para añadir un nuevo tipo de servicio a la base de datos.
       * 
       * Entradas:Token del administrador que realiza la operacion,informacion del tipo de servicio a agregar 
       * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
       */
        public HttpResponseMessage createService(Tipo_Servicio service,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.Tipo_Servicio.Any(o=>o.Nombre==service.Nombre))
                {
                    try
                    {
                        context.Tipo_Servicio.Add(service);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Tipo de servicio agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nombre del nuevo tipo de servicio ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para modificar un tipo de servicio en la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion actualizada del tipo de servicio  a modificar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage updateService(string currentName,Tipo_Servicio service,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {

                if(currentName==service.Nombre||!context.Tipo_Servicio.Any(o=>o.Nombre==service.Nombre))
                {
                    try
                    {
                        context.updateService(currentName,service.Nombre,service.Descripcion);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Servicio modificado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nuevo nombre del servicio ya se encuentra registrado por otro servicio");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }


        /*Metodo para añadir un nuevo producto a la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion del producto a agregar 
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage createProduct(Producto product,string token)
        {
            HttpResponseMessage response = null;
            if(tokenVerifier(token,"Administrador")||tokenVerifier(token,"Dependiente Tienda"))
            {
                if(!context.Productoes.Any(o=> o.Codigo_Barras==product.Codigo_Barras))
                {
                    try
                    {
                        context.Productoes.Add(product);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Producto agregado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El codigo de barras del nuevo producto ya se encuentra registrado");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }

        /*Metodo para modificar un producto en la base de datos.
        * 
        * Entradas:Token del administrador que realiza la operacion,informacion actualizada del producto a modificar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage updateProduct(string currentCode, Producto product, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador")||tokenVerifier(token,"Dependiente Tienda"))
            {

                if (currentCode == product.Codigo_Barras ||!context.Productoes.Any(o => o.Codigo_Barras ==product.Codigo_Barras))
                {
                    try
                    {
                        context.updateProduct(currentCode,product.Codigo_Barras,product.Nombre,product.Descripcion,product.Costo);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Producto modificado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El nuevo codigo de barras ya se encuentra registrado por otro producto");
                return response;

            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;

        }
        /*Metodo para asignar un producto a una sede en especifico
        * 
        * Entradas:Token del administrador que realiza la operacion,identificador del producto a asignar,sede a la que se le va a asignar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage assignProduct(string barCode,string gymName,string token)
        {
            HttpResponseMessage response = null;

            if(tokenVerifier(token,"Administrador"))
            {
                if(!context.getProduct(barCode).ToList()[0].Sucursals.Any(o=>o.Nombre==gymName))
                {
                    try
                    {
                        context.assignProduct(barCode, gymName);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Producto asociado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El producto indicado ya se encuentra registrado en esta sucursal");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
            return response;
        }


        /*Metodo para desasignar un producto de una sede en especifico
        * 
        * Entradas:Token del administrador que realiza la operacion,identificador del producto a desasignar,sede a la que se le va a desasignar
        * Salidas:Respuesta de tipo HTTP que indica si la operacion fue exitosa.
        */
        public HttpResponseMessage unsignProduct(string barCode, string gymName, string token)
        {
            HttpResponseMessage response = null;

            if (tokenVerifier(token, "Administrador"))
            {
                if (context.getProduct(barCode).ToList()[0].Sucursals.Any(o => o.Nombre == gymName))
                {
                    try
                    {
                        context.unsignProduct(barCode, gymName);
                        context.SaveChanges();
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent("Producto desasociado correctamente!");
                        return response;
                    }
                    catch (Exception e)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El producto indicado no se encuentra registrado en esta sucursal");
                return response;
            }
            response = new HttpResponseMessage(HttpStatusCode.Conflict);
            response.Content = new StringContent("Token invalido");
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
        public bool tokenVerifier(string token,string job)
        {
            return context.Empleadoes.Any(o => o.Token == token && o.Puesto == job);
        }

        public bool selfTokenVerifier(string token,string id)
        {
            return context.Empleadoes.Any(o => o.Token == token && o.Cedula == id);
        }
        /*Metodo para verificar que la fecha de una clase este dentro del horario de la sede donde se imparte.
         * 
         * Entrada:Informacion de la clase
         * Salida: Booleano que indica si la fecha es correcta
         */
        public bool classDayScheduleVerifier(Clase classInfo)
        {
            string day = new CultureInfo("Es-Es").DateTimeFormat.GetDayName(classInfo.Fecha.DayOfWeek);
            if (context.Sucursal_Horario.Any(o => o.Sucursal == classInfo.Sucursal && o.Dia == day))
            {
                Sucursal_Horario scheduleForTheDay = context.Sucursal_Horario.Where(m => m.Sucursal == classInfo.Sucursal && m.Dia == day).ToList()[0];
                return (scheduleForTheDay.Hora_Apertura <= classInfo.Hora_Inicio && classInfo.Hora_Final <= scheduleForTheDay.Hora_Cierre);
            }
            return false;
        }
        

        /*Metodo para eliminar valores de la base de datos.
        * 
        * Entrada:Token de quien realiza la solicitud,tabla en la que se va a hacer la eliminacion,identificador de la fila
        * Salida: Respuesta de tipo HTTP que indica si la operacion fue exitosa
        */
        public HttpResponseMessage deleteFromDatabase(string token,string table,string key1,string key2)
        {
            HttpResponseMessage response = null;
            string[] instructorTables = new string[] {"Clase"};

            List<Empleado> employees = context.Empleadoes.Where(m => m.Token == token).ToList();
            if (employees.Count()==0)
            {
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Token invalido");
                return response;
            }
            else if(employees[0].Puesto.Equals("Sin Asignar"))
            {
                response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("El usuario no tiene un puesto asignado dentro del sistema");
                return response;
            }
            Empleado employee = employees[0];

            if (!employee.Puesto.Equals("Administrador"))
            {
                if (employee.Puesto.Equals("Instructor") && !table.Equals("Clase")|| 
                    employee.Puesto.Equals("Dependiente Tienda") && !table.Equals("Producto")||
                    employee.Puesto.Equals("Dependiente Spa") && !table.Equals("Tratamiento_Spa")||
                    Array.IndexOf(new string[] { "Instructor","Dependiente Tienda","Dependiente Spa"},employee.Puesto)==-1)

                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                    response.Content = new StringContent("Este usuario no tiene acceso a esta funcion");
                    return response;
                }
                
            }

            try
            {
                if (table.Equals("Sucursal"))
                {
                    context.deleteGym(key1);
                }
                else if(table.Equals("Empleado"))
                {
                    context.deleteEmployee(key1);
                }
                else if(table.Equals("SucursalTelefono"))
                {
                    context.deletePhoneNumb(key1);
                }
                else if(table.Equals("Maquina"))
                {
                    context.deleteMachine(key1);
                }
                else if(table.Equals("Tipo_Equipo"))
                {
                    context.deleteMachineType(key1);
                }
                else if(table.Equals("Planilla"))
                {
                    context.deletePayroll(key1);
                }
                else if(table.Equals("Producto"))
                {
                    context.deleteProduct(key1);
                }
                else if(table.Equals("Clase"))
                {
                    context.deleteClass(Convert.ToInt32(key1));
                }
                else if(table.Equals("SucursalHorario"))
                {
                    context.deleteSchedule(key1, key2);
                }
                else if (table.Equals("Tipo_Servicio"))
                {
                    context.deleteService(key1);
                }
                else if(table.Equals("Tratamiento_Spa"))
                {
                    try
                    {
                        context.deleteTreatment(Convert.ToInt32(key1));
                    }
                    catch (Exception e)
                    {

                        if (e.InnerException.Message.Split('\r')[0].Equals("defaultTreatmentModification"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("No es posible eliminar los tratamientos por default");
                            return response;
                        }
                        else if (e.InnerException.Message.Split('\r')[0].Equals("assignedTreatment"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("No es posible eliminar un tratamiento que esta disponible en una sucursal");
                            return response;
                        }
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                else if (table.Equals("Puesto"))
                {
                    try
                    {
                        context.deleteJob(key1);
                    }
                    catch (Exception e)
                    {

                        if (e.InnerException.Message.Split('\r')[0].Equals("defaultJobModification"))
                        {
                            response = new HttpResponseMessage(HttpStatusCode.Conflict);
                            response.Content = new StringContent("No es posible eliminar los puestos por default");
                            return response;
                        }
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Error inesperado");
                        return response;
                    }
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    response.Content = new StringContent("La tabla indicada no existe");
                    return response;
                }
                context.SaveChanges();
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("Valor eliminado correctamente");
                return response;
            }
            catch (Exception e)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Error inesperado");
                return response;
            }

        }
    }
}