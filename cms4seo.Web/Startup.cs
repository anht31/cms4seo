using cms4seo.Data.ConnectionString;
using Owin;

namespace IdentitySample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // check connectionString
            if(ConnectionStringProvider.Get() != null)
                ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}
