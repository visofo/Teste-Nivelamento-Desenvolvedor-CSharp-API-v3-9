using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Services.Controllers;
using Xunit;

namespace Questao5.Tests
{
	public class SaldoControllerTests
	{
		private readonly IMediator _mediator;
		private readonly SaldoController _controller;

		public SaldoControllerTests()
		{
			_mediator = Substitute.For<IMediator>();
			_controller = new SaldoController(_mediator);
		}

		[Fact]
		public async Task ConsultarSaldo_DeveRetornarOkComSaldo()
		{
			// Arrange
			var id = Guid.NewGuid();
			var saldoEsperado = new SaldoResponse { SaldoAtual = 100.0m };
			_mediator.Send(Arg.Any<SaldoQuery>()).Returns(Task.FromResult(saldoEsperado));

			// Act
			var result = await _controller.ConsultarSaldo(id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
			Assert.Equal(saldoEsperado, okResult.Value);
		}

		[Fact]
		public async Task ConsultarSaldo_DeveRetornarBadRequestQuandoBusinessException()
		{
			// Arrange
			var id = Guid.NewGuid();
			var mensagemErro = "Erro de negócio";
			var tipoErro = "TipoErroExemplo";
			_mediator.Send(Arg.Any<SaldoQuery>()).Throws(new BusinessException(mensagemErro, tipoErro));

			// Act
			var result = await _controller.ConsultarSaldo(id);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
			
			var retorno = JObject.FromObject(badRequestResult.Value);
			Assert.Equal(mensagemErro, retorno["Erro"].ToString());
			Assert.Equal(tipoErro, retorno["Tipo"].ToString());
		}
	}
}
