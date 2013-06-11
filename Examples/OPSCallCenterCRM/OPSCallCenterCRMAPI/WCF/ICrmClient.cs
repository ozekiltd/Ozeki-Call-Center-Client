using System.ServiceModel;

namespace OPSCallCenterCRMAPI.WCF
{
    [ServiceContract]
    public interface ICrmClient
    {
        [OperationContract(IsOneWay = true)]
        void CallReceived(CallSessionInfo session);

        [OperationContract(IsOneWay = true)]
        void CallHistoryEntryAdded(CallHistoryEntry entry);

        [OperationContract(IsOneWay = true)]
        void CrmEntryAdded(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void CrmEntryDeleted(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void CrmEntryModified(CrmEntry entry);

        [OperationContract(IsOneWay = true)]
        void Ping();
    }
}
