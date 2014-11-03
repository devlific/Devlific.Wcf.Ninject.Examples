using System.ServiceModel;

namespace Devlific.Wcf.Ninject.Examples.IoC
{
    public static class InstanceContextModeDefinition
    {
        /// <summary>
        ///     The InstanceContextMode used for the web services.
        /// </summary>
        public const InstanceContextMode Mode =
#if SINGLETON
            InstanceContextMode.Single;
    #else
#if SESSION
                InstanceContextMode.PerSession;
        #else
            InstanceContextMode.PerCall;
#endif
#endif
    }
}