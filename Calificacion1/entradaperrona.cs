Import "aritmetica.YYY";
Import "ordenamiento.YYY";

nombre : String = "nombre1"; //declaración e inicialización de una variable
xxx : Double = 12.1+6.2-9.3*8.4+4.5+6.6*8.7-5.8+8.9*5.11-4.12*5.13;

yyy,zzz : Double; //declaración de varias variables
yyy = 12.5*4.1+5-1; //Asignando a variables
zzz = 12.5/6.1+12-2*5;

Contadores : Int [2+2+2]={2,4,6,4,2,0};
Nombres : String [1+1];
Contadores2 : Int[6] = Contadores;
Nombres={"juan", "jose"};

Main : Int (operacion : String)
{
    aaa : Double = 12.1+6.2-9.3*8.4+4.5/6.6*8.7-5.8/8.9*5.11-4.12/5.13;
    bbb : Double = 12.1+6.2-9*8.4+4.5/6*8.7-5/8.9*5.11-4.12/5.13;
    ccc : Double = 12.1+6-9*8+4/6*8-5/8*51-4/5;
    ddd : Double = 12.1+6-9*8+4+6*8-5-8*51-4+5;

    eee : Boolean = aaa > bbb;

    Print("el valor es " + eee);

    var1, var2, var3 : Int;
    var1 = 7*2;
    var2 = 3+2*2;
    numero : Int = -1+4;

    var3 = var1+var2;
    Print("Operaciones prueba ="+var3);
    Contadores[5] = 10;
    Suma : Int = Contadores[0]+Contadores[1]+Contadores[2]+Contadores[3]+Contadores[4]+Contadores[5];
    Print("Suma arreglo Contadores= "+Suma);

    arre : String[2];
    arre = Nombres; //Son arreglos

    Print(Nombres[0]);
    Nombres[0] = "EL PRRO MALDITO";
    Print(Nombres[0]);

    Return 679;
}









//==================================================
Import "aritmetica.YYY";

//---------WHILE
Main : String (operacion : String)
{
    cont : Int = 0;

    While ( cont <10 ){
        Print("Hola Mundo " + cont);
        If(cont == 5)
        {
            Break;
        }
        cont++;
    }
    Return "Va";
}
//--------------DO-WHILE
Main : String (operacion : String)
{
    cont : Int = 0;

    Do{
        Print("Hola Mundo " + cont);
        If(cont == 7)
        {
            Break;
        }Else{
            If(cont == 5)
            {
                Break;
            }
        }
        cont++;
    }While( cont <10 )

    Return "Va";
}
//--------------FOR
Main : String (operacion : String)
{
    cont : Int = 0;

    For ( i : Int = 0 ; i<10 ; i++){
        Print("Hola Mundo " + i);
        If(i == 7)
        {
            Break;
        }Else{
            If(i == 5)
            {
                Break;
            }
        }
    }

    Return "Va";
}

//----------------SWITCH
Main : String (operacion : String)
{
    cont : Int = 1;
    cont2 : Int = 1;

    Switch ( cont ){
        Case 1:

            Print("Hola Mundo " + "uno");

            i : Int =0;
                While(i<5){
                    j : Int = 5;
                    While(j>0){
                        If(j == 3){
                            Break;
                        }
                        Print("Iteracion Anidada -> " + i +" : "+ j);
                        j--;
                    }
                    i++;
                }

        Break;
        Case 2:
            Print("Hola Mundo " + "dos");

                Switch ( cont2 ){
                    Case 1:
                    
                        Print("Hola Mundo " + "uno");
                        
                    Break;
                    Case 2:
                        Print("Hola Mundo " + "dos");
                    Case 3:
                        Print("Hola Mundo " + "tres");
                    Break;
                    Default:
                        Print("Hola Mundo " + "default");
                    Break;
                }
                Print("FINALIZO DESPUES DEL SWITCH");
                Break;
        Case 3:
            Print("Hola Mundo " + "tres");
        Break;
        Default:
            Print("Hola Mundo " + "default");
        Break;
    }

    Return "Va";
}


//------------------
Import "aritmetica.YYY";

Main : String (operacion : String)
{
    uno : Boolean = True;
    dos : Boolean = False;

    If(uno || dos){
        Print("Entro al primer if, bien -> :)");
        If(25*2 > 50 ){
            Print("Entro al segundo if, mal -> :( ");    
            
        }Else{
            Print("Entro al segundo else, bien -> :)");
        }
    }Else{
        Print("No entro al if, mal -> :( ");
        
    }
}


Main : String (operacion : String)
{
    var : String = Mundo(4,1);
    Print(var);
    var2 : Int = 4+(2*2);
    Print(var2); 
    Print(operacion + " Gracias por participar");
    Return "Tankyou";
}
Mundo : String(val:Int, val2:Int)
{
    val3:Int = val+val2;
    Print("Entro al Mundo Void======================"+val3);
    var : String = Metodo1(22,"Alex ixva", 175.523);
    Return ">>>>"+var+"<<<<";
}

Metodo1 : String (edad:Int, nombre:String, altura:Double)
{
    Print("Entro Metodo1 String======================");
    Print("Mi nombre es: "+nombre+" y tengo "+ edad +" y me mide "+altura);
    Return ("Mi nombre es: "+nombre+" y tengo "+ (edad-2.5) +" y me mide "+altura);
}




//----------------------otros------------------
Main : String (operacion : String)
{
    If(operacion > "a")
    {
        Print("Se ingreso algo weno");
    }
    Else
    {
        Print("Se ingreso algo malo");
    }

    var1 : String = "Cadena1";
    var2 : String = "Cadena 1";
    
    If(var1 > var2)
    {
        Print("Cadena1 es mayor que cadena2");
    }
    Else
    {
        Print("Cadena2 es mayor que cadena1");
    }

    var3 : Char = 'a';
    var4 : Char = 'A';
    
    If(var3 > var4)
    {
        Print("char1 es mayor que char2");
    }
    Else
    {
        Print("char2 es mayor que char1");
    }

    var5 : Boolean = True;
    var6 : Boolean = False;
    
    If(var5 > var6)
    {
        Print("Booleanos iguales");
    }
    Else
    {
        Print("Distintos");
    }

    var7 : Int = 5;
    var8 : Double = 5.5;
    
    If(var7 > var8)
    {
        Print("numero1 es mayor que numero2");
    }
    Else
    {
        Print("numero2 es mayor que numero1");
    }

    Return "Tankyou";
}

//----------------SOBRECARGA DE METODOS -----------------------
Main : String (operacion : String)
{
    Print(operacion);
    var : String = Sobres(4,1);
    Print(var);
    var2 : Int = 4+(2*2);
    Print(var2); 
    Print(operacion + " Gracias por participar");
    Return "Tankyou";
}
Sobres : String(val:Int, val2:Int)
{
    val3:Int = val+val2;
    Print("Entro al Mundo Void======================"+val3);
    var : String = Sobres(22,"Alex ixva", 175.523);
    Return ">>>>"+var+"<<<<";
}

Sobres : String (edad:Int, nombre:String, altura:Double)
{
    Print(Sobres("hola","maquina"));
    Print("Entro Metodo1 String======================");
    Print("Mi nombre es: "+nombre+" y tengo "+ edad +" y me mide "+altura);
    Return ("Mi nombre es: "+nombre+" y tengo "+ (edad-2.5) +" y me mide "+altura);
}

Sobres : String(val:String, val2:String)
{
    Print("wizmichu prros "+val+" || "+val2);
    Print(Sobres()+10);
    Return "Salio del metodo mas engazado";
}

Sobres : Int()
{
    Return (33+33);
}



//----------------SOBRECARGA DE METODOS VOIDS -----------------------
Main : String (operacion : String)
{
    Print(operacion);
 Sobres(4,1);
    Return "Tankyou";
}
Sobres : Void(val:Int, val2:Int)
{
    Print("METODO2 con dos parametros String String [-->"+val+" || "+val2);
Sobres(22,"Alex ixva", 175.523);
}

Sobres : Void (edad:Int, nombre:String, altura:Double)
{
    Sobres("hola","maquina");
    Print("Metodo Void de 3 paramatros Int String double " + edad + " || "+nombre +" || "+altura);
}

Sobres : Void(val:String, val2:String)
{
    Print("METODO con dos parametros String String [-->"+val+" || "+val2);
    Sobres();
}

Sobres : Void()
{
   Print ("Metodo void sin parametros");
}



//*********************************************************************
Fibonacci:Int(num:Int)
{
    If((num == 0) || (num == 1))
    {
        Print("if");
        Return num;
    }
    Else
    {
        Print("else");
        var : Int = Fibonacci(num-1) + Fibonacci(num-2);
        Return var;
    }
}

Main:String(var:String)
{
    resultado:Int = Fibonacci(10);
    Print("la serie de fibonacci de 10 es:"+resultado);
    Return resultado;
}

//*********************************************************************
Fibonacci:Int(num:Int)
{
    If((num == 0) || (num == 1))
    {
        Print("if");
        Return num;
    }
    Else
    {
        Print("else");
        val1 : Int = Fibonacci(num-1);
        val2 : Int = Fibonacci(num-2);
        suma : Int = val1+val2;
        var : Int = suma;
        Return var;
    }
}

Main:String(var:String)
{
    resultado:Int = Fibonacci(10);
    Print("la serie de fibonacci de 10 es:"+Fibonacci(10));
    Return "prro";
}


//*****************************************************************************
Factorial:Int(num:Int)
{
    If(num == 0)
    {
        Return 1;
    }
    Else
    {
        Return num*Factorial(num-1);
    }
}

Main:String(consulta:String)
{
Print("El factorial de 5 es 120: "+Factorial(5));
}
