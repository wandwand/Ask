namespace SistemaDelivery.Cardapios;

// [REQUISITO: Classe Abstrata - Base para os produtos do cardápio]
public abstract class ItemCardapio
{
    // [REQUISITO: Modificador protected - Acessível diretamente pelas subclasses]
    protected string codigo;
    protected string nome;
    protected double precoBase;
    protected string descricao;

    protected ItemCardapio(string codigo, string nome, double precoBase, string descricao)
    {
        this.codigo = codigo;
        this.nome = nome;
        this.precoBase = precoBase;
        this.descricao = descricao;
    }

    // Métodos abstratos que obrigam as subclasses a implementar regras próprias (Polimorfismo)
    public abstract double CalcularPrecoFinal();
    public abstract int CalcularTempoPreparo();
    public abstract string ExibirDetalhes();

    // [REQUISITO: Modificador public - Propriedades para acesso controlado]
    public string Codigo
    {
        get => codigo;
        set => codigo = value;
    }

    public string Nome
    {
        get => nome;
        set => nome = value;
    }

    public double PrecoBase
    {
        get => precoBase;
        set => precoBase = value;
    }

    public string Descricao
    {
        get => descricao;
        set => descricao = value;
    }
}
