using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Infrastructure.Services.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MovimentacaoController : ControllerBase
	{
		private readonly IMediator _mediator;

		public MovimentacaoController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		[SwaggerOperation(Summary = "Realiza uma movimentação na conta corrente")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Movimentar([FromBody] MovimentacaoCommand command)
		{
			try
			{
				var movimentoId = await _mediator.Send(command);
				return Ok(new { IdMovimento = movimentoId });
			}
			catch (BusinessException ex)
			{
				return BadRequest(new { Erro = ex.Message, Tipo = ex.TipoErro });
			}
		}
	}

}
