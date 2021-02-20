using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Data.Repository
{
    public abstract class Repository<T>  where T : class 
    {
        public ANQContentServiceManageDb _db { get; set; }

        protected DbSet <T> DbSet { get; set; }
        

        public Repository()
        {
            _db = new ANQContentServiceManageDb();
            DbSet = _db.Set<T>();       
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            var all = DbSet.ToListAsync();
            return await all;
        }

        public virtual async Task<T> GetById(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<T>GetByStringColumn(string column)
        {
            return await DbSet.FindAsync(column);
        }


        public virtual async void Add (T entity)
        {   
           await  DbSet.AddAsync(entity);
        }

        public virtual async void SaveChanges()
        {
           await _db.SaveChangesAsync();
        }


        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {  
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        void DisposeNow()
        {
              this.Dispose(true);     
        }
        #endregion


    }
}