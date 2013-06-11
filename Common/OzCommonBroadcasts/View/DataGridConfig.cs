using System.Collections.Generic;

namespace OzCommonBroadcasts.View
{
    class DataGridConfig
    {
        public List<string> ReadOnlyColumns { get; private set; }
        public List<string> InvisibleColumns { get; private set; }

        public DataGridConfig()
        {
            ReadOnlyColumns = new List <string>();
            InvisibleColumns = new List <string>();
        }

        public void AddReadOnlyColumnName(string columnName)
        {
            ReadOnlyColumns.Add(columnName);
        }

        public void AddInvisibleColumnName(string columnName)
        {
            InvisibleColumns.Add(columnName);
        }
    }
}
