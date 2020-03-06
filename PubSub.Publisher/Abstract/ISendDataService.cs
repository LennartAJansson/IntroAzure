
using System.Threading.Tasks;

using PubSub.Common.Abstract;

namespace PubSub.Publisher.Abstract
{
    interface ISendDataService
    {
        Task PostAsync(IRequestContractData contractDataRequest);
    }
}
