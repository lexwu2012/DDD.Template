namespace ThemePark.ApplicationDto.BasicData
{
    public class FindAddressInput
    {
        public long? ProvinceId { get; set; }
        
        public long? CityId { get; set; }
        
        public long? CountyId { get; set; }
        
        public long? StreetId { get; set; }
    }
}
