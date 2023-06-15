﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591

namespace Helsenorge.Registries
{
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SubscriptionEventSource", Namespace="http://schemas.nhn.no/reg/serviceBusManagerV2")]
    public enum SubscriptionEventSource : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AddressRegister = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Resh = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Hpr = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lsr = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Cppa = 4,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EventSubscription", Namespace="http://schemas.nhn.no/reg/serviceBusManagerV2")]
    public partial class EventSubscription : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string EventNameField;
        
        private Helsenorge.Registries.SubscriptionEventSource EventSourceField;
        
        private string QueueNameField;
        
        private string UserSystemIdentField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EventName
        {
            get
            {
                return this.EventNameField;
            }
            set
            {
                this.EventNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Helsenorge.Registries.SubscriptionEventSource EventSource
        {
            get
            {
                return this.EventSourceField;
            }
            set
            {
                this.EventSourceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string QueueName
        {
            get
            {
                return this.QueueNameField;
            }
            set
            {
                this.QueueNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserSystemIdent
        {
            get
            {
                return this.UserSystemIdentField;
            }
            set
            {
                this.UserSystemIdentField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GenericFault", Namespace="http://register.nhn.no/Common")]
    public partial class GenericFault : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ErrorCodeField;
        
        private string MessageField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode
        {
            get
            {
                return this.ErrorCodeField;
            }
            set
            {
                this.ErrorCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://schemas.nhn.no/reg/serviceBusManagerV2", ConfigurationName="Helsenorge.Registries.IServiceBusManagerV2")]
    public interface IServiceBusManagerV2
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/Subscribe", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/SubscribeRespo" +
            "nse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Helsenorge.Registries.GenericFault), Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/SubscribeGener" +
            "icFaultFault", Name="GenericFault", Namespace="http://register.nhn.no/Common")]
        Helsenorge.Registries.EventSubscription Subscribe(Helsenorge.Registries.SubscriptionEventSource eventSource, string eventName, string userSystemIdent);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/Subscribe", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/SubscribeRespo" +
            "nse")]
        System.Threading.Tasks.Task<Helsenorge.Registries.EventSubscription> SubscribeAsync(Helsenorge.Registries.SubscriptionEventSource eventSource, string eventName, string userSystemIdent);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/Unsubscribe", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/UnsubscribeRes" +
            "ponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Helsenorge.Registries.GenericFault), Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/UnsubscribeGen" +
            "ericFaultFault", Name="GenericFault", Namespace="http://register.nhn.no/Common")]
        void Unsubscribe(string queueName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/Unsubscribe", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/UnsubscribeRes" +
            "ponse")]
        System.Threading.Tasks.Task UnsubscribeAsync(string queueName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/GetSubscriptio" +
            "ns", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/GetSubscriptio" +
            "nsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Helsenorge.Registries.GenericFault), Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/GetSubscriptio" +
            "nsGenericFaultFault", Name="GenericFault", Namespace="http://register.nhn.no/Common")]
        Helsenorge.Registries.EventSubscription[] GetSubscriptions();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/GetSubscriptio" +
            "ns", ReplyAction="http://schemas.nhn.no/reg/serviceBusManagerV2/IServiceBusManagerV2/GetSubscriptio" +
            "nsResponse")]
        System.Threading.Tasks.Task<Helsenorge.Registries.EventSubscription[]> GetSubscriptionsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceBusManagerV2Channel : Helsenorge.Registries.IServiceBusManagerV2, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceBusManagerV2Client : System.ServiceModel.ClientBase<Helsenorge.Registries.IServiceBusManagerV2>, Helsenorge.Registries.IServiceBusManagerV2
    {
        
        public ServiceBusManagerV2Client()
        {
        }
        
        public ServiceBusManagerV2Client(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public ServiceBusManagerV2Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ServiceBusManagerV2Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ServiceBusManagerV2Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public Helsenorge.Registries.EventSubscription Subscribe(Helsenorge.Registries.SubscriptionEventSource eventSource, string eventName, string userSystemIdent)
        {
            return base.Channel.Subscribe(eventSource, eventName, userSystemIdent);
        }
        
        public System.Threading.Tasks.Task<Helsenorge.Registries.EventSubscription> SubscribeAsync(Helsenorge.Registries.SubscriptionEventSource eventSource, string eventName, string userSystemIdent)
        {
            return base.Channel.SubscribeAsync(eventSource, eventName, userSystemIdent);
        }
        
        public void Unsubscribe(string queueName)
        {
            base.Channel.Unsubscribe(queueName);
        }
        
        public System.Threading.Tasks.Task UnsubscribeAsync(string queueName)
        {
            return base.Channel.UnsubscribeAsync(queueName);
        }
        
        public Helsenorge.Registries.EventSubscription[] GetSubscriptions()
        {
            return base.Channel.GetSubscriptions();
        }
        
        public System.Threading.Tasks.Task<Helsenorge.Registries.EventSubscription[]> GetSubscriptionsAsync()
        {
            return base.Channel.GetSubscriptionsAsync();
        }
    }
}
