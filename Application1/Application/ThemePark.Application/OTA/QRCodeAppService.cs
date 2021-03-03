using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ThoughtWorks.QRCode.Codec;

namespace ThemePark.Application.OTA
{
    public class QRCodeAppService : IQRCodeAppService
    {
        /// <summary>
        /// 生成二维码图片，返回图片路径
        /// </summary>
        /// <param name="code"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Image GenerateQRCodeImg(string code)
        {
            //生成二维码
            var qre = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeScale = 5,
                QRCodeVersion = 0,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L
            };
            //设置编码类型
            //设置尺寸
            //设置版本
            //设置纠错
            //假如白边
            var image = qre.Encode(code).WhiteAll(10);

            return image;
        }

    }


    /// <summary>
    /// 二维码图片ex
    /// </summary>
    public static class ImageEx
    {
        private static string QRCodeImagePath = "~/QRCode/";

        /// <summary>
        /// 新增白边
        /// </summary>
        /// <param name="image"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static Image WhiteAll(this Image image, int margin)
        {
            //获取图片宽高
            var width = image.Width;
            var height = image.Height;
            //获取图片水平和垂直的分辨率
            var dpiX = image.HorizontalResolution;
            var dpiY = image.VerticalResolution;
            //创建一个位图文件
            var bitmapResult = new Bitmap(width + margin * 2, height + margin * 2, PixelFormat.Format24bppRgb);
            //设置位图文件的水平和垂直分辨率  与Img一致
            bitmapResult.SetResolution(dpiX, dpiY);
            //在位图文件上填充一个矩形框
            var grp = Graphics.FromImage(bitmapResult);
            var rec = new Rectangle(0, 0, width + margin * 2, height + margin * 2);
            //定义一个白色的画刷
            var mySolidBrush = new SolidBrush(Color.White);
            //Grp.Clear(Color.White);
            //将矩形框填充为白色
            grp.FillRectangle(mySolidBrush, rec);
            //Grp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //Grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //Grp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //向矩形框内填充Img
            grp.DrawImage(image, margin, margin, rec, GraphicsUnit.Pixel);
            //返回位图文件
            grp.Dispose();
            GC.Collect();
            return bitmapResult;
        }



        /// <summary>
        /// 保存二维码
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string SaveQRCodeImg(this Image image)
        {
            var path = QRCodeImagePath + DateTime.Now.ToString("yyyyMMdd") + "/";
            var physicalPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (System.IO.Directory.Exists(physicalPath) == false)   //如果不存在就创建file文件夹 
                System.IO.Directory.CreateDirectory(physicalPath);
            var imageName = QRCodeImageName(20) + ".jpg";
            image.Save(physicalPath + imageName);
            return path.Replace("~/", "/") + imageName;
        }


        /// <summary>
        /// image 转string
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(this Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                var oldWidth = (float)image.Width;
                var oldHeight = (float)image.Height;
                const float xPoint = 25; // 新的宽度    
                const float yPoint = 25; // 新的高度
                var bmOutput = new Bitmap((int)(oldWidth + 2 * xPoint), (int)(oldHeight + 2 * yPoint));
                var gc = Graphics.FromImage(bmOutput);
                var tbBg = new SolidBrush(Color.White);
                gc.FillRectangle(tbBg, 0, 0, (oldWidth + 2 * xPoint), (oldHeight + 2 * yPoint)); //填充为白色   
                gc.DrawImage(image, xPoint, yPoint, (int)oldWidth, (int)oldHeight);
                bmOutput.Save(ms, format);
                var imageBytes = ms.ToArray();
                // Convert byte[] to Base64 String
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private static readonly System.Security.Cryptography.RNGCryptoServiceProvider Rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            //System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }


        /// <summary>
        /// 生成二维码文件名称
        /// </summary>
        /// <param name="Length"></param>
        /// <param name="chr"></param>
        /// <returns></returns>
        private static string QRCodeImageName(int Length, char[] chr = null)
        {
            char[] constant ={
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            '0','1','2','3','4','5','6','7','8','9'
            };

            if (chr == null)
                chr = constant;
            StringBuilder sb = new StringBuilder();
            Random rd = new Random(GetRandomSeed());
            for (int i = 0; i < Length; i++)
            {
                sb.Append(chr[rd.Next(chr.Length)]);
            }
            return sb.ToString();
        }
    }


}
