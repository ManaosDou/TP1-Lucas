//Advertencia: Al crear la habitacion se recomienda que la cantidad de filas y columnas sea la misma para evitar posibles bugs
public class Program //Llamar al juego luego de preguntar filas y columnas
    {
        public static int Hcolumnas = 10;
        public static int Hfilas = 10;
        public static void Main(string[] args)
    {
        Correr();
    }
        public static void Correr()
        {
        // Solicitar al usuario el número de columnas
        //do
        //{
            //Console.Write("Ingresa el número de columnas (debe ser mayor a 5): ");
            //Hcolumnas = Convert.ToInt32(Console.ReadLine());
        //} while (Hcolumnas <= 5);

        // Solicitar al usuario el número de filas
        //do
        //{
            //Console.Write("Ingresa el número de filas (debe ser mayor a 5): ");
            //Hfilas = Convert.ToInt32(Console.ReadLine());
        //} while (Hfilas <= 5);

            var juego = new Juego();
            juego.CorrerJuego();
        }
        public static void Reiniciar()
    {
        Console.Clear(); 
        Correr(); 
    }
    }
    

    class Juego
    {
        int frame;
        Jugador jugador= null!;
        Habitacion habitacion= null!;
        Random rand= null!;
        List<Bala> balas= null!;
        List<Enemigo> enemigos = null!;
        
        public void CorrerJuego() //Gameloop
        {
            // Inicializacion
            Inicializacion();

            // Dibujo el primer cuadro
            DibujarPantalla();

            // Game Loop!
            while (true)
            {
                // Escucho Input
                ConsoleKeyInfo? input = null;
                if (Console.KeyAvailable)
                    input = Console.ReadKey();

                // Actualizo Datos
                ActualizarDatos(input);
             
                // Dibujo Pantalla
                DibujarPantalla();

                Thread.Sleep(100);
            }
        }

        void Inicializacion() //Creacion
        {
            frame = 0;
            habitacion = new Habitacion(Program.Hfilas, Program.Hcolumnas); 
            jugador = new Jugador((Program.Hfilas / 2), (Program.Hcolumnas - 2), habitacion, '^', balas);  
            rand = new Random();
            balas = new List<Bala>();
            
            enemigos = new List<Enemigo>(); 
            int cantidadEnemigos = (Program.Hcolumnas * Program.Hfilas / 10); // Generación de enemigos a partir del tamaño de la habitación
            int contadorEnemigos = 0;
            while (contadorEnemigos < cantidadEnemigos)
            {
             int min_x = 2; 
             int max_x = Program.Hcolumnas - 2; 

             int min_y = 2; 
             int max_y = Program.Hfilas - 3; 

             int x = rand.Next(min_x, max_x);
             int y = rand.Next(min_y, max_y);

             if (habitacion.EstaLibre(x, y) && !habitacion.EstaOcupado(x, y, enemigos))
             {
             enemigos.Add(new Enemigo(x, y, habitacion, '='));
             contadorEnemigos++;
             }
            }
    
        }

        void ActualizarDatos(ConsoleKeyInfo? input)
        {
            // Actualizo el frame
            frame++;

            if (input.HasValue)
            {
                var tecla = input.Value.Key;

                // Muevo al jugador
                if (tecla == ConsoleKey.RightArrow)
                    jugador.Mover(1, 0);
                if (tecla == ConsoleKey.LeftArrow)
                    jugador.Mover(-1, 0);
                if (tecla == ConsoleKey.UpArrow)
                    jugador.Mover(0, -1);
                if (tecla == ConsoleKey.DownArrow)
                    jugador.Mover(0, 1);
        
                if (tecla == ConsoleKey.Spacebar) // Dispara al presionar la barra espaciadora
            {
                var NuevaBala = jugador.Disparar();
                if (NuevaBala != null)
                    balas.Add(NuevaBala);
            }
            }

            foreach (var bala in balas) //Movimiento de las balas
            {
                bala.Mover(0, -1);
            }

            for (int i = balas.Count - 1; i >= 0; i--)
            {
               var bala = balas[i];

        
             if (bala.y == 1) // Eliminar la bala al llegar a la pared
             {
             balas.RemoveAt(i);
             }
            else // Eliminar la bala y el enemigo al colisionar
            {
             bool balaEliminada = false;
             foreach (var enemigo in enemigos)
             {
             if (bala.x == enemigo.x && bala.y == enemigo.y)
             {
                balas.RemoveAt(i);
                enemigos.Remove(enemigo);
                balaEliminada = true;
                break;
             }
             }
            if (balaEliminada)
             break;
            }
            }

            if (enemigos.Count == 0) //Reiniciar juego
            {
                Console.Clear();
                Program.Reiniciar();
            }
        }

        void DibujarPantalla() //Crear lienzo y dibujar los objetos
        {
            Lienzo lienzo = new Lienzo(Program.Hfilas, Program.Hcolumnas); 
            habitacion.Dibujar(lienzo); 
            jugador.Dibujar(lienzo);

            foreach (var enemigo in enemigos)
        {
            enemigo.Dibujar(lienzo);
        }
            
            foreach (var bala in balas)
        {
            bala.Dibujar(lienzo);
        }
        
            lienzo.MostrarEnPantalla();
    }
    }

abstract class Movible
{
    public int x, y;
    protected IMapa mapa;

    public Movible(int x, int y, IMapa mapa)
    {
        this.x = x;
        this.y = y;
        this.mapa = mapa;
    }

    public abstract void Mover(int deltaX, int deltaY);
}
class Jugador : Movible
{
    private char dibujo;
    private List<Bala> balas;

    public Jugador(int x, int y, IMapa mapa, char dibujo, List<Bala> balas) : base(x, y, mapa)
    {
        this.dibujo = dibujo;
        this.balas = balas;
    }
public void Dibujar(Lienzo lienzo)
    {
        lienzo.Dibujar(x, y, dibujo);
    }

    public Bala? Disparar()
    {
        
        if (mapa.EstaLibre(x, y))
        {

            return new Bala(x, y, mapa, balas); // Devuelve una nueva bala si el espacio está libre
        }
        else
        {
            return null;
        }
    }

    public override void Mover(int deltaX, int deltaY)
    {
        if (mapa.EstaLibre(x + deltaX, y + deltaY))
        {
            x += deltaX;
            y += deltaY;
        }
    }
}
    
 class Bala : Movible
{
    private char dibujo;
    private List<Bala> balas;

    public Bala(int x, int y, IMapa mapa, List<Bala> balas) : base(x, y, mapa)
    {
        this.dibujo = '|';
        this.balas = balas;
    }

    public void Dibujar(Lienzo lienzo)
    {
        lienzo.Dibujar(x, y, dibujo);
    }

    public override void Mover(int deltaX, int deltaY)
    {
        if (mapa.EstaLibre(x + deltaX, y + deltaY))
        {
            x += deltaX;
            y += deltaY;
        }
        
    }
}
class Enemigo : Movible
{
    private char dibujo;

    public Enemigo(int x, int y, IMapa mapa, char dibujo) : base(x, y, mapa)
    {
        this.dibujo = dibujo;
    }

    public void Dibujar(Lienzo lienzo)
    {
        lienzo.Dibujar(x, y, dibujo);
    }

    public override void Mover(int deltaX, int deltaY)
    {
        
    }
}
    class Lienzo
    {
        private char[,] celdas;
        private int ancho, alto;

        public Lienzo(int ancho, int alto)
        {
            this.ancho = ancho;
            this.alto = alto;
            celdas = new char[ancho, alto];
        }

        public void Dibujar(int x, int y, char celda)
        {
            celdas[x, y] = celda;
        }

        public void MostrarEnPantalla()
        {
            // Limpio la consola antes de dibujar el nuevo cuadro
            Console.Clear();

            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    Console.Write(celdas[x, y]);
                }
                Console.Write("\n");
            }
        }
    }

    interface IMapa
    {
        bool EstaLibre(int x, int y);
        bool EstaOcupado(int x, int y, List<Enemigo> enemigos);
    }

    class Habitacion : IMapa
    {
        private List<Fila> filas;
        
        public Habitacion(int ancho, int alto)
        {
            // Inicializo filas
            filas = new List<Fila>();

            filas.Add(new FPared(ancho));
            for (int fila = 1; fila < alto - 1; fila++)
            {
                filas.Add(new FLibre(ancho));
            }
            filas.Add(new FPared(ancho));
        }

        public void Dibujar(Lienzo lienzo)
        {
            for (int y = 0; y < filas.Count(); y++)
            {
                filas[y].Dibujar(lienzo, y);
            }
        }

        public bool EstaLibre(int x, int y)
        {
            return filas[y].EstaLibre(x);
        }
         public bool EstaOcupado(int x, int y, List<Enemigo> enemigos)
        {
         foreach (var enemigo in enemigos)
         {
            if (enemigo.x == x && enemigo.y == y)
            {
                return true;
            }
         }
        return false;
       }
    }

    abstract class Fila
    {
        protected List<char> celdas;

        public Fila(int cantidadCeldas)
        {
            this.celdas = new List<char>();

            AgregarPunta();
            for (int i = 1; i < cantidadCeldas - 1; i++)
            {
                AgregarMedio();
            }
            AgregarPunta();
        }

        protected abstract void AgregarMedio();
        protected abstract void AgregarPunta();

        public void Dibujar(Lienzo lienzo, int y)
        {
            for (int x = 0; x < celdas.Count(); x++)
            {
                lienzo.Dibujar(x, y, celdas[x]);
            }
        }

        internal bool EstaLibre(int x)
        {
            return celdas[x] == '.';
        }
    }

    class FLibre : Fila
    {
        public FLibre(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add('.');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }

    class FPared : Fila
    {
        public FPared(int cantidadCeldas) : base(cantidadCeldas)
        {
        }

        protected override void AgregarMedio()
        {
            celdas.Add('#');
        }
        protected override void AgregarPunta()
        {
            celdas.Add('#');
        }
    }