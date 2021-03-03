using D5ScannerFPCClassLibrary;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// TXW指纹类
    /// </summary>
    public class TxwFPClass
    {

        /// <summary>
        /// 检测某图像是否有指纹
        /// </summary>
        /// <param name="byte1"></param>
        /// <returns></returns>
        public static bool CheckFP(byte[] byte1)
        {
            /*
                说明：
                    检测某图像是否有指纹
                参数
                    pImage			图像数据
                                    宽度152, 高度200, 数据按行顺序排列, 每个像素用1字节表示灰度		
                返回：
                    1:				检测到指纹
                    0:				无指纹
            */
            D5ScannerFPCClass d5ScannerFpc = new D5ScannerFPCClass();
            return d5ScannerFpc.CheckFP(byte1);
        }


        /// <summary>
        /// TXW指纹比对
        /// </summary>
        /// <param name="byte1">数据库指纹</param>
        /// <param name="byte2">闸机指纹</param>
        /// <returns></returns>
        public static bool TwxCompareByte(byte[] byte1, byte[] byte2)
        {
            /*
                说明：
                    对输入的两个指纹特征值进行比对
                参数
                    pFeature1:		指纹特征值1
                    pFeature2:		指纹特征值2
                    uRotate:		旋转角度(1-180)
                    uLevel:			匹配等级(0-9)
                返回：
                    0:				匹配成功
                    -1:				匹配失败
                    -2:				系统错误
            */
            D5ScannerFPCClass d5ScannerFpc = new D5ScannerFPCClass();
            if (d5ScannerFpc.Match(byte1, byte2, 180, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 检查指纹有效性（重复）
        /// </summary>
        /// <param name="regTemplate"></param>
        /// <param name="verTemplate"></param>
        /// <param name="ARegFeatureChanged"></param>
        /// <returns></returns>
        public static bool VerFinger(ref object regTemplate, object verTemplate, ref bool ARegFeatureChanged)
        {
            byte[] regTemp = (byte[])regTemplate;
            byte[] verTemp = (byte[])verTemplate;
            return TwxCompareByte(regTemp, verTemp);
        }

    }
}
