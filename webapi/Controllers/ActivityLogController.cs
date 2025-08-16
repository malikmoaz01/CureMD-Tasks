using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Repositories;
using System.Security.Claims;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogController(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivityLogs()
        {
            try
            {
                var activityLogs = await _activityLogRepository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ActivityLog>>.SuccessResult(activityLogs, "Activity logs retrieved successfully."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllActivityLogs: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error retrieving activity logs: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivityLogById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid activity log ID."));
                }

                var activityLog = await _activityLogRepository.GetByIdAsync(id);
                
                if (activityLog == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Activity log not found."));
                }

                return Ok(ApiResponse<ActivityLog>.SuccessResult(activityLog, "Activity log retrieved successfully."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetActivityLogById: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error retrieving activity log: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivityLog([FromBody] ActivityLog activityLog)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(activityLog?.Action))
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Action is required."));
                }

                if (activityLog.Action.Length > 100)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Action cannot exceed 100 characters."));
                }

                if (!string.IsNullOrEmpty(activityLog.Details) && activityLog.Details.Length > 500)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Details cannot exceed 500 characters."));
                }

                var currentUserId = GetCurrentUserId();
                activityLog.UserId = currentUserId;
                activityLog.LogDateTime = DateTime.UtcNow;

                var activityLogId = await _activityLogRepository.AddAsync(activityLog);
                
                if (activityLogId > 0)
                {
                    var createdActivityLog = await _activityLogRepository.GetByIdAsync(activityLogId);
                    return CreatedAtAction(nameof(GetActivityLogById), new { id = activityLogId }, 
                        ApiResponse<ActivityLog>.SuccessResult(createdActivityLog, "Activity log created successfully."));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to create activity log."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateActivityLog: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error creating activity log: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")] // Temporarily remove to test
        public async Task<IActionResult> DeleteActivityLog(int id)
        {
            try
            {
                Console.WriteLine($"Attempting to delete activity log with ID: {id}");
                
                if (id <= 0)
                {
                    Console.WriteLine("Invalid ID provided");
                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid activity log ID."));
                }

                var existingActivityLog = await _activityLogRepository.GetByIdAsync(id);
                Console.WriteLine($"Existing activity log found: {existingActivityLog != null}");
                
                if (existingActivityLog == null)
                {
                    Console.WriteLine("Activity log not found");
                    return NotFound(ApiResponse<object>.ErrorResult("Activity log not found."));
                }

                var success = await _activityLogRepository.DeleteAsync(id);
                Console.WriteLine($"Delete operation result: {success}");
                
                if (success)
                {
                    Console.WriteLine("Delete successful");
                    return Ok(ApiResponse<object>.SuccessResult(null, "Activity log deleted successfully."));
                }
                else
                {
                    Console.WriteLine("Delete failed - repository returned false");
                    return StatusCode(500, ApiResponse<object>.ErrorResult("Failed to delete activity log - no rows affected."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteActivityLog: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error deleting activity log: {ex.Message}"));
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}