namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// ���ؽ��
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// ���״̬��
        /// </summary>
        ResultCode Code { get; set; }

        /// <summary>
        /// ��ʾ��Ϣ
        /// </summary>
        /// <example>�����ɹ�</example>
        string Message { get; set; }

        /// <summary>
        /// �Ƿ�ɹ�
        /// </summary>
        bool Success { get; }
    }

    /// <summary>
    /// ���ؽ��
    /// </summary>
    public interface IResult<TData> : IResult
    {
        /// <summary>
        /// ���״̬��
        /// </summary>
        TData Data { get; set; }
    }
}