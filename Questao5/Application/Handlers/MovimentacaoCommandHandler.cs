using Dapper;
using MediatR;
using Newtonsoft.Json;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities.Questao5.Domain.Entities;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using System.Data;

namespace Questao5.Application.Handlers
{
	public class MovimentacaoCommandHandler : IRequestHandler<MovimentacaoCommand, Guid>
	{
		private readonly IDbConnection _dbConnection;
		private readonly IMediator _mediator;

		public MovimentacaoCommandHandler(IDbConnection dbConnection, IMediator mediator)
		{
			_dbConnection = dbConnection;
			_mediator = mediator;
		}

		public async Task<Guid> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
		{
			// Verificações de negócio
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

			if (request.Valor <= 0)
			{
				throw new BusinessException("Valor inválido", "INVALID_VALUE");
			}

			if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
			{
				throw new BusinessException("Tipo de movimento inválido", "INVALID_TYPE");
			}

			// Verificação de idempotência
			var idempotencia = await _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(
				"SELECT * FROM idempotencia WHERE chave_idempotencia = @Key",
				new { Key = request.IdempotencyKey }
			);

			if (idempotencia != null)
			{
				// Requisição já processada
				return Guid.Parse(idempotencia.Resultado);
			}

			// Inserir movimento
			var movimentoId = Guid.NewGuid();
			await _dbConnection.ExecuteAsync(
				"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@Id, @ContaId, @Data, @Tipo, @Valor)",
				new
				{
					Id = movimentoId,
					ContaId = request.IdContaCorrente,
					Data = DateTime.Now.ToString("dd/MM/yyyy"),
					Tipo = request.TipoMovimento,
					Valor = request.Valor
				}
			);

			// Registrar idempotência
			await _dbConnection.ExecuteAsync(
				"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@Key, @Request, @Resultado)",
				new
				{
					Key = request.IdempotencyKey,
					Request = JsonConvert.SerializeObject(request),
					Resultado = movimentoId.ToString()
				}
			);

			return movimentoId;
		}
	}

}
