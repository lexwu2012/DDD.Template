using System;
using System.ServiceModel.Configuration;

namespace ThemePark.Infrastructure.Services
{
    public class SessionPropagationBehaviorElement : BehaviorExtensionElement
    {
        /// <summary>基于当前配置设置来创建行为扩展。</summary>
        /// <returns>行为扩展。</returns>
        protected override object CreateBehavior()
        {
            return new SessionPropagationBehavior();
        }

        /// <summary>获取行为的类型。</summary>
        /// <returns>行为特质。</returns>
        public override Type BehaviorType => typeof(SessionPropagationBehavior);
    }
}
