using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto
{
    public partial class Form1 : Form
    {

        private List<ProcessoSimulado> processosSimulados = new List<ProcessoSimulado>();
        Dictionary<int, TimeSpan> cpuTimesAntigos = new Dictionary<int, TimeSpan>();
        DateTime ultimaAtualizacao = DateTime.Now;
        private DataGridView dgvProcessos = new DataGridView();
        private Timer timerAtualizacao;
        private TextBox txtBusca;
        private CheckBox btnAtualizar;
        
        //private AtualizadorRemoto atualizadorRemoto;
        
        private Timer timerAtualizacaoRemota;
        private string ipRemotoValidacao;
        private string filtroRemotoAtual;
        private TextBox txtBuscaRemoto;
        private CheckBox btnAtualizarRemoto;
        private TextBox txtIP;


        public Form1()
        {
            InitializeComponent();


            // Configuração visual do DataGridView
            dgvProcessos.Size = new Size(530, 200);     // ajuste tamanho
            dgvProcessos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProcessos.ReadOnly = true;
            dgvProcessos.AllowUserToAddRows = false;

            // Adiciona ao formulário
            dgvProcessos.Columns.Clear();
            dgvProcessos.Columns.Add("Nome", "Nome");
            dgvProcessos.Columns.Add("PID", "PID");
            dgvProcessos.Columns.Add("Memoria", "Memória (MB)");
            dgvProcessos.Columns.Add("CPU", "Uso de CPU (%)");
            dgvProcessos.Columns.Add("Prioridade", "Prioridade");

            //panelCorpo.Controls.Add(dgvProcessos);

            timerAtualizacao = new Timer();
            timerAtualizacao.Interval = 8000;
            timerAtualizacao.Tick += (s, ev) => {
                if (string.IsNullOrWhiteSpace(txtBusca.Text) && btnAtualizar.Checked && btnAtualizar != null && txtBusca != null)
                {
                    AtualizarListaDeProcessos();
                }
            };


            timerAtualizacaoRemota = new Timer();
            timerAtualizacaoRemota.Interval = 8000;
            timerAtualizacaoRemota.Tick += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(ipRemotoValidacao) || !string.IsNullOrEmpty(txtBuscaRemoto.Text) || !btnAtualizarRemoto.Checked 
                || txtIP.Text != ipRemotoValidacao) return;

                List<ProcessoRemoto> lista = await Cliente.ObterProcessos(ipRemotoValidacao);
      
                dgvProcessos.Rows.Clear();

                foreach (var p in lista)
                {
                    dgvProcessos.Rows.Add(
                        p.ProcessName,
                        p.Id,
                        p.Memoria.ToString("F2"),
                        p.usoCpu.ToString("F2"),
                        p.BasePriority
                    );
                }
            };

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonSimuladdor_Click(object sender, EventArgs e)
        {
            ipRemotoValidacao = null;
            filtroRemotoAtual = null;
            timerAtualizacaoRemota.Stop();
            timerAtualizacao.Stop();
            panelCorpo.Controls.Clear();
            dgvProcessos.Rows.Clear();

            // Label título
            var lblTitulo = new Label()
            {
                Text = "Simulador de Escalonamento",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(120, 1),
                Width = 400,                 
                Height = 40,                  
                TextAlign = ContentAlignment.MiddleLeft 
            };
            panelCorpo.Controls.Add(lblTitulo);

            // Campos de entrada

            // Label e TextBox para Nome
            var lblNome = new Label() { Text = "Nome", Location = new Point(20, 40), Width = 120 };
            var txtNome = new TextBox() { Width = 120, Location = new Point(20, 65) };

            // Label e TextBox para Chegada
            var lblChegada = new Label() { Text = "Chegada", Location = new Point(150, 40), Width = 80 };
            var txtChegada = new TextBox() { Width = 80, Location = new Point(150, 65) };

            // Label e TextBox para Duração
            var lblDuracao = new Label() { Text = "Duração", Location = new Point(240, 40), Width = 80 };
            var txtDuracao = new TextBox() { Width = 80, Location = new Point(240, 65) };

            // Label e TextBox para Prioridade
            var lblPrioridade = new Label() { Text = "Prioridade", Location = new Point(330, 40), Width = 80 };
            var txtPrioridade = new TextBox() { Width = 80, Location = new Point(330, 65) };

            // Label e TextBox para Quantum
            var lblQuantum = new Label() { Text = "Quantum (ex: 2)", Location = new Point(420, 40), Width = 100 };
            var txtQuantum = new TextBox() { Width = 100, Location = new Point(420, 65)};

            // Adiciona os Labels e TextBoxes ao painel
            panelCorpo.Controls.AddRange(new Control[] {

            lblNome, txtNome,
            lblChegada, txtChegada,
            lblDuracao, txtDuracao,
            lblPrioridade, txtPrioridade,
            lblQuantum, txtQuantum
            });

            // Botão Adicionar
            var btnAdicionar = new Button()
            {
                Text = "Adicionar Processo",
                Location = new Point(540, 60),
                Width = 140
            };
            panelCorpo.Controls.Add(btnAdicionar);


            // DataGridView para exibir os processos
            // DataGridView para exibir os processos
            var dgvProcessosSimulados = new DataGridView()
            {
                Location = new Point(20, 100),
                Width = 540,
                Height = 200,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };
            dgvProcessosSimulados.Columns.Add("Nome", "Nome");
            dgvProcessosSimulados.Columns.Add("Chegada", "Chegada");
            dgvProcessosSimulados.Columns.Add("Duracao", "Duração");
            dgvProcessosSimulados.Columns.Add("Prioridade", "Prioridade");
            panelCorpo.Controls.Add(dgvProcessosSimulados);

            // Botão Executar FCFS
            var btnExecutarFCFS = new Button()
            {
                Text = "Executar FCFS",
                Location = new Point(20, 320),
                Width = 140
            };
            panelCorpo.Controls.Add(btnExecutarFCFS);

            // Botão Executar Round-Robin
            var btnExecutarRR = new Button()
            {
                Text = "Executar Round-Robin",
                Location = new Point(180, 320),
                Width = 160
            };
            panelCorpo.Controls.Add(btnExecutarRR);

            // Botão Executar SJF
            var btnExecutarSJF = new Button()
            {
                Text = "Executar SJF",
                Location = new Point(360, 320),
                Width = 140
            };
            panelCorpo.Controls.Add(btnExecutarSJF);

            // Evento para adicionar processo à lista
            btnAdicionar.Click += (s, ev) =>
            {
                string nome = txtNome.Text;
                if (!int.TryParse(txtChegada.Text, out int chegada) ||
                    !int.TryParse(txtDuracao.Text, out int duracao) ||
                    !int.TryParse(txtPrioridade.Text, out int prioridade))
                {
                    MessageBox.Show("Preencha todos os campos corretamente.");
                    return;
                }

                var processo = new ProcessoSimulado(nome, chegada, duracao, prioridade);
                processosSimulados.Add(processo);

                dgvProcessosSimulados.Rows.Add(processo.Nome, processo.TempoChegada, processo.Duracao, processo.Prioridade);

                txtNome.Clear();
                txtChegada.Clear();
                txtDuracao.Clear();
                txtPrioridade.Clear();
                
            };

            // Evento para executar o algoritmo FCFS
            btnExecutarFCFS.Click += (s, ev) =>
            {
                if (processosSimulados.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um processo.");
                    return;
                }

                var processosOrdenados = processosSimulados.OrderBy(p => p.TempoChegada).ToList();
                string resultado = "Ordem de execução (FCFS):\n\n";
                int tempoAtual = 0;

                foreach (var p in processosOrdenados)
                {
                    resultado += $"{p.Nome} (Chegada: {p.TempoChegada}, Duração: {p.Duracao}) inicia em {tempoAtual}\n";
                    tempoAtual += p.Duracao;
                }

                MessageBox.Show(resultado, "Execução FCFS");

            };

            // Evento para executar o algoritmo Round-Robin
            btnExecutarRR.Click += (s, ev) =>
            {
                if (processosSimulados.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um processo.");
                    return;
                }

                if (!int.TryParse(txtQuantum.Text, out int quantum) || quantum <= 0)
                {
                    MessageBox.Show("Informe um quantum válido (maior que zero).");
                    return;
                }

                var fila = new Queue<ProcessoSimulado>();
                var temposRestantes = new Dictionary<ProcessoSimulado, int>();
                var processosOrdenados = processosSimulados.OrderBy(p => p.TempoChegada).ToList();

                int tempoAtual = 0;
                int indiceProximo = 0;
                string log = "Execução Round-Robin:\n\n";

                while (fila.Count > 0 || indiceProximo < processosOrdenados.Count)
                {
                    while (indiceProximo < processosOrdenados.Count && processosOrdenados[indiceProximo].TempoChegada <= tempoAtual)
                    {
                        var p = processosOrdenados[indiceProximo];
                        fila.Enqueue(p);
                        temposRestantes[p] = p.Duracao;
                        indiceProximo++;
                    }

                    if (fila.Count == 0)
                    {
                        tempoAtual++;
                        continue;
                    }

                    var processoAtual = fila.Dequeue();
                    int tempoRestante = temposRestantes[processoAtual];
                    int tempoExecutado = Math.Min(quantum, tempoRestante);

                    log += $"{processoAtual.Nome} executa de {tempoAtual} até {tempoAtual + tempoExecutado}\n";

                    tempoAtual += tempoExecutado;
                    temposRestantes[processoAtual] -= tempoExecutado;

                    if (temposRestantes[processoAtual] > 0)
                    {
                        while (indiceProximo < processosOrdenados.Count && processosOrdenados[indiceProximo].TempoChegada <= tempoAtual)
                        {
                            var p = processosOrdenados[indiceProximo];
                            fila.Enqueue(p);
                            temposRestantes[p] = p.Duracao;
                            indiceProximo++;
                        }

                        fila.Enqueue(processoAtual);
                    }
                }

                MessageBox.Show(log, "Resultado Round-Robin");
            };

            // Evento para executar o algoritmo SJF (Shortest Job First)    
            btnExecutarSJF.Click += (s, ev) =>
            {
                if (processosSimulados.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um processo.");
                    return;
                }

                // Copia a lista para não alterar a original
                var processosRestantes = processosSimulados
                    .Select(p => new ProcessoSimulado(p.Nome, p.TempoChegada, p.Duracao, p.Prioridade))
                    .ToList();

                int tempoAtual = 0;
                string resultado = "Ordem de execução (SJF):\n\n";

                while (processosRestantes.Count > 0)
                {
                    // Seleciona processos já chegados
                    var disponiveis = processosRestantes
                        .Where(p => p.TempoChegada <= tempoAtual)
                        .ToList();

                    if (disponiveis.Count == 0)
                    {
                        tempoAtual++;
                        continue;
                    }

                    // Seleciona o de menor duração
                    var proximo = disponiveis.OrderBy(p => p.Duracao).First();

                    resultado += $"{proximo.Nome} (Chegada: {proximo.TempoChegada}, Duração: {proximo.Duracao}) inicia em {tempoAtual}\n";
                    tempoAtual += proximo.Duracao;
                    processosRestantes.Remove(proximo);
                }

                MessageBox.Show(resultado, "Execução SJF");
            };

        }

        private void buttonReal_Click(object sender, EventArgs e)
        {
            ipRemotoValidacao = null;
            filtroRemotoAtual = null;
            timerAtualizacaoRemota.Stop();
            timerAtualizacao.Start();
            panelCorpo.Controls.Clear();
            dgvProcessos.Rows.Clear();
            dgvProcessos.Location = new Point(20, 95); // ajuste posição
            panelCorpo.Controls.Add(this.dgvProcessos); // adiciona a grid novamente

            // Label título
            Label lblTitulo = new Label()
            {
                Text = "Listagem de Processos Reais",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(120, 1),
                Width = 400,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelCorpo.Controls.Add(lblTitulo);

            // Label para busca de processos
            var lblBusca = new Label()
            {
                Text = "Buscar processo por nome:",
                Location = new Point(20, 60),
                Width = 150
            };
            panelCorpo.Controls.Add(lblBusca);

            // TextBox para busca de processos
            txtBusca = new TextBox()
            {
                Location = new Point(190, 60),
                Width = 200
            };
            panelCorpo.Controls.Add(txtBusca);

            // Botão de busca
            var btnBuscar = new Button()
            {
                Text = "Buscar",
                Location = new Point(390, 60),
                Width = 100
            };
            panelCorpo.Controls.Add(btnBuscar);

            // Botao Listar Processos
            var btnListar = new Button()
            {
                Text = "Listar Processos Reais",
                Location = new Point(20, 300),
                Width = 200
            };
            panelCorpo.Controls.Add(btnListar);

            // Botao atualizar lista de processos (Sim - Nao)
            btnAtualizar = new CheckBox()
            {
                Text = "Atualizar lista a cada 8 segundos",
                Location = new Point(240, 300),
                Width = 250,
                Checked = true
            };
            panelCorpo.Controls.Add(btnAtualizar);

            // Label para selecionar processo
            var lblSelecionar = new Label()
            {
                Text = "Selecionar processo:",
                Location = new Point(20, 340),
                Width = 130
            };
            panelCorpo.Controls.Add(lblSelecionar);

            // ComboBox  selecionar processo
            var cbProcessos = new ComboBox()
            {
                Location = new Point(180, 340),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelCorpo.Controls.Add(cbProcessos);

            
            var lblPrioridade = new Label()
            {
                Text = "Nova prioridade:",
                Location = new Point(20, 380),
                Width = 130
            };
            panelCorpo.Controls.Add(lblPrioridade);

            // ComboBox para selecionar prioridade
            var cbPrioridades = new ComboBox()
            {
                Location = new Point(180, 380),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbPrioridades.Items.AddRange(new object[]
            {
                ProcessPriorityClass.Idle,
                ProcessPriorityClass.BelowNormal,
                ProcessPriorityClass.Normal,
                ProcessPriorityClass.AboveNormal,
                ProcessPriorityClass.High,
                ProcessPriorityClass.RealTime
            });
            cbPrioridades.SelectedIndex = 2; // Normal
            panelCorpo.Controls.Add(cbPrioridades);

            var btnAlterarPrioridade = new Button()
            {
                Text = "Alterar Prioridade",
                Location = new Point(410, 380),
                Width = 150
            };
            panelCorpo.Controls.Add(btnAlterarPrioridade);

            // Botão para encerrar processo
            var btnEncerrarProcesso = new Button()
            {
                Text = "Encerrar Processo",
                Location = new Point(420, 340),
                Width = 150
            };
            panelCorpo.Controls.Add(btnEncerrarProcesso);


            // Evento para listar processos reais
            btnListar.Click += (s, ev) =>
            {
                dgvProcessos.Rows.Clear();
                AtualizarListaDeProcessos();
            };

            // Evento para buscar processo por nome
            btnBuscar.Click += (s, ev) =>
            {
                string termo = txtBusca.Text.Trim().ToLower();

                var processosFiltrados = Process.GetProcesses()
                    .Where(p => p.ProcessName.ToLower().Contains(termo))
                    .OrderBy(p => p.ProcessName)
                    .ToList();

                dgvProcessos.Rows.Clear();
                cbProcessos.Items.Clear();

                foreach (var processo in processosFiltrados)
                {
                    try
                    {
                        // Calcula uso de memória
                        double memoriaMB = processo.WorkingSet64 / (1024.0 * 1024.0);

                        // Calcula uso de CPU estimado
                        double usoCpu = 0;
                        TimeSpan cpuAtual = processo.TotalProcessorTime;

                        if (cpuTimesAntigos.TryGetValue(processo.Id, out TimeSpan cpuAntigo))
                        {
                            TimeSpan intervalo = DateTime.Now - ultimaAtualizacao;
                            usoCpu = (cpuAtual - cpuAntigo).TotalMilliseconds / intervalo.TotalMilliseconds;
                            usoCpu = usoCpu * 100 / Environment.ProcessorCount;
                        }

                        cpuTimesAntigos[processo.Id] = cpuAtual;

                        // Adiciona ao DataGridView
                        dgvProcessos.Rows.Add(
                            processo.ProcessName,
                            processo.Id,
                            memoriaMB.ToString("F2"),
                            usoCpu.ToString("F2"),
                            processo.BasePriority
                        );

                        // Adiciona ao ComboBox
                        cbProcessos.Items.Add(new
                        {
                            Display = $"{processo.ProcessName} (PID: {processo.Id})",
                            Processo = processo
                        });
                    }
                    catch { 
                        
                    }
                }

                cbProcessos.DisplayMember = "Display";
                cbProcessos.ValueMember = "Processo";

                if (cbProcessos.Items.Count > 0)
                    cbProcessos.SelectedIndex = 0;
            };

            // Evento para alterar prioridade do processo selecionado
            btnAlterarPrioridade.Click += (s, ev) =>
            {
                if (cbProcessos.SelectedItem == null || cbPrioridades.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um processo e uma prioridade.");
                    return;
                }

                dynamic itemSelecionado = cbProcessos.SelectedItem;
                Process processo = itemSelecionado.Processo;
                ProcessPriorityClass novaPrioridade = (ProcessPriorityClass)cbPrioridades.SelectedItem;

                try
                {
                    processo.PriorityClass = novaPrioridade;
                    MessageBox.Show($"Prioridade de '{processo.ProcessName}' alterada para '{novaPrioridade}'.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao alterar prioridade: " + ex.Message);
                }
            };

            // Evento para encerrar processo
            btnEncerrarProcesso.Click += (s, ev) =>
            {
                if (cbProcessos.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um processo para encerrar.");
                    return;
                }

                dynamic itemSelecionado = cbProcessos.SelectedItem;
                Process processo = itemSelecionado.Processo;

                DialogResult resultado = MessageBox.Show(
                    $"Tem certeza que deseja encerrar o processo '{processo.ProcessName}' (PID: {processo.Id})?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        processo.Kill();
                        MessageBox.Show("Processo encerrado com sucesso.");

                        // Atualiza lista após encerrar
                        cbProcessos.Items.Remove(cbProcessos.SelectedItem);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao encerrar o processo: " + ex.Message);
                    }
                }
            };


            void AtualizarComboBoxComProcessos(List<Process> processos)
            {
                cbProcessos.Items.Clear();

                foreach (var processo in processos)
                {
                    try
                    {
                        cbProcessos.Items.Add(new
                        {
                            Display = $"{processo.ProcessName} (PID: {processo.Id})",
                            Processo = processo
                        });
                    }
                    catch { }
                }

                cbProcessos.DisplayMember = "Display";
                cbProcessos.ValueMember = "Processo";

                if (cbProcessos.Items.Count > 0)
                    cbProcessos.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timerAtualizacao.Stop();
            panelCorpo.Controls.Clear();
            dgvProcessos.Location = new Point(20, 150);
            dgvProcessos.Rows.Clear();
            panelCorpo.Controls.Add(this.dgvProcessos);

            // Label título
            Label lblTitulo = new Label()
            {
                Text = "Listagem de Processos Remotos",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(120, 1),
                Width = 400,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelCorpo.Controls.Add(lblTitulo);

            // Label para busca de processos
            var lblBusca = new Label()
            {
                Text = "Buscar processo por nome:",
                Location = new Point(20, 120),
                Width = 150
            };
            panelCorpo.Controls.Add(lblBusca);

            // TextBox para busca de processos
            txtBuscaRemoto = new TextBox()
            {
                Location = new Point(180, 120),
                Width = 200
            };
            panelCorpo.Controls.Add(txtBuscaRemoto);

            // Botão de busca
            var btnBuscar = new Button()
            {
                Text = "Buscar",
                Location = new Point(390, 119),
                Width = 100
            };
            panelCorpo.Controls.Add(btnBuscar);

            var lblIP = new Label()
            {
                Text = "IP da máquina remota:",
                Location = new Point(20, 80),
                Width = 150
            };
            panelCorpo.Controls.Add(lblIP);

            txtIP = new TextBox()
            {
                Location = new Point(180, 80),
                Width = 200,
                Name = "txtIP"
            };
            panelCorpo.Controls.Add(txtIP);

            var btnConectar = new Button()
            {
                Text = "Conectar",
                Location = new Point(390, 79),
                Width = 100
            };
            panelCorpo.Controls.Add(btnConectar);

            btnAtualizarRemoto = new CheckBox()
            {
                Text = "Atualizar lista a cada 8 segundos",
                Location = new Point(20, 350),
                Width = 250,
                Checked = true
            };
            panelCorpo.Controls.Add(btnAtualizarRemoto);

            var lblSelecionar = new Label()
            {
                Text = "Selecionar processo:",
                Location = new Point(20, 400),
                Width = 130
            };
            panelCorpo.Controls.Add(lblSelecionar);

            // ComboBox  selecionar processo
            var cbProcessos = new ComboBox()
            {
                Location = new Point(180, 400),
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelCorpo.Controls.Add(cbProcessos);


            var lblPrioridade = new Label()
            {
                Text = "Nova prioridade:",
                Location = new Point(20, 440),
                Width = 130
            };
            panelCorpo.Controls.Add(lblPrioridade);

            // ComboBox para selecionar prioridade
            var cbPrioridades = new ComboBox()
            {
                Location = new Point(180, 440),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbPrioridades.Items.AddRange(new object[]
            {
                ProcessPriorityClass.Idle,
                ProcessPriorityClass.BelowNormal,
                ProcessPriorityClass.Normal,
                ProcessPriorityClass.AboveNormal,
                ProcessPriorityClass.High,
                ProcessPriorityClass.RealTime
            });
            cbPrioridades.SelectedIndex = 2; // Normal
            panelCorpo.Controls.Add(cbPrioridades);

            var btnAlterarPrioridade = new Button()
            {
                Text = "Alterar Prioridade",
                Location = new Point(410, 440),
                Width = 150
            };
            panelCorpo.Controls.Add(btnAlterarPrioridade);

            // Botão para encerrar processo
            var btnEncerrarProcesso = new Button()
            {
                Text = "Encerrar Processo",
                Location = new Point(420, 400),
                Width = 150
            };
            panelCorpo.Controls.Add(btnEncerrarProcesso);

            // Evento Buscar Processos Remotos
            btnConectar.Click += async (s, ev) =>
            {
                string ip = txtIP.Text.Trim();
                if (string.IsNullOrWhiteSpace(ip))
                {
                    MessageBox.Show("Informe o IP da máquina remota.");
                    return;
                }

                ipRemotoValidacao = ip;
                filtroRemotoAtual = "";
                var processos = await Cliente.ObterProcessos(ip);

                if(processos == null || processos.Count == 0)
                {
                    MessageBox.Show("Nenhum processo encontrado ou erro ao conectar.");
                    return;
                }

                dgvProcessos.Rows.Clear();
                cbProcessos.Items.Clear();

                foreach (var processo in processos)
                {
                    dgvProcessos.Rows.Add(
                        processo.ProcessName,
                        processo.Id,
                        processo.Memoria.ToString("F2"),
                        processo.usoCpu.ToString("F2"),
                        processo.BasePriority
                    );
                    cbProcessos.Items.Add(new
                    {
                        Display = $"{processo.ProcessName} (PID: {processo.Id})",
                        Processo = processo
                    });
                }
                cbProcessos.DisplayMember = "Display";
                cbProcessos.ValueMember = "Processo";

                if (cbProcessos.Items.Count > 0)
                    cbProcessos.SelectedIndex = 0;

                if (!timerAtualizacaoRemota.Enabled)
                    timerAtualizacaoRemota.Start();

            };

            // Evento para buscar Processos Remoto por nome
            btnBuscar.Click += async (s, ev) =>
            {
                string ip = txtIP.Text.Trim();
                string termo = txtBuscaRemoto.Text.Trim();

                if (string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(termo))
                {
                    MessageBox.Show("Informe o IP e o termo de busca.");
                    return;
                }

                var lista = await Cliente.BuscarProcessosPorNome(ip, termo);

                dgvProcessos.Rows.Clear();
                cbProcessos.Items.Clear();

                foreach (var p in lista)
                {
                    dgvProcessos.Rows.Add(
                        p.ProcessName,
                        p.Id,
                        p.Memoria.ToString("F2"),
                        p.usoCpu.ToString("F2"),
                        p.BasePriority
                    );

                    // Adiciona ao ComboBox
                    cbProcessos.Items.Add(new
                    {
                        Display = $"{p.ProcessName} (PID: {p.Id})",
                        Processo = p
                    });
                }

                cbProcessos.DisplayMember = "Display";
                cbProcessos.ValueMember = "Processo";

                if (cbProcessos.Items.Count > 0)
                    cbProcessos.SelectedIndex = 0;
            };

            // Evento para Alterar Prioridade do processo Remoto
            btnAlterarPrioridade.Click += async (s, ev) =>
            {
                if (cbProcessos.SelectedItem == null || cbPrioridades.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um processo e uma prioridade.");
                    return;
                }

                dynamic item = cbProcessos.SelectedItem;
                ProcessoRemoto processo = item.Processo;
                string ip = txtIP.Text.Trim();
                string novaPrioridade = cbPrioridades.SelectedItem.ToString();

                var resposta = await Cliente.AlterarPrioridade(ip, processo.Id, novaPrioridade);

                //Atualiza Lista
                var processosAtualizados = await Cliente.ObterProcessos(ip);
                dgvProcessos.Rows.Clear();
                foreach (var p in processosAtualizados)
                {
                    dgvProcessos.Rows.Add(
                        p.ProcessName,
                        p.Id,
                        p.Memoria.ToString("F2"),
                        p.usoCpu.ToString("F2"),
                        p.BasePriority
                    );
                }

                MessageBox.Show("Resposta do servidor: " + resposta);
            };

            //Encerrar Processo Remoto
            btnEncerrarProcesso.Click += async (s, ev) =>
            {
                if (cbProcessos.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um processo.");
                    return;
                }

                dynamic item = cbProcessos.SelectedItem;
                ProcessoRemoto processo = item.Processo;
                string ip = txtIP.Text.Trim();

                var resposta = await Cliente.EncerrarProcesso(ip, processo.Id);

                MessageBox.Show("Resposta do servidor: " + resposta);

                // Recarrega a lista
                var lista = await Cliente.ObterProcessos(ip);
                dgvProcessos.Rows.Clear();
                foreach (var p in lista)
                {
                    dgvProcessos.Rows.Add(
                        p.ProcessName,
                        p.Id,
                        p.Memoria.ToString("F2"),
                        p.usoCpu.ToString("F2"),
                        p.BasePriority
                    );
                }
            };


        }


        // Evento para atualizar a lista de processos a cada 8 segundos
        private void AtualizarListaDeProcessos()
        {
            dgvProcessos.Rows.Clear();
            dgvProcessos.DataSource = null;

            var processos = Process.GetProcesses();
            //Console.WriteLine(processos);

            TimeSpan intervalo = DateTime.Now - ultimaAtualizacao;
            ultimaAtualizacao = DateTime.Now;

            foreach (var processo in processos)
            {
                try
                {
                    double memoriaMB = processo.WorkingSet64 / (1024.0 * 1024.0);
                    TimeSpan cpuAtual = processo.TotalProcessorTime;

                    double usoCpu = 0;

                    if (cpuTimesAntigos.TryGetValue(processo.Id, out TimeSpan cpuAntigo))
                    {
                        usoCpu = (cpuAtual - cpuAntigo).TotalMilliseconds / intervalo.TotalMilliseconds;
                        usoCpu = usoCpu * 100 / Environment.ProcessorCount;
                    }

                    // Atualiza o tempo atual no dicionário
                    cpuTimesAntigos[processo.Id] = cpuAtual;

                    int index = dgvProcessos.Rows.Add();
                    dgvProcessos.Rows[index].Cells["Nome"].Value = processo.ProcessName;
                    dgvProcessos.Rows[index].Cells["PID"].Value = processo.Id;
                    dgvProcessos.Rows[index].Cells["Memoria"].Value = memoriaMB.ToString("F2");
                    dgvProcessos.Rows[index].Cells["CPU"].Value = usoCpu.ToString("F2");
                    dgvProcessos.Rows[index].Cells["Prioridade"].Value = processo.BasePriority;
                }

                catch
                {
                    // ignora processos protegidos ou que já foram finalizados
                }
            }
        }

    }

}
