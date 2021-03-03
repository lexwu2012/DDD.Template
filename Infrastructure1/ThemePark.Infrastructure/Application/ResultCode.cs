using System.ComponentModel.DataAnnotations;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// Api����״̬��
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// �����ɹ�
        ///</summary>
        [Display(Name = "�����ɹ�", GroupName = Result.SuccessCode)]
        Ok = 0,

        /// <summary>
        /// ����ʧ��
        ///</summary>
        [Display(Name = "����ʧ��")]
        Fail = 1,

        /// <summary>
        /// ���������쳣
        ///</summary>
        [Display(Name = "���������쳣")]
        ServerError = 10,

        /// <summary>
        /// δ��¼
        ///</summary>
        [Display(Name = "δ��¼")]
        Unauthorized = 20,

        /// <summary>
        /// δ��Ȩ
        /// </summary>
        [Display(Name = "δ��Ȩ")]
        Forbidden = 21,

        /// <summary>
        /// Token ʧЧ
        /// </summary>
        [Display(Name = "Token ʧЧ")]
        InvalidToken = 22,

        /// <summary>
        /// ������֤ʧ��
        /// </summary>
        [Display(Name = "������֤ʧ��")]
        SpaFailed = 23,

        /// <summary>
        /// �����������
        /// </summary>
        [Display(Name = "�����������")]
        WrongNewPassword = 24,

        /// <summary>
        /// ǩ����֤ʧ��
        /// </summary>
        [Display(Name = "ǩ����֤ʧ��")]
        InvalidSign = 402,

        /// <summary>
        /// ������֤ʧ��
        /// </summary>
        [Display(Name = "������֤ʧ��")]
        InvalidData = 403,

        /// <summary>
        /// û�д�����¼
        ///</summary>
        [Display(Name = "û�д�����¼")]
        NoRecord = 404,

        /// <summary>
        /// �ظ���¼
        /// </summary>
        [Display(Name = "���м�¼�������ظ�����")]
        DuplicateRecord = 405,

        /// <summary>
        /// ȱʧ��������
        /// </summary>
        [Display(Name = "ȱʧ��������")]
        MissEssentialData = 406,

        /// <summary>
        /// �����֤ʧ��
        /// </summary>
        [Display(Name = "�����֤ʧ��")]
        InvalidAmount = 407,

        /// <summary>
        /// ȱ�ٶ�ӦƱ��
        /// </summary>
        [Display(Name = "ȱ�ٶ�ӦƱ��")]
        MissTicketType = 408,

        /// <summary>
        /// ֧��ʧ��
        /// </summary>
        [Display(Name = "֧��ʧ��")]
        PayFail = 500,

        /// <summary>
        /// д���Ʊ��¼ʧ��
        /// </summary>
        [Display(Name = "д���Ʊ��¼ʧ��")]
        WriteTicketRecordFail = 501,

        /// <summary>
        /// д���꿨ƾ֤��¼ʧ��
        /// </summary>
        [Display(Name = "д���꿨ƾ֤��¼ʧ��")]
        WriteVoucherRecordFail = 502,


        /// <summary>
        /// �����ظ���Ʊ�Ż�Ʊ��Ϊ����
        /// </summary>
        [Display(Name = "�����ظ���Ʊ�Ż�Ʊ��Ϊ����")]
        DuplicateInvoiceRecord = 503,

        /// <summary>
        /// Ԥ��������Ѿ�����������ƽ��
        /// </summary>
        [Display(Name = "Ԥ��������Ѿ�����������ƽ��")]
        InsufficientBalance = 504,


        #region ������Ʊ���ӿ�״̬�� 1000~2000

        /// <summary>
        /// ���������ڻ��ѹ���
        /// </summary>
        [Display(Name = "���������ڻ��ѹ���")]
        VendorOrderNoExists = 1000,

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "������ȡƱ")]
        VendorOrderConsumed = 1001,


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "��Ʊ����")]
        VendorTicketNotEnough = 1002,



        #endregion

        #region ����ƽ̨�ӿ�״̬�� 10000~19999



        /// <summary>
        /// ����������
        /// </summary>
        [Display(Name = "����������")]
        OrderidNoExists = 7 + 10000,

        /// <summary>
        /// ���ط��������ṩ��ѯ
        /// </summary>
        [Display(Name = "���ط��������ṩ��ѯ")]
        ParkServiceNotProvide = 8 + 10000,

        /// <summary>
        /// ���ط��������ñ���
        /// </summary>
        [Display(Name = "���ط��������ñ���")]
        ParkServiceError = 9 + 10000,

        /// <summary>
        /// �û�ip��������Χ
        /// </summary>
        [Display(Name = "�û�ip��������Χ")]
        UserIpNotAllow = 10 + 10000,

        /// <summary>
        /// ���֤��֤�ɹ�
        /// </summary>
        [Display(Name = "���֤��֤�ɹ�", GroupName = Result.SuccessCode)]
        IdnumCheckSuccess = 11 + 10000,

        /// <summary>
        /// Ԥ���ɹ����ȴ�֧��ȷ��
        /// </summary>
        [Display(Name = "Ԥ���ɹ����ȴ�֧��ȷ��", GroupName = Result.SuccessCode)]
        OrderBookSuccess = 99 + 10000,

        /// <summary>
        /// ��Ʊ�ɹ�
        /// </summary>
        [Display(Name = "��Ʊ�ɹ�", GroupName = Result.SuccessCode)]
        OrderSuccess = 100 + 10000,

        /// <summary>
        /// ����Ʊ�����쳣
        /// </summary>
        [Display(Name = "����Ʊ�����쳣")]
        TicketDataError = 101 + 10000,

        /// <summary>
        /// ������������
        /// </summary>
        [Display(Name = "������������")]
        OrderPlanDataError = 102 + 10000,

        /// <summary>
        /// ��԰������
        /// </summary>
        [Display(Name = "��԰������")]
        ParkNoExist = 103 + 10000,

        /// <summary>
        /// �����Ѿ�����
        /// </summary>
        [Display(Name = "�����Ѿ�����")]
        OrderAlreadyExist = 104 + 10000,

        /// <summary>
        /// ȡƱ���Ѿ���ʹ��
        /// </summary>
        [Display(Name = "ȡƱ���Ѿ���ʹ��")]
        PrepaymentLimit = 105 + 10000,

        /// <summary>
        /// ���������֤��Ʊ������������
        /// </summary>
        [Display(Name = "���������֤��Ʊ������������")]
        OrderIdnumUseLimit = 106 + 10000,

        /// <summary>
        /// ������Ʊ����Ч
        /// </summary>
        [Display(Name = "������Ʊ����Ч")]
        OrderTicketTypeIdError = 107 + 10000,

        /// <summary>
        /// ���������֤�Ѷ���Ʊ
        /// </summary>
        [Display(Name = "���������֤�Ѷ���Ʊ")]
        OrderIdnumUseAlready = 108 + 10000,

        /// <summary>
        /// ���֤��ʽ����
        /// </summary>
        [Display(Name = "���֤��ʽ����")]
        IdnumError = 109 + 10000,

        /// <summary>
        /// ����Ʊ�۴���
        /// </summary>
        [Display(Name = "����Ʊ�۴���")]
        OrderTicketPriceError = 110 + 10000,

        /// <summary>
        /// �����ܼ۴���
        /// </summary>
        [Display(Name = "�����۸����")]
        OrderTotalPriceError = 111 + 10000,

        /// <summary>
        /// ������ֻ����һ�����֤
        /// </summary>
        [Display(Name = "������ֻ����һ�����֤")]
        OrderIdnumLimit = 112 + 10000,

        /// <summary>
        /// �������ݲ�����Ҫ��
        /// </summary>
        [Display(Name = "�������ݲ�����Ҫ��")]
        OrderTicketDataError = 113 + 10000,

        /// <summary>
        /// ������֧��
        /// </summary>
        [Display(Name = "������֧��")]
        OrderTicketPayed = 114 + 10000,

        /// <summary>
        /// ������ȡ��
        /// </summary>
        [Display(Name = "������ȡ��")]
        OrderAlreadyCancel = 201 + 10000,

        /// <summary>
        /// ����ȡ��ʧ��
        /// </summary>
        [Display(Name = "����ȡ��ʧ��")]
        OrderCancelFail = 202 + 10000,

        /// <summary>
        /// δ֧��������������Ʊ
        /// </summary>
        [Display(Name = "δ֧��������������Ʊ")]
        OrderCancelFailForNoPay = 203 + 10000,

        /// <summary>
        /// ����ȡ���ɹ�
        /// </summary>
        [Display(Name = "����ȡ���ɹ�")]
        OrderCancelSuccess = 204 + 10000,

        /// <summary>
        /// ����ȡ��ʧ�ܣ������Ų�����
        /// </summary>
        [Display(Name = "����ȡ��ʧ�ܣ������Ų�����")]
        OrderCancelFailOrderidNoExist = 205 + 10000,

        /// <summary>
        /// ����ȡ��ʧ�ܣ������Ų�����
        /// </summary>
        [Display(Name = "����ȡ��ʧ�ܣ������Ų�����")]
        OrderCancelFailParkOrderidNoExist = 206 + 10000,

        /// <summary>
        /// ����ȡ��ʧ�ܣ�������ʹ��
        /// </summary>
        [Display(Name = "����ȡ��ʧ�ܣ�������ʹ��")]
        OrderCancelFailAlreadyUsed = 207 + 10000,

        /// <summary>
        /// ����ȡ��ʧ�ܣ�������ʹ��
        /// </summary>
        [Display(Name = "����ȡ��ʧ�ܣ������Ѷ���")]
        OrderCancelFailFrozen = 209 + 10000,

        /// <summary>
        /// �����Ѿ�ȡ�����������޸�
        /// </summary>
        [Display(Name = "�����Ѿ�ȡ�����������޸�")]
        OrderUpdateFailAlreadyCancel = 301 + 10000,

        /// <summary>
        /// �����Ѿ�ʹ�ã��������޸�
        /// </summary>
        [Display(Name = "�����Ѿ�ʹ�ã��������޸�")]
        OrderUpdateFailAlreadyUsed = 302 + 10000,

        /// <summary>
        /// �������ܰ
        /// </summary>
        [Display(Name = "�������ܰ")]
        TicketSaleOut = 303 + 10000,

        #endregion



    }
}