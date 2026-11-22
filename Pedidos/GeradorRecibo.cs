using System.Text;
using SistemaDelivery.Cardapios;

namespace SistemaDelivery.Pedidos;

// [REQUISITO: Dependência - Recebe Pedido como parâmetro apenas durante a execução de ImprimirRecibo]
public class GeradorRecibo
{
    public void ImprimirRecibo(Pedido pedido)
    {
        if (pedido == null)
        {
            Console.WriteLine("Pedido inválido para geração de recibo.");
            return;
        }

        Console.WriteLine(GerarReciboTexto(pedido));
    }

    public string GerarReciboTexto(Pedido pedido)
    {
        if (pedido == null)
            return "Pedido inválido para geração de recibo.";

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("==================================================");
        sb.AppendLine("              CUPOM FISCAL - DELIVERY             ");
        sb.AppendLine("==================================================");
        sb.AppendLine($"Pedido Nº: #{pedido.NumeroPedido:D4}");
        sb.AppendLine($"Cliente  : {pedido.Cliente.Nome}");
        sb.AppendLine($"Endereço : {pedido.Cliente.Endereco}");
        sb.AppendLine($"Telefone : {pedido.Cliente.Telefone}");

        if (pedido.Entregador != null)
        {
            sb.AppendLine($"Entregador: {pedido.Entregador.Nome} (Moto: {pedido.Entregador.PlacaMoto} | Distância: {pedido.DistanciaKm:F1} km)");
        }
        else
        {
            sb.AppendLine("Entregador: Retirada no balcão / Não atribuído");
        }

        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine("ITENS DO PEDIDO:");
        foreach (var itemPedido in pedido.Itens)
        {
            ItemCardapio item = itemPedido.Item;
            sb.AppendLine($"  {itemPedido.Quantidade}x {item.Nome,-25} R$ {itemPedido.GetSubtotal(),7:F2}");
            sb.AppendLine($"     Obs: {itemPedido.Observacao}");
        }

        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine($"Subtotal dos Itens : R$ {pedido.CalcularSubtotalItens(),8:F2}");
        sb.AppendLine($"Taxa de Entrega    : R$ {pedido.TaxaEntrega,8:F2}");
        sb.AppendLine($"Total Bruto        : R$ {pedido.CalcularTotalComFrete(),8:F2}");
        sb.AppendLine($"Tempo Est. Preparo : {pedido.CalcularTempoTotalPreparo()} minutos");

        if (pedido.Finalizado)
        {
            sb.AppendLine($"Forma de Pagamento : {pedido.MetodoPagamentoUtilizado}");
            sb.AppendLine($"VALOR FINAL PAGO   : R$ {pedido.ValorFinalPago,8:F2}");
        }
        else
        {
            sb.AppendLine("Status             : AGUARDANDO PAGAMENTO");
        }

        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine("COMANDA DA COZINHA:");
        foreach (var itemPedido in pedido.Itens)
        {
            ItemCardapio item = itemPedido.Item;
            // [REQUISITO: Polimorfismo 3 - Chamada via referência da interface IPreparavel]
            if (item is IPreparavel preparavel)
            {
                sb.AppendLine($"  [{itemPedido.Quantidade}x] {preparavel.GerarInstrucoesCozinha()}");
            }
            else
            {
                sb.AppendLine($"  [{itemPedido.Quantidade}x] BALCÃO -> Separar {item.Nome} (não vai para a cozinha).");
            }
        }
        sb.AppendLine("==================================================");
        return sb.ToString();
    }
}
