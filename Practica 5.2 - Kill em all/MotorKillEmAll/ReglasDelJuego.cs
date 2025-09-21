using System;
using System.Collections.Generic;
using System.Text;

namespace KillEmAll
{

    public enum TipoAccion { Percepcion, Movimiento, BolaDeFuego, Arco, CuerpoACuerpo }
    public enum Clase { Guerrero, Arquero, Mago }
    public class ReglasDelJuego
    {

        public const int PX_POR_PIEZA = 50;
        public const int PX_POR_ULTIMO_EN_PIE = 100;
        public const int PX_POR_PV = 1;
        public const int VIDA_GUERRERO = 100;
        public const int VIDA_ARQUERO = 80;
        public const int VIDA_MAGO = 60;
        public const int DISTANCIA_MINIMA_ARCO = 10;
        public const int ALCANCE_ARCO = 90;
        public const int ALCANCE_CUERPO_A_CUERPO = 10;
        public const int ALCANCE_BOLA_DE_FUEGO = 150;
        public const int RADIO_BOLA_DE_FUEGO = 40;
        public const int MOVIMIENTO_MAXIMO = 50;
        public const int MOVIMIENTO_MAXIMO_CON_ATAQUE = 25;
        public const int ANCHO_ARENA = 800;
        public const int ALTO_ARENA = 800;
        //Es la distancia máxima a la que un personaje se puede encontrar de la trayectoria
        //de una flecha para considerar que ha interceptado la flecha y se lleva el daño
        public const float DISTANCIA_MAXIMA_INTERCEPCION = 20;
        public const int DANHO_AUTOMATICO = 10;

        public static Dictionary<TipoAccion, float> s_modificadoresAtaquesPorMovimiento = new Dictionary<TipoAccion, float>()
        {
            {TipoAccion.BolaDeFuego,1 },
            {TipoAccion.Arco,0.75f },
            {TipoAccion.CuerpoACuerpo,0.75f }
        };

        public static Dictionary<Clase, int> s_vidaPorClase = new Dictionary<Clase, int>() {
            { Clase.Guerrero,VIDA_GUERRERO},
            {Clase.Arquero,VIDA_ARQUERO},
            {Clase.Mago,VIDA_MAGO }
        };

        public static Dictionary<Clase, List<TipoAccion>> s_accionesPermitidas = new Dictionary<Clase, List<TipoAccion>>
        {
            {Clase.Guerrero,new List<TipoAccion>(new TipoAccion[]{TipoAccion.CuerpoACuerpo,TipoAccion.Arco,TipoAccion.Movimiento,TipoAccion.Percepcion }) },
            {Clase.Arquero,new List<TipoAccion>(new TipoAccion[]{TipoAccion.CuerpoACuerpo,TipoAccion.Arco,TipoAccion.Movimiento,TipoAccion.Percepcion }) },
            {Clase.Mago,new List<TipoAccion>(new TipoAccion[]{TipoAccion.CuerpoACuerpo,TipoAccion.BolaDeFuego,TipoAccion.Movimiento,TipoAccion.Percepcion }) }
        };

        public static Dictionary<Clase, Dictionary<TipoAccion, float>> s_modificadoresAtaques = new Dictionary<Clase, Dictionary<TipoAccion, float>>() {
            {Clase.Guerrero,new Dictionary<TipoAccion,float>(){
                    {TipoAccion.CuerpoACuerpo,1.2f},
                    {TipoAccion.Arco,1},
                    {TipoAccion.BolaDeFuego,1 }
                }
            },
            {Clase.Arquero,new Dictionary<TipoAccion,float>(){
                    {TipoAccion.CuerpoACuerpo,1},
                    {TipoAccion.Arco,1.2f},
                    {TipoAccion.BolaDeFuego,1 }
                }
            },
            {Clase.Mago,new Dictionary<TipoAccion,float>(){
                    {TipoAccion.CuerpoACuerpo,1},
                    {TipoAccion.Arco,1},
                    {TipoAccion.BolaDeFuego,1 }
                }
            }
        };

        public static Dictionary<TipoAccion, Dictionary<CuadrantePercepcion, float>> s_modificadoresPorDireccion = new Dictionary<TipoAccion, Dictionary<CuadrantePercepcion, float>> {

            {TipoAccion.CuerpoACuerpo, new Dictionary<CuadrantePercepcion, float>() {
                                                    {CuadrantePercepcion.Frente,1 },
                                                    {CuadrantePercepcion.Izquierda,1.15f },
                                                    {CuadrantePercepcion.Derecha,1.15f },
                                                    {CuadrantePercepcion.Detras,1.3f }
                                            }
            },
            {TipoAccion.Arco, new Dictionary<CuadrantePercepcion, float>() {
                                                    {CuadrantePercepcion.Frente,1 },
                                                    {CuadrantePercepcion.Izquierda,1.15f },
                                                    {CuadrantePercepcion.Derecha,1.15f },
                                                    {CuadrantePercepcion.Detras,1.3f }
                                            }
            },
            {TipoAccion.BolaDeFuego, new Dictionary<CuadrantePercepcion, float>() {
                                                    {CuadrantePercepcion.Frente,1 },
                                                    {CuadrantePercepcion.Izquierda,1 },
                                                    {CuadrantePercepcion.Derecha,1 },
                                                    {CuadrantePercepcion.Detras,1 }
                                            }
            }
        };

        public static Dictionary<TipoAccion, int> s_danhoMinimo = new Dictionary<TipoAccion, int>()
        {
            {TipoAccion.CuerpoACuerpo,10 },
            {TipoAccion.Arco,10 },
            {TipoAccion.BolaDeFuego,10 }
        };

        public static Dictionary<TipoAccion, int> s_danhoMaximo = new Dictionary<TipoAccion, int>()
        {
            {TipoAccion.CuerpoACuerpo,15 },
            {TipoAccion.Arco,15 },
            {TipoAccion.BolaDeFuego,15 }
        };

    }
}
