using System;
using Abp.Dependency;

namespace ThemePark.Infrastructure.Security.DataProtection
{
    /// <summary>
    /// 实现 <see cref="ITimeLimitedDataProtectionProvider"/>
    /// </summary>
    public class TimeLimitedDataProtectionProvider : ITimeLimitedDataProtectionProvider, ITransientDependency
    {
        private readonly IDataProtectionProvider _innerProtectionProvider;
        private const string TimeLimitedPurposeString = "TimeLimitedDataProtectionProvider.TimeLimitedDataProtector.v1";


        /// <summary>
        /// 创建一个新的 <see cref="TimeLimitedDataProtectionProvider"/>
        /// </summary>
        /// <param name="innerProtectionProvider"></param>
        public TimeLimitedDataProtectionProvider(IDataProtectionProvider innerProtectionProvider)
        {
            _innerProtectionProvider = innerProtectionProvider;
        }

        /// <summary>
        /// 创建一个数据保护器
        /// </summary>
        /// <param name="purposes">保护数据增加的额外的标识</param>
        public ITimeLimitedDataProtector Create(params string[] purposes)
        {
            var overwitePurposes = ConcatPurposes(purposes, TimeLimitedPurposeString);
            var protector = _innerProtectionProvider.Create(overwitePurposes);
            return new TimeLimitedDataProtector(protector);
        }

        private static string[] ConcatPurposes(string[] originalPurposes, string newPurpose)
        {
            if (originalPurposes != null && originalPurposes.Length > 0)
            {
                var newPurposes = new string[originalPurposes.Length + 1];
                Array.Copy(originalPurposes, 0, newPurposes, 0, originalPurposes.Length);
                newPurposes[originalPurposes.Length] = newPurpose;
                return newPurposes;
            }
            else
            {
                return new[] { newPurpose };
            }
        }
    }
}