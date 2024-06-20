using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Infrastructure.Services.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SaldoController : ControllerBase
	{
		private readonly IMediator _mediator;

		public SaldoController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{id}")]
		[SwaggerOperation(Summary = "Consulta o saldo da conta corrente")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ConsultarSaldo(Guid id)
		{
			try
			{
				var saldo = await _mediator.Send(new SaldoQuery { IdContaCorrente = id });
				return Ok(saldo);
			}
			catch (BusinessException ex)
			{
				return BadRequest(new { Erro = ex.Message, Tipo = ex.TipoErro });
			}
		}
	}

}
