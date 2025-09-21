using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace KillEmAll
{
    internal class ArenaGrafica
    {
        private Panel _panelArena;
        private Button _botonSiguienteTurno;
        private Button _botonAvanzarNTurnos;
        private NumericUpDown _turnosAvanzar;
        private Button _botonReset;
        private RichTextBox _textoLog;
        private Label _labelTurno;
        private Arena _arena;
        private GraficadorDeArena _graficador;
        private int turno;


        public ArenaGrafica(Panel panelArena, Button botonSiguienteTurno, Button botonAvanzarNTurnos, NumericUpDown turnosAvanzar, Button botonReset, RichTextBox textoLog, Label labelTurno) {
            _labelTurno = labelTurno;
            _panelArena = panelArena;
            _textoLog = textoLog;
            _botonSiguienteTurno = botonSiguienteTurno;
            _botonAvanzarNTurnos = botonAvanzarNTurnos;
            _turnosAvanzar = turnosAvanzar;
            _botonReset = botonReset;
            _botonReset.Click += Reset;
            Reset(null,null);
        }

        public void SiguienteTurno(object sender, EventArgs e) {
            ++turno;
            _labelTurno.Text = turno.ToString();
            if (_arena.CombateTerminado)
            {
                _textoLog.Text = _arena.GetInformeFinal();
                _botonSiguienteTurno.Click -= SiguienteTurno;
                _botonAvanzarNTurnos.Click -= AvanzarNTurnos;
            }
            else
            {
                Dictionary<int, EstadoAventurero> estados = _arena.RealizarTurno();
                _textoLog.Text = _arena.GetInformeDeAventureros();
                _graficador.MostrarCambios(estados);
            }
        }

        public void AvanzarNTurnos(object sender, EventArgs e)
        {
            int turnosAAvanzar = (int)_turnosAvanzar.Value;
            for (int i = 0; i < turnosAAvanzar && !_arena.CombateTerminado; ++i)
            {
                SiguienteTurno(null, null);
            }
            if (_arena.CombateTerminado)
            {
                SiguienteTurno(null, null);
            }
        }

        public void Reset(object sender, EventArgs e) {
            turno = 0;
            _labelTurno.Text = "0";
            if (_arena==null || _arena.CombateTerminado)
            {
                _botonSiguienteTurno.Click += SiguienteTurno;
                _botonAvanzarNTurnos.Click += AvanzarNTurnos;
            }
            Dictionary<int, Aventurero> aventureros = GeneradorDeAventureros.GenerarAventureros();
            Dictionary<int, EstadoAventurero> estados = GeneradorDeAventureros.GenerarEstadosIniciales(aventureros);
            _arena = new Arena(aventureros, estados);
            _textoLog.Text = _arena.GetInformeDeAventureros();
            if (_graficador!=null) _graficador.Eliminar();
            _graficador = new GraficadorDeArena(_panelArena, aventureros);
            _graficador.MostrarCambios(estados);
        }
    }
}
