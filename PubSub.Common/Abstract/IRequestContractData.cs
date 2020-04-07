using System;

namespace PubSub.Common.Abstract
{
    public interface IRequestContractData
    {
        Guid CorrelationId { get; set; }
        DateTimeOffset Created { get; set; }
        int Id { get; set; }
    }
}