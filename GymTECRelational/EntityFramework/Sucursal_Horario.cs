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
    
    public partial class Sucursal_Horario
    {
        public string Dia { get; set; }
        public string Sucursal { get; set; }
        public Nullable<System.TimeSpan> Hora_Apertura { get; set; }
        public Nullable<System.TimeSpan> Hora_Cierre { get; set; }
    }
}
