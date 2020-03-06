namespace PubSub.Subscriber2
{
    internal class ServiceBusSubscriberConfig
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}
