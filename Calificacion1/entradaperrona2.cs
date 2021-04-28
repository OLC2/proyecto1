Main : String (operacion : String)
{
	VAR : Int = -1+5;
	Print(VAR);
    Mundo(4,1);
	Print("----");
	Metodo1(5,"perrones", 175.523);
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
