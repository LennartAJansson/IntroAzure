
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Logging;

using PubSub.Common.Abstract;
using PubSub.Common.Extension;

namespace PubSub.Subscriber2.Service
{
    class RequestDataConsumer : IConsumer<IRequestContractData>
    {
        //https://masstransit-project.com/MassTransit/usage/request-response.html
        private readonly ILogger<RequestDataConsumer> logger;

        public RequestDataConsumer(ILogger<RequestDataConsumer> logger) =>
            this.logger = logger;

        public Task Consume(ConsumeContext<IRequestContractData> context)
        {
            IRequestContractData requestContractData = context.Message;

            logger.LogInformation("Received request:");
            logger.LogInformation(requestContractData.AsString());

            return Task.CompletedTask;
        }
    }
}
