using SistemaDelivery.Interfaces;

namespace SistemaDelivery.Models;

// [REQUISITO: Herança Genuína 3 - Subclasse de ItemCardapio que também implementa IPreparavel]
public class Sobremesa : ItemCardapio, IPreparavel
{
    // [REQUISITO: Modificador private - Encapsulamento de atributos específicos]
    private readonly bool contemLactose;
    private readonly string temperaturaServir;
    private readonly int porcaoGramas;

    private const double TaxaEmbalagemEspecial = 1.50;

    public Sobremesa(
        string codigo,
        string nome,
        double precoBase,
        string descricao,
        bool contemLactose,
        string temperaturaServir,
        int porcaoGramas)
        : base(codigo, nome, precoBase, descricao)
    {
        this.contemLactose = contemLactose;
        this.temperaturaServir = temperaturaServir;
        this.porcaoGramas = porcaoGramas;
    }

    public override double CalcularPrecoFinal()
    {
        return precoBase + TaxaEmbalagemEspecial;
    }

    public override int CalcularTempoPreparo()
    {
        return string.Equals(temperaturaServir, "Quente", StringComparison.OrdinalIgnoreCase) ? 10 : 4;
    }

    public override string ExibirDetalhes()
    {
        string lactose = contemLactose ? "Contém lactose" : "Zero lactose";
        return $"[{codigo}] Sobremesa: {nome} ({porcaoGramas}g | Servir: {temperaturaServir} | {lactose}) | Preparo: {CalcularTempoPreparo()} min | Preço Final: R$ {CalcularPrecoFinal():F2}";
    }

    public string GerarInstrucoesCozinha()
    {
        string avisoLactose = contemLactose ? "Atenção: contém lactose" : "Produto sem lactose";
        return $"CONFEITARIA -> Montar {nome} ({porcaoGramas}g) para servir {temperaturaServir.ToUpperInvariant()}. {avisoLactose}. Embalar em recipiente térmico.";
    }

    public bool ContemLactose => contemLactose;
    public string TemperaturaServir => temperaturaServir;
    public int PorcaoGramas => porcaoGramas;
}
