using log4net;
using System;
using System.ComponentModel;

namespace FPOSUpdater.BusinessLogic
{

    public class Transferable : INotifyPropertyChanged
    {
        internal string fullPath;

        /// <summary>
        /// Full file path including filename
        /// </summary>
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                if (value != path)
                    fullPath = value;
                RaisePropertyChange(nameof(FullPath));

            }
        }

        internal string path;
        /// <summary>
        /// Directory path
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (value != path)
                    path = value;
                RaisePropertyChange(nameof(Path));

            }
        }
        internal bool isNotRunning = true;
        public bool IsNotRunning
        {
            get
            {
                return isNotRunning;
            }
            set
            {
                if (value != isNotRunning)
                    isNotRunning = value;
                RaisePropertyChange(nameof(IsNotRunning));

            }
        }
        internal bool isDone = false;
        public bool IsDone
        {
            get
            {
                return isDone;
            }
            set
            {
                if (value != isDone)
                    isDone = value;
                RaisePropertyChange(nameof(IsDone));
            }
        }
        internal string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                    status = value;
                RaisePropertyChange(nameof(this.Status));

            }
        }
        public ILog Log;
        public Transferable(ILog log)
        {
            this.Log = log;
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
