namespace SistemaDelivery.Cardapios;

// [REQUISITO: Herança Genuína 1 - Subclasse de ItemCardapio que também implementa IPreparavel]
public class Lanche : ItemCardapio, IPreparavel
{
    // [REQUISITO: Modificador private - Encapsulamento de atributos específicos]
    private readonly List<string> ingredientes;
    private bool adicionalBordaOuBacon;
    private readonly int tempoBasePreparoMinutos;

    private const double ValorAdicional = 6.00;

    public Lanche(
        string codigo,
        string nome,
        double precoBase,
        string descricao,
        IEnumerable<string> ingredientes,
        bool adicionalBordaOuBacon,
        int tempoBasePreparoMinutos)
        : base(codigo, nome, precoBase, descricao)
    {
        this.ingredientes = new List<string>(ingredientes);
        this.adicionalBordaOuBacon = adicionalBordaOuBacon;
        this.tempoBasePreparoMinutos = tempoBasePreparoMinutos;
    }

    public override double CalcularPrecoFinal()
    {
        // Acesso direto ao atributo protected precoBase da superclasse
        return adicionalBordaOuBacon ? precoBase + ValorAdicional : precoBase;
    }

    public override int CalcularTempoPreparo()
    {
        return adicionalBordaOuBacon ? tempoBasePreparoMinutos + 5 : tempoBasePreparoMinutos;
    }

    public override string ExibirDetalhes()
    {
        string extra = adicionalBordaOuBacon ? " (Com adicional de Bacon/Borda +R$ 6,00)" : "";
        return $"[{codigo}] Lanche: {nome}{extra} | Ingredientes: {string.Join(", ", ingredientes)} | Preparo: {CalcularTempoPreparo()} min | Preço Final: R$ {CalcularPrecoFinal():F2}";
    }

    public string GerarInstrucoesCozinha()
    {
        string extra = adicionalBordaOuBacon ? "INCLUIR ADICIONAL DE BACON/BORDA RECHEADA." : "Sem adicionais extras.";
        return $"CHAPA/FORNO -> Preparar {nome} com [{string.Join(", ", ingredientes)}]. {extra} Tempo alvo: {CalcularTempoPreparo()} min.";
    }

    public IReadOnlyList<string> Ingredientes => ingredientes.AsReadOnly();

    public bool AdicionalBordaOuBacon
    {
        get => adicionalBordaOuBacon;
        set => adicionalBordaOuBacon = value;
    }

    public int TempoBasePreparoMinutos => tempoBasePreparoMinutos;
}
