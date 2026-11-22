using SistemaDelivery.Cardapios;
using SistemaDelivery.Clientes;
using SistemaDelivery.Entregas;
using SistemaDelivery.Pagamentos;

namespace SistemaDelivery.Pedidos;

public class Pedido
{
    // [REQUISITO: Atributos Estáticos - Compartilhados por toda a classe Pedido]
    public static double TaxaEntregaBase = 5.00;
    private static int totalPedidosRealizados = 0;
    private static double faturamentoTotal = 0.0;
    private static int proximoNumeroPedido = 1;

    private readonly int numeroPedido;

    // [REQUISITO: Associação Simples - Pedido possui referência direta a Cliente e Entregador]
    private readonly Cliente cliente;
    private Entregador? entregador;

    // [REQUISITO: Composição - Pedido é composto por ItemPedido, instanciados internamente]
    private readonly List<ItemPedido> itens;

    private double distanciaKm;
    private double taxaEntrega;
    private double valorFinalPago;
    private bool finalizado;
    private string? metodoPagamentoUtilizado;

    public Pedido(Cliente cliente)
    {
        this.cliente = cliente ?? throw new ArgumentNullException(nameof(cliente), "O pedido precisa de um cliente.");
        this.numeroPedido = proximoNumeroPedido++;
        this.itens = new List<ItemPedido>();
        this.taxaEntrega = TaxaEntregaBase;
        this.finalizado = false;
    }

    // [REQUISITO: Método Estático 1 - Cálculo de frete baseado na distância]
    public static double CalcularTaxaPorDistancia(double km)
    {
        if (km < 0)
            throw new ArgumentOutOfRangeException(nameof(km), "A distância não pode ser negativa.");

        return TaxaEntregaBase + (km * 1.50);
    }

    // [REQUISITO: Método Estático 2 - Consulta de estatística global]
    public static int GetTotalPedidosRealizados() => totalPedidosRealizados;

    public static double GetFaturamentoTotal() => faturamentoTotal;

    // [REQUISITO: Composição - ItemPedido é criado e gerenciado exclusivamente dentro de Pedido]
    public void AdicionarItem(ItemCardapio item, int quantidade, string observacao)
    {
        if (finalizado)
            throw new InvalidOperationException("Não é possível adicionar itens a um pedido já finalizado.");

        var novoItem = new ItemPedido(item, quantidade, observacao);
        itens.Add(novoItem);
    }

    public bool RemoverItem(ItemPedido itemPedido)
    {
        if (finalizado)
            throw new InvalidOperationException("Não é possível remover itens de um pedido já finalizado.");

        return itens.Remove(itemPedido);
    }

    public void AtribuirEntregador(Entregador entregador, double distanciaKm)
    {
        if (entregador == null)
            throw new ArgumentNullException(nameof(entregador), "Entregador inválido.");
        if (!entregador.Disponivel)
            throw new InvalidOperationException($"O entregador {entregador.Nome} não está disponível no momento.");

        if (this.entregador != null)
            this.entregador.Disponivel = true;

        this.entregador = entregador;
        this.entregador.Disponivel = false;
        this.distanciaKm = distanciaKm;
        this.taxaEntrega = CalcularTaxaPorDistancia(distanciaKm);
    }

    public double CalcularSubtotalItens()
    {
        double subtotal = 0.0;
        foreach (var itemPedido in itens)
        {
            subtotal += itemPedido.GetSubtotal();
        }
        return subtotal;
    }

    public double CalcularTotalComFrete() => CalcularSubtotalItens() + taxaEntrega;

    public int CalcularTempoTotalPreparo()
    {
        int maiorTempo = 0;
        foreach (var itemPedido in itens)
        {
            // Chamada polimórfica a CalcularTempoPreparo()
            int tempoItem = itemPedido.Item.CalcularTempoPreparo();
            if (tempoItem > maiorTempo)
                maiorTempo = tempoItem;
        }
        return maiorTempo;
    }

    // [REQUISITO: Dependência e Polimorfismo 2 - Recebe IMetodoPagamento apenas na execução do método]
    public double FinalizarPedido(IMetodoPagamento pagamento)
    {
        if (finalizado)
            throw new InvalidOperationException("Este pedido já foi finalizado.");
        if (itens.Count == 0)
            throw new InvalidOperationException("Não é possível finalizar um pedido sem itens.");
        if (pagamento == null)
            throw new ArgumentNullException(nameof(pagamento), "Método de pagamento não informado.");

        double totalBruto = CalcularTotalComFrete();
        valorFinalPago = pagamento.ProcessarPagamento(totalBruto);
        metodoPagamentoUtilizado = pagamento.GetNomeMetodo();
        finalizado = true;

        totalPedidosRealizados++;
        faturamentoTotal += valorFinalPago;

        return valorFinalPago;
    }

    public int NumeroPedido => numeroPedido;
    public Cliente Cliente => cliente;
    public Entregador? Entregador => entregador;
    public IReadOnlyList<ItemPedido> Itens => itens.AsReadOnly();
    public double DistanciaKm => distanciaKm;
    public double TaxaEntrega => taxaEntrega;
    public double ValorFinalPago => valorFinalPago;
    public bool Finalizado => finalizado;
    public string? MetodoPagamentoUtilizado => metodoPagamentoUtilizado;
}
