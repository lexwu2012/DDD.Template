using System.Reflection;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using ThemePark.Application.AgentTicket;
using ThemePark.ApplicationDto;
using ThemePark.Application.Payment;
using ThemePark.EntityFramework;
using ThemePark.Application.SaleCard;
using ThemePark.Application.DataSync;
using ThemePark.Application.OTA.V1;
using ThemePark.Application.InPark;
using ThemePark.Application.Message;
using ThemePark.Application.VerifyTicket.Finger;
using ThemePark.Application.DataSync.Interfaces;

namespace ThemePark.Application
{
    /// <inheritdoc />
    [DependsOn(typeof(ThemeParkDataModule), typeof(AbpAutoMapperModule), typeof(ThemeParkApplicationDtoModule))]
    public class ThemeParkApplicationModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            Configuration.Settings.Providers.Add<PaymentSettingsProvider>();
            Configuration.Settings.Providers.Add<SaleCardSettingsProvider>();
            Configuration.Settings.Providers.Add<EnterBillSettingsProvider>();
            Configuration.Settings.Providers.Add<DataSyncSettingsProvider>();
            Configuration.Settings.Providers.Add<V1OTASettingsProvider>();
            Configuration.Settings.Providers.Add<SendMessageSettingsProvider>();
            Configuration.Settings.Providers.Add<TicketSettingsProvider>();
            Configuration.Settings.Providers.Add<TravelSettingsProvider>();
            Configuration.Settings.Providers.Add<PrePaymentSettingsProvider>();

            IocManager.Register(typeof(FingerService), DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.RegisterIfNot<IDataSyncManager, DataSyncManager>(DependencyLifeStyle.Singleton);
        }
    }
}
