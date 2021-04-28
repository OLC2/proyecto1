using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace _COMPI1_Proyecto2_2S
{
    class Gramatica : Grammar
    {

        public Gramatica() : base(caseSensitive: true) //True es Case Sensitive - False es No Case Sensitive
        {
            //TERMINALES
            KeyTerm puntocoma = ToTerm(";");
            KeyTerm punto = ToTerm(".");
            KeyTerm dospuntos = ToTerm(":");
            KeyTerm coma = ToTerm(",");
            KeyTerm mas = ToTerm("+");
            KeyTerm menos = ToTerm("-");
            KeyTerm por = ToTerm("*");
            KeyTerm division = ToTerm("/");
            KeyTerm modulo = ToTerm("%");
            KeyTerm potencia = ToTerm("^");
            KeyTerm opSuma = ToTerm("+=");
            KeyTerm opResta = ToTerm("-=");
            KeyTerm incrementa = ToTerm("++");
            KeyTerm decrementa = ToTerm("--");
            KeyTerm parentA = ToTerm("(");
            KeyTerm parentC = ToTerm(")");
            KeyTerm llaveA = ToTerm("{");
            KeyTerm llaveC = ToTerm("}");
            KeyTerm corchA = ToTerm("[");
            KeyTerm corchC = ToTerm("]");
            ////DECLARACION DE TERMINALES POR MEDIO DE ER.
            IdentifierTerminal id = new IdentifierTerminal("id");
            RegexBasedTerminal numero = new RegexBasedTerminal("numero", "[0-9]+");
            RegexBasedTerminal Decimal = new RegexBasedTerminal("Decimal", "[0-9]+[.][0-9]+");
            StringLiteral cadena = new StringLiteral("cadena", "\"", StringOptions.IsTemplate);
            StringLiteral caracter = TerminalFactory.CreateCSharpChar("CharLiteral");
            //NO TERMINALES
            NonTerminal S = new NonTerminal("S");
            NonTerminal LSIMPORT = new NonTerminal("LSIMPORT");
            NonTerminal IMPORT = new NonTerminal("IMPORT");
            NonTerminal ESTRUCTURA = new NonTerminal("ESTRUCTURA");
            NonTerminal BLOQUE = new NonTerminal("BLOQUE");
            NonTerminal VARGLOBAL = new NonTerminal("VARGLOBAL");
            NonTerminal LSTIDG = new NonTerminal("LSTIDG");
            NonTerminal FUNCIONES = new NonTerminal("FUNCIONES");
            NonTerminal METODOS = new NonTerminal("METODOS");
            NonTerminal MAIN = new NonTerminal("MAIN");
            NonTerminal PARAMETROS = new NonTerminal("PARAMETROS");
            NonTerminal PARAMETRO = new NonTerminal("PARAMETRO");
            NonTerminal ASIGNAR_PARAMETRO = new NonTerminal("ASIGNAR_PARAMETRO");
            NonTerminal SENTENCIAS = new NonTerminal("SENTENCIAS");
            NonTerminal SENTENCIA = new NonTerminal("SENTENCIA");
            NonTerminal IFF = new NonTerminal("IFF");
            NonTerminal ELSEE = new NonTerminal("ELSEE");
            NonTerminal WHILEE = new NonTerminal("WHILEE");
            NonTerminal DOWHILEE = new NonTerminal("DOWHILEE");
            NonTerminal PRINT = new NonTerminal("PRINT");
            NonTerminal VARLOCAL = new NonTerminal("VARLOCAL");
            NonTerminal LSTIDL = new NonTerminal("LSTIDL");
            NonTerminal OPERAFOR = new NonTerminal("OPERAFOR");
            NonTerminal LSTARREGLO = new NonTerminal("LSTARREGLO");
            NonTerminal LSCASOS = new NonTerminal("LSCASOS");
            NonTerminal CASO = new NonTerminal("CASO");
            NonTerminal DEFAUL = new NonTerminal("DEFAUL");
            NonTerminal LSSWITCH = new NonTerminal("LSSWITCH");
            NonTerminal TERMCASO = new NonTerminal("TERMCASO");
            NonTerminal CONDICION = new NonTerminal("CONDICION");
            NonTerminal COND1 = new NonTerminal("COND1");
            NonTerminal COND2 = new NonTerminal("COND2");
            NonTerminal COND3 = new NonTerminal("COND3");
            NonTerminal COND4 = new NonTerminal("COND4");
            NonTerminal COND5 = new NonTerminal("COND5");
            NonTerminal COND6 = new NonTerminal("COND6");
            NonTerminal COND7 = new NonTerminal("COND7");
            NonTerminal COND8 = new NonTerminal("COND8");
            NonTerminal COND9 = new NonTerminal("COND9");
            NonTerminal EXPRESION = new NonTerminal("EXPRESION");
            NonTerminal EXP1 = new NonTerminal("EXP1");
            NonTerminal EXP2 = new NonTerminal("EXP2");
            NonTerminal EXP3 = new NonTerminal("EXP3");
            NonTerminal EXP4 = new NonTerminal("EXP4");
            NonTerminal EXP5 = new NonTerminal("EXP5");
            NonTerminal TERMINALES = new NonTerminal("TERMINALES");
            NonTerminal TIPODATOF = new NonTerminal("TIPODATOF");
            NonTerminal TIPODATO = new NonTerminal("TIPODATO");

            CommentTerminal comentarioLinea = new CommentTerminal("comentarioLinea", "//", "\n", "\r\n");//Comentario de una Linea
            CommentTerminal comentarioBloque = new CommentTerminal("comentarioBloque", "/*", "*/");//Comentario de muchas lineas
            //CommentTerminal comentLine = new CommentTerminal("comentLine", "{", "}", "\r\n");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            //GRAMATICA
            S.Rule = LSIMPORT + ESTRUCTURA
                    | ESTRUCTURA
                    ;
            LSIMPORT.Rule = LSIMPORT + IMPORT
                            | IMPORT
                            ;
            IMPORT.Rule = ToTerm("Import") + cadena + puntocoma
                        ;
            ESTRUCTURA.Rule = ESTRUCTURA + BLOQUE
                            | BLOQUE
                            ;
            BLOQUE.Rule = VARGLOBAL
                        | FUNCIONES
                        | MAIN
                        ;
            VARGLOBAL.ErrorRule = SyntaxError + puntocoma;
            VARGLOBAL.Rule = LSTIDG + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id
                            | LSTIDG + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion y asignacion de arreglo, Condicion es entero, LSTIDG es solo un id, LSTARREGLO es del mismo tipo que Condicion
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id, id es un arreglo
                            | id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                            | id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                            ;
            LSTIDG.Rule = LSTIDG + coma + id
                    | id
                    ;
                                    // LSTIDG <-- SOLO DEBERIA DE TENER UN ID, Y NO UNA LISTA, ESTO ES PARA QUE NO DE ERROR DE AMBIGUEDAD
            FUNCIONES.ErrorRule = SyntaxError + llaveC;
            FUNCIONES.Rule = LSTIDG + dospuntos + TIPODATOF + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //Void no lleva return
                            | LSTIDG + dospuntos + TIPODATOF + parentA + parentC + llaveA + SENTENCIAS + llaveC //Void no lleva return
                            ;
            MAIN.ErrorRule = SyntaxError + llaveC;
            MAIN.Rule = ToTerm("Main") + dospuntos + TIPODATOF + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //VOID NO LLEVA RETURN
                        | ToTerm("Main") + dospuntos + TIPODATOF + parentA + parentC + llaveA + SENTENCIAS + llaveC //VOID NO LLEVA RETURN
                        ;
            PARAMETROS.Rule = PARAMETROS + coma + PARAMETRO
                            | PARAMETRO
                            ;
            PARAMETRO.Rule = id + dospuntos + TIPODATO
                            ;
            SENTENCIAS.Rule = SENTENCIAS + SENTENCIA
                            | SENTENCIA
                            ;
            SENTENCIA.ErrorRule = SyntaxError + llaveC;
            SENTENCIA.ErrorRule = SyntaxError + puntocoma;
            SENTENCIA.ErrorRule = SyntaxError + parentC;
            SENTENCIA.Rule = VARLOCAL
                            | id + parentA + ASIGNAR_PARAMETRO + parentC + puntocoma //Invocacion de metodo con parametros
                            | id + parentA + parentC + puntocoma //Invocacion de metodo sin parametros
                            | ToTerm("If") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //IF
                            | ToTerm("If") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC + ToTerm("Else") + llaveA + SENTENCIAS + llaveC // IF ELSE
                            | ToTerm("While") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //WHILE
                            | ToTerm("Do") + llaveA + SENTENCIAS + llaveC + ToTerm("While") + parentA + CONDICION + parentC //DO-WHILE
                            | ToTerm("Switch") + parentA + CONDICION + parentC + llaveA + LSCASOS + DEFAUL + llaveC //Strings, Double e Int son los tipos de datos unicos que puede operar esta estructura
                            | ToTerm("For") + parentA + id + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma + CONDICION + puntocoma + OPERAFOR + parentC + llaveA + SENTENCIAS + llaveC //FOR
                            | ToTerm("Print") + parentA + CONDICION + parentC + puntocoma //PRINT
                            | ToTerm("Return") + CONDICION + puntocoma //Para funciones unicamente
                            | ToTerm("Return") + puntocoma //Para metodos unicamente
                            | ToTerm("Break") + puntocoma //Unicamente funcional para Switch, While y For //Colocarle una bandera para su uso                            
                            ;
            VARLOCAL.ErrorRule = SyntaxError + puntocoma;
            VARLOCAL.Rule = LSTIDL + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id
                            | LSTIDL + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion y asignacion de arreglo, Condicion es entero, LSTIDL es solo un id, LSTARREGLO es del mismo tipo que Condicion
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id, id es un arreglo
                            | id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                            | id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                            | id + incrementa + puntocoma //id tipo Int, Double
                            | id + decrementa + puntocoma //id tipo Int, Double
                            | id + corchA + CONDICION + corchC + ToTerm("=") + CONDICION + puntocoma //Asignando valor a posicion de arreglo
                            ;
            LSTIDL.Rule = LSTIDL + coma + id
                    | id
                    ;
            OPERAFOR.Rule = id + incrementa
                            | id + decrementa
                            ;
            LSTARREGLO.Rule = LSTARREGLO + coma + CONDICION
                            | CONDICION
                            ;
            LSCASOS.Rule = LSCASOS + CASO
                            | CASO
                            ;
            CASO.Rule = ToTerm("Case") + TERMCASO + dospuntos + SENTENCIAS
                        ;
            DEFAUL.Rule = ToTerm("Default") + dospuntos + SENTENCIAS
                        | Empty //Vacio 
                        ;
            LSSWITCH.Rule = LSSWITCH + SENTENCIA
                            | SENTENCIA
                            | ToTerm("Break") + puntocoma
                            ;
            TERMCASO.Rule = numero //int
                            | Decimal //double
                            | menos + numero //int
                            | menos + Decimal //double
                            | cadena //String
                            ;
            //CONDICIONES
            CONDICION.Rule = CONDICION + ToTerm("||") + COND1
                        | COND1
                        ;
            COND1.Rule = COND1 + ToTerm("!&") + COND2
                        | COND2
                        ;
            COND2.Rule = COND2 + ToTerm("&&") + COND3
                        | COND3
                        ;
            COND3.Rule = ToTerm("!") + COND4
                        | COND4
                        ;
            COND4.Rule = COND4 + ToTerm("<=") + COND5
                        | COND5
                        ;
            COND5.Rule = COND5 + ToTerm(">=") + COND6
                        | COND6
                        ;
            COND6.Rule = COND6 + ToTerm("<") + COND7
                        | COND7
                        ;
            COND7.Rule = COND7 + ToTerm(">") + COND8
                        | COND8
                        ;
            COND8.Rule = COND8 + ToTerm("==") + COND9
                        | COND9
                        ;
            COND9.Rule = COND9 + ToTerm("!=") + EXPRESION
                        | EXPRESION
                        ;
            //EXPRESIONES
            EXPRESION.Rule = EXPRESION + mas + EXP1
                        | EXP1
                        ;
            EXP1.Rule = EXP1 + menos + EXP2
                        | EXP2
                        ;
            EXP2.Rule = EXP2 + por + EXP3
                        | EXP3
                        ;
            EXP3.Rule = EXP3 + division + EXP4
                        | EXP4
                        ;
            EXP4.Rule = EXP4 + modulo + EXP5
                        | EXP5
                        ;
            EXP5.Rule = EXP5 + potencia + TERMINALES
                        | TERMINALES
                        ;
            TERMINALES.Rule = numero
                            | Decimal
                            | menos + numero // Especial coco a esto
                            | menos + Decimal // Especial coco a esto
                            | cadena
                            | caracter //CharLiteral
                            | ToTerm("True")
                            | ToTerm("False")
                            | ToTerm("GetUser") + parentA + parentC
                            | id // Esto puede ser una VARIABLE o un ARREGLO
                            | id + punto + ToTerm("CompareTo") + parentA + CONDICION + parentC // Condicion debe ser tipo String, sino error
                            | id + parentA + parentC // Invocacion funcion si parametros
                            | id + parentA + ASIGNAR_PARAMETRO + parentC // Invocacion funcion con parametros
                            | id + corchA + CONDICION + corchC // Obteniendo valor de un arreglo, condicion debe ser entero para acceder a esa posicion del arreglo
                            | parentA + CONDICION + parentC
                            ;
            ASIGNAR_PARAMETRO.Rule = ASIGNAR_PARAMETRO + coma + CONDICION
                                    | CONDICION
                                    ;
            TIPODATOF.Rule = ToTerm("Boolean")
                            | ToTerm("Double")
                            | ToTerm("String")
                            | ToTerm("Int")
                            | ToTerm("Char")
                            | ToTerm("Void")
                            ;
            TIPODATO.Rule = ToTerm("Boolean")
                            | ToTerm("Double")
                            | ToTerm("String")
                            | ToTerm("Int")
                            | ToTerm("Char")
                            ;
            

            this.Root = S;
        }
    }
}
