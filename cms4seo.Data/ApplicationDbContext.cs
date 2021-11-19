using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using cms4seo.Data.ConnectionString;
using cms4seo.Data.IdentityModels;
using cms4seo.Model.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace cms4seo.Data
{

    // ApplicationDbContext =======================================================
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole,
            string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {

        private static string connectionString = ConnectionStringProvider.Get();
        
        //ConnectionStringProvider.Build(@".\sqlexpress", "lekimax_net_v3_db",
        //    "anht31", "234", false);
        //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private static string ConnectionString
        {
            get
            {
                if (connectionString != null)
                    return connectionString;

                connectionString = ConnectionStringProvider.Get();
                return connectionString;

            }
        }

        public ApplicationDbContext()
            : base(ConnectionString)
        {
        }

        /// <summary>
        /// For test TestConnectionEntityFramework
        /// </summary>
        public ApplicationDbContext(string connectionStringTest)
            : base(connectionStringTest)
        {
        }

        //public ApplicationDbContext()
        //    : base("DefaultConnection")
        //{
        //}


        static ApplicationDbContext()
        {
            // Set the database initializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ConfigurationMigration>());
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Add the ApplicationGroups property:
        public virtual IDbSet<ApplicationGroup> ApplicationGroups { get; set; }


        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Make sure to call the base method first:
            base.OnModelCreating(modelBuilder);



            // Map Users to Groups:
            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>((ApplicationGroup g) => g.ApplicationUsers)
                .WithRequired()
                .HasForeignKey<string>((ApplicationUserGroup ag) => ag.ApplicationGroupId);
            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey((ApplicationUserGroup r) =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            // Map Roles to Groups:
            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>((ApplicationGroup g) => g.ApplicationRoles)
                .WithRequired()
                .HasForeignKey<string>((ApplicationGroupRole ap) => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey((ApplicationGroupRole gr) =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");
        }


        // product ------------------------------------
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        // article ------------------------------------
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }


        // share ---------------------------------------
        public DbSet<Photo> Photos { get; set; }

        // cms ---------------------------------------
        public DbSet<HitCounter> HitCounters { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Supporter> Supporters { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Info> Infos { get; set; }

        public DbSet<UserCounter> UserCounters { get; set; }

        public DbSet<Shop> Shops { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<ExtraSiteMap> ExtraSiteMaps { get; set; }

        public DbSet<Permalink> Permalinks { get; set; }

        public DbSet<SettingModel> Settings { get; set; }
        public DbSet<Content> Contents { get; set; }

        public DbSet<Property> Properties { get; set; }
        public DbSet<ProductProperties> ProductProperties { get; set; }

        //de-bug disable for PhotoService -> 
        // generic-Repository -------------------------------------------------
        //public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        //{
        //    return base.Set<TEntity>();
        //}

    }




    //internal sealed class ConfigurationMigration : DbMigrationsConfiguration<ApplicationDbContext>
    //{
    //    public ConfigurationMigration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //    }

    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        // register admin
    //        //AuthDbConfig.RegisterAdmin();
    //    }
    //}
}