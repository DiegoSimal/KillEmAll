using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace KillEmAll
{

    public class CalculadoraDeAtaques
    {
        private static Random generador = new Random();


        public static DanhoRecibido GenerarAtaque(EstadoAventurero atacante, EstadoAventurero defensor,bool intercepcion=false)
        {
            int danhoMinimo = ReglasDelJuego.s_danhoMinimo[atacante.TipoAccion];
            int danhoMaximo = ReglasDelJuego.s_danhoMaximo[atacante.TipoAccion];
            float modificadorPorClase = ReglasDelJuego.s_modificadoresAtaques[atacante.Clase][atacante.TipoAccion];
            CuadrantePercepcion direccionAtaque = CalculadoraGeometrica.CalcularDireccion(defensor.Posicion, atacante.Posicion, defensor.Orientacion);
            float modificadorPorDireccion = ReglasDelJuego.s_modificadoresPorDireccion[atacante.TipoAccion][direccionAtaque];
            float modificadorPorMovimiento = 1;
            if (defensor.SeHaMovido)
            {
                modificadorPorMovimiento = ReglasDelJuego.s_modificadoresAtaquesPorMovimiento[atacante.TipoAccion];
            }
            int danho = (int)Math.Round(generador.Next(danhoMinimo, danhoMaximo + 1) * modificadorPorClase * modificadorPorDireccion * modificadorPorMovimiento);
            bool ataqueMortal = false;
            if (danho >= defensor.Vida)
            {
                ataqueMortal = true;
                danho = defensor.Vida;
            }
            string aclaraciones=null;
            if (intercepcion) aclaraciones = "El ataque no iba dirigido contra él.";
            DanhoRecibido resultado = new DanhoRecibido(danho,atacante.TipoAccion,defensor.Posicion,atacante.Id, atacante.Nombre, defensor.Nombre,atacante.Posicion,defensor.Orientacion,ataqueMortal,aclaraciones);
            return resultado;
        }

        public static EstadoAventurero BuscarAventureroEnTrayectoriaDeAtaque(EstadoAventurero atacante, EstadoAventurero defensor, Dictionary<int, EstadoAventurero> estados, float margen)
        {
            EstadoAventurero resultado = defensor;
            float distancia = CalculadoraGeometrica.CalcularDistancia(atacante.Posicion, defensor.Posicion);
            foreach (KeyValuePair<int,EstadoAventurero> par in estados)
            {
                EstadoAventurero estado = par.Value;
                if (estado!=atacante && estado.EstaVivo() && CalculadoraGeometrica.EstaDentroDeTrayectoria(atacante, defensor, estado, margen))
                {
                    float distanciaAux = CalculadoraGeometrica.CalcularDistancia(atacante.Posicion, estado.Posicion);
                    if (distanciaAux < distancia)
                    {
                        resultado = estado;
                        distancia = distanciaAux;
                    }
                }
            }
            return resultado;
        }

        public static EstadoAventurero BuscarAventureroEnTrayectoriaDeAtaque(EstadoAventurero atacante, Point objetivo, Dictionary<int, EstadoAventurero> estados, float alcance, float margen)
        {
            float distancia = alcance;
            objetivo = CalculadoraGeometrica.CalcularPosicionDeMaximoAlcance(atacante.Posicion, objetivo, (int)alcance);
            EstadoAventurero resultado=null;
            foreach (KeyValuePair<int, EstadoAventurero> par in estados)
            {
                EstadoAventurero estado = par.Value;
                if (estado != atacante && estado.EstaVivo() && CalculadoraGeometrica.EstaDentroDeTrayectoria(estado.Posicion,atacante.Posicion, objetivo, margen))
                {
                    float distanciaAux = CalculadoraGeometrica.CalcularDistancia(atacante.Posicion, estado.Posicion);
                    if (distanciaAux < distancia)
                    {
                        resultado = estado;
                        distancia = distanciaAux;
                    }
                }
            }
            return resultado;
        }

        public static List<EstadoAventurero> BuscarAventurerosEnTrayectoriaDeAtaque(EstadoAventurero atacante, Point destino, Dictionary<int, EstadoAventurero> estados, float alcance, float margen)
        {
            destino = CalculadoraGeometrica.CalcularPosicionDeMaximoAlcance(atacante.Posicion, destino, (int)alcance);
            List<EstadoAventurero> atacados = new List<EstadoAventurero> ();
            foreach(EstadoAventurero estado in estados.Values)
            {
                if (estado!= atacante && estado.EstaVivo() && CalculadoraGeometrica.EstanADistancia(atacante.Posicion,estado.Posicion,alcance) && CalculadoraGeometrica.EstaDentroDeTrayectoria(estado.Posicion, atacante.Posicion, destino, margen)){
                    atacados.Add(estado);
                }
            }
            return atacados;
        }
        
        
 
    }
}
