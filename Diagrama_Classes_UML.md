# Diagrama de Classes UML — Sistema de Pedidos / Delivery

Diagrama fiel ao código-fonte atual em `C:\git\Ask` (organizado por *Feature Folders* e namespaces `SistemaDelivery.*`).

```mermaid
classDiagram
    direction TB

    namespace SistemaDelivery.Cardapios {
        class IPreparavel {
            <<interface>>
            +GerarInstrucoesCozinha() string
        }

        class ItemCardapio {
            <<abstract>>
            #codigo : string
            #nome : string
            #precoBase : double
            #descricao : string
            #ItemCardapio(codigo, nome, precoBase, descricao)
            +CalcularPrecoFinal()* double
            +CalcularTempoPreparo()* int
            +ExibirDetalhes()* string
            +Codigo : string
            +Nome : string
            +PrecoBase : double
            +Descricao : string
        }

        class Lanche {
            -ingredientes : List~string~
            -adicionalBordaOuBacon : bool
            -tempoBasePreparoMinutos : int
            -ValorAdicional : double$
            +Lanche(...)
            +CalcularPrecoFinal() double
            +CalcularTempoPreparo() int
            +ExibirDetalhes() string
            +GerarInstrucoesCozinha() string
            +Ingredientes : IReadOnlyList~string~
            +AdicionalBordaOuBacon : bool
            +TempoBasePreparoMinutos : int
        }

        class Bebida {
            -volumeMl : int
            -isAlcoolica : bool
            -embalagemTermica : bool
            -TaxaEmbalagemTermica : double$
            +Bebida(...)
            +CalcularPrecoFinal() double
            +CalcularTempoPreparo() int
            +ExibirDetalhes() string
            +VolumeMl : int
            +IsAlcoolica : bool
            +EmbalagemTermica : bool
        }

        class Sobremesa {
            -contemLactose : bool
            -temperaturaServir : string
            -porcaoGramas : int
            -TaxaEmbalagemEspecial : double$
            +Sobremesa(...)
            +CalcularPrecoFinal() double
            +CalcularTempoPreparo() int
            +ExibirDetalhes() string
            +GerarInstrucoesCozinha() string
            +ContemLactose : bool
            +TemperaturaServir : string
            +PorcaoGramas : int
        }

        class Cardapio {
            -nomeRestaurante : string
            -itens : List~ItemCardapio~
            +Cardapio(nomeRestaurante)
            +AdicionarItem(item : ItemCardapio) void
            +BuscarPorCodigo(codigo : string) ItemCardapio
            +Itens : IReadOnlyList~ItemCardapio~
            +NomeRestaurante : string
        }
    }

    namespace SistemaDelivery.Pagamentos {
        class IMetodoPagamento {
            <<interface>>
            +ProcessarPagamento(valor : double) double
            +GetNomeMetodo() string
        }

        class PagamentoPix {
            -DescontoPix : double$
            -chavePix : string
            +PagamentoPix(chavePix)
            +ProcessarPagamento(valor : double) double
            +GetNomeMetodo() string
            +ChavePix : string
        }

        class PagamentoCartao {
            -TaxaBaseMaquininha : double$
            -numeroCartaoMascarado : string
            -parcelas : int
            +PagamentoCartao(numeroCartaoMascarado, parcelas)
            +ProcessarPagamento(valor : double) double
            +GetNomeMetodo() string
            +NumeroCartaoMascarado : string
            +Parcelas : int
        }
    }

    namespace SistemaDelivery.Clientes {
        class Cliente {
            -id : int
            -nome : string
            -endereco : string
            -telefone : string
            +Cliente(id, nome, endereco, telefone)
            +Id : int
            +Nome : string
            +Endereco : string
            +Telefone : string
            +ToString() string
        }
    }

    namespace SistemaDelivery.Entregas {
        class Entregador {
            -id : int
            -nome : string
            -placaMoto : string
            -disponivel : bool
            +Entregador(id, nome, placaMoto, disponivel)
            +Id : int
            +Nome : string
            +PlacaMoto : string
            +Disponivel : bool
            +ToString() string
        }
    }

    namespace SistemaDelivery.Pedidos {
        class ItemPedido {
            -item : ItemCardapio
            -quantidade : int
            -observacao : string
            ~ItemPedido(item, quantidade, observacao)
            +GetSubtotal() double
            +GetTempoPreparoTotal() int
            +Item : ItemCardapio
            +Quantidade : int
            +Observacao : string
        }

        class Pedido {
            +TaxaEntregaBase : double$
            -totalPedidosRealizados : int$
            -faturamentoTotal : double$
            -proximoNumeroPedido : int$
            -numeroPedido : int
            -cliente : Cliente
            -entregador : Entregador
            -itens : List~ItemPedido~
            -distanciaKm : double
            -taxaEntrega : double
            -valorFinalPago : double
            -finalizado : bool
            -metodoPagamentoUtilizado : string
            +Pedido(cliente : Cliente)
            +CalcularTaxaPorDistancia(km : double)$ double
            +GetTotalPedidosRealizados()$ int
            +GetFaturamentoTotal()$ double
            +AdicionarItem(item, quantidade, observacao) void
            +AtribuirEntregador(entregador, distanciaKm) void
            +CalcularSubtotalItens() double
            +CalcularTotalComEntrega() double
            +CalcularTempoEstimadoPreparo() int
            +FinalizarPedido(pagamento : IMetodoPagamento) double
        }

        class GeradorRecibo {
            +ImprimirRecibo(pedido : Pedido) void
        }
    }

    namespace SistemaDelivery.Services {
        class RestauranteService {
            -cardapio : Cardapio
            -clientes : List~Cliente~
            -entregadores : List~Entregador~
            -pedidos : List~Pedido~
            +RestauranteService()
            +ListarCardapioPolimorfico() void
            +BuscarClientePorId(id : int) Cliente
            +BuscarEntregadorPorId(id : int) Entregador
            +RegistrarPedido(pedido : Pedido) void
        }
    }

    %% Heranças
    ItemCardapio <|-- Lanche
    ItemCardapio <|-- Bebida
    ItemCardapio <|-- Sobremesa

    %% Realização de Interfaces
    IPreparavel <|.. Lanche
    IPreparavel <|.. Sobremesa
    IMetodoPagamento <|.. PagamentoPix
    IMetodoPagamento <|.. PagamentoCartao

    %% Agregação
    Cardapio "1" o-- "0..*" ItemCardapio : agrega

    %% Composição
    Pedido "1" *-- "1..*" ItemPedido : compõe

    %% Associações Simples
    Pedido "0..*" --> "1" Cliente : cliente
    Pedido "0..*" --> "0..1" Entregador : entregador
    ItemPedido "0..*" --> "1" ItemCardapio : item
    RestauranteService "1" --> "1" Cardapio : gerencia
    RestauranteService "1" --> "0..*" Cliente : gerencia
    RestauranteService "1" --> "0..*" Entregador : gerencia
    RestauranteService "1" --> "0..*" Pedido : registra

    %% Dependências
    Pedido ..> IMetodoPagamento : usa em FinalizarPedido()
    GeradorRecibo ..> Pedido : imprime
    GeradorRecibo ..> IPreparavel : instrui cozinha
```

---

## Legenda dos Relacionamentos e Requisitos de POO

| Símbolo UML | Relacionamento | Onde ocorre no código |
| :---: | :--- | :--- |
| `<\|--` | **Herança** | `Lanche`, `Bebida` e `Sobremesa` herdam de `ItemCardapio` |
| `<\|..` | **Realização (Interface)** | `Lanche` e `Sobremesa` implementam `IPreparavel`; `PagamentoPix` e `PagamentoCartao` implementam `IMetodoPagamento` |
| `*--` | **Composição** | `Pedido` cria e gerencia `ItemPedido` internamente (`construtor internal`) |
| `o--` | **Agregação** | `Cardapio` agrega instâncias de `ItemCardapio` criadas externamente |
| `-->` | **Associação Simples** | `Pedido` referencia `Cliente` e `Entregador`; `ItemPedido` referencia `ItemCardapio`; `RestauranteService` gerencia entidades |
| `..>` | **Dependência** | `Pedido.FinalizarPedido(IMetodoPagamento)` e `GeradorRecibo.ImprimirRecibo(Pedido)` |
