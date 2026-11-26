# Manual de Execução — Sistema de Pedidos de Restaurante / Delivery

Aplicação de POO com base no projeto final da disciplina de programação orientada a objetos, foi desenvolvida em **C# (.NET 10)** para gerenciamento de pedidos de restaurante e delivery, com a intenção de criar uma aplciação com os requisitos 2) do trabalho final.

---

## 1. Pré-requisitos

- **.NET 10.0 SDK** instalado (`net10.0`, conforme configurado em `SistemaDelivery.csproj`).
- Terminal de linha de comando (PowerShell, CMD, Bash ou terminal integrado do VS Code / Visual Studio).

Para conferir se o .NET SDK está instalado, execute:

```powershell
dotnet --version
```

---

## 2. Como Compilar e Executar

## Opção 1
1. Abra o terminal na pasta raiz do projeto (onde está o arquivo `SistemaDelivery.csproj`):

   ```powershell
   cd C:\git\Ask
   ```

2. Compile o projeto para verificar dependências e build:

   ```powershell
   dotnet build
   ```

3. Execute a aplicação:

   ```powershell
   dotnet run
   ```
## Opção 2
Clique no projeto  `SistemaDelivery.csproj`— Projeto de c#.
---

## 3. Como Utilizar o Sistema (Passo a Passo)

Ao iniciar, o sistema carrega automaticamente um cardápio, clientes e entregadores pré-cadastrados e exibe o menu principal no terminal:

1. **Opção `1` — Listar Cardápio Completo:**
   - Exibe todos os itens disponíveis (`Lanche`, `Bebida` e `Sobremesa`) com código, ingredientes/detalhes, tempo estimado de preparo e preço final calculado.
2. **Opção `2` — Criar Novo Pedido e Adicionar Itens:**
   - Lista os clientes cadastrados e solicita o **ID do cliente**.
   - Em seguida, solicite os códigos dos itens do cardápio (ex.: `L01`, `B01`, `S01`), a **quantidade** e uma **observação** opcional.
   - Caso já exista um pedido em aberto, o sistema permite adicionar novos itens a ele.
3. **Opção `3` — Atribuir Entregador e Calcular Frete:**
   - Lista os entregadores disponíveis e solicita o **ID do entregador** e a **distância da entrega (em km)** para calcular a taxa de entrega.
4. **Opção `4` — Finalizar Pedido (Pagamento + Recibo):**
   - Exibe o resumo do pedido em aberto e solicita a forma de pagamento:
     - **Pix** (aplica 5% de desconto sobre o valor total); ou
     - **Cartão de Crédito** (permite informar parcelas e aplica taxa da maquininha).
   - Conclui o pedido, libera o entregador e imprime o **cupom fiscal / recibo** completo no terminal (incluindo instruções de preparo para a cozinha).
5. **Opção `5` — Exibir Estatísticas Gerais:**
   - Mostra o total de pedidos finalizados e o faturamento acumulado na execução atual.
6. **Opção `0` — Sair:**
   - Encerra a aplicação.

---

## 4. Estrutura do Projeto

- `SistemaDelivery.csproj`— Projeto de c#.
- `Program.cs` — Ponto de entrada e menu interativo no console.
- `Cardapios/` — `ItemCardapio`, `Lanche`, `Bebida`, `Sobremesa`, `Cardapio`, `IPreparavel`.
- `Pedidos/` — `Pedido`, `ItemPedido`, `GeradorRecibo`.
- `Pagamentos/` — `IMetodoPagamento`, `PagamentoPix`, `PagamentoCartao`.
- `Clientes/` — `Cliente`.
- `Entregas/` — `Entregador`.
- `Services/` — `RestauranteService`.
