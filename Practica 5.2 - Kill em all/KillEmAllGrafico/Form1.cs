using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KillEmAll
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
            ArenaGrafica arena = new ArenaGrafica(PanelArena, BotonSiguienteTurno, _botonAvanzarNTurnos, _turnosAvanzar, BotonReset, TextoLog, labelTurno);
        }

    }
}
