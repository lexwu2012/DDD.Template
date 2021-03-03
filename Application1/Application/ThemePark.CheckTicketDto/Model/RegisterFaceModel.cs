namespace ThemePark.VerifyTicketDto.Model
{
    public class RegisterFaceModel
    {
        public string VerifyCode { get; set; }
        public int Terminal { get; set; }
        public byte[] FaceData { get; set; }
        public byte[] FaceFeat { get; set; }
    }
}
