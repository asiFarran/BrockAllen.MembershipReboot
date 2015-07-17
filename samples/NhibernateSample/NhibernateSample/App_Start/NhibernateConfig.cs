namespace NhibernateSample
{
    using BrockAllen.MembershipReboot.Nh;
    using BrockAllen.MembershipReboot.Nh.Extensions;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.Cfg;

    public class NhibernateConfig
    {
        public static ISessionFactory GetSessionFactory()
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration.MsSql2012
               .ConnectionString(x => x.FromConnectionStringWithKey("ApplicationDatabase"))
              .ShowSql())
              .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NhGroup>())
               .ExposeConfiguration(cfg =>
               {
                   cfg.SetProperty("hbm2ddl.keywords", "auto-quote");
                   cfg.SetProperty("hbm2ddl.auto", "update");
               }).BuildSessionFactory();

           /* var config = GetConfiguration();
            config.BuildMappings();
            return config.BuildSessionFactory();*/
        }

        private static Configuration GetConfiguration()
        {
            var config = new Configuration();
            config.Configure();
            config.ConfigureMembershipReboot();
            return config;
        }
    }
}