using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Description
{
    //
    // Summary:
    //     Represents the endpoint for a service that allows clients of the service to find
    //     and communicate with the service.
    [DebuggerDisplay("Address={address}")]
    [DebuggerDisplay("Name={name}")]
    public class ServiceEndpoint
    {
#if OPENSILVER
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Description.ServiceEndpoint
        //     class for a specified contract.
        //
        // Parameters:
        //   contract:
        //     The System.ServiceModel.Description.ContractDescription for the service endpoint.
        [OpenSilver.NotImplemented]
        public ServiceEndpoint(ContractDescription contract);
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Description.ServiceEndpoint
        //     class with a specified contract, binding, and address.
        //
        // Parameters:
        //   contract:
        //     The System.ServiceModel.Description.ContractDescription for the service endpoint.
        //
        //   binding:
        //     The System.ServiceModel.Channels.Binding that specifies how the service endpoint
        //     communicates with the world.
        //
        //   address:
        //     The System.ServiceModel.EndpointAddress for the service endpoint.
		[OpenSilver.NotImplemented]
        public ServiceEndpoint(ContractDescription contract, Binding binding, EndpointAddress address);
#endif

        //
        // Summary:
        //     Gets or sets the endpoint address for the service endpoint.
        //
        // Returns:
        //     The System.ServiceModel.EndpointAddress that specifies the location of the service
        //     endpoint.
        public EndpointAddress Address { get; set; }
#if OPENSILVER
        //
        // Summary:
        //     Gets the behaviors for the service endpoint.
        //
        // Returns:
        //     The System.Collections.Generic.KeyedByTypeCollection`1 of type System.ServiceModel.Description.IEndpointBehavior
        //     that contains the behaviors specified for the service endpoint.
        [OpenSilver.NotImplemented]
        public KeyedByTypeCollection<IEndpointBehavior> Behaviors { get; }
#endif

        //
        // Summary:
        //     Gets or sets the binding for the service endpoint.
        //
        // Returns:
        //     The System.ServiceModel.Channels.Binding for the service endpoint.
        [OpenSilver.NotImplemented]
        public Binding Binding { get; set; }
#if OPENSILVER
        //
        // Summary:
        //     Gets the contract for the service endpoint.
        //
        // Returns:
        //     The System.ServiceModel.Description.ContractDescription that specifies the contract
        //     for the service endpoint.
        [OpenSilver.NotImplemented]
        public ContractDescription Contract { get; }
#endif
        //
        // Summary:
        //     Gets or sets the name of the service endpoint.
        //
        // Returns:
        //     The name of the service endpoint. The default value is the concatenation of the
        //     binding name and the contract description name. For example, System.ServiceModel.Channels.Binding.Name
        //     + '_' + System.ServiceModel.Description.ContractDescription.Name.
        public string Name { get; set; }
    }
}