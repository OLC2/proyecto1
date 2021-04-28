Import "aritmetica.YYY";
Import "ordenamiento.YYY";

nombre : String = "nombre1"; //declaración e inicialización de una variable
val1,val2,val3 : Int; //declaración de varias variables

nombre= "Jose"; // asignacion directa
var2=getMedia(1,2,nombre); // asignación en términos de una expresión
var3= var2+(2*3); //asignación en términos de una expresión

xxx = 12.1+6.2-9.3*8.4+4.5+6.6*8.7-5.8+8.9*5.11-4.12*5.13;
yyy = 12.5*4.1+5-1;
zzz = 12.5/6.1+12-2*5;
aaa = 12.1+6.2-9.3*8.4+4.5/6.6*8.7-5.8/8.9*5.11-4.12/5.13;
bbb = 12.1+6.2-9*8.4+4.5/6*8.7-5/8.9*5.11-4.12/5.13;
ccc = 12.1+6-9*8+4/6*8-5/8*51-4/5;
ddd = 12.1+6-9*8+4+6*8-5-8*51-4+5;

Contadores : Int [1+getIndice()]={1,2,3,4,5,6};
Indices : Int [3]=valores;
Nombres : String [1+1];
Nombres={"juan", "jose"};


GetMedia : Double (val1 : Int, val2 : Int){
    //Ejemplo de función que devuelve un valor
    Val3 : Double=(val1+val2)/2;
    Return val3;
}
Mostrar : Void (){
    //ejemplo de método
    Print("Hola Mundo");
}
Mostrar : Void (nombre : String){
    //ejemplo de método
    Print("Hola Mundo " + cadena);
}


Main : Void (operacion : String){
    //ejemplo de método
    Print("Hola Mundo " + cadena);  
    If ( var1>5 && True ){
        
            Print("Hola Mundo " + cadena);
    }
    If ( var1==5){
        
            Print("Hola Mundo " + cadena);
    }Else{
        
            Print("Hola Mundo " + cadena);
    }
    While ( var1<10 ){
        Contadores : Int [1+getIndice()]={1,2,3,4,5,6};
        Indices : Int [3]=valores;
        Nombres : String [1+1];
        Nombres={"juan", "jose"};
            Print("Hola Mundo " + cadena);
    }
    Do{
            Print("Hola Mundo " + cadena);
    } While ( var1<10 )

    Switch ( varX ){
        Case 1:
            Print("Hola Mundo " + cadena);
            Nombres={"juan", "jose"};
        Break;
        Case 2:
            Print("Hola Mundo " + cadena);
        Case 3:
            Print("Hola Mundo " + cadena);
        Break;
        Default:
            Print("Hola Mundo " + cadena);
        Break;
    }

    For ( i : Int = 0 ; i<10 ; i++){
        //SENTENCIAS
        Print("Hola Mundo " + cadena);
    }

    If(descripcion.CompareTo("cantidad")){
        jj : Int = 0;
        Print("Existe");
    }Else{
        ppp : Int = 0;
        Print("No Existe");
    }
}
Main : String (operacion : String){
    //ejemplo de método

    metodo();
    metodo1(55,var,true,"cad");
    //Return 1+indices[2]+getVariable(); //En una función
    Return ; //En un metodo
    Mostrar();
    Media : Double = getMedia(1,2);
    Return "ProductoX";
    nombreCompleto:String=GetUser();
    Print(GetUser());
    Return ; //En un metodo

    //HACER FUNCIONAL ESTAS DOS LINEAS
    Return 1+indices[2]+getVariable(); //En una función
    Print ( "indice: "+ var1 +" valor: "+arreglo[var1] );
}





