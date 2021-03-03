using System.ComponentModel.DataAnnotations;

namespace ThemePark.ApplicationDto.BasicData
{
    public class DeleteParkInput
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}
