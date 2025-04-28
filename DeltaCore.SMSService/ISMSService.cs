using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaCore.SMSService
{
    public interface ISMSService
    {
        Task<string?> SendOTPAsync(string phoneNumber, string channel = "sms", CancellationToken cancellationToken = default);
        Task<bool> VerifyOTPAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
        Task<string?> GetVerificationStatusAsync(string verificationSid, CancellationToken cancellationToken = default);
        Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}
