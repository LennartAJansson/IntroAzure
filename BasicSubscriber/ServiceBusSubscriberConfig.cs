namespace BasicSubscriber
{
    internal class ServiceBusSubscriberConfig
    {
        public string ConnectionString { get; set; } //Secret ConnectionStrings:ServiceBus
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}
