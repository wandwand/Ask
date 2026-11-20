# Sistema de Pedidos de Restaurante / Delivery (C# / .NET)

Projeto de console em **C# (.NET 10)** desenvolvido para demonstrar os pilares de **Programação Orientada a Objetos (POO)**: abstração, encapsulamento, herança, polimorfismo, associações (composição, agregação, associação simples e dependência) e membros estáticos.

---

## Estrutura de Pastas e Namespaces

```text
SistemaDelivery.csproj
Program.cs                         (namespace SistemaDelivery)
├── Interfaces/
│   ├── IMetodoPagamento.cs        (namespace SistemaDelivery.Interfaces)
│   └── IPreparavel.cs             (namespace SistemaDelivery.Interfaces)
├── Models/
│   ├── Bebida.cs                  (namespace SistemaDelivery.Models)
│   ├── Cardapio.cs                (namespace SistemaDelivery.Models)
│   ├── Cliente.cs                 (namespace SistemaDelivery.Models)
│   ├── Entregador.cs              (namespace SistemaDelivery.Models)
│   ├── ItemCardapio.cs            (namespace SistemaDelivery.Models)
│   ├── ItemPedido.cs              (namespace SistemaDelivery.Models)
│   ├── Lanche.cs                  (namespace SistemaDelivery.Models)
│   ├── PagamentoCartao.cs         (namespace SistemaDelivery.Models)
│   ├── PagamentoPix.cs            (namespace SistemaDelivery.Models)
│   ├── Pedido.cs                  (namespace SistemaDelivery.Models)
│   └── Sobremesa.cs               (namespace SistemaDelivery.Models)
└── Services/
    ├── GeradorRecibo.cs           (namespace SistemaDelivery.Services)
    └── RestauranteService.cs      (namespace SistemaDelivery.Services)
```

---

## Como Compilar e Executar via Terminal

A partir da raiz do projeto (`C:\git\Ask`):

### 1. Compilar
```powershell
dotnet build
```

### 2. Executar
```powershell
dotnet run
```

---

## Resumo para Diagrama de Classes UML

### 1. Interfaces e Classes Abstratas
- **`<<interface>> IMetodoPagamento`** (`SistemaDelivery.Interfaces`)
  - `+ ProcessarPagamento(valor: double): double`
  - `+ GetNomeMetodo(): string`
- **`<<interface>> IPreparavel`** (`SistemaDelivery.Interfaces`)
  - `+ GerarInstrucoesCozinha(): string`
- **`<<abstract>> ItemCardapio`** (`SistemaDelivery.Models`)
  - `# codigo: string`
  - `# nome: string`
  - `# precoBase: double`
  - `# descricao: string`
  - `+ CalcularPrecoFinal(): double` *(abstract)*
  - `+ CalcularTempoPreparo(): int` *(abstract)*
  - `+ ExibirDetalhes(): string` *(abstract)*

### 2. Subclasses (Herança e Implementação de Interfaces)
- **`Lanche`** `--|>` `ItemCardapio` *(herança)* e `..|>` `IPreparavel` *(implementação)*
  - `- ingredientes: List<string>`
  - `- adicionalBordaOuBacon: bool`
  - `- tempoBasePreparoMinutos: int`
- **`Bebida`** `--|>` `ItemCardapio` *(herança)*
  - `- volumeMl: int`
  - `- isAlcoolica: bool`
  - `- embalagemTermica: bool`
- **`Sobremesa`** `--|>` `ItemCardapio` *(herança)* e `..|>` `IPreparavel` *(implementação)*
  - `- contemLactose: bool`
  - `- temperaturaServir: string`
  - `- porcaoGramas: int`
- **`PagamentoPix`** `..|>` `IMetodoPagamento` *(implementação)*
  - `- chavePix: string`
- **`PagamentoCartao`** `..|>` `IMetodoPagamento` *(implementação)*
  - `- numeroCartaoMascarado: string`
  - `- parcelas: int`

### 3. Entidades e Regras de Negócio
- **`Cliente`** (`SistemaDelivery.Models`)
  - `- id: int`, `- nome: string`, `- endereco: string`, `- telefone: string`
- **`Entregador`** (`SistemaDelivery.Models`)
  - `- id: int`, `- nome: string`, `- placaMoto: string`, `- disponivel: bool`
- **`ItemPedido`** (`SistemaDelivery.Models`)
  - `- item: ItemCardapio`, `- quantidade: int`, `- observacao: string`
  - Construtor `internal`: instanciado exclusivamente por `Pedido` dentro do assembly.
- **`Pedido`** (`SistemaDelivery.Models`)
  - `+ TaxaEntregaBase: double` *(static)*
  - `- totalPedidosRealizados: int` *(static)*
  - `- faturamentoTotal: double` *(static)*
  - `+ CalcularTaxaPorDistancia(km: double): double` *(static)*
  - `+ GetTotalPedidosRealizados(): int` *(static)*
  - `+ GetFaturamentoTotal(): double` *(static)*
  - `+ AdicionarItem(item: ItemCardapio, quantidade: int, observacao: string): void`
  - `+ AtribuirEntregador(entregador: Entregador, distanciaKm: double): void`
  - `+ FinalizarPedido(pagamento: IMetodoPagamento): double`
- **`Cardapio`** (`SistemaDelivery.Models`)
  - `- nomeRestaurante: string`, `- itens: List<ItemCardapio>`
- **`GeradorRecibo`** (`SistemaDelivery.Services`)
  - `+ ImprimirRecibo(pedido: Pedido): void`
- **`RestauranteService`** (`SistemaDelivery.Services`)
  - Gerencia `Cardapio`, `Cliente`s, `Entregador`es e `Pedido`s pré-cadastrados.

### 4. Relacionamentos UML (Associações)
1. **Herança (`--|>`):**
   - `Lanche`, `Bebida` e `Sobremesa` herdam de `ItemCardapio`.
2. **Realização / Implementação (`..|>`):**
   - `PagamentoPix` e `PagamentoCartao` implementam `IMetodoPagamento`.
   - `Lanche` e `Sobremesa` implementam `IPreparavel`.
3. **Composição (`*--`):**
   - `Pedido` `1` `*--` `1..*` `ItemPedido`: `ItemPedido` é instanciado internamente no método `Pedido.AdicionarItem(...)` e não existe sem um `Pedido`.
4. **Agregação (`o--`):**
   - `Cardapio` `1` `o--` `0..*` `ItemCardapio`: os itens são instanciados externamente e adicionados ao `Cardapio`.
5. **Associação Simples (`-->`):**
   - `Pedido` `0..*` `-->` `1` `Cliente`
   - `Pedido` `0..*` `-->` `0..1` `Entregador`
   - `ItemPedido` `0..*` `-->` `1` `ItemCardapio`
6. **Dependência (`..>`):**
   - `Pedido` `..>` `IMetodoPagamento` (no método `FinalizarPedido(IMetodoPagamento)`).
   - `GeradorRecibo` `..>` `Pedido` e `IPreparavel` (no método `ImprimirRecibo(Pedido)`).
