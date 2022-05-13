using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinancialHub.Domain.Models;
using FinancialHub.Domain.Filters;
using FinancialHub.Domain.Responses.Errors;
using FinancialHub.Domain.Responses.Success;
using FinancialHub.Domain.Interfaces.Services;
using System.Linq;

namespace FinancialHub.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionsService service;

        public TransactionsController(ITransactionsService service)
        {
            this.service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<TransactionModel>), 200)]
        /// <summary>
        /// Get all transaction of the system (will be changed to only one user and added filters)
        /// </summary>
        public async Task<IActionResult> GetMyTransactions([FromQuery] TransactionFilter filter)
        {
            var response = await service.GetAllByUserAsync("mock", filter);
            return Ok(new ListResponse<TransactionModel>(response.Data));
        }

        [HttpPost]
        [ProducesResponseType(typeof(SaveResponse<TransactionModel>), 200)]
        [ProducesResponseType(typeof(SaveResponse<TransactionModel>), 400)]
        /// <summary>
        /// Creates an transaction on database (will be changed to only one user)
        /// </summary>
        /// <param name="category">Transaction to be created</param>
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionModel transaction)
        {
            var result = await service.CreateAsync(transaction);

            if (result.HasError)
            {
                return StatusCode(
                    400,
                    new SaveResponse<TransactionModel>(
                        result.Data,
                        result.ToString(),
                        result.Errors.Select(x => new ValidationErrorResponse(x.Message)).ToArray()
                    )
                 );
            }

            return Ok(new SaveResponse<TransactionModel>(result.Data));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SaveResponse<TransactionModel>), 200)]
        [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
        /// <summary>
        /// Updates an existing transaction on database
        /// </summary>
        /// <param name="id">id of the transaction</param>
        /// <param name="transaction">transaction changes</param>
        public async Task<IActionResult> UpdateTransaction([FromRoute] Guid id, [FromBody] TransactionModel transaction)
        {
            var result = await service.UpdateAsync(id, transaction);

            if (result.HasError)
            {
                return StatusCode(
                    result.Error.Code,
                    new ValidationErrorResponse(result.Error.Message)
                 );
            }

            return Ok(new SaveResponse<TransactionModel>(result.Data));

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        /// <summary>
        /// Deletes an existing transaction on database
        /// </summary>
        /// <param name="id">id of the transaction</param>
        public async Task<IActionResult> DeleteTransaction([FromRoute] Guid id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
    }
}
