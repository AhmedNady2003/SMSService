using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaCore.SMSService
{
    public class TwilioService : ISMSService
    {
        private readonly TwilioSettings _twilio;
        private readonly ILogger<TwilioService> _logger;
        private readonly Dictionary<string, DateTime> _otpSentTimes = new Dictionary<string, DateTime>();

        public TwilioService(IOptions<TwilioSettings> twilio, ILogger<TwilioService> logger)
        {
            _twilio = twilio.Value;
            _logger = logger;
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
        }

        public async Task<string?> SendOTPAsync(string phoneNumber, string channel = "sms", CancellationToken cancellationToken = default)
        {
            try
            {
                if (_otpSentTimes.ContainsKey(phoneNumber))
                {
                    var lastSentTime = _otpSentTimes[phoneNumber];
                    if (DateTime.UtcNow - lastSentTime < TimeSpan.FromSeconds(60))
                    {
                        _logger.LogWarning("[Twilio Warning] OTP already sent recently.");
                        return null; 
                    }
                }

                var verification = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: channel, // "sms" or "call"
                    pathServiceSid: _twilio.VerifyServiceSID
                );

                _otpSentTimes[phoneNumber] = DateTime.UtcNow;

                return verification.Sid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Twilio Error] Failed to send OTP.");
                return null;
            }
        }

        public async Task<bool> VerifyOTPAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
        {
            try
            {
                var verificationCheck = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: _twilio.VerifyServiceSID
                );

                return verificationCheck.Status == "approved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Twilio Error] OTP verification failed.");
                return false;
            }
        }

        public async Task<string?> GetVerificationStatusAsync(string verificationSid, CancellationToken cancellationToken = default)
        {
            try
            {
                var verification = await VerificationResource.FetchAsync(
                    pathServiceSid: _twilio.VerifyServiceSID,
                    pathSid: verificationSid
                );

                return verification.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Twilio Error] Failed to fetch verification status.");
                return null;
            }
        }

        public async Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                return messageResource.ErrorCode == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Twilio Error] Failed to send message.");
                return false;
            }
        }
    }
}
