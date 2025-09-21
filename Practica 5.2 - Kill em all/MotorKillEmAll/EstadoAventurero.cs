using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;

namespace KillEmAll
{
    public class EstadoAventurero:IComparable<EstadoAventurero>
    {
        private delegate int DelegadoFuncionComparacion(double a, double b);

        private int _id;
        private string _nombre;
        private int _vida;
        private int _puntuacion;
        private int _orientacion;
        private Point _posicion;
        private Accion _accion;
        private Clase _clase;
        private bool _seHaMovido;
        private bool _accionRealizada;
        private string _log;
        private int _puntuacionPorPV;
        private int _puntuacionPorPiezas;

        public DelegadoAccionARealizar accionARealizar;

        public int Id {
            get {
                return _id;
            }
        }

        public string Nombre {
            get {
                return _nombre;
            }
        }

        public int Vida {
            get {
                return _vida;
            }
        }

        public int Orientacion {
            get {
                return _orientacion;
            }
            set {
                _orientacion = value;
            }
        }

        public int Puntuacion {
            get {
                return _puntuacion;
            }
        }

        public Point Posicion {
            get {
                return _posicion;
            }
        }

        public Point? PosicionObjetivo
        {
            get
            {
                return _accion.PosicionObjetivo;
            }
        }

        public Accion Accion {
            set {
                _accion = value;
            }
        }

        public TipoAccion TipoAccion {
            get {
                return _accion.TipoAccion;
            }
            set {
                _accion.TipoAccion = value;
            }
        }

        public Clase Clase
        {
            get
            {
                return _clase;
            }
        }

        public double DistanciaAObjetivo {
            get {
                return CalcularDistancia(_posicion, _accion.PosicionObjetivo);
            }
        }

        public int? IdObjetivo {
            get {
                return _accion.IdObjetivo;
            }
            set
            {
                _accion.IdObjetivo = value;
            }
        }

        public double DistanciaARecorrerEnAtaque {
            get {
                double distancia = 0;
                if (_accion.TipoAccion == TipoAccion.CuerpoACuerpo) {
                    distancia = Math.Min(DistanciaAObjetivo, ReglasDelJuego.MOVIMIENTO_MAXIMO_CON_ATAQUE);
                }
                return distancia;
            }
        }

        public bool SeHaMovido
        {
            get
            {
                return _seHaMovido;
            }
            set
            {
                _seHaMovido = value;
            }
        }

        public bool AccionRealizada
        {
            get
            {
                return _accionRealizada;
            }
            set
            {
                _accionRealizada = value;
            }
        }

        public Situacion Situacion {
            get {
                return new Situacion(_id, _nombre, _orientacion, _posicion, _vida, _clase);
            }
        }

        public string Log
        {
            get
            {
                return _log;
            }
        }

        public int PuntuacionPorPV
        {
            get
            {
                return _puntuacionPorPV;
            }
        }

        public int PuntuacionPorPiezas
        {
            get
            {
                return _puntuacionPorPiezas;
            }
        }


        public EstadoAventurero(int id, string nombre, int vida, Point posicion, Clase clase) {
            _id = id;
            _nombre = nombre;
            _vida = vida;
            _posicion = posicion;
            _puntuacion = 0;
            _orientacion = new Random().Next(0, 360);
            _accion = new Accion();
            accionARealizar = null;
            SeHaMovido = false;
            _clase = clase;
        }

        public void AplicarDanho(int danho) {
            _vida -= danho;
        }

        public void AplicarDanho(DanhoRecibido danhoAplicado)
        {
            AplicarDanho(danhoAplicado.Danho);
        }

        private void SumarPuntuacion(int puntos) {
            _log += $"Añadidos {puntos}\n";
            _puntuacion += puntos;
        }

        public void SumarPuntuacion(DanhoRecibido danhoAplicado)
        {
            SumarPuntuacion(danhoAplicado.Danho*ReglasDelJuego.PX_POR_PV);
            _puntuacionPorPV += danhoAplicado.Danho * ReglasDelJuego.PX_POR_PV;
            if (danhoAplicado.EsDanhoMortal)
            {
                _log += $"Pieza abatida\t";
                SumarPuntuacion(ReglasDelJuego.PX_POR_PIEZA);
                _puntuacionPorPiezas += ReglasDelJuego.PX_POR_PIEZA;
            }
        }

        public void SumarPuntuacionPorSerElUltimoEnPie()
        {
            SumarPuntuacion(ReglasDelJuego.PX_POR_ULTIMO_EN_PIE);
        }

        public bool EstaVivo() {
            return _vida > 0;
        }

        public double MoverAObjetivo(int distanciaMaxima) {
            float distanciaAObjetivo = CalculadoraGeometrica.CalcularDistancia(Posicion, (Point)PosicionObjetivo);
            if (distanciaAObjetivo - ReglasDelJuego.ALCANCE_CUERPO_A_CUERPO < distanciaMaxima)
            {
                distanciaMaxima = Math.Max(0,(int)distanciaAObjetivo - ReglasDelJuego.ALCANCE_CUERPO_A_CUERPO);
            }
            Point vectorMovimiento = ToparMovimiento((Point)PosicionObjetivo, distanciaMaxima);
            Point vectorDireccion = ToparMovimiento((Point)PosicionObjetivo, 10);
            RecalcularOrientacion(vectorDireccion);
            _posicion.X += vectorMovimiento.X;
            _posicion.Y += vectorMovimiento.Y;
            SeHaMovido = CalculadoraGeometrica.CalcularModulo(vectorMovimiento) != 0;
            EncerrarPosicionEnArena();
            double distanciaRecorrida= CalculadoraGeometrica.CalcularModulo(vectorMovimiento);
            if (distanciaRecorrida > 0)
            {
                SeHaMovido = true;
            }
            return distanciaRecorrida;
        }

        public void OrientarHaciaObjetivo() {
            Point vectorOrientacion = new Point(_accion.PosicionObjetivo.Value.X - Posicion.X, _accion.PosicionObjetivo.Value.Y - Posicion.Y);
            RecalcularOrientacion(vectorOrientacion);
        }

        private Point ToparMovimiento(Point nuevaPosicion, int distanciaMaxima) {
            Point diferencia = CalculadoraGeometrica.CalcularVectorDiferencia(_posicion,nuevaPosicion);
            double distancia = CalculadoraGeometrica.CalcularModulo(diferencia);
            if (distancia > distanciaMaxima) {
                diferencia = CalculadoraGeometrica.NormalizarVector(diferencia, distanciaMaxima);
            }
            return diferencia;
        }

        private void RecalcularOrientacion(Point direccion) {
            _orientacion = (int)Math.Round(Math.Atan2(direccion.Y, direccion.X)*180/Math.PI);
            if (_orientacion < 0) _orientacion += 360;
        }

        private double CalcularDistancia(Point a, Point? b) {
            double modulo = 0;
            if (b != null) {
                modulo = CalculadoraGeometrica.CalcularDistancia(a, (Point)b);
            }
            return Math.Round(modulo);
        }

        public void RefrescarPosicionObjetivo(Point nuevaPosicion, int id) {
            if (_accion.IdObjetivo == id) {
                RefrescarPosicionObjetivo(nuevaPosicion);
            }
        }

        public void RefrescarPosicionObjetivo(Point nuevaPosicion) {
            _accion.PosicionObjetivo = nuevaPosicion;
        }

        public bool PuedeVerA(EstadoAventurero otro) {
            return CalculadoraGeometrica.APuedeVerAB(this, otro);
        }

        public override string ToString()
        {
            return $"{_nombre} ({_id}): vida: {_vida} / posición: {_posicion} / {_puntuacion} PX\n\tHa realizado {_accion.TipoAccion}";
        }

        #region Funciones de comparación para ordenar los estados de los aventureros

        public int CompareTo(EstadoAventurero otro) {
            int resultado = 0;
            if (_accion.TipoAccion < otro._accion.TipoAccion) {
                resultado = 1;
            }
            else if (_accion.TipoAccion > otro._accion.TipoAccion) {
                resultado = -1;
            }
            else {
                resultado = CompararOtrasAcciones(otro);
            }

            return resultado;
        }

        private int CompararOtrasAcciones(EstadoAventurero otro) {
            int resultado;
            //Aquí todas las opciones para cuando la accion es la misma para ambos
            switch (_accion.TipoAccion) {
                case TipoAccion.Percepcion:
                case TipoAccion.Movimiento:
                    //El orden en la percepción y en el movimiento son irrelevantes
                    resultado = MenorPrimeroComparar(Id, otro.Id);
                    break;
                case TipoAccion.BolaDeFuego:
                case TipoAccion.Arco:
                case TipoAccion.CuerpoACuerpo:
                    //Tanto para arco como para bola de fuego el orden no depende del movimiento
                    //porque no puede haber movimiento en esas acciones
                    resultado = CompararCriteriosParaOrdenDeAtaque(otro);
                    break;
                default:
                    resultado = 0;
                    break;
            }

            return resultado;
        }

        private int CompararCriteriosParaOrdenDeAtaque(EstadoAventurero otro) {
            double[] campos1 = new double[] { DistanciaAObjetivo, Vida, Puntuacion, Id };
            double[] campos2 = new double[] { otro.DistanciaAObjetivo, otro.Vida, otro.Puntuacion, otro.Id };
            DelegadoFuncionComparacion[] comparadores = new DelegadoFuncionComparacion[] { MenorPrimeroComparar,MayorPrimeroComparar,MenorPrimeroComparar,MenorPrimeroComparar };
            int i = 0;
            int resultado = 0;
            while (resultado == 0 && i < campos1.Length) {
                resultado = comparadores[i](campos1[i], campos2[i]);
                ++i;
            }
            return resultado;
        }

        private int MenorPrimeroComparar(double a, double b) {
            int resultado = 0;
            if (a < b) {
                resultado = 1;
            }
            else if (a > b) {
                resultado = -1;
            }
            return resultado;
        }

        private int MayorPrimeroComparar(double a, double b) {
            int resultado = 0;
            if (a > b) {
                resultado = 1;
            }
            else if (a < b) {
                resultado = -1;
            }
            return resultado;
        }


        public bool EsVisiblePor(EstadoAventurero otro) {
            return CalculadoraGeometrica.APuedeVerAB(otro,this);
        }

        private void EncerrarPosicionEnArena()
        {
            _posicion.X = Clamp(Posicion.X, 0, ReglasDelJuego.ANCHO_ARENA);
            _posicion.Y = Clamp(Posicion.Y, 0, ReglasDelJuego.ALTO_ARENA);
        }

        private int Clamp(int valor, int min, int max)
        {
            if (valor < min)
            {
                valor = min;
            }
            else if (valor > max)
            {
                valor = max;
            }
            return valor;
        }

        #endregion

    }
}
