namespace Domain.Model.RabbitMQ
{
    public class RabbitMQSettingModel
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Uri { get; set; }
    }
}
