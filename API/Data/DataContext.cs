using API.Entities;
using API.Entities.Applications;
using API.Entities.Chat;
using API.Entities.CompanyJobPost;
using API.Entities.JobPost;
using API.Entities.Notification;
using API.Entities.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Transactions;

namespace API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserJobPost> UserJobPosts { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobPostStatus> JobPostStatuses { get; set; }
        public DbSet<ApplicantEducation> ApplicantEducations { get; set; }
        public DbSet<AdvertisementType> AdvertisementTypes { get; set; }
        public DbSet<UserEducation> UserEducations { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyJobPost> CompanyJobPosts { get; set; }
        public DbSet<PricingPlan> PricingPlans { get; set; }
        public DbSet<PricingPlanCompanies> PricingPlanCompanies { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<UserPreviousCompanies> UserPreviousCompanies { get; set; }
        public DbSet<ApplicantPreviousCompanies> ApplicantPreviousCompanies { get; set; }
        public DbSet<EmploymentStatus> EmploymentStatuses { get; set; }
        public DbSet<UserApplication> UserApplications { get; set; }
        public DbSet<UserApplicationEducation> UserApplicationEducations { get; set; }
        public DbSet<UserApplicationPreviousCompanies> UserApplicationPreviousCompanies { get; set; }
        public DbSet<UserNotificationSettings> UserNotificationSettings { get; set; }
        public DbSet<CompanyJobCategoryInterests> CompanyJobCategoryInterests { get; set; }
        public DbSet<CompanyNotificationPreferences> CompanyNotificationPreferences { get; set; }
        public DbSet<DemoMeetingRequest> DemoMeetingRequests { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<CandidateComment> CandidateComments { get; set; }
        public DbSet<UserTransaction> UserTransactions { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<EarlyAccess> EarlyAccesses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            RegisterUserRolesTables(builder);
            RegisterAddressTables(builder);
            RegisterJobPostTables(builder);
            RegisterCompanyJobPostTables(builder);
            RegisterUserApplicationTables(builder);
            RegisterChatTables(builder);
        }

        private void RegisterChatTables(ModelBuilder builder)
        {
            builder.Entity<Conversation>()
            .HasOne(c => c.FromUser)
            .WithMany()
            .HasForeignKey(c => c.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);  // ✅ Prevents cascade delete

            builder.Entity<Conversation>()
                .HasOne(c => c.ToUser)
                .WithMany()
                .HasForeignKey(c => c.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void RegisterUserApplicationTables(ModelBuilder builder)
        {
            builder.Entity<UserApplication>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.User)
                      .WithMany(u => u.UserApplications)
                      .HasForeignKey(e => e.SubmittingUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Educations)
                      .WithOne(c => c.UserApplication)
                      .HasForeignKey(e => e.UserApplicationId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);


                entity.HasMany(e => e.PreviousCompanies)
                      .WithOne(c => c.UserApplication)
                      .HasForeignKey(e => e.UserApplicationId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.City)
                      .WithMany()
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.Property(c => c.CityId).HasDefaultValue(1);
            });

            builder.Entity<JobType>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            //builder.Entity<JobCategory>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.HasOne(c => c.ParentCategory) // Each category has one parent category
            //        .WithMany(c => c.Subcategories) // Each category can have multiple subcategories
            //        .HasForeignKey(c => c.ParentId) // Foreign key property
            //        .IsRequired(false); // ParentId is nullable to allow top-level categories
            //});

            builder.Entity<JobCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            builder.Entity<JobPostStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

        private void RegisterCompanyJobPostTables(ModelBuilder builder)
        {
            builder.Entity<CompanyJobPost>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.CompanyJobPosts)
                      .HasForeignKey(e => e.SubmittingUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.JobType)
                      .WithMany()
                      .HasForeignKey(e => e.JobTypeId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.JobCategory)
                      .WithMany(c => c.CompanyJobPosts)
                      .HasForeignKey(e => e.JobCategoryId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.City)
                      .WithMany()
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.Property(c => c.CityId).HasDefaultValue(1);

                entity.HasOne(e => e.PricingPlan)
                  .WithMany()
                  .HasForeignKey(e => e.PricingPlanId)
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.NoAction);
            });
        }

        private void RegisterJobPostTables(ModelBuilder builder)
        {
            builder.Entity<UserJobPost>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.UserJobPosts)
                      .HasForeignKey(e => e.SubmittingUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.JobType)
                      .WithMany()
                      .HasForeignKey(e => e.JobTypeId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.JobCategory)
                      .WithMany(c => c.UserJobPosts)
                      .HasForeignKey(e => e.JobCategoryId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.ApplicantEducations)
                      .WithOne(c => c.UserJobPost)
                      .HasForeignKey(e => e.UserJobPostId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.City)
                      .WithMany()
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.Property(c => c.CityId).HasDefaultValue(1);

                entity.HasOne(e => e.AdvertisementType)
                      .WithMany(e => e.UserJobPosts)
                      .HasForeignKey(e => e.AdvertisementTypeId)
                      .IsRequired(true)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.PricingPlan)
                      .WithMany()
                      .HasForeignKey(e => e.PricingPlanId)
                      .IsRequired(true)
                      .OnDelete(DeleteBehavior.NoAction);


                //entity.Property(c => c.AdvertisementType).HasDefaultValue(1);
            });

            builder.Entity<JobType>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            //builder.Entity<JobCategory>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.HasOne(c => c.ParentCategory) // Each category has one parent category
            //        .WithMany(c => c.Subcategories) // Each category can have multiple subcategories
            //        .HasForeignKey(c => c.ParentId) // Foreign key property
            //        .IsRequired(false); // ParentId is nullable to allow top-level categories
            //});

            builder.Entity<JobCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            builder.Entity<JobPostStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

        private void RegisterUserRolesTables(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasOne(u => u.JobCategory)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                .HasOne(u => u.JobType)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany()
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>()
                .HasMany(ar => ar.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserEducation>()
                .HasOne(u => u.User)
                .WithMany(u => u.UserEducations)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserTransaction>()
                .HasOne(t => t.User) // A transaction has one user
                .WithMany() // User can have many transactions
                .HasForeignKey(t => t.UserId) // Foreign key property
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .HasMany(u => u.UserTransactions)  // User has many transactions
                .WithOne(t => t.User)  // A transaction has one user
                .HasForeignKey(t => t.UserId);
        }


        private void RegisterAddressTables(ModelBuilder builder)
        {

            builder.Entity<Country>()
                .HasKey(c => c.Id);

            builder.Entity<City>()
                .HasKey(c => c.Id);

            builder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<City>()
            //   .HasMany(c => c.Municipalities)
            //   .WithOne(m => m.City)
            //   .HasForeignKey(m => m.CityId)
            //   .IsRequired(false)
            //   .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<City>()
            //   .HasOne(c => c.Canton)
            //   .WithMany(m => m.Cities)
            //   .HasForeignKey(m => m.CantonId)
            //   .IsRequired(false)
            //   .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Canton>()
            //    .HasKey(c => c.Id);

            //builder.Entity<Canton>()
            //    .HasOne(c => c.Country)
            //    .WithMany(c => c.Cantons)
            //    .HasForeignKey(c => c.CountryId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Municipality>()
            //    .HasKey(m => m.Id);
        }
    }
}
