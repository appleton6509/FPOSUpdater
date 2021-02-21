using FPOSPriceUpdater.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSPriceUpdater.BusinessLogic
{
   public interface ITransferable
    {
        void UpdateStatus(StatusMessage status);
        bool IsReady();
    }
}
