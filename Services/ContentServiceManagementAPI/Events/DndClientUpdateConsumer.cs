using ANQ.Messages.NonScheduledTasks.UserProfile;
using ContentServiceManagementAPI.Services.ClientAppService;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Events
{
    public class DndClientUpdateConsumer : IConsumer<ISendDndClientUpdate>
    {
        private readonly IClientAppService _ClientAppService;


        public DndClientUpdateConsumer(IClientAppService ClientAppService)
        {
            _ClientAppService = ClientAppService;
        }

        // public async Task Consume(ConsumeContext context)

        public async Task Consume(ConsumeContext<ISendDndClientUpdate> context)
        {
            try
            {
                var Client = await _ClientAppService.GetClientById(context.Message.ClientId);
                if(Client != null)
                {
                    await _ClientAppService.UpdateDndClient(new Models.DTO.Client.DndClientUpdateRequest
                    {
                        ClientId = context.Message.ClientId,
                        EmailAddress = context.Message.EmailAddress,
                        FirstName = context.Message.FirstName,
                        LastName = context.Message.LastName
                    });
                }

            }
            catch(Exception ex)
            {

            }
        }
    }
}
