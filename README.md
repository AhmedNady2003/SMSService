# SMSService
# DeltaCore.SMSService

## Overview 📚

**DeltaCore.SMSService** is a lightweight .NET 8 library that simplifies sending and verifying OTPs via SMS or call using **Twilio Verify API**, and sending custom messages using **Twilio Messaging API**.  
It also includes a simple throttling mechanism to prevent sending OTPs within 60 seconds to the same number.

✅ Send OTPs via SMS or call  
✅ Verify OTPs easily  
✅ Send custom text messages  
✅ Prevent spamming (resending OTP within 60 seconds)  
✅ Built-in error handling and logging

---

## Install from NuGet 📦

You can install the package from [NuGet](https://www.nuget.org/):

```bash
dotnet add package DeltaCore.SMSService
```
or via the Package Manager Console:
```bash
Install-Package DeltaCore.SMSService
```

---
## Using in Your App 🚀
- **Configure TwilioSettings**
In your `appsettings.json`, add:
```json
{
  "TwilioSettings": {
    "AccountSID": "your_account_sid",
    "AuthToken": "your_auth_token",
    "TwilioPhoneNumber": "your_twilio_phone_number",
    "VerifyServiceSID": "your_verify_service_sid"
  }
}
```
- **Register the Service in `Startup.cs` or `Program.cs`**
```csharp
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
builder.Services.AddSingleton<ISMSService, TwilioService>();
```
- **Using `ISMSService` in Your Application**
Inject ISMSService where needed, for example in a controller or service:
```csharp
public class AccountController : ControllerBase
{
    private readonly ISMSService _smsService;

    public AccountController(ISMSService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(string phoneNumber)
    {
        var verificationSid = await _smsService.SendOTPAsync(phoneNumber);
        if (verificationSid == null)
            return BadRequest("OTP was sent recently. Please wait.");
        
        return Ok(new { verificationSid });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(string phoneNumber, string code)
    {
        var isValid = await _smsService.VerifyOTPAsync(phoneNumber, code);
        if (!isValid)
            return BadRequest("Invalid or expired OTP.");

        return Ok("OTP verified successfully.");
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage(string phoneNumber, string message)
    {
        var isSent = await _smsService.SendMessageAsync(phoneNumber, message);
        if (!isSent)
            return BadRequest("Failed to send message.");

        return Ok("Message sent successfully.");
    }
}
```
---
## Important Notes 📝
- OTP requests are rate-limited: the same number cannot request a new OTP within 60 seconds.

- Errors during sending/verification are logged using ILogger.

- Supports both SMS and voice calls when sending OTPs by specifying the channel parameter.

---
## License 🛡️
This project is licensed under the MIT License.

---
## Support
If you encounter any issues or have questions about using DeltaCore.SMSService, feel free to create an issue on GitHub or reach out to us.

---
## Author


👤 **Ahmed Nady**

[![GitHub](https://img.shields.io/badge/GitHub-000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/AhmedNady2003) [![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ahmed-nady-386383266/) [![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:ahmednady122003@gmail.com)
