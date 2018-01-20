﻿using System;
using System.Data.Entity;
using System.Linq;
using FootballTeamSystem.Data.Model;
using FootballTeamSystem.Data.Model.Contracts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballTeamSystem.Data
{
    public class MsSqlDbContext : IdentityDbContext<User>, IMsSqlDbContext
    {
        public MsSqlDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<MsSqlDbContext>());

        }

        public virtual IDbSet<Post> Posts { get; set; }
        public virtual IDbSet<Player> Players { get; set; }


        public static MsSqlDbContext Create()
        {
            return new MsSqlDbContext();
        }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges();
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        private void ApplyAuditInfoRules()
        {
            foreach (var entry in
                this.ChangeTracker.Entries()
                    .Where(
                        e =>
                            e.Entity is IAuditable && ((e.State == EntityState.Added) || (e.State == EntityState.Modified))))
            {
                var entity = (IAuditable)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default(DateTime))
                {
                    entity.CreatedOn = DateTime.Now;
                }
                else
                {
                    entity.ModifiedOn = DateTime.Now;
                }
            }
        }
    }
}
