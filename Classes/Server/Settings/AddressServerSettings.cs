namespace Classes.Server.Settings
{
    public class AddressServerSettings
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int ClientCount { get; set; } = 1024;
    }
}
