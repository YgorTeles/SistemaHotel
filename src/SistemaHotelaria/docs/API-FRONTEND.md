# Sistema de Hotelaria: especificação da API para o frontend React

Documento de referência para construir a interface web (React) do Sistema de Hotelaria.
Descreve **todas** as funcionalidades, endpoints, formatos de dados, regras de negócio
e mensagens de erro do backend.

> **Status do backend:** todos os endpoints abaixo estão implementados. O backend é só
> API (sem páginas HTML). A documentação interativa (Swagger) fica em
> `http://localhost:56879/swagger`. Se algum endpoint não responder como descrito, a
> correção deve ser feita no backend, não contornada no frontend.

---

## 1. Visão geral do sistema

Sistema para gerenciar um hotel com três áreas:

| Área | O que faz |
|---|---|
| **Hóspedes** | Cadastrar, listar, buscar por CPF, editar e remover hóspedes. |
| **Quartos** | Cadastrar, listar, buscar por número, editar e remover quartos; consultar quartos disponíveis em um período; ver se o quarto está ocupado agora. |
| **Reservas** | Criar reservas, listar, fazer check-in, fazer check-out e cancelar. |

### Ciclo de vida de uma reserva

```
             criar
               │
               ▼
        ┌─────────────┐   cancelar   ┌───────────┐
        │ Confirmada  │ ───────────► │ Cancelada │
        └─────────────┘              └───────────┘
               │ check-in
               ▼
        ┌─────────────┐
        │ EmAndamento │   (quarto fica ocupado = true)
        └─────────────┘
               │ check-out
               ▼
        ┌─────────────┐
        │ Finalizada  │   (quarto volta a ocupado = false)
        └─────────────┘
```

- **Confirmada**: reserva criada; o hóspede ainda não chegou.
- **EmAndamento**: check-in feito; o hóspede está no quarto.
- **Finalizada**: check-out feito; estado final.
- **Cancelada**: cancelada antes do check-in; estado final.

### Dois conceitos diferentes no quarto: não confundir

| Conceito | Onde aparece | Significado |
|---|---|---|
| **Ocupado** | Campo `ocupado` do quarto | Há um hóspede **dentro do quarto agora** (entre o check-in e o check-out). Muda **somente** no check-in e no check-out. |
| **Disponível para reservar** | Endpoint `GET /api/quartos/disponiveis` | O quarto **não tem reserva** (não cancelada) que se sobreponha ao período pedido. É calculado por datas. |

Um quarto pode estar `ocupado: false` hoje e mesmo assim **não estar disponível** para a
semana que vem (porque já existe reserva), e vice-versa.

---

## 2. Convenções gerais da API

### URL base
- Desenvolvimento: `http://localhost:56879` (HTTP) ou `https://localhost:56878` (HTTPS).
- Todas as rotas começam com `/api`.
- Deixe a URL base configurável no frontend (ex.: variável `VITE_API_URL`).

### CORS
O backend vai liberar a origem do servidor de desenvolvimento do React:
`http://localhost:5173` (porta padrão do Vite). Se usar outra porta, ela precisa ser
liberada no backend.

### Formato
- Requisições e respostas em **JSON** (`Content-Type: application/json`).
- Nomes de campos em **camelCase** (ex.: `valorDiaria`, `dataEntrada`).

### Tipos de dados

| Tipo | Formato no JSON | Exemplo | Observação |
|---|---|---|---|
| Id | string (GUID) | `"3f2a9c1e-8b7d-4e21-9a0f-1c2d3e4f5a6b"` | Gerado pelo backend. Nunca enviar na criação. |
| CPF | string com 11 dígitos | `"12345678901"` | Pode **enviar** com ou sem máscara (`"123.456.789-01"`); o backend remove tudo que não é dígito. **Sempre retorna só os 11 dígitos.** Aplique a máscara apenas na exibição. |
| Data (envio) | string `YYYY-MM-DD` | `"2026-11-20"` | Somente data, sem hora. |
| Data (resposta) | string ISO sem fuso | `"2026-11-20T00:00:00"` | **Trate como data pura.** Use os 10 primeiros caracteres. **Não** faça `new Date(valor)` direto, porque o fuso horário pode deslocar o dia. |
| Dinheiro | number | `250.00` | Valores em reais (BRL). Formate com `Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })`. |
| Status | string | `"EmAndamento"` | Um de: `"Confirmada"`, `"EmAndamento"`, `"Finalizada"`, `"Cancelada"`. |

### Rótulos sugeridos para status (exibição)

| Valor da API | Rótulo na tela | Cor sugerida |
|---|---|---|
| `Confirmada` | Confirmada | azul |
| `EmAndamento` | Em andamento (hospedado) | verde |
| `Finalizada` | Finalizada | cinza |
| `Cancelada` | Cancelada | vermelho |

### Erros

Todo erro de regra de negócio retorna um JSON com a mensagem pronta para exibir ao usuário:

```json
{ "mensagem": "Já existe um hóspede com esse CPF." }
```

| Código | Quando |
|---|---|
| `400 Bad Request` | Dado inválido ou regra de negócio violada (ex.: CPF com menos de 11 dígitos, check-in fora da data). |
| `404 Not Found` | O recurso não existe (hóspede, quarto ou reserva não encontrado). |
| `500 Internal Server Error` | Falha inesperada (ex.: banco fora do ar). Mostrar mensagem genérica: "Erro inesperado. Tente novamente." |

**O frontend deve sempre exibir o campo `mensagem` dos erros 400/404**, porque as mensagens já
vêm em português e prontas para o usuário. As mensagens exatas de cada endpoint estão
listadas abaixo.

---

## 3. Modelos de dados (TypeScript)

```ts
export type StatusReserva = 'Confirmada' | 'EmAndamento' | 'Finalizada' | 'Cancelada';

export interface Hospede {
  id: string;
  nome: string;
  cpf: string;        // 11 dígitos, sem máscara
  telefone: string;   // pode ser "" (opcional)
  email: string;      // pode ser "" (opcional)
}

export interface Quarto {
  id: string;
  numero: number;     // inteiro > 0, único
  tipo: string;       // ex.: "Solteiro", "Casal", "Suíte" (texto livre)
  capacidade: number; // inteiro > 0 (nº de pessoas)
  valorDiaria: number;// > 0
  ocupado: boolean;   // true = hóspede dentro do quarto agora
}

// Usado na listagem de reservas (já vem com dados do hóspede e do quarto)
export interface ReservaDetalhe {
  id: string;
  nomeHospede: string;
  cpfHospede: string;
  numeroQuarto: number;
  dataEntrada: string; // "YYYY-MM-DDT00:00:00"
  dataSaida: string;   // "YYYY-MM-DDT00:00:00"
  valorTotal: number;
  status: StatusReserva;
}

export interface ErroApi {
  mensagem: string;
}
```

---

## 4. Endpoints: Hóspedes

O hóspede é identificado nas rotas pelo **CPF** (não pelo id).

### 4.1 Listar hóspedes
`GET /api/hospedes`

**Resposta 200**
```json
[
  {
    "id": "3f2a9c1e-8b7d-4e21-9a0f-1c2d3e4f5a6b",
    "nome": "Maria Silva",
    "cpf": "12345678901",
    "telefone": "11999998888",
    "email": "maria@email.com"
  }
]
```
- Lista vazia retorna `[]`. Exibir "Nenhum hóspede cadastrado."
- A lista **não vem ordenada**. Ordene por nome no frontend se quiser.

### 4.2 Buscar hóspede por CPF
`GET /api/hospedes/{cpf}`

**Resposta 200**: um objeto `Hospede`.

**Erros**
| Código | mensagem |
|---|---|
| 400 | `CPF é obrigatório.` |
| 400 | `CPF deve ter 11 dígitos.` |
| 404 | `Nenhum hóspede encontrado com esse CPF.` |

### 4.3 Cadastrar hóspede
`POST /api/hospedes`

**Corpo**
```json
{
  "nome": "Maria Silva",
  "cpf": "123.456.789-01",
  "telefone": "11999998888",
  "email": "maria@email.com"
}
```

| Campo | Obrigatório | Regra |
|---|---|---|
| `nome` | sim | Não pode ser vazio nem só espaços. |
| `cpf` | sim | Depois de remover tudo que não é dígito, precisa ter exatamente 11 dígitos. Precisa ser único. **Não há validação de dígito verificador.** |
| `telefone` | não | Texto livre, sem validação. |
| `email` | não | Texto livre, sem validação de formato no backend. Validar o formato no frontend é recomendado. |

**Resposta 201**: o `Hospede` criado.

**Erros** (verificados nesta ordem)
| Código | mensagem |
|---|---|
| 400 | `Nome é obrigatório.` |
| 400 | `CPF é obrigatório.` |
| 400 | `CPF deve ter 11 dígitos.` |
| 400 | `Já existe um hóspede com esse CPF.` |

### 4.4 Editar hóspede
`PUT /api/hospedes/{cpf}`

**O CPF não pode ser alterado.** Ele vai só na URL. No formulário de edição, mostre o CPF
como campo somente leitura.

**Corpo**
```json
{
  "nome": "Maria Silva Souza",
  "telefone": "11988887777",
  "email": "maria.souza@email.com"
}
```

**Resposta 200**: o `Hospede` atualizado.

**Erros** (nesta ordem)
| Código | mensagem |
|---|---|
| 400 | `Nome é obrigatório.` |
| 400 | `CPF é obrigatório.` |
| 400 | `CPF deve ter 11 dígitos.` |
| 404 | `Nenhum hóspede encontrado com esse CPF.` |

### 4.5 Remover hóspede
`DELETE /api/hospedes/{cpf}`

**Resposta 204**: sem corpo.

**Erros**
| Código | mensagem |
|---|---|
| 400 | `CPF é obrigatório.` / `CPF deve ter 11 dígitos.` |
| 404 | `Nenhum hóspede encontrado com esse CPF.` |
| 500 | Pode ocorrer se o hóspede tiver reservas vinculadas (restrição do banco). Exibir mensagem genérica. |

**Frontend:** pedir confirmação antes ("Remover este hóspede?").

---

## 5. Endpoints: Quartos

O quarto é identificado nas rotas pelo **número** (não pelo id).

### 5.1 Listar quartos
`GET /api/quartos`

**Resposta 200**: lista de `Quarto`, **ordenada por número**.
```json
[
  {
    "id": "a1b2c3d4-0000-0000-0000-000000000001",
    "numero": 101,
    "tipo": "Casal",
    "capacidade": 2,
    "valorDiaria": 250.00,
    "ocupado": false
  }
]
```
- Lista vazia retorna `[]`. Exibir "Nenhum quarto cadastrado."
- Exibir o campo `ocupado` como selo: **Ocupado** (vermelho) / **Livre** (verde).

### 5.2 Buscar quarto por número
`GET /api/quartos/{numero}`

**Resposta 200**: um `Quarto`.

**Erros**
| Código | mensagem |
|---|---|
| 404 | `Nenhum quarto encontrado com esse número.` |

### 5.3 Quartos disponíveis em um período
`GET /api/quartos/disponiveis?entrada=YYYY-MM-DD&saida=YYYY-MM-DD`

Retorna os quartos **sem reserva (não cancelada) que se sobreponha** ao período, ordenados
por número. É o endpoint que alimenta a escolha de quarto no formulário de nova reserva.

**Regra de sobreposição:** um quarto está indisponível se existir reserva com status
diferente de `Cancelada` em que `entrada < dataSaidaDaReserva` **e** `saida > dataEntradaDaReserva`.
O dia da saída de uma reserva pode ser o dia de entrada de outra (ex.: reserva A sai dia 20,
reserva B pode entrar dia 20).

**Exemplo:** `GET /api/quartos/disponiveis?entrada=2026-11-20&saida=2026-11-23`

**Resposta 200**: lista de `Quarto` (pode ser `[]`, então exibir "Nenhum quarto disponível nesse período.").

**Erros**
| Código | mensagem |
|---|---|
| 400 | `A data de entrada não pode ser no passado.` |
| 400 | `A data de saída deve ser depois da data de entrada.` |

> O campo `ocupado` também vem nessa lista, mas **não importa** para reservas futuras.
> Não esconda quartos ocupados agora nessa tela.

### 5.4 Cadastrar quarto
`POST /api/quartos`

**Corpo**
```json
{
  "numero": 101,
  "tipo": "Casal",
  "capacidade": 2,
  "valorDiaria": 250.00
}
```

| Campo | Obrigatório | Regra |
|---|---|---|
| `numero` | sim | Inteiro maior que zero. Único. |
| `tipo` | sim | Não pode ser vazio. Texto livre. Sugestão de UI: select com "Solteiro", "Casal", "Suíte", permitindo outro valor. |
| `capacidade` | sim | Inteiro maior que zero. |
| `valorDiaria` | sim | Maior que zero. |

O quarto sempre é criado com `ocupado: false`. Não envie esse campo.

**Resposta 201**: o `Quarto` criado.

**Erros** (nesta ordem)
| Código | mensagem |
|---|---|
| 400 | `Número do quarto deve ser maior que zero.` |
| 400 | `Tipo é obrigatório.` |
| 400 | `Capacidade deve ser maior que zero.` |
| 400 | `Valor da diária deve ser maior que zero.` |
| 400 | `Já existe um quarto com esse número.` |

### 5.5 Editar quarto
`PUT /api/quartos/{numero}`

**O número não pode ser alterado.** Mostre-o como somente leitura. O campo `ocupado`
**não é editável** manualmente; ele só muda via check-in e check-out.

**Corpo**
```json
{
  "tipo": "Suíte",
  "capacidade": 3,
  "valorDiaria": 400.00
}
```

**Resposta 200**: o `Quarto` atualizado.

**Erros** (nesta ordem)
| Código | mensagem |
|---|---|
| 400 | `Tipo é obrigatório.` |
| 400 | `Capacidade deve ser maior que zero.` |
| 400 | `Valor da diária deve ser maior que zero.` |
| 404 | `Nenhum quarto encontrado com esse número.` |

> Alterar a diária **não** altera o `valorTotal` de reservas já criadas.

### 5.6 Remover quarto
`DELETE /api/quartos/{numero}`

**Resposta 204**: sem corpo.

**Erros**
| Código | mensagem |
|---|---|
| 404 | `Nenhum quarto encontrado com esse número.` |
| 400 | `Não é possível remover um quarto ocupado.` |
| 500 | Pode ocorrer se o quarto tiver reservas vinculadas (restrição do banco). Exibir mensagem genérica. |

**Frontend:** pedir confirmação. Desabilitar o botão "Remover" quando `ocupado === true`.

---

## 6. Endpoints: Reservas

A reserva é identificada pelo **id** (GUID).

### 6.1 Listar reservas
`GET /api/reservas`

**Resposta 200**: lista de `ReservaDetalheDto`, **ordenada por data de entrada (crescente)**.
Inclui reservas de **todos os status**, inclusive canceladas e finalizadas.
```json
[
  {
    "id": "9e8d7c6b-0000-0000-0000-0000000000aa",
    "nomeHospede": "Maria Silva",
    "cpfHospede": "12345678901",
    "numeroQuarto": 101,
    "dataEntrada": "2026-11-20T00:00:00",
    "dataSaida": "2026-11-23T00:00:00",
    "valorTotal": 750.00,
    "status": "Confirmada"
  }
]
```
- Lista vazia retorna `[]`. Exibir "Nenhuma reserva cadastrada."
- **Sugestão:** filtro por status no frontend (abas "Todas / Confirmadas / Em andamento / Finalizadas / Canceladas").

### 6.2 Criar reserva
`POST /api/reservas`

**Corpo**
```json
{
  "cpf": "12345678901",
  "numeroQuarto": 101,
  "dataEntrada": "2026-11-20",
  "dataSaida": "2026-11-23"
}
```

| Campo | Regra |
|---|---|
| `cpf` | CPF de um hóspede **já cadastrado** (aceita com máscara). |
| `numeroQuarto` | Número de um quarto **já cadastrado**. |
| `dataEntrada` | Hoje ou futura. Hora é ignorada. |
| `dataSaida` | Estritamente depois de `dataEntrada` (mínimo 1 noite). |

**Cálculo do valor (feito no backend):**
`valorTotal = valorDiaria do quarto × número de noites`, sendo `noites = dataSaida − dataEntrada` em dias.
Ex.: entrada 20/11, saída 23/11 → 3 noites × R$ 250,00 = R$ 750,00.
O frontend pode mostrar uma **prévia** desse valor no formulário, usando o mesmo cálculo
com o `valorDiaria` do quarto escolhido.

A reserva é criada com status `Confirmada`. Criar a reserva **não** muda o `ocupado` do
quarto. É permitido reservar um quarto que está ocupado hoje, desde que as datas não conflitem.

**Resposta 201**
```json
{ "id": "9e8d7c6b-0000-0000-0000-0000000000aa" }
```

**Erros** (nesta ordem)
| Código | mensagem |
|---|---|
| 400 | `A data de entrada não pode ser no passado.` |
| 400 | `A data de saída deve ser depois da data de entrada.` |
| 400 | `CPF é obrigatório.` |
| 400 | `CPF deve ter 11 dígitos.` |
| 404 | `Nenhum hóspede encontrado com esse CPF.` |
| 404 | `Nenhum quarto encontrado com esse número.` |
| 400 | `O quarto já está reservado nesse período.` |

**Fluxo de tela recomendado para "Nova reserva":**
1. Usuário escolhe **data de entrada** e **data de saída** (inputs `type="date"`; `min` da entrada = hoje; `min` da saída = entrada + 1 dia).
2. Frontend chama `GET /api/quartos/disponiveis?entrada=...&saida=...` e mostra os quartos livres (número, tipo, capacidade, diária).
3. Usuário escolhe o quarto e informa o **CPF** do hóspede. Opcional: chamar `GET /api/hospedes/{cpf}` para exibir o nome do hóspede e confirmar que ele existe. Se der 404, oferecer link para cadastrar hóspede.
4. Exibir **prévia do valor total** (noites × diária).
5. Enviar `POST /api/reservas`. Em sucesso, ir para a lista de reservas. Em erro, exibir `mensagem`. O erro `O quarto já está reservado nesse período.` pode acontecer se outra pessoa reservou no meio do caminho; nesse caso, recarregar os disponíveis.

### 6.3 Fazer check-in
`POST /api/reservas/{id}/checkin` (sem corpo)

**Efeito:** reserva passa de `Confirmada` para `EmAndamento` e o quarto passa a `ocupado: true`.

**Resposta 204**: sem corpo. Recarregar a lista de reservas (e de quartos, se estiver visível).

**Regras (verificadas nesta ordem)**
| Código | mensagem | Quando |
|---|---|---|
| 404 | `Reserva não encontrada.` | Id não existe. |
| 400 | `Só é possível fazer check-in de uma reserva confirmada.` | Status diferente de `Confirmada`. |
| 400 | `O check-in só pode ser feito a partir da data de entrada.` | Hoje é antes de `dataEntrada`. |
| 400 | `O período desta reserva já terminou.` | Hoje é igual ou depois de `dataSaida`. |
| 404 | `Quarto não encontrado.` | O quarto da reserva foi removido. |
| 400 | `O quarto já está ocupado.` | O hóspede anterior ainda não fez check-out. |

**Frontend:** mostrar o botão **"Check-in"** somente quando `status === 'Confirmada'`
**e** `dataEntrada <= hoje < dataSaida`. Comparar só as datas, sem hora.

### 6.4 Fazer check-out
`POST /api/reservas/{id}/checkout` (sem corpo)

**Efeito:** reserva passa de `EmAndamento` para `Finalizada` e o quarto volta a `ocupado: false`.
Pode ser feito em qualquer dia (inclusive antes da data de saída prevista). O `valorTotal`
**não** é recalculado.

**Resposta 204**: sem corpo.

**Erros**
| Código | mensagem |
|---|---|
| 404 | `Reserva não encontrada.` |
| 400 | `Só é possível fazer check-out de uma reserva em andamento.` |
| 404 | `Quarto não encontrado.` |

**Frontend:** mostrar o botão **"Check-out"** somente quando `status === 'EmAndamento'`.

### 6.5 Cancelar reserva
`POST /api/reservas/{id}/cancelar` (sem corpo)

**Efeito:** reserva passa de `Confirmada` para `Cancelada`. **A reserva não é apagada**;
continua na listagem com status `Cancelada`. As datas daquele quarto ficam livres
para novas reservas.

**Resposta 204**: sem corpo.

**Erros**
| Código | mensagem |
|---|---|
| 404 | `Reserva não encontrada.` |
| 400 | `Só é possível cancelar uma reserva antes do check-in.` |

**Frontend:** mostrar o botão **"Cancelar"** somente quando `status === 'Confirmada'`,
com confirmação ("Cancelar esta reserva?").

### Resumo: ações disponíveis por status

| Status | Check-in | Check-out | Cancelar |
|---|---|---|---|
| `Confirmada` | ✅ (só entre entrada e saída) | ❌ | ✅ |
| `EmAndamento` | ❌ | ✅ | ❌ |
| `Finalizada` | ❌ | ❌ | ❌ |
| `Cancelada` | ❌ | ❌ | ❌ |

---

## 7. Tabela resumo de endpoints

| Método | Rota | Corpo | Sucesso | Descrição |
|---|---|---|---|---|
| GET | `/api/hospedes` | — | 200 `Hospede[]` | Listar hóspedes |
| GET | `/api/hospedes/{cpf}` | — | 200 `Hospede` | Buscar por CPF |
| POST | `/api/hospedes` | `{nome, cpf, telefone, email}` | 201 `Hospede` | Cadastrar |
| PUT | `/api/hospedes/{cpf}` | `{nome, telefone, email}` | 200 `Hospede` | Editar |
| DELETE | `/api/hospedes/{cpf}` | — | 204 | Remover |
| GET | `/api/quartos` | — | 200 `Quarto[]` | Listar quartos |
| GET | `/api/quartos/{numero}` | — | 200 `Quarto` | Buscar por número |
| GET | `/api/quartos/disponiveis?entrada=&saida=` | — | 200 `Quarto[]` | Livres no período |
| POST | `/api/quartos` | `{numero, tipo, capacidade, valorDiaria}` | 201 `Quarto` | Cadastrar |
| PUT | `/api/quartos/{numero}` | `{tipo, capacidade, valorDiaria}` | 200 `Quarto` | Editar |
| DELETE | `/api/quartos/{numero}` | — | 204 | Remover |
| GET | `/api/reservas` | — | 200 `ReservaDetalheDto[]` | Listar reservas |
| POST | `/api/reservas` | `{cpf, numeroQuarto, dataEntrada, dataSaida}` | 201 `{id}` | Criar reserva |
| POST | `/api/reservas/{id}/checkin` | — | 204 | Check-in |
| POST | `/api/reservas/{id}/checkout` | — | 204 | Check-out |
| POST | `/api/reservas/{id}/cancelar` | — | 204 | Cancelar |

Erros: sempre `{ "mensagem": "..." }` com 400 ou 404; 500 para falhas inesperadas.

---

## 8. Telas sugeridas

A página inicial deve ser a lista de **Hóspedes** (o sistema atual redireciona `/` para hóspedes).
Menu fixo com: **Hóspedes**, **Quartos**, **Reservas**.

### Hóspedes
- **Lista** (`/hospedes`): tabela com Nome, CPF (com máscara `000.000.000-00`), Telefone, E-mail e ações Editar / Remover. Botão "Cadastrar hóspede". Campo de busca opcional (filtro local por nome/CPF).
- **Cadastro** (`/hospedes/novo`): Nome*, CPF*, Telefone, E-mail. Máscara de CPF no input.
- **Edição** (`/hospedes/:cpf/editar`): carrega via `GET /api/hospedes/{cpf}`; CPF somente leitura; Nome*, Telefone, E-mail.

### Quartos
- **Lista** (`/quartos`): tabela com Número, Tipo, Capacidade, Diária (R$), Situação (selo Ocupado/Livre) e ações Editar / Remover (Remover desabilitado se ocupado). Botão "Cadastrar quarto". Opcional: cards com contagem "X livres / Y ocupados".
- **Cadastro** (`/quartos/novo`): Número*, Tipo*, Capacidade*, Valor da diária*.
- **Edição** (`/quartos/:numero/editar`): Número somente leitura; Tipo*, Capacidade*, Valor da diária*.

### Reservas
- **Lista** (`/reservas`): tabela com Hóspede (nome + CPF), Quarto, Entrada, Saída, Noites (calculado), Valor total, Status (selo colorido) e botões Check-in / Check-out / Cancelar conforme a tabela da seção 6. Filtro por status. Botão "Nova reserva".
- **Nova reserva** (`/reservas/nova`): fluxo em etapas descrito em 6.2.

### Comportamentos gerais
- Exibir estado de **carregando** durante requisições e desabilitar botões para evitar clique duplo.
- Exibir a `mensagem` de erro da API próxima ao formulário ou em um toast.
- Após qualquer ação (criar, editar, remover, check-in, check-out, cancelar), **recarregar a lista** correspondente; não tentar adivinhar o novo estado localmente.
- Datas exibidas em `dd/MM/yyyy`; dinheiro em `R$ 0.000,00`.
- Campos obrigatórios marcados com `*` e validados também no frontend (mesmas regras da API), mas **a validação do backend é a definitiva**.

---

## 9. Limitações conhecidas (não são bugs do frontend)

- **Sem autenticação**: qualquer pessoa com acesso à API pode usar todos os endpoints.
- **Sem paginação**: as listagens retornam todos os registros.
- **CPF**: só é validado o tamanho (11 dígitos), não os dígitos verificadores.
- **E-mail e telefone**: sem validação de formato no backend.
- **Remover hóspede ou quarto com reservas** pode retornar 500, dependendo das restrições do banco.
- **Check-out antecipado** não recalcula o valor nem libera as datas restantes da reserva para novas reservas.
- **Não existe endpoint para editar uma reserva** (trocar datas ou quarto). Para isso, cancelar e criar outra.
