using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using KillEmAlGrafico;

namespace KillEmAll
{
    public class AventureroGrafico:Panel
    {
        private const float ESCALA_CONO_DE_VISION = 0.5f;
        private const int TRANSPARENCIA_HUD = 128;
        private Label _etiqueta;
        private PictureBox _icono;
        private Bitmap _conoDeVisionOriginal=KillEmAllGrafico.Properties.Resources.cono_de_vision;
        private Dictionary<Pose, Bitmap> _spritesheet;
        private Graphics _gPantalla;
        private int _vidaMaxima;


        public AventureroGrafico(string nombre, int vida, Graphics gPantalla)
        {
            _vidaMaxima = vida;
            _gPantalla = gPantalla;
            _spritesheet = GestorDeSpritesheets.Instance.ElegirSpritesheetAleatoria();
            _etiqueta = new Label();
            _etiqueta.Text = nombre;
            _etiqueta.BackColor = Color.Transparent;
            _etiqueta.ForeColor = Color.Black;
            _etiqueta.Location = new Point(0, 20);
            BackColor = Color.Transparent;
            Size = new Size(200, 200);
            _icono = new PictureBox();
            _icono.SizeMode = PictureBoxSizeMode.StretchImage;
            _icono.BackColor = Color.Transparent;
            _icono.Size = Size;
            Controls.Add(_icono);
            
            
        }

        public void ActualizarOrientacion(Point posicion, int orientacion, int vida) {
            if (vida > 0)
            {
                _gPantalla.TranslateTransform(posicion.X, posicion.Y);
                //Se dibuja la vida
                SolidBrush brush = ElegirColor(vida);
                _gPantalla.FillRectangle(brush, new Rectangle(-50, -50, vida, 20));

                //Se dibuja el nombre
                Bitmap aux = new Bitmap(_etiqueta.Width, _etiqueta.Height);
                _etiqueta.DrawToBitmap(aux, new Rectangle(0, 0, _etiqueta.Width, _etiqueta.Height));
                _gPantalla.DrawImage(aux, new Rectangle(-50, -30, _etiqueta.Width, _etiqueta.Height));

                //Se dibuja el cono de visión
                float anchoSprite = 40;
                float altoSprite = 40;
                float offsetX = -anchoSprite / 2;
                float offsetY = -3 * altoSprite / 8;
                _gPantalla.DrawImage(ElegirSprite(orientacion), offsetX, offsetY, anchoSprite, altoSprite);
                _gPantalla.RotateTransform(-orientacion - 45);
                float ancho = _conoDeVisionOriginal.Width * ESCALA_CONO_DE_VISION;
                float alto = _conoDeVisionOriginal.Height * ESCALA_CONO_DE_VISION;
                _gPantalla.DrawImage(_conoDeVisionOriginal, 0, 0, ancho, alto);
                _gPantalla.RotateTransform(orientacion + 45);


                _gPantalla.TranslateTransform(-posicion.X, -posicion.Y);
            }
        }

        private SolidBrush ElegirColor(int vida) {
            SolidBrush brush;
            float fraccionVida = (float)vida / _vidaMaxima;
            if (fraccionVida <= 0.25f) {
                brush = new SolidBrush(Color.FromArgb(TRANSPARENCIA_HUD, 255,0,0));
            }
            else if (fraccionVida <= 0.5f) {
                brush = new SolidBrush(Color.FromArgb(TRANSPARENCIA_HUD, 255, 255, 0));
            }
            else {
                brush = new SolidBrush(Color.FromArgb(TRANSPARENCIA_HUD, 0, 255, 0));
            }
            return brush;
        }

        private Bitmap ElegirSprite(int orientacion) {
            Bitmap sprite;
            if (orientacion>315 || orientacion < 45) {
                sprite = _spritesheet[Pose.Derecha];
            }
            else if (orientacion < 135) {
                sprite = _spritesheet[Pose.Arriba];
            }
            else if (orientacion < 225) {
                sprite = _spritesheet[Pose.Izquierda];
            }
            else {
                sprite = _spritesheet[Pose.Abajo];
            }
            return sprite;
        }
    }
}
