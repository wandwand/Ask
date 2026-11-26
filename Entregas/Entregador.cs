namespace SistemaDelivery.Entregas;

// Associação Simples - Entidade Entregador referenciada por Pedido
public class Entregador
{
    private readonly int id;
    private readonly string nome;
    private readonly string placaMoto;
    private bool disponivel;

    public Entregador(int id, string nome, string placaMoto, bool disponivel)
    {
        this.id = id;
        this.nome = nome;
        this.placaMoto = placaMoto;
        this.disponivel = disponivel;
    }

    public int Id => id;
    public string Nome => nome;
    public string PlacaMoto => placaMoto;

    public bool Disponivel
    {
        get => disponivel;
        set => disponivel = value;
    }

    public override string ToString()
    {
        string status = disponivel ? "Disponível" : "Em entrega";
        return $"#{id} - {nome} | Moto: {placaMoto} | Status: {status}";
    }
}
