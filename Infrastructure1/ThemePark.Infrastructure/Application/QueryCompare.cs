using System.ComponentModel.DataAnnotations;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// ��ѯ�ȽϷ�ʽ
    /// </summary>
    public enum QueryCompare
    {
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        Equal,

        /// <summary>
        /// ������
        /// </summary>
        [Display(Name = "������")]
        NotEqual,

        /// <summary>
        /// ģ��ƥ��
        /// </summary>
        [Display(Name = "ģ��ƥ��")]
        Like,

        /// <summary>
        /// ������ģ��ƥ��
        /// </summary>
        [Display(Name = "������ģ��ƥ��")]
        NotLike,

        /// <summary>
        /// ��...��ͷ
        /// </summary>
        [Display(Name = "��...��ͷ")]
        StartWidth,

        /// <summary>
        /// С��
        /// </summary>
        [Display(Name = "С��")]
        LessThan,

        /// <summary>
        /// С�ڵ���
        /// </summary>
        [Display(Name = "С�ڵ���")]
        LessThanOrEqual,

        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        GreaterThan,

        /// <summary>
        /// ���ڵ���
        /// </summary>
        [Display(Name = "���ڵ���")]
        GreaterThanOrEqual,

        /// <summary>
        /// ��...֮�䣬���Ա�����һ�����ϣ��򶺺ŷָ����ַ�������ȡ��һ�����һ��ֵ��
        /// </summary>
        [Display(Name = "��...֮��")]
        Between,

        /// <summary>
        /// ���ڵ�����ʼ��С�ڽ��������Ա�����һ�����ϣ��򶺺ŷָ����ַ�������ȡ��һ�����һ��ֵ��
        /// </summary>
        [Display(Name = "���ڵ�����ʼ��С�ڽ���")]
        GreaterEqualAndLess,

        /// <summary>
        /// ���������Ա�����һ�����ϣ��򶺺ŷָ����ַ�����
        /// </summary>
        [Display(Name = "����")]
        Include,

        /// <summary>
        /// �����������Ա�����һ�����ϣ��򶺺ŷָ����ַ�����
        /// </summary>
        [Display(Name = "������")]
        NotInclude,

        /// <summary>
        /// Ϊ�ջ�Ϊ�գ�����Ϊ bool���ͣ���ɿ����͡�
        /// </summary>
        [Display(Name = "Ϊ�ջ�Ϊ��")]
        IsNull,

        /// <summary>
        /// �Ƿ����ָ��ö��
        /// </summary>
        [Display(Name = "�Ƿ����ָ��ö��")]
        HasFlag,
    }
}