using System.Collections.Generic;
using System.ServiceModel;

namespace OPSCallCenterCRMAPI.WCF
{
    [ServiceContract(CallbackContract = typeof(ICrmClient))]
    public interface ICrmServer
    {
        [OperationContract(IsOneWay = true)]
        void Login(ClientCredential user);

        [OperationContract(IsOneWay = true)]
        void Disconnect(ClientCredential user);

        [OperationContract(IsOneWay = true)]
        void AddEntry(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void DeleteEntry(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void ModifyEntry(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void Ping();

        [OperationContract]
        List<CrmEntry> GetAllCrmEntries();
    }
}
