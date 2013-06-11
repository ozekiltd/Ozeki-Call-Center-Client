using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSSDK;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.Utils.Schedule;
using OzCommon.ViewModel;
using OzCommonBroadcasts.Model;
using OzCommonBroadcasts.Model.Csv;
using OzCommonBroadcasts.Utils;

namespace OzCommonBroadcasts.ViewModel
{

    public abstract class BroadcastMainViewModel<T> : CommonMainViewModel<T>, IBroadcastMainViewModel where T : EventArgs, ICompletedWork, new()
    {
        #region Declares

        #region RelayCommmands
        public RelayCommand Start { get; protected set; }
        public RelayCommand Stop { get; protected set; }
        public RelayCommand New { get; protected set; }
        public RelayCommand Open { get; protected set; }
        public RelayCommand Save { get; protected set; }
        public RelayCommand<IList> Cut { get; protected set; }
        public RelayCommand<IList> Copy { get; protected set; }
        public RelayCommand<IList> Paste { get; protected set; }
        public RelayCommand ShowSettings { get; protected set; }

        #endregion

        #region Properties

        public ObservableCollectionEx<T> Customers { get; set; }
        public ApplicationInformation ApplicationInformation { get; private set; }
        private readonly ICsvImporter<T> csvImporter;
        private readonly ICsvExporter<T> csvExporter;
        private readonly ClipboardHelper<T> clipboardHelper;
        private readonly IDialogService dialogService;
        private readonly IScheduler<T> scheduler;
        private readonly IExtensionContainer extensionContainer;
        private readonly IClient client;
        private readonly object sync;

        private Int32 allJobs;
        private Int32 doneJobs;
        private bool isReadOnly;
        private Int32 checkedJobs;


        #region Properties => OnProertyChanged

        public Int32 AllJobs
        {
            get { return allJobs; }
            set
            {
                allJobs = value;
                RaisePropertyChanged(() => AllJobs);
            }
        }

        public Int32 DoneJobs
        {
            get { return doneJobs; }
            set
            {
                doneJobs = value;
                RaisePropertyChanged(() => DoneJobs);
            }
        }

        public Boolean IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; RaisePropertyChanged(() => IsReadOnly); }
        }

        public Int32 CheckedJobs
        {
            get { return checkedJobs; }
            set { checkedJobs = value; RaisePropertyChanged(() => CheckedJobs); }
        }

        #endregion

        #endregion

        #endregion

        public BroadcastMainViewModel()
        {
            #region Initialising

            InitBroadcastCommands();
            ApplicationInformation = SimpleIoc.Default.GetInstance<ApplicationInformation>();
            Customers = new ObservableCollectionEx<T>();
            clipboardHelper = new ClipboardHelper<T>();
            dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            csvImporter = SimpleIoc.Default.GetInstance<ICsvImporter<T>>();
            csvExporter = SimpleIoc.Default.GetInstance<ICsvExporter<T>>();
            scheduler = SimpleIoc.Default.GetInstance<IScheduler<T>>();
            extensionContainer = SimpleIoc.Default.GetInstance<IExtensionContainer>();
            client = SimpleIoc.Default.GetInstance<IClient>();
            sync = new object();

            #endregion

            #region Subscribes
            clipboardHelper.ItemLoaded += ClipboardHelperOnItemLoaded;
            csvExporter.AllItemSaved += csvExporter_AllItemSaved;
            csvImporter.ItemLoaded += csvLoader_ItemLoaded;
            csvImporter.AllItemLoaded += csvImporter_AllItemLoaded;
            Customers.CollectionChanged += Customers_CollectionChanged;
            client.ErrorOccurred += ClientOnErrorOccurred;

            scheduler.WorksCompleted += SchedulerOnWorksCompleted;
            scheduler.OneWorkCompleted += SchedulerOneWorkCompleted;
            #endregion
        }

        public void InitBroadcastCommands()
        {
            #region Commands

            Start = new RelayCommand(() =>
                                         {
                                             var id = GetApiExtensionID();

                                             if (string.IsNullOrEmpty(id))
                                             {
                                                 dialogService.ShowMessageBox("Please set the extension id in settings window before start your works.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                 return;
                                             }

                                             try
                                             {
                                                 Messenger.Default.Send(new NotificationMessage(Messages.ReValidateAllRows));
                                                 var ext = extensionContainer.GetExtension();
                                                 if (ext == null || ext.ExtensionId != id)
                                                 {
                                                     Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindowLoading));
                                                     ext = client.GetAPIExtension(id);
                                                     extensionContainer.SetExtension(ext);
                                                 }

                                                 if (ValidateRowsBeforeStarting()) return;
                                                 Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));

                                                 if (ext == null)
                                                 {
                                                     dialogService.ShowMessageBox("API extension not configured or disabled on Ozeki Phone System XE.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                     return;
                                                 }

                                                 IsReadOnly = true;
                                                 scheduler.SetMaxConcurrentWorkers(GetMaxConcurrentWorkers());
                                                 DoneJobs = 0;
                                                 CheckedJobs = 0;

                                                 scheduler.StartWorks(Customers);
                                             }
                                             catch (TargetInvocationException)
                                             {
                                                 Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));
                                                 dialogService.ShowMessageBox("Connection lost with Ozeki Phone System XE.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                             }
                                         }, () => !scheduler.Working && DoneJobs != AllJobs);

            Stop = new RelayCommand(() =>
            {
                scheduler.StopWorks();
                IsReadOnly = false;
            }, () => scheduler.Working);

            New = new RelayCommand(() =>
            {
                if (SaveBefore()) return;
                Customers.Clear();
            }, () => !scheduler.Working);

            Open = new RelayCommand(() =>
            {
                if (SaveBefore()) return;

                var fileName = dialogService.ShowOpenFileDialog("Csv or txt documents|*.csv;*.txt");
                if (String.IsNullOrEmpty(fileName)) return;

                Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindowLoading));
                Customers.Clear();
                csvImporter.LoadFile(fileName);
            },
            () => !csvImporter.IsLoading && !scheduler.Working);

            Save = new RelayCommand(() =>
            {
                var fileName = dialogService.ShowSaveFileDialog(".csv", "Csv or txt documents|*.csv;*.txt");
                if (String.IsNullOrEmpty(fileName)) return;

                Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindowSaving));
                csvExporter.ExportToFile(Customers.ToList(), fileName);
            },
            () => (!csvExporter.IsSaving && Customers.Count > 0) && !scheduler.Working);


            Paste = new RelayCommand<IList>((param) =>
            {
                RemoveEntrys(param);
                clipboardHelper.PasteData();
            }, (param) => !scheduler.Working && !string.IsNullOrEmpty(Clipboard.GetText()));
            Copy = new RelayCommand<IList>((param) =>
            {
                clipboardHelper.CopyString(param);
            }, (param) => !scheduler.Working && param != null && param.Count > 0);

            Cut = new RelayCommand<IList>((param) =>
            {
                clipboardHelper.CopyString(param);
                RemoveEntrys(param);
            }, (param) => !scheduler.Working && param != null && param.Count > 0);

            ShowSettings = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage(this, GetSettingsViewModel(), Messages.ShowSettings));
            }, () => !scheduler.Working);
            #endregion
        }

        #region abstract methods
        protected abstract object GetSettingsViewModel();

        protected abstract Int32 GetMaxConcurrentWorkers();

        protected abstract string GetApiExtensionID();
        #endregion

        #region Helpers
        public static void RaiseInvalidateRequerySuggested()
        {
            Dispatcher dispatcher = null;

            if (Application.Current != null)
                dispatcher = Application.Current.Dispatcher;

            if (dispatcher != null && !dispatcher.CheckAccess())
                dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)CommandManager.InvalidateRequerySuggested);
        }

        private bool ValidateRowsBeforeStarting()
        {
            foreach (var customer in Customers)
            {
                if (!customer.IsValid)
                {
                    Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));
                    var result = dialogService.ShowMessageBox("Some entry is invalid. If you would like to start the valid entries select 'Yes'! Elsewhere if you would like edit the rows before send please select 'No'!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                        return true;
                    else break;
                }
            }
            return false;
        }
        void RemoveEntrys(IList selectedItems)
        {
            if (selectedItems == null) return;

            var selectedItemsCount = selectedItems.Count;

            for (var i = 0; i < selectedItemsCount; i++)
                Customers.Remove(selectedItems[0] as T);
        }

        private bool SaveBefore()
        {
            if (Customers.Count > 0)
            {
                var result = dialogService.ShowMessageBox("Would you like to save changes?", "Confirmation",
                                                          MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    Save.Execute(null);
                else if (result == MessageBoxResult.Cancel)
                    return true;
            }
            DoneJobs = 0;
            return false;
        }

        void AllItemImportedExported(CounterEventArg e, string errorMessage, string successMessage)
        {
            Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));

            if (e.Count == -1)
                dialogService.ShowMessageBox(errorMessage, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                dialogService.ShowMessageBox(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public ApplicationInformation GetApplicationInformation()
        {
            return ApplicationInformation;
        }

        public void CancelLoading()
        {
            csvExporter.Cancel();
            csvImporter.Cancel();
        }

        public Type GetEntryType()
        {
            return typeof(T);
        }
        #endregion

        #region Events
        void SchedulerOneWorkCompleted(object sender, EventArgs eventArgs)
        {
            lock (sync)
            {
                ++CheckedJobs;
                var workResult = eventArgs as WorkResult;
                if (workResult != null && workResult.IsSuccess)
                    ++DoneJobs;
            }
        }

        void SchedulerOnWorksCompleted(object sender, CounterEventArg eventArgs)
        {
            IsReadOnly = false;
            RaiseInvalidateRequerySuggested();
        }

        void csvImporter_AllItemLoaded(object sender, CounterEventArg e)
        {
            AllItemImportedExported(e, "Cannot import all of the rows.", e.Count + " row(s) imported.");
            Messenger.Default.Send(new NotificationMessage(Messages.ReValidateAllRows));
        }

        void csvExporter_AllItemSaved(object sender, CounterEventArg e)
        {
            AllItemImportedExported(e, "Cannot export all of the rows.", "Successfully exported all of the rows.");
        }

        void csvLoader_ItemLoaded(object sender, T e)
        {
            Customers.Add(e);
        }

        void ClipboardHelperOnItemLoaded(object sender, T e)
        {
            Customers.Add(e);
        }

        void Customers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AllJobs = Customers.Count;
        }

        private void ClientOnErrorOccurred(object sender, ErrorInfo errorInfo)
        {
            switch (errorInfo.Type)
            {
                case ErrorType.ConnectionFailure:
                    dialogService.ShowMessageBox("Network connection error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case ErrorType.VersionMismatch:
                    dialogService.ShowMessageBox("Version mismatch error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    dialogService.ShowMessageBox("Unknown error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }
        #endregion
    }
}
