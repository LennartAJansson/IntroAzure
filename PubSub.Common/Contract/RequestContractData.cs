using PubSub.Common.Abstract;

using System;

namespace PubSub.Common.Contract
{
    //Data model for a queue request
    //SHOW! Using partial to be able to extend with methods if neccessary
    public partial class RequestContractData : IRequestContractData
    {
        public Guid CorrelationId { get; set; }
        public DateTimeOffset Created { get; set; }
        public int Id { get; set; }
    }
}
