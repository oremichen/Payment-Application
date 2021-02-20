using ANQ.Messages.NonScheduledTasks.UserProfile;
using ANQ.Messages.NonScheduledTasks.UserProfile.Model;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Services.ClientAppService;
using ContentServiceManagementAPI.Services.ContentProviderService;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using MassTransit;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Events
{
    public class CreateUserRequestConsumer : IConsumer<ICreateUserRequest>
    {
        private readonly IClientAppService _clientAppService;
        private readonly IContentProviderAppService _contentProviderAppService;
        private readonly IServiceProviderAppService _serviceProviderAppService;

        public CreateUserRequestConsumer(IClientAppService clientAppService
            , IContentProviderAppService contentProviderAppService
            , IServiceProviderAppService serviceProviderAppService)
        {
            _clientAppService = clientAppService;
            _contentProviderAppService = contentProviderAppService;
            _serviceProviderAppService = serviceProviderAppService;
        }

        public async Task Consume(ConsumeContext<ICreateUserRequest> context)
        {
            var model = new CreateUserDto()
            {
                AccountEmail = context.Message.AccountEmail,
                AuthenticationId = context.Message.AuthenticationId,
                CompanyName = context.Message.CompanyName,
                ContactPersonEmail = context.Message.ContactPersonFirstName,
                ContactPersonFirstName = context.Message.ContactPersonFirstName,
                ContactPersonLastName = context.Message.ContactPersonLastName,
                ContactPersonPhoneNumber = context.Message.ContactPersonPhoneNumber,
                RoleName = context.Message.RoleName,
                ApplicationId = context.Message.ApplicationId
            };

            if(model.RoleName == "Client")
            {
                await SaveClient(model);
            }
            else if(model.RoleName == "Service Provider")
            {
                await SaveServiceProvider(model);
            }
            else if(model.RoleName == "Content Provider")
            {
                await SaveContentProvider(model);
            }
        }

        private async Task SaveClient(CreateUserDto model)
        {
            var data = new Client {
                AccountEmail = model.AccountEmail,
                AuthenticationId = model.AuthenticationId,
                ClientName = model.CompanyName,
                ContactPersonEmail = model.ContactPersonEmail,
                ContactPersonFirstName = model.ContactPersonFirstName,
                ContactPersonLastName = model.ContactPersonLastName,
                ContactPersonPhoneNumber = model.ContactPersonPhoneNumber,
                ApplicationId = model.ApplicationId
            };
            await _clientAppService.AddClient(data);
        }


        private async Task SaveContentProvider(CreateUserDto model)
        {
            var data = new ContentProvider
            {
                AccountEmail = model.AccountEmail,
                AuthenticationId = model.AuthenticationId,
                ContentProviderName = model.CompanyName,
                ContactPersonEmail = model.ContactPersonEmail,
                ContactPersonFirstName = model.ContactPersonFirstName,
                ContactPersonLastName = model.ContactPersonLastName,
                ContactPersonPhoneNumber = model.ContactPersonPhoneNumber,
                ApplicationId = model.ApplicationId
            };

            await _contentProviderAppService.AddContentProvider(data);
        }


        private async Task SaveServiceProvider(CreateUserDto model)
        {
            var data = new ServiceProvider
            {
                AccountEmail = model.AccountEmail,
                AuthenticationId = model.AuthenticationId,
                ServiceProviderName = model.CompanyName,
                ContactPersonEmail = model.ContactPersonEmail,
                ContactPersonFirstName = model.ContactPersonFirstName,
                ContactPersonLastName = model.ContactPersonLastName,
                ContactPersonPhoneNumber = model.ContactPersonPhoneNumber,
                ApplicationId = model.ApplicationId
            };

            await _serviceProviderAppService.AddServiceProvider(data);
        }
    }
}
