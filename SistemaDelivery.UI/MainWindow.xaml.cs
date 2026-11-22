using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SistemaDelivery.Cardapios;
using SistemaDelivery.Clientes;
using SistemaDelivery.Entregas;
using SistemaDelivery.Pagamentos;
using SistemaDelivery.Pedidos;
using SistemaDelivery.Services;

namespace SistemaDelivery.UI;

public partial class MainWindow : Window
{
    private readonly RestauranteService service = new();
    private readonly GeradorRecibo geradorRecibo = new();
    private Pedido? pedidoAtual;

    public MainWindow()
    {
        InitializeComponent();
        TxtObservacao.TextChanged += (_, _) =>
            LblObsPlaceholder.Visibility = string.IsNullOrEmpty(TxtObservacao.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

        CarregarDadosIniciais();
        AtualizarInterface();
    }

    private void CarregarDadosIniciais()
    {
        TxtNomeRestaurante.Text = service.Cardapio.NomeRestaurante;

        var linhasCardapio = service.Cardapio.Itens.Select(item => new CardapioRow
        {
            Item = item,
            Codigo = item.Codigo,
            Categoria = item.GetType().Name,
            Nome = item.Nome,
            PrecoBaseFormatado = $"R$ {item.PrecoBase:F2}",
            PrecoFinalFormatado = $"R$ {item.CalcularPrecoFinal():F2}",
            TempoPreparoFormatado = $"{item.CalcularTempoPreparo()} min",
            Detalhes = item.ExibirDetalhes(),
            InstrucoesCozinha = item is IPreparavel prep
                ? prep.GerarInstrucoesCozinha()
                : "Separação direta no balcão"
        }).ToList();

        GridCardapio.ItemsSource = linhasCardapio;

        CmbClientes.ItemsSource = service.Clientes;
        if (service.Clientes.Count > 0)
            CmbClientes.SelectedIndex = 0;

        CmbItensCardapio.ItemsSource = service.Cardapio.Itens
            .Select(i => new ComboItemCardapio(i))
            .ToList();
        if (service.Cardapio.Itens.Count > 0)
            CmbItensCardapio.SelectedIndex = 0;

        AtualizarComboEntregadores();
    }

    private void AtualizarComboEntregadores()
    {
        int selectedIndex = CmbEntregadores.SelectedIndex;
        CmbEntregadores.ItemsSource = service.Entregadores.ToList();
        if (service.Entregadores.Count > 0)
            CmbEntregadores.SelectedIndex = selectedIndex >= 0 && selectedIndex < service.Entregadores.Count
                ? selectedIndex
                : 0;
    }

    private void AtualizarInterface()
    {
        if (pedidoAtual != null && !pedidoAtual.Finalizado)
        {
            TxtPedidoAtualStatus.Text = $"Pedido em andamento: #{pedidoAtual.NumeroPedido:D4} | Cliente: {pedidoAtual.Cliente.Nome} | Itens: {pedidoAtual.Itens.Count}";
            GridItensPedido.ItemsSource = pedidoAtual.Itens.Select(ip => new ItemPedidoRow
            {
                ItemPedido = ip,
                Quantidade = ip.Quantidade,
                NomeItem = ip.Item.Nome,
                PrecoUnitarioFormatado = $"R$ {ip.Item.CalcularPrecoFinal():F2}",
                SubtotalFormatado = $"R$ {ip.GetSubtotal():F2}",
                Observacao = ip.Observacao
            }).ToList();

            TxtSubtotalItens.Text = $"R$ {pedidoAtual.CalcularSubtotalItens():F2}";
            TxtTaxaEntrega.Text = $"R$ {pedidoAtual.TaxaEntrega:F2}";
            TxtTempoPreparo.Text = $"{pedidoAtual.CalcularTempoTotalPreparo()} min";
            TxtTotalBruto.Text = $"R$ {pedidoAtual.CalcularTotalComFrete():F2}";

            TxtEntregadorAtribuido.Text = pedidoAtual.Entregador != null
                ? $"Entregador: {pedidoAtual.Entregador.Nome} ({pedidoAtual.DistanciaKm:F1} km)"
                : "Entregador: Não atribuído (taxa base)";
        }
        else
        {
            TxtPedidoAtualStatus.Text = "Nenhum pedido aberto no momento";
            GridItensPedido.ItemsSource = null;
            TxtSubtotalItens.Text = "R$ 0,00";
            TxtTaxaEntrega.Text = "R$ 0,00";
            TxtTempoPreparo.Text = "0 min";
            TxtTotalBruto.Text = "R$ 0,00";
            TxtEntregadorAtribuido.Text = "Entregador: Não atribuído";
        }

        // Estatísticas estáticas
        int totalPedidos = Pedido.GetTotalPedidosRealizados();
        double faturamento = Pedido.GetFaturamentoTotal();
        TxtResumoEstatisticas.Text = $"Pedidos: {totalPedidos} | Faturamento: R$ {faturamento:F2}";
        TxtTaxaBase.Text = $"Taxa Base: R$ {Pedido.TaxaEntregaBase:F2}";

        TxtStatTaxaBase.Text = $"Taxa Entrega Base: R$ {Pedido.TaxaEntregaBase:F2}";
        TxtStatTotalPedidos.Text = $"Pedidos Realizados: {totalPedidos}";
        TxtStatFaturamento.Text = $"Faturamento Total: R$ {faturamento:F2}";
        TxtStatFrete5Km.Text = $"Simulação Frete 5 km: R$ {Pedido.CalcularTaxaPorDistancia(5.0):F2}";

        LstHistoricoPedidos.ItemsSource = service.Pedidos
            .Select(p => new HistoricoPedidoItem(p))
            .ToList();
    }

    private void BtnIrParaPedidoComItem_Click(object sender, RoutedEventArgs e)
    {
        if (GridCardapio.SelectedItem is CardapioRow row)
        {
            var comboList = CmbItensCardapio.ItemsSource as List<ComboItemCardapio>;
            var match = comboList?.FirstOrDefault(c => c.Item.Codigo == row.Codigo);
            if (match != null)
                CmbItensCardapio.SelectedItem = match;
        }
        MainTabs.SelectedIndex = 1;
    }

    private void BtnCriarPedido_Click(object sender, RoutedEventArgs e)
    {
        if (CmbClientes.SelectedItem is not Cliente cliente)
        {
            MessageBox.Show("Selecione um cliente para criar o pedido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (pedidoAtual != null && !pedidoAtual.Finalizado)
        {
            var confirmar = MessageBox.Show(
                $"Já existe um pedido em aberto (#{pedidoAtual.NumeroPedido:D4}). Deseja descartar o controle atual e iniciar um novo?",
                "Pedido em andamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar != MessageBoxResult.Yes)
                return;

            if (pedidoAtual.Entregador != null)
                pedidoAtual.Entregador.Disponivel = true;
        }

        pedidoAtual = new Pedido(cliente);
        service.RegistrarPedido(pedidoAtual);
        AtualizarComboEntregadores();
        AtualizarInterface();
        TxtStatusBar.Text = $"Pedido #{pedidoAtual.NumeroPedido:D4} criado para {cliente.Nome}.";
    }

    private void BtnAdicionarItem_Click(object sender, RoutedEventArgs e)
    {
        if (pedidoAtual == null || pedidoAtual.Finalizado)
        {
            MessageBox.Show("Crie um novo pedido primeiro antes de adicionar itens.", "Nenhum pedido aberto", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (CmbItensCardapio.SelectedItem is not ComboItemCardapio selecionado)
        {
            MessageBox.Show("Selecione um item do cardápio.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(TxtQuantidade.Text.Trim(), out int quantidade) || quantidade <= 0)
        {
            MessageBox.Show("Informe uma quantidade inteira maior que zero.", "Quantidade inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        pedidoAtual.AdicionarItem(selecionado.Item, quantidade, TxtObservacao.Text);
        TxtObservacao.Clear();
        TxtQuantidade.Text = "1";
        AtualizarInterface();
        TxtStatusBar.Text = $"Adicionado {quantidade}x {selecionado.Item.Nome} ao pedido #{pedidoAtual.NumeroPedido:D4}.";
    }

    private void BtnAtribuirEntregador_Click(object sender, RoutedEventArgs e)
    {
        if (pedidoAtual == null || pedidoAtual.Finalizado)
        {
            MessageBox.Show("Crie um novo pedido primeiro.", "Nenhum pedido aberto", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (CmbEntregadores.SelectedItem is not Entregador entregador)
        {
            MessageBox.Show("Selecione um entregador.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string distTexto = TxtDistanciaKm.Text.Trim().Replace(',', '.');
        if (!double.TryParse(distTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out double distanciaKm) || distanciaKm < 0)
        {
            MessageBox.Show("Informe uma distância válida em km (ex: 3.5).", "Distância inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            pedidoAtual.AtribuirEntregador(entregador, distanciaKm);
            AtualizarComboEntregadores();
            AtualizarInterface();
            TxtStatusBar.Text = $"Entregador {entregador.Nome} atribuído! Frete: R$ {pedidoAtual.TaxaEntrega:F2}.";
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Entregador indisponível", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void PagamentoMetodo_Changed(object sender, RoutedEventArgs e)
    {
        if (PanelPix == null || PanelCartao == null)
            return;

        bool isPix = RbPix.IsChecked == true;
        PanelPix.Visibility = isPix ? Visibility.Visible : Visibility.Collapsed;
        PanelCartao.Visibility = isPix ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnFinalizarPedido_Click(object sender, RoutedEventArgs e)
    {
        if (pedidoAtual == null || pedidoAtual.Finalizado)
        {
            MessageBox.Show("Nenhum pedido em aberto para finalizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (pedidoAtual.Itens.Count == 0)
        {
            MessageBox.Show("Adicione pelo menos um item ao pedido antes de finalizar.", "Pedido vazio", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IMetodoPagamento pagamento;
        if (RbPix.IsChecked == true)
        {
            string chave = string.IsNullOrWhiteSpace(TxtChavePix.Text) ? "restaurante@pix.com.br" : TxtChavePix.Text.Trim();
            pagamento = new PagamentoPix(chave);
        }
        else
        {
            string digitos = string.IsNullOrWhiteSpace(TxtDigitosCartao.Text) ? "0000" : TxtDigitosCartao.Text.Trim();
            int parcelas = CmbParcelas.SelectedIndex + 1;
            pagamento = new PagamentoCartao($"****-****-****-{digitos}", parcelas);
        }

        double valorPago = pedidoAtual.FinalizarPedido(pagamento);
        if (pedidoAtual.Entregador != null)
            pedidoAtual.Entregador.Disponivel = true;

        Pedido pedidoFinalizado = pedidoAtual;
        pedidoAtual = null;

        TxtRecibo.Text = geradorRecibo.GerarReciboTexto(pedidoFinalizado);
        AtualizarComboEntregadores();
        AtualizarInterface();

        TxtStatusBar.Text = $"Pedido #{pedidoFinalizado.NumeroPedido:D4} finalizado! Valor pago: R$ {valorPago:F2}.";
        MainTabs.SelectedIndex = 2;
    }

    private void LstHistoricoPedidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LstHistoricoPedidos.SelectedItem is HistoricoPedidoItem item)
        {
            TxtRecibo.Text = geradorRecibo.GerarReciboTexto(item.Pedido);
        }
    }

    private sealed class CardapioRow
    {
        public required ItemCardapio Item { get; init; }
        public required string Codigo { get; init; }
        public required string Categoria { get; init; }
        public required string Nome { get; init; }
        public required string PrecoBaseFormatado { get; init; }
        public required string PrecoFinalFormatado { get; init; }
        public required string TempoPreparoFormatado { get; init; }
        public required string Detalhes { get; init; }
        public required string InstrucoesCozinha { get; init; }
    }

    private sealed class ComboItemCardapio(ItemCardapio item)
    {
        public ItemCardapio Item { get; } = item;
        public override string ToString() => $"[{Item.Codigo}] {Item.Nome} - R$ {Item.CalcularPrecoFinal():F2}";
    }

    private void GridItensPedido_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DependencyObject? dep = (DependencyObject)e.OriginalSource;
        while (dep != null && dep is not DataGridRow)
            dep = VisualTreeHelper.GetParent(dep);

        if (dep is DataGridRow row)
            row.IsSelected = true;
    }

    private void BtnRemoverItemPedido_Click(object sender, RoutedEventArgs e)
    {
        if (pedidoAtual == null || pedidoAtual.Finalizado)
            return;

        if (GridItensPedido.SelectedItem is not ItemPedidoRow row)
        {
            MessageBox.Show("Selecione um item do pedido para excluir.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (pedidoAtual.RemoverItem(row.ItemPedido))
        {
            AtualizarInterface();
            TxtStatusBar.Text = $"Item \"{row.NomeItem}\" removido do pedido #{pedidoAtual.NumeroPedido:D4}.";
        }
    }

    private sealed class ItemPedidoRow
    {
        public required ItemPedido ItemPedido { get; init; }
        public int Quantidade { get; init; }
        public required string NomeItem { get; init; }
        public required string PrecoUnitarioFormatado { get; init; }
        public required string SubtotalFormatado { get; init; }
        public required string Observacao { get; init; }
    }

    private sealed class HistoricoPedidoItem(Pedido pedido)
    {
        public Pedido Pedido { get; } = pedido;
        public override string ToString()
        {
            string status = Pedido.Finalizado ? $"Pago R$ {Pedido.ValorFinalPago:F2}" : "Em aberto";
            return $"#{Pedido.NumeroPedido:D4} - {Pedido.Cliente.Nome} ({status})";
        }
    }
}
