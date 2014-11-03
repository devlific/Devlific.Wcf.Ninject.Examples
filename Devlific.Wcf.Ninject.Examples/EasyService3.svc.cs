using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Devlific.Wcf.Ninject.Examples.IoC;
using Devlific.Wcf.Ninject.Examples.Models;

namespace Devlific.Wcf.Ninject.Examples
{
    [ServiceContract(Namespace = "Devlific.Wcf.Ninject.Examples")]
    public interface IEasyService3
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        EasyResponse3 DoWork(EasyRequest3 tally);
    }

    [DataContract]
    public class EasyResponse3
    {
        [DataMember(Order = 1)]
        public string DoWork { get; set; }
    }

    [DataContract]
    public class EasyRequest3
    {
        [DataMember(Order = 1)]
        public string WorkTally { get; set; }
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextModeDefinition.Mode)]
    public class EasyService3 : IEasyService3
    {
        private readonly IDoWorkWell _worker;

        public EasyService3(IDoWorkWell worker)
        {
            _worker = worker;
        }

        public EasyResponse3 DoWork()
        {
            return new EasyResponse3
            {
                DoWork = _worker.DoWork()
            };
        }

        public EasyResponse3 DoWork(EasyRequest3 tally)
        {
            return new EasyResponse3
            {
                DoWork = _worker.DoWork()
            };
        }
    }
}
