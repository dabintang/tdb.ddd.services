using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading;
using tdb.common;
using tdb.ddd.contracts;

namespace tdb.ddd.webapi
{
    /// <summary>
    /// 动态策略生成器
    /// </summary>
    public class TdbAuthPolicyProvider : IAuthorizationPolicyProvider, ITdbIOCScoped
    {
        /// <summary>
        /// 策略名分隔符
        /// </summary>
        public const string PolicyNameSeparator = "|";

        /// <summary>
        /// 授权选项
        /// </summary>
        private readonly AuthorizationOptions authorizationOptions;

        /// <summary>
        /// 默认策略生成器
        /// </summary>
        private DefaultAuthorizationPolicyProvider DefaultPolicyProvider { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">授权选项</param>
        public TdbAuthPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            this.DefaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            this.authorizationOptions = options.Value;
        }

        private static readonly object _lock = new();
        /// <summary>
        /// 获取策略
        /// </summary>
        /// <param name="policyName">策略名</param>
        /// <returns></returns>
        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName is null)
            {
                throw new TdbException("策略名不能为空");
            }

            //若策略实例已存在，则直接返回
            var policy = await this.DefaultPolicyProvider.GetPolicyAsync(policyName);
            if (policy is not null)
            {
                return policy;
            }

            lock (_lock)
            {
                //若策略实例已存在，则直接返回
                policy = DefaultPolicyProvider.GetPolicyAsync(policyName).Result;
                if (policy is not null)
                {
                    return policy;
                }

                //拆分策略名和参数
                var policyAndParam = policyName.Split(PolicyNameSeparator, StringSplitOptions.TrimEntries);
                if (policyAndParam is not null && policyAndParam.Length >= 1)
                {
                    var policyNamePrefix = policyAndParam[0] ?? "";
                    var param = policyAndParam.Length > 1 ? policyAndParam[1] : "";

                    //动态创建策略
                    var builder = new AuthorizationPolicyBuilder();
                    //添加 Requirement
                    switch (policyNamePrefix)
                    {
                        case TdbAuthClientIPAttribute.PolicyName:
                            builder.AddRequirements(new TdbAuthClientIPRequirement());
                            break;
                        case TdbAuthAuthorityAttribute.PolicyName:
                            builder.AddRequirements(new TdbAuthAuthorityRequirement(Convert.ToInt64(param)));
                            break;
                        case TdbAuthRoleAttribute.PolicyName:
                            builder.AddRequirements(new TdbAuthRoleRequirement(Convert.ToInt64(param)));
                            break;
                        case TdbAuthWhiteListIPAttribute.PolicyName:
                            {
                                //白名单IP，如有多个用英文逗号,隔开（如：127.0.0.1,localhost）
                                var whiteListIP = param.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                                builder.AddRequirements(new TdbAuthWhiteListIPRequirement(whiteListIP));
                            }
                            break;
                        default:
                            throw new TdbException($"不支持此策略：{policyName}");
                    }
                    //创建策略
                    policy = builder.Build();
                    //将策略添加到选项
                    authorizationOptions.AddPolicy(policyName, policy);

                    return policy;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取默认策略
        /// </summary>
        /// <returns></returns>
        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await this.DefaultPolicyProvider.GetDefaultPolicyAsync();
        }

        /// <summary>
        /// 获取回退策略
        /// </summary>
        /// <returns></returns>
        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return await this.DefaultPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}
