using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _COMPI1_Proyecto2_2S.Ejecucion
{
    class Simbolo
    {
        public String Ambito;
        public String Nombre;
        public String Valor; //PARA UN ARREGLO EL VALOR SERAN LAS DIMENSIONES
        public String Tipo;
        public String TipoObjeto; //VARIABLE || ARREGLO
        public String Linea;
        public String Columna;
        public Boolean Activo;
        public List<Celda> Arreglo;

        public Simbolo(string ambito, string nombre, string valor, string tipo, String tipoobjeto, string linea, string columna, Boolean activo, List<Celda> Arreglo)
        {
            this.Ambito = ambito;
            this.Nombre = nombre;
            this.Valor = valor;
            this.Tipo = tipo;
            this.TipoObjeto = tipoobjeto;
            this.Linea = linea;
            this.Columna = columna;
            this.Activo = activo;
            this.Arreglo = Arreglo;
        }
    }
}
