using FinancialHub.Domain.Entities;
using FinancialHub.Domain.Filters;
using FinancialHub.Domain.Models;
using FinancialHub.Domain.Queries;
using FinancialHub.Domain.Interfaces.Services;
using FinancialHub.Domain.Interfaces.Repositories;
using FinancialHub.Domain.Interfaces.Mappers;
using FinancialHub.Domain.Results;
using FinancialHub.Domain.Results.Errors;

namespace FinancialHub.Services.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IMapperWrapper mapper;
        private readonly ITransactionsRepository repository;
        private readonly IAccountsRepository accountsRepository;
        private readonly ICategoriesRepository categoriesRepository;

        public TransactionsService(
            IMapperWrapper mapper, 
            ITransactionsRepository repository, 
            IAccountsRepository accountsRepository, ICategoriesRepository categoriesRepository
        )
        {
            this.mapper = mapper;
            this.repository = repository;
            this.accountsRepository = accountsRepository;
            this.categoriesRepository = categoriesRepository;
        }

        private async Task<ServiceResult<bool>> ValidateTransaction(TransactionEntity transaction)
        {
            var account = await this.accountsRepository.GetByIdAsync(transaction.AccountId);
            if (account == null)
            {
                return new NotFoundError($"Not found Account with id {transaction.AccountId}");
            }

            var category = await this.categoriesRepository.GetByIdAsync(transaction.CategoryId);
            if (category == null)
            {
                return new NotFoundError($"Not found Category with id {transaction.CategoryId}");
            }

            return true;
        }

        private async Task<ServiceResult<bool>> Validate(TransactionEntity transaction)
        {
            var result = new ServiceResult<bool>();

            var account = await this.accountsRepository.GetByIdAsync(transaction.AccountId);
            if (account == null)
            {
                result.Errors.Add(new NotFoundError($"Not found Account with id {transaction.AccountId}"));
            }

            var category = await this.categoriesRepository.GetByIdAsync(transaction.CategoryId);
            if (category == null)
            {
                result.Errors.Add(new NotFoundError($"Not found Category with id {transaction.CategoryId}"));
            }

            return result;
        }

        public async Task<ServiceResult<TransactionModel>> CreateAsync(TransactionModel category)
        {
            var entity = mapper.Map<TransactionEntity>(category);

            var validationResult = await this.Validate(entity);
            if (validationResult.HasError)
            {
                return new ServiceResult<TransactionModel>(default, validationResult.Errors);
            }

            entity = await this.repository.CreateAsync(entity);

            return mapper.Map<TransactionModel>(entity);
        }

        public async Task<ServiceResult<int>> DeleteAsync(Guid id)
        {
            return await this.repository.DeleteAsync(id);
        }

        public async Task<ServiceResult<ICollection<TransactionModel>>> GetAllByUserAsync(string userId, TransactionFilter filter)
        {
            var query = mapper.Map<TransactionQuery>(filter);
            //query.UserId = userId;

            var entities = await this.repository.GetAsync(query.Query());

            var models = mapper.Map<ICollection<TransactionModel>>(entities);

            return models.ToArray();
        }

        public async Task<ServiceResult<TransactionModel>> UpdateAsync(Guid id, TransactionModel transaction)
        {
            var entity = await this.repository.GetByIdAsync(id);

            if (entity == null)
            {
                return new NotFoundError($"Not found Transaction with id {id}");
            }

            entity = this.mapper.Map<TransactionEntity>(transaction);

            var validation = await this.ValidateTransaction(entity);
            if (validation.HasError)
            {
                return new ServiceResult<TransactionModel>(error: validation.Error);
            }

            entity = await this.repository.UpdateAsync(entity);

            return mapper.Map<TransactionModel>(entity);
        }
    }
}
