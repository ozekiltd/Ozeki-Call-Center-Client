using System.Windows.Controls;
using OPSCallCenterCRMAPI;

namespace OPSCallCenterCRM.View
{
    /// <summary>
    /// Interaction logic for IncomingCallPopup.xaml
    /// </summary>
    public partial class IncomingCallPopup : UserControl
    {
        public IncomingCallPopup(CrmEntry entry)
        {
            InitializeComponent();
            DataContext = entry;
        }
    }
}
