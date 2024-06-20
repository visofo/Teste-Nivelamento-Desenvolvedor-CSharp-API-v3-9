namespace Questao5.Domain.Exceptions
{
	namespace Questao5.Domain.Exceptions
	{
		public class BusinessException : Exception
		{
			public string TipoErro { get; }

			public BusinessException(string message, string tipoErro) : base(message)
			{
				TipoErro = tipoErro;
			}
		}
	}

}
