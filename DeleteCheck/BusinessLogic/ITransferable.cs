
namespace FPOSPriceUpdater.BusinessLogic
{
   public interface ITransferable
    {
        void UpdateStatus(StatusMessage status);
        bool IsReady();
    }
}
