using MediatR;

namespace Questao5.Application.Commands.Requests
{
	public class MovimentacaoCommand : IRequest<Guid>
	{
		public Guid IdempotencyKey { get; set; }
		public Guid IdContaCorrente { get; set; }
		public Guid IdMovimento { get; set; }
		public decimal Valor { get; set; }
		public string TipoMovimento { get; set; }
	}

}
