﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication9.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Requisition1Entities : DbContext
    {
        public Requisition1Entities()
            : base("name=Requisition1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Fund> Funds { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<ReqAdd> ReqAdds { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserRegistrationToken> UserRegistrationTokens { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Requisition> Requisitions { get; set; }
    }
}
