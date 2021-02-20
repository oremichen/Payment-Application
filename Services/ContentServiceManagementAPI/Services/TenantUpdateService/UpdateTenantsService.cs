using ContentServiceManagementAPI.Models.DTO.Client;
using ContentServiceManagementAPI.Models.DTO.ContentProvider;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.TenantUpdateService
{
    public class UpdateTenantsService : IUpdateTenantsService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UpdateTenantsService> logger;

        public UpdateTenantsService(
            IConfiguration config, 
            ILogger<UpdateTenantsService> logger)
        {
            _config = config;
            this.logger = logger;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("ANQContentServiceManageDbConnection"));
            }
        }

        public Task UpdateClient(UserProfileClientDto model)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sql = $"dbo.spUpdateTenantClient @AuthenticationId, @ClientName, @ContactPersonFirstName,@ContactPersonLastName,@ContactPersonPhoneNumber,@AccountEmail";
                    conn.Open();
                    var result = conn.Execute(sql, new
                    {
                        model.AuthenticationId,
                        model.ClientName,
                        model.ContactPersonFirstName,
                        model.ContactPersonLastName,
                        model.ContactPersonPhoneNumber,
                        model.AccountEmail
                    });
                    return Task.FromResult(model);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
                           
        }

        public Task UpdateServiceProvider(UserProfileServiceProviderDto model)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sql = $"dbo.spUpdateTenantServiceProvider @ServiceProviderId, @ServiceProviderName, @AccountEmail, @ContactPersonFirstName, @ContactPersonLastName, @ContactPersonPhoneNumber";
                    conn.Open();
                    var result = conn.Execute(sql, new
                    {
                        model.ServiceProviderId,
                        model.ServiceProviderName,
                        model.AccountEmail,
                        model.ContactPersonFirstName,
                        model.ContactPersonLastName,
                        model.ContactPersonPhoneNumber
                    });
                    return Task.FromResult(model);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public Task UpdateContentProvider(UserProfileContentProviderDto model)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sql = $"dbo.spUpdateTenantContentProvider @ContentProviderId, @ContentProviderName, @AccountEmail, @ContactPersonFirstName, @ContactPersonLastName, @ContactPersonPhoneNumber";
                    conn.Open();
                    var result = conn.Execute(sql, new
                    {
                        model.ContentProviderId,
                        model.ContentProviderName,
                        model.AccountEmail,
                        model.ContactPersonFirstName,
                        model.ContactPersonLastName,
                        model.ContactPersonPhoneNumber
                    });
                    return Task.FromResult(model);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
           
        }
    }
}
