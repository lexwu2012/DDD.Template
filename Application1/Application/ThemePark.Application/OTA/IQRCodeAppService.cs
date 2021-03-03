using System.Drawing;
using Abp.Application.Services;

namespace ThemePark.Application.OTA
{
    public interface IQRCodeAppService: IApplicationService
    {

        /// <summary>
        /// 生成二维码图片，返回图片地址
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Image GenerateQRCodeImg(string code);
    }

}
