using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !OPENSILVER

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Represents a thread-safe, read-only collection of address headers.
    /// </summary>
    public sealed class AddressHeaderCollection : ReadOnlyCollection<AddressHeader>
    {

        ///// <summary>
        ///// Initializes a new instance of the System.ServiceModel.Channels.AddressHeaderCollection
        ///// class.
        ///// </summary>
        //public AddressHeaderCollection() { }

        // Exceptions:
        //   T:System.ArgumentException:
        //     One of the address headers in the addressHeaders parameter is null.
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.Channels.AddressHeaderCollection
        /// class from an enumerable set of address headers.
        /// </summary>
        /// <param name="addressHeaders">
        /// The System.Collections.Generic.IEnumerable`1 set of System.ServiceModel.Channels.AddressHeader
        /// objects used to initialize the collection.
        /// </param>
        public AddressHeaderCollection(IEnumerable<AddressHeader> addressHeaders) : base(addressHeaders.ToList<AddressHeader>()) { }

        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     message is null.
        ///// <summary>
        ///// Adds the headers in the collection to the headers of a specified message.
        ///// </summary>
        ///// <param name="message">The System.ServiceModel.Channels.Message to which the headers are added.</param>
        //public void AddHeadersTo(Message message);
      
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     name or ns is null.
        ///// <summary>
        ///// Finds all the address headers in the collection with the specified name and namespace.
        ///// </summary>
        ///// <param name="name">The name of the address header to be found.</param>
        ///// <param name="ns">The namespace of the address header to be found.</param>
        ///// <returns>
        ///// The System.Array of type System.ServiceModel.Channels.AddressHeader that contains
        ///// all the headers in the collection with the specified name and namespace.
        ///// </returns>
        //public AddressHeader[] FindAll(string name, string ns);

        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     name or ns is null.
        ////
        ////   T:System.ArgumentException:
        ////     There is more than one header that has the specified name and ns.
        ///// <summary>
        ///// Finds the first address header in the collection with a specified name and namespace.
        ///// </summary>
        ///// <param name="name">The name of the address header to be found.</param>
        ///// <param name="ns">The namespace of the address header to be found.</param>
        ///// <returns>
        ///// The System.ServiceModel.Channels.AddressHeader in the collection with the specified
        ///// name and namespace.
        ///// </returns>
        //public AddressHeader FindHeader(string name, string ns);
    }
}

#endif