using System;

using IntroducingServiceBus.Common.Abstract;

namespace IntroducingServiceBus.Common.Contract
{
    public partial class ResponseContractData : IResponseContractData
    {
        public Guid CorrelationId { get; set; }
        public DateTimeOffset Created { get; set; }
        public int Id { get; set; }
    }
}
