using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace KillEmAll
{
    public class CalculadoraGeometrica
    {
        private static float THRESHOLD = 0.01f;

        //El algoritmo funciona de la siguiente forma:
        //Giramos los ejes para que la dirección del personaje sea 45º
        //Al girar también el vector que va de la posición del personaje a la posición del atacante,
        //este vector caerá en uno de los cuatro cuadrantes. El primer cuadrante es para el frente,
        //el segundo para la izquierda, el tercero para detrás y el cuarto para la derecha
        /*
                 |
                 |
                 |    x (personaje, girado para mirar a 45º)
           o1    |  
        ---------+--------
                 |
                 |  o3
              o2 |
                 |
        */
        
        public static bool APuedeVerAB(EstadoAventurero estadoA, EstadoAventurero estadoB) {
            CuadrantePercepcion cuadrante = CalcularDireccion(estadoA.Posicion, estadoB.Posicion, estadoA.Orientacion);
            return cuadrante == CuadrantePercepcion.Frente;
        }
        
        
        public static CuadrantePercepcion CalcularDireccion(Point posicion1, Point posicion2, int orientacion1) {
            PointF vectorAtacante = new PointF(posicion2.X - posicion1.X, posicion2.Y - posicion1.Y);
            PointF vectorAtacanteGirado = GirarVector(orientacion1, vectorAtacante);
            CuadrantePercepcion direccion = DeterminarCuadrante(vectorAtacanteGirado);
            return direccion;
        }

        private static CuadrantePercepcion DeterminarCuadrante(PointF vectorAtacanteGirado) {
            CuadrantePercepcion direccion = CuadrantePercepcion.Frente;
            if (vectorAtacanteGirado.X < -THRESHOLD) {
                if (vectorAtacanteGirado.Y > 0) {
                    direccion = CuadrantePercepcion.Izquierda;
                }
                else {
                    direccion = CuadrantePercepcion.Detras;
                }
            }
            else if (vectorAtacanteGirado.Y < -THRESHOLD) {
                direccion = CuadrantePercepcion.Derecha;
            }
            return direccion;
        }

        private static PointF GirarVector(int orientacionActual, PointF vectorAtacante) {
            double giro = (45 - orientacionActual) * Math.PI / 180;
            //giramos el vector atacante
            PointF vectorAtacanteGirado = new PointF(
                (float)(vectorAtacante.X * Math.Cos(giro) - vectorAtacante.Y * Math.Sin(giro)),
                (float)(vectorAtacante.Y * Math.Cos(giro) + vectorAtacante.X * Math.Sin(giro))
                );
            return vectorAtacanteGirado;
        }

        public static Point CalcularVectorDiferencia(Point inicio, Point final)
        {
            return new Point(final.X - inicio.X, final.Y - inicio.Y);
        }

        public static bool EstanADistancia(Point puntoA, Point puntoB, float distancia)
        {
            return CalcularDistancia(puntoA, puntoB) <= distancia;
        }

        public static float CalcularDistancia(Point puntoA, Point puntoB)
        {
            return (float)CalcularModulo(CalcularVectorDiferencia(puntoA,puntoB));
        }

        public static double CalcularModulo(Point a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }

        public static Point NormalizarVector(Point vector, float longitudFinal)
        {
            double modulo = CalcularModulo(vector);
            Point resultado = new Point();
            resultado.X = (int)Math.Round(vector.X * longitudFinal / modulo);
            resultado.Y = (int)Math.Round(vector.Y * longitudFinal / modulo);
            return resultado;
        }

        public static Point RecalcularPosicionObjetivo(Point origen, Point final, int distanciaMaxima)
        {
            Point nuevoFinal = final;
            if (!EstanADistancia(origen, final, distanciaMaxima))
            {
                nuevoFinal=CalcularPosicionDeMaximoAlcance(origen, final, distanciaMaxima);
            }

            return nuevoFinal;
        }

        public static Point CalcularPosicionDeMaximoAlcance(Point origen, Point final, int distanciaMaxima)
        {
            Point vector = NormalizarVector(CalcularVectorDiferencia(origen, final), distanciaMaxima);
            return new Point(origen.X + vector.X, origen.Y + vector.Y);
        }

        public static bool EstaDentroDeTrayectoria(EstadoAventurero atacante, EstadoAventurero defensor, EstadoAventurero interceptor, float margen)
        {
            return EstaDentroDeTrayectoria(interceptor.Posicion, atacante.Posicion, defensor.Posicion,margen);
        }

        public static bool EstaDentroDeTrayectoria(Point interceptor, Point origen, Point final, float margen)
        {
            bool estaDentroDeTrayectoria = false;
            float distanciaATrayectoria = DistanciaDeRectaAPunto(origen, final, interceptor);
            if (distanciaATrayectoria <= margen)
            {
                float producto1 = ProductoEscalar(origen, final, origen, interceptor);
                float producto2 = ProductoEscalar(final, origen, final, interceptor);
                if (producto1>=0 && producto2 >= 0)
                {
                    estaDentroDeTrayectoria=true;
                }
            }
            return estaDentroDeTrayectoria;
        }

        private static float ProductoEscalar(Point origen1, Point extremo1, Point origen2, Point extremo2) { 
            Point vector1 = new Point(extremo1.X-origen1.X,extremo1.Y-origen1.Y);
            Point vector2 = new Point(extremo2.X-origen2.X,extremo2.Y-origen2.Y);
            float resultado = vector1.X*vector2.X+ vector1.Y*vector2.Y;
            return resultado;
        }

        private static float DistanciaDeRectaAPunto(Point segmento1,Point segmento2, Point punto)
        {
            float diferenciaY = (segmento2.Y - segmento1.Y);
            float diferenciaX = (segmento2.X - segmento1.X);
            float numerador = Math.Abs(punto.X*diferenciaY-punto.Y*diferenciaX+segmento1.Y*segmento2.X-segmento1.X*segmento2.Y);
            float denominador = (float)Math.Sqrt(diferenciaX * diferenciaX + diferenciaY * diferenciaY);
            return numerador/denominador;
        }

    }
}
