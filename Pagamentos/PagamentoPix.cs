namespace SistemaDelivery.Pagamentos;

// Implementação de Interface 1 - Pagamento via Pix com 5% de desconto
public class PagamentoPix : IMetodoPagamento
{
    private const double DescontoPix = 0.05;
    private readonly string chavePix;

    public PagamentoPix(string chavePix)
    {
        this.chavePix = chavePix;
    }

    public double ProcessarPagamento(double valor)
    {
        double valorComDesconto = valor * (1.0 - DescontoPix);
        Console.WriteLine($"-> Processando Pix (chave: {chavePix}): 5% de desconto aplicado! Valor final: R$ {valorComDesconto:F2}");
        return valorComDesconto;
    }

    public string GetNomeMetodo()
    {
        return "Pix (5% de desconto)";
    }

    public string ChavePix => chavePix;
}
