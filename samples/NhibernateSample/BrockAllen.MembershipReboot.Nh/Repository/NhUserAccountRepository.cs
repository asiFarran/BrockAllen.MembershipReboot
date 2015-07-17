namespace BrockAllen.MembershipReboot.Nh.Repository
{
    using System;
    using System.Linq;

    using BrockAllen.MembershipReboot;
    using BrockAllen.MembershipReboot.Nh;

    public class NhUserAccountRepository<TAccount> :
        IUserAccountRepository<TAccount>,
        IUserAccountQuery,
        IUserAccountQuery<TAccount>
        where TAccount : NhUserAccount
    {
        private readonly IRepository<TAccount> accountRepository;

        public bool UseEqualsOrdinalIgnoreCaseForQueries { get; set; }

        public Func<IQueryable<TAccount>, string, IQueryable<TAccount>> QueryFilter { get; set; }
        public Func<IQueryable<TAccount>, IQueryable<TAccount>> QuerySort { get; set; }

        public NhUserAccountRepository(IRepository<TAccount> accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        protected IQueryable<TAccount> Queryable
        {
            get
            {
                return this.accountRepository.FindAll();
            }
        }

        public TAccount Create()
        {
            var account = Activator.CreateInstance<TAccount>();
            return account;
        }

        public void Add(TAccount item)
        {
            this.accountRepository.Save(item);
        }

        public void Remove(TAccount item)
        {
            this.accountRepository.Delete(item);
        }

        public void Update(TAccount item)
        {
            this.accountRepository.Update(item);
        }

        public TAccount GetByLinkedAccount(string tenant, string provider, string id)
        {
            var accounts = from a in this.accountRepository.FindAll()
                          where a.Tenant == tenant
                          from la in a.LinkedAccountsCollection
                          where la.ProviderName == provider && la.ProviderAccountID == id
                          select a;

            return accounts.SingleOrDefault();
        }

        public TAccount GetByCertificate(string tenant, string thumbprint)
        {
            var accounts =
                from a in this.accountRepository.FindAll()
                where a.Tenant == tenant
                from c in a.CertificatesCollection
                where c.Thumbprint == thumbprint
                select a;
            return accounts.SingleOrDefault();
        }

        public TAccount GetByID(Guid id)
        {
            return Queryable.SingleOrDefault(x => x.ID == id);
        }

        public TAccount GetByUsername(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            if (UseEqualsOrdinalIgnoreCaseForQueries)
            {
                var NAME = username.ToUpper();

                return Queryable.SingleOrDefault(x => NAME == x.Username.ToUpper());             
            }
            else
            {
                return Queryable.SingleOrDefault(x => username == x.Username);
            }
        }

        public TAccount GetByUsername(string tenant, string username)
        {
            if (String.IsNullOrWhiteSpace(tenant) ||
                String.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            if (UseEqualsOrdinalIgnoreCaseForQueries)
            {
                var TENANT = tenant.ToUpper();
                var NAME = username.ToUpper();

                return Queryable.SingleOrDefault(x =>
                    TENANT == x.Tenant.ToUpper() &&
                    NAME == x.Username.ToUpper());
            }
            else
            {
                return Queryable.SingleOrDefault(x =>
                    tenant == x.Tenant &&
                    username == x.Username);
            }
        }

        public TAccount GetByEmail(string tenant, string email)
        {
            if (String.IsNullOrWhiteSpace(tenant) ||
                String.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            if (UseEqualsOrdinalIgnoreCaseForQueries)
            {
                var TENANT = tenant.ToUpper();

                return Queryable.SingleOrDefault(x =>
                    TENANT == x.Tenant.ToUpper() &&
                    email == x.Email);
            }
            else
            {
                return Queryable.SingleOrDefault(x =>
                    tenant == x.Tenant &&
                    email == x.Email);
            }
        }

        public TAccount GetByMobilePhone(string tenant, string phone)
        {
            if (String.IsNullOrWhiteSpace(tenant) ||
                String.IsNullOrWhiteSpace(phone))
            {
                return null;
            }

            return Queryable.SingleOrDefault(x =>
                    tenant == x.Tenant &&
                    phone == x.MobilePhoneNumber);
            
        }

        public TAccount GetByVerificationKey(string key)
        {
            return Queryable.SingleOrDefault(x => x.VerificationKey == key);
        }


        protected IQueryable<TAccount> DefaultQuerySort(IQueryable<TAccount> query)
        {
            return query.OrderBy(x => x.Tenant).ThenBy(x => x.Username);
        }

        protected IQueryable<TAccount> DefaultQueryFilter(IQueryable<TAccount> query, string filter)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (filter == null) throw new ArgumentNullException("filter");

            return
                from a in query
                where
                    a.Username.Contains(filter) ||
                    a.Email.Contains(filter)
                select a;
        }


        // IUserAccountQuery
        public System.Collections.Generic.IEnumerable<string> GetAllTenants()
        {
            return Queryable.Select(x => x.Tenant).Distinct().ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(string filter)
        {
            var query =
                from a in Queryable
                select a;

            if (!String.IsNullOrWhiteSpace(filter) && QueryFilter != null)
            {
                query = QueryFilter(query, filter);
            }

            if (QuerySort != null)
            {
                query = QuerySort(query);
            }

            var result =
                from a in query
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            return result.ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(string tenant, string filter)
        {
            var query =
                from a in Queryable
                where a.Tenant == tenant
                select a;

            if (!String.IsNullOrWhiteSpace(filter) && QueryFilter != null)
            {
                query = QueryFilter(query, filter);
            }

            if (QuerySort != null)
            {
                query = QuerySort(query);
            }

            var result =
                from a in query
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            return result.ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(string filter, int skip, int count, out int totalCount)
        {
            var query =
                from a in Queryable
                select a;

            if (!String.IsNullOrWhiteSpace(filter) && QueryFilter != null)
            {
                query = QueryFilter(query, filter);
            }

            if (QuerySort != null)
            {
                query = QuerySort(query);
            }

            var result =
                from a in query
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            totalCount = result.Count();
            return result.Skip(skip).Take(count).ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(string tenant, string filter, int skip, int count, out int totalCount)
        {
            var query =
                from a in Queryable
                where a.Tenant == tenant
                select a;

            if (!String.IsNullOrWhiteSpace(filter) && QueryFilter != null)
            {
                query = QueryFilter(query, filter);
            }

            if (QuerySort != null)
            {
                query = QuerySort(query);
            }

            var result =
                from a in query
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            totalCount = result.Count();
            return result.Skip(skip).Take(count).ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(Func<IQueryable<TAccount>, IQueryable<TAccount>> filter)
        {
            var query =
                from a in Queryable
                select a;

            if (filter != null) query = filter(query);

            var result =
                from a in query
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            return result.ToArray();
        }

        public System.Collections.Generic.IEnumerable<UserAccountQueryResult> Query(
           Func<IQueryable<TAccount>, IQueryable<TAccount>> filter,
           Func<IQueryable<TAccount>, IQueryable<TAccount>> sort,
           int skip, int count, out int totalCount)
        {
            var query =
                from a in Queryable
                select a;

            if (filter != null) query = filter(query);
            var sorted = (sort ?? QuerySort)(query);

            var result =
                from a in sorted
                select new UserAccountQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Username = a.Username,
                    Email = a.Email
                };

            totalCount = result.Count();
            return result.Skip(skip).Take(count).ToArray();
        }
    }

    public class NhUserAccountRepository : NhUserAccountRepository<NhUserAccount>
    {
        public NhUserAccountRepository(IRepository<NhUserAccount> accountRepository)
            : base(accountRepository)
        {
        }
    }
}