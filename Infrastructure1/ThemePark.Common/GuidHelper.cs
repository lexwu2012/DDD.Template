using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Common
{
    public class GuidHelper
    {
        public Guid CreateFromUlong(ulong num)
        {
            var a1 = UlongToByte(num);

            byte[] array = new byte[16];

            a1.CopyTo(array, array.Length - a1.Length);

            return new Guid(array);
        }

        private byte[] UlongToByte(ulong ul)
        {
            byte[] aOut = new byte[8];
            aOut[0] = (byte)((ul >> 56) & 0xff);
            aOut[1] = (byte)((ul >> 48) & 0xff);
            aOut[2] = (byte)((ul >> 40) & 0xff);
            aOut[3] = (byte)((ul >> 32) & 0xff);
            aOut[4] = (byte)((ul >> 24) & 0xff);
            aOut[5] = (byte)((ul >> 16) & 0xff);
            aOut[6] = (byte)((ul >> 8) & 0xff);
            aOut[7] = (byte)(ul & 0xff);
            return aOut;
        }
    }
}
