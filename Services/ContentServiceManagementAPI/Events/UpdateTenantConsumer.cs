using ANQ.Messages.NonScheduledTasks.UserProfile;
using ContentServiceManagementAPI.Models.DTO.Client;
using ContentServiceManagementAPI.Models.DTO.ContentProvider;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using ContentServiceManagementAPI.Services.ClientAppService;
using ContentServiceManagementAPI.Services.ContentProviderService;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using ContentServiceManagementAPI.Services.TenantUpdateService;
using MassTransit;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Events
{
    public class UpdateTenantConsumer : IConsumer<IUpdateTenants>
    {
        private readonly IUpdateTenantsService _updateTenantsService;
        private readonly IClientAppService _clientAppService;
        private readonly IServiceProviderAppService _serviceProviderAppService;
        private readonly IContentProviderAppService _contentProviderAppService;

        public UpdateTenantConsumer(IUpdateTenantsService updateTenantsService, IClientAppService clientAppService, 
            IServiceProviderAppService serviceProviderAppService, IContentProviderAppService contentProviderAppService)
        {
            _updateTenantsService = updateTenantsService;
            _clientAppService = clientAppService;
            _serviceProviderAppService = serviceProviderAppService;
            _contentProviderAppService = contentProviderAppService;            
        }

        public async Task Consume(ConsumeContext<IUpdateTenants> context)
        {
           
           var updateClient = await _clientAppService.GetAllClients();

            foreach(var client in updateClient)
            {
                if(context.Message.AuthenticationId== client.AuthenticationId)
                {
                    await _updateTenantsService.UpdateClient(new UserProfileClientDto
                    {
                        AuthenticationId= context.Message.AuthenticationId,
                        ClientName= context.Message.CompanyName,
                        ContactPersonFirstName= context.Message.FirstName,
                        ContactPersonLastName= context.Message.LastName,
                        ContactPersonPhoneNumber= context.Message.PhoneNumber,
                        AccountEmail= context.Message.Email
                        
                    });
                }
            }

            var updateServiceProvider = await _serviceProviderAppService.GetServiceProviders();

            foreach(var serviceProvider in updateServiceProvider)
            {
                if(context.Message.ServiceProviderId== serviceProvider.ServiceProviderId)
                {
                    await _updateTenantsService.UpdateServiceProvider(new UserProfileServiceProviderDto
                    {
                        ServiceProviderId= context.Message.ServiceProviderId,
                        ServiceProviderName= context.Message.CompanyName,
                        ContactPersonFirstName= context.Message.FirstName,
                        ContactPersonLastName= context.Message.LastName,
                        ContactPersonPhoneNumber= context.Message.PhoneNumber,
                        AccountEmail= context.Message.Email
                    });
                }
            }

            var updateContentProvider = await _contentProviderAppService.GetContentProviders();

            foreach(var contentProvider in updateContentProvider)
            {
                if (context.Message.ContentProviderId == contentProvider.ContentProviderId)
                {
                    await _updateTenantsService.UpdateContentProvider(new UserProfileContentProviderDto
                    {
                        ContentProviderId= context.Message.ContentProviderId,
                        ContentProviderName = context.Message.CompanyName,
                        AccountEmail = context.Message.Email,
                        ContactPersonFirstName = context.Message.FirstName,
                        ContactPersonLastName = context.Message.LastName,
                        ContactPersonPhoneNumber = context.Message.PhoneNumber
                    });
                }
            }

        }
    }
}
