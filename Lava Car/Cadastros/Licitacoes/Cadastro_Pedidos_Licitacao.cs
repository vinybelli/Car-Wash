﻿using Lava_Car.Cadastros.Agenda.Classes;
using Lava_Car.Cadastros.Clientes;
using Lava_Car.Cadastros.Clientes.Classes;
using Lava_Car.Cadastros.Pedidos.Classes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lava_Car.Cadastros.Licitacoes
{
    public partial class Cadastro_Pedidos_Licitacao : Form
    {
        List<Classe_Clientes> ClientesPj = new List<Classe_Clientes>();

        Classe_Pedidos Pedido = new Classe_Pedidos();

        Classe_Agenda Agenda = new Classe_Agenda();

        string Id = "";

        bool EditarPedidoLicitacao = false;

        bool CadastrarAgendamento = false;

        public Cadastro_Pedidos_Licitacao(bool editarPedidoLicitacao, string id, bool cadastrarAgendamento)
        {
            InitializeComponent();

            AtualizarClientesPj();

            EditarPedidoLicitacao = editarPedidoLicitacao;
            CadastrarAgendamento = cadastrarAgendamento;
            Id = id;

            comboBox2.SelectedIndex = 3;

            if (editarPedidoLicitacao)
            {
                button7.Visible = true;
                
                BuscarPedido();
            }
            else
            {
                button7.Visible = false;
            }

            if (CadastrarAgendamento)
            {
                BuscarDadosAgenda();
            }

            comboBox1.Select();
        }

        public void AtualizarClientesPj()
        {
            ClientesPj.Clear();

            BuscarClientesPjBD();

            comboBox1.Items.Clear();

            for (int i = 0; i < ClientesPj.Count; i++)
            {
                comboBox1.Items.Add(ClientesPj[i].Nome_Razao);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        public void BuscarClientesPjBD()
        {
            string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("" +
                    "SELECT * " +
                    "FROM Clientes " +
                    "WHERE Tipo_Cliente = 'Pessoa Jurídica' AND Excluido = 'false' " +
                    "ORDER BY Nome_Razao ASC", connection))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            int colunaCliente = dr.GetOrdinal("Nome_Razao");
                            int colunaId = dr.GetOrdinal("Id");

                            while (dr.Read())
                            {
                                Classe_Clientes clientePj = new Classe_Clientes();

                                clientePj.Nome_Razao = dr.GetString(colunaCliente);
                                clientePj.Id = dr.GetInt32(colunaId);

                                ClientesPj.Add(clientePj);
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cadastro_Cliente clientes = new Cadastro_Cliente(false, "");

            clientes.ShowDialog();

            AtualizarClientesPj();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DeletarPedidoLicitacao();
        }

        private void DeletarPedidoLicitacao()
        {
            DialogResult confirmacao = MessageBox.Show("Deseja realmente Excluir este Pedido de Licitação?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacao == DialogResult.No)
            {
                return;
            }

            ExcluirPedidoBD();

            MessageBox.Show("Pedido de Licitação excluído com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        private void ExcluirPedidoBD()
        {
            string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(
                    "UPDATE Pedidos_Licitacao " +
                    "SET Excluido = 'true', Data_Alteracao = '" + DateTime.Now + "' " +
                    "WHERE Id = '" + Id + "'", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Cadastro_Pedidos_Licitacao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 113)
            {
                Salvar();
            }

            if (e.KeyValue == 46)
            {
                DeletarPedidoLicitacao();
            }
        }

        private void Cadastro_Pedidos_Licitacao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                this.Close();
            }
        }


        public void Salvar()
        {
            if (comboBox1.SelectedIndex == -1 || comboBox2.Text == "" || textBox3.Text == "" || comboBox3.Text == "")
            {
                MessageBox.Show("Todos os campos destacados em amarelo devem ser preenchidos!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal valor;

            if (decimal.TryParse(textBox3.Text, out valor) == false)
            {
                MessageBox.Show("O valor do Pedido deve ser numérico!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacao = MessageBox.Show("Deseja inserir este Pedido de Licitação?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacao == DialogResult.No)
            {
                return;
            }

            SalvarPedidoBD();

            MessageBox.Show("Pedido de Licitação inserido com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        public void SalvarPedidoBD()
        {
            if (EditarPedidoLicitacao)
            {
                string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "UPDATE Pedidos_Licitacao SET " +
                        "Cliente = @cliente, " +
                        "Id_Cliente = @idCliente, " +
                        "Veiculo = @veiculo, " +
                        "Placa = @placa, " +
                        "Servico = @servico, " +
                        "Data_Alteracao = @dataAlteracao, " +
                        "Valor = CAST(@valor AS NUMERIC), " +
                        "Observacao = @observacao, " +
                        "Forma_Pagamento = @formaPagamento, " +
                        "Situacao = @situacao " +
                        "WHERE Id = '" + Id + "'", connection))
                    {
                        command.Parameters.AddWithValue("@cliente", comboBox1.Text);
                        if (comboBox1.Text == Pedido.Cliente)
                        {
                            command.Parameters.AddWithValue("@idCliente", Pedido.Id_Cliente);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@idCliente", ClientesPj[comboBox1.SelectedIndex].Id);
                        }
                        command.Parameters.AddWithValue("@veiculo", textBox6.Text);
                        command.Parameters.AddWithValue("@placa", maskedTextBox1.Text);
                        command.Parameters.AddWithValue("@servico", comboBox2.Text);
                        command.Parameters.AddWithValue("@dataAlteracao", DateTime.Now);
                        command.Parameters.AddWithValue("@valor", textBox3.Text.Replace(",", "."));
                        command.Parameters.AddWithValue("@observacao", textBox1.Text);
                        command.Parameters.AddWithValue("@formaPagamento", comboBox4.Text);
                        command.Parameters.AddWithValue("@situacao", comboBox3.Text);
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "INSERT INTO Pedidos_Licitacao (Cliente, Id_Cliente, Veiculo, Placa, Servico, Data, Data_Alteracao, Valor, Observacao, Forma_Pagamento, Situacao) " +
                        "VALUES (@cliente, @idCliente, @veiculo, @placa, @servico, @data, @dataAlteracao, CAST(@valor AS NUMERIC), @observacao, @formaPagamento, @situacao)", connection))
                    {
                        command.Parameters.AddWithValue("@cliente", comboBox1.Text);
                        command.Parameters.AddWithValue("@idCliente", ClientesPj[comboBox1.SelectedIndex].Id);
                        command.Parameters.AddWithValue("@veiculo", textBox6.Text);
                        command.Parameters.AddWithValue("@placa", maskedTextBox1.Text);
                        command.Parameters.AddWithValue("@servico", comboBox2.Text);
                        command.Parameters.AddWithValue("@data", DateTime.Now);
                        command.Parameters.AddWithValue("@dataAlteracao", DateTime.Now);
                        command.Parameters.AddWithValue("@valor", textBox3.Text.Replace(",", "."));
                        command.Parameters.AddWithValue("@observacao", textBox1.Text);
                        command.Parameters.AddWithValue("@formaPagamento", comboBox4.Text);
                        command.Parameters.AddWithValue("@situacao", comboBox3.Text);
                        command.ExecuteNonQuery();
                    }
                }
            }

            if (CadastrarAgendamento)
            {
                //Exclui o agendamento após inserir no BD
                string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "UPDATE Agendamento " +
                        "SET Excluido = 'true' " +
                        "WHERE Id = '" + Id + "'", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Salvar();
        }

        public void BuscarPedido()
        {
            BuscarPedidoBD();

            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.Items[i].ToString() == Pedido.Cliente)
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.Items[i].ToString() == Pedido.Servico)
                {
                    comboBox2.SelectedIndex = i;
                    break;
                }
            }

            textBox6.Text = Pedido.Veiculo;
            maskedTextBox1.Text = Pedido.Placa;
            textBox3.Text = Pedido.Valor.ToString();
            textBox1.Text = Pedido.Observacao;

            for (int i = 0; i < comboBox4.Items.Count; i++)
            {
                if (comboBox4.Items[i].ToString() == Pedido.Forma_Pagamento)
                {
                    comboBox4.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < comboBox3.Items.Count; i++)
            {
                if (comboBox3.Items[i].ToString() == Pedido.Situacao)
                {
                    comboBox3.SelectedIndex = i;
                    break;
                }
            }
        }

        public void BuscarPedidoBD()
        {
            string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("" +
                    "SELECT * " +
                    "FROM Pedidos_Licitacao " +
                    "WHERE Id = '" + int.Parse(Id) + "'", connection))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            int colunaId = dr.GetOrdinal("Id");
                            int colunaCliente = dr.GetOrdinal("Cliente");
                            int colunaIdCliente = dr.GetOrdinal("Id_Cliente");
                            int colunaVeiculo = dr.GetOrdinal("Veiculo");
                            int colunaPlaca = dr.GetOrdinal("Placa");
                            int colunaServico = dr.GetOrdinal("Servico");
                            int colunaData = dr.GetOrdinal("Data");
                            int colunaValor = dr.GetOrdinal("Valor");
                            int colunaFormaPag = dr.GetOrdinal("Forma_Pagamento");
                            int colunaSituacao = dr.GetOrdinal("Situacao");
                            int colunaObservacao = dr.GetOrdinal("Observacao");

                            while (dr.Read())
                            {
                                Pedido.Id = dr.GetInt32(colunaId);
                                Pedido.Cliente = dr.GetString(colunaCliente);
                                Pedido.Id_Cliente = dr.GetInt32(colunaIdCliente);
                                Pedido.Veiculo = dr.GetString(colunaVeiculo);
                                Pedido.Placa = dr.GetString(colunaPlaca);
                                Pedido.Servico = dr.GetString(colunaServico);
                                Pedido.Data = dr.GetDateTime(colunaData);
                                Pedido.Valor = dr.GetDecimal(colunaValor);
                                Pedido.Forma_Pagamento = dr.GetString(colunaFormaPag);
                                Pedido.Situacao = dr.GetString(colunaSituacao);
                                Pedido.Observacao = dr.GetString(colunaObservacao);
                            }
                        }
                    }
                }
            }
        }

        public void BuscarDadosAgenda()
        {
            BuscarDadosAgendaBD();

            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.Items[i].ToString() == Agenda.Cliente)
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.Items[i].ToString() == Agenda.Servico)
                {
                    comboBox2.SelectedIndex = i;
                    break;
                }
            }

            textBox1.Text = Agenda.Observacao;
            textBox6.Text = Agenda.Veiculo;
        }

        public void BuscarDadosAgendaBD()
        {
            string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();


                using (var command = new NpgsqlCommand("" +
                    "SELECT * " +
                    "FROM Agendamento " +
                    "WHERE Id = '" + Id + "' " +
                    "ORDER BY Id ASC", connection))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            int colunaId = dr.GetOrdinal("Id");
                            int colunaCliente = dr.GetOrdinal("Cliente");
                            int colunaIdCliente = dr.GetOrdinal("Id_Cliente");
                            int colunaVeiculo = dr.GetOrdinal("Veiculo");
                            int colunaServico = dr.GetOrdinal("Servico");
                            int colunaObservacao = dr.GetOrdinal("Observacao");

                            while (dr.Read())
                            {
                                Agenda.Id = dr.GetInt32(colunaId);
                                Agenda.Cliente = dr.GetString(colunaCliente);
                                Agenda.Id_Cliente = dr.GetInt32(colunaIdCliente);
                                Agenda.Veiculo = dr.GetString(colunaVeiculo);
                                Agenda.Servico = dr.GetString(colunaServico);
                                Agenda.Observacao = dr.GetString(colunaObservacao);
                            }
                        }
                    }
                }
            }
        }
    }
}
