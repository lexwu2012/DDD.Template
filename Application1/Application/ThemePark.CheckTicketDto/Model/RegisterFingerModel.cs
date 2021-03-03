
namespace ThemePark.VerifyTicketDto.Model
{
    public class RegisterFingerModel
    {
        public string VerifyCode { get; set; }
        public string Id { get; set; }
        public byte[] FingerData { get; set; }
        public int FingerType { get; set; }
        public int Terminal { get; set; }
    }
}