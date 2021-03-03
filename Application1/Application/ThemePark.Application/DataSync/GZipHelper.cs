using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步的GZ压缩帮助类
    /// </summary>
    public static class GZHelper
    {
        /// <summary>
        /// 将JSON对象列表压缩成gz文件
        /// </summary>
        /// <param name="gzFileName">压缩后的文件名</param>
        /// <param name="jsons">JSON对象列表</param>
        public static void Compress(string gzFileName, IList<string> jsons)
        {
            MemoryStream originalStream = new MemoryStream();
            StreamWriter originalStreamWriter = new StreamWriter(originalStream);

            foreach (var json in jsons)
            {
                originalStreamWriter.WriteLine(json);
            }
            originalStreamWriter.Flush();
            originalStream.Seek(0, SeekOrigin.Begin);

            using (FileStream compressedFileStream = File.Create(gzFileName))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                   CompressionMode.Compress))
                {
                    originalStream.CopyTo(compressionStream);
                }
            }
        }

        /// <summary>
        /// 解压压缩的gz文件，返回JSON对象列表
        /// </summary>
        /// <param name="gzFileName">gz文件名</param>
        /// <param name="jsons">返回JSON对象列表</param>
        public static void Decompress(string gzFileName, ref List<string> jsons)
        {
            FileStream originalStream = File.OpenRead(gzFileName);
            using (MemoryStream decompressedFileStream = new MemoryStream())
            {
                using (GZipStream decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }

                decompressedFileStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(decompressedFileStream);
                string json = reader.ReadLine();
                while (!String.IsNullOrEmpty(json))
                {
                    jsons.Add(json);
                    json = reader.ReadLine();                    
                }

            }
        }
    }

}


