using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace KillEmAll
{
    public delegate void DelegadoAccionARealizar(EstadoAventurero a);
    public class Arena
    {
        //Los estados pertenecen a Arena para que los Aventureros no puedan cambiar arbitrariamente
        //su vida, su posición o su puntuación. Al terminar las acciones, se actualizan en los aventureros los cambios pertinentes.
        private Dictionary<int, EstadoAventurero> _estadosAventureros;

        private Dictionary<int, Aventurero> _aventureros;
        private Dictionary<int, Aventurero> _aventurerosCaidos;

        private Dictionary<TipoAccion, List<EstadoAventurero>> _estadosPorTipoAccion;
        private Dictionary<TipoAccion, DelegadoAccionARealizar> _delegados;
        private Dictionary<TipoAccion, DelegadoAccionARealizar> _delegadosAtaques;
        private int _turnosRealizados;

        private bool _combateTerminado;
        public bool CombateTerminado
        {
            get
            {
                return _combateTerminado;
            }
        }
        

        public Arena(Dictionary<int, Aventurero> aventureros, Dictionary<int, EstadoAventurero> estados)
        {
            _delegados = new Dictionary<TipoAccion, DelegadoAccionARealizar> {
                {TipoAccion.Percepcion, RealizarPercepcion},
                {TipoAccion.Arco, RealizarAtaque },
                {TipoAccion.BolaDeFuego, RealizarAtaque },
                {TipoAccion.Movimiento, RealizarMovimiento },
                {TipoAccion.CuerpoACuerpo, RealizarAtaque }
             };
            _delegadosAtaques = new Dictionary<TipoAccion, DelegadoAccionARealizar> {
                {TipoAccion.Arco, RealizarAtaqueConArco },
                {TipoAccion.BolaDeFuego, RealizarAtaqueBolaDeFuego },
                {TipoAccion.CuerpoACuerpo, RealizarPrimeraRondaDeAtaqueCuerpoACuerpo }
            };
            _aventureros = aventureros;
            _estadosAventureros = estados;
            _aventurerosCaidos=new Dictionary<int, Aventurero>();
            _turnosRealizados = 0;
        }

        //Al actualizar la posición en un aventurero, hay que actualizarla en todos los aventureros que lo tengan como objetivo
        //Y hay que reordenarlos de nuevo!!! Para ello, mejor sacarlos de la lista antes de reordenar.
        public Dictionary<int,EstadoAventurero> RealizarTurno() {
            if (_aventureros.Count > 1)
            {
                //Primero hacemos que los aventureros elijan su accion
                ForzarEleccionDeAccionALosAventureros();
                //Extraemos todas las acciones de los aventureros y organizamos los estados en el diccionario
                OrganizarAcciones();
                //Realizamos todas las acciones en el orden marcado. Realizar ataques cuerpo a cuerpo es
                //especial porque implica un poco de movimiento, así que se tienen que reordenar las situaciones
                //cada vez que se realiza la acción de un Aventurero
                //Además, se realiza una primera ronda de ataques cuerpo a cuerpo antes del movimiento,
                //y una segunda después del movimiento
                RealizarAtaquesCuerpoACuerpo();
                RealizarAccionesSimples(TipoAccion.Movimiento);
                RealizarAccionesSimples(TipoAccion.BolaDeFuego);
                RealizarAccionesSimples(TipoAccion.Arco);
                RealizarAtaquesCuerpoACuerpo();
                RealizarAccionesSimples(TipoAccion.Percepcion);

                //Si llevamos muchos turnos en tablas, aplicamos un daño automático sobre los supervivientes
                if (++_turnosRealizados > 100)
                {
                    AplicarDanhoAutomatico();
                }


                //Por último, se rellenan los alrededores de todos 
                RellenarAlrededores();
                //Se eliminan los aventureros caídos
                EliminarAventurerosCaidos();
                //Para terminar, se actualizan en los aventureros sus estados.
                ActualizarEstadosDeAventureros();

                if (_aventureros.Count == 1 && !_combateTerminado)
                {
                    _combateTerminado = true;
                    Dictionary<int, Aventurero>.Enumerator e = _aventureros.GetEnumerator();
                    e.MoveNext();
                    Aventurero ultimoEnPie = e.Current.Value;
                    _estadosAventureros[ultimoEnPie.Id].SumarPuntuacionPorSerElUltimoEnPie();
                }
                else if (_aventureros.Count == 0)
                {
                    _combateTerminado = true;
                }
                
            }
            return _estadosAventureros;
        }

        
        private void ForzarEleccionDeAccionALosAventureros() {
            Logger.Log("Comienza turno");
            foreach(Aventurero aventurero in _aventureros.Values) {
                if (aventurero.Vida > 0)
                {
                    Logger.Log($"\tPidiendo accion a {aventurero.Nombre}");
                    aventurero.DecidirAccion();
                }
            }
        }


        private void RealizarAccionesSimples(TipoAccion accion) {
            foreach (EstadoAventurero estado in _estadosPorTipoAccion[accion]) {
                if (estado.Vida > 0) {
                    estado.accionARealizar(estado);
                }
            }
        }

        private void RealizarMovimiento(EstadoAventurero estadoAventurero) {
            RealizarMovimiento(estadoAventurero, ReglasDelJuego.MOVIMIENTO_MAXIMO);
        }

        private void RealizarMovimiento(EstadoAventurero estadoAventurero, int distanciaMaxima)
        {
            estadoAventurero.MoverAObjetivo(distanciaMaxima);
            Accion accion = _aventureros[estadoAventurero.Id].Accion;
            if (accion.IdObjetivo != null && accion.IdObjetivo != 0)
            {
                if (_aventureros.ContainsKey((int)accion.IdObjetivo))
                {
                    _aventureros[estadoAventurero.Id].NombreObjetivo = _aventureros[(int)accion.IdObjetivo].Nombre;
                }
                else if (_aventurerosCaidos.ContainsKey((int)accion.IdObjetivo))
                {
                    _aventureros[estadoAventurero.Id].NombreObjetivo = _aventurerosCaidos[(int)accion.IdObjetivo].Nombre;
                }
            }
            else if (accion.PosicionObjetivo != null)
            {
                _aventureros[estadoAventurero.Id].NombreObjetivo = accion.PosicionObjetivo.ToString();
            }
            ActualizarPosicionDeObjetivoEnLosDemasAventureros(estadoAventurero.Id, estadoAventurero.Posicion);
        }

        private void RealizarAtaque(EstadoAventurero estadoAventurero) {
            if (ObjetivoEstaMuerto(estadoAventurero)) {
                ReasignarAccionAPercepcion(estadoAventurero);
            }
            else {
                Accion accion = _aventureros[estadoAventurero.Id].Accion;
                if (accion.IdObjetivo != null && accion.IdObjetivo!=0)
                {
                    _aventureros[estadoAventurero.Id].NombreObjetivo = _aventureros[(int)accion.IdObjetivo].Nombre;
                }
                else if (accion.PosicionObjetivo!=null)
                {
                    _aventureros[estadoAventurero.Id].NombreObjetivo = accion.PosicionObjetivo.ToString();
                }
                _delegadosAtaques[estadoAventurero.TipoAccion](estadoAventurero);
            }
        }
        

        private void ReasignarAccionAPercepcion(EstadoAventurero estado) {
            estado.TipoAccion = TipoAccion.Percepcion;
            _estadosPorTipoAccion[TipoAccion.Percepcion].Add(estado);
            estado.accionARealizar = _delegados[estado.TipoAccion];
        }

        private void ReasignarAccion(EstadoAventurero estado, TipoAccion accion)
        {
            estado.TipoAccion = accion;
            estado.accionARealizar = _delegados[estado.TipoAccion];
        }

        private void RealizarAtaqueBolaDeFuego(EstadoAventurero estadoAtacante) {
            estadoAtacante.OrientarHaciaObjetivo();
            //Si el objetivo está al alcance, realiza el ataque
            if (CalculadoraGeometrica.EstanADistancia(estadoAtacante.Posicion, (Point)estadoAtacante.PosicionObjetivo, ReglasDelJuego.ALCANCE_BOLA_DE_FUEGO))
            {
                List<EstadoAventurero> aventurerosAfectados = AventurerosDentroDelRadioDeLaBolaDeFuego((Point)estadoAtacante.PosicionObjetivo);
                foreach (EstadoAventurero estadoDefensor in aventurerosAfectados)
                {
                    AplicarDanhoDeAtaque(estadoAtacante, estadoDefensor);
                }
            }
            //Si no está al alcance, se mueve todo lo posible para poder atacar en el próximo turno, quedando fuera del alcance de la bola
            else
            {
                float distanciaMaxima = Math.Min(ReglasDelJuego.MOVIMIENTO_MAXIMO,CalculadoraGeometrica.CalcularDistancia(estadoAtacante.Posicion, (Point)estadoAtacante.PosicionObjetivo)-ReglasDelJuego.RADIO_BOLA_DE_FUEGO);
                Point destino = CalculadoraGeometrica.RecalcularPosicionObjetivo(estadoAtacante.Posicion, (Point)estadoAtacante.PosicionObjetivo, (int)distanciaMaxima);
                estadoAtacante.RefrescarPosicionObjetivo(destino);
                estadoAtacante.IdObjetivo = null;
                ReasignarAccion(estadoAtacante, TipoAccion.Movimiento);
                RealizarMovimiento(estadoAtacante);
            }
        }

        private List<EstadoAventurero> AventurerosDentroDelRadioDeLaBolaDeFuego(Point centro)
        {
            List<EstadoAventurero> aventurerosAfectados = new List<EstadoAventurero>();
            foreach(EstadoAventurero estado in _estadosAventureros.Values)
            {
                if (estado.EstaVivo() && CalculadoraGeometrica.EstanADistancia(centro, estado.Posicion, ReglasDelJuego.RADIO_BOLA_DE_FUEGO))
                {
                    aventurerosAfectados.Add(estado);
                }
            }
            return aventurerosAfectados;
        }

        private void RealizarAtaqueConArco(EstadoAventurero estadoAtacante) {
            //Cambiar la orientación del aventurero atacante para que mire a su objetivo
            estadoAtacante.OrientarHaciaObjetivo();
            //Si el objetivo está demasiado cerca, realiza percepción
            if (estadoAtacante.DistanciaAObjetivo < ReglasDelJuego.DISTANCIA_MINIMA_ARCO)
            {
                ReasignarAccionAPercepcion(estadoAtacante);
            }
            else if (estadoAtacante.DistanciaAObjetivo < ReglasDelJuego.ALCANCE_ARCO)
            {
                List<EstadoAventurero> aventurerosAtacados = new List<EstadoAventurero>();
                //Un arquero tiene flechas perforantes
                bool ataqueInterceptado = false;
                if (estadoAtacante.Clase == Clase.Arquero)
                {
                    aventurerosAtacados = CalculadoraDeAtaques.BuscarAventurerosEnTrayectoriaDeAtaque(estadoAtacante, (Point)estadoAtacante.PosicionObjetivo, _estadosAventureros, ReglasDelJuego.ALCANCE_ARCO, ReglasDelJuego.DISTANCIA_MAXIMA_INTERCEPCION);
                }
                //Un ataque de arco normal puede ser interceptado
                else
                {
                    EstadoAventurero receptorDelAtaque = CalculadoraDeAtaques.BuscarAventureroEnTrayectoriaDeAtaque(estadoAtacante, (Point)estadoAtacante.PosicionObjetivo, _estadosAventureros, ReglasDelJuego.ALCANCE_ARCO,ReglasDelJuego.DISTANCIA_MAXIMA_INTERCEPCION);
                    if (receptorDelAtaque!=null) aventurerosAtacados.Add(receptorDelAtaque);
                    if (receptorDelAtaque.Posicion != estadoAtacante.PosicionObjetivo)
                    {
                        ataqueInterceptado = true;
                    }
                }
                foreach (EstadoAventurero estado in aventurerosAtacados)
                {
                    AplicarDanhoDeAtaque(estadoAtacante, estado,ataqueInterceptado);
                }
            }
            //Si el objetivo está demasiado lejos, tratará de acercarse todo lo posible
            else
            {
                estadoAtacante.IdObjetivo = null;
                ReasignarAccion(estadoAtacante, TipoAccion.Movimiento);
                RealizarMovimiento(estadoAtacante);
            }
        }
            
        

        private void ActualizarEstadosDeAventureros() {
            foreach(Aventurero aventurero in _aventureros.Values) {
                EstadoAventurero estado = _estadosAventureros[aventurero.Id];
                aventurero.Posicion = estado.Posicion;
                aventurero.Orientacion = estado.Orientacion;
                aventurero.Vida = estado.Vida;
            }
        }

        private void RealizarAtaquesCuerpoACuerpo() {
            List<EstadoAventurero> ataquesCuerpoACuerpo = new List<EstadoAventurero>(_estadosPorTipoAccion[TipoAccion.CuerpoACuerpo].ToArray());
            while(ataquesCuerpoACuerpo.Count > 0) {
                if (ataquesCuerpoACuerpo[0].Vida>0 && !ataquesCuerpoACuerpo[0].AccionRealizada)
                {
                    ataquesCuerpoACuerpo[0].accionARealizar(ataquesCuerpoACuerpo[0]);
                }
                ataquesCuerpoACuerpo.RemoveAt(0);
                ataquesCuerpoACuerpo.Sort();
            }
        }

        private void RealizarPrimeraRondaDeAtaqueCuerpoACuerpo(EstadoAventurero estadoAtacante)
        {
            if (!ObjetivoExiste(estadoAtacante))
            {
                DanhoRecibido danhoRecibido = CalculadoraDeAtaques.GenerarAtaque(estadoAtacante, estadoAtacante);
                _aventureros[estadoAtacante.Id].AnhadirDanhoRecibido(danhoRecibido);
                estadoAtacante.AplicarDanho(danhoRecibido);
                estadoAtacante.AccionRealizada = true;
                if (!estadoAtacante.EstaVivo()) {
                    _aventureros[estadoAtacante.Id].Verdugo = _aventureros[estadoAtacante.Id].Situacion;
                }
            }
            else
            {
                if (estadoAtacante.DistanciaAObjetivo - ReglasDelJuego.ALCANCE_CUERPO_A_CUERPO <= estadoAtacante.DistanciaARecorrerEnAtaque)
                {
                    RealizarMovimiento(estadoAtacante, ReglasDelJuego.MOVIMIENTO_MAXIMO_CON_ATAQUE);
                    AplicarDanhoDeAtaque(estadoAtacante);
                    estadoAtacante.AccionRealizada = true;
                }
                else
                {
                    estadoAtacante.accionARealizar = RealizarSegundaRondaDeAtaqueCuerpoACuerpo;
                }
            }           
        }


        private void RealizarSegundaRondaDeAtaqueCuerpoACuerpo(EstadoAventurero estadoAtacante) {
            if (estadoAtacante.DistanciaAObjetivo - ReglasDelJuego.ALCANCE_CUERPO_A_CUERPO <= estadoAtacante.DistanciaARecorrerEnAtaque)
            {
                RealizarMovimiento(estadoAtacante, ReglasDelJuego.MOVIMIENTO_MAXIMO_CON_ATAQUE);
                AplicarDanhoDeAtaque(estadoAtacante);
            }
            else
            {
                RealizarMovimiento(estadoAtacante, ReglasDelJuego.MOVIMIENTO_MAXIMO);
            }
        }

        private void AplicarDanhoDeAtaque(EstadoAventurero atacante)
        {
            EstadoAventurero defensor = _estadosAventureros[(int)atacante.IdObjetivo];
            AplicarDanhoDeAtaque(atacante, defensor);
        }

        private void AplicarDanhoDeAtaque(EstadoAventurero atacante, EstadoAventurero defensor, bool intercepcion=false)
        {
            DanhoRecibido danhoRecibido = CalculadoraDeAtaques.GenerarAtaque(atacante, defensor);
            if (atacante.Id != defensor.Id)
            {
                atacante.SumarPuntuacion(danhoRecibido);
                _aventureros[atacante.Id].AnhadirAventureroAtacado(danhoRecibido);
            }
            defensor.AplicarDanho(danhoRecibido);
            _aventureros[defensor.Id].AnhadirDanhoRecibido(danhoRecibido);
            if (!defensor.EstaVivo())
            {
                _aventureros[defensor.Id].Verdugo = atacante.Situacion;
                _aventureros[atacante.Id].AnhadirAventureroMuerto(defensor.Situacion);
            }
        }

        private void AplicarDanhoAutomatico()
        {
            foreach(Aventurero aventurero in _aventureros.Values)
            {
                _estadosAventureros[aventurero.Id].AplicarDanho(ReglasDelJuego.DANHO_AUTOMATICO);
            }
        }

        private bool ObjetivoEstaMuerto(EstadoAventurero estado) {
            bool respuesta = false;
            if (estado.IdObjetivo != null && ObjetivoExiste(estado)) {
                EstadoAventurero estadoObjetivo = _estadosAventureros[(int)estado.IdObjetivo];
                respuesta = !estadoObjetivo.EstaVivo();
            }
            return respuesta;
        }

        private bool ObjetivoExiste(EstadoAventurero estado)
        {
            return _estadosAventureros.ContainsKey((int)estado.IdObjetivo);
        }

        private void EliminarAventurerosCaidos()
        {
            foreach(Aventurero aventurero in _aventureros.Values)
            {
                EstadoAventurero estado = _estadosAventureros[aventurero.Id];
                if (!estado.EstaVivo())
                {
                    aventurero.Vida = 0;
                    _aventureros.Remove(aventurero.Id);
                    _aventurerosCaidos.Add(aventurero.Id, aventurero);
                }
            }
        }

        private void AsignarPosicionesDeObjetivos()
        {
            foreach (EstadoAventurero estadoAventurero in _estadosAventureros.Values)
            {
                InsertarPosicionObjetivo(estadoAventurero);
            }
        }

        //Inserta la posición del objetivo. Si el objetivo es incorrecto (no existe en la lista de aventureros) y no tiene una posición marcada, la posición es la del propio
        //aventurero.
        private void InsertarPosicionObjetivo(EstadoAventurero estadoAventurero)
        {
            if (estadoAventurero.IdObjetivo != null && ObjetivoExiste(estadoAventurero))
            {
                estadoAventurero.RefrescarPosicionObjetivo(_estadosAventureros[(int)estadoAventurero.IdObjetivo].Posicion);
            }
            else if (estadoAventurero.PosicionObjetivo == null)
            {
                estadoAventurero.RefrescarPosicionObjetivo(estadoAventurero.Posicion);
            }
        }

        private void ActualizarPosicionDeObjetivoEnLosDemasAventureros(int idObjetivo, Point nuevaPosicion) {
            foreach (EstadoAventurero estadoAventurero in _estadosAventureros.Values) {
                estadoAventurero.RefrescarPosicionObjetivo(nuevaPosicion, idObjetivo);
            }
        }

        #region Percepción y rellenado de alrededores
        private void RealizarPercepcion(EstadoAventurero estadoAventurero) {
            RellenarAlrededores(estadoAventurero.Id);
            
            estadoAventurero.Orientacion = _aventureros[estadoAventurero.Id].Orientacion;
        }

        private void RellenarAlrededores() {
            foreach (TipoAccion accion in Enum.GetValues(typeof(TipoAccion))) {
                if (accion != TipoAccion.Percepcion) {
                    RellenarAlrededoresPorTipoAccion(_estadosPorTipoAccion[accion]);
                }
            }
        }

        private void RellenarAlrededoresPorTipoAccion(List<EstadoAventurero> estados) {
            foreach (EstadoAventurero estado in estados) {
                RellenarAlrededores(estado.Id);
            }
        }

        //Esta función es para todos los aventureros que no han realizado percepción
        private void RellenarAlrededores(int id) {
            //Rellenamos los alrededores del aventurero con id dado
            EstadoAventurero estado = _estadosAventureros[id];
            List<Situacion> alrededores = new List<Situacion>();
            foreach(Aventurero aux in _aventureros.Values) {
                if (aux.Id != id) {
                    EstadoAventurero otro = _estadosAventureros[aux.Id];
                    if (otro.EstaVivo() && (estado.TipoAccion==TipoAccion.Percepcion || estado.PuedeVerA(otro)) ) {
                        alrededores.Add(otro.Situacion);
                    }
                }
            }
            alrededores = OrdenarAleatoriamente(alrededores);
            _aventureros[id].Alrededores= alrededores.ToArray();
        }

        private List<Situacion> OrdenarAleatoriamente(List<Situacion> original) {
            List<Situacion> listaDesordenada = new List<Situacion>();
            Random generador = new Random();
            Situacion aux;
            while (original.Count > 0) {
                aux = original[generador.Next(original.Count)];
                listaDesordenada.Add(aux);
                original.Remove(aux);
            }
            return listaDesordenada;
        }

        #endregion



        #region Organización de las acciones antes de procesarlas

        private void OrganizarAcciones() {
            FiltrarAcciones(ExtraerAcciones());
            AsignarPosicionesDeObjetivos();
        }

        private List<EstadoAventurero> ExtraerAcciones() {
            List<EstadoAventurero> lista = new List<EstadoAventurero>();
            foreach(Aventurero aventurero in _aventureros.Values) {
                aventurero.ResetAlrededores();
                aventurero.ResetDanhoRecibido();
                aventurero.ResetAventurerosMuertos();
                aventurero.ResetAventurerosAtacados();
                Accion accion = aventurero.Accion;
                EstadoAventurero estadoAventurero = _estadosAventureros[aventurero.Id];
                //Si la clase no tiene permitido realizar la acción, hará percepción
                if (!ReglasDelJuego.s_accionesPermitidas[estadoAventurero.Clase].Contains(accion.TipoAccion))
                {
                    accion = new Accion(TipoAccion.Percepcion);
                }
                estadoAventurero.Accion = accion;
                estadoAventurero.accionARealizar = _delegados[accion.TipoAccion];
                estadoAventurero.SeHaMovido = false;
                estadoAventurero.AccionRealizada = false;
                _estadosAventureros[estadoAventurero.Id] = estadoAventurero;
                lista.Add(estadoAventurero);
            }
            return lista;
        }

        private void FiltrarAcciones(List<EstadoAventurero> lista) {
            ResetearDiccionarioDeSituaciones();
            GuardarAccionesEnSuDiccionario(lista);
            OrdenarTodasLasAcciones();
        }

        private void ResetearDiccionarioDeSituaciones() {
            _estadosPorTipoAccion = new Dictionary<TipoAccion, List<EstadoAventurero>>();
            foreach (TipoAccion accion in Enum.GetValues(typeof(TipoAccion))) {
                _estadosPorTipoAccion[accion] = new List<EstadoAventurero>();
            }
        }

        private void OrdenarTodasLasAcciones() {
            foreach (KeyValuePair<TipoAccion, List<EstadoAventurero>> par in _estadosPorTipoAccion) {
                par.Value.Sort();
            }
        }

        private void GuardarAccionesEnSuDiccionario(List<EstadoAventurero> lista) {
            foreach (EstadoAventurero estado in lista) {
                _estadosPorTipoAccion[estado.TipoAccion].Add(estado);
            }
        }

        

        #endregion


        public string GetInformeDeAventureros() {
            StringBuilder sb = new StringBuilder();
            foreach(Aventurero a in _aventureros.Values) {
                sb.Append(_estadosAventureros[a.Id].ToString());
                sb.Append(a.ToString());
                sb.Append("\n");
            }
            foreach (Aventurero a in _aventurerosCaidos.Values)
            {
                if (!a.LlevaUnTurnoMuerto)
                {
                    sb.Append(_estadosAventureros[a.Id].ToString());
                    sb.Append(a.ToString());
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        private List<Aventurero> OrdenarAventurerosPorPuntuacion()
        {
            List<Aventurero> aventurerosOrdenados = new List<Aventurero>();
            foreach(Aventurero a in _aventureros.Values)
            {
                a.Puntuacion = _estadosAventureros[a.Id].Puntuacion;
                aventurerosOrdenados.Add(a);
            }
            foreach(Aventurero a in _aventurerosCaidos.Values)
            {
                a.Puntuacion = _estadosAventureros[a.Id].Puntuacion;
                aventurerosOrdenados.Add(a);
            }
            aventurerosOrdenados.Sort();
            return aventurerosOrdenados;
        }

        public string GetInformeFinal()
        {
            List<Aventurero> aventurerosOrdenados = OrdenarAventurerosPorPuntuacion();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < aventurerosOrdenados.Count; ++i)
            {
                sb.Append(aventurerosOrdenados[i].InformeFinal());
                if (aventurerosOrdenados[i].Vida > 0)
                {
                    sb.Append("  ULTIMO EN PIE");
                }
                sb.Append('\n');
                sb.Append(_estadosAventureros[aventurerosOrdenados[i].Id].Log);
                sb.Append("\n\n");
            }
            return sb.ToString();
        }
    }
}
