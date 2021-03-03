using System;
using System.Collections.Generic;
using Nito.AsyncEx;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Services.Interface;

namespace ThemePark.Services.Implementation
{
    public class CityService : ICityService, IDisposable
    {
        private readonly ICityAppService _cityAppService;

        public CityService(ICityAppService cityAppService)
        {
            _cityAppService = cityAppService;
        }

        public IList<CityDto> DefaultCitiesQuery()
        {
            return AsyncContext.Run(() => _cityAppService.GetAllProvincesAsync());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
