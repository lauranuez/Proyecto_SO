namespace v1
{
    partial class PartidasJugadas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.query = new System.Windows.Forms.Button();
            this.id2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.Jugador = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Partidastiempo = new System.Windows.Forms.RadioButton();
            this.ResultadosPartida = new System.Windows.Forms.RadioButton();
            this.JugadoresPartida = new System.Windows.Forms.RadioButton();
            this.Conectadoslbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // query
            // 
            this.query.Location = new System.Drawing.Point(368, 232);
            this.query.Margin = new System.Windows.Forms.Padding(4);
            this.query.Name = "query";
            this.query.Size = new System.Drawing.Size(117, 28);
            this.query.TabIndex = 63;
            this.query.Text = "CONSULTAR";
            this.query.UseVisualStyleBackColor = true;
            this.query.Click += new System.EventHandler(this.query_Click);
            // 
            // id2
            // 
            this.id2.Location = new System.Drawing.Point(139, 124);
            this.id2.Margin = new System.Windows.Forms.Padding(4);
            this.id2.Name = "id2";
            this.id2.Size = new System.Drawing.Size(132, 22);
            this.id2.TabIndex = 62;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(57, 124);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 17);
            this.label12.TabIndex = 61;
            this.label12.Text = "Tiempo:";
            // 
            // Jugador
            // 
            this.Jugador.Location = new System.Drawing.Point(139, 209);
            this.Jugador.Margin = new System.Windows.Forms.Padding(4);
            this.Jugador.Name = "Jugador";
            this.Jugador.Size = new System.Drawing.Size(132, 22);
            this.Jugador.TabIndex = 60;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(57, 209);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 17);
            this.label13.TabIndex = 59;
            this.label13.Text = "Nombre:";
            // 
            // Partidastiempo
            // 
            this.Partidastiempo.AutoSize = true;
            this.Partidastiempo.Location = new System.Drawing.Point(39, 79);
            this.Partidastiempo.Margin = new System.Windows.Forms.Padding(4);
            this.Partidastiempo.Name = "Partidastiempo";
            this.Partidastiempo.Size = new System.Drawing.Size(398, 21);
            this.Partidastiempo.TabIndex = 58;
            this.Partidastiempo.TabStop = true;
            this.Partidastiempo.Text = "Listado de partidas jugadas en un periodo de tiempo dado";
            this.Partidastiempo.UseVisualStyleBackColor = true;
            // 
            // ResultadosPartida
            // 
            this.ResultadosPartida.AutoSize = true;
            this.ResultadosPartida.Location = new System.Drawing.Point(39, 158);
            this.ResultadosPartida.Margin = new System.Windows.Forms.Padding(4);
            this.ResultadosPartida.Name = "ResultadosPartida";
            this.ResultadosPartida.Size = new System.Drawing.Size(282, 21);
            this.ResultadosPartida.TabIndex = 57;
            this.ResultadosPartida.TabStop = true;
            this.ResultadosPartida.Text = "Resultados de las partidas jugadas con:";
            this.ResultadosPartida.UseVisualStyleBackColor = true;
            // 
            // JugadoresPartida
            // 
            this.JugadoresPartida.AutoSize = true;
            this.JugadoresPartida.Location = new System.Drawing.Point(39, 34);
            this.JugadoresPartida.Margin = new System.Windows.Forms.Padding(4);
            this.JugadoresPartida.Name = "JugadoresPartida";
            this.JugadoresPartida.Size = new System.Drawing.Size(339, 21);
            this.JugadoresPartida.TabIndex = 56;
            this.JugadoresPartida.TabStop = true;
            this.JugadoresPartida.Text = "Jugadores con los que he echado alguna partida";
            this.JugadoresPartida.UseVisualStyleBackColor = true;
            // 
            // Conectadoslbl
            // 
            this.Conectadoslbl.AutoEllipsis = true;
            this.Conectadoslbl.AutoSize = true;
            this.Conectadoslbl.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Conectadoslbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Conectadoslbl.Location = new System.Drawing.Point(39, 276);
            this.Conectadoslbl.Name = "Conectadoslbl";
            this.Conectadoslbl.Size = new System.Drawing.Size(14, 19);
            this.Conectadoslbl.TabIndex = 64;
            this.Conectadoslbl.Text = " ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 17);
            this.label1.TabIndex = 65;
            this.label1.Text = "Escribe una fecha separada por \"/\"";
            // 
            // PartidasJugadas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(1, 1);
            this.AutoScrollMinSize = new System.Drawing.Size(1, 1);
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(498, 482);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Conectadoslbl);
            this.Controls.Add(this.query);
            this.Controls.Add(this.id2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.Jugador);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.Partidastiempo);
            this.Controls.Add(this.ResultadosPartida);
            this.Controls.Add(this.JugadoresPartida);
            this.Name = "PartidasJugadas";
            this.Text = "PartidasJugadas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button query;
        private System.Windows.Forms.TextBox id2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox Jugador;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton Partidastiempo;
        private System.Windows.Forms.RadioButton ResultadosPartida;
        private System.Windows.Forms.RadioButton JugadoresPartida;
        private System.Windows.Forms.Label Conectadoslbl;
        private System.Windows.Forms.Label label1;
    }
}