using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Design;

namespace KillEmAlGrafico
{
    public enum Pose {AbajoCaminar1=0,Abajo,AbajoCaminar2,IzquierdaCaminar1,Izquierda,IzquierdaCaminar2,DerechaCaminar1,Derecha,DerechaCaminar2,ArribaCaminar1,Arriba,ArribaCaminar2};

    internal class GestorDeSpritesheets
    {
        public static GestorDeSpritesheets _instance;
        public const int ANCHO = 80;
        private List<Dictionary<Pose,Bitmap>> _bitmaps;
        private List<int> _bitmapsUtilizados; 


        private GestorDeSpritesheets() {
            _bitmaps = new List<Dictionary<Pose, Bitmap>>();
            _bitmapsUtilizados = new List<int>();
        }

        public static GestorDeSpritesheets Instance {
            get {
                if (_instance== null) {
                    _instance=new GestorDeSpritesheets();
                    _instance.RellenarBitmaps();
                }
                return _instance;
            }
        }

        private void RellenarBitmaps() {
            bool salir = false;
            int contador = 1;
            
            while (!salir) {
                Bitmap spritesheet = (Bitmap)KillEmAllGrafico.Properties.Resources.ResourceManager.GetObject($"spritesheet ({contador})");
                ++contador;
                if (spritesheet != null ) {
                   _bitmaps.Add(ExtraerBitmaps(spritesheet));
                }
                else {
                    salir = true;
                }
            }
        }

        private Dictionary<Pose,Bitmap> ExtraerBitmaps(Bitmap spritesheet) {
            Dictionary<Pose,Bitmap> resultado = new Dictionary<Pose,Bitmap>();
            foreach(Pose pose in Enum.GetValues(typeof(Pose))) {
                resultado[pose]=ExtraerBitmap(pose,spritesheet);
            }
            return resultado;
        }


        private Bitmap ExtraerBitmap(Pose pose, Bitmap bitmap) {
            Point[] esquinas = ObtenerEsquinas(pose, bitmap);
            float aspectRatio = CalcularAspectRatio(bitmap);
            Bitmap resultado = new Bitmap(ANCHO, (int)(ANCHO / aspectRatio));
            Graphics g = Graphics.FromImage(resultado);
            g.DrawImage(bitmap, new Rectangle(0, 0, resultado.Width, resultado.Height), esquinas[0].X, esquinas[0].Y, esquinas[1].X, esquinas[1].Y, GraphicsUnit.Pixel);
            return resultado;
        }

        private float CalcularAspectRatio(Bitmap bitmap) {
            return (bitmap.Width / 3f) / (bitmap.Height / 4f);
        }

        private Point[] ObtenerEsquinas(Pose pose, Bitmap bitmap) {
            Point[] resultado = new Point[2];
            resultado[0] = new Point(bitmap.Width / 3 * ((int)pose % 3), bitmap.Height / 4 * ((int)pose / 3));
            resultado[1] = new Point(bitmap.Width / 3, bitmap.Height / 4);
            return resultado;
        }

        public Dictionary<Pose,Bitmap> ElegirSpritesheetAleatoria() {
            int id;
            do {
                id = new Random().Next(0, _bitmaps.Count);
            } while (_bitmapsUtilizados.Contains(id));

            _bitmapsUtilizados.Add(id);
            return _bitmaps[id];
        }
    }
}
