using FinancialHub.Domain.Results.Errors;
using System.Collections.Generic;

namespace FinancialHub.Domain.Results
{
    public class ServiceResult<T>
    {
        public bool HasError => this.Error != null || this.Errors.Count > 0;
        public ServiceError Error { get; protected set; }
        public ICollection<ServiceError> Errors { get; }
        public T Data { get; protected set; }

        public ServiceResult(T data = default, ServiceError error = null)
        {
            this.Data = data;
            this.Error = error;
            this.Errors = new List<ServiceError>();
        }

        public ServiceResult(T data, ICollection<ServiceError> errors)//TODO: change to params
        {
            this.Data = data;
            this.Errors = errors;
        }

        public static implicit operator ServiceResult<T>(T result)
        {
            return new ServiceResult<T>(result);
        }

        public static implicit operator ServiceResult<T>(ServiceError error)
        {
            return new ServiceResult<T>(error: error);
        }
    }
}
