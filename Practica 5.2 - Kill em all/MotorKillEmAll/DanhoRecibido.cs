using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace KillEmAll
{

    public enum CuadrantePercepcion { Frente, Izquierda, Derecha, Detras}

    public sealed class DanhoRecibido
    {

        private int _danho;
        private CuadrantePercepcion _direccionAtaque;
        private TipoAccion _tipoDeAtaque;
        private Point? _origenAtaque;
        private int? _idAtacante;
        private bool _ataqueMortal;
        private string _aclaraciones;
        private string _nombreAtacante;
        private string _nombreVictima;

        public CuadrantePercepcion DireccionAtaque {
            get {
                return _direccionAtaque;
            }
        }

        public Point? OrigenAtaque {
            get {
                return _origenAtaque;
            }
        }

        public int Danho {
            get {
                return _danho;
            }
        }

        public TipoAccion TipoDeAtaque {
            get {
                return _tipoDeAtaque;
            }
        }

        public int? IdAtacante {
            get {
                return _idAtacante;
            }
        }

        public bool EsDanhoMortal
        {
            get
            {
                return _ataqueMortal;
            }
        }

        public string Aclaraciones
        {
            get
            {
                string resultado = _aclaraciones;
                if (resultado == null)
                {
                    resultado = "";
                }
                return resultado;
            }
        }

        public string NombreAtacante
        {
            get
            {
                string resultado = "<Desconocido>";
                if (_nombreAtacante != null)
                {
                    resultado = _nombreAtacante;
                }
                return resultado;
            }
        }

        public string NombreVictima
        {
            get
            {
                return _nombreVictima;
            }
        }


        public DanhoRecibido(int danho, TipoAccion tipoDeAtaque, Point posicionActual, int idAtacante, string nombreAtacante, string nombreVictima, Point origenAtaque, int orientacionActual, bool ataqueMortal, string aclaraciones=null) {
            _danho = danho;
            _tipoDeAtaque = tipoDeAtaque;
            _direccionAtaque = CalculadoraGeometrica.CalcularDireccion(posicionActual, origenAtaque, orientacionActual);
            if (_direccionAtaque == CuadrantePercepcion.Frente) {
                _origenAtaque= origenAtaque;
                _idAtacante = idAtacante;
                _nombreAtacante= nombreAtacante;
            }
            else {
                _origenAtaque = null;
                _idAtacante = null;
                _nombreAtacante = null;
            }
            _nombreVictima = nombreVictima;
            _ataqueMortal = ataqueMortal;
            _aclaraciones = aclaraciones;
        }
    }
}
