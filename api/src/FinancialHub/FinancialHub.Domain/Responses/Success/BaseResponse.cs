using FinancialHub.Domain.Responses.Errors;

namespace FinancialHub.Domain.Responses.Success
{
    public abstract class BaseResponse<T>
    {
        public T Data { get; }
        public string Title { get; }
        public bool HasErrors => this.Errors.Length > 0;
        public BaseErrorResponse[] Errors { get ;}
        
        public BaseResponse(T data)
        {
            this.Data = data;
        }

        public BaseResponse(T data,string title, params BaseErrorResponse[] errors)
        {
            this.Data = data;
            this.Title = title;
            this.Errors = errors;
        }
    }
}
