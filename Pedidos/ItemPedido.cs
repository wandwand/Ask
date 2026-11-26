using SistemaDelivery.Cardapios;

namespace SistemaDelivery.Pedidos;

// Composição (Parte) - ItemPedido só faz sentido dentro de um Pedido
public class ItemPedido
{
    private readonly ItemCardapio item;
    private readonly int quantidade;
    private readonly string observacao;

    // Construtor internal: criado e gerenciado exclusivamente por Pedido no mesmo assembly
    internal ItemPedido(ItemCardapio item, int quantidade, string observacao)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item do cardápio não pode ser nulo.");
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "Quantidade deve ser maior que zero.");

        this.item = item;
        this.quantidade = quantidade;
        this.observacao = string.IsNullOrWhiteSpace(observacao) ? "Sem observações" : observacao.Trim();
    }

    public double GetSubtotal()
    {
        // Chamada polimórfica ao CalcularPrecoFinal() de Lanche, Bebida ou Sobremesa
        return item.CalcularPrecoFinal() * quantidade;
    }

    public ItemCardapio Item => item;
    public int Quantidade => quantidade;
    public string Observacao => observacao;
}
