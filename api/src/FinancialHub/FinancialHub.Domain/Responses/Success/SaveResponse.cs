using FinancialHub.Domain.Responses.Errors;

namespace FinancialHub.Domain.Responses.Success
{
    public class SaveResponse<T> : BaseResponse<T>
    {
        public SaveResponse(T data) : base(data)
        {
        }

        public SaveResponse(T data, string title, params BaseErrorResponse[] errors) : base(data, title, errors)
        {
        }
    }
}
