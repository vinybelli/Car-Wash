using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lava_Car.Cadastros.Despesas.Classes
{
    public class Classe_Despesas
    {
        public int Id { get; set; }
        public string Despesa { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public DateTime Data_Alteracao { get; set; }
        public bool Excluido { get; set; }
    }
}
