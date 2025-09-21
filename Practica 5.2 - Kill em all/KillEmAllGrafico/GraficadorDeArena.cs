using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KillEmAll;
using System.Drawing;

namespace KillEmAll
{
    public class GraficadorDeArena
    {
        private Dictionary<int, AventureroGrafico> _iconos;
        private Size _tamanhoPanel;
        private PictureBox _pantalla;
        private Bitmap _bitmap;
        private Graphics _gPantalla;
        private Panel _panel;

        public GraficadorDeArena(Panel panel, Dictionary<int,Aventurero> aventureros)
        {
            _tamanhoPanel = panel.Size;
            _pantalla = new PictureBox();
            _pantalla.Size = panel.Size;
            _panel = panel;
            _panel.Controls.Add(_pantalla);
            _bitmap = new Bitmap(_pantalla.Width, _pantalla.Height);
            _gPantalla = Graphics.FromImage(_bitmap);
            _iconos = new Dictionary<int, AventureroGrafico>();
            foreach(KeyValuePair<int, Aventurero> par in aventureros)
            {
                AventureroGrafico ag = new AventureroGrafico(par.Value.Nombre, ReglasDelJuego.s_vidaPorClase[par.Value.Clase], _gPantalla);
                _iconos.Add(par.Key, ag);
            }
        }

        public void MostrarCambios(Dictionary<int, EstadoAventurero> estados) {
            BorrarPantalla();
            foreach (KeyValuePair<int, EstadoAventurero> par in estados) {
                AventureroGrafico ag = _iconos[par.Key];
                ag.ActualizarOrientacion(new Point(par.Value.Posicion.X, _tamanhoPanel.Height - par.Value.Posicion.Y), par.Value.Orientacion, par.Value.Vida);
            }
            _pantalla.Image = _bitmap;
        }

        public void BorrarPantalla()
        {
            _gPantalla.Clear(Color.Transparent);
            //_pantalla.Image = _bitmap;
        }

        public void Eliminar()
        {
            //BorrarPantalla();
            _panel.Controls.Remove(_pantalla);
        }
    }
}
