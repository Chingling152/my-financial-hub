﻿using FinancialHub.Domain.Entities;
using FinancialHub.Domain.Filters;
using FinancialHub.Domain.Models;
using FinancialHub.Domain.Queries;
using FinancialHub.Domain.Interfaces.Services;
using FinancialHub.Domain.Interfaces.Repositories;
using FinancialHub.Domain.Interfaces.Mappers;
using FinancialHub.Domain.Results;
using FinancialHub.Domain.Results.Errors;
using FinancialHub.Domain.Enums;

namespace FinancialHub.Services.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IMapperWrapper mapper;
        private readonly ITransactionsRepository repository;
        private readonly IBalancesRepository balancesRepository;
        private readonly ICategoriesRepository categoriesRepository;

        public TransactionsService(
            IMapperWrapper mapper,
            ITransactionsRepository repository,
            IBalancesRepository balancesRepository, ICategoriesRepository categoriesRepository
        )
        {
            this.mapper = mapper;
            this.repository = repository;
            this.balancesRepository = balancesRepository;
            this.categoriesRepository = categoriesRepository;
        }

        private async Task<ServiceResult<bool>> ValidateTransaction(TransactionEntity transaction)
        {
            var balance = await this.balancesRepository.GetByIdAsync(transaction.BalanceId);
            if (balance == null)
            {
                return new NotFoundError($"Not found Balance with id {transaction.BalanceId}");
            }

            var category = await this.categoriesRepository.GetByIdAsync(transaction.CategoryId);
            if (category == null)
            {
                return new NotFoundError($"Not found Category with id {transaction.CategoryId}");
            }

            return true;
        }

        public async Task<ServiceResult<TransactionModel>> CreateAsync(TransactionModel transaction)
        {
            var entity = mapper.Map<TransactionEntity>(transaction);

            var validation = await this.ValidateTransaction(entity);
            if (validation.HasError)
            {
                return new ServiceResult<TransactionModel>(error: validation.Error);
            }

            entity = await this.repository.CreateAsync(entity);

            if (entity.Status == TransactionStatus.Committed && entity.IsActive)
            {
                await this.balancesRepository.AddAmountAsync(entity);
            }

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
            var oldTransaction = await this.repository.GetByIdAsync(id);
            if (oldTransaction == null)
            {
                return new NotFoundError($"Not found Transaction with id {id}");
            }

            var newTransaction = this.mapper.Map<TransactionEntity>(transaction);

            var validation = await this.ValidateTransaction(newTransaction);
            if (validation.HasError)
            {
                return new ServiceResult<TransactionModel>(error: validation.Error);
            }

            newTransaction = await this.repository.UpdateAsync(newTransaction);

            if 
            (
                newTransaction.IsActive && newTransaction.Status == TransactionStatus.Committed &&
                (!oldTransaction.IsActive || newTransaction.Status != oldTransaction.Status)
            )
            {
                await this.balancesRepository.AddAmountAsync(newTransaction);
            }
            else if
            (
                oldTransaction.IsActive && oldTransaction.Status == TransactionStatus.Committed &&
                (!newTransaction.IsActive || newTransaction.Status != oldTransaction.Status) 
            )
            {
                await this.balancesRepository.RemoveAmountAsync(newTransaction);
            }

            return mapper.Map<TransactionModel>(newTransaction);
        }
    }
}
