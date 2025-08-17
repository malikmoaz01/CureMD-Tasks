// using Microsoft.AspNetCore.Mvc;
// using webapi.Models;
// using webapi.Services;

// namespace webapi.Controllers
// {
    
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ForgotPasswordController : ControllerBase
//     {
//         private readonly IForgotPasswordService _forgotPasswordService;

//         public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
//         {
//             _forgotPasswordService = forgotPasswordService;
//         }

//         [HttpGet]
// public IActionResult Index()
// {
//     return Ok("ForgotPassword API is running.");
// }

//         [HttpPost("send-otp")]
//         public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordRequestDto request)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(new { 
//                     success = false, 
//                     message = "Invalid input", 
//                     errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
//                 });
//             }

//             var result = await _forgotPasswordService.SendOtpAsync(request);
            
//             return Ok(new { 
//                 success = result.Success, 
//                 message = result.Message 
//             });
//         }

//         [HttpPost("verify-otp")]
//         public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(new { 
//                     success = false, 
//                     message = "Invalid input", 
//                     errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
//                 });
//             }

//             var result = await _forgotPasswordService.VerifyOtpAsync(request);
            
//             return Ok(new { 
//                 success = result.Success, 
//                 message = result.Message 
//             });
//         }

//         [HttpPost("reset-password")]
//         public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(new { 
//                     success = false, 
//                     message = "Invalid input", 
//                     errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
//                 });
//             }

//             var result = await _forgotPasswordService.ResetPasswordAsync(request);
            
//             return Ok(new { 
//                 success = result.Success, 
//                 message = result.Message 
//             });
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services;
using FluentValidation;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
        private readonly IValidator<VerifyOtpDto> _verifyOtpValidator;
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;

        public ForgotPasswordController(IForgotPasswordService forgotPasswordService, 
            IValidator<ForgotPasswordRequestDto> forgotPasswordValidator,
            IValidator<VerifyOtpDto> verifyOtpValidator,
            IValidator<ResetPasswordDto> resetPasswordValidator)
        {
            _forgotPasswordService = forgotPasswordService;
            _forgotPasswordValidator = forgotPasswordValidator;
            _verifyOtpValidator = verifyOtpValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("ForgotPassword API is running.");
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordRequestDto request)
        {
            var validationResult = await _forgotPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _forgotPasswordService.SendOtpAsync(request);
            
            return Ok(ApiResponse<object>.SuccessResult(null, result.Message));
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
        {
            var validationResult = await _verifyOtpValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _forgotPasswordService.VerifyOtpAsync(request);
            
            return Ok(ApiResponse<object>.SuccessResult(null, result.Message));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _forgotPasswordService.ResetPasswordAsync(request);
            
            return Ok(ApiResponse<object>.SuccessResult(null, result.Message));
        }
    }
}