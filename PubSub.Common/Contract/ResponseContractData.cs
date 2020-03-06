using PubSub.Common.Abstract;

using System;

namespace PubSub.Common.Contract
{
    //Data model for a queue response
    public partial class ResponseContractData : IResponseContractData
    {
        public Guid CorrelationId { get; set; }
        public DateTimeOffset Created { get; set; }
        public int Id { get; set; }
    }
}
