using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lava_Car.Cadastros.Clientes.Classes
{
    public class Classe_Clientes
    {
        public int Id { get; set; }
        public string Nome_Razao { get; set; }
        public string Nome_Fantasia { get; set; }
        public string CPF_CNPJ { get; set; }
        public string IE { get; set; }
        public string Tipo_Cliente { get; set; }
        public string Telefone { get; set; }
        public DateTime Data { get; set; }
        public DateTime Data_Alteracao { get; set; }
        public bool Excluido { get; set; }
    }
}
