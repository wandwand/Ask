using SistemaDelivery.Cardapios;

namespace SistemaDelivery.Pedidos;

//  Dependência - Recebe Pedido como parâmetro apenas durante a execução de ImprimirRecibo
public class GeradorRecibo
{
    public void ImprimirRecibo(Pedido pedido)
    {
        if (pedido == null)
        {
            Console.WriteLine("Pedido inválido para geração de recibo.");
            return;
        }

        Console.WriteLine("\n==================================================");
        Console.WriteLine("              CUPOM FISCAL - DELIVERY             ");
        Console.WriteLine("==================================================");
        Console.WriteLine($"Pedido Nº: #{pedido.NumeroPedido:D4}");
        Console.WriteLine($"Cliente  : {pedido.Cliente.Nome}");
        Console.WriteLine($"Endereço : {pedido.Cliente.Endereco}");
        Console.WriteLine($"Telefone : {pedido.Cliente.Telefone}");

        if (pedido.Entregador != null)
        {
            Console.WriteLine($"Entregador: {pedido.Entregador.Nome} (Moto: {pedido.Entregador.PlacaMoto} | Distância: {pedido.DistanciaKm:F1} km)");
        }
        else
        {
            Console.WriteLine("Entregador: Retirada no balcão / Não atribuído");
        }

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ITENS DO PEDIDO:");
        foreach (var itemPedido in pedido.Itens)
        {
            ItemCardapio item = itemPedido.Item;
            Console.WriteLine($"  {itemPedido.Quantidade}x {item.Nome,-25} R$ {itemPedido.GetSubtotal(),7:F2}");
            Console.WriteLine($"     Obs: {itemPedido.Observacao}");
        }

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Subtotal dos Itens : R$ {pedido.CalcularSubtotalItens(),8:F2}");
        Console.WriteLine($"Taxa de Entrega    : R$ {pedido.TaxaEntrega,8:F2}");
        Console.WriteLine($"Total Bruto        : R$ {pedido.CalcularTotalComFrete(),8:F2}");
        Console.WriteLine($"Tempo Est. Preparo : {pedido.CalcularTempoTotalPreparo()} minutos");

        if (pedido.Finalizado)
        {
            Console.WriteLine($"Forma de Pagamento : {pedido.MetodoPagamentoUtilizado}");
            Console.WriteLine($"VALOR FINAL PAGO   : R$ {pedido.ValorFinalPago,8:F2}");
        }
        else
        {
            Console.WriteLine("Status             : AGUARDANDO PAGAMENTO");
        }

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("COMANDA DA COZINHA:");
        foreach (var itemPedido in pedido.Itens)
        {
            ItemCardapio item = itemPedido.Item;
            //  Polimorfismo 3 - Chamada via referência da interface IPreparavel
            if (item is IPreparavel preparavel)
            {
                Console.WriteLine($"  [{itemPedido.Quantidade}x] {preparavel.GerarInstrucoesCozinha()}");
            }
            else
            {
                Console.WriteLine($"  [{itemPedido.Quantidade}x] BALCÃO -> Separar {item.Nome} (não vai para a cozinha).");
            }
        }
        Console.WriteLine("==================================================\n");
    }
}
