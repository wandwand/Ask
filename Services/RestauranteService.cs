using SistemaDelivery.Interfaces;
using SistemaDelivery.Models;

namespace SistemaDelivery.Services;

public class RestauranteService
{
    private readonly Cardapio cardapio;
    private readonly List<Cliente> clientes;
    private readonly List<Entregador> entregadores;
    private readonly List<Pedido> pedidos;

    public RestauranteService()
    {
        cardapio = new Cardapio("Sabor & Código Delivery");
        clientes = new List<Cliente>();
        entregadores = new List<Entregador>();
        pedidos = new List<Pedido>();
        CarregarDadosIniciais();
    }

    private void CarregarDadosIniciais()
    {
        // Itens criados externamente e agregados ao Cardapio (Agregação)
        ItemCardapio l1 = new Lanche(
            "L01", "X-Burger Artesanal", 28.00, "Hambúrguer 180g na brasa",
            new[] { "Pão brioche", "Blend 180g", "Queijo cheddar", "Maionese da casa" },
            false, 15);

        ItemCardapio l2 = new Lanche(
            "L02", "X-Bacon Supremo", 34.00, "Hambúrguer duplo com bacon crocante",
            new[] { "Pão australiano", "2x Blend 150g", "Bacon", "Cebola caramelizada" },
            true, 20);

        ItemCardapio b1 = new Bebida("B01", "Refrigerante Lata", 7.00, "Coca-Cola tradicional gelada", 350, false, false);
        ItemCardapio b2 = new Bebida("B02", "Suco Natural de Laranja", 12.00, "Suco fresco espremido na hora", 500, false, true);
        ItemCardapio b3 = new Bebida("B03", "Cerveja Artesanal IPA", 18.00, "Cerveja puro malte lupulada", 473, true, true);

        ItemCardapio s1 = new Sobremesa("S01", "Petit Gateau", 22.00, "Bolo quente de chocolate com sorvete", true, "Quente", 180);
        ItemCardapio s2 = new Sobremesa("S02", "Mousse de Maracujá", 14.00, "Mousse cremoso sem lactose", false, "Gelada", 150);

        cardapio.AdicionarItem(l1);
        cardapio.AdicionarItem(l2);
        cardapio.AdicionarItem(b1);
        cardapio.AdicionarItem(b2);
        cardapio.AdicionarItem(b3);
        cardapio.AdicionarItem(s1);
        cardapio.AdicionarItem(s2);

        clientes.Add(new Cliente(1, "Ana Souza", "Rua das Flores, 120 - Centro", "(11) 99999-1111"));
        clientes.Add(new Cliente(2, "Carlos Oliveira", "Av. Brasil, 850 - Jardim América", "(11) 98888-2222"));
        clientes.Add(new Cliente(3, "Mariana Lima", "Rua Vergueiro, 410 - Vila Mariana", "(11) 97777-3333"));

        entregadores.Add(new Entregador(1, "João Motoboy", "ABC-1D23", true));
        entregadores.Add(new Entregador(2, "Pedro Flash", "XYZ-9K87", true));
        entregadores.Add(new Entregador(3, "Lucas Express", "MNO-4F56", false));
    }

    public void ListarCardapioPolimorfico()
    {
        Console.WriteLine($"\n================ CARDÁPIO: {cardapio.NomeRestaurante.ToUpperInvariant()} ================");
        IReadOnlyList<ItemCardapio> itens = cardapio.Itens;

        // [REQUISITO: Polimorfismo 1 e 3 - Iteração sobre IReadOnlyList<ItemCardapio> chamando métodos polimórficos]
        foreach (ItemCardapio item in itens)
        {
            Console.WriteLine(item.ExibirDetalhes());
            Console.WriteLine($"   -> Cálculo Polimórfico: Preço Final = R$ {item.CalcularPrecoFinal():F2} | Tempo de Preparo = {item.CalcularTempoPreparo()} min");

            if (item is IPreparavel preparavel)
            {
                Console.WriteLine($"   -> Cozinha: {preparavel.GerarInstrucoesCozinha()}");
            }
        }
        Console.WriteLine("==========================================================================\n");
    }

    public Cardapio Cardapio => cardapio;
    public IReadOnlyList<Cliente> Clientes => clientes.AsReadOnly();
    public IReadOnlyList<Entregador> Entregadores => entregadores.AsReadOnly();
    public IReadOnlyList<Pedido> Pedidos => pedidos.AsReadOnly();

    public void RegistrarPedido(Pedido pedido)
    {
        if (pedido != null)
            pedidos.Add(pedido);
    }

    public Cliente? BuscarClientePorId(int id) => clientes.FirstOrDefault(c => c.Id == id);

    public Entregador? BuscarEntregadorPorId(int id) => entregadores.FirstOrDefault(e => e.Id == id);
}
