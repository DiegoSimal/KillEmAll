namespace KillEmAll
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PanelArena = new System.Windows.Forms.Panel();
            BotonSiguienteTurno = new System.Windows.Forms.Button();
            TextoLog = new System.Windows.Forms.RichTextBox();
            label1 = new System.Windows.Forms.Label();
            labelTurno = new System.Windows.Forms.Label();
            _botonAvanzarNTurnos = new System.Windows.Forms.Button();
            _turnosAvanzar = new System.Windows.Forms.NumericUpDown();
            BotonReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)_turnosAvanzar).BeginInit();
            SuspendLayout();
            // 
            // PanelArena
            // 
            PanelArena.Location = new System.Drawing.Point(12, 12);
            PanelArena.Name = "PanelArena";
            PanelArena.Size = new System.Drawing.Size(800, 800);
            PanelArena.TabIndex = 0;
            // 
            // BotonSiguienteTurno
            // 
            BotonSiguienteTurno.Location = new System.Drawing.Point(892, 58);
            BotonSiguienteTurno.Name = "BotonSiguienteTurno";
            BotonSiguienteTurno.Size = new System.Drawing.Size(129, 53);
            BotonSiguienteTurno.TabIndex = 1;
            BotonSiguienteTurno.Text = "Siguiente turno";
            BotonSiguienteTurno.UseVisualStyleBackColor = true;
            // 
            // TextoLog
            // 
            TextoLog.Location = new System.Drawing.Point(851, 134);
            TextoLog.Name = "TextoLog";
            TextoLog.Size = new System.Drawing.Size(509, 607);
            TextoLog.TabIndex = 3;
            TextoLog.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(1183, 58);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 15);
            label1.TabIndex = 4;
            label1.Text = "Turno:";
            // 
            // labelTurno
            // 
            labelTurno.AutoSize = true;
            labelTurno.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            labelTurno.Location = new System.Drawing.Point(1183, 77);
            labelTurno.Name = "labelTurno";
            labelTurno.Size = new System.Drawing.Size(40, 17);
            labelTurno.TabIndex = 5;
            labelTurno.Text = "label2";
            // 
            // _botonAvanzarNTurnos
            // 
            _botonAvanzarNTurnos.Location = new System.Drawing.Point(1036, 58);
            _botonAvanzarNTurnos.Name = "_botonAvanzarNTurnos";
            _botonAvanzarNTurnos.Size = new System.Drawing.Size(129, 53);
            _botonAvanzarNTurnos.TabIndex = 1;
            _botonAvanzarNTurnos.Text = "Avanzar N turnos";
            _botonAvanzarNTurnos.UseVisualStyleBackColor = true;
            // 
            // _turnosAvanzar
            // 
            _turnosAvanzar.Location = new System.Drawing.Point(1036, 29);
            _turnosAvanzar.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _turnosAvanzar.Name = "_turnosAvanzar";
            _turnosAvanzar.Size = new System.Drawing.Size(82, 23);
            _turnosAvanzar.TabIndex = 6;
            _turnosAvanzar.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // BotonReset
            // 
            BotonReset.Location = new System.Drawing.Point(1231, 58);
            BotonReset.Name = "BotonReset";
            BotonReset.Size = new System.Drawing.Size(129, 53);
            BotonReset.TabIndex = 1;
            BotonReset.Text = "Recomenzar combate";
            BotonReset.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1370, 750);
            Controls.Add(_turnosAvanzar);
            Controls.Add(labelTurno);
            Controls.Add(label1);
            Controls.Add(TextoLog);
            Controls.Add(BotonReset);
            Controls.Add(_botonAvanzarNTurnos);
            Controls.Add(BotonSiguienteTurno);
            Controls.Add(PanelArena);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)_turnosAvanzar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel PanelArena;
        private System.Windows.Forms.Button BotonSiguienteTurno;
        private System.Windows.Forms.RichTextBox TextoLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTurno;
        private System.Windows.Forms.Button _botonAvanzarNTurnos;
        private System.Windows.Forms.NumericUpDown _turnosAvanzar;
        private System.Windows.Forms.Button BotonReset;
    }
}
