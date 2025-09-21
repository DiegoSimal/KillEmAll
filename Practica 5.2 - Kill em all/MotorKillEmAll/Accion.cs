using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KillEmAll
{
    public struct Accion
    {

        private TipoAccion _accion;
        private Point? _posicionObjetivo;
        private int? _idObjetivo;

        public TipoAccion TipoAccion
        {
            get
            {
                return _accion;
            }
            set
            {
                _accion = value;
            }
        }

        public int? IdObjetivo {
            get {
                return _idObjetivo;
            }
            set
            {
                _idObjetivo = value;
            }
        }

        public Point? PosicionObjetivo {
            get {
                return _posicionObjetivo;
            }
            set {
                _posicionObjetivo = value;
            }
        }

        public Accion(TipoAccion accion)
        {
            _accion = accion;
            _posicionObjetivo = null;
            _idObjetivo = null;
        }

        public Accion(TipoAccion accion,int idObjetivo)
        {
            _accion = accion;
            _idObjetivo = idObjetivo;
            _posicionObjetivo = null;
        }

        public Accion(TipoAccion accion, Point posicionObjetivo) {
            _accion = accion;
            _idObjetivo = null;
            _posicionObjetivo = posicionObjetivo;
        }


       
 

    }
}
