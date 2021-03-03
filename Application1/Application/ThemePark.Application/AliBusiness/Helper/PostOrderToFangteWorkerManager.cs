using System.Collections.Generic;
using System.Threading;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using Castle.Core.Logging;
using StackExchange.Redis;
using ThemePark.AliPartner.Constants;
using ThemePark.AliPartner.Model;
using System.Threading.Tasks;

namespace ThemePark.Application.AliBusiness.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class PostOrderToFangteWorkerManager : ISingletonDependency
    {
        #region Fields
        private Dictionary<int, Thread> activeThreads = new Dictionary<int, Thread>();

        private Thread _resendThread;
        #endregion

        public void Start()
        {
            var threads = new Thread[AliAppSetingsModel.ActiveThread];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                    IocManager.Instance.Resolve<PostDataJob>().SendData(AliBusinessNotificationMethod.Send))
                {
                    IsBackground = true
                };
                threads[i].Start();
                activeThreads.Add(i, threads[i]);
                //Task.Run(() => IocManager.Instance.Resolve<PostDataJob>().SendData(AliBusinessNotificationMethod.Send));
            }
            //_resendThread = new Thread(() =>
            //    IocManager.Instance.Resolve<PostDataJob>().SendData(AliBusinessNotificationMethod.ReSend))
            //{
            //    IsBackground = true
            //};
            //_resendThread.Start();
        }

        public void Stop()
        {
            foreach (var syncThread in activeThreads)
            {
                while (syncThread.Value.IsAlive)
                {
                    Thread.Sleep(100);
                }

                activeThreads.Remove(syncThread.Key);
            }
            _resendThread.Abort();
        }
    }
}
