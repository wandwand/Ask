namespace SistemaDelivery.Cardapios;

// Agregação - Cardapio agrega ItemCardapio criados externamente; se Cardapio sumir, os itens continuam existindo
public class Cardapio
{
    private readonly string nomeRestaurante;
    private readonly List<ItemCardapio> itens;

    public Cardapio(string nomeRestaurante)
    {
        this.nomeRestaurante = nomeRestaurante;
        this.itens = new List<ItemCardapio>();
    }

    public void AdicionarItem(ItemCardapio item)
    {
        if (item != null)
            itens.Add(item);
    }

    public ItemCardapio? BuscarPorCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        string codigoNormalizado = codigo.Trim();
        foreach (var item in itens)
        {
            if (string.Equals(item.Codigo, codigoNormalizado, StringComparison.OrdinalIgnoreCase))
                return item;
        }
        return null;
    }

    public IReadOnlyList<ItemCardapio> Itens => itens.AsReadOnly();
    public string NomeRestaurante => nomeRestaurante;
}
