﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Automator
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_automator_2Entities : DbContext
    {
        public db_automator_2Entities()
            : base("name=db_automator_2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Authority> Authorities { get; set; }
        public virtual DbSet<Code> Codes { get; set; }
        public virtual DbSet<history> histories { get; set; }
        public virtual DbSet<history1> histories1 { get; set; }
        public virtual DbSet<person> persons { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}