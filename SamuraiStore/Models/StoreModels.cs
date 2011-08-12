using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SamuraiStore.Models
{
    public class Thing
    {
        public int ThingId { get; set; }

        public string Name { get; set; }
        public float Price { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int ThingId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string TransactionRef { get; set; }
        public string CreditRef { get; set; }

        public bool IsCredited { get; set; }

        public virtual Thing Thing { get; set; }
    }

    public class Reserve
    {
        public int ReserveId { get; set; }
        public int ThingId { get; set; }
        //public int OrderId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CapturedAt { get; set; }

        public string TransactionRef { get; set; }
        public string CapturingRef { get; set; }
        public string VoidingRef { get; set; }

        public bool IsCaptured { get; set; }
        public bool IsVoided { get; set; }

        public virtual Thing Thing { get; set; }
        //public virtual Order Order { get; set; }
    }

    public class Method
    {
        public int MethodId { get; set; }
        public string Token { get; set; }
        public string HolderName { get; set; }
        public string MethodName { get; set; }
    }

    public class StoreContext : DbContext
    {
        public DbSet<Thing> Things { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<Method> Methods { get; set; }
    }
}