namespace Domain.Model.Redis
{
    public class RedisSettingModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int DefaultDatabase { get; set; }
    }

}
