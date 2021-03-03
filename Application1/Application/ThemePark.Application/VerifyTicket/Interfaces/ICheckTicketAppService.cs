using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.VerifyTicketDto.Model;

namespace ThemePark.Application.VerifyTicket.Interfaces
{
    /// <summary>
    /// 验票服务
    /// </summary>
    public interface ICheckTicketAppService
    {


        ///// <summary>
        ///// 
        ///// </summary>
        //void CreateFingerCache();
        /// <summary>
        /// 验票逻辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Result<VerifyDto>> Verify(VerifyModel model);
        
        /// <summary>
        /// 写入园记录
        /// </summary>
        /// <param name="model">入园模型</param>
        /// <returns></returns>
        Task<Result> WriteInPark(InParkModel model);

        /// <summary>
        /// 登记指纹
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        Task<Result> RegisterFinger(string verifyCode, string id, ZWJType fingerType, byte[] fingerData, int terminal);
        /// <summary>
        /// 登记照片
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="photoData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        Task<Result> RegisterPhoto(string verifyCode, string id, byte[] photoData, int terminal);
        /// <summary>
        /// 管理理确认
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="confirmType"></param>
        /// <returns></returns>
        Task<Result> Confirm(string cardNo, int confirmType);

        /// <summary>
        /// 验指纹
        /// </summary>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        Task<Result<VerifyDto>> VerifyFinger(byte[] fingerData, int terminal, ZWJType fingerType);

        /// <summary>
        /// 验证指纹是否匹配（之前已经验票）
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <param name="fingerType"></param>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        Task<Result> CheckFinger(string verifyCode, string id, ZWJType fingerType, byte[] fingerData, int terminal);

        /// <summary>
        /// 取照片
        /// </summary>
        /// <param name="verifyCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<GetPhotoDto>> GetPhoto(string verifyCode, string id);
        /// <summary>
        /// 检查条码是否可登记二次入园
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        Task<Result<VerifySecondTicketDto>> CheckBarcodeSecond(string barcode);

        /// <summary>
        /// 登记人脸
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result<long>> RegisterFace(RegisterFaceModel model);
    }
}
