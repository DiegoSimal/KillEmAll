using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace KillEmAll
{
    

    public struct Situacion
    {
        private int _id;
        private string _nombre;
        private Point _posicion;
        private int _orientacion;
        private Clase _clase;
        private int _vida;

        public Point Posicion
        {
            get
            {
                return _posicion;
            }
        }

        public int Orientacion
        {
            get
            {
                return _orientacion;
            }
        }

        public Clase Clase {
            get {
                return _clase;
            }
            set {
                _clase = value;
            }
        }

        public int ID {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        public string Nombre {
            get {
                return _nombre;
            }
        }

        public int Vida
        {
            get
            {
                return _vida;
            }
            set
            {
                _vida = value;
            }

        }

        public Situacion(int id, string nombre, int orientacion, Point posicion, int vida, Clase clase) {
            _id = id;
            _nombre = nombre;
            _orientacion = orientacion;
            _clase = clase;
            _posicion = posicion;
            _vida = vida;
        }

        public static Situacion CrearSituacionVacia()
        {
            return new Situacion(0, "", 0, new Point(), 0, Clase.Guerrero);
        }

    }
}
