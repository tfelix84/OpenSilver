
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Represents a header that encapsulates an address information item used to
    /// identify or interact with an endpoint.
    /// </summary>
    public abstract partial class AddressHeader
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.Channels.AddressHeader
        /// class.
        /// </summary>
        protected AddressHeader() { }

        //
        // Summary:
        //     When implemented, gets the name of the address header.
        //
        // Returns:
        //     The name of the address header.
        public abstract string Name { get; }
        //
        // Summary:
        //     When implemented, gets the namespace of the address header.
        //
        // Returns:
        //     The namespace of the address header.
        public abstract string Namespace { get; }

        //
        // Summary:
        //     Creates a new instance of the System.ServiceModel.Channels.AddressHeader class
        //     with a specified value, name and namespace.
        //
        // Parameters:
        //   name:
        //     The name of the address header.
        //
        //   ns:
        //     The namespace of the address header.
        //
        //   value:
        //     The information item for the address header.
        //
        // Returns:
        //     The System.ServiceModel.Channels.AddressHeader with the specified name and ns
        //     that contains the information item specified by value.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The name is null or name.Length == 0.
        public static AddressHeader CreateAddressHeader(string name, string ns, object value)
        {
            return new PlainAddressHeader(name, ns, value);
        }


        #region Added for quick implementation

        /// <summary>
        /// When overriden, returns the value of the <see cref="AddressHeader"/> as the string that will be used in the message.
        /// </summary>
        /// <returns>Tthe value of the <see cref="AddressHeader"/> as the string that will be used in the message.</returns>
        public abstract string GetValueAsString();

        #endregion



        //
        // Summary:
        //     Creates a new instance of the System.ServiceModel.Channels.AddressHeader class
        //     with a specified name and namespace that uses a specified formatter to serialize
        //     the information item from a specified object.
        //
        // Parameters:
        //   name:
        //     The name of the address header.
        //
        //   ns:
        //     The namespace of the address header.
        //
        //   value:
        //     The information item for the address header.
        //
        //   serializer:
        //     The System.Runtime.Serialization.XmlObjectSerializer used to serialize the specified
        //     object in the value parameter.
        //
        // Returns:
        //     The System.ServiceModel.Channels.AddressHeader with the specified name and ns
        //     that contains the information item specified by value.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name or serializer is null or name.Length == 0.
        //public static AddressHeader CreateAddressHeader(string name, string ns, object value, Runtime.Serialization.XmlObjectSerializer serializer);
        ////
        //// Summary:
        ////     Determines whether the information item in a specified object is equal to the
        ////     object contained in the current address header.
        ////
        //// Parameters:
        ////   obj:
        ////     The System.Object to compare with the object contained in the current address
        ////     header.
        ////
        //// Returns:
        ////     true if the information item in a specified obj is equivalent to the information
        ////     item contained in the current address header; otherwise, false. In particular,
        ////     returns false if obj is null.
        //public override bool Equals(object obj);
        ////
        //// Summary:
        ////     Returns an XML reader that can serialize the current address header object.
        ////
        //// Returns:
        ////     An instance of System.Xml.XmlDictionaryReader that can serialize the current
        ////     address header object.
        //public virtual Xml.XmlDictionaryReader GetAddressHeaderReader();
        ////
        //// Summary:
        ////     Provides a unique hash code for an address header.
        ////
        //// Returns:
        ////     A unique hash code for the current address header.
        //public override int GetHashCode();
        ////
        //// Summary:
        ////     Deserializes the information item from the current address header to an object
        ////     of a specified type.
        ////
        //// Type parameters:
        ////   T:
        ////     A class of type T with its fields and properties set to the values supplied by
        ////     the current address header object.
        ////
        //// Returns:
        ////     An instance of a class of type T with its fields and properties set to the values
        ////     supplied by the current address header object.
        //public T GetValue<T>();
        ////
        //// Summary:
        ////     Deserializes the information item from the current address header to an object
        ////     of a specified type that uses a specified formatter to serialize this information.
        ////
        //// Parameters:
        ////   serializer:
        ////     The System.Runtime.Serialization.XmlObjectSerializer used to deserialize the
        ////     information item from the current address header object.
        ////
        //// Type parameters:
        ////   T:
        ////     A class of type T with its fields and properties set to the values supplied by
        ////     the current address header object.
        ////
        //// Returns:
        ////     An instance of a class of type T with its fields and properties set to the values
        ////     supplied by the current address header object.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The serializer is null.
        //public T GetValue<T>(Runtime.Serialization.XmlObjectSerializer serializer);
        ////
        //// Summary:
        ////     Wraps the address header as a message header.
        ////
        //// Returns:
        ////     The System.ServiceModel.Channels.MessageHeader that wraps the current address
        ////     header.
        //public MessageHeader ToMessageHeader();
        ////
        //// Summary:
        ////     Writes the address header to a stream or file using a specified System.Xml.XmlWriter.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlWriter used to write the address header to a stream or file.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The writer is null.
        //public void WriteAddressHeader(XmlWriter writer);
        ////
        //// Summary:
        ////     Writes the address header to a stream or file using a specified System.Xml.XmlDictionaryWriter.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlDictionaryWriter used to write the address header to a stream
        ////     or file.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The writer is null.
        //public void WriteAddressHeader(Xml.XmlDictionaryWriter writer);
        ////
        //// Summary:
        ////     Writes the address header contents to a stream or file.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlDictionaryWriter used to write the address header contents
        ////     to a stream or file.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The writer is null.
        //public void WriteAddressHeaderContents(Xml.XmlDictionaryWriter writer);
        ////
        //// Summary:
        ////     Starts to write the address header contents to a stream or file.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlDictionaryWriter used to write the address header to a stream
        ////     or file.
        ////
        //// Exceptions:
        ////   T:System.ArgumentNullException:
        ////     The writer is null.
        //public void WriteStartAddressHeader(Xml.XmlDictionaryWriter writer);
        ////
        //// Summary:
        ////     When overridden in a derived class, is invoked when the address header contents
        ////     are written to a stream or file.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlDictionaryWriter used to write the address header contents
        ////     to a stream or file.
        //protected abstract void OnWriteAddressHeaderContents(Xml.XmlDictionaryWriter writer);
        ////
        //// Summary:
        ////     When overridden in a derived class, is invoked when the address header contents
        ////     begin to be written to a stream or file.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlDictionaryWriter used to write the address header to a stream
        ////     or file.
        //protected virtual void OnWriteStartAddressHeader(Xml.XmlDictionaryWriter writer);
    }

    class PlainAddressHeader : AddressHeader
    {
        string value;
        string name;
        string ns;

        public PlainAddressHeader(string name, string ns, object value)
        {
            if ((null == name) || (name.Length == 0))
            {
                throw new ArgumentNullException("name");
            }

            this.value = value != null ? value.ToString() : "";
            this.name = name;
            this.ns = ns;
        }

        public override string Name
        {
            get { return name; }
        }

        public override string Namespace
        {
            get { return ns; }
        }

        public override string GetValueAsString()
        {
            return value;
        }
    }
}

#endif