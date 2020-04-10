using System;
using System.Threading.Tasks;

using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Extension;
using IntroducingServiceBus.Sender.Abstract;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroducingServiceBus.Sender.Service
{
    class SendDataService : ISendDataService
    {
        private readonly ILogger<SendDataService> logger;
        private readonly IBus bus;
        private readonly ServiceBusSettings serviceBusSettings;

        public SendDataService(ILogger<SendDataService> logger, IBus bus, IOptionsMonitor<ServiceBusSettings> options)
        {
            this.logger = logger;
            this.bus = bus;
            serviceBusSettings = options.CurrentValue;
            logger.LogInformation("SendDataProcess.SendDataProcess has been called");
        }

        public async Task<IResponseContractData> PostAsync(IRequestContractData contractDataRequest)
        {
            logger.LogInformation("SendDataProcess.PostAsync has been called");

            try
            {
                IRequestClient<IRequestContractData, IResponseContractData> client = bus.CreateRequestClient<IRequestContractData, IResponseContractData>(new Uri($"{serviceBusSettings.Url}{serviceBusSettings.Queue}"), TimeSpan.FromSeconds(15));

                var responseContractData = await client.Request(contractDataRequest);

                logger.LogInformation($"Received response: {responseContractData.AsString()}");

                return responseContractData;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message + "\n" + ex.InnerException?.ToString());
                throw ex;
            }
        }
    }
}
