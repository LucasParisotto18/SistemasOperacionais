namespace Projeto
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMenu = new System.Windows.Forms.Panel();
            this.buttonConectar = new System.Windows.Forms.Button();
            this.buttonReal = new System.Windows.Forms.Button();
            this.buttonSimulador = new System.Windows.Forms.Button();
            this.panelCorpo = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.Controls.Add(this.buttonConectar);
            this.panelMenu.Controls.Add(this.buttonReal);
            this.panelMenu.Controls.Add(this.buttonSimulador);
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(157, 611);
            this.panelMenu.TabIndex = 0;
            this.panelMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // buttonConectar
            // 
            this.buttonConectar.Location = new System.Drawing.Point(21, 337);
            this.buttonConectar.Name = "buttonConectar";
            this.buttonConectar.Size = new System.Drawing.Size(116, 49);
            this.buttonConectar.TabIndex = 2;
            this.buttonConectar.Text = "Remoto";
            this.buttonConectar.UseVisualStyleBackColor = true;
            this.buttonConectar.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonReal
            // 
            this.buttonReal.Location = new System.Drawing.Point(21, 259);
            this.buttonReal.Name = "buttonReal";
            this.buttonReal.Size = new System.Drawing.Size(116, 49);
            this.buttonReal.TabIndex = 1;
            this.buttonReal.Text = "Real";
            this.buttonReal.UseVisualStyleBackColor = true;
            this.buttonReal.Click += new System.EventHandler(this.buttonReal_Click);
            // 
            // buttonSimulador
            // 
            this.buttonSimulador.Location = new System.Drawing.Point(21, 176);
            this.buttonSimulador.Name = "buttonSimulador";
            this.buttonSimulador.Size = new System.Drawing.Size(116, 52);
            this.buttonSimulador.TabIndex = 0;
            this.buttonSimulador.Text = "Simulador";
            this.buttonSimulador.UseVisualStyleBackColor = true;
            this.buttonSimulador.Click += new System.EventHandler(this.buttonSimuladdor_Click);
            // 
            // panelCorpo
            // 
            this.panelCorpo.Location = new System.Drawing.Point(163, 0);
            this.panelCorpo.Name = "panelCorpo";
            this.panelCorpo.Size = new System.Drawing.Size(1151, 611);
            this.panelCorpo.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 611);
            this.Controls.Add(this.panelCorpo);
            this.Controls.Add(this.panelMenu);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button buttonReal;
        private System.Windows.Forms.Button buttonSimulador;
        private System.Windows.Forms.Panel panelCorpo;
        private System.Windows.Forms.Button buttonConectar;
    }
}

