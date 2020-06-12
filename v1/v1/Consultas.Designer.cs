namespace v1
{
    partial class Consultas
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
            this.label14 = new System.Windows.Forms.Label();
            this.query = new System.Windows.Forms.Button();
            this.id2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.id1 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.partida = new System.Windows.Forms.RadioButton();
            this.NombresGanadores = new System.Windows.Forms.RadioButton();
            this.NombresPartida = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(201, 38);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(84, 20);
            this.label14.TabIndex = 56;
            this.label14.Text = "QUERYS";
            // 
            // query
            // 
            this.query.Location = new System.Drawing.Point(301, 394);
            this.query.Margin = new System.Windows.Forms.Padding(4);
            this.query.Name = "query";
            this.query.Size = new System.Drawing.Size(117, 28);
            this.query.TabIndex = 55;
            this.query.Text = "CONSULTAR";
            this.query.UseVisualStyleBackColor = true;
            this.query.Click += new System.EventHandler(this.query_Click);
            // 
            // id2
            // 
            this.id2.Location = new System.Drawing.Point(124, 345);
            this.id2.Margin = new System.Windows.Forms.Padding(4);
            this.id2.Name = "id2";
            this.id2.Size = new System.Drawing.Size(132, 22);
            this.id2.TabIndex = 54;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(65, 348);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 17);
            this.label12.TabIndex = 53;
            this.label12.Text = "ID:";
            // 
            // id1
            // 
            this.id1.Location = new System.Drawing.Point(124, 140);
            this.id1.Margin = new System.Windows.Forms.Padding(4);
            this.id1.Name = "id1";
            this.id1.Size = new System.Drawing.Size(132, 22);
            this.id1.TabIndex = 52;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(65, 144);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(25, 17);
            this.label13.TabIndex = 51;
            this.label13.Text = "ID:";
            // 
            // partida
            // 
            this.partida.AutoSize = true;
            this.partida.Location = new System.Drawing.Point(64, 298);
            this.partida.Margin = new System.Windows.Forms.Padding(4);
            this.partida.Name = "partida";
            this.partida.Size = new System.Drawing.Size(271, 21);
            this.partida.TabIndex = 50;
            this.partida.TabStop = true;
            this.partida.Text = "Dame los datos de la siguiente partida";
            this.partida.UseVisualStyleBackColor = true;
            // 
            // NombresGanadores
            // 
            this.NombresGanadores.AutoSize = true;
            this.NombresGanadores.Location = new System.Drawing.Point(64, 215);
            this.NombresGanadores.Margin = new System.Windows.Forms.Padding(4);
            this.NombresGanadores.Name = "NombresGanadores";
            this.NombresGanadores.Size = new System.Drawing.Size(232, 21);
            this.NombresGanadores.TabIndex = 49;
            this.NombresGanadores.TabStop = true;
            this.NombresGanadores.Text = "Muestrame todos los ganadores";
            this.NombresGanadores.UseVisualStyleBackColor = true;
            // 
            // NombresPartida
            // 
            this.NombresPartida.AutoSize = true;
            this.NombresPartida.Location = new System.Drawing.Point(39, 89);
            this.NombresPartida.Margin = new System.Windows.Forms.Padding(4);
            this.NombresPartida.Name = "NombresPartida";
            this.NombresPartida.Size = new System.Drawing.Size(414, 21);
            this.NombresPartida.TabIndex = 48;
            this.NombresPartida.TabStop = true;
            this.NombresPartida.Text = "Muestra los nombres de los jugadores de la siguiente partida";
            this.NombresPartida.UseVisualStyleBackColor = true;
            // 
            // Consultas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 447);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.query);
            this.Controls.Add(this.id2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.id1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.partida);
            this.Controls.Add(this.NombresGanadores);
            this.Controls.Add(this.NombresPartida);
            this.Name = "Consultas";
            this.Text = "Consultas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button query;
        private System.Windows.Forms.TextBox id2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox id1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton partida;
        private System.Windows.Forms.RadioButton NombresGanadores;
        private System.Windows.Forms.RadioButton NombresPartida;
    }
}