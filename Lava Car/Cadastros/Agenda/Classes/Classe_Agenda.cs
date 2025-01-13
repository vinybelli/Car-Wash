using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lava_Car.Cadastros.Agenda.Classes
{
    internal class Classe_Agenda
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Horario { get; set; }
        public string Cliente { get; set; }
        public int Id_Cliente { get; set; }
        public string Veiculo { get; set; }
        public string Servico { get; set; }
        public string Observacao { get; set; }
        public bool Excluido { get; set; }
    }
}
