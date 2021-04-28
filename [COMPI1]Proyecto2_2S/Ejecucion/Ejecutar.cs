using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;


namespace _COMPI1_Proyecto2_2S.Ejecucion
{
    class Ejecutar
    {
        public TablaFunciones tablafunciones = new TablaFunciones();

        public Stack<TablaSimbolos> pilaSimbolos;
        private TablaSimbolos cima;                     //Esto tiene el ambito actual
        ////public static TablaSimbolos cimaM;            //Esto tiene el ambito de la funcion actual, incluyendo al Main
        private TablaSimbolos cimaG;                    //Esto tiene el ambito de las variables globales

        private List<Parametro> lstParametros;          //Para los parametros de las funciones
        private List<Celda> arreglo;                    //Para los arreglos
        public List<String> lstPrint = new List<String>();
        public List<Error> lstError = new List<Error>();

        public String nameUsuario = "Alexander Ixvalan";

        private Boolean BanderaCaso = false; //Controla que los casos no repitan sus condiciones

        //private Boolean isRetornoG = false;
        //private Retorno RetornoG;

        //*********************** MINUTOS ANTES DE JODERME XD ------------------------------------------
        //**************************************

        //private int ContadorParams = 0;
        private int nivelActual = 1; //Este controla el nivel que se estara consultando para crear, buscar y modificar las variables locales dentro de metodos, condiciones, ciclos, etc...

        //=================================================================================================== PRIMERA PASADA ================================================================================

        public void IniciarPrimeraPasada(ParseTreeNode Nodo)
        { 
            pilaSimbolos = new Stack<TablaSimbolos>();

            //isRetornoG = false;

            TablaSimbolos varg = new TablaSimbolos(0, Reservada.varGlobal, false, false, false);
            pilaSimbolos.Push(varg);
            cimaG = varg;
            Console.WriteLine("ejecutando... Inserto TS - VariablesGlobales");
            
            PrimeraPasada(Nodo);
        }

        public void PrimeraPasada(ParseTreeNode Nodo)
        {
            //isRetornoG = false;
            
            if (Nodo != null)
            {
                switch (Nodo.Term.Name)
                {
                    case "S":
                        #region
                        if (Nodo.ChildNodes.Count == 2)
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> LSIMPORT
                            PrimeraPasada(Nodo.ChildNodes[1]); // ChildNodes[1] --> ESTRUCTURA
                        }
                        else
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> ESTRUCTURA
                        }
                        #endregion
                        break;
                    case "LSIMPORT":
                        #region
                        if (Nodo.ChildNodes.Count == 2)
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> LSIMPORT
                            PrimeraPasada(Nodo.ChildNodes[1]); // ChildNodes[1] --> IMPORT
                        }
                        else
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> IMPORT
                        }
                        #endregion
                        break;
                    case "ESTRUCTURA":
                        #region
                        if (Nodo.ChildNodes.Count == 2) // ESTRUCTURA + BLOQUE
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> ESTRUCTURA
                            PrimeraPasada(Nodo.ChildNodes[1]); // ChildNodes[1] --> BLOQUE
                        }
                        else // BLOQUE
                        {
                            PrimeraPasada(Nodo.ChildNodes[0]); // ChildNodes[0] --> BLOQUE
                        }
                        #endregion
                        break;
                    case "IMPORT":
                        Importaciones(Nodo);
                        break;
                    case "BLOQUE":
                        #region
                        switch (Nodo.ChildNodes[0].Term.Name)
                        {
                            case "VARGLOBAL":
                                VariablesGlobales(Nodo.ChildNodes[0]);
                                break;
                            case "FUNCIONES":
                                Console.WriteLine("(((((((((((((((((((((((((((((((("+Nodo.ChildNodes[0].Term.Name + "))))))))))))))))))))))))))))");
                                AlmacenarFuncion(Nodo.ChildNodes[0]);
                                break;
                            case "MAIN":
                                AlmacenarMain(Nodo.ChildNodes[0]);
                                break;
                        }
                        #endregion
                        break;
                }
            }
        }

        private void Importaciones(ParseTreeNode Nodo)
        {
            Console.WriteLine(Nodo.Term.Name);
            int Importaciones = 2;
        }

        private void AlmacenarMain(ParseTreeNode Nodo)
        {
            /*
            MAIN.Rule = ToTerm("Main") + dospuntos + TIPODATOF + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //VOID NO LLEVA RETURN
                        | ToTerm("Main") + dospuntos + TIPODATOF + parentA + parentC + llaveA + SENTENCIAS + llaveC //VOID NO LLEVA RETURN
            */
            if (!tablafunciones.existeFuncion(Reservada.Main))
            {
                String tipodato = getTipoDatoF(Nodo.ChildNodes[2]);
                
                if (Nodo.ChildNodes.Count == 9)
                {
                    // LSTIDG + dospuntos + TIPODATO + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //Void no lleva return
                    if (!tipodato.Equals(Reservada.Void))
                    {
                        lstParametros = new List<Parametro>(); //Creando lista para parametros
                        AgregarParametros(Nodo.ChildNodes[4]); //Llenando lista de parametros
                        tablafunciones.addFuncion(Reservada.Main, Reservada.Main, Reservada.Main, getInicialDato(tipodato), tipodato, lstParametros, Nodo.ChildNodes[7], getLinea(Nodo.ChildNodes[1]), "0");
                    }
                    else
                    {
                        lstParametros = new List<Parametro>(); //Creando lista para parametros
                        AgregarParametros(Nodo.ChildNodes[4]); //Llenando lista de parametros
                        tablafunciones.addFuncion(Reservada.Main, Reservada.Main, Reservada.Main, "nulo", tipodato, lstParametros, Nodo.ChildNodes[7], getLinea(Nodo.ChildNodes[1]), "0");
                    }
                }
                else if (Nodo.ChildNodes.Count == 8)
                {
                    // LSTIDG + dospuntos + TIPODATO + parentA + parentC + llaveA + SENTENCIAS + llaveC //Void no lleva return
                    if (!tipodato.Equals(Reservada.nulo))
                    {
                        tablafunciones.addFuncion(Reservada.Main, Reservada.Main, Reservada.Main, getInicialDato(tipodato), tipodato, null, Nodo.ChildNodes[6], getLinea(Nodo.ChildNodes[1]), "0");
                    }
                    else
                    {
                        tablafunciones.addFuncion(Reservada.Main, Reservada.Main, Reservada.Main, "nulo", tipodato, null, Nodo.ChildNodes[6], getLinea(Nodo.ChildNodes[1]), "0");
                    }
                }
            }
            else
            {
                Console.WriteLine("Error Main");
                Console.WriteLine("Error Semantico-->Objeto Main ya existe, imposible crear linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                lstError.Add(new Error(Reservada.ErrorSemantico, "Objeto Main ya existe, imposible crear", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
            }
        }

        private void AlmacenarFuncion(ParseTreeNode Nodo)
        {
            /*
                            // LSTIDG <-- SOLO DEBERIA DE TENER UN ID, Y NO UNA LISTA, ESTO ES PARA QUE NO DE ERROR DE AMBIGUEDAD
            FUNCIONES.Rule = LSTIDG + dospuntos + TIPODATOF + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //Void no lleva return
                            | LSTIDG + dospuntos + TIPODATOF + parentA + parentC + llaveA + SENTENCIAS + llaveC //Void no lleva return
             */
            Console.WriteLine(Nodo.Term.Name);
            String tipodato = getTipoDatoF(Nodo.ChildNodes[2]);
            Retorno id = devolverUnicoID(Nodo.ChildNodes[0], tipodato);
            
            if(id != null)
            {
                String claveMetodo = GetKey(id.Valor,Nodo);
                //Console.WriteLine("clave metodo -> " + claveMetodo);

                if (!tablafunciones.existeFuncionByKey(claveMetodo))
                {
                    if (Nodo.ChildNodes.Count == 9)
                    {
                        // LSTIDG + dospuntos + TIPODATO + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //Void no lleva return
                        if (!tipodato.Equals(Reservada.Void))
                        {
                            lstParametros = new List<Parametro>(); //Creando lista para parametros
                            AgregarParametros(Nodo.ChildNodes[4]); //Llenando lista de parametros
                            tablafunciones.addFuncion(claveMetodo,Reservada.Funcion, id.Valor, getInicialDato(tipodato), tipodato, lstParametros, Nodo.ChildNodes[7], getLinea(Nodo.ChildNodes[1]), "0");
                        }
                        else
                        {
                            lstParametros = new List<Parametro>(); //Creando lista para parametros
                            AgregarParametros(Nodo.ChildNodes[4]); //Llenando lista de parametros
                            tablafunciones.addFuncion(claveMetodo, Reservada.Funcion, id.Valor, "nulo", tipodato, lstParametros, Nodo.ChildNodes[7], getLinea(Nodo.ChildNodes[1]), "0");
                        }
                    }
                    else if (Nodo.ChildNodes.Count == 8)
                    {
                        // LSTIDG + dospuntos + TIPODATO + parentA + parentC + llaveA + SENTENCIAS + llaveC //Void no lleva return
                        if (!tipodato.Equals(Reservada.nulo))
                        {
                            tablafunciones.addFuncion(claveMetodo,Reservada.Funcion, id.Valor, getInicialDato(tipodato), tipodato, null, Nodo.ChildNodes[6], getLinea(Nodo.ChildNodes[1]), "0");
                        }
                        else
                        {
                            tablafunciones.addFuncion(claveMetodo, Reservada.Funcion, id.Valor, "nulo", tipodato, null, Nodo.ChildNodes[6], getLinea(Nodo.ChildNodes[1]), "0");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Imposible sobrecargar funcion");
                    Console.WriteLine("Error Semantico-->Imposible sobrecargar funcion linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                    lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible sobrecargar funcion", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                }
            }
            else
            {
                Console.WriteLine("Accion no valida");
                Console.WriteLine("Error Semantico-->Acciones no validas para identificadores linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para identificadores", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
            }
        }
        
        private void VariablesGlobales(ParseTreeNode Nodo)
        {
            /*
            VARGLOBAL.Rule = LSTIDG + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id
                            | LSTIDG + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion y asignacion de arreglo, Condicion es entero, LSTIDG es solo un id, LSTARREGLO es del mismo tipo que Condicion
                            | LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id, id es un arreglo
                            | id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                            | id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                            
             */
            switch(Nodo.ChildNodes.Count)
            {
                case 4: //LSTIDG + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                        //id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                    #region
                    switch (Nodo.ChildNodes[1].Token.Value.ToString())
                    {
                        case ":": //LSTIDG + dospuntos + TIPODATO + puntocoma //Declaracion de variables

                            String td1 = getTipoDato(Nodo.ChildNodes[2]);
                            DeclaracionVariableG(td1,Nodo.ChildNodes[0]);

                            break;
                        case "=": //id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                            
                            String id = Nodo.ChildNodes[0].Token.Value.ToString();
                            
                            Simbolo var = cimaG.RetornarSimbolo(id); //Capturo el simbolo

                            if (var != null) //Si la variable existe
                            {
                                Retorno ret = Condicion(Nodo.ChildNodes[2]); //Es el valor que se quiere asignar a la variable

                                if(ret != null)
                                {
                                    if (ret.Tipo.Equals(var.Tipo)) //Si son del mismo tipo se pueden asignar (variable con variable)
                                    {
                                        Console.WriteLine("Se asigno variable: " + id + " --> " + ret.Valor + " (" + ret.Tipo + ")");
                                        var.Valor = ret.Valor; //Asigno a la variable
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Asignacion no valida, expresion incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable " + id + " no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + id + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            break;
                    }
                    #endregion
                    break;
                case 6: //LSTIDG + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables
                        //id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                    #region
                    switch (Nodo.ChildNodes[1].Token.Value.ToString())
                    {
                        case ":": //LSTIDG + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables

                            String td1 = getTipoDato(Nodo.ChildNodes[2]);
                            Retorno ret0 = Condicion(Nodo.ChildNodes[4]);

                            DeclaracionAsignacionVariableG(td1, ret0, Nodo.ChildNodes[0]);

                            break;
                        case "=": //id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                            #region
                            String id6 = Nodo.ChildNodes[0].Token.Value.ToString();

                            Simbolo ArregloA = cimaG.RetornarSimbolo(id6);

                            if (ArregloA != null)
                            {
                                if(ArregloA.TipoObjeto.Equals(Reservada.arreglo)) //Verificando si es una arreglo
                                {
                                    int tam = Int32.Parse(ArregloA.Valor);
                                    arreglo = new List<Celda>(); //Este arreglo TEMPORAL es solo para almacenar los datos del arreglo asignado
                                    AsignarValidarArreglo(ArregloA.Tipo, Nodo.ChildNodes[3], getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));

                                    if(arreglo.Count <= tam) //Si el arreglo nuevo es menor o igual al arreglo que se desea asignar
                                    {
                                        List<Celda> arregloAct = ArregloA.Arreglo;
                                        int i = 0;
                                        while(i < arreglo.Count) //Si el arreglo donde almacenamos
                                        {
                                            arregloAct.ElementAt(i).valor = arreglo.ElementAt(i).valor; //Asignamos al arreglo actual los datos del arreglo TEMPORAL.
                                            i++;
                                        }
                                        Console.WriteLine("Se asigno arreglo: " + ArregloA.Nombre + "[" + tam + "]" + "(" + ArregloA.Tipo + ")");
                                        ArregloA.Arreglo = arregloAct; //Asignando el arreglo ya con los valores del arreglo que se asigno

                                        //foreach (Celda cel in arregloAct)
                                        //{
                                        //    Console.WriteLine("\t\t" + ArregloA.Nombre + ":" + ArregloA.Tipo + "{" + cel.tipo + "-" + cel.valor + "}");
                                        //}
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Cantidad de valores asignados al arreglo incorrectas linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de valores asignados al arreglo incorrectas", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->La variable no es un arreglo linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no es un arreglo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->La variable no existe linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no existe, tipo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case 7: //LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id
                        //Si el arreglo solo es declarado se le asignará valores por defecto como las variables normales.
                    #region
                    String tipodato7 = getTipoDato(Nodo.ChildNodes[2]);
                    Retorno id7 = devolverUnicoID(Nodo.ChildNodes[0], tipodato7);

                    if (id7 != null)
                    {
                        if (!cimaG.existeSimbolo(id7.Valor))
                        {
                            Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                            if(dimens != null)
                            {
                                if (dimens.Tipo.Equals(Reservada.Entero))
                                {
                                    int tam = Int32.Parse(dimens.Valor);

                                    if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                    {
                                        arreglo = new List<Celda>();

                                        int i = 0;
                                        while (i < tam) //Este llena el arreglo con los datos iniciales segun sea el tipo de dato
                                        {
                                            arreglo.Add(new Celda(tipodato7, getInicialDato(tipodato7)));
                                            i++;
                                        }
                                        Console.WriteLine("Se creo arreglo: " + id7.Valor + "[" + tam + "]" + "(" + tipodato7 + ")");
                                        cimaG.addSimbolo(Reservada.arregloGlobal, id7.Valor, tam + "", tipodato7, Reservada.arreglo, id7.Linea, id7.Columna, true, arreglo);
                                        //foreach (Celda cel in arreglo)
                                        //{
                                        //    Console.WriteLine("\t\t" + id7.Valor + ":" + tipodato7 + "{" + cel.tipo + "-" + cel.valor + "}");
                                        //}
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id7.Linea + " columna:" + id7.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id7.Linea, id7.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Accion no valida");
                        Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                    }
                    #endregion
                    break;
                case 9: //LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDG es solo un id, id es un arreglo
                    #region
                    String tipodato9 = getTipoDato(Nodo.ChildNodes[2]);
                    Retorno id9 = devolverUnicoID(Nodo.ChildNodes[0], tipodato9);

                    if (id9 != null)
                    {
                        if (!cimaG.existeSimbolo(id9.Valor))
                        {
                            Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                            if(dimens != null)
                            {
                                if (dimens.Tipo.Equals(Reservada.Entero))
                                {
                                    int tam = Int32.Parse(dimens.Valor);

                                    if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                    {
                                        arreglo = new List<Celda>(); //Arreglo Nuevo

                                        String id = Nodo.ChildNodes[7].Token.Value.ToString();

                                        Simbolo ObjArreglo = cimaG.RetornarSimbolo(id);

                                        if (ObjArreglo != null)
                                        {
                                            if (ObjArreglo.TipoObjeto.Equals(Reservada.arreglo)) //Si el objeto id buscado es un Arreglo
                                            {
                                                if (ObjArreglo.Tipo.Equals(tipodato9))
                                                {
                                                    int tamArregloAsig = Int32.Parse(ObjArreglo.Valor);
                                                    List<Celda> arregloAsignado = ObjArreglo.Arreglo;

                                                    if (tam == tamArregloAsig)
                                                    {
                                                        int i = 0;
                                                        while (i < tam) //Asignando los valores del arreglo asignado al nuevo arreglo
                                                        {
                                                            arreglo.Add(arregloAsignado.ElementAt(i));
                                                            i++;
                                                        }
                                                        Console.WriteLine("Se creo arreglo: " + id9.Valor + "[" + tam + "]" + "(" + tipodato9 + ")");
                                                        cimaG.addSimbolo(Reservada.arregloGlobal, id9.Valor, tam + "", tipodato9, Reservada.arreglo, id9.Linea, id9.Columna, true, arreglo);
                                                        //foreach (Celda cel in arreglo)
                                                        //{
                                                        //    Console.WriteLine("\t\t" + id9.Valor + ":" + tipodato9 + "{" + cel.tipo + "-" + cel.valor + "}");
                                                        //}

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Error Semantico-->El arreglo asignado no posee las dimensiones correctas linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                        lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo asignado no posee las dimensiones correctas", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Error Semantico-->El arreglo asignado no posee un tipo de dato valido linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                    lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo asignado no posee un tipo de dato valido", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->El objeto asignado no es Arreglo sino ID linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "El objeto asignado no es Arreglo sino ID", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Arreglo asignado no existente linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Arreglo asignado no existente", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            }                        
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id9.Linea + " columna:" + id9.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id9.Linea, id9.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Accion no valida");
                        Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                    }
                    #endregion
                    break;
                case 11: //LSTIDG + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion y asignacion de arreglo, Condicion es entero, LSTIDG es solo un id, LSTARREGLO es del mismo tipo que Condicion
                    #region
                    String tipodato11 = getTipoDato(Nodo.ChildNodes[2]);
                    Retorno id11 = devolverUnicoID(Nodo.ChildNodes[0], tipodato11);

                    if(id11 != null)
                    {
                        if(!cimaG.existeSimbolo(id11.Valor))
                        {
                            Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                            if(dimens != null)
                            {
                                if (dimens.Tipo.Equals(Reservada.Entero))
                                {
                                    int tam = Int32.Parse(dimens.Valor);

                                    if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                    {
                                        arreglo = new List<Celda>();
                                        AsignarValidarArreglo(tipodato11, Nodo.ChildNodes[8], id11.Linea, id11.Columna);

                                        if (arreglo.Count == tam) //Verificando que el tamanio del arreglo sea de la dimension correcta
                                        {
                                            Console.WriteLine("Se creo arreglo: " + id11.Valor + "[" + tam + "]" + "(" + tipodato11 + ")");
                                            cimaG.addSimbolo(Reservada.arregloGlobal, id11.Valor, tam + "", tipodato11, Reservada.arreglo, id11.Linea, id11.Columna, true, arreglo);
                                            //foreach (Celda cel in arreglo)
                                            //{
                                            //    Console.WriteLine("\t\t" + id11.Valor + ":" + tipodato11 + "{" + cel.tipo + "-" + cel.valor + "}");
                                            //}
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Cantidad de valores asignados al arreglo incorrectas linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de valores asignados al arreglo incorrectas", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id11.Linea + " columna:" + id11.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id11.Linea, id11.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Accion no valida");
                        Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                    }
                    #endregion
                    break;
            }
        }

        private void DeclaracionVariableG(String tipodato,ParseTreeNode Nodo)
        {                                                                       //METODO SOLO PARA LAS VARIABLES GLOBALES
            //  LSTIDG + coma + id
            //| id
            #region
            switch (Nodo.Term.Name)
            {
                // Cuando viene coma no hace nada esto :3
                case "LSTIDG":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                            DeclaracionVariableG(tipodato, hijo);
                    }
                    break;
                case "id":
                    String id = Nodo.Token.Value.ToString();

                    if(!cimaG.existeSimbolo(id))
                    {
                        cimaG.addSimbolo(Reservada.varGlobal, id, getInicialDato(tipodato), tipodato, Reservada.var, getLinea(Nodo), getColumna(Nodo), false, null);
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable ya existente linea:" + getLinea(Nodo) + " columna:" + getColumna(Nodo));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable ya existente", getLinea(Nodo), getColumna(Nodo)));
                    }
                    break;
            }
            #endregion
        }

        private void DeclaracionAsignacionVariableG(String tipodato, Retorno ret, ParseTreeNode Nodo)
        {                                                                       //METODO SOLO PARA LAS VARIABLES GLOBALES
            // LSTIDG + coma + id
            //| id

            #region
            switch (Nodo.ChildNodes.Count)
            {
                case 3:
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        if (!Nodo.ChildNodes[1].Token.Value.ToString().Equals(","))
                        {
                            DeclaracionAsignacionVariableG(tipodato, ret, hijo);
                        }
                    }
                    break;
                case 1:
                    String id = Nodo.ChildNodes[0].Token.Value.ToString();

                    if (!cimaG.existeSimbolo(id))
                    {
                        if(ret != null)
                        {
                            if (ret.Tipo.Equals(tipodato)) //Si son del mismo tipo se pueden asignar (variable con variable)
                            {
                                Console.WriteLine("Se creo variable: " + id + " --> " + ret.Valor + " (" + ret.Tipo + ")");
                                cimaG.addSimbolo(Reservada.varGlobal, id, ret.Valor, tipodato, Reservada.var, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]), true, null);

                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Asignacion no valida, expresion incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable ya existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable ya existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                    }
                    break;
            }
            #endregion
        }

        private string GetKey(String id, ParseTreeNode Nodo)
        {
            /*
            // LSTIDG <-- SOLO DEBERIA DE TENER UN ID, Y NO UNA LISTA, ESTO ES PARA QUE NO DE ERROR DE AMBIGUEDAD
            FUNCIONES.Rule = LSTIDG + dospuntos + TIPODATOF + parentA + PARAMETROS + parentC + llaveA + SENTENCIAS + llaveC //Con parametros //Void no lleva return
                            | LSTIDG + dospuntos + TIPODATOF + parentA + parentC + llaveA + SENTENCIAS + llaveC //Void no lleva return
             */
            if (Nodo.ChildNodes.Count == 9) // Con parametros
            {
                String claveParam = ClaveParametros(Nodo.ChildNodes[4]);
                return id + getTipoDatoF(Nodo.ChildNodes[2]) + claveParam;
            }
            else // Sin parametros
            {
                return id + getTipoDatoF(Nodo.ChildNodes[2]);
            }
        }
        
        private string ClaveParametros(ParseTreeNode Nodo)
        {
            if (Nodo.Term.Name == "PARAMETRO")
            {
                return getTipoDato(Nodo.ChildNodes[2]);
            }
            else
            {
                String res = "";
                foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                {
                    res += ClaveParametros(hijo);
                }
                return res;
            }
        }

        public void AgregarParametros(ParseTreeNode Nodo)
        {
            //PARAMETROS + coma + PARAMETRO
            //| PARAMETRO
            switch (Nodo.Term.Name)
            {
                case "PARAMETROS":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        AgregarParametros(hijo);
                    }
                    break;
                case "PARAMETRO": // id + dospuntos + TIPODATO
                    String tipodato = getTipoDato(Nodo.ChildNodes[2]);
                    String id = Nodo.ChildNodes[0].Token.Value.ToString();
                    Console.WriteLine("param: " + tipodato + " - " + id);
                    lstParametros.Add(new Parametro(Reservada.Funcion, id, getInicialDato(tipodato), tipodato, getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1]), true));
                    break;
            }
        }

        //====================================================================================================== EJECUCION ===================================================================================

        public RetornoAc EjecutarX(List<Celda> arregloX)
        {
            Funciones funcion = tablafunciones.RetornarFuncion(Reservada.Main);
            
            if (funcion != null)
            {
                if (!funcion.getTipo().Equals(Reservada.Void)) //Si el MAIN es de tipo VOID no retorna nada
                {
                    #region
                    if (arregloX.Count == funcion.getParametros().Count)
                    {
                        TablaSimbolos mainn = new TablaSimbolos(1, Reservada.Main, true, false, false); //Esto depende de si es VOID
                        pilaSimbolos.Push(mainn);
                        cima = mainn; //Estableciendo la tabla de simbolos cima
                        nivelActual = 1; //Estableciendo el nivel actual

                        int cont = 0;
                        foreach (Parametro parametro in funcion.getParametros())
                        {
                            if (parametro.Tipo.Equals(arregloX.ElementAt(cont).tipo))
                            {
                                if (!ExisteSimbolo(parametro.Nombre)) //Verifico que no exista
                                {
                                    //arreglo.ElementAt(cont).valor <<-- es el valor de parametro que contiene el metodo cuando fue invocado
                                    cima.addSimbolo(parametro.Ambito, parametro.Nombre, arregloX.ElementAt(cont).valor, parametro.Tipo, Reservada.var, parametro.Linea, parametro.Columna, true, null);
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Parametro ya existente linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro ya existente", parametro.Linea, parametro.Columna));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Parametro introducido de tipo incompatible linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro introducido de tipo incompatible", parametro.Linea, parametro.Columna));
                            }
                            cont++;
                        }

                        RetornoAc retorno = Sentencias(funcion.getCuerpo());

                        if (retorno != null)
                        {
                            if (retorno.Tipo.Equals(funcion.getTipo()))
                            {
                            
                                    nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                    if (retorno.RetornaVal && cima.RetornaVal)
                                    {
                                        Console.WriteLine("EL MAIN RETORNO UN VALOR =" + retorno.Valor);
                                        return retorno;
                                    }
                                    else if (retorno.Retorna && cima.Retorna)
                                    {
                                        Console.WriteLine("EL MAIN RETORNO UN VALOR VACIO");
                                        return retorno;
                                    }
                                    else if (retorno.Detener && cima.Detener)
                                    {
                                        Console.WriteLine("EL MAIN RETORNO UN BREAK");
                                        return retorno;
                                    }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico--> linea:Tipo de dato retornado no valido" + funcion.getLinea() + " columna:" + funcion.getColumna());
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Tipo de dato retornado no valido", funcion.getLinea(), funcion.getColumna()));
                            }
                        }

                        return retorno;
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Cantidad de parametros introducidos no valida linea:" + funcion.getLinea() + " columna:" + funcion.getColumna());
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de parametros introducidos no valida", funcion.getLinea(), funcion.getColumna()));
                    }
                    #endregion
                }
                else //En el caso de que el MAIN sea de tipo VOID
                {
                    #region
                    TablaSimbolos mainn = new TablaSimbolos(1, Reservada.Main, false, false, true); //Esto depende de si es VOID
                    pilaSimbolos.Push(mainn);
                    cima = mainn; //Estableciendo la tabla de simbolos cima
                    nivelActual = 1; //Estableciendo el nivel actual

                    int cont = 0;
                    foreach (Parametro parametro in funcion.getParametros())
                    {
                        if (parametro.Tipo.Equals(arregloX.ElementAt(cont).tipo))
                        {
                            if (!ExisteSimbolo(parametro.Nombre)) //Verifico que no exista
                            {
                                //arreglo.ElementAt(cont).valor <<-- es el valor de parametro que contiene el metodo cuando fue invocado
                                cima.addSimbolo(parametro.Ambito, parametro.Nombre, arregloX.ElementAt(cont).valor, parametro.Tipo, Reservada.var, parametro.Linea, parametro.Columna, true, null);
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Parametro ya existente linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro ya existente", parametro.Linea, parametro.Columna));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Parametro introducido de tipo incompatible linea:" + parametro.Linea + " columna:" + parametro.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro introducido de tipo incompatible", parametro.Linea, parametro.Columna));
                        }
                        cont++;
                    }

                    RetornoAc retorno = Sentencias(funcion.getCuerpo());

                    nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                    return retorno;
                    #endregion
                }
            }
            else
            {
                Console.WriteLine("Error Semantico-->Funcion no existente linea:" + funcion.getLinea() + " columna:" + funcion.getColumna());
                lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no existente", funcion.getLinea(), funcion.getColumna()));
            }

            return new RetornoAc("-", "-", "0", "0");
        }
        
        private RetornoAc Sentencias(ParseTreeNode Nodo)
        {
            //if (!isRetornoG)
            //{
            switch (Nodo.Term.Name)
            {
                case "SENTENCIAS":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        RetornoAc retorno = Sentencias(hijo); // SENTENCIA | SENTENCIAS

                        //if (retorno != null)
                        //{
                            if (retorno.RetornaVal && cima.RetornaVal)
                            {
                                return retorno;
                            }
                            else if (retorno.Retorna && cima.Retorna)
                            {
                                return retorno;
                            }
                            else if (retorno.Detener && cima.Detener)
                            {
                                return retorno;
                            }
                        //}
                    }
                    break;
                case "SENTENCIA":
                    /*
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
                        */
                    #region
                    switch (Nodo.ChildNodes.Count)
                    {
                        case 1: // VARLOCAL
                            Console.WriteLine("VARLOCAL");
                            #region
                            VariablesLocales(Nodo.ChildNodes[0]);
                            
                            RetornoAc retornoVar = new RetornoAc("-", "-", "0", "0");
                                
                            return new RetornoAc("-", "-", "0", "0");
                            #endregion
                        case 2: //| ToTerm("Return") + puntocoma //Para metodos unicamente
                                //| ToTerm("Break") + puntocoma //Unicamente funcional para Switch, While y For //Colocarle una bandera para su uso
                            //Console.WriteLine("BREAK O RETURN");
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "Return":
                                    //isRetornoG = true;
                                    RetornoAc retornoR = new RetornoAc("-", "-", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                    retornoR.Retorna = true;
                                    return retornoR;
                                    
                                case "Break":
                                    RetornoAc retornoB = new RetornoAc("-", "-", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                    retornoB.Detener = true;
                                    return retornoB;
                                        
                            }
                            #endregion
                            break;
                        case 3: //| ToTerm("Return") + CONDICION + puntocoma //Para funciones unicamente
                            Console.WriteLine("RETURNN");
                            #region
                            //isRetornoG = true;
                            //RetornoG = Condicion(Nodo.ChildNodes[1]);
                            Retorno retu = Condicion(Nodo.ChildNodes[1]);
                            
                            if(retu != null)
                            {
                                RetornoAc retornoV = new RetornoAc(retu.Tipo, retu.Valor, retu.Linea, retu.Columna);
                                retornoV.RetornaVal = true;
                                return retornoV;
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Retono de expresion incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Retono de expresion incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                        case 4: //| id + parentA + parentC + puntocoma //Invocacion de metodo sin parametros
                            Console.WriteLine("INVOCACION METODO SIN PARAMETROS");
                            #region
                            String id3 = Nodo.ChildNodes[0].Token.Value.ToString();
                            //Funciones func3 = tablafunciones.RetornarFuncion(id3);
                            Funciones func3 = tablafunciones.RetornarFuncionEvaluandoSobrecargaVoid(id3);

                            if (func3 != null)
                            {
                                nivelActual++; //Aumentamos el nivel actual ya que accedemos a otro metodo
                                TablaSimbolos metodo4 = new TablaSimbolos(nivelActual, Reservada.Metodo, true, false, true);
                                pilaSimbolos.Push(metodo4);
                                cima = metodo4; //Estableciendo la tabla de simbolos cima
                                    
                                Sentencias(func3.getCuerpo());
                                //isRetornoG = false; // Volviendo a la normalidad la bandera

                                nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                    
                                return new RetornoAc("-", "-", "0", "0");
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Funcion no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                        case 5: //| id + parentA + ASIGNAR_PARAMETRO + parentC + puntocoma //Invocacion de metodo con parametros
                                //| ToTerm("Print") + parentA + CONDICION + parentC + puntocoma //PRINT
                            Console.WriteLine("INVOCACION METODO CON PARAMETROS O PRINT");
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "id":
                                    #region
                                    //------------------------------------
                                    //PRIMERO OBTENGO LA CANTIDAD DE VALORES EN MIS PARAMETROS ACEPTADOS POR EL ARREGLO
                                    arreglo = new List<Celda>(); //Este arreglo es para almacenar los parametros del metodo que se invoco
                                    ValidarParametrosMetodo(Nodo.ChildNodes[2]); // Mandamos los parametros
                                    //AHORA BUSCO LA FUNCION EN BASE AL NOMBRE Y A MI ARREGLO DE PARAMETROS
                                    String id5 = Nodo.ChildNodes[0].Token.Value.ToString();
                                    //Funciones func5 = tablafunciones.RetornarFuncion(id5);
                                    Funciones func5 = tablafunciones.RetornarFuncionEvaluandoSobrecarga(id5, arreglo);
                                    //--------------------------------------

                                    if (func5 != null)
                                    {
                                        //arreglo = new List<Celda>(); //Este arreglo es para almacenar los parametros del metodo que se invoco
                                        //ValidarParametrosMetodo(Nodo.ChildNodes[2]); // Mandamos los parametros

                                        if (arreglo.Count == func5.getParametros().Count) 
                                        {
                                            nivelActual++; //Aumentamos el nivel actual ya que accedemos a otro metodo
                                            TablaSimbolos metodo5 = new TablaSimbolos(nivelActual, Reservada.Metodo, true, false, true);
                                            pilaSimbolos.Push(metodo5);
                                            cima = metodo5; //Estableciendo la tabla de simbolos cima

                                            int cont = 0;
                                            foreach (Parametro parametro in func5.getParametros())
                                            {
                                                if (parametro.Tipo.Equals(arreglo.ElementAt(cont).tipo))
                                                {
                                                    if (!ExisteSimbolo(parametro.Nombre)) //Verifico que no exista
                                                    {
                                                        //arreglo.ElementAt(cont).valor <<-- es el valor de parametro que contiene el metodo cuando fue invocado
                                                        cima.addSimbolo(parametro.Ambito, parametro.Nombre, arreglo.ElementAt(cont).valor, parametro.Tipo, Reservada.var, parametro.Linea, parametro.Columna, true, null);
                                                        
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Error Semantico-->Parametro ya existente linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro ya existente", parametro.Linea, parametro.Columna));
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Error Semantico-->Parametro introducido de tipo incompatible linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro introducido de tipo incompatible", parametro.Linea, parametro.Columna));
                                                }
                                                cont++;
                                            }
                                                    
                                            Sentencias(func5.getCuerpo());
                                            //isRetornoG = false; // Volviendo a la normalidad la bandera

                                            nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                            return new RetornoAc("-", "-", "0", "0");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Cantidad de parametros introducidos no valida linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de parametros introducidos no valida", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Funcion no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }
                                    #endregion
                                    break;
                                case "Print":
                                    Retorno ret = Condicion(Nodo.ChildNodes[2]);

                                    if (ret != null)
                                    {
                                        lstPrint.Add(ret.Valor);
                                        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++PRINT++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                    break;
                            }
                            #endregion
                            break;
                        case 7: //| ToTerm("If") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //IF
                                //| ToTerm("While") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //WHILE
                            Console.WriteLine("IF O WHILE");
                            #region
                            Retorno cond7 = Condicion(Nodo.ChildNodes[2]);

                            if (Nodo.ChildNodes[0].Term.Name.Equals("If")) // ToTerm("If") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //IF
                            {
                                if (cond7 != null)
                                {
                                    if (cond7.Tipo.Equals(Reservada.Booleano)) // Si la condicion es booleana
                                    {
                                        if (cond7.Valor.Equals("True"))
                                        {
                                            TablaSimbolos iff = new TablaSimbolos(nivelActual, Reservada.Iff, cima.RetornaVal, cima.Detener, cima.Retorna);
                                            pilaSimbolos.Push(iff);
                                            cima = iff; //Estableciendo la tabla de simbolos cima
                                            //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de If

                                            RetornoAc ret1 =  Sentencias(Nodo.ChildNodes[5]);

                                            if (ret1.RetornaVal && cima.RetornaVal)
                                            {
                                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF RETORNO VALOR ==============================================");
                                                return ret1;
                                            }
                                            else if (ret1.Retorna && cima.Retorna)
                                            {
                                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF RETORNO VACIO ==============================================");
                                                return ret1;
                                            }
                                            else if (ret1.Detener && cima.Detener)
                                            {
                                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF RETORNO DETENER ==============================================");
                                                return ret1;
                                            }

                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            return new RetornoAc("-", "-", "0", "0");
                                        }
                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Valor de condicion invalida");
                                        Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Valor de condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Condicion invalida");
                                    Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                }
                            }
                            else if (Nodo.ChildNodes[0].Term.Name.Equals("While")) // ToTerm("While") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC //WHILE
                            {
                                if (cond7 != null)
                                {
                                    if (cond7.Tipo.Equals(Reservada.Booleano)) // Si la condicion es booleana
                                    {
                                        if (cond7.Valor.Equals("True"))
                                        {
                                            int contador = 1;
                                            while (true)
                                            {
                                                TablaSimbolos whilee = new TablaSimbolos(nivelActual, Reservada.Whilee, cima.RetornaVal, true, cima.Retorna);
                                                pilaSimbolos.Push(whilee);
                                                cima = whilee; //Estableciendo la tabla de simbolos cima
                                                //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de While

                                                RetornoAc ret1 = Sentencias(Nodo.ChildNodes[5]);
                                                cond7 = Condicion(Nodo.ChildNodes[2]); // Esta condicion se vuelve a evaluar, como las variables se actualizan entonces el resultado de la condicion cambia
                                                    
                                                if (ret1.RetornaVal) 
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= WHILE RETORNO VALOR ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Retorna)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= WHILE RETORNO VACIO ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Detener)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= WHILE RETORNO DETENER ==============================================");
                                                    //return ret1;
                                                    break;
                                                }

                                                if ((cond7.Valor.Equals("False")) || (contador == 50)) // Si la condicion es falsa // Maximo de iteraciones del while es 50
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    break;
                                                }

                                                contador++;
                                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            }
                                            return new RetornoAc("-", "-", "0", "0");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Valor de condicion invalida");
                                        Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Valor de condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Condicion invalida");
                                    Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                }
                            }
                            #endregion
                            break;
                        case 8: //| ToTerm("Do") + llaveA + SENTENCIAS + llaveC + ToTerm("While") + parentA + CONDICION + parentC //DO-WHILE
                                //| ToTerm("Switch") + parentA + CONDICION + parentC + llaveA + LSCASOS + DEFAUL + llaveC //Strings, Double e Int son los tipos de datos unicos que puede operar esta estructura
                            Console.WriteLine("DOWHILE O SWITCH");
                            #region
                            if (Nodo.ChildNodes[0].Term.Name.Equals(Reservada.Doo))
                            { // ToTerm("Do") + llaveA + SENTENCIAS + llaveC + ToTerm("While") + parentA + CONDICION + parentC //DO-WHILE
                                Retorno condW = Condicion(Nodo.ChildNodes[6]);

                                if (condW != null)
                                {
                                    if (condW.Tipo.Equals(Reservada.Booleano)) // Si la condicion es booleana
                                    {
                                        TablaSimbolos dowhilee = new TablaSimbolos(nivelActual, Reservada.DoWhile, cima.RetornaVal, true, cima.Retorna);
                                        pilaSimbolos.Push(dowhilee);
                                        cima = dowhilee; //Estableciendo la tabla de simbolos cima
                                        //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de Do-While

                                        RetornoAc ret1 = Sentencias(Nodo.ChildNodes[2]); // Las sentencias se ejecutan al menos una vez en el Do-While

                                        if (ret1.RetornaVal)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO VALOR ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Retorna)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO VACIO ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Detener)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO DETENER ==============================================");
                                            //return ret1;
                                            break;
                                        }

                                        pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                        if (condW.Valor.Equals("True"))
                                        {
                                            int contador = 1;

                                            while (true)
                                            {
                                                TablaSimbolos dowhileee = new TablaSimbolos(nivelActual, Reservada.DoWhile, cima.RetornaVal, true, cima.Retorna);
                                                pilaSimbolos.Push(dowhileee);
                                                cima = dowhileee; //Estableciendo la tabla de simbolos cima
                                                //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de Do-While

                                                ret1 = Sentencias(Nodo.ChildNodes[2]);

                                                condW = Condicion(Nodo.ChildNodes[6]); // Esta condicion se vuelve a evaluar, como las variables se actualizan entonces el resultado de la condicion cambia

                                                if (ret1.RetornaVal)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO VALOR ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Retorna)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO VACIO ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Detener)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= DO-WHILE RETORNO DETENER ==============================================");
                                                    return ret1;
                                                }

                                                if ((condW.Valor.Equals("False")) || (contador == 50)) // Si la condicion es falsa // Maximo de iteraciones del do-while es 50
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    break;
                                                }
                                                contador++;

                                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            }
                                        }
                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Valor de condicion invalida");
                                        Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[5]) + " columna:" + getColumna(Nodo.ChildNodes[5]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Valor de condicion invalida", getLinea(Nodo.ChildNodes[5]), getColumna(Nodo.ChildNodes[5])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Condicion invalida");
                                    Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[5]) + " columna:" + getColumna(Nodo.ChildNodes[5]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion invalida", getLinea(Nodo.ChildNodes[5]), getColumna(Nodo.ChildNodes[5])));
                                }
                            }
                            else if (Nodo.ChildNodes[0].Term.Name.Equals(Reservada.Switchh))
                            { // ToTerm("Switch") + parentA + CONDICION + parentC + llaveA + LSCASOS + DEFAUL + llaveC //Strings, Double e Int son los tipos de datos unicos que puede operar esta estructura

                                Retorno retS = Condicion(Nodo.ChildNodes[2]);

                                if (retS != null)
                                {
                                    if (retS.Tipo.Equals(Reservada.Cadena) || retS.Tipo.Equals(Reservada.Doublee) || retS.Tipo.Equals(Reservada.Entero)) //Tipos de datos aceptables
                                    {
                                        TablaSimbolos switchh = new TablaSimbolos(nivelActual, Reservada.Switchh, cima.RetornaVal, true, cima.Retorna);
                                        pilaSimbolos.Push(switchh);
                                        cima = switchh; //Estableciendo la tabla de simbolos cima
                                        //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de switch

                                        BanderaCaso = false; //Controla que los casos no se repitan

                                        RetornoAc ret1 = EjecucionSwitch(retS, Nodo.ChildNodes[5]);

                                        if (ret1.RetornaVal)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VALOR ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Retorna)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VACIO ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Detener)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO DETENER ==============================================");
                                            return ret1;
                                        }

                                        if (!BanderaCaso) //Si bandera es True ejecuto DEFAUL
                                        {
                                            if(Nodo.ChildNodes[6].ChildNodes.Count == 3) //Que sea distinto de Vacio(Empty)
                                            {
                                                ret1 = Sentencias(Nodo.ChildNodes[6].ChildNodes[2]);

                                                if (ret1.RetornaVal)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VALOR ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Retorna)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VACIO ==============================================");
                                                    return ret1;
                                                }
                                                else if (ret1.Detener)
                                                {
                                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO DETENER ==============================================");
                                                    return ret1;
                                                }
                                            }
                                        }

                                        

                                        pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Tipo de dato condicional incorrecta linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Tipo de dato condicional incorrecta", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Condicion invalida linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                }
                            }
                            #endregion
                            break;
                        case 11: //| ToTerm("If") + parentA + CONDICION + parentC + llaveA + SENTENCIAS + llaveC + ToTerm("Else") + llaveA + SENTENCIAS + llaveC // IF ELSE
                            Console.WriteLine("IF ELSE");
                            #region
                            Retorno cond11 = Condicion(Nodo.ChildNodes[2]);

                            if (cond11 != null)
                            {
                                if (cond11.Tipo.Equals(Reservada.Booleano)) // Si la condicion es booleana
                                {
                                    TablaSimbolos iff = new TablaSimbolos(nivelActual, Reservada.Iff, cima.RetornaVal, cima.Detener, cima.Retorna);
                                    pilaSimbolos.Push(iff);
                                    cima = iff; //Estableciendo la tabla de simbolos cima
                                    //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de If

                                    if (cond11.Valor.Equals("True"))
                                    {
                                        RetornoAc ret1 = Sentencias(Nodo.ChildNodes[5]); //Ejecutando sentencias del If

                                        if (ret1.RetornaVal && cima.RetornaVal)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO VALOR ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Retorna && cima.Retorna)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO VACIO ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Detener && cima.Detener)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO DETENER ==============================================");
                                            return ret1;
                                        }

                                        pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                    else
                                    {
                                        RetornoAc ret1 = Sentencias(Nodo.ChildNodes[9]); //Ejecutando sentencias del Else

                                        if (ret1.RetornaVal && cima.RetornaVal)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO VALOR ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Retorna && cima.Retorna)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO VACIO ==============================================");
                                            return ret1;
                                        }
                                        else if (ret1.Detener && cima.Detener)
                                        {
                                            pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                            cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= IF-ELSE RETORNO DETENER ==============================================");
                                            return ret1;
                                        }

                                        pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                        return new RetornoAc("-", "-", "0", "0");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Valor de condicion invalida");
                                    Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Valor de condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Condicion invalida");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion invalida", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            }
                            #endregion
                            break;
                        case 15: //| ToTerm("For") + parentA + id + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma + CONDICION + puntocoma + OPERAFOR + parentC + llaveA + SENTENCIAS + llaveC //FOR
                            Console.WriteLine("FOR");
                            #region
                            String id15 = Nodo.ChildNodes[2].Token.Value.ToString();
                            Simbolo var15 = RetornarSimbolo(id15);
                            String td15 = getTipoDato(Nodo.ChildNodes[4]);
                            Retorno ret15 = Condicion(Nodo.ChildNodes[6]);

                            if (var15 == null) //Si no existe la variable lo creamos
                            {
                                if (ret15 != null) //Si mi asignacion de variable es distinta de null
                                {
                                    if (ret15.Tipo.Equals(td15)) //Si son del mismo tipo se pueden asignar (variable con expresion)
                                    {
                                        if (td15.Equals(Reservada.Entero) || td15.Equals(Reservada.Doublee)) //La variable creada debe ser numerico
                                        {
                                            Console.WriteLine("Se creo variable: " + id15 + " --> " + ret15.Valor + " (" + ret15.Tipo + ")");
                                            cima.addSimbolo(Reservada.varLocal, id15, ret15.Valor, td15, Reservada.var, getLinea(Nodo.ChildNodes[2]), getColumna(Nodo.ChildNodes[2]), true, null); //Creo la variable

                                            RetornoAc ret1 = EjecucionFor(Nodo);

                                            return ret1;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato no numerico linea:" + getLinea(Nodo.ChildNodes[5]) + " columna:" + getColumna(Nodo.ChildNodes[5]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato no numerico", getLinea(Nodo.ChildNodes[5]), getColumna(Nodo.ChildNodes[5])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[5]) + " columna:" + getColumna(Nodo.ChildNodes[5]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[5]), getColumna(Nodo.ChildNodes[5])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Asignacion no valida, expresion incorrecta linea:" + ret15.Linea + " columna:" + ret15.Columna);
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta", ret15.Linea, ret15.Columna));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->La variable ya existe linea:" + getLinea(Nodo.ChildNodes[2]) + " columna:" + getColumna(Nodo.ChildNodes[2]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "La variable ya existe", getLinea(Nodo.ChildNodes[2]), getColumna(Nodo.ChildNodes[2])));
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
            }
            //}
            //else
            //{
            //    Console.WriteLine("Se dejo de ejecutar funcion por el RETURN");
            //}
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ===================================== RETORNANDO SALVATION =============================================");
            
            return new RetornoAc("-", "-", "0", "0");
        }

        private RetornoAc EjecucionFor(ParseTreeNode Nodo)
        { // ToTerm("For") + parentA + id + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma + CONDICION + puntocoma + OPERAFOR + parentC + llaveA + SENTENCIAS + llaveC //FOR
            #region
            String id15 = Nodo.ChildNodes[2].Token.Value.ToString();
            Simbolo var15 = RetornarSimbolo(id15);
            Retorno cond = Condicion(Nodo.ChildNodes[8]);

            if (var15 != null) //Si existe la variable
            {
                if (cond != null) //Si mi asignacion de variable es distinta de null
                {
                    if (cond.Tipo.Equals(Reservada.Booleano)) //Si la condicion es Boolean
                    {
                        //Condicion cuerpo incremento

                        if(cond.Valor.Equals("True")) //Si la condicion es True se ejecutan las sentencias
                        {
                            int contador = 1;

                            while (true)
                            {
                                TablaSimbolos forr = new TablaSimbolos(nivelActual, Reservada.Forr, cima.RetornaVal, true, cima.Retorna);
                                pilaSimbolos.Push(forr);
                                cima = forr; //Estableciendo la tabla de simbolos cima
                                //nivelActual = 1; //Estableciendo el nivel actual <<-- No en este caso de For

                                RetornoAc ret1 = Sentencias(Nodo.ChildNodes[13]); //Ejecuta sentencias

                                if (ret1.RetornaVal)
                                {
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= FOR RETORNO VALOR ==============================================");
                                    return ret1;
                                }
                                else if (ret1.Retorna)
                                {
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= FOR RETORNO VACIO ==============================================");
                                    return ret1;
                                }
                                else if (ret1.Detener)
                                {
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= FOR RETORNO DETENER ==============================================");
                                    //return ret1;
                                    break;
                                }

                                OperacionFor(Nodo.ChildNodes[10]); //Ejecuta operacion incrementa/decremento

                                cond = Condicion(Nodo.ChildNodes[8]);

                                if((cond.Valor.Equals("False")) || (contador == 50)) // Si la condicion es falsa // Maximo de iteraciones del For es 50
                                {
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                    break;
                                }

                                contador++;

                                pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                            }
                            return new RetornoAc("-", "-", "0", "0");
                        }
                        return new RetornoAc("-", "-", "0", "0");
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[5]) + " columna:" + getColumna(Nodo.ChildNodes[5]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[5]), getColumna(Nodo.ChildNodes[5])));
                    }
                }
                else
                {
                    Console.WriteLine("Error Semantico-->Condicion no valida linea:" + cond.Linea + " columna:" + cond.Columna);
                    lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion no valida", cond.Linea, cond.Columna));
                }
            }
            else
            {
                Console.WriteLine("Error Semantico-->La variable no existe linea:" + getLinea(Nodo.ChildNodes[2]) + " columna:" + getColumna(Nodo.ChildNodes[2]));
                lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no existe", getLinea(Nodo.ChildNodes[2]), getColumna(Nodo.ChildNodes[2])));
            }

            return new RetornoAc("-", "-", "0", "0");
            #endregion
        }

        private void OperacionFor(ParseTreeNode Nodo)
        {
            //OPERAFOR.Rule = id + incrementa
            //                | id + decrementa
            #region
            Simbolo var15 = RetornarSimbolo(Nodo.ChildNodes[0].Token.Value.ToString());

            if (var15 != null) //Si existe la variable
            {
                if (var15.Tipo.Equals(Reservada.Entero) || var15.Tipo.Equals(Reservada.Doublee))
                {
                    Double oper = Double.Parse(var15.Valor);

                    if (Nodo.ChildNodes[1].Term.Name.Equals("++"))
                    {
                        oper++;
                        var15.Valor = oper+"";
                        Console.WriteLine("Se incremento " + var15.Nombre + "=" + oper);
                    }
                    else if (Nodo.ChildNodes[1].Term.Name.Equals("--"))
                    {
                        oper--;
                        var15.Valor = oper+"";
                        Console.WriteLine("Se decremento " + var15.Nombre + "=" + oper);
                    }
                }
                else
                {
                    Console.WriteLine("Error Semantico-->Tipo dato variable no numerica para operar linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                    lstError.Add(new Error(Reservada.ErrorSemantico, "Tipo dato variable no numerica para operar", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                }
            }
            else
            {
                Console.WriteLine("Error Semantico-->La variable no existe linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no existe", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
            }
            #endregion
        }

        private RetornoAc EjecucionSwitch(Retorno condicion, ParseTreeNode Nodo)
        {
            //LSCASOS.Rule = LSCASOS + CASO
            //                | CASO
            //                ;
            //CASO.Rule = ToTerm("Case") + TERMCASO + dospuntos + SENTENCIAS
            //            ;
            #region
            switch (Nodo.Term.Name)
            {
                case "LSCASOS":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        RetornoAc retorno = EjecucionSwitch(condicion, hijo); // LSCASOS | CASO

                        if (retorno.RetornaVal && cima.RetornaVal)
                        {
                            return retorno;
                        }
                        else if (retorno.Retorna && cima.Retorna)
                        {
                            return retorno;
                        }
                        else if (retorno.Detener && cima.Detener)
                        {
                            return retorno;
                        }
                    }
                    break;
                case "CASO": //ToTerm("Case") + TERMCASO + dospuntos + SENTENCIAS
                    
                    Retorno condAct = CondicionCaso(Nodo.ChildNodes[1]);
                    //Console.WriteLine("********************** Casos *****************");
                    if(condAct != null)
                    {
                        if(condicion.Tipo.Equals(condAct.Tipo))
                        {
                            if(condicion.Valor.Equals(condAct.Valor)) //Si el valor evaluado del switch es igual al caso
                            {
                                if(!BanderaCaso) //Si la bandera es verdadera quiere decir que esta condicion ya fue ejecutada y evaluada, se repite
                                {
                                    BanderaCaso = true;
                                    RetornoAc ret1 = Sentencias(Nodo.ChildNodes[3]);

                                    if (ret1.RetornaVal)
                                    {
                                        //pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        //cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                        Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VALOR ==============================================");
                                        return ret1;
                                    }
                                    else if (ret1.Retorna)
                                    {
                                        //pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        //cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                        Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO VACIO ==============================================");
                                        return ret1;
                                    }
                                    else if (ret1.Detener)
                                    {
                                        //pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        //cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima
                                        Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ============================================= SWITCH RETORNO DETENER ==============================================");
                                        //return ret1;
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Caso repetido linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Caso repetido", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Caso de tipo incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Caso de tipo incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Condicion de caso invalida(null) linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Condicion de caso invalida(null)", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                    }
                    break;
            }
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t ===================================== RETORNANDO SALVATION switch =============================================");
            return new RetornoAc("-", "-", "0", "0");
            #endregion
        }

        private void VariablesLocales(ParseTreeNode Nodo)
        {
            /*
             VARLOCAL.Rule = LSTIDL + dospuntos + TIPODATO + puntocoma //Declaracion de variables *
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id *
                            | LSTIDL + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion e inicializacion de variables *
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id, LSTARREGLO es del mismo tipo que Condicion *
                            | LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id, id es un arreglo *
                            | id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables *
                            | id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo *
                            | id + incrementa + puntocoma //id tipo Int, Double
                            | id + decrementa + puntocoma //id tipo Int, Double
                            | id + corchA + CONDICION + corchC + ToTerm("=") + CONDICION + puntocoma //Asignando valor a posicion de arreglo
             */
            //Console.WriteLine("VARIABLE LOCAL");
            switch (Nodo.ChildNodes.Count)
            {
                case 3: //| id + incrementa + puntocoma //id tipo Int, Double
                        //| id + decrementa + puntocoma //id tipo Int, Double
                    #region
                    String id3 = Nodo.ChildNodes[0].Token.Value.ToString();
                    Simbolo sim = RetornarSimbolo(id3);

                    if(sim == null) //Si no existe en el nivel actual se busca en las globales
                    {
                        sim = cimaG.RetornarSimbolo(id3);
                    }

                    if(sim != null)
                    {
                        if(sim.TipoObjeto.Equals(Reservada.var))
                        {
                            if (Nodo.ChildNodes[1].Term.Name.Equals("++"))
                            {
                                if (sim.Tipo.Equals(Reservada.Entero))
                                {
                                    int incrementa = Convert.ToInt32(sim.Valor) + 1;
                                    Console.WriteLine("Se incremento " + id3 + "=" + incrementa);
                                    sim.Valor = (incrementa + "");
                                }
                                else if (sim.Tipo.Equals(Reservada.Doublee))
                                {
                                    double incrementa = Double.Parse(sim.Valor) + 1;
                                    Console.WriteLine("Se incremento " + id3 + "=" + incrementa);
                                    sim.Valor = (incrementa + "");
                                }
                                else
                                {
                                    Console.WriteLine("Errora");
                                    Console.WriteLine("Error Semantico-->Variable no numerica para incrementar linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Valor no numerica para incrementar", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else if (Nodo.ChildNodes[1].Term.Name.Equals("--"))
                            {
                                if (sim.Tipo.Equals(Reservada.Entero))
                                {
                                    int decrementa = Convert.ToInt32(sim.Valor) - 1;
                                    Console.WriteLine("Se decremento " + id3 + "=" + decrementa);
                                    sim.Valor = (decrementa + "");
                                }
                                else if (sim.Tipo.Equals(Reservada.Doublee))
                                {
                                    double decrementa = Double.Parse(sim.Valor) - 1;
                                    Console.WriteLine("Se decremento " + id3 + "=" + decrementa);
                                    sim.Valor = (decrementa + "");
                                }
                                else
                                {
                                    Console.WriteLine("Errorc");
                                    Console.WriteLine("Error Semantico-->Variable no numerica para decrementar linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Variable no numerica para incrementar", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("ERROR EN INCREMENTO O DECREMENTO!");
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Error en incremento o decremento", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->No puede operar un arreglo linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "No puede operar un arreglo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable no existe linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable no existe", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                    }                    
                    #endregion
                    break;
                case 4: // LSTIDL + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                        // id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                    #region
                    switch (Nodo.ChildNodes[1].Token.Value.ToString())
                    {
                        case ":": // LSTIDL + dospuntos + TIPODATO + puntocoma //Declaracion de variables
                            
                            String td1 = getTipoDato(Nodo.ChildNodes[2]);
                            DeclaracionVariableL(td1, Nodo.ChildNodes[0]);

                            break;
                        case "=": // id + ToTerm("=") + CONDICION + puntocoma //Asignacion de variables
                            #region
                            String id = Nodo.ChildNodes[0].Token.Value.ToString();

                            Simbolo var = RetornarSimbolo(id); //Busco en mi nivel actual

                            if(var == null) //Si no existe en mi nivel actual busco en las globales
                            {
                                var = cimaG.RetornarSimbolo(id);
                                Console.WriteLine(">>> Se busco en las globalbes <<<");
                            }

                            if (var != null) //Si la variable existe
                            {
                                Retorno ret = Condicion(Nodo.ChildNodes[2]);

                                if (ret != null)
                                {
                                    if (ret.Tipo.Equals(var.Tipo)) //Si son del mismo tipo se pueden asignar (variable con variable)
                                    {
                                        Console.WriteLine("Se asigno variable: " + id + " --> " + ret.Valor + " (" + ret.Tipo + ")");
                                        var.Valor = ret.Valor; // Asignamos el nuevo valor al id
                                    }
                                    else if (ret.Tipo.Equals(Reservada.arreglo) && var.TipoObjeto.Equals(Reservada.arreglo))
                                    {
                                        Simbolo arregloAsignar = RetornarSimbolo(ret.Valor); // ret.Valor contiene el nombre del arreglo a asignar

                                        if(arregloAsignar == null) //Si no existe en mi nivel actual busco en las globales
                                        {
                                            arregloAsignar = cimaG.RetornarSimbolo(ret.Valor);
                                            Console.WriteLine(">>> Se busco en las globalbes <<<");
                                        }
                                        Console.WriteLine(">>> SE RECONOCIO ASIGNACION DE ARREGLOS PRRONES <<<");

                                        if (arregloAsignar != null)
                                        {
                                            Console.WriteLine(">>> SE RECONOCIO ASIGNACION DE ARREGLOS PRRONES <<<");
                                            Console.WriteLine("Se asigno ARREGLO: " + id + " --> " + ret.Valor + " (" + ret.Tipo + ")");
                                            if (arregloAsignar.Tipo.Equals(var.Tipo))
                                            {
                                                if (var.Arreglo.Count >= arregloAsignar.Arreglo.Count)
                                                {
                                                    int i = 0;
                                                    foreach (Celda cel in arregloAsignar.Arreglo)
                                                    {
                                                        var.Arreglo.ElementAt(i).valor = arregloAsignar.Arreglo.ElementAt(i).valor;
                                                        i++;
                                                    }
                                                }
                                                else
                                                {
                                                    int i = 0;
                                                    foreach (Celda cel in var.Arreglo)
                                                    {
                                                        var.Arreglo.ElementAt(i).valor = arregloAsignar.Arreglo.ElementAt(i).valor;
                                                        i++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Asignacion no valida de arreglo linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida de arreglo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Asignacion no valida, expresion incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable " + id + " no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + id + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            } 
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case 6: // LSTIDL + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion e inicializacion de variables
                        // id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                    #region
                    switch (Nodo.ChildNodes[1].Token.Value.ToString())
                    {
                        case ":": //LSTIDG + dospuntos + TIPODATO + ToTerm("=") + CONDICION + puntocoma //Declaracion y asignacion de variables

                            String td1 = getTipoDato(Nodo.ChildNodes[2]);
                            Retorno ret0 = Condicion(Nodo.ChildNodes[4]);

                            DeclaracionAsignacionVariableL(td1, ret0, Nodo.ChildNodes[0]);

                            break;
                        case "=": //id + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Asignando valores al arreglo
                            #region
                            String id6 = Nodo.ChildNodes[0].Token.Value.ToString();

                            Simbolo ArregloA = RetornarSimbolo(id6);

                            if (ArregloA == null) //Si no existe en mi nivel actual busco en las globales
                            {
                                ArregloA = cimaG.RetornarSimbolo(id6);
                                Console.WriteLine(">>> Se busco en las globalbes <<<");
                            }

                            if (ArregloA != null)
                            {
                                if (ArregloA.TipoObjeto.Equals(Reservada.arreglo)) //Verificando si es una arreglo
                                {
                                    int tam = Int32.Parse(ArregloA.Valor);
                                    arreglo = new List<Celda>(); //Este arreglo TEMPORAL es solo para almacenar los datos del arreglo asignado
                                    AsignarValidarArreglo(ArregloA.Tipo, Nodo.ChildNodes[3], getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));

                                    if (arreglo.Count <= tam) //Si el arreglo nuevo es menor o igual al arreglo que se desea asignar
                                    {
                                        List<Celda> arregloAct = ArregloA.Arreglo;
                                        int i = 0;
                                        while (i < arreglo.Count) //Si el arreglo donde almacenamos
                                        {
                                            arregloAct.ElementAt(i).valor = arreglo.ElementAt(i).valor; //Asignamos al arreglo actual los datos del arreglo TEMPORAL.
                                            i++;
                                        }
                                        Console.WriteLine("Se asigno arreglo: " + ArregloA.Nombre + "[" + tam + "]" + "(" + ArregloA.Tipo + ")");
                                        ArregloA.Arreglo = arregloAct; //Asignando el arreglo ya con los valores del arreglo que se asigno

                                        foreach (Celda cel in arregloAct)
                                        {
                                            Console.WriteLine("\t\t" + ArregloA.Nombre + ":" + ArregloA.Tipo + "{" + cel.tipo + "-" + cel.valor + "}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Cantidad de valores asignados al arreglo incorrectas linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de valores asignados al arreglo incorrectas", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->La variable no es un arreglo linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no es un arreglo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->La variable no existe linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "La variable no existe, tipo", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case 7: // LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id
                        //Si el arreglo solo es declarado se le asignará valores por defecto como las variables normales.
                        // id + corchA + CONDICION + corchC + ToTerm("=") + CONDICION + puntocoma //Asignando valor a posicion de arreglo
                    #region
                    switch(Nodo.ChildNodes[0].Term.Name)
                    {
                        case "LSTIDL": // LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id
                            #region
                            String tipodato7 = getTipoDato(Nodo.ChildNodes[2]);
                            Retorno id7 = devolverUnicoID(Nodo.ChildNodes[0], tipodato7);
                            
                            if (id7 != null)
                            {
                                if (!ExisteSimbolo(id7.Valor))
                                {
                                    Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                                    if (dimens != null)
                                    {
                                        if (dimens.Tipo.Equals(Reservada.Entero))
                                        {
                                            int tam = Int32.Parse(dimens.Valor);

                                            if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                            {
                                                arreglo = new List<Celda>();

                                                int i = 0;
                                                while (i < tam) //Este llena el arreglo con los datos iniciales segun sea el tipo de dato
                                                {
                                                    arreglo.Add(new Celda(tipodato7, getInicialDato(tipodato7)));
                                                    i++;
                                                }
                                                Console.WriteLine("Se creo arreglo: " + id7.Valor + "[" + tam + "]" + "(" + tipodato7 + ")");
                                                cima.addSimbolo(Reservada.arregloLocal, id7.Valor, tam + "", tipodato7, Reservada.arreglo, id7.Linea, id7.Columna, true, arreglo);
                                                foreach (Celda cel in arreglo)
                                                {
                                                    Console.WriteLine("\t\t" + id7.Valor + ":" + tipodato7 + "{" + cel.tipo + "-" + cel.valor + "}");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id7.Linea + " columna:" + id7.Columna);
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id7.Linea, id7.Columna));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Accion no valida");
                                Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            }
                            #endregion
                            break;
                        case "id": // id + corchA + CONDICION + corchC + ToTerm("=") + CONDICION + puntocoma //Asignando valor a posicion de arreglo
                            Console.WriteLine("ASIGNANDO ALGO BIEN CHINGON A MIS ARREGLOS PERRONES PERRO");
                            #region
                            String idv = Nodo.ChildNodes[0].Token.Value.ToString();
                            
                            Simbolo arre = RetornarSimbolo(idv);

                            if (arre == null) //Si no existe en mi nivel actual busco en las globales
                            {
                                arre = cimaG.RetornarSimbolo(idv);
                                Console.WriteLine(">>> Se busco en las globales <<<");
                            }

                            if (arre != null)
                            {
                                if (arre.TipoObjeto.Equals(Reservada.arreglo))
                                {
                                    Retorno posicion = Condicion(Nodo.ChildNodes[2]); 

                                    Retorno asignacion = Condicion(Nodo.ChildNodes[5]); // Valor que se desea asignar a la posicion del arreglo

                                    if (posicion != null)
                                    {
                                        if (posicion.Tipo.Equals(Reservada.Entero))
                                        {
                                            int pos = Int32.Parse(posicion.Valor);

                                            if(asignacion != null)
                                            {
                                                if(arre.Tipo.Equals(asignacion.Tipo))
                                                {
                                                    arre.Arreglo.ElementAt(pos).valor = asignacion.Valor;
                                                    //return new Retorno(sim.Tipo, sim.Arreglo.ElementAt(pos).valor, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])); //Retorno el valor de la posicion del arreglo posicionado
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Error Semantico-->Asignacion de arreglo no valida(tipo) linea:" + getLinea(Nodo.ChildNodes[4]) + " columna:" + getColumna(Nodo.ChildNodes[4]));
                                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion de arreglo no valida(tipo)", getLinea(Nodo.ChildNodes[4]), getColumna(Nodo.ChildNodes[4])));
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->Asignacion de arreglo no valida(null) linea:" + getLinea(Nodo.ChildNodes[4]) + " columna:" + getColumna(Nodo.ChildNodes[4]));
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion de arreglo no valida(null)", getLinea(Nodo.ChildNodes[4]), getColumna(Nodo.ChildNodes[4])));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Posicion de arreglo no valida(tipo) linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Posicion de arreglo no valida(tipo)", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Posicion de arreglo no valida(null) linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Posicion de arreglo no valida(null)", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Variable " + idv + " existente como variable linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + idv + " existente como variable", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable " + idv + " no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + idv + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                    }                    
                    #endregion
                    break;
                case 9: // LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + id + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id, id es un arreglo
                    #region
                    String tipodato9 = getTipoDato(Nodo.ChildNodes[2]);
                    Retorno id9 = devolverUnicoID(Nodo.ChildNodes[0], tipodato9);

                    if (id9 != null)
                    {
                        if (!ExisteSimbolo(id9.Valor))
                        {
                            Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                            if (dimens != null)
                            {
                                if (dimens.Tipo.Equals(Reservada.Entero))
                                {
                                    int tam = Int32.Parse(dimens.Valor);

                                    if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                    {
                                        arreglo = new List<Celda>(); //Arreglo Nuevo

                                        String id = Nodo.ChildNodes[7].Token.Value.ToString();

                                        Simbolo ObjArreglo = RetornarSimbolo(id);

                                        if (ObjArreglo == null) //Si no existe en mi nivel actual busco en las globales
                                        {
                                            ObjArreglo = cimaG.RetornarSimbolo(id);
                                            Console.WriteLine(">>> Se busco en las globalbes <<<");
                                        }

                                        if (ObjArreglo != null)
                                        {
                                            if (ObjArreglo.TipoObjeto.Equals(Reservada.arreglo)) //Si el objeto id buscado es un Arreglo
                                            {
                                                if (ObjArreglo.Tipo.Equals(tipodato9))
                                                {
                                                    int tamArregloAsig = Int32.Parse(ObjArreglo.Valor);
                                                    List<Celda> arregloAsignado = ObjArreglo.Arreglo;

                                                    if (tam == tamArregloAsig)
                                                    {
                                                        int i = 0;
                                                        while (i < tam) //Asignando los valores del arreglo asignado al nuevo arreglo
                                                        {
                                                            arreglo.Add(arregloAsignado.ElementAt(i));
                                                            i++;
                                                        }
                                                        Console.WriteLine("Se creo arreglo: " + id9.Valor + "[" + tam + "]" + "(" + tipodato9 + ")");
                                                        cima.addSimbolo(Reservada.arregloLocal, id9.Valor, tam + "", tipodato9, Reservada.arreglo, id9.Linea, id9.Columna, true, arreglo);
                                                        foreach (Celda cel in arreglo)
                                                        {
                                                            Console.WriteLine("\t\t" + id9.Valor + ":" + tipodato9 + "{" + cel.tipo + "-" + cel.valor + "}");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Error Semantico-->El arreglo asignado no posee las dimensiones correctas linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                        lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo asignado no posee las dimensiones correctas", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Error Semantico-->El arreglo asignado no posee un tipo de dato valido linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                    lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo asignado no posee un tipo de dato valido", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->El objeto asignado no es Arreglo sino ID linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "El objeto asignado no es Arreglo sino ID", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Arreglo asignado no existente linea:" + getLinea(Nodo.ChildNodes[7]) + " columna:" + getColumna(Nodo.ChildNodes[7]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Arreglo asignado no existente", getLinea(Nodo.ChildNodes[7]), getColumna(Nodo.ChildNodes[7])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id9.Linea + " columna:" + id9.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id9.Linea, id9.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Accion no valida");
                        Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                    }
                    #endregion
                    break;
                case 11: // LSTIDL + dospuntos + TIPODATO + corchA + CONDICION + corchC + ToTerm("=") + llaveA + LSTARREGLO + llaveC + puntocoma //Declaracion de arreglo, Condicion es entero, LSTIDL es solo un id, LSTARREGLO es del mismo tipo que Condicion
                    #region
                    String tipodato11 = getTipoDato(Nodo.ChildNodes[2]);
                    Retorno id11 = devolverUnicoID(Nodo.ChildNodes[0], tipodato11);

                    if (id11 != null)
                    {
                        if (!ExisteSimbolo(id11.Valor))
                        {
                            Retorno dimens = Condicion(Nodo.ChildNodes[4]);

                            if (dimens != null)
                            {
                                if (dimens.Tipo.Equals(Reservada.Entero))
                                {
                                    int tam = Int32.Parse(dimens.Valor);

                                    if (tam > 0) //Verificando que el tamanio arreglo sea valido
                                    {
                                        arreglo = new List<Celda>(); //Esto contendra los valores del arreglo

                                        AsignarValidarArreglo(tipodato11, Nodo.ChildNodes[8], id11.Linea, id11.Columna);//LLENANDO MI LISTA DE LOS VALORES DEL ARREGLO

                                        if (arreglo.Count == tam) //Verificando que el tamanio del arreglo sea de la dimension correcta
                                        {
                                            Console.WriteLine("Se creo arreglo: " + id11.Valor + "[" + tam + "]" + "(" + tipodato11 + ")");

                                            cima.addSimbolo(Reservada.arregloLocal, id11.Valor, tam + "", tipodato11, Reservada.arreglo, id11.Linea, id11.Columna, true, arreglo);

                                            foreach (Celda cel in arreglo)
                                            {
                                                Console.WriteLine("\t\t" + id11.Valor + ":" + tipodato11 + "{" + cel.tipo + "-" + cel.valor + "}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Cantidad de valores asignados al arreglo incorrectas linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de valores asignados al arreglo incorrectas", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tamanio linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tamanio", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Dimension arreglo incorrecta, tipo linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Dimension arreglo incorrecta, tipo", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->El arreglo ya existe linea:" + id11.Linea + " columna:" + id11.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "El arreglo ya existe, tipo", id11.Linea, id11.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Accion no valida");
                        Console.WriteLine("Error Semantico-->Acciones no validas para arreglos linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Acciones no validas para arreglos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                    }
                    #endregion
                    break;
            }
        }

        private void DeclaracionVariableL(String tipodato, ParseTreeNode Nodo)
        {                                                                       //METODO SOLO PARA LAS VARIABLES LOCALES
            //  LSTIDL + coma + id
            //| id
            #region
            switch (Nodo.Term.Name)
            {
                case "LSTIDL":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        DeclaracionVariableL(tipodato, hijo);
                    }
                    break;
                case "id":
                    String id = Nodo.Token.Value.ToString();

                    if (!ExisteSimbolo(id))
                    {
                        Console.WriteLine("se agrego variable a cima: " + cima.Nivel + " - " + cima.Tipo+" tamanio:"+cima.ts.Count);

                        cima.addSimbolo(Reservada.varLocal, id, getInicialDato(tipodato), tipodato, Reservada.var, getLinea(Nodo), getColumna(Nodo), false, null);
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable ya existente linea:" + getLinea(Nodo) + " columna:" + getColumna(Nodo));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable ya existente", getLinea(Nodo), getColumna(Nodo)));
                    }
                    break;
            }
            #endregion
        }

        private void DeclaracionAsignacionVariableL(String tipodato, Retorno ret, ParseTreeNode Nodo)
        {                                                                       //METODO SOLO PARA LAS VARIABLES LOCALES
            // LSTIDL + coma + id
            //| id

            #region
            switch (Nodo.ChildNodes.Count)
            {
                case 3:
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        //CREO QUE LA CONDICION IF ESTA MALA!, CREO QUE ES
                        //hijo.Token.Value.ToString().Equals(",")
                        if (!Nodo.ChildNodes[1].Token.Value.ToString().Equals(",")) 
                        {
                            DeclaracionAsignacionVariableL(tipodato, ret, hijo);
                        }
                    }
                    break;
                case 1:
                    String id = Nodo.ChildNodes[0].Token.Value.ToString();

                    if (!ExisteSimbolo(id))
                    {
                        if (ret != null)
                        {
                            if (ret.Tipo.Equals(tipodato)) //Si son del mismo tipo se pueden asignar (variable con variable)
                            {
                                Console.WriteLine("Se creo variable: " + id + " --> " + ret.Valor + " (" + ret.Tipo + ")");
                                cima.addSimbolo(Reservada.varLocal, id, ret.Valor, tipodato, Reservada.var, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]), true, null);

                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Asignacion no valida, tipo de dato incorrecto linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Asignacion no valida, expresion incorrecta linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable ya existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable ya existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                    }
                    break;
            }
            #endregion
        }

        private void AsignarValidarArreglo(String tipodato, ParseTreeNode Nodo, String linea, String col)
        {
            //LSTARREGLO + coma + CONDICION
            //| CONDICION
            #region
            switch (Nodo.Term.Name)
            {
                case "LSTARREGLO":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        AsignarValidarArreglo(tipodato, hijo, linea, col);
                    }
                    break;
                case "CONDICION":
                    Retorno retorno = Condicion(Nodo);

                    if (retorno != null)
                    {
                        if (retorno.Tipo.Equals(tipodato))
                        {
                            arreglo.Add(new Celda(retorno.Tipo, retorno.Valor));
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Tipo de dato de arreglo incorrecta linea:" + retorno.Linea + " columna:" + retorno.Columna);
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Tipo de dato de arreglo incorrecta", retorno.Linea, retorno.Columna));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Asignacion de valor arreglo incorrecta linea:" + linea + " columna:" + col);
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion de valor arreglo incorrecta", linea + "", col + ""));
                    }
                    break;
            }
            #endregion
        }

        private Retorno Condicion(ParseTreeNode Nodo)
        {
            //CONDICIONES
            //CONDICION.Rule = CONDICION + ToTerm("||") + COND1
            //            | COND1
            //            ;
            //COND1.Rule = COND1 + ToTerm("!&") + COND2
            //            | COND2
            //            ;
            //COND2.Rule = COND2 + ToTerm("&&") + COND3
            //            | COND3
            //            ;
            //COND3.Rule = ToTerm("!") + COND4
            //            | COND4
            //            ;
            //COND4.Rule = COND4 + ToTerm("<=") + COND5
            //            | COND5
            //            ;
            //COND5.Rule = COND5 + ToTerm(">=") + COND6
            //            | COND6
            //            ;
            //COND6.Rule = COND6 + ToTerm("<") + COND7
            //            | COND7
            //            ;
            //COND7.Rule = COND7 + ToTerm(">") + COND8
            //            | COND8
            //            ;
            //COND8.Rule = COND8 + ToTerm("==") + COND9
            //            | COND9
            //            ;
            //COND9.Rule = COND9 + ToTerm("!=") + EXPRESION
            //            | EXPRESION
            //            ;
            //Console.WriteLine("===============>>CONDICION<<===============");
            if (Nodo.ChildNodes.Count == 3)
            {
                switch (Nodo.ChildNodes[0].Term.Name)
                {
                    case "CONDICION": // CONDICION + ToTerm("||") + COND1
                        #region
                        Retorno condA1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condA2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condA1 != null) && (condA2 != null)) // Si ambos son distintos de null entra
                        {
                            if (condA1.Tipo.Equals(Reservada.Booleano) && condA2.Tipo.Equals(Reservada.Booleano)) // si ambos son booleanos
                            {
                                if (condA1.Valor.Equals("False") && condA2.Valor.Equals("False")) // si ambos son false 
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                            }
                            else
                            {
                                Console.WriteLine("Imposible evaluar condicion OR con valores no booleanos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion OR con valores no booleanos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion OR");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion OR", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND1": // COND1 + ToTerm("!&") + COND2
                        #region
                        Retorno condB1a = Condicion(Nodo.ChildNodes[0]);
                        Retorno condB2b = Condicion(Nodo.ChildNodes[2]);

                        if ((condB1a != null) && (condB2b != null)) // Si ambos son distintos de null entra
                        {
                            if (condB1a.Tipo.Equals(Reservada.Booleano) && condB2b.Tipo.Equals(Reservada.Booleano)) // si ambos son booleanos
                            {
                                if (condB1a.Valor.Equals(condB2b.Valor)) // si ambos son true o false 
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else
                            {
                                Console.WriteLine("Imposible evaluar condicion XOR con valores no booleanos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion XOR con valores no booleanos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion XOR");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion XOR", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND2": // COND2 + ToTerm("&&") + COND3
                        #region
                        Retorno condB1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condB2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condB1 != null) && (condB2 != null)) // Si ambos son distintos de null entra
                        {
                            if (condB1.Tipo.Equals(Reservada.Booleano) && condB2.Tipo.Equals(Reservada.Booleano)) // si ambos son booleanos
                            {
                                if (condB1.Valor.Equals("True") && condB2.Valor.Equals("True")) // si ambos son true 
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else 
                            {
                                Console.WriteLine("Imposible evaluar condicion AND con valores no booleanos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion AND con valores no booleanos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion AND");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion AND", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND4": // COND4 + ToTerm("<=") + COND5
                        #region
                        Retorno condC1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condC2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condC1 != null) && (condC2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condC1.Tipo.Equals(Reservada.Entero) && condC2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es entero y otro double
                                     (condC1.Tipo.Equals(Reservada.Doublee) && condC2.Tipo.Equals(Reservada.Entero)) ||   // si uno es double y otro entero
                                         (condC1.Tipo.Equals(Reservada.Entero) && condC2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son enteros
                                             (condC1.Tipo.Equals(Reservada.Doublee) && condC2.Tipo.Equals(Reservada.Doublee)))   // si ambos son double
                            {
                                double val1 = double.Parse(condC1.Valor);
                                double val2 = double.Parse(condC2.Valor);

                                if (val1 <= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else if((condC1.Tipo.Equals(Reservada.Cadena) && condC2.Tipo.Equals(Reservada.Cadena)) ||       //Si ambos son String
                                    (condC1.Tipo.Equals(Reservada.Caracter) && condC2.Tipo.Equals(Reservada.Caracter)))     //Si ambos son Char
                            {
                                int v1 = getCantAscii(condC1.Valor);
                                int v2 = getCantAscii(condC2.Valor);

                                if (v1 <= v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion <= con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion <= con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion <=");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion <=", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND5": // COND5 + ToTerm(">=") + COND6
                        #region
                        Retorno condD1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condD2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condD1 != null) && (condD2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condD1.Tipo.Equals(Reservada.Entero) && condD2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es entero y otro double
                                     (condD1.Tipo.Equals(Reservada.Doublee) && condD2.Tipo.Equals(Reservada.Entero)) ||   // si uno es double y otro entero
                                         (condD1.Tipo.Equals(Reservada.Entero) && condD2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son enteros
                                             (condD1.Tipo.Equals(Reservada.Doublee) && condD2.Tipo.Equals(Reservada.Doublee)))   // si ambos son double
                            {
                                double val1 = double.Parse(condD1.Valor);
                                double val2 = double.Parse(condD2.Valor);

                                if (val1 >= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else if ((condD1.Tipo.Equals(Reservada.Cadena) && condD2.Tipo.Equals(Reservada.Cadena)) ||      //Si ambos son String
                                    (condD1.Tipo.Equals(Reservada.Caracter) && condD2.Tipo.Equals(Reservada.Caracter)))     //Si ambos son Char
                            {
                                int v1 = getCantAscii(condD1.Valor);
                                int v2 = getCantAscii(condD2.Valor);

                                if (v1 >= v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion >= con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion >= con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion >=");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion >=", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND6": // COND6 + ToTerm("<") + COND7
                        #region
                        Retorno condE1 = Condicion(Nodo.ChildNodes[0]);//COND6
                        Retorno condE2 = Condicion(Nodo.ChildNodes[2]);//COND7

                        if ((condE1 != null) && (condE2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condE1.Tipo.Equals(Reservada.Entero) && condE2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es Entero y otro Doublee
                                     (condE1.Tipo.Equals(Reservada.Doublee) && condE2.Tipo.Equals(Reservada.Entero)) ||   // si uno es Doublee y otro Entero
                                         (condE1.Tipo.Equals(Reservada.Entero) && condE2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son Enteros
                                             (condE1.Tipo.Equals(Reservada.Doublee) && condE2.Tipo.Equals(Reservada.Doublee)))   // si ambos son Doublees
                            {
                                double val1 = double.Parse(condE1.Valor);
                                double val2 = double.Parse(condE2.Valor);

                                if (val1 < val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else if ((condE1.Tipo.Equals(Reservada.Cadena) && condE2.Tipo.Equals(Reservada.Cadena)) ||      //Si ambos son String
                                    (condE1.Tipo.Equals(Reservada.Caracter) && condE2.Tipo.Equals(Reservada.Caracter)))     //Si ambos son Char
                            {
                                int v1 = getCantAscii(condE1.Valor);
                                int v2 = getCantAscii(condE2.Valor);

                                if (v1 < v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion < con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion < con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion <");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion <", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND7": // COND7 + ToTerm(">") + COND8
                        #region
                        Retorno condF1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condF2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condF1 != null) && (condF2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condF1.Tipo.Equals(Reservada.Entero) && condF2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es Entero y otro Doublee
                                     (condF1.Tipo.Equals(Reservada.Doublee) && condF2.Tipo.Equals(Reservada.Entero)) ||   // si uno es Doublee y otro Entero
                                         (condF1.Tipo.Equals(Reservada.Entero) && condF2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son Enteros
                                             (condF1.Tipo.Equals(Reservada.Doublee) && condF2.Tipo.Equals(Reservada.Doublee)))   // si ambos son Doublees
                            {
                                double val1 = double.Parse(condF1.Valor);
                                double val2 = double.Parse(condF2.Valor);

                                if (val1 > val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno True
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno False
                                }
                            }
                            else if ((condF1.Tipo.Equals(Reservada.Cadena) && condF2.Tipo.Equals(Reservada.Cadena)) ||      //Si ambos son String
                                    (condF1.Tipo.Equals(Reservada.Caracter) && condF2.Tipo.Equals(Reservada.Caracter)))     //Si ambos son Char
                            {
                                int v1 = getCantAscii(condF1.Valor);
                                int v2 = getCantAscii(condF2.Valor);

                                if (v1 > v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion > con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion > con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion >");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion >", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND8": // COND8 + ToTerm("==") + COND9
                        #region
                        Retorno condG1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condG2 = Condicion(Nodo.ChildNodes[2]);

                        if ((condG1 != null) && (condG2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condG1.Tipo.Equals(Reservada.Entero) && condG2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es Entero y otro Doublee
                                     (condG1.Tipo.Equals(Reservada.Doublee) && condG2.Tipo.Equals(Reservada.Entero)) ||   // si uno es Doublee y otro Entero
                                         (condG1.Tipo.Equals(Reservada.Entero) && condG2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son Enteros
                                             (condG1.Tipo.Equals(Reservada.Doublee) && condG2.Tipo.Equals(Reservada.Doublee)))   // si ambos son Doublees
                            {
                                double val1 = double.Parse(condG1.Valor);
                                double val2 = double.Parse(condG2.Valor);

                                if (val1 == val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else if ((condG1.Tipo.Equals(Reservada.Cadena) && condG2.Tipo.Equals(Reservada.Cadena)) ||      //Si ambos son String
                                    (condG1.Tipo.Equals(Reservada.Caracter) && condG2.Tipo.Equals(Reservada.Caracter)) ||   //Si ambos son Char
                                    (condG1.Tipo.Equals(Reservada.Booleano) && condG2.Tipo.Equals(Reservada.Booleano)))     //Si ambos son Boolean
                            {
                                int v1 = getCantAscii(condG1.Valor);
                                int v2 = getCantAscii(condG2.Valor);

                                if (v1 == v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion == con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion == con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion ==");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion ==", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "COND9": // COND9 + ToTerm("!=") + EXPRESION
                        #region
                        Retorno condH1 = Condicion(Nodo.ChildNodes[0]);
                        Retorno condH2 = Expresion(Nodo.ChildNodes[2]);

                        if ((condH1 != null) && (condH2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((condH1.Tipo.Equals(Reservada.Entero) && condH2.Tipo.Equals(Reservada.Doublee)) ||  // si uno es Entero y otro Doublee
                                     (condH1.Tipo.Equals(Reservada.Doublee) && condH2.Tipo.Equals(Reservada.Entero)) ||   // si uno es Doublee y otro Entero
                                         (condH1.Tipo.Equals(Reservada.Entero) && condH2.Tipo.Equals(Reservada.Entero)) ||     // si ambos son Enteros
                                             (condH1.Tipo.Equals(Reservada.Doublee) && condH2.Tipo.Equals(Reservada.Doublee)))   // si ambos son Doublees
                            {
                                double val1 = double.Parse(condH1.Valor);
                                double val2 = double.Parse(condH2.Valor);

                                if (val1 != val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno True
                                }
                            }
                            else if ((condH1.Tipo.Equals(Reservada.Cadena) && condH2.Tipo.Equals(Reservada.Cadena)) ||      //Si ambos son String
                                    (condH1.Tipo.Equals(Reservada.Caracter) && condH2.Tipo.Equals(Reservada.Caracter)) ||   //Si ambos son Char
                                    (condH1.Tipo.Equals(Reservada.Booleano) && condH2.Tipo.Equals(Reservada.Booleano)))     //Si ambos son Boolean
                            {
                                int v1 = getCantAscii(condH1.Valor);
                                int v2 = getCantAscii(condH2.Valor);

                                if (v1 != v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Console.WriteLine("Imposible evaluar condicion != con valores desconocidos");
                                Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion != con valores desconocidos", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Imposible evaluar condicion !=");
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion !=", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                }
            }
            else if(Nodo.ChildNodes.Count == 2)
            {
                // ToTerm("!") + COND4
                #region
                Retorno condB1 = Condicion(Nodo.ChildNodes[1]);

                if (condB1 != null) 
                {
                    if (condB1.Tipo.Equals(Reservada.Booleano)) // si es booleano
                    {
                        if (condB1.Tipo.Equals("True")) // si ambos son true 
                        {
                            return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])); //retorno False
                        }
                        else
                        {
                            return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])); //retorno True
                        }
                    }
                    else
                    {
                        Console.WriteLine("Imposible evaluar condicion NOT con valores no booleanos");
                        Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion NOT con valores no booleanos", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Imposible evaluar condicion NOT");
                    Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                    lstError.Add(new Error(Reservada.ErrorSemantico, "Imposible evaluar condicion NOT", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                    return null;
                }
                #endregion
            }
            else if (Nodo.ChildNodes.Count == 1)
            {
                if (Nodo.ChildNodes[0].Term.Name.Equals("EXPRESION")) //EXPRESION
                {
                    return Expresion(Nodo.ChildNodes[0]);
                }
                else // COND1, COND2, COND3, COND4.... CONDX
                {
                    return Condicion(Nodo.ChildNodes[0]);
                }
            }
            return null;
        }
        
        private Retorno Expresion(ParseTreeNode Nodo)
        {
            //EXPRESION.Rule = EXPRESION + mas + EXP1
            //            | EXP1
            //EXP1.Rule = EXP1 + menos + EXP2
            //            | EXP2
            //EXP2.Rule = EXP2 + por + EXP3
            //            | EXP3
            //EXP3.Rule = EXP3 + division + EXP4
            //            | EXP4
            //EXP4.Rule = EXP4 + modulo + EXP5
            //            | EXP5
            //EXP5.Rule = EXP5 + potencia + TERMINALES
            //            | TERMINALES
            //Console.WriteLine("Expresion");
            if (Nodo.ChildNodes.Count == 3)
            {
                Retorno ra1 = Expresion(Nodo.ChildNodes[0]);
                Retorno ra2 = Expresion(Nodo.ChildNodes[2]);
                String linea1 = getLinea(Nodo.ChildNodes[1]);
                String colum1 = getColumna(Nodo.ChildNodes[1]);

                switch (Nodo.ChildNodes[0].Term.Name)
                {
                    case "EXPRESION": // EXPRESION + mas + EXP1
                        #region
                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if (ra1.Tipo.Equals(Reservada.Cadena) || ra2.Tipo.Equals(Reservada.Cadena)) // Si alguno es String concateno
                            {
                                String concat = "";
                                //if (ra1.Tipo.Equals(Reservada.Cadena))
                                //{
                                //    concat = ra1.Valor + GetOperable(ra2);
                                //}
                                //else
                                //{
                                //    concat = GetOperable(ra1) + ra2.Valor;
                                //}
                                concat = GetOperable(ra1).Valor + GetOperable(ra2).Valor;
                                return new Retorno(Reservada.Cadena, concat, linea1, colum1);
                            }
                            else if (ra1.Tipo.Equals(ra2.Tipo) && !ra1.Tipo.Equals(Reservada.Cadena)) // Si ambos son del mismo tipo y distinto de Cadena
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);

                                if(ra1.Tipo.Equals(Reservada.Caracter))
                                {
                                    return new Retorno(Reservada.Caracter, suma + "", linea1, colum1);
                                }
                                else if(ra1.Tipo.Equals(Reservada.Booleano))
                                {
                                    return new Retorno(Reservada.Booleano, suma + "", linea1, colum1);
                                }
                                else
                                {
                                    return new Retorno(ra1.Tipo, suma + "", linea1, colum1);
                                }                                
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter)) || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Entero, suma + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee)) || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano)))
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, suma + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero)) || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano)))
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Entero, suma + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero)) || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee)))
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, suma + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter)) || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee)))
                            {
                                double suma = double.Parse(GetOperable(ra1).Valor) + double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, suma + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter)) || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano)))
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para suma linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para suma", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para suma linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para suma", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            if (ra2 == null)
                            {
                                
                            }
                            Console.WriteLine("Error Semantico-->Expresion no operable(null) linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable(null)", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "EXP1": // EXP1 + menos + EXP2
                        #region
                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Doublee)) //Cualquier combinacion de estos valores da Double
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee)))
                            {
                                double resta = double.Parse(GetOperable(ra1).Valor) - double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, resta + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Caracter)) //Cualquier combinacion de estos valores da Entero
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double resta = double.Parse(GetOperable(ra1).Valor) - double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Entero, resta + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Cadena)) //Cualquier combinacion de estos valores da Error
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Cadena)))
                            {
                                Console.WriteLine("Error Semantico--> Error al restar linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para restar", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para restar linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para restar", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "EXP2": // EXP2 + por + EXP3
                        #region
                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Doublee)) //Cualquier combinacion de estos valores da Double
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee)))
                            {
                                double mul = double.Parse(GetOperable(ra1).Valor) * double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, mul + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Caracter)) //Cualquier combinacion de estos valores da Entero
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double mul = double.Parse(GetOperable(ra1).Valor) * double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Entero, mul + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Cadena)) //Cualquier combinacion de estos valores da Error
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Cadena)))
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para multiplicar linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para multiplicar", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para multiplicar linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para multiplicar", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "EXP3": // EXP3 + division + EXP4
                        #region
                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Doublee)) //Cualquier combinacion de estos valores da Double
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double div = double.Parse(GetOperable(ra1).Valor) / double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, div + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Cadena)) //Cualquier combinacion de estos valores da Error
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Cadena)))
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para dividir linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para dividir", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para dividir linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para dividir", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "EXP4": // EXP4 + modulo + EXP5
                        #region
                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Doublee)) //Cualquier combinacion de estos valores da Double
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double mod = double.Parse(GetOperable(ra1).Valor) % double.Parse(GetOperable(ra2).Valor);
                                return new Retorno(Reservada.Doublee, mod + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Cadena)) //Cualquier combinacion de estos valores da Error
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Cadena)))
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para modulo linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para modulo", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para modulo linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para modulo", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                        
                    case "EXP5": // EXP5 + potencia + TERMINALES
                        #region
                        ra2 = Terminales(Nodo.ChildNodes[2]); //Aca vuelvo a asignar a ra2 el valor del TERMINAL

                        if ((ra1 != null) && (ra2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Caracter)) //Cualquier combinacion de estos valores da Double
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Entero)))
                            {
                                double pot = Math.Pow(double.Parse(GetOperable(ra1).Valor), double.Parse(GetOperable(ra2).Valor));
                                return new Retorno(Reservada.Doublee, pot + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Entero)) //Cualquier combinacion de estos valores da Entero
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Caracter)))
                            {
                                double pot = Math.Pow(double.Parse(GetOperable(ra1).Valor), double.Parse(GetOperable(ra2).Valor));
                                return new Retorno(Reservada.Entero, pot + "", linea1, colum1);
                            }
                            else if ((ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Cadena)) //Cualquier combinacion de estos valores da Error
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Booleano) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Booleano))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Doublee))
                                || (ra1.Tipo.Equals(Reservada.Doublee) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Entero))
                                || (ra1.Tipo.Equals(Reservada.Entero) && ra2.Tipo.Equals(Reservada.Cadena))
                                || (ra1.Tipo.Equals(Reservada.Cadena) && ra2.Tipo.Equals(Reservada.Caracter))
                                || (ra1.Tipo.Equals(Reservada.Caracter) && ra2.Tipo.Equals(Reservada.Cadena)))
                            {
                                Console.WriteLine("Error Semantico--> Error al potenciar linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para potencia", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                            else //SENOS vino un error INESPERADO PAPU (aiuda!!!)
                            {
                                Console.WriteLine("Error Semantico--> Expresion no operable para potencia linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable para potencia", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico--> linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                            return null;
                        }
                        #endregion
                }
            }
            else if (Nodo.ChildNodes.Count == 1)
            {
                if (Nodo.ChildNodes[0].Term.Name.Equals("TERMINALES"))
                {
                    return Terminales(Nodo.ChildNodes[0]);
                }
                else
                {
                    return Expresion(Nodo.ChildNodes[0]);
                }
            }
            return null;
        }

        private Retorno Terminales(ParseTreeNode Nodo)
        {
            /*
             TERMINALES.Rule = numero
                            | Decimal
                            | menos + numero // Especial coco a esto
                            | menos + Decimal // Especial coco a esto
                            | cadena
                            | caracter
                            | ToTerm("True")
                            | ToTerm("False")
                            | ToTerm("GetUser") + parentA + parentC
                            | id // Esto puede ser una VARIABLE o un ARREGLO
                            | id + punto + ToTerm("CompareTo") + parentA + CONDICION + parentC // Condicion debe ser tipo String, sino error
                            | id + parentA + parentC // Invocacion funcion si parametros
                            | id + parentA + ASIGNAR_PARAMETRO + parentC // Invocacion funcion con parametros
                            | id + corchA + CONDICION + corchC // Obteniendo valor de un arreglo, condicion debe ser entero para acceder a esa posicion del arreglo
                            | parentA + CONDICION + parentC
             */
            switch (Nodo.ChildNodes.Count)
            {
                case 1:
                    //1----
                    //| numero
                    //| Decimal
                    //| cadena
                    //| caracter
                    //| ToTerm("True")
                    //| ToTerm("False")
                    //| id //Esto puede ser una VARIABLE o un ARREGLO
                    #region
                    switch (Nodo.ChildNodes[0].Term.Name)
                    {
                        case "numero":
                            return new Retorno(Reservada.Entero, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "Decimal":
                            return new Retorno(Reservada.Doublee, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "cadena":
                            return new Retorno(Reservada.Cadena, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "CharLiteral":
                            return new Retorno(Reservada.Caracter, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "True":
                            return new Retorno(Reservada.Booleano, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "False":
                            return new Retorno(Reservada.Booleano, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "id": //Esto puede ser una VARIABLE o un ARREGLO

                            #region
                            String id = Nodo.ChildNodes[0].Token.Value.ToString();
                            Simbolo sim = RetornarSimbolo(id);

                            if (sim == null) //Si no existe en mi nivel actual busco en las globales
                            {
                                sim = cimaG.RetornarSimbolo(id);
                                Console.WriteLine(">>> Se busco en las globales <<<");
                            }

                            if (sim != null)
                            {
                                if(sim.TipoObjeto.Equals(Reservada.arreglo))
                                {
                                    return new Retorno(Reservada.arreglo, id, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                }
                                else
                                {
                                    return new Retorno(sim.Tipo, sim.Valor, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable "+id+" no Existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + id + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                return null;
                            }
                            #endregion

                    }
                    #endregion
                    break;
                case 2:
                    //| menos + numero // Especial coco a esto
                    //| menos + Decimal // Especial coco a esto  
                    
                    #region
                    switch (Nodo.ChildNodes[1].Term.Name)
                    {
                        case "numero":
                            Console.WriteLine(Nodo.ChildNodes[0].Term.Name + Nodo.ChildNodes[1].Token.Value.ToString() + " <<================= SE ESTA RETORNANDO UN NEGATIVO BIEN PRRON");
                            return new Retorno(Reservada.Entero, Nodo.ChildNodes[0].Term.Name + Nodo.ChildNodes[1].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                        case "Decimal":
                            return new Retorno(Reservada.Doublee, Nodo.ChildNodes[0].Term.Name + Nodo.ChildNodes[1].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                    }
                    #endregion
                    break;
                case 3:
                    //| ToTerm("GetUser") + parentA + parentC
                    //| id + parentA + parentC // Invocacion funcion si parametros
                    //| parentA + CONDICION + parentC
                    #region
                    switch (Nodo.ChildNodes[0].Term.Name)
                    {
                        case "GetUser": //| ToTerm("GetUser") + parentA + parentC

                            return new Retorno(Reservada.Cadena, nameUsuario, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            
                        case "id": //| id + parentA + parentC // Invocacion funcion si parametros

                            String id3 = Nodo.ChildNodes[0].Token.Value.ToString();
                            //Funciones func3 = tablafunciones.RetornarFuncion(id3);
                            Funciones func3 = tablafunciones.RetornarFuncionEvaluandoSobrecargaVoid(id3);

                            if (func3 != null)
                            {
                                if (!func3.getTipo().Equals(Reservada.Void)) //Si el metodo es de tipo VOID no retorna nada, ERROR
                                {
                                    nivelActual++; //Aumentamos el nivel actual ya que accedemos a otro metodo
                                    TablaSimbolos metodo4 = new TablaSimbolos(nivelActual, Reservada.Metodo, true, false, true);
                                    pilaSimbolos.Push(metodo4);
                                    cima = metodo4; //Estableciendo la tabla de simbolos cima

                                    Retorno reto = EjecutarFuncion(func3);

                                    nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                                    pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                    cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                    return reto;// ORIGINALMENTE DEVOLVER ESTO PERRO
                                    //return new Retorno(Reservada.Cadena, nameUsuario, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Funcion no retorna valor linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no retorna valor", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Funcion no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            break;
                        case "(": //| parentA + CONDICION + parentC

                            Retorno ret = Condicion(Nodo.ChildNodes[1]);

                            if (ret != null)
                            {
                                return new Retorno(ret.Tipo, ret.Valor, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Retornno de parentesis mala linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Retornno de parentesis mala", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            break;
                    }
                    #endregion
                    break;
                case 4:
                    //| id + parentA + ASIGNAR_PARAMETRO + parentC // Invocacion funcion con parametros
                    //| id + corchA + CONDICION + corchC // Obteniendo valor de un arreglo, condicion debe ser entero para acceder a esa posicion del arreglo
                    #region
                    switch(Nodo.ChildNodes[2].Term.Name)
                    {
                        case "ASIGNAR_PARAMETRO":
                            #region
                            //PRIMERO OBTENGO LA CANTIDAD DE VALORES EN MIS PARAMETROS ACEPTADOS POR EL ARREGLO
                            arreglo = new List<Celda>(); //Este arreglo es para almacenar los parametros del metodo que se invoco
                            ValidarParametrosMetodo(Nodo.ChildNodes[2]); // Mandamos los parametros
                            //AHORA BUSCO LA FUNCION EN BASE AL NOMBRE Y A MI ARREGLO DE PARAMETROS
                            String id4 = Nodo.ChildNodes[0].Token.Value.ToString();
                            //Funciones func4 = tablafunciones.RetornarFuncion(id4);
                            Funciones func4 = tablafunciones.RetornarFuncionEvaluandoSobrecarga(id4, arreglo);

                            if (func4 != null)
                            {
                                if (!func4.getTipo().Equals(Reservada.Void)) //Si el metodo es de tipo VOID no retorna nada, ERROR
                                {
                                    //arreglo = new List<Celda>(); //Este arreglo es para almacenar los parametros del metodo que se invoco
                                    //ValidarParametrosMetodo(Nodo.ChildNodes[2]); // Mandamos los parametros

                                    if(arreglo.Count == func4.getParametros().Count)
                                    {
                                        nivelActual++; //Aumentamos el nivel actual ya que accedemos a otro metodo
                                        TablaSimbolos metodo4 = new TablaSimbolos(nivelActual, Reservada.Metodo, true, false, true);
                                        pilaSimbolos.Push(metodo4);
                                        cima = metodo4; //Estableciendo la tabla de simbolos cima

                                        int cont = 0;
                                        foreach (Parametro parametro in func4.getParametros())
                                        {
                                            if(parametro.Tipo.Equals(arreglo.ElementAt(cont).tipo))
                                            {
                                                if(!ExisteSimbolo(parametro.Nombre)) //Verifico que no exista
                                                {
                                                    //arreglo.ElementAt(cont).valor <<-- es el valor de parametro que contiene el metodo cuando fue invocado
                                                    cima.addSimbolo(parametro.Ambito, parametro.Nombre, arreglo.ElementAt(cont).valor, parametro.Tipo, Reservada.var, parametro.Linea, parametro.Columna, true, null);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Error Semantico-->Parametro ya existente linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro ya existente", parametro.Linea, parametro.Columna));
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error Semantico-->Parametro introducido de tipo incompatible linea:" + parametro.Linea + " columna:" + parametro.Columna);
                                                lstError.Add(new Error(Reservada.ErrorSemantico, "Parametro introducido de tipo incompatible", parametro.Linea, parametro.Columna));
                                            }
                                            cont++;
                                        }

                                        Retorno reto = EjecutarFuncion(func4);

                                        nivelActual--; //Disminuimos el nivel actual ya que salimos del metodo invocado
                                        pilaSimbolos.Pop(); //Eliminando la tabla de simbolos cima actual
                                        cima = pilaSimbolos.Peek(); //Estableciendo la nueva tabla de simbolo cima

                                        return reto;// ORIGINALMENTE DEVOLVER ESTO PERRO
                                        //return new Retorno(Reservada.Cadena, nameUsuario, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Cantidad de parametros introducidos no valida linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Cantidad de parametros introducidos no valida", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    }                                    
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Funcion no retorna valor linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no retorna valor", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Funcion no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Funcion no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                            }
                            #endregion
                            break;
                        case "CONDICION":
                            #region
                            String idC = Nodo.ChildNodes[0].Token.Value.ToString();
                            Simbolo sim = RetornarSimbolo(idC);

                            if (sim == null) //Si no existe en mi nivel actual busco en las globales
                            {
                                sim = cimaG.RetornarSimbolo(idC);
                                Console.WriteLine(">>> Se busco en las globales <<<");
                            }

                            if (sim != null)
                            {
                                if (sim.TipoObjeto.Equals(Reservada.arreglo))
                                {
                                    Retorno posicion = Condicion(Nodo.ChildNodes[2]);

                                    if(posicion != null)
                                    {
                                        if(posicion.Tipo.Equals(Reservada.Entero))
                                        {
                                            int pos = Int32.Parse(posicion.Valor);
                                            return new Retorno(sim.Tipo, sim.Arreglo.ElementAt(pos).valor, getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])); //Retorno el valor de la posicion del arreglo posicionado
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error Semantico-->Posicion de arreglo no valida(tipo) linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                            lstError.Add(new Error(Reservada.ErrorSemantico, "Posicion de arreglo no valida(tipo)", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->Posicion de arreglo no valida(null) linea:" + getLinea(Nodo.ChildNodes[1]) + " columna:" + getColumna(Nodo.ChildNodes[1]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "Posicion de arreglo no valida(null)", getLinea(Nodo.ChildNodes[1]), getColumna(Nodo.ChildNodes[1])));
                                        return null;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->Variable " + idC + " existente como variable linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + idC + " existente como variable", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    return null;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable " + idC + " no existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + idC + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                return null;
                            }
                            #endregion
                    }
                    #endregion
                    break;
                case 6: //| id + punto + ToTerm("CompareTo") + parentA + CONDICION + parentC //Condicion debe ser tipo String, sino error
                    #region
                    String id6 = Nodo.ChildNodes[0].Token.Value.ToString();
                    Simbolo sim6 = RetornarSimbolo(id6);

                    Retorno ret6 = Condicion(Nodo.ChildNodes[4]);

                    if (sim6 == null) //Si no existe en mi nivel actual busco en las globales
                    {
                        sim6 = cimaG.RetornarSimbolo(id6); //Buscando en las globales
                        Console.WriteLine(">>> Se busco en las globales <<<");
                    }

                    if (sim6 != null)
                    {
                        if (ret6 != null)
                        {
                            if (!sim6.TipoObjeto.Equals(Reservada.arreglo))
                            {
                                if (sim6.Tipo.Equals(Reservada.Cadena))
                                {
                                    if(ret6.Tipo.Equals(Reservada.Cadena))
                                    {
                                        if (sim6.Valor.Equals(ret6.Valor))
                                        {
                                            return new Retorno(Reservada.Booleano, "True", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                        }
                                        else
                                        {
                                            return new Retorno(Reservada.Booleano, "False", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Semantico-->La expresion no es de tipo String linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                                        lstError.Add(new Error(Reservada.ErrorSemantico, "La expresion no es de tipo String", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                                        return null;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error Semantico-->El id no es de tipo String linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                    lstError.Add(new Error(Reservada.ErrorSemantico, "El id no es de tipo String", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                    return null;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Semantico-->Variable no operable(arreglo) linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                                lstError.Add(new Error(Reservada.ErrorSemantico, "Variable no operable(arreglo)", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Semantico-->Expresion no operable(null) linea:" + getLinea(Nodo.ChildNodes[3]) + " columna:" + getColumna(Nodo.ChildNodes[3]));
                            lstError.Add(new Error(Reservada.ErrorSemantico, "Expresion no operable(null)", getLinea(Nodo.ChildNodes[3]), getColumna(Nodo.ChildNodes[3])));
                            return null;
                        }                        
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Variable " + id6 + " no Existente linea:" + getLinea(Nodo.ChildNodes[0]) + " columna:" + getColumna(Nodo.ChildNodes[0]));
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Variable " + id6 + " no existente", getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0])));
                        return null;
                    }
                    #endregion
                    
                default:
                    return null;
                    
            }
            return null;
        }

        private Retorno EjecutarFuncion(Funciones func)
        {
            RetornoAc reta = Sentencias(func.getCuerpo());

            if(reta.RetornaVal)
            {
                return new Retorno(reta.Tipo, reta.Valor, reta.Linea, reta.Columna);
            }
            else if(reta.Retorna)
            {
                return new Retorno(reta.Tipo, reta.Valor, reta.Linea, reta.Columna);
            }
            return null;
        }

        private void ValidarParametrosMetodo(ParseTreeNode Nodo)
        {
            //ASIGNAR_PARAMETRO.Rule = ASIGNAR_PARAMETRO + coma + CONDICION
            //                         | CONDICION
            #region
            switch (Nodo.Term.Name)
            {
                case "ASIGNAR_PARAMETRO":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        ValidarParametrosMetodo(hijo);
                    }
                    break;
                case "CONDICION":
                    Retorno retorno = Condicion(Nodo);
                    Console.WriteLine("VALOR DE PARAMETRO DE UN METODO KACHEYUSE");
                    if (retorno != null)
                    {
                        arreglo.Add(new Celda(retorno.Tipo, retorno.Valor));
                    }
                    else
                    {
                        Console.WriteLine("Error Semantico-->Asignacion de parametro incorrecta linea:" + "0" + " columna:" + "0");
                        lstError.Add(new Error(Reservada.ErrorSemantico, "Asignacion de parametro incorrecta", "0" + "", "0" + ""));
                    }
                    break;
            }
            #endregion
        }

        private Retorno CondicionCaso(ParseTreeNode Nodo)
        {
            //TERMCASO.Rule = numero //int
            //                | Decimal //double
            //                | menos + numero //int
            //                | menos + Decimal //double
            //                | cadena //String

            switch (Nodo.ChildNodes[0].Term.Name)
            {
                case "numero":
                    return new Retorno(Reservada.Entero, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));

                case "Decimal":
                    return new Retorno(Reservada.Doublee, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));

                case "menos":

                    if(Nodo.ChildNodes[1].Term.Name.Equals("numero"))
                    {
                        return new Retorno(Reservada.Entero, "-"+Nodo.ChildNodes[1].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                    }
                    else if (Nodo.ChildNodes[1].Term.Name.Equals("Decimal"))
                    {
                        return new Retorno(Reservada.Doublee, "-"+Nodo.ChildNodes[1].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
                    }
                    return null;

                case "cadena":
                    return new Retorno(Reservada.Cadena, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));

                default:
                    return null;
            }
        }

        private string getTipoDato(ParseTreeNode Nodo)
        {
            switch (Nodo.ChildNodes[0].Term.Name)
            {
                case "Int":
                    return "Int";
                   
                case "Double":
                    return "Double";
                    
                case "Char":
                    return "Char";
                    
                case "Boolean":
                    return "Boolean";
                    
                case "String":
                    return "String";
                    
                default:
                    return "null";
                    
            }
        }

        private string getTipoDatoF(ParseTreeNode Nodo)
        {
            switch (Nodo.ChildNodes[0].Term.Name)
            {
                case "Int":
                    return "Int";
                    
                case "Double":
                    return "Double";
                    
                case "Char":
                    return "Char";
                    
                case "Boolean":
                    return "Boolean";
                    
                case "String":
                    return "String";
                    
                case "Void":
                    return "Void";
                    
                default:
                    return "null";
                    
            }
        }

        private string getInicialDato(string tipodato)
        {
            if (tipodato.Equals(Reservada.Entero))
            {
                return "0";
            }
            else if (tipodato.Equals(Reservada.Doublee))
            {
                return "0.0";
            }
            else if (tipodato.Equals(Reservada.Cadena))
            {
                return "\"\"";
            }
            else if (tipodato.Equals(Reservada.Caracter))
            {
                return "nulo";
            }
            else if (tipodato.Equals(Reservada.Booleano))
            {
                return "False";
            }
            return "";
        }

        private Retorno devolverUnicoID(ParseTreeNode Nodo, String tipodato)
        {
            if(Nodo.ChildNodes.Count == 1)
            {
                return new Retorno(tipodato, Nodo.ChildNodes[0].Token.Value.ToString(), getLinea(Nodo.ChildNodes[0]), getColumna(Nodo.ChildNodes[0]));
            }
            else
            {
                return null;
            }
        }

        private Retorno GetOperable(Retorno retornable)
        {
            switch(retornable.Tipo)
            {
                case "Char": //Cambia a ascii
                    retornable.Valor = GetAscii(retornable.Valor)+"";
                    return retornable;
                    
                case "Boolean": //Cambia a 0 o 1
                    if(retornable.Valor.Equals("True"))
                    {
                        retornable.Valor = "1";
                        return retornable;
                    }
                    else
                    {
                        retornable.Valor = "0";
                        return retornable;
                    }
                    
               default:
                    return retornable;                
            }
        }

        private int getCantAscii(String cadena)
        {
            Char[] Caracter = cadena.ToCharArray();

            int SumaAscii = 0;

            for (int i = 0; i < Caracter.Length; i++)
            {
                SumaAscii += GetAscii(Caracter[i] + "");
            }
            return SumaAscii;
        }

        private int GetAscii(String caracter)
        {
            return Encoding.ASCII.GetBytes(caracter)[0];
        }

        private string getLinea(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Line + 1) + "";
        }

        private string getColumna(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Column + 1) + "";
        }

        //====================================================================================================== BUSQUEDAS AVANZADAS ===================================================================================

        private Boolean ExisteSimbolo(string nombre)
        {
            if (!Vacio())
            {
                foreach (TablaSimbolos ts in pilaSimbolos)
                {
                    if(ts.Nivel == nivelActual) //Busca el simbolo en el nivel que se maneja actualmente
                    {
                        //foreach (Simbolo simbolo in ts.getTS())
                        foreach (Simbolo simbolo in ts.ts)
                        {
                            if (simbolo.Nombre.Equals(nombre))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private Simbolo RetornarSimbolo(String nombre)
        {
            if (!Vacio())
            {
                foreach (TablaSimbolos ts in pilaSimbolos)
                {
                    if(ts.Nivel == nivelActual) //Busca el simbolo en el nivel que se maneja actualmente
                    {
                        foreach (Simbolo simbolo in ts.ts)
                        {
                            if (simbolo.Nombre.Equals(nombre))
                            {
                                return simbolo;
                            }
                        }
                    }                    
                }
            }
            return null;
        }

        private Boolean Vacio()
        {
            if (!pilaSimbolos.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
