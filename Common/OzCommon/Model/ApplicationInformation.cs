namespace OzCommon.Model
{
    public class ApplicationInformation
    {
        public string ProductName { get; set; }

        public ApplicationInformation(string productName)
        {
            ProductName = productName;
        }
    }
}
