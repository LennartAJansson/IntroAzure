using System;

namespace IntroducingServiceBus.Common.Abstract
{
    public interface IResponseContractData
    {
        Guid CorrelationId { get; set; }
        DateTimeOffset Created { get; set; }
        int Id { get; set; }
    }
}
