﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SimaSzamlaAdatbazissal
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SzamlaEntities : DbContext
    {
        public SzamlaEntities()
            : base("name=SzamlaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Szamlak> Szamlak { get; set; }
        public virtual DbSet<CommercialPapers> CommercialPapers { get; set; }
        public virtual DbSet<RateTable> RateTable { get; set; }
    }
}
