using System.Globalization;
using System.Text;
using SistemaDelivery.Interfaces;
using SistemaDelivery.Models;
using SistemaDelivery.Services;

namespace SistemaDelivery;

public class Program
{
    private static readonly RestauranteService Service = new();
    private static readonly GeradorRecibo GeradorRecibo = new();
    private static Pedido? pedidoAtual;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        int opcao = -1;
        Console.WriteLine($"Bem-vindo ao Sistema de Pedidos & Delivery - {Service.Cardapio.NomeRestaurante}");

        while (opcao != 0)
        {
            ExibirMenu();
            opcao = LerInteiro("Escolha uma opção: ");

            switch (opcao)
            {
                case 1:
                    // [REQUISITO: Polimorfismo 1 e 3]
                    Service.ListarCardapioPolimorfico();
                    break;
                case 2:
                    CriarNovoPedido();
                    break;
                case 3:
                    AtribuirEntregadorAoPedido();
                    break;
                case 4:
                    FinalizarPedidoAtual();
                    break;
                case 5:
                    ExibirEstatisticasGerais();
                    break;
                case 0:
                    Console.WriteLine("Encerrando o sistema. Até logo!");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    private static void ExibirMenu()
    {
        Console.WriteLine("\n================ MENU PRINCIPAL ================");
        if (pedidoAtual != null && !pedidoAtual.Finalizado)
        {
            Console.WriteLine($" [Pedido em andamento: #{pedidoAtual.NumeroPedido:D4} | Cliente: {pedidoAtual.Cliente.Nome} | Itens: {pedidoAtual.Itens.Count}]");
        }
        else
        {
            Console.WriteLine(" [Nenhum pedido aberto no momento]");
        }
        Console.WriteLine("1. Listar Cardápio Completo (Polimorfismo)");
        Console.WriteLine("2. Criar Novo Pedido e Adicionar Itens");
        Console.WriteLine("3. Atribuir Entregador e Calcular Frete (Estático)");
        Console.WriteLine("4. Finalizar Pedido (Pagamento Pix/Cartão + Recibo)");
        Console.WriteLine("5. Exibir Estatísticas Gerais (Membros Estáticos)");
        Console.WriteLine("0. Sair");
        Console.WriteLine("================================================");
    }

    private static void CriarNovoPedido()
    {
        if (pedidoAtual != null && !pedidoAtual.Finalizado)
        {
            Console.WriteLine($"Já existe um pedido em aberto (#{pedidoAtual.NumeroPedido}). Deseja adicionar mais itens a ele? (S/N)");
            string resp = (Console.ReadLine() ?? string.Empty).Trim();
            if (resp.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                AdicionarItensAoPedido(pedidoAtual);
                return;
            }
        }

        Console.WriteLine("\n--- CLIENTES CADASTRADOS ---");
        foreach (Cliente c in Service.Clientes)
        {
            Console.WriteLine(c);
        }

        int idCliente = LerInteiro("Digite o ID do cliente para o pedido: ");
        Cliente? cliente = Service.BuscarClientePorId(idCliente);
        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        pedidoAtual = new Pedido(cliente);
        Service.RegistrarPedido(pedidoAtual);
        Console.WriteLine($"Pedido #{pedidoAtual.NumeroPedido} criado para {cliente.Nome}!");

        AdicionarItensAoPedido(pedidoAtual);
    }

    private static void AdicionarItensAoPedido(Pedido pedido)
    {
        bool continuar = true;
        while (continuar)
        {
            Service.ListarCardapioPolimorfico();
            Console.Write("Digite o código do item (ou 'FIM' para encerrar adição): ");
            string codigo = (Console.ReadLine() ?? string.Empty).Trim();

            if (codigo.Equals("FIM", StringComparison.OrdinalIgnoreCase))
            {
                continuar = false;
                continue;
            }

            ItemCardapio? item = Service.Cardapio.BuscarPorCodigo(codigo);
            if (item == null)
            {
                Console.WriteLine("Código não encontrado no cardápio.");
                continue;
            }

            int quantidade = LerInteiro("Quantidade: ");
            if (quantidade <= 0)
            {
                Console.WriteLine("Quantidade deve ser maior que zero.");
                continue;
            }

            Console.Write("Observação (ex: sem cebola, gelo e limão) [Enter para nenhuma]: ");
            string observacao = Console.ReadLine() ?? string.Empty;

            pedido.AdicionarItem(item, quantidade, observacao);
            Console.WriteLine($"Adicionado: {quantidade}x {item.Nome} (Subtotal atual dos itens: R$ {pedido.CalcularSubtotalItens():F2})");
        }
    }

    private static void AtribuirEntregadorAoPedido()
    {
        if (!ValidarPedidoAberto())
            return;

        Console.WriteLine("\n--- ENTREGADORES ---");
        foreach (Entregador e in Service.Entregadores)
        {
            Console.WriteLine(e);
        }

        int idEntregador = LerInteiro("Digite o ID do entregador disponível: ");
        Entregador? entregador = Service.BuscarEntregadorPorId(idEntregador);
        if (entregador == null)
        {
            Console.WriteLine("Entregador não encontrado.");
            return;
        }

        double distanciaKm = LerDouble("Informe a distância da entrega em km: ");
        if (distanciaKm < 0)
        {
            Console.WriteLine("Distância inválida.");
            return;
        }

        try
        {
            // Uso de método estático dentro de AtribuirEntregador
            double fretePrevisto = Pedido.CalcularTaxaPorDistancia(distanciaKm);
            pedidoAtual!.AtribuirEntregador(entregador, distanciaKm);
            Console.WriteLine($"Entregador {entregador.Nome} atribuído! Frete calculado (base R$ {Pedido.TaxaEntregaBase:F2} + distância): R$ {fretePrevisto:F2}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private static void FinalizarPedidoAtual()
    {
        if (!ValidarPedidoAberto())
            return;

        if (pedidoAtual!.Itens.Count == 0)
        {
            Console.WriteLine("O pedido está vazio. Adicione pelo menos um item antes de finalizar.");
            return;
        }

        Console.WriteLine($"\nTotal dos itens: R$ {pedidoAtual.CalcularSubtotalItens():F2} | Taxa de entrega: R$ {pedidoAtual.TaxaEntrega:F2} | Total bruto: R$ {pedidoAtual.CalcularTotalComFrete():F2}");
        Console.WriteLine("Escolha a forma de pagamento:");
        Console.WriteLine("1. Pix (5% de desconto)");
        Console.WriteLine("2. Cartão de Crédito (taxa de maquininha / parcelamento)");
        int escolha = LerInteiro("Opção de pagamento: ");

        // [REQUISITO: Polimorfismo 2 - Variável da interface IMetodoPagamento recebe implementações concretas]
        IMetodoPagamento pagamento;
        if (escolha == 1)
        {
            Console.Write("Informe a chave Pix do pagador (ou Enter para padrão): ");
            string chave = (Console.ReadLine() ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(chave))
                chave = "restaurante@pix.com.br";

            pagamento = new PagamentoPix(chave);
        }
        else if (escolha == 2)
        {
            Console.Write("Informe os 4 últimos dígitos do cartão (ex: 1234): ");
            string digitos = (Console.ReadLine() ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(digitos))
                digitos = "0000";

            int parcelas = LerInteiro("Número de parcelas (1 a 6): ");
            if (parcelas < 1 || parcelas > 6)
                parcelas = 1;

            pagamento = new PagamentoCartao($"****-****-****-{digitos}", parcelas);
        }
        else
        {
            Console.WriteLine("Forma de pagamento inválida.");
            return;
        }

        pedidoAtual.FinalizarPedido(pagamento);
        // [REQUISITO: Dependência - GeradorRecibo recebe pedido apenas durante a execução do método]
        GeradorRecibo.ImprimirRecibo(pedidoAtual);

        // Libera o entregador após concluir a simulação de entrega
        if (pedidoAtual.Entregador != null)
            pedidoAtual.Entregador.Disponivel = true;

        pedidoAtual = null;
    }

    private static void ExibirEstatisticasGerais()
    {
        Console.WriteLine("\n============== ESTATÍSTICAS GERAIS ==============");
        Console.WriteLine($"Taxa de Entrega Base (estática) : R$ {Pedido.TaxaEntregaBase:F2}");
        Console.WriteLine($"Total de Pedidos Realizados     : {Pedido.GetTotalPedidosRealizados()}");
        Console.WriteLine($"Faturamento Total Acumulado     : R$ {Pedido.GetFaturamentoTotal():F2}");
        Console.WriteLine($"Exemplo de Frete p/ 5.0 km      : R$ {Pedido.CalcularTaxaPorDistancia(5.0):F2}");
        Console.WriteLine("=================================================\n");
    }

    private static bool ValidarPedidoAberto()
    {
        if (pedidoAtual == null || pedidoAtual.Finalizado)
        {
            Console.WriteLine("Nenhum pedido em aberto. Crie um novo pedido na opção 2 primeiro.");
            return false;
        }
        return true;
    }

    private static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string entrada = (Console.ReadLine() ?? string.Empty).Trim();
            if (int.TryParse(entrada, out int valor))
                return valor;

            Console.WriteLine("Entrada inválida. Digite um número inteiro.");
        }
    }

    private static double LerDouble(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string entrada = (Console.ReadLine() ?? string.Empty).Trim().Replace(',', '.');
            if (double.TryParse(entrada, NumberStyles.Any, CultureInfo.InvariantCulture, out double valor))
                return valor;

            Console.WriteLine("Entrada inválida. Digite um número decimal (ex: 4.5).");
        }
    }
}
