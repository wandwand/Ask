namespace SistemaDelivery.Interfaces;

// [REQUISITO: Interface 1 - Contrato para polimorfismo de formas de pagamento]
public interface IMetodoPagamento
{
    double ProcessarPagamento(double valor);
    string GetNomeMetodo();
}
