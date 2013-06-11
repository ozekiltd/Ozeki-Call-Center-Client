namespace OPSCallCenterCRMServer.Model.Database
{
    public class DatabaseConnectionProperties
    {
        public static DatabaseConnectionProperties Default = new DatabaseConnectionProperties("localhost", 23238);

        public string Address { get; set; }
        public int Port { get; set; }

        public string ConnectionString { get { return string.Format("mongodb://{0}:{1}", Address, Port); } }

        public DatabaseConnectionProperties(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public DatabaseConnectionProperties()
        { }
    }
}
