namespace BrockAllen.MembershipReboot.Nh.Extensions
{
    using System;
    using System.Reflection;

    using BrockAllen.MembershipReboot.Nh.Mappings;

    using NHibernate.Cfg;
    using NHibernate.Dialect;
    using NHibernate.Mapping.ByCode;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    public static class ConfigurationExtensions
    {        
        public static FluentConfiguration FluentlyConfigureMembershipReboot(this Configuration configuration)
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration.MsSql2012
               .ConnectionString(x => x.FromConnectionStringWithKey("ApplicationDatabase"))
              .ShowSql())
              .Mappings(m =>m.FluentMappings.AddFromAssemblyOf<NhGroup>())               
               .ExposeConfiguration(cfg =>
               {
                   cfg.SetProperty("hbm2ddl.keywords", "auto-quote");
                   cfg.SetProperty("hbm2ddl.auto", "update");
               });
        }
    }
}