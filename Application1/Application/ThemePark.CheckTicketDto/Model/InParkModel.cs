
namespace ThemePark.VerifyTicketDto.Model
{
    public class InParkModel
    {
        //
        public string VerifyCode { get; set; }

        //条码/IC卡号等唯一码
        public string Id { get; set; }
        public int NoPast { get; set; }
        public int Terminal { get; set; }
        public string Remark { get; set; }
    }
}