using FPOSDB.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPOSPriceUpdater.ViewModels
{
    public class ConnectionBaseViewModel : INotifyPropertyChanged
    {
        private bool _isTestRunning = false;
        public bool IsTestRunning
        {
            get
            {
                return !_isTestRunning;
            }
            set
            {
                if (value != _isTestRunning)
                {
                    this._isTestRunning = value;
                    RaisePropertyChange(nameof(IsTestRunning));
                }
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (value != _isConnected)
                {
                    this._isConnected = value;
                    RaisePropertyChange(nameof(IsConnected));
                }
            }
        }

        private string _connectionStatus;
        public string ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                if (value != _connectionStatus)
                {
                    this._connectionStatus = value;
                    RaisePropertyChange(nameof(ConnectionStatus));
                }

            }
        }

        public void TestConnection()
        {
            IsTestRunning = true;
            DBService db = new DBService(ConnectionString.GetString());
            Task.Run(() =>
            {
                bool isValid = db.IsConnectionValid();
                IsConnected = isValid;
                IsTestRunning = false;
                if (isValid)
                    ConnectionStatus = "Connected!";
                else
                    ConnectionStatus = "Not Connected";
            });
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
