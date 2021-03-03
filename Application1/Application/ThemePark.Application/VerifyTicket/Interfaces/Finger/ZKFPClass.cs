
//using ZKFPEngXControl;

namespace ThemePark.Application.VerifyTicket
{
    public class ZKFPClass
    {
        //public ZKFPEngXControl.ZKFPEngXClass zkfp;
        //private int m_nHandle;

        //public int m_EnrollCount
        //{
        //    get
        //    {
        //        return zkfp.EnrollCount;
        //    }
        //    set
        //    {
        //        zkfp.EnrollCount = value;
        //    }
        //}

        //public ZKFPClass()
        //{
        //    zkfp = new ZKFPEngXClass();
        //    zkfp.InitEngine();
        //    zkfp.EnrollCount = 1;
        //    zkfp.Threshold = 8;
        //    zkfp.OneToOneThreshold = 8;
        //}

        //public virtual bool VerFinger(ref object regTemplate, object verTemplate, ref bool ARegFeatureChanged)
        //{
        //    return zkfp.VerFinger(ref regTemplate, verTemplate, false, ref ARegFeatureChanged);
        //}

        //public virtual void BeginEnroll()
        //{
        //    zkfp.BeginEnroll();
        //}

        
        //public virtual void CancelEnroll()
        //{
        //    zkfp.CancelEnroll();
        //}

        //public virtual object GetTemplate()
        //{
        //    return zkfp.GetTemplate();
        //}

        ///// <summary>
        ///// 匹配指纹
        ///// </summary>
        ///// <param name="byteImageData"></param>
        ///// <param name="imageSize"></param>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public virtual int Capture(byte[] byteImageData, int imageSize,  ref object obj)
        //{
        //    char[] charImageData = new char[imageSize / 2];
        //    Buffer.BlockCopy(byteImageData, 0, charImageData, 0, imageSize);
        //    string strImage = new string(charImageData);
        //    return zkfp.ExtractImageFromURU(strImage, imageSize, true, ref obj);
        //}

        //public virtual void RemoveTemplate(int FPID)
        //{
        //    zkfp.RemoveRegTemplateFromFPCacheDB(m_nHandle, FPID);
        //}

        //public virtual void AddTemplate(int FPID, object pRegTemplate)
        //{
        //    zkfp.AddRegTemplateToFPCacheDB(m_nHandle, FPID, pRegTemplate);
        //}

        //public virtual void FreeCache()
        //{
        //    zkfp.FreeFPCacheDB(m_nHandle);
        //}


        //public virtual void CreateCache()
        //{
        //    m_nHandle = zkfp.CreateFPCacheDB();
        //}

        //public virtual bool CheckEnrollCount()
        //{
        //    return zkfp.EnrollIndex == zkfp.EnrollCount;
        //}
    }
}