using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;
using ThemePark.VerifyTicketDto.Model;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验证指纹策略
    /// </summary>
    public class CheckFingerStrategy : CheckStrategyBase
    {
        /// <summary>
        /// 验证指纹
        /// </summary>
        /// <param name="finger"></param>
        /// <param name="terminal"></param>
        /// <param name="fingerType"></param>
        /// <returns></returns>
        public async Task<Result<VerifyDto>> Verify(byte[] finger, int terminal, ZWJType fingerType)
        {
            if (fingerType == ZWJType.ZK)
            {
                var result = await VerifyZkFinger(finger, terminal);
                if (result != null)
                {
                    return result;
                }
            }
            return Failed("", VerifyType.InvalidFinger, "指纹无效");
        }

        /// <summary>
        /// 验证指纹(中控指纹机)
        /// </summary>
        /// <param name="fingerData"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task<Result<VerifyDto>> VerifyZkFinger(byte[] fingerData, int terminal)
        {
            //var checkData = new TicketCheckData()
            //{
            //    VerifyCodeType = VerifyType.Finger,
            //    Terminal = terminal

            //};

            //int imageSize = fingerData.Length;

            //var verifyFingerDto = new VerifyFingerDto();

            //try
            //{
            //    char[] charImageData;
            //    charImageData = new char[(imageSize) / 2];
            //    Buffer.BlockCopy(fingerData, 0, charImageData, 0, imageSize);
            //    string strImage = new string(charImageData);
            //    //验证指纹
            //    object obj = null;

            //    //匹配指纹
            //    int extractResult = ((ZKFPClass)FingerCache.ZkFPclass).zkfp.ExtractImageFromURU(strImage, imageSize, true, ref obj);
            //    if (extractResult > 0)
            //    {
            //        foreach (List<FingerDataItem> item in FingerCache.DicFinger.Values)
            //        {
            //            for (int i = 0; i < item.Count; i++)
            //            {
            //                if (item[i].EnrollId == extractResult)
            //                {
            //                    //判断指纹类型 年卡、二次入园、套票
            //                    if (item[i].FingerType == FingerType.VipCard)
            //                    {
            //                        return await IocManager.Instance.Resolve<CheckICNoStrategy>().Verify(item[i].IcBarcode, terminal);
            //                    }
            //                    else if (item[i].FingerType == FingerType.SecondTicket)
            //                    {
            //                        return await IocManager.Instance.Resolve<CheckCacheStrategy>().Verify(item[i].IcBarcode, terminal);
            //                    }
            //                    else
            //                    {
            //                        return await IocManager.Instance.Resolve<CheckBarcodeStrategy>().Verify(item[i].IcBarcode, terminal);
            //                    }
            //                }
            //            }
            //            if (!string.IsNullOrWhiteSpace(verifyFingerDto.VerifyCode))
            //            {
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return Failed("", VerifyType.InvalidFinger, "指纹无效");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return Failed("", VerifyType.InvalidFinger, ex.Message);
            //}

            //checkData.VerifyCode = verifyFingerDto.VerifyCode;
            //checkData.FingerDto = verifyFingerDto;

            //// 返回指纹验证结果
            //return Success(checkData);


            return Failed("", VerifyType.InvalidFinger, "指纹无效");
        }

        /// <summary>
        /// TXW指纹验证第三方类
        /// </summary>
        /// <param name="pFeature1"></param>
        /// <param name="pFeature2"></param>
        /// <param name="uRotate"></param>
        /// <param name="uLevel"></param>
        /// <returns></returns>

        [DllImport("D5Scanner.dll", EntryPoint = "D5Match")]

        public static extern int D5Match(byte[] pFeature1, byte[] pFeature2, ushort uRotate, ushort uLevel);
    }
}
