using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlackRise.Identity.Persistence.CustomTokenProvider;

public class ResetPasswordTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    public ResetPasswordTokenProvider(IDataProtectionProvider dataProtectionProvider,
        IOptions<ResetPasswordTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {

    }
}

public class ResetPasswordTokenProviderOptions : DataProtectionTokenProviderOptions { }
