using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Devlific.Wcf.Ninject.Examples
{
    [ServiceContract(Namespace = "Devlific.Wcf.Ninject.Examples")]
    //[ServiceContract()]
    public interface IEasyService
    {
        [WebInvoke(Method = "GET")]
        [OperationContract]
        string DoWork();
    }

    [ServiceBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class EasyService : IEasyService
    {
        public string DoWork()
        {
            return "Hello World";
        }
    }
}
