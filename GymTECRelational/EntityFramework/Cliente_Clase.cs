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
    
    public partial class Cliente_Clase
    {
        public int Id { get; set; }
        public string Cliente { get; set; }
    
        public virtual Clase Clase { get; set; }
    }
}
