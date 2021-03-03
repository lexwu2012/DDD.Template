using System;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// ��Ʊ�Ǽ�Dto
    /// </summary>
    public class MultiTicketEnrollDto
    {
        /// <summary>
        /// ��Դ��԰
        /// </summary>
        public int FromParkid { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// ָ������
        /// </summary>
        public byte[] Finger { get; set; }


        /// <summary>
        /// ��Ƭ����
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// �Ǽ�ͨ����
        /// </summary>
        public int TerminalId { get; set; }

        /// <summary>
        /// ��԰ʱ��
        /// </summary>
        public DateTime EnrollTime { get; set; }
    }
}