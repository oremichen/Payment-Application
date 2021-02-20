using ANQ.Messages.NonScheduledTasks.UserProfile;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Events
{
    public class DndSpUpdateConsumer : IConsumer<ISendDndSpUpdate>
    {
        private readonly IServiceProviderAppService _serviceProviderAppService;


        public DndSpUpdateConsumer(IServiceProviderAppService serviceProviderAppService)
        {
            _serviceProviderAppService = serviceProviderAppService;
        }

        // public async Task Consume(ConsumeContext context)

        public async Task Consume(ConsumeContext<ISendDndSpUpdate> context)
        {
            try
            {
                var ServiceProvider = await _serviceProviderAppService.GetServiceProvider(context.Message.ServiceProviderId);
                if (ServiceProvider != null)
                {
                    await _serviceProviderAppService.UpdateDndServiceProvider(new Models.DTO.ServiceProvider.DndSpUpdateRequest
                    {
                        ServiceProviderId = context.Message.ServiceProviderId,
                        PhoneNumber = context.Message.PhoneNumber,
                        FirstName = context.Message.FirstName,
                        LastName = context.Message.LastName
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
