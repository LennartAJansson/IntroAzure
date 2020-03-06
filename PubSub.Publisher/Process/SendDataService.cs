using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Logging;

using PubSub.Common.Abstract;
using PubSub.Publisher.Abstract;

namespace PubSub.Publisher.Process
{
    class SendDataService : ISendDataService
    {
        private readonly ILogger<SendDataService> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public SendDataService(ILogger<SendDataService> logger, IPublishEndpoint publishEndpoint)
        {
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
            logger.LogInformation("SendDataProcess.SendDataProcess has been called");
        }

        public async Task PostAsync(IRequestContractData contractDataRequest)
        {
            logger.LogInformation("SendDataProcess.PostAsync has been called");

            await publishEndpoint.Publish<IRequestContractData>(contractDataRequest);
        }
    }
}
