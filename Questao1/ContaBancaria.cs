using System;
using System.Globalization;

namespace Questao1
{
	class ContaBancaria
	{
		private int numeroConta;
		private string titular;
		private double saldo;

		public ContaBancaria(int numeroConta, string titular, double depositoInicial = 0)
		{
			this.numeroConta = numeroConta;
			this.titular = titular;
			this.saldo = depositoInicial;
		}

		public void Deposito(double valor)
		{
			if (valor > 0)
			{
				this.saldo += valor;
				Console.WriteLine("Depósito realizado com sucesso!");
			}
			else
			{
				Console.WriteLine("Valor inválido para depósito. Tente novamente.");
			}
		}

		public void Saque(double valor)
		{
			double taxa = 3.50;
			if (valor > 0 && valor <= this.saldo + taxa)
			{
				this.saldo -= valor + taxa;
				Console.WriteLine("Saque realizado com sucesso!");
			}
			else if (valor <= 0)
			{
				Console.WriteLine("Valor inválido para saque. Tente novamente.");
			}
			else
			{
				Console.WriteLine("Saldo insuficiente para saque. Tente novamente.");
			}
		}

		public override string ToString()
		{
			return $"Conta {numeroConta}, Titular: {titular}, Saldo: $ {saldo:F2}";
		}
	}
}
