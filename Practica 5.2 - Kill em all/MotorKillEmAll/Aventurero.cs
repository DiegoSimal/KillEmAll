using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;

namespace KillEmAll
{

    
    public abstract class Aventurero : AventureroBase
    {
        protected readonly int id;
        protected readonly string nombre;
        protected TipoAccion accion;
        protected int vida;
        protected int orientacion;
        protected int? idObjetivo;
        protected Point posicion;
        protected Point? posicionObjetivo;
        protected Situacion[] alrededores;
        protected List<DanhoRecibido> danhoRecibido;
        private Point _posicionAnterior;
        protected readonly Clase clase;
        protected List<Situacion> aventurerosEliminadosEsteTurno;
        protected List<DanhoRecibido> aventurerosAtacadosEsteTurno;
        private Situacion _verdugo;
        private Point? _posicionObjetivoAnterior;
        private int? _idObjetivoAnterior;
        private bool _llevaUnTurnoMuerto;
        private string _nombreObjetivo;

        public sealed override int Id {
            get {
                return id;
            }
        }

        public sealed override string Nombre
        {
            get
            {
                return nombre;
            }
        }

        public sealed override string NombreObjetivo
        {
            set
            {
                if (value == null)
                {
                    _nombreObjetivo = "<Ninguno>";
                }
                else
                {
                    _nombreObjetivo = value;
                }
            }
        }


        public int Vida {
            set {
                vida = value;
            }
            get {
                return vida;
            }
        }

        public Point Posicion {
            set {
                posicion = value;
            }
        }

        public int Orientacion {
            get
            {
                return orientacion;
            }
            set {
                orientacion = value;
            }
        }

        public Clase Clase {
            get {
                return clase;
            }
        }

        public Situacion Verdugo
        {
            
            set
            {
                _verdugo= value;
            }
        }


        //El juego accede a la Situación para cambiar la orientación tanto como quiera
        //En cada turno, el juego cambiará la situacion completa (realiza el movimiento)

        public sealed override Situacion Situacion
        {
            set {
                _posicionAnterior = posicion;
                posicion = value.Posicion;
                orientacion = value.Orientacion;
                vida = value.Vida;
                //Pasamos a null el idObjetivo para evitar que se quede un objetivo obsoleto
                //entre turnos
                idObjetivo = null;
                posicionObjetivo = null;
            }
        }

        public sealed override Accion Accion
        {
            get
            {
                Accion accionARealizar;
                if (idObjetivo == null)
                {
                    if (posicionObjetivo == null) {
                        accionARealizar = new Accion(accion);
                    }
                    else {
                        accionARealizar = new Accion(accion, (Point)posicionObjetivo);
                    }
                }
                else
                {
                    accionARealizar = new Accion(accion, (int)idObjetivo);
                }
                _posicionObjetivoAnterior = posicionObjetivo;
                _idObjetivoAnterior = idObjetivo;
                return accionARealizar;
            }
        }

        //Alrededores guarda las posiciones y acciones de todos los elementos
        public Situacion[] Alrededores
        {
            set
            {
                alrededores = value;
            }
        }

        public bool LlevaUnTurnoMuerto
        {
            get
            {
                //Esta propiedad devuelve false mientras está vivo y justo en el turno en el que
                //ha muerto, pero a partir de ahí devuelve true.
                bool aux = false;
                if (vida <= 0)
                {
                    if (!_llevaUnTurnoMuerto)
                    {
                        _llevaUnTurnoMuerto = true;
                    }
                    else
                    {
                        aux = true;
                    }                 
                }
                return aux;
            }
        }
        


        public Aventurero(string nombre, Clase clase)
        {
            this.nombre = nombre;
            this.clase = clase;
            alrededores = new Situacion[0];
            id=GetHashCode();
            _llevaUnTurnoMuerto = false;
            aventurerosEliminadosEsteTurno = new List<Situacion>();
            aventurerosAtacadosEsteTurno = new List<DanhoRecibido>();
        }


        //DecidirAccion ha de terminar en modificar el objeto situacion, en lo que
        //respecta a la acción y el objetivo (que puede ser un Id de Aventurero
        //o bien una posición).
        public abstract void DecidirAccion();


        public sealed override void ResetAlrededores() {
            alrededores = new Situacion[0];
        }

        public sealed override void ResetDanhoRecibido() {
            danhoRecibido = new List<DanhoRecibido>();
        }

        public sealed override void ResetAventurerosMuertos()
        {
            aventurerosEliminadosEsteTurno.Clear();
        }

        public sealed override void ResetAventurerosAtacados()
        {
            aventurerosAtacadosEsteTurno.Clear();
        }


        public sealed override void AnhadirDanhoRecibido(DanhoRecibido danho) {
            danhoRecibido.Add(danho);
        }

        public sealed override void AnhadirAventureroMuerto(Situacion victima)
        {
            aventurerosEliminadosEsteTurno.Add(victima);
        }

        public sealed override void AnhadirAventureroAtacado(DanhoRecibido danho)
        {
            aventurerosAtacadosEsteTurno.Add(danho);
        }

        public override string ToString() {
            StringBuilder informe=new StringBuilder();
            string aux_objetivo="";
            if (_posicionObjetivoAnterior != null)
            {
                aux_objetivo = _posicionObjetivoAnterior.ToString();
            }
            else if (_idObjetivoAnterior != null)
            {
                aux_objetivo = _nombreObjetivo;
            }
            informe.Append($"\n\tAl principio del turno estaba en {_posicionAnterior}");
            if (accion != TipoAccion.Percepcion)
            {
                informe.Append($" y su objetivo era {aux_objetivo}.");
            }
            informe.Append($"\n\tAhora está en {posicion} y su orientación es {orientacion}.");
            if (aventurerosAtacadosEsteTurno!=null && aventurerosAtacadosEsteTurno.Count > 0)
            {
                informe.Append("\n\tSu ataque ha causado los siguientes daños:");
                foreach(DanhoRecibido danho in aventurerosAtacadosEsteTurno)
                {
                    if (danho.NombreVictima == this.nombre)
                    {
                        informe.Append($"\n\t\t{danho.Danho} PV infligido A SÍ MISMO!!!");
                    }
                    else
                    {
                        informe.Append($"\n\t\t{danho.Danho} PV a {danho.NombreVictima}");
                    }
                }

            }
            if (vida <= 0)
            {
                if (_verdugo.ID != Id)
                {
                    informe.Append($"\n\tHa muerto a manos de {_verdugo.Nombre}");
                }
                else
                {
                    informe.Append($"\n\tEl infeliz se ha matado a sí mismo");
                }
            }
            if (aventurerosEliminadosEsteTurno != null && aventurerosEliminadosEsteTurno.Count > 0)
            {
                informe.Append($"\n\tEste turno ha eliminado a:");
                foreach(Situacion situacion in aventurerosEliminadosEsteTurno)
                {
                    informe.Append($"\n\t\t{situacion.Nombre}");
                }
            }
            if (vida > 0)
            {
                if (danhoRecibido!=null && danhoRecibido.Count > 0)
                {
                    informe.Append($"\n\tDaño recibido este turno:");
                    foreach (DanhoRecibido d in danhoRecibido)
                    {
                        if (d.NombreAtacante != Nombre) { 
                            informe.Append($"\n\t\t{d.Danho} PV por un ataque de {d.NombreAtacante}");
                        }
                        else
                        {
                            informe.Append($"\n\t\t{d.Danho} PV autoinfligidos");
                        }
                        if (d.EsDanhoMortal) informe.Append(". Este ataque ha sido mortal");
                    }
                }
                if (alrededores.Length > 0)
                {
                    informe.Append($"\n\tPuede ver a:");
                    for (int i = 0; i < alrededores.Length; ++i)
                    {
                        informe.Append($"\n\t\t{alrededores[i].Nombre} - está en {alrededores[i].Posicion}, a {CalculadoraGeometrica.CalcularDistancia(posicion, alrededores[i].Posicion)} unidades de distancia");

                    }
                }
                else
                {
                    informe.Append("\n\tNo hay nadie vivo en su rango de visión");
                }
            }
            informe.AppendLine();
            return informe.ToString();
        }

        public string InformeFinal()
        {
            StringBuilder informe = new StringBuilder();
            informe.Append($"{nombre} / {clase.ToString()}\tPuntuación: {Puntuacion}");
            return informe.ToString();
        }
    }

    public abstract class AventureroBase : IComparable<AventureroBase>
    {
        private int _puntuacion;
        private static Random s_generador = new Random();

        public int Puntuacion
        {
            get
            {
                return _puntuacion;
            }
            set
            {
                _puntuacion = value;
            }
        }

        public virtual Situacion Situacion
        {
            get
            {
                return Situacion.CrearSituacionVacia();
            }
            set
            {

            }
        }



        public virtual string Nombre
        {
            get
            {
                return null;
            }
        }

        public virtual string NombreObjetivo
        {
            set
            {

            }
        }

        public virtual Accion Accion
        {
            get
            {
                return new Accion();
            }
        }

        public virtual int Id
        {
            get
            {
                return 0;
            }
        }

        public abstract void ResetAlrededores();

        public abstract void ResetDanhoRecibido();

        public abstract void ResetAventurerosMuertos();
        
        public abstract void ResetAventurerosAtacados();
        
        public abstract void AnhadirDanhoRecibido(DanhoRecibido danho);

        public abstract void AnhadirAventureroMuerto(Situacion victima);

        public abstract void AnhadirAventureroAtacado(DanhoRecibido victima);

        public sealed override int GetHashCode()
        {
            SHA256 hashAlgorithm = SHA256.Create();
            byte[] hash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(this.ToString() + s_generador.NextDouble()));
            int intHash = BitConverter.ToInt32(hash);
            return intHash;
        }

        public int CompareTo(AventureroBase otro)
        {
            if (_puntuacion < otro._puntuacion)
            {
                return 1;
            }
            else if (_puntuacion> otro._puntuacion)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }


}
