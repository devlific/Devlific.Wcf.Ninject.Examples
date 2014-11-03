using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Devlific.Wcf.Ninject.Examples.IoC;
using Devlific.Wcf.Ninject.Examples.Models;

namespace Devlific.Wcf.Ninject.Examples
{
    [ServiceContract(Namespace = "Devlific.Wcf.Ninject.Examples")]
    public interface IEasyService2
    {
        [WebInvoke(Method = "GET")]
        [OperationContract]
        EasyResponse2 DoWork();
    }

    [DataContract]
    public class EasyResponse2
    {
        [DataMember(Order = 1)]
        public string DoWork { get; set; }
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextModeDefinition.Mode)]
    public class EasyService2 : IEasyService2
    {
        private readonly IDoWorkWell _worker;

        public EasyService2(IDoWorkWell worker)
        {
            _worker = worker;
        }

        public EasyResponse2 DoWork()
        {
            return new EasyResponse2
            {
                DoWork = _worker.DoWork()
            };
        }
    }
}
