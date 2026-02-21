using psts.web.Domain.Enums;
using psts.web.Dto;


namespace psts.web.Services

{
    public interface ITimeServices
    {
        Task<ServiceResult<Guid>> EnterNewTimeTransaction(string _requestorId, RoleTypes _requestorRole, NewTimeTransactionDto _newTransactionData);
        Task<ServiceResult<bool>> ApproveTransactionAdjustment(string _requestorId, RoleTypes _requestorRole, Guid _transactionIdToApprove);
    }
}
