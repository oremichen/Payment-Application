using ContentServiceManagementAPI.Services.VasProviderAppService;
using MassTransit;
using System.Threading.Tasks;
using ANQ.Messages.NonScheduledTasks.UserProfile;
using ContentServiceManagementAPI.Models.DTO.DeliveryService;

namespace ContentServiceManagementAPI.Events
{
    public class CompleteVasConfigurationConsumer : IConsumer<ICompleteVhpVasConfiguration>
    {
        private readonly IVasProviderAppService _vasProviderAppService;
        public CompleteVasConfigurationConsumer(
            IVasProviderAppService vasProviderAppService
            )
        {
            _vasProviderAppService = vasProviderAppService;
        }


        public async Task Consume(ConsumeContext<ICompleteVhpVasConfiguration> context)
        {
            VasProviderConfigurationDto vasProviderConfigurationDto = new VasProviderConfigurationDto() {
                ApiKey = context.Message.ApiKey,
                serviceProviderDto = new Models.DTO.ServiceProviderDto()
                {
                    AccountEmail = context.Message.AccountEmail,
                    ApplicationId = context.Message.ApplicationId,
                    Approved = context.Message.Approved,
                    AuthenticationId = context.Message.AuthenticationId,
                    ContactPersonEmail = context.Message.ContactPersonEmail,
                    ContactPersonFirstName = context.Message.ContactPersonFirstName,
                    ContactPersonLastName = context.Message.ContactPersonLastName,
                    ContactPersonPhoneNumber = context.Message.ContactPersonPhoneNumber,
                    ServiceProviderId = context.Message.ServiceProviderId,
                    ServiceProviderName = context.Message.ServiceProviderName,
                    Status = (Enums.EntityStatus)((long)context.Message.Status),
                    VasLicenseActiveDate = context.Message.VasLicenseActiveDate,
                    VasLicenseExpiryDate = context.Message.VasLicenseExpiryDate,
                    VasLicenseId = context.Message.VasLicenseId
                }
            };

            await _vasProviderAppService.CompleteVasProviderConfiguration(vasProviderConfigurationDto);
        }
    }
}
