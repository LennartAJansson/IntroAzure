using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Contract;

namespace IntroducingServiceBus.Common.Extension
{
    public static class RequestContractDataExtension
    {
        public static IResponseContractData ToResponseContractData(this IRequestContractData requestContractData) =>
            new ResponseContractData
            {
                Id = requestContractData.Id,
                CorrelationId = requestContractData.CorrelationId,
                Created = requestContractData.Created
            };

        public static string AsString(this IRequestContractData requestContractData) =>
            $"\nId: {requestContractData.Id}\nCorrelationId: {requestContractData.CorrelationId}\nCreated: {requestContractData.Created}";
    }
}
