Wcf 4 and Ninject 3 is a pretty sweet combination, but geting bootstrapped with it can be a tricky experience...maybe that's an understatement. Hopefully this will help.

######Pre-Requisites

1. You've spent at least 3 hours trying to bootstrap Wcf 4 and Ninject 3
2. Have sifted through at least 3 hours of countless articles only to produce 400's (Bad Request) or 500 (KaBomb) status's with informative but inactionable messages presented to you
3. Just wish there was on place where you could frikon see a working fundamental example and be spared the misery

## Brief background on Wcf 4 and json

I'm a fan of minimal configuration, and in Wcf 4 it's pretty simple to set up a service to respond to and accept json requests. Wcf services can also be served up over port 80 alongside your web application, instead of having to be hosted. Here's what that configuration looks like:

In the .svc file we have a declaration of the factory attribute pointed to "System.ServiceModel.Activation.WebServiceHostFactory" 
```xml
<%@ ServiceHost Language="C#" Debug="true" Service="Devlific.Wcf.IoC.Examples.EasyService3" CodeBehind="EasyService3.svc.cs" Factory="System.ServiceModel.Activation.WebServiceHostFactory" 
```

In web.config we have the following 2 elements, very minimal config

```xml
...
<serviceBehaviors>
	<behavior name="">
		<serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" />
		<serviceDebug includeExceptionDetailInFaults="true" />
	</behavior>
</serviceBehaviors>
... 
<serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true" />
...
```

On the operation contract we add the json formatting instructions via WebInvoke
```c#
[OperationContract]
[WebInvoke(Method = "POST",
	BodyStyle = WebMessageBodyStyle.Wrapped,
	RequestFormat = WebMessageFormat.Json,
	ResponseFormat = WebMessageFormat.Json)]
```

Lastly, we have an attribute insttruction stating that it's ok to serve up this service alongside an asp.net application over the same port
```c#
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class EasyService : IEasyService...
```

That's it, and you're off to the races serving and recieving json through Wcf. The key here is the minimal xml configuration neccessary to get going.

## Wcf 4 and Ninject 3

Suffice it to say following the instructions from the ninject wcf extensions wiki [here](https://github.com/ninject/Ninject.Extensions.Wcf/wiki/Configure-wcf-services-using-svc-file) and [here](https://github.com/ninject/Ninject.Extensions.Wcf/wiki) using my current (above) convention resulted in a series of unfortunate wcf errors, and a lot of head scratching.

I tried bootstrapping both StructureMap and Ninject with Wcf the minimal configuration route, and recieved a variety of errors. Finally though I got it working and here are some of my take aways:

1. There must be some configuration in the web.config's serviceModel element describing the service's endpoint, the behavior, and binding. This is because the Ninject.Extensions.Wcf's custom web service host factory need's to be able loop through stated endpoints to attach it's functonality and basically tell Wcf to use Ninject to perform dependency resolution.
2. The endpoints must implement "webHttpBinding", to enable exposure over http (this is key), instead of soap
3. Using "webHttpBinding" elminates the need to specify a BodyStyle, RequestFormat, and ResponseFormat 

What you'll need

1. Ninject.dll, and Ninject.Common.Web
2. The Ninject extensions for Wcf here on GitHub thanks to [scott-xu](https://github.com/scott-xu) located [here]( https://github.com/ninject/ninject.extensions.wcf)
3. To enable auto-registration for Ninject with assembly scanning you'll need the convention extension for Ninject located [here](https://github.com/ninject/ninject.extensions.conventions), thanks also to [scott-xu](https://github.com/scott-xu)
4. An Asp.Net application with some Wcf services going

## Bootstrap

So in this effort I wanted to be able to test my services using

1. client side ajax call
2. soapUI
3. fiddler
4. WcfTestClient.exe (on my machine that's located here: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE

In the web.config file I wanted to have just the baseline configuration needed to accept and respond properly through my endpoint(s)

```xml
<system.serviceModel>
  <behaviors>
    <serviceBehaviors>
      <behavior name="">
        <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" />
        <serviceDebug includeExceptionDetailInFaults="true" />
      </behavior>
      <behavior name="easyServiceBehavior">
        <serviceMetadata httpGetEnabled="true" httpsGetEnabled="true" />
        <serviceDebug includeExceptionDetailInFaults="true" />
      </behavior>
    </serviceBehaviors>
    <endpointBehaviors>
      <behavior name="webBehavior">
        <webHttp automaticFormatSelectionEnabled="true" />
      </behavior>
    </endpointBehaviors>
  </behaviors>
  <services>
    <service name="Devlific.Wcf.Ninject.Examples.EasyService" behaviorConfiguration="easyServiceBehavior">
      <endpoint address="" binding="webHttpBinding" contract="Devlific.Wcf.Ninject.Examples.IEasyService" />
      <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
    </service>
    <service name="Devlific.Wcf.Ninject.Examples.EasyService2" behaviorConfiguration="easyServiceBehavior">
      <endpoint address="" binding="webHttpBinding" contract="Devlific.Wcf.Ninject.Examples.IEasyService2" />
      <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
    </service>
    <service name="Devlific.Wcf.Ninject.Examples.EasyService3" behaviorConfiguration="easyServiceBehavior">
      <endpoint address="" binding="webHttpBinding" contract="Devlific.Wcf.Ninject.Examples.IEasyService3" />
      <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
    </service>
  </services>
  <serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true" />
</system.serviceModel>
```

I used nuget to bring down Ninject.Extensions.Wcf, and it's dependencies (Ninject, and Ninject.Common.Web). I then also used nuget to bring down Ninject.Extensions.Conventions for auto-registering my Ninject dependencies. In my Global.asax.cs file, I removed the Application_Start method and implemented the Ninject convention

```c#
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Web.Common;

namespace Devlific.Wcf.Ninject.Examples
{
    public class WebApiApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind(scanner => scanner
                .FromThisAssembly()
                .SelectAllClasses()
                .BindDefaultInterface());
            return kernel;
        }
    }
}
```

Finally in my Wcf classes I made note to specify the InstanceContextMode in my service behavior. From what I understand it is supposed to be per call but poking around in the source for Ninject.Extensions.Wcf I found that the mode was being discerned by the context, and so I rolled with that (thanks again to [scott-xu](https://github.com/scott-xu))

InstanceContextMode switching
```c#
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
```


Wcf Implementation
```c#
[ServiceContract(Namespace = "Devlific.Wcf.IoC.Examples")]
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
```

That's it. I now have a working Wcf 4 and Ninject 3 example using both 'GET', and 'POST'. Hopefully this has been helpful for you, it sure has been for me. As we all know Wcf can sometime be a pain in the ascii.

Cheers,
Devlific
