
using IntroducingServiceBus.Common.Abstract;

using System.Threading.Tasks;

namespace IntroducingServiceBus.Sender.Abstract
{
    interface ISendDataService
    {
        Task<IResponseContractData> PostAsync(IRequestContractData contractDataRequest);
    }
}
