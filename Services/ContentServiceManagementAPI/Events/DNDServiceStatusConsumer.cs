using Anq.Enums;
using ANQ.Messages.ScheduledTasks.ServiceStatus;
using ContentServiceManagementAPI.Services.ServiceAppService;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Events
{
    public class DNDServiceStatusConsumer : IConsumer<ISendScheduleToDNDServiceStatus>
    {
        private readonly IServiceAppService _serviceAppService;


        public DNDServiceStatusConsumer(IServiceAppService serviceAppService)
        {
            _serviceAppService = serviceAppService;
        }

        // public async Task Consume(ConsumeContext context)

        public async Task Consume(ConsumeContext<ISendScheduleToDNDServiceStatus> context)
        {
            await ChangeServiceStatus();
        }

        public async Task ChangeServiceStatus()
        {
            var services = await _serviceAppService.GetAll((long)ApplicationEnum.DND);

            foreach (var service in services)
            {
                if (service.ApplicationId == (int)ApplicationEnum.DND)
                {
                    int serviceStatusId = (int)service.ServiceStatus;

                    if ((serviceStatusId == (int)ServiceStatus.Partial) && (service.ExpiryDate <= DateTime.Now))
                    {
                        service.ServiceStatus = (int)ServiceStatus.Inactive;
                        await _serviceAppService.UpdateServiceStatus(service);
                    }

                    if ((serviceStatusId == (int)ServiceStatus.Active) && (service.ExpiryDate <= DateTime.Now))
                    {
                        service.ServiceStatus = (int)ServiceStatus.Inactive;
                        await _serviceAppService.UpdateServiceStatus(service);
                    }

                    if ((serviceStatusId == (int)ServiceStatus.Inactive) && (service.ActiveDate <= DateTime.Now))
                    {
                        service.ServiceStatus = (int)ServiceStatus.Active;
                        await _serviceAppService.UpdateServiceStatus(service);
                    }
                }

            }
        }

    }
}
