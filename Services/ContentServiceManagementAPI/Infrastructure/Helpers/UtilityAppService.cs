using ContentServiceManagementAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.Utitlities
{
    public class UtilityAppService<T> : IUtilityAppService where T: class 
    {
        public ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;


        protected DbSet<T> DbSet { get; set; }

        public UtilityAppService(ANQContentServiceManageDb aNQContentServiceManageDb, IHttpContextAccessor httpContextAccessor)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;

            DbSet = _aNQContentServiceManageDb.Set<T>();
        }

        public async Task<bool> CheckIfIdExist (long id)
        {
           var IdHasContent = await  DbSet.FindAsync(id);
           if (IdHasContent != null)
            {
                return true;
            }
            return false;
        }

        public static string PhoneNumber(string value)
        {
            value = new System.Text.RegularExpressions.Regex(@"\D")
                .Replace(value, string.Empty);
            value = value.TrimStart('1');
            if (value.Length == 7)
                return Convert.ToInt64(value).ToString("###-####");
            if (value.Length == 10)
                return Convert.ToInt64(value).ToString("###-###-####");
            if (value.Length > 10)
                return Convert.ToInt64(value)
                    .ToString("###-###-#### " + new String('#', (value.Length - 10)));

            return value;
        }
    }
}
