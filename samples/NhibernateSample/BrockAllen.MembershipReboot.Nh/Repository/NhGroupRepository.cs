namespace BrockAllen.MembershipReboot.Nh.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NhGroupRepository<TGroup> :
        IGroupRepository<TGroup>,
        IGroupQuery
        where TGroup : NhGroup
    {
        private readonly IRepository<TGroup> groupRepository;

        public NhGroupRepository(IRepository<TGroup> groupRepository)
        {
            this.groupRepository = groupRepository;
        }

        protected IQueryable<TGroup> Queryable
        {
            get
            {
                return this.groupRepository.FindAll();
            }
        }

        protected IQueryable<TGroup> SortedQueryable
        {
            get
            {
                return Queryable.OrderBy(x => x.Tenant).ThenBy(x => x.Name);
            }
        }

        public TGroup Create()
        {
            var group = Activator.CreateInstance<TGroup>();
            return group;
        }

        public void Add(TGroup item)
        {
            this.groupRepository.Save(item);
        }

        public void Remove(TGroup item)
        {
            this.groupRepository.Delete(item);
        }

        public void Update(TGroup item)
        {
            this.groupRepository.Update(item);
        }

        public TGroup GetByID(Guid id)
        {
            return Queryable.SingleOrDefault(x => x.ID == id);
        }

        public TGroup GetByName(string tenant, string name)
        {
            
            if (String.IsNullOrWhiteSpace(tenant) ||
                String.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var TENANT = tenant.ToUpper();
            var NAME = name.ToUpper();


            return Queryable.SingleOrDefault(x =>
                TENANT == x.Tenant.ToUpper() &&
                NAME == x.Name.ToUpper());
        }

        public System.Collections.Generic.IEnumerable<TGroup> GetByIDs(Guid[] ids)
        {
            return Queryable.Where(x => ids.Contains(x.ID)).ToArray();
        }

        // IGroupQuery
        public System.Collections.Generic.IEnumerable<string> GetAllTenants()
        {
            return Queryable.Select(x => x.Tenant).Distinct().ToArray();
        }

        public System.Collections.Generic.IEnumerable<GroupQueryResult> Query(string filter)
        {
            var query =
                from a in SortedQueryable
                select a;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query =
                    from a in query
                    where
                        a.Tenant.Contains(filter) ||
                        a.Name.Contains(filter)
                    select a;
            }

            var result =
                from a in query
                select new GroupQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Name = a.Name
                };

            return result.ToArray();
        }

        public System.Collections.Generic.IEnumerable<GroupQueryResult> Query(string tenant, string filter)
        {
            var query =
                from a in SortedQueryable
                where a.Tenant == tenant
                select a;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query =
                    from a in query
                    where
                        a.Name.Contains(filter)
                    select a;
            }

            var result =
                from a in query
                select new GroupQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Name = a.Name
                };

            return result.ToArray();
        }

        public System.Collections.Generic.IEnumerable<GroupQueryResult> Query(string filter, int skip, int count, out int totalCount)
        {
            var query =
                from a in SortedQueryable
                select a;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query =
                    from a in query
                    where
                        a.Tenant.Contains(filter) ||
                        a.Name.Contains(filter)
                    select a;
            }

            var result =
                from a in query
                select new GroupQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Name = a.Name
                };

            totalCount = query.Count();
            return result.Skip(skip).Take(count).ToArray();
        }

        public System.Collections.Generic.IEnumerable<GroupQueryResult> Query(string tenant, string filter, int skip, int count, out int totalCount)
        {
            var query =
                from a in SortedQueryable
                where a.Tenant == tenant
                select a;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                query =
                    from a in query
                    where
                        a.Name.Contains(filter)
                    select a;
            }

            var result =
                from a in query
                select new GroupQueryResult
                {
                    ID = a.ID,
                    Tenant = a.Tenant,
                    Name = a.Name
                };

            totalCount = query.Count();
            return result.Skip(skip).Take(count).ToArray();
        }

        public System.Collections.Generic.IEnumerable<string> GetRoleNames(string tenant)
        {
            return Queryable.Where(x => x.Tenant == tenant).Select(x => x.Name).ToArray();
        }


        // IGroupQuery
        public IEnumerable<TGroup> GetByChildID(Guid childGroupID)
        {
            var query =
                from g in this.groupRepository.FindAll()
                from c in g.ChildrenCollection
                where c.ChildGroupID == childGroupID
                select g;
            return query;
        }
    }
}