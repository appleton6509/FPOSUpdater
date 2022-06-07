
using System;
using System.ComponentModel;


namespace FPOSUpdater.BusinessLogic
{
    public enum TransferStatus
    {
        Started,
        Stopped,
        Failed,
        StartFailure,
        Success,
        Reset
    }
    public class StatusMessage : INotifyPropertyChanged
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { 
                if (message != value)
                {
                    RaisePropertyChange(Message);
                    message = value;
                }
            }
        }

        public Exception Exception { get; set; }
        public TransferStatus Status { get; set; }
        public StatusMessage() { }

        private StatusMessage(TransferStatus status, string message, Exception ex)
        {
            this.Status = status;
            this.Message = message;
            this.Exception = ex;
        }
        public static StatusMessage Create(TransferStatus status, string message = "", Exception ex = null)
        {
            return new StatusMessage(status, message, ex);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChange(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
