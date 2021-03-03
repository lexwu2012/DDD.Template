using System.Collections.Generic;
using System.ServiceModel;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Infrastructure.Services;

namespace ThemePark.Services.Interface
{
    [ServiceContract(Namespace = "http://www.themepark.com")]
    public interface ICityService : IWcfService
    {
        [OperationContract]
        IList<CityDto> DefaultCitiesQuery();
    }
}
