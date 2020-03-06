using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Contract;

namespace IntroducingServiceBus.Common.Extension
{
    public static class ResponseContractDataExtension
    {
        public static IRequestContractData ToRequestContractData(this IResponseContractData responseContractData) =>
            new RequestContractData
            {
                Id = responseContractData.Id,
                CorrelationId = responseContractData.CorrelationId,
                Created = responseContractData.Created
            };

        public static string AsString(this IResponseContractData responseContractData) =>
            $"\nId: {responseContractData.Id}\nCorrelationId: {responseContractData.CorrelationId}\nCreated: {responseContractData.Created}";
    }
}
