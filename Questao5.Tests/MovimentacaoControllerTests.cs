using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Exceptions.Questao5.Domain.Exceptions;
using Questao5.Infrastructure.Services.Controllers;
using Xunit;

namespace Questao5.Tests
{
	public class MovimentacaoControllerTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly MovimentacaoController _controller;

		public MovimentacaoControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new MovimentacaoController(_mediatorMock.Object);
		}

		[Fact]
		public async Task Movimentar_DeveRetornarOkComIdMovimento_QuandoComandoForValido()
		{
			// Arrange
			var command = new MovimentacaoCommand();
			var movimentoId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
				.ReturnsAsync(movimentoId);

			// Act
			var result = await _controller.Movimentar(command);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

			var retorno = JObject.FromObject(okResult.Value);
			Assert.Equal(movimentoId.ToString(), retorno["IdMovimento"].ToString());
		}

		[Fact]
		public async Task Movimentar_DeveRetornarBadRequest_QuandoOcorrerBusinessException()
		{
			// Arrange
			var command = new MovimentacaoCommand();
			var exception = new BusinessException("Erro de negócio", "TipoErro");
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
				.ThrowsAsync(exception);

			// Act
			var result = await _controller.Movimentar(command);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

			var retorno = JObject.FromObject(badRequestResult.Value);
			Assert.Equal("Erro de negócio", retorno["Erro"].ToString());
			Assert.Equal("TipoErro", retorno["Tipo"].ToString());
		}
	}
}
