using System.Threading.Tasks;

using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Extension;

using MassTransit;

using Microsoft.Extensions.Logging;

namespace IntroducingServiceBus.Listener.Service
{
    class RequestDataConsumer : IConsumer<IRequestContractData>
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
