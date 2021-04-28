using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _COMPI1_Proyecto2_2S.Ejecucion
{
    class Error
    {
        public string Tipo;
        public string Descripcion;
        public string Linea;
        public string Columna;

        public Error(string Tipo, string Descripcion, string Linea, string Columna)
        {
            this.Tipo = Tipo;
            this.Descripcion = Descripcion;
            this.Linea = Linea;
            this.Columna = Columna;
        }
    }
}
