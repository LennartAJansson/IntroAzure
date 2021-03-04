using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Extension;

using MassTransit;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace IntroducingServiceBus.Listener.Service
{
    internal class RequestDataConsumer : IConsumer<IRequestContractData>
    {
        //https://masstransit-project.com/MassTransit/usage/request-response.html
        private readonly ILogger<RequestDataConsumer> logger;

        public RequestDataConsumer(ILogger<RequestDataConsumer> logger)
        {
            this.logger = logger;
            logger.LogInformation("Creating RequestDataConsumer");
        }

        public async Task Consume(ConsumeContext<IRequestContractData> context)
        {
            IRequestContractData requestContractData = context.Message;

            logger.LogInformation($"Received request: {requestContractData.AsString()}");

            IResponseContractData responseContractData = requestContractData.ToResponseContractData();

            await context.RespondAsync<IResponseContractData>(responseContractData);
        }
    }
}