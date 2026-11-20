namespace SistemaDelivery.Clientes;

// [REQUISITO: Associação Simples (Parte 1) - Entidade Cliente referenciada por Pedido]
public class Cliente
{
    private readonly int id;
    private string nome;
    private string endereco;
    private string telefone;

    public Cliente(int id, string nome, string endereco, string telefone)
    {
        this.id = id;
        this.nome = nome;
        this.endereco = endereco;
        this.telefone = telefone;
    }

    public int Id => id;

    public string Nome
    {
        get => nome;
        set => nome = value;
    }

    public string Endereco
    {
        get => endereco;
        set => endereco = value;
    }

    public string Telefone
    {
        get => telefone;
        set => telefone = value;
    }

    public override string ToString()
    {
        return $"#{id} - {nome} | Endereço: {endereco} | Tel: {telefone}";
    }
}
