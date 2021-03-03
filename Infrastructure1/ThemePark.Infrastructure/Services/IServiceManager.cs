namespace ThemePark.Infrastructure.Services
{
    public interface IServiceManager
    {
        /// <summary>
        ///   Implementors should perform any initialization logic.
        /// </summary>
        void Initialize();

        /// <summary>
        /// start all services
        /// </summary>
        void StartAllServices();

        /// <summary>
        /// stop all services
        /// </summary>
        void StopAllServices();
    }
}
