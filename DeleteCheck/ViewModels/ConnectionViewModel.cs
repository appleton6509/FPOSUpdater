
using FPOSDB.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FPOSUpdater.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        /// <summary>
        /// The database name
        /// </summary>
        public string Database {
            get
            {
                return ConnectionString.DBName;
            }
            set
            {
                if (value != ConnectionString.DBName)
                    ConnectionString.DBName = value;
                RaisePropertyChange(nameof(Database));
            }
        }
        /// <summary>
        /// Server/Instance name for the database
        /// </summary>
        public string Instance {
            get
            {
                return ConnectionString.ServerInstance;
            }
            set
            {
                if (value != ConnectionString.ServerInstance)
                    ConnectionString.ServerInstance = value;
                RaisePropertyChange(nameof(Instance));
            }
        }

        public ConnectionViewModel()
        {


        }
    }
}
