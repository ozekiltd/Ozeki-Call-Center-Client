namespace OPSCallCenterCRMAPI.WCF
{
    public class WcfConnectionProperties
    {
        public static WcfConnectionProperties Default = new WcfConnectionProperties("localhost", 23200);

        public string Address { get; set; }
        public int Port { get; set; }

        public string ConnectionString { get { return string.Format("net.tcp://{0}:{1}", Address, Port); } }

        public WcfConnectionProperties(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public WcfConnectionProperties()
        { }
    }
}
