using psts.web.Domain.Enums;
using psts.web.Dto;


namespace psts.web.Services

{
    public interface ITimeServices
    {
        Task<ServiceResult<Guid>> CreateNewTimeTransaction(string _requestorId, RoleTypes _requestorRole, NewTimeTransactionDto _newTransactionData);
        Task<ServiceResult<Guid>> CreateTimeTransactionAdjustment(string _requestorId, RoleTypes _requestorRole, NewTimeTransactionAdjustmentDto _newAdjustmentData);
        Task<ServiceResult<bool>> AdjudicateTransactionAdjustment(string _requestorId, RoleTypes _requestorRole, ApprovalDecisionDto _decision);
        Task<ServiceResult<List<UnadjudicatedAdjustmentItem>>> FindUnadjudicatedTransactionAdjustments(DateTime? _StartEnteredDate, DateTime? _EndEnteredDate);
    }
}
