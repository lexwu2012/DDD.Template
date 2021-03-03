
namespace ThemePark.VerifyTicketDto.Model
{
    public class RegisterPhotoModel
    {
        public string VerifyCode { get; set; }
        public string Id { get; set; }
        public int Terminal { get; set; }
        public byte[] PhotoData { get; set; }
    }
}