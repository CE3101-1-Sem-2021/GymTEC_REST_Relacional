//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GymTECRelational.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Administrador
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Contraseña { get; set; }
        public string Salt { get; set; }
        public string Token { get; set; }
    }
}
