using webapi.Models;
using webapi.Repositories;

namespace webapi.Services
{
    public interface IPatientVisitService
    {
        Task<PatientVisit> GetByIdAsync(int id);
        Task<IEnumerable<PatientVisit>> GetAllAsync();
        Task<PatientVisit> CreateVisitAsync(PatientVisit patientVisit);
        Task<PatientVisit> UpdateVisitAsync(PatientVisit patientVisit);
        Task<bool> DeleteVisitAsync(int id);
        Task<decimal> CalculateFeeAsync(int visitTypeId, int durationInMinutes);
        Task<IEnumerable<PatientVisit>> GetVisitsByPatientIdAsync(int patientId);
        Task<IEnumerable<PatientVisit>> GetVisitsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<PatientVisit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public class PatientVisitService : IPatientVisitService
    {
        private readonly IPatientVisitRepository _patientVisitRepository;
        private readonly IFeeRateRepository _feeRateRepository;
        private readonly IVisitTypeRepository _visitTypeRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public PatientVisitService(
            IPatientVisitRepository patientVisitRepository,
            IFeeRateRepository feeRateRepository,
            IVisitTypeRepository visitTypeRepository,
            IActivityLogRepository activityLogRepository)
        {
            _patientVisitRepository = patientVisitRepository;
            _feeRateRepository = feeRateRepository;
            _visitTypeRepository = visitTypeRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<PatientVisit> GetByIdAsync(int id)
        {
            try
            {
                var visit = await _patientVisitRepository.GetByIdAsync(id);
                
                if (visit != null)
                {
                    await LogActivityAsync("GetVisitById", true, $"Retrieved visit ID: {id}", null, id);
                }

                return visit;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("GetVisitById", false, $"Error retrieving visit ID {id}: {ex.Message}", null, id);
                throw;
            }
        }

        public async Task<IEnumerable<PatientVisit>> GetAllAsync()
        {
            try
            {
                var visits = await _patientVisitRepository.GetAllAsync();
                await LogActivityAsync("GetAllVisits", true, $"Retrieved {visits.Count()} visits", null, null);
                return visits;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("GetAllVisits", false, $"Error retrieving all visits: {ex.Message}", null, null);
                throw;
            }
        }

        public async Task<PatientVisit> CreateVisitAsync(PatientVisit patientVisit)
        {
            try
            { 
                if (patientVisit.Fee == 0)
                {
                    patientVisit.Fee = await CalculateFeeAsync(patientVisit.VisitTypeId, patientVisit.DurationInMinutes);
                }
 
                patientVisit.CreatedDate = DateTime.Now;

                var visitId = await _patientVisitRepository.AddAsync(patientVisit);
                patientVisit.Id = visitId;

                await LogActivityAsync("CreateVisit", true, 
                    $"Created visit for Patient ID: {patientVisit.PatientId}, Fee: {patientVisit.Fee:C}", 
                    patientVisit.CreatedBy, visitId);

                return patientVisit;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("CreateVisit", false, 
                    $"Error creating visit for Patient ID {patientVisit.PatientId}: {ex.Message}", 
                    patientVisit.CreatedBy, null);
                throw;
            }
        }

        public async Task<PatientVisit> UpdateVisitAsync(PatientVisit patientVisit)
        {
            try
            { 
                patientVisit.Fee = await CalculateFeeAsync(patientVisit.VisitTypeId, patientVisit.DurationInMinutes);
                 
                patientVisit.ModifiedDate = DateTime.Now;

                var success = await _patientVisitRepository.UpdateAsync(patientVisit);
                
                if (!success)
                {
                    throw new InvalidOperationException("Visit not found or could not be updated");
                }

                await LogActivityAsync("UpdateVisit", true, 
                    $"Updated visit ID: {patientVisit.Id}, New Fee: {patientVisit.Fee:C}", 
                    patientVisit.ModifiedBy, patientVisit.Id);

                return patientVisit;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("UpdateVisit", false, 
                    $"Error updating visit ID {patientVisit.Id}: {ex.Message}", 
                    patientVisit.ModifiedBy, patientVisit.Id);
                throw;
            }
        }

        public async Task<bool> DeleteVisitAsync(int id)
        {
            try
            {
                var success = await _patientVisitRepository.DeleteAsync(id);
                
                await LogActivityAsync("DeleteVisit", success, 
                    success ? $"Deleted visit ID: {id}" : $"Failed to delete visit ID: {id}", 
                    null, id);

                return success;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("DeleteVisit", false, 
                    $"Error deleting visit ID {id}: {ex.Message}", null, id);
                throw;
            }
        }

        public async Task<decimal> CalculateFeeAsync(int visitTypeId, int durationInMinutes)
        {
            try
            { 
                var feeRates = await _feeRateRepository.GetAllAsync();
                var feeRate = feeRates.FirstOrDefault(fr => fr.VisitTypeId == visitTypeId);

                if (feeRate != null)
                {
                    var baseAmount = feeRate.BaseRate;
                     
                    if (durationInMinutes > feeRate.ExtraTimeThreshold)
                    {
                        var extraMinutes = durationInMinutes - feeRate.ExtraTimeThreshold;
                        var extraFee = baseAmount * (decimal)extraMinutes * feeRate.ExtraTimeRate;
                        return baseAmount + extraFee;
                    }
                    
                    return baseAmount;
                }
 
                var visitType = await _visitTypeRepository.GetByIdAsync(visitTypeId);
                if (visitType != null)
                {
                    var baseAmount = visitType.BaseRate;
                     
                    if (durationInMinutes > 30)
                    {
                        var extraMinutes = durationInMinutes - 30;
                        var extraFee = baseAmount * (decimal)extraMinutes * 0.25m;
                        return baseAmount + extraFee;
                    }
                    
                    return baseAmount;
                }
 
                return 500.00m;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("CalculateFee", false, 
                    $"Error calculating fee for VisitType {visitTypeId}: {ex.Message}", null, null);
                throw;
            }
        }

        public async Task<IEnumerable<PatientVisit>> GetVisitsByPatientIdAsync(int patientId)
        {
            try
            {
                var allVisits = await _patientVisitRepository.GetAllAsync();
                var patientVisits = allVisits.Where(v => v.PatientId == patientId).ToList();
                
                await LogActivityAsync("GetVisitsByPatient", true, 
                    $"Retrieved {patientVisits.Count} visits for Patient ID: {patientId}", null, null);
                
                return patientVisits;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("GetVisitsByPatient", false, 
                    $"Error retrieving visits for Patient ID {patientId}: {ex.Message}", null, null);
                throw;
            }
        }

        public async Task<IEnumerable<PatientVisit>> GetVisitsByDoctorIdAsync(int doctorId)
        {
            try
            {
                var allVisits = await _patientVisitRepository.GetAllAsync();
                var doctorVisits = allVisits.Where(v => v.DoctorId == doctorId).ToList();
                
                await LogActivityAsync("GetVisitsByDoctor", true, 
                    $"Retrieved {doctorVisits.Count} visits for Doctor ID: {doctorId}", null, null);
                
                return doctorVisits;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("GetVisitsByDoctor", false, 
                    $"Error retrieving visits for Doctor ID {doctorId}: {ex.Message}", null, null);
                throw;
            }
        }

        public async Task<IEnumerable<PatientVisit>> GetVisitsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var allVisits = await _patientVisitRepository.GetAllAsync();
                var dateRangeVisits = allVisits.Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate).ToList();
                
                await LogActivityAsync("GetVisitsByDateRange", true, 
                    $"Retrieved {dateRangeVisits.Count} visits between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}", 
                    null, null);
                
                return dateRangeVisits;
            }
            catch (Exception ex)
            {
                await LogActivityAsync("GetVisitsByDateRange", false, 
                    $"Error retrieving visits by date range: {ex.Message}", null, null);
                throw;
            }
        }

        private async Task LogActivityAsync(string action, bool success, string details, int? userId, int? visitId)
        {
            try
            {
                var log = new ActivityLog
                {
                    Action = action,
                    Success = success,
                    Details = details,
                    UserId = userId,
                    VisitId = visitId,
                    LogDateTime = DateTime.Now
                };

                await _activityLogRepository.AddAsync(log);
            }
            catch
            {
                
            }
        }
    }
}