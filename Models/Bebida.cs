namespace SistemaDelivery.Models;

// [REQUISITO: Herança Genuína 2 - Subclasse de ItemCardapio]
public class Bebida : ItemCardapio
{
    // [REQUISITO: Modificador private - Encapsulamento de atributos específicos]
    private readonly int volumeMl;
    private readonly bool isAlcoolica;
    private bool embalagemTermica;

    private const double TaxaEmbalagemTermica = 2.50;

    public Bebida(
        string codigo,
        string nome,
        double precoBase,
        string descricao,
        int volumeMl,
        bool isAlcoolica,
        bool embalagemTermica)
        : base(codigo, nome, precoBase, descricao)
    {
        this.volumeMl = volumeMl;
        this.isAlcoolica = isAlcoolica;
        this.embalagemTermica = embalagemTermica;
    }

    public override double CalcularPrecoFinal()
    {
        return embalagemTermica ? precoBase + TaxaEmbalagemTermica : precoBase;
    }

    public override int CalcularTempoPreparo()
    {
        // Bebidas praticamente não exigem tempo de preparo (apenas separação)
        return embalagemTermica ? 2 : 1;
    }

    public override string ExibirDetalhes()
    {
        string alcool = isAlcoolica ? "Alcoólica (+18)" : "Sem álcool";
        string termica = embalagemTermica ? " | Embalagem Térmica (+R$ 2,50)" : "";
        return $"[{codigo}] Bebida: {nome} ({volumeMl}ml - {alcool}){termica} | Preparo: {CalcularTempoPreparo()} min | Preço Final: R$ {CalcularPrecoFinal():F2}";
    }

    public int VolumeMl => volumeMl;
    public bool IsAlcoolica => isAlcoolica;

    public bool EmbalagemTermica
    {
        get => embalagemTermica;
        set => embalagemTermica = value;
    }
}
