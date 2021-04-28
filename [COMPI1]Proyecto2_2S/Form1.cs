using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Threading;
using Irony.Parsing;
using System.Collections;
using _COMPI1_Proyecto2_2S.Ejecucion;

namespace _COMPI1_Proyecto2_2S
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void analizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(parametro.Text);

            List<Celda> arregloX = new List<Celda>();

            arregloX.Add(new Celda("String", parametro.Text.ToString()));

            Boolean resultado = Analizar.esCadenaValida(richTextBox1.Text);

            richTextBox2.Clear();
            richTextBox3.Clear();

            if (resultado)
            {
                MessageBox.Show("Analisis Exitoso");

                RetornoAc retorno = Analizar.EjecutarAccionesPerronas(arregloX);

                txtAreaChat.AppendText("[Ciente] " + parametro.Text.ToString()+"\n");

                if (!Analizar.ejecutar.lstError.Any())
                {
                    ImprimirEjecucion();                    
                }
                else
                {
                    ImprimirErrores();
                }

                if (retorno.RetornaVal) 
                {
                    MessageBox.Show("El Resultado de las acciones es: " + retorno.Valor);
                    txtAreaChat.AppendText("\t[Servidor] " + retorno.Valor.ToString() + "\n");
                }
                //else if (retorno.Detener)
                //{
                //    MessageBox.Show("El resultado de las acciones es incompleta debido a Break");
                //    txtAreaChat.AppendText("[Servidor] " + retorno.Valor.ToString());
                //}
                else if (retorno.Retorna)
                {
                    MessageBox.Show("Ejecucion Incompleta");
                    txtAreaChat.AppendText("\t[Servidor] Ejecucion Incompleta" + "\n");
                }
                //txtAreaChat.AppendText("[Servidor] " + retorno.Valor.ToString());
            }
            else
            {
                MessageBox.Show("Entrada contiene errores lexicos o sintacticos");
                ImprimirErroresLexicoSintactico();
            }
        }

        private void abrirArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Title = "Abrir";
            abrir.FileName = "";
            var resultado = abrir.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                StreamReader leer = new StreamReader(abrir.FileName);
                richTextBox1.Text = leer.ReadToEnd();
                leer.Close();
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(saveFile.FileName, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(richTextBox1.Text);
                }
            }
        }

        private void guardarComiendoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void graficarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraficarArbol(Analizar.arbol.Root);
        }

        private void testerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\t\t\t\t\t============================= TABLA SIMBOLOS =============================");
            if(Analizar.ejecutar != null)
            {
                if (Analizar.ejecutar.pilaSimbolos != null)
                {
                    foreach (TablaSimbolos sim in Analizar.ejecutar.pilaSimbolos)
                    {
                        Console.WriteLine("\t\t\t\t\t\t-----------------------------------" + sim.Tipo + "[" + sim.Nivel + "]-----------------------------------");
                        foreach (Simbolo ts in sim.ts)
                        {
                            Console.WriteLine("\t\t\t\t\t\t\t\t" + ts.Tipo + "[" + ts.TipoObjeto + "]" + " " + ts.Nombre + " = " + ts.Valor + " fila:" + ts.Linea + " col:" + ts.Columna);

                            if (ts.TipoObjeto.Equals(Reservada.arreglo))
                            {
                                foreach (Celda cel in ts.Arreglo)
                                {
                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t" + ts.Nombre + ":" + ts.Tipo + "[" + cel.tipo + "-" + cel.valor + "]");
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("\t\t\t\t\t============================= TABLA FUNCIONES =============================");
                foreach (Funciones fun in Analizar.ejecutar.tablafunciones.tf)
                {
                    Console.WriteLine(fun.getTipo() + " " + fun.getNombre() + " = " + fun.getValorRetorno() + " (" + fun.getKey() + ")" + " fila:" + fun.getLinea() + " col:" + fun.getColumna());
                    if (fun.getParametros() != null)
                    {
                        Console.WriteLine("\t\t\t\t\t\t------Parametros----- ");
                        foreach (Parametro param in fun.getParametros())
                        {
                            Console.WriteLine("\t\t\t\t\t\t\t" + param.Tipo + " - " + param.Nombre);
                        }
                        Console.WriteLine("\t\t\t\t\t\t--------------------- ");
                    }
                    else
                    {
                        Console.WriteLine("\t\t\t\t\t\t ---Sin parametros---");
                    }

                }
            }

            int i = 1;
            int j = 1;
            while(i <= 5)
            {
                while (j <= 5)
                {
                    
                    
                    Console.WriteLine(i + "+" + j + "=" + (i + j));
                    break;
                    //j++;
                }
                
                j = 1;
                i++;
            }

            String aaa = "Japón";//1331
            String bbb = "Japón";//1331

            int v1 = getCantAscii(aaa);
            int v2 = getCantAscii(bbb);
            
            if(v1 == v2)
            {
                MessageBox.Show("La cadena1(" + (v1) + ") es IGUAL que cadena2(" + (v2) + ")");
            }
            else if(v1 > v2)
            {
                MessageBox.Show("La cadena1("+(v1)+") es mayor que cadena2("+(v2)+")");
            }
            else
            {
                MessageBox.Show("La cadena2(" + (v2) + ") es mayor que cadena1(" + (v1) + ")");
            }
            

            MessageBox.Show("Testeando!"+division(200,25));

            MessageBox.Show("Fibonacci de 10 es !" + Fibonacci(10));
        }

        int division(int a, int b)
        {
            if(b > a){
                return 0;
            }
            else{
                return division(a-b, b) +1;
            }
        }

        private int getCantAscii(String cadena)
        {
            Char[] Caracter = cadena.ToCharArray();

            int SumaAscii = 0;

            for (int i = 0; i < Caracter.Length; i++)
            {
                SumaAscii += GetAscii(Caracter[i]+"");
            }
            return SumaAscii;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private int GetAscii(String caracter)
        {
            return Encoding.ASCII.GetBytes(caracter)[0];
        }

        int Fibonacci(int num)
        {
            if((num == 0) || (num == 1))
            {
                return num;
            }
            else
            {
                return (Fibonacci(num - 1) + Fibonacci(num - 2));
            }
        }


        private void ImprimirEjecucion()
        {
            if (Analizar.ejecutar.lstPrint.Any())
            {
                foreach (string prin in Analizar.ejecutar.lstPrint)
                {
                    richTextBox2.AppendText(prin + "\n");
                }
            }
            else
            {
                richTextBox2.AppendText("** No impresiones **" + "\n");
            }
        }

        private void ImprimirErrores()
        {
            if (Analizar.ejecutar.lstError.Any())
            {
                richTextBox3.AppendText("================== ERRORES SEMANTICOS ==================" + "\n");
                richTextBox3.AppendText("Linea" + "\t" + "Columna" + "\t" + "Tipo" + "\t\t" + "Descripcion" + "\n");
                foreach (Error error in Analizar.ejecutar.lstError)
                {
                    richTextBox3.AppendText(error.Linea + "\t" + error.Columna + "\t" + error.Tipo + "\t" + error.Descripcion + "\n");
                }
            }
            else
            {
                richTextBox3.AppendText("** No errores **" + "\n");
            }
        }

        private void ImprimirErroresLexicoSintactico()
        {
            richTextBox3.AppendText("================== ERRORES LEXICOS Y SINTACTICOS ==================" + "\n");
            richTextBox3.AppendText("Linea" + "\t" + "Columna" + "\t" + "Tipo" + "\t" + "Descripcion" + "\n");
            foreach (TokenError error in Analizar.lista_errores)
            {
                richTextBox3.AppendText(error.linea + "\t" + error.columna + "\t" + error.tipo + "\t" + error.descripcion + "\n");
            }
        }

        public void GraficarArbol(ParseTreeNode root)
        {
            try
            {
                String grafica = "Digraph Arbol_Sintactico{\n\n" + GraficaNodos(root, "0") + "\n\n}";
                FileStream stream = new FileStream("Arbol.dot", FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(grafica);
                writer.Close();
                //Ejecuta el codigo
                var command = string.Format("dot -Tjpg Arbol.dot -o Arbol.jpg");
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C " + command);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();

                Thread.Sleep(2000);
                Process.Start(@"Arbol.jpg");
            }
            catch (Exception x)
            {
                MessageBox.Show("Error inesperado cuando se intento graficar: " + x.ToString(), "error");
            }
        }

        private String GraficaNodos(ParseTreeNode nodo, String i)
        {
            int k = 0;
            String r = "";
            String nodoTerm = nodo.Term.ToString();
            nodoTerm = nodoTerm.Replace("\"", "");
            nodoTerm = nodoTerm.Replace("\\", "\\\\");
            r = "node" + i + "[label = \"" + nodoTerm + "\"];\n";

            for (int j = 0; j <= nodo.ChildNodes.Count() - 1; j++)
            {  // Nodos padres
                r = r + "node" + i + " -> node" + i + k + "\n";
                r = r + GraficaNodos(nodo.ChildNodes[j], "" + i + k);
                k++;
            }

            if (nodo.Token != null)
            {
                String nodoToken = nodo.Token.Text;
                nodoToken = nodoToken.Replace("\"", "");
                nodoToken = nodoToken.Replace("\\", "\\\\");
                if (nodo.ChildNodes.Count() == 0 && nodoTerm != nodoToken)
                {  // Nodos Hojas
                    r += "node" + i + "c[label = \"" + nodoToken + "\"];\n";
                    r += "node" + i + " -> node" + i + "c\n";
                }
            }

            return r;
        }

        private void erroresLexicosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("LexicosSintacticos.pdf", FileMode.Create));

            // Abrimos el archivo
            doc.Open();

            Paragraph title = new Paragraph(string.Format("REPORTE DE ERRORES LEXICOS Y SINTACTICOS"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Creamos el tipo de Font que vamos utilizar
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Escribimos el encabezamiento en el documento
            doc.Add(new Paragraph("Creado por Alex Ixva"));
            doc.Add(Chunk.NEWLINE);

            // Creamos una tabla que contendrá el nombre, apellido y país
            // de nuestros visitante.
            PdfPTable tblPrueba = new PdfPTable(5);
            tblPrueba.WidthPercentage = 100;

            // Configuramos el título de las columnas de la tabla
            PdfPCell clNumero = new PdfPCell(new Phrase("#", _standardFont));
            clNumero.BorderWidth = 1;
            clNumero.BorderWidthBottom = 0.75f;

            PdfPCell clTipo = new PdfPCell(new Phrase("Tipo", _standardFont));
            clTipo.BorderWidth = 1;
            clTipo.BorderWidthBottom = 0.75f;

            PdfPCell clDescripcion = new PdfPCell(new Phrase("Descripcion", _standardFont));
            clDescripcion.BorderWidth = 1;
            clDescripcion.BorderWidthBottom = 0.75f;

            PdfPCell clLinea = new PdfPCell(new Phrase("Linea", _standardFont));
            clLinea.BorderWidth = 1;
            clLinea.BorderWidthBottom = 0.75f;

            PdfPCell clColumna = new PdfPCell(new Phrase("Columna", _standardFont));
            clColumna.BorderWidth = 1;
            clColumna.BorderWidthBottom = 0.75f;

            // Añadimos las celdas a la tabla
            tblPrueba.AddCell(clNumero);
            tblPrueba.AddCell(clTipo);
            tblPrueba.AddCell(clDescripcion);
            tblPrueba.AddCell(clLinea);
            tblPrueba.AddCell(clColumna);
            
            // Llenamos la tabla con información
            int cont = 1;
            foreach (TokenError error in Analizar.lista_errores)
            {
                //richTextBox3.AppendText(error.linea + "\t" + error.columna + "\t" + error.tipo + "\t" + error.descripcion + "\n");
                clNumero = new PdfPCell(new Phrase(cont+"", _standardFont));
                clNumero.BorderWidth = 0;

                clTipo = new PdfPCell(new Phrase(error.tipo, _standardFont));
                clTipo.BorderWidth = 1;

                clDescripcion = new PdfPCell(new Phrase(error.descripcion, _standardFont));
                clDescripcion.BorderWidth = 0;

                clLinea = new PdfPCell(new Phrase(error.linea+"", _standardFont));
                clLinea.BorderWidth = 1;

                clColumna = new PdfPCell(new Phrase(error.columna+"", _standardFont));
                clColumna.BorderWidth = 0;

                // Añadimos las celdas a la tabla
                tblPrueba.AddCell(clNumero);
                tblPrueba.AddCell(clTipo);
                tblPrueba.AddCell(clDescripcion);
                tblPrueba.AddCell(clLinea);
                tblPrueba.AddCell(clColumna);

                cont++;
            }

            // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
            doc.Add(tblPrueba);

            doc.Close();
            writer.Close();

            MessageBox.Show("Genero PDF de errores Lexicos y Sintacticos");
        }

        private void erroresSintacticosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("Semanticos.pdf", FileMode.Create));

            // Abrimos el archivo
            doc.Open();

            Paragraph title = new Paragraph(string.Format("REPORTE DE ERRORES SEMANTICOS"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Creamos el tipo de Font que vamos utilizar
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Escribimos el encabezamiento en el documento
            doc.Add(new Paragraph("Creado por Alex Ixva"));
            doc.Add(Chunk.NEWLINE);

            // Creamos una tabla que contendrá el nombre, apellido y país
            // de nuestros visitante.
            PdfPTable tblPrueba = new PdfPTable(5);
            tblPrueba.WidthPercentage = 100;

            // Configuramos el título de las columnas de la tabla
            PdfPCell clNumero = new PdfPCell(new Phrase("#", _standardFont));
            clNumero.BorderWidth = 1;
            clNumero.BorderWidthBottom = 0.75f;

            PdfPCell clTipo = new PdfPCell(new Phrase("Tipo", _standardFont));
            clTipo.BorderWidth = 1;
            clTipo.BorderWidthBottom = 0.75f;

            PdfPCell clDescripcion = new PdfPCell(new Phrase("Descripcion", _standardFont));
            clDescripcion.BorderWidth = 1;
            clDescripcion.BorderWidthBottom = 0.75f;

            PdfPCell clLinea = new PdfPCell(new Phrase("Linea", _standardFont));
            clLinea.BorderWidth = 1;
            clLinea.BorderWidthBottom = 0.75f;

            PdfPCell clColumna = new PdfPCell(new Phrase("Columna", _standardFont));
            clColumna.BorderWidth = 1;
            clColumna.BorderWidthBottom = 0.75f;

            // Añadimos las celdas a la tabla
            tblPrueba.AddCell(clNumero);
            tblPrueba.AddCell(clTipo);
            tblPrueba.AddCell(clDescripcion);
            tblPrueba.AddCell(clLinea);
            tblPrueba.AddCell(clColumna);

            // Llenamos la tabla con información
            if (Analizar.ejecutar.lstError.Any())
            {
                int cont = 1;
                foreach (Error error in Analizar.ejecutar.lstError)
                {
                    clNumero = new PdfPCell(new Phrase(cont + "", _standardFont));
                    clNumero.BorderWidth = 0;

                    clTipo = new PdfPCell(new Phrase(error.Tipo, _standardFont));
                    clTipo.BorderWidth = 1;

                    clDescripcion = new PdfPCell(new Phrase(error.Descripcion, _standardFont));
                    clDescripcion.BorderWidth = 0;

                    clLinea = new PdfPCell(new Phrase(error.Linea, _standardFont));
                    clLinea.BorderWidth = 1;

                    clColumna = new PdfPCell(new Phrase(error.Columna, _standardFont));
                    clColumna.BorderWidth = 0;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(clNumero);
                    tblPrueba.AddCell(clTipo);
                    tblPrueba.AddCell(clDescripcion);
                    tblPrueba.AddCell(clLinea);
                    tblPrueba.AddCell(clColumna);

                    cont++;
                }
            }

            // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
            doc.Add(tblPrueba);

            doc.Close();
            writer.Close();

            MessageBox.Show("Genero PDF de errores Sematicos");
        }

        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
