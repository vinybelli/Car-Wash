using Lava_Car.Cadastros.Despesas.Classes;
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

namespace Lava_Car.Cadastros.Despesas
{
    public partial class Gestao_Despesas : Form
    {
        List<Classe_Despesas> Despesas = new List<Classe_Despesas>();

        public Gestao_Despesas()
        {
            InitializeComponent();

            int ano = DateTime.Now.Year;
            int mes = DateTime.Now.Month;

            int diasMes = DateTime.DaysInMonth(ano, mes);

            dateTimePicker3.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            dateTimePicker4.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, diasMes) ;

            AtualizarDespesas();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Cadastro_Despesas cadastroPedido = new Cadastro_Despesas(false, "");

            cadastroPedido.ShowDialog();

            AtualizarDespesas();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int linha = dataGridView5.SelectedCells[0].RowIndex;

            Cadastro_Despesas cadastroPedido = new Cadastro_Despesas(true, dataGridView5.Rows[linha].Cells[0].Value.ToString());

            cadastroPedido.ShowDialog();

            AtualizarDespesas();
        }

        private void dataGridView5_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int linha = e.RowIndex;

            Cadastro_Despesas cadastroPedido = new Cadastro_Despesas(true, dataGridView5.Rows[linha].Cells[0].Value.ToString());

            cadastroPedido.ShowDialog();

            AtualizarDespesas();
        }


        public void AtualizarDespesas()
        {
            Despesas.Clear();

            BuscarDespesasBD();

            dataGridView5.Rows.Clear();

            int qntdDespesas = 0;
            decimal valorTotalDespesas = 0;

            for (int i = 0; i < Despesas.Count; i++)
            {
                dataGridView5.Rows.Add(Despesas[i].Id,
                Despesas[i].Data.ToString("dd/MM/yyyy"),
                Despesas[i].Despesa,
                Despesas[i].Valor,
                Despesas[i].Observacao);

                qntdDespesas++;

                valorTotalDespesas += Despesas[i].Valor;
            }

            label7.Text = valorTotalDespesas.ToString();
            label8.Text = qntdDespesas.ToString();
        }

        public void BuscarDespesasBD()
        {
            string connectionString = "Server=localhost;Database=postgres;User Id=postgres;Password=123;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("" +
                    "SELECT * " +
                    "FROM Despesas " +
                    "WHERE Excluido = 'false' and Data >= '" + dateTimePicker3.Value.ToString("yyyy-MM-dd 00:00:00") + "' AND Data <= '" + dateTimePicker4.Value.ToString("yyyy-MM-dd 23:59:59") + "' " +
                    "ORDER BY Id ASC", connection))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.HasRows == true)
                        {
                            int colunaId = dr.GetOrdinal("Id");
                            int colunaDespesa = dr.GetOrdinal("Despesa");
                            int colunaValor = dr.GetOrdinal("Valor");
                            int colunaObservacao = dr.GetOrdinal("Observacao");
                            int colunaData = dr.GetOrdinal("Data");

                            while (dr.Read())
                            {
                                Classe_Despesas despesa = new Classe_Despesas();

                                despesa.Id = dr.GetInt32(colunaId);
                                despesa.Despesa = dr.GetString(colunaDespesa);
                                despesa.Valor = dr.GetDecimal(colunaValor);
                                despesa.Observacao = dr.GetString(colunaObservacao);
                                despesa.Data = dr.GetDateTime(colunaData);

                                Despesas.Add(despesa);
                            }
                        }
                    }
                }
            }
        }

        private void Gestao_Despesas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 27)
            {
                this.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AtualizarDespesas();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
