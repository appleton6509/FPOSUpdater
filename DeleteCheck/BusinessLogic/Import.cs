using FPOSDB.Context;
using FPOSPriceUpdater.DTO;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FPOSPriceUpdater.BusinessLogic
{
    public class Import : Transferable, ITransferable
    {
        public  Import(ILog log): base(log)
        {
            this.Log = log;
            IsDone = false;
            IsNotRunning = true;
        }
        public void UpdateStatus(StatusMessage status)
        {
            switch (status.Status)
            {
                case TransferStatus.Started:
                    this.IsDone = false;
                    this.IsNotRunning = false;
                    this.Status = status.Message;
                    Log.Info("***** Import Started *****");
                    break;
                case TransferStatus.Failed:
                    this.IsDone = true;
                    this.IsNotRunning = true;
                    this.Status = status.Message;
                    Log.Error(status.Message, status.Exception);
                    Log.Error("***** Import Failed *****");
                    break;
                case TransferStatus.StartFailure:
                    this.Status = status.Message;
                    Log.Error(status.Exception);
                    break;
                case TransferStatus.Stopped:
                    this.IsDone = true;
                    this.IsNotRunning = true;
                    Log.Info("***** Import Stopped *****");
                    break;
                case TransferStatus.Success:
                    this.IsDone = true;
                    this.IsNotRunning = true;
                    Log.Info(status.Message);
                    this.Status = status.Message;
                    Log.Info("***** Import Successful *****");
                    break;
            }
        }
        public bool IsReady()
        {
            if (String.IsNullOrEmpty(this.Path) || !File.Exists(this.Path))
            {
                this.UpdateStatus(StatusMessage.Create(TransferStatus.StartFailure, "Failed!" + Environment.NewLine + "Import file is missing or invalid"));
                return false;
            }
            DBService db = new DBService(ConnectionString.GetString());
            if (!db.IsConnectionValid())
            {
                this.UpdateStatus(StatusMessage.Create(TransferStatus.StartFailure, $"Failed! {Environment.NewLine} Check your database connection."));
                return false;
            }
            return true;
        }

    }
}
