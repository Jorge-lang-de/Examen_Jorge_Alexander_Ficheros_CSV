using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HolaMundo
{
    // ponemos la estructura de cada fila del CSV
    public class Partido
    {
        public int Jornada { get; set; }
        public string EquipoLocal { get; set; }
        public string EquipoVisitante { get; set; }
        public string Resultado { get; set; }
        public int PuntosLocal { get; set; }
        public int PuntosVisitante { get; set; }
        public string Mvp { get; set; }
    }

    // Esto sirve para acumular datos y calcular la clasificación de Estadísticas de cada equipo
    public class EstadisticasEquipo
    {
        public string Nombre { get; set; }
        public int Victorias { get; set; }
        public int Derrotas { get; set; }
        public int PuntosAnotados { get; set; }
        public int PuntosRecibidos { get; set; }

        // Esta es la Propiedad calculada de la diferencia de puntos para desempatar en la tabla
        public int DiferenciaPuntos => PuntosAnotados - PuntosRecibidos;
    }

    class Program
    {
        // Ponemos la ruta del archivo de datos CSV de la ACB
        static string rutaCsv = "acb2526.csv";

        static void Main(string[] args)
        {
            int opcion = 0;
            // Hacemos un bucle para que el menú se repita hasta que el usuario pulse '6' que seria (Salir)
            do
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("       GESTIÓN TEMPORADA ACB 25-26      ");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("1. Ver y exportar resultados de un equipo");
                Console.WriteLine("2. Ver y exportar resultados de una jornada");
                Console.WriteLine("3. Ver Top 3 MVPs y exportar");
                Console.WriteLine("4. Ver Clasificación de la liga");
                Console.WriteLine("5. Generar ficheros .txt individuales por equipo");
                Console.WriteLine("6. Salir");
                Console.WriteLine("-------------------------------------------");
                Console.Write("Elige una opción (1-6): ");
                // Validamos para que el usuario introduzca un número entero correctamente
                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    // Hacemos la estructura switch para derivar la ejecución según la opción que vayamos a elegir
                    switch (opcion)
                    {
                        case 1:
                            ConsultarResultadosEquipo();
                            break;
                        case 2:
                            ConsultarResultadosJornada();
                            break;
                        case 3:
                            ConsultarTopMvps();
                            break;
                        case 4:
                            MostrarClasificacion();
                            break;
                        case 5:
                            GenerarFicherosPorEquipo();
                            break;
                        case 6:
                            Console.WriteLine("\n¡Saliendo del programa.");
                            break;
                        default:
                            Console.WriteLine("\nOpción incorrecta. Elige un número del 1 al 6.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPor favor, introduce un número válido.");
                }

                if (opcion != 6)
                {
                    Console.WriteLine("\nPulsa cualquier tecla para continuar...");
                    Console.ReadKey();
                }

            } while (opcion != 6);
        }

        
        
        // esta es la función que Lee el CSV. Devuelve la lista con todos los datos cargados.
        static List<Partido> CargarPartidos()
        {
            // aqui creamos una lista vacía para ir guardando los partidos del CSV
            List<Partido> listaPartidos = new List<Partido>();

            if (!File.Exists(rutaCsv))
            {
                Console.WriteLine($"[Error] No encuentro el archivo '{rutaCsv}' en la carpeta.");
                return listaPartidos;
            }
            // Leemos todas las líneas del fichero de texto plano
            string[] lineas = File.ReadAllLines(rutaCsv);

            // Empezamos en i = 1 para saltarnos la primera línea (la cabecera del CSV)
            for (int i = 1; i < lineas.Length; i++)
            {
                // Obtenemos la línea actual y comprobamos que no esté vacía
                string linea = lineas[i];
                if (string.IsNullOrWhiteSpace(linea)) continue;

                // Separamos los campos de la línea utilizando la coma como delimitador
                string[] datos = linea.Split(',');

                // Validamos que tenga al menos las 9 columnas esperadas
                if (datos.Length >= 9)
                {
                    try
                    {
                        // Extrae el número de la jornada (ejem. "Jornada 1" pasa a ser el número entero 1)
                        string jornadaTexto = datos[0].Replace("Jornada", "").Trim();
                        int numJornada = int.Parse(jornadaTexto);

                        // esto es para separar el resultado (ej. "79-69") en dos números enteros: puntos del local y puntos del visitante
                        string[] partesResultado = datos[3].Split('-');
                        int pLocal = int.Parse(partesResultado[0].Trim());
                        int pVisitante = int.Parse(partesResultado[1].Trim());

                        // Creamos un objeto llamado partido con los datos limpios y lo añade a la lista general en orden de aparición en el CSV
                        Partido p = new Partido
                        {
                            Jornada = numJornada,
                            EquipoLocal = datos[1].Trim(),
                            EquipoVisitante = datos[2].Trim(),
                            Resultado = datos[3].Trim(),
                            PuntosLocal = pLocal,
                            PuntosVisitante = pVisitante,
                            Mvp = datos[4].Trim() // El MVP saldra en la columna 4
                        };
                        listaPartidos.Add(p);
                    }
                    catch
                    {
                        // Ignoramos la línea si hay errores puntuales
                    }
                }
            }

            return listaPartidos;
        }
        // hacemos el metodo para consultar y mostrar los partidos de un equipo específico
        static void ConsultarResultadosEquipo()
        {
            // Cargamos la lista de partidos desde el CSV
            List<Partido> partidos = CargarPartidos();
            if (partidos.Count == 0) return;

            Console.Write("\nEscribe el nombre del equipo (ej. Bilbao Basket): ");
            string equipoBuscado = Console.ReadLine().Trim();

            // Filtramos los partidos usando un bucle foreach
            List<Partido> resultados = new List<Partido>();
            foreach (var p in partidos)
            {
                // Comprobamos si el equipo buscado coincide con el equipo local o visitante, ignorando mayúsculas y minúsculas
                if (p.EquipoLocal.Equals(equipoBuscado, StringComparison.OrdinalIgnoreCase) || 
                    p.EquipoVisitante.Equals(equipoBuscado, StringComparison.OrdinalIgnoreCase))
                {
                    
                    resultados.Add(p);
                }
            }

            if (resultados.Count == 0)
            {
                Console.WriteLine($"\nNo se han encontrado partidos para '{equipoBuscado}'.");
                return;
            }

            // Aqui mostramos los resultados en la consola y preparamos las líneas para guardar en un archivo de texto
            Console.WriteLine($"\n--- PARTIDOS DE {equipoBuscado.ToUpper()} ---");
            List<string> lineasParaGuardar = new List<string> { $"Resultados del equipo: {equipoBuscado}" };

            // Recorremos los partidos encontrados y los mostramos en la consola
            foreach (var p in resultados)
            {
                string texto = $"Jornada {p.Jornada}: {p.EquipoLocal} {p.Resultado} {p.EquipoVisitante} [MVP: {p.Mvp}]";
                Console.WriteLine(texto);
                lineasParaGuardar.Add(texto);
            }
            // Esta seria la opción para guardar los resultados en un archivo de texto
            GuardarEnFicheroOpcional(lineasParaGuardar);
        }



        // hacemos el metodo para consultar y mostrar los partidos de una jornada específica
        static void ConsultarResultadosJornada()
        {
            // Cargamos la lista de partidos desde el CSV
            List<Partido> partidos = CargarPartidos();
            if (partidos.Count == 0) return;

            // Pedimos al usuario que introduzca el número de la jornada que desea consultar
            Console.Write("\nIntroduce el número de la jornada: ");

            // Validamos que el usuario haya introducido un número entero válido
            if (int.TryParse(Console.ReadLine(), out int numJornada))
            {
                var resultados = partidos.Where(p => p.Jornada == numJornada).ToList();

                if (resultados.Count == 0)
                {
                    Console.WriteLine($"\nNo hay partidos en la jornada {numJornada}.");
                    return;
                }

                // Mostramos los resultados de la jornada en la consola y preparamos las líneas para guardar en un archivo de texto
                Console.WriteLine($"\n--- JORNADA {numJornada} ---");
                List<string> lineasParaGuardar = new List<string> { $"Partidos de la Jornada {numJornada}" };

                // Recorremos los partidos encontrados y los mostramos en la consola
                foreach (var p in resultados)
                {
                    string texto = $"{p.EquipoLocal} {p.Resultado} {p.EquipoVisitante} (MVP: {p.Mvp})";
                    Console.WriteLine(texto);
                    lineasParaGuardar.Add(texto);
                }
                // Esta seria la opción para guardar los resultados en un archivo de texto
                GuardarEnFicheroOpcional(lineasParaGuardar);
            }
            else
            {
                Console.WriteLine("\nHas escrito un número de jornada no válido.");
            }
        }




        // hacemos el metodo para consultar y mostrar los 3 jugadores que más veces han sido MVP
        static void ConsultarTopMvps()
        {
            // Cargamos la lista de partidos desde el CSV
            List<Partido> partidos = CargarPartidos();
            if (partidos.Count == 0) return;

            // Agrupamos los partidos por el nombre del MVP y contamos cuántas veces aparece cada jugador
            var topMvps = partidos
                .GroupBy(p => p.Mvp)
                .Select(g => new { Jugador = g.Key, TotalVeces = g.Count() })
                .OrderByDescending(x => x.TotalVeces)
                .Take(3)
                .ToList();

            // Mostramos los resultados en la consola y preparamos las líneas para guardar en un archivo de texto
            Console.WriteLine("\n--- TOP 3 JUGADORES MÁS VECES MVP ---");
            List<string> lineasParaGuardar = new List<string> { "Listado Top 3 MVPs" };
            // Recorremos los jugadores y los mostramos en la consola
            int posicion = 1;
            foreach (var item in topMvps)
            {
                string texto = $"{posicion}. {item.Jugador} con {item.TotalVeces} MVP(s)";
                Console.WriteLine(texto);
                lineasParaGuardar.Add(texto);
                posicion++;
            }
            // Esta seria la opción para guardar los resultados en un archivo de texto
            GuardarEnFicheroOpcional(lineasParaGuardar);
        }



        // hacemos el metodo para mostrar la clasificación de la liga según los partidos jugados
        static void MostrarClasificacion()
        {
            // Cargamos la lista de partidos desde el CSV
            List<Partido> partidos = CargarPartidos();
            if (partidos.Count == 0) return;
            // Obtenemos la lista de equipos únicos a partir de los partidos
            var listaEquipos = partidos.Select(p => p.EquipoLocal)
                                    .Union(partidos.Select(p => p.EquipoVisitante))
                                    .Distinct()
                                    .ToList();
            // Creamos una lista para almacenar las estadísticas de cada equipo
            List<EstadisticasEquipo> tablaClasificacion = new List<EstadisticasEquipo>();

            // Recorremos cada equipo y calculamos sus estadísticas a partir de los partidos jugados
            foreach (string eq in listaEquipos)
            {
                int ganados = 0;
                int perdidos = 0;
                int anotados = 0;
                int recibidos = 0;

                // Recorremos todos los partidos y acumulamos las estadísticas del equipo actual
                foreach (var p in partidos)
                {

                    // Comprobamos si el equipo es local o visitante y actualizamos las estadísticas en consecuencia
                    if (p.EquipoLocal.Equals(eq, StringComparison.OrdinalIgnoreCase))
                    {
                        // Si el equipo es local, sumamos los puntos anotados y recibidos, y contamos victorias y derrotas
                        anotados += p.PuntosLocal;
                        recibidos += p.PuntosVisitante;
                        // Comprobamos quién ganó el partido y actualizamos los contadores de victorias y derrotas
                        if (p.PuntosLocal > p.PuntosVisitante) ganados++;
                        // Si el equipo local perdió, incrementamos el contador de derrotas
                        else if (p.PuntosLocal < p.PuntosVisitante) perdidos++;
                    }
                    else if (p.EquipoVisitante.Equals(eq, StringComparison.OrdinalIgnoreCase))
                    {
                        anotados += p.PuntosVisitante;
                        recibidos += p.PuntosLocal;
                        if (p.PuntosVisitante > p.PuntosLocal) ganados++;
                        else if (p.PuntosVisitante < p.PuntosLocal) perdidos++;
                    }
                }

                // Creamos un objeto EstadisticasEquipo con los datos calculados y lo añadimos a la lista de clasificación
                tablaClasificacion.Add(new EstadisticasEquipo
                {
                    Nombre = eq,
                    Victorias = ganados,
                    Derrotas = perdidos,
                    PuntosAnotados = anotados,
                    PuntosRecibidos = recibidos
                });
            }
            // Ordenamos la tabla de clasificación primero por victorias y luego por diferencia de puntos
            var ordenada = tablaClasificacion
                .OrderByDescending(e => e.Victorias)
                .ThenByDescending(e => e.DiferenciaPuntos)
                .ToList();

            Console.WriteLine("");
            Console.WriteLine("             CLASIFICACIÓN OFICIAL                ");
            Console.WriteLine("");
            Console.WriteLine($"{"Pos",-3} {"Equipo",-35} {"G",-3} {"P",-3} {"Dif",-5}");
            Console.WriteLine("--------------------------------------------------");

            // Recorremos la lista ordenada y mostramos la clasificación en la consola
            int pos = 1;
            foreach (var e in ordenada)
            {
                Console.WriteLine($"{pos,-3} {e.Nombre,-35} {e.Victorias,-3} {e.Derrotas,-3} {e.DiferenciaPuntos,-5}");
                pos++;
            }
            Console.WriteLine("==================================================");
        }



        // hacemos el metodo para generar ficheros de texto individuales por equipo con su historial de partidos
        static void GenerarFicherosPorEquipo()
        {
            // Cargamos la lista de partidos desde el CSV
            List<Partido> partidos = CargarPartidos();
            if (partidos.Count == 0) return;

            // Creamos la carpeta de salida asegurando una ruta local limpia
            string carpetaSalida = "FicherosEquipos";
            Directory.CreateDirectory(carpetaSalida);

            // Creamos una lista para almacenar los nombres de los equipos únicos
            List<string> listaEquipos = new List<string>();
            foreach (var p in partidos)
            {
                // Obtenemos los nombres de los equipos local y visitante, eliminando espacios en blanco
                string local = p.EquipoLocal?.Trim();
                string visitante = p.EquipoVisitante?.Trim();

                // Comprobamos si el equipo local ya está en la lista y lo añadimos si no está presente
                if (!string.IsNullOrEmpty(local))
                {
                    bool existeLocal = false;
                    foreach (string eq in listaEquipos)
                    {
                        if (eq.Equals(local, StringComparison.OrdinalIgnoreCase)) { existeLocal = true; break; }
                    }
                    if (!existeLocal) listaEquipos.Add(local);
                }
                
                if (!string.IsNullOrEmpty(visitante))
                {
                    bool existeVisitante = false;
                    foreach (string eq in listaEquipos)
                    {
                        if (eq.Equals(visitante, StringComparison.OrdinalIgnoreCase)) { existeVisitante = true; break; }
                    }
                    if (!existeVisitante) listaEquipos.Add(visitante);
                }
            }
            // Recorremos la lista de equipos y generamos un archivo de texto para cada uno con su historial de partidos
            foreach (string eq in listaEquipos)
            {
                // Filtramos los partidos del equipo actual y los almacenamos en una lista
                List<Partido> misPartidos = new List<Partido>();
                foreach (var p in partidos)
                {
                    // Comprobamos si el equipo actual es el local o visitante en cada partido y lo añadimos a la lista de partidos del equipo
                    if ((p.EquipoLocal != null && p.EquipoLocal.Trim().Equals(eq, StringComparison.OrdinalIgnoreCase)) || 
                        (p.EquipoVisitante != null && p.EquipoVisitante.Trim().Equals(eq, StringComparison.OrdinalIgnoreCase)))
                    {
                        misPartidos.Add(p);
                    }
                }

                // Generamos un nombre de archivo limpio eliminando caracteres inválidos para nombres de archivo
                string nombreLimpio = "";
                char[] caracteresInvalidos = Path.GetInvalidFileNameChars();
                foreach (char c in eq)
                {
                    bool esInvalido = false;
                    foreach (char invalido in caracteresInvalidos)
                    {
                        if (c == invalido)
                        {
                            esInvalido = true;
                            break;
                        }
                    }
                    if (!esInvalido)
                    {
                        nombreLimpio += c;
                    }
                }
                // Añadimos la extensión .txt al nombre del archivo y combinamos con la ruta de salida
                nombreLimpio = nombreLimpio.Trim() + ".txt";
                string rutaCompleta = Path.Combine(carpetaSalida, nombreLimpio);
                // Creamos una lista de cadenas para almacenar el contenido del archivo de texto
                List<string> contenidoFichero = new List<string> { $"Historial de partidos de: {eq}" };
                foreach (var p in misPartidos)
                {
                    contenidoFichero.Add($"Jornada {p.Jornada}: {p.EquipoLocal} {p.Resultado} {p.EquipoVisitante} (MVP: {p.Mvp})");
                }
                // Escribimos todas las líneas en el archivo de texto correspondiente al equipo
                File.WriteAllLines(rutaCompleta, contenidoFichero);
            }

            Console.WriteLine($"\n¡Listo! Ficheros generados correctamente en la carpeta '{carpetaSalida}' para los {listaEquipos.Count} equipos.");
        }


        // Hacemos el método para guardar los resultados en un fichero de texto si el usuario lo desea
        static void GuardarEnFicheroOpcional(List<string> lineas)
        {
            Console.Write("\n¿Quieres guardar estos resultados en un fichero de texto? (s/n): ");
            string respuesta = Console.ReadLine()?.Trim().ToLower();

            // Validamos la respuesta del usuario para decidir si guardar o no el archivo
            if (respuesta == "s" || respuesta == "si")
            {
                Console.Write("Escribe el nombre del archivo (ej. consulta.txt): ");
                string nombreArchivo = Console.ReadLine()?.Trim();
                
                // Validamos que el nombre del archivo no esté vacío y le añadimos la extensión .txt si no la tiene
                if (!string.IsNullOrEmpty(nombreArchivo))
                {
                    if (!nombreArchivo.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        nombreArchivo += ".txt";
                    }
                    
                    // Escribimos todas las líneas en el archivo de texto especificado por el usuario
                    File.WriteAllLines(nombreArchivo, lineas);
                    Console.WriteLine($"¡Guardado con éxito en '{nombreArchivo}'!");
                }
                else
                {
                    Console.WriteLine("Nombre no válido. No se ha guardado nada.");
                }
            }
        }
    }
}
