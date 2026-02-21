using Microsoft.EntityFrameworkCore;
using psts.web.Data;
using psts.web.Domain.Enums;
using psts.web.Dto;
using Psts.Web.Data;


namespace psts.web.Services
{
    public class TimeServices : ITimeServices
    {
        private readonly PstsDbContext _db;
        private readonly ISettingsService _settings;

        public TimeServices(PstsDbContext db, ISettingsService settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<ServiceResult<Guid>> EnterNewTimeTransaction(string _requestorId, RoleTypes _requestorRole, NewTimeTransactionDto _newTransactionData)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.PstsUserProfiles.AnyAsync(u => u.EmployeeId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<Guid>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid role.");
                }

                if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Employee))
                {
                    return ServiceResult<Guid>.Fail("Insufficient privileges.");
                }

                // Build new ticket
                var newTimeTransaction = new PstsTimeTransactions();
                newTimeTransaction.TransactionId = new Guid();

                // Verify TaskId
                bool validTask = await _db.PstsTaskDefinitions.AnyAsync(u => u.TaskId == _newTransactionData.TaskId);
                if (!validTask)
                {
                    return ServiceResult<Guid>.Fail("Invalid Task Id.");
                }
                newTimeTransaction.TaskId = _newTransactionData.TaskId;

                // Requestor is enterer and already validated above
                newTimeTransaction.EnteredBy = _newTransactionData.EnteredBy;

                // Verify Work completed by
                bool validWorkCompletedBy = await _db.PstsUserProfiles.AnyAsync(u => u.EmployeeId == _newTransactionData.WorkCompletedBy);
                if (!validWorkCompletedBy)
                {
                    return ServiceResult<Guid>.Fail("Invalid completed by Id.");
                }
                newTimeTransaction.WorkCompletedBy = _newTransactionData.WorkCompletedBy;

                // Create entry time stamp
                newTimeTransaction.EnteredTimeStamp = DateTime.UtcNow;

                // Verify work completed date is within window
                int days = await _settings.GetSetting<int>(SystemSettings.MaxDaysInPastForEntry);
                DateOnly maxPast = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));
                days = await _settings.GetSetting<int>(SystemSettings.MaxDaysInFutureForEntry);
                DateOnly maxFuture = DateOnly.FromDateTime(DateTime.Today.AddDays(days));

                // Validate date work was completed on is within allowed window
                if ((_newTransactionData.WorkCompletedDate < maxPast) || (_newTransactionData.WorkCompletedDate < maxFuture))
                {
                    return ServiceResult<Guid>.Fail("Work Completed outside range of " + maxPast.ToString() + " - " + maxFuture.ToString());
                }
                newTimeTransaction.WorkCompletedDate = _newTransactionData.WorkCompletedDate;

                // Store hours
                newTimeTransaction.WorkCompletedHours = _newTransactionData.WorkCompletedHours;

                // Store notes
                newTimeTransaction.Notes = _newTransactionData.Notes;

                // If this is an adjustment transaction then verify a Related Id is supplied
                if (_newTransactionData.IsAdjustment)
                {
                    if (_newTransactionData.RelatedId == null)
                    {
                        return ServiceResult<Guid>.Fail("Ticket is an adjustment but has no related ticket Id.");
                    }
                    
                    bool validRelatedTicket = await _db.PstsTimeTransactionss.AnyAsync(u => u.TransactionId == _newTransactionData.RelatedId);
                    if (!validRelatedTicket)
                    {
                        return ServiceResult<Guid>.Fail("Ticket is an adjustment but related ticket Id is not valid.");
                    }
                }
                newTimeTransaction.IsAdjustment = _newTransactionData.IsAdjustment;
                newTimeTransaction.RelatedId = _newTransactionData.RelatedId;


                await _db.PstsTimeTransactionss.AddAsync(newTimeTransaction);
                await _db.SaveChangesAsync();

                return ServiceResult<Guid>.Ok(newTimeTransaction.TransactionId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> AdjudicateTransactionAdjustment(string _requestorId, RoleTypes _requestorRole, ApprovalDecisionDto _decision)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.PstsUserProfiles.AnyAsync(u => u.EmployeeId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<bool>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<bool>.Fail("Invalid role.");
                }
                
                bool managerApprovalRequired = await _settings.GetSetting<bool>(SystemSettings.ManagerApprovalForAdjustments);

                if (managerApprovalRequired)
                {
                    if (_requestorRole != RoleTypes.Manager)
                    {
                        return ServiceResult<bool>.Fail("Insufficient privileges. Managers Only.");
                    }
                }
                else
                {
                    if ((_requestorRole != RoleTypes.Manager) || (_requestorRole != RoleTypes.Employee))
                    {
                        return ServiceResult<bool>.Fail("Insufficient privileges.");
                    }
                }

                // Validate transaction to approve
                var transactionToApprove = await _db.PstsTimeTransactionss.FindAsync(_decision.SubjectTransactionId);
                if (transactionToApprove == null)
                {
                    return ServiceResult<bool>.Fail("Transaction Id to approve not found.");
                }

                if (!transactionToApprove.IsAdjustment)
                {
                    return ServiceResult<bool>.Fail("Transaction to approve is not an adjustment.");
                }

                var verifyNoCurrentApproval = await _db.pstsTimeAdjustmentApprovalLedgers.FindAsync(_decision.SubjectTransactionId);
                if (verifyNoCurrentApproval != null)
                {
                    return ServiceResult<bool>.Fail("Transaction already " + verifyNoCurrentApproval.Decision.ToString());
                }

                var newApprovalAction = new PstsTimeAdjustmentApprovalLedger();
                newApprovalAction.SubjectTransactionId = _decision.SubjectTransactionId;
                newApprovalAction.ApprovalAuthority = _requestorId;
                newApprovalAction.Decision = _decision.Decision;
                newApprovalAction.Notes = _decision.Notes;
                newApprovalAction.DecisionTimeStamp = DateTime.UtcNow;

                await _db.pstsTimeAdjustmentApprovalLedgers.AddAsync(newApprovalAction);
                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
