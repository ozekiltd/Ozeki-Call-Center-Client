using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using OPSCallCenterCRMAPI.Database;

namespace OPSCallCenterCRMAPI
{
    [DataContract]
    public class CrmEntry : DatabaseEntity
    {
        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string FaxNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string JobTitle { get; set; }

        [DataMember]
        public string BusinessName { get; set; }

        [DataMember]
        public List<CrmNote> Notes { get; set; }

        [DataMember]
        public List<CallHistoryEntry> CallHistoryEntries { get; set; }

       
        public CrmEntry()
        {
            CallHistoryEntries = new List<CallHistoryEntry>();
            Notes = new List<CrmNote>();
        }

        public CrmEntry(CrmEntry oldEntry)
        {
            PhoneNumber = oldEntry.PhoneNumber;
            FaxNumber = oldEntry.FaxNumber;
            Email = oldEntry.Email;
            FirstName = oldEntry.FirstName;
            MiddleName = oldEntry.MiddleName;
            LastName = oldEntry.LastName;
            Category = oldEntry.Category;
            FullName = oldEntry.FullName;
            JobTitle = oldEntry.JobTitle;
            BusinessName = oldEntry.BusinessName;
            Notes = oldEntry.Notes;
            CallHistoryEntries = oldEntry.CallHistoryEntries;
            ID = oldEntry.ID;
        }

    }
}
