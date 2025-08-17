// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using webapi.Models;
// using webapi.Repositories;

// namespace webapi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     [Authorize]
//     public class FeeRateController : ControllerBase
//     {
//         private readonly IFeeRateRepository _feeRateRepository;

//         public FeeRateController(IFeeRateRepository feeRateRepository)
//         {
//             _feeRateRepository = feeRateRepository;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAllFeeRates()
//         {
//             try
//             {
//                 var feeRates = await _feeRateRepository.GetAllAsync();
//                 return Ok(ApiResponse<IEnumerable<FeeRate>>.SuccessResult(feeRates, "Fee rates retrieved successfully."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving fee rates."));
//             }
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetFeeRateById(int id)
//         {
//             try
//             {
//                 if (id <= 0)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
//                 }

//                 var feeRate = await _feeRateRepository.GetByIdAsync(id);
                
//                 if (feeRate == null)
//                 {
//                     return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
//                 }

//                 return Ok(ApiResponse<FeeRate>.SuccessResult(feeRate, "Fee rate retrieved successfully."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving fee rate."));
//             }
//         }

//         [HttpPost]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> CreateFeeRate([FromBody] FeeRate feeRate)
//         {
//             try
//             {
//                 if (!ModelState.IsValid)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
//                 }

//                 var feeRateId = await _feeRateRepository.AddAsync(feeRate);
                
//                 if (feeRateId > 0)
//                 {
//                     var createdFeeRate = await _feeRateRepository.GetByIdAsync(feeRateId);
//                     return CreatedAtAction(nameof(GetFeeRateById), new { id = feeRateId }, 
//                         ApiResponse<FeeRate>.SuccessResult(createdFeeRate, "Fee rate created successfully."));
//                 }

//                 return BadRequest(ApiResponse<object>.ErrorResult("Failed to create fee rate."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating fee rate."));
//             }
//         }

//         [HttpPut("{id}")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> UpdateFeeRate(int id, [FromBody] FeeRate feeRate)
//         {
//             try
//             {
//                 if (id <= 0 || feeRate.FeeRateId != id)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
//                 }

//                 if (!ModelState.IsValid)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid input data."));
//                 }

//                 var existingFeeRate = await _feeRateRepository.GetByIdAsync(id);
//                 if (existingFeeRate == null)
//                 {
//                     return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
//                 }

//                 var success = await _feeRateRepository.UpdateAsync(feeRate);
                
//                 if (success)
//                 {
//                     var updatedFeeRate = await _feeRateRepository.GetByIdAsync(id);
//                     return Ok(ApiResponse<FeeRate>.SuccessResult(updatedFeeRate, "Fee rate updated successfully."));
//                 }

//                 return BadRequest(ApiResponse<object>.ErrorResult("Failed to update fee rate."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating fee rate."));
//             }
//         }

//         [HttpDelete("{id}")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> DeleteFeeRate(int id)
//         {
//             try
//             {
//                 if (id <= 0)
//                 {
//                     return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
//                 }

//                 var existingFeeRate = await _feeRateRepository.GetByIdAsync(id);
//                 if (existingFeeRate == null)
//                 {
//                     return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
//                 }

//                 var success = await _feeRateRepository.DeleteAsync(id);
                
//                 if (success)
//                 {
//                     return Ok(ApiResponse<object>.SuccessResult(null, "Fee rate deleted successfully."));
//                 }

//                 return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete fee rate."));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting fee rate."));
//             }
//         }
//     }
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;
using FluentValidation;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeeRateController : ControllerBase
    {
        private readonly IFeeRateRepository _feeRateRepository;
        private readonly IValidator<FeeRate> _feeRateValidator;

        public FeeRateController(IFeeRateRepository feeRateRepository, IValidator<FeeRate> feeRateValidator)
        {
            _feeRateRepository = feeRateRepository;
            _feeRateValidator = feeRateValidator;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetAllFeeRates()
        {
            try
            {
                var feeRates = await _feeRateRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<FeeRate>>.SuccessResult(feeRates, "Fee rates retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving fee rates."));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> GetFeeRateById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
                }

                var feeRate = await _feeRateRepository.GetByIdAsync(id);
                
                if (feeRate == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
                }

                return Ok(ApiResponse<FeeRate>.SuccessResult(feeRate, "Fee rate retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving fee rate."));
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrReceptionist")]
        public async Task<IActionResult> CreateFeeRate([FromBody] FeeRate feeRate)
        {
            try
            {
                var validationResult = await _feeRateValidator.ValidateAsync(feeRate);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var feeRateId = await _feeRateRepository.AddAsync(feeRate);
                
                if (feeRateId > 0)
                {
                    var createdFeeRate = await _feeRateRepository.GetByIdAsync(feeRateId);
                    return CreatedAtAction(nameof(GetFeeRateById), new { id = feeRateId }, 
                        ApiResponse<FeeRate>.SuccessResult(createdFeeRate, "Fee rate created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create fee rate."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating fee rate."));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateFeeRate(int id, [FromBody] FeeRate feeRate)
        {
            try
            {
                if (id <= 0 || feeRate.FeeRateId != id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
                }

                var validationResult = await _feeRateValidator.ValidateAsync(feeRate);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
                }

                var existingFeeRate = await _feeRateRepository.GetByIdAsync(id);
                if (existingFeeRate == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
                }

                var success = await _feeRateRepository.UpdateAsync(feeRate);
                
                if (success)
                {
                    var updatedFeeRate = await _feeRateRepository.GetByIdAsync(id);
                    return Ok(ApiResponse<FeeRate>.SuccessResult(updatedFeeRate, "Fee rate updated successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to update fee rate."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating fee rate."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteFeeRate(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid fee rate ID."));
                }

                var existingFeeRate = await _feeRateRepository.GetByIdAsync(id);
                if (existingFeeRate == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Fee rate not found."));
                }

                var success = await _feeRateRepository.DeleteAsync(id);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Fee rate deleted successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete fee rate."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Error deleting fee rate."));
            }
        }
    }
}
