using SistemaDelivery.Interfaces;

namespace SistemaDelivery.Models;

// [REQUISITO: Implementação de Interface 2 - Pagamento via Cartão com taxa de maquininha/parcelamento]
public class PagamentoCartao : IMetodoPagamento
{
    private const double TaxaBaseMaquininha = 0.03;
    private readonly string numeroCartaoMascarado;
    private readonly int parcelas;

    public PagamentoCartao(string numeroCartaoMascarado, int parcelas)
    {
        this.numeroCartaoMascarado = numeroCartaoMascarado;
        this.parcelas = Math.Max(1, parcelas);
    }

    public double ProcessarPagamento(double valor)
    {
        // Taxa base de 3% + 1% adicional por parcela extra acima de 1x
        double taxaTotal = TaxaBaseMaquininha + (parcelas > 1 ? (parcelas - 1) * 0.01 : 0.0);
        double valorFinal = valor * (1.0 + taxaTotal);
        double valorParcela = valorFinal / parcelas;

        Console.WriteLine($"-> Processando Cartão ({numeroCartaoMascarado}) em {parcelas}x de R$ {valorParcela:F2} (taxa total: {taxaTotal * 100:F0}%). Valor final: R$ {valorFinal:F2}");
        return valorFinal;
    }

    public string GetNomeMetodo()
    {
        return $"Cartão de Crédito ({parcelas}x)";
    }

    public string NumeroCartaoMascarado => numeroCartaoMascarado;
    public int Parcelas => parcelas;
}
