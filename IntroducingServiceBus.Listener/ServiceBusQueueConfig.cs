namespace IntroducingServiceBus.Listener
{
    //internal class BusSettings
    //{
    //    public string HostName { get; set; }
    //    public string QueueName { get; set; }
    //    public string SharedAccessKey { get; set; }
    //    public string Key { get; set; }
    //    public string FullPath => $"{HostName}/{QueueName}";
    //}

    internal class ServiceBusQueueConfig
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
