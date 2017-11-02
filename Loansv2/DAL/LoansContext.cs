using Loansv2.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Loansv2.DAL
{
    public class LoansContext : DbContext
    {
        public LoansContext() : base("LoansContext")
        {
        }

        public DbSet<Party> Parties { get; set; }
        public DbSet<PhysicalParty> PhysicalParties { get; set; }
        public DbSet<IndividualParty> IndividualParties { get; set; }
        public DbSet<JuristicParty> JuristicParties { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LoanAgreement> LoanAgreements { get; set; }
        public DbSet<CreditPlan> CreditPlans { get; set; }
        public DbSet<DebtPlan> DebtPlans { get; set; }
        public DbSet<AnnumRate> AnnumRates { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<File> Files { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}