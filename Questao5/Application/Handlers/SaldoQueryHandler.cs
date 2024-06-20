using Dapper;
using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities.Questao5.Domain.Entities;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using System.Data;

namespace Questao5.Application.Handlers
{
	public class SaldoQueryHandler : IRequestHandler<SaldoQuery, SaldoResponse>
	{
		private readonly IDbConnection _dbConnection;

		public SaldoQueryHandler(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<SaldoResponse> Handle(SaldoQuery request, CancellationToken cancellationToken)
		{
			var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
				"SELECT * FROM contacorrente WHERE idcontacorrente = @Id",
				new { Id = request.IdContaCorrente }
			);

			if (conta == null)
			{
				throw new BusinessException("Conta corrente não cadastrada", "INVALID_ACCOUNT");
			}

			if (conta.Ativo == 0)
			{
				throw new BusinessException("Conta corrente inativa", "INACTIVE_ACCOUNT");
			}

			var creditos = await _dbConnection.QueryAsync<decimal>(
				"SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'C'",
				new { Id = request.IdContaCorrente }
			);

			var debitos = await _dbConnection.QueryAsync<decimal>(
				"SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @Id AND tipomovimento = 'D'",
				new { Id = request.IdContaCorrente }
			);

			var saldo = creditos.Sum() - debitos.Sum();

			return new SaldoResponse
			{
				NumeroConta = conta.Numero,
				NomeTitular = conta.Nome,
				DataHoraConsulta = DateTime.Now,
				SaldoAtual = saldo
			};
		}
	}

}
