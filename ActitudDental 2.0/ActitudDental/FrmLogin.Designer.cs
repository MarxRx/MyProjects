
namespace ActitudDental
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.PnlBg = new System.Windows.Forms.Panel();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtUsr = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PnlBg
            // 
            this.PnlBg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PnlBg.BackgroundImage")));
            this.PnlBg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PnlBg.Location = new System.Drawing.Point(0, 0);
            this.PnlBg.Name = "PnlBg";
            this.PnlBg.Size = new System.Drawing.Size(175, 175);
            this.PnlBg.TabIndex = 7;
            // 
            // txtPass
            // 
            this.txtPass.BackColor = System.Drawing.SystemColors.Window;
            this.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPass.ForeColor = System.Drawing.Color.Blue;
            this.txtPass.Location = new System.Drawing.Point(304, 73);
            this.txtPass.Margin = new System.Windows.Forms.Padding(6);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(216, 21);
            this.txtPass.TabIndex = 12;
            this.txtPass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPass.UseSystemPasswordChar = true;
            // 
            // txtUsr
            // 
            this.txtUsr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsr.ForeColor = System.Drawing.Color.Blue;
            this.txtUsr.Location = new System.Drawing.Point(304, 40);
            this.txtUsr.Margin = new System.Windows.Forms.Padding(6);
            this.txtUsr.Name = "txtUsr";
            this.txtUsr.Size = new System.Drawing.Size(216, 21);
            this.txtUsr.TabIndex = 11;
            this.txtUsr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.White;
            this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.DarkMagenta;
            this.btnLogin.FlatAppearance.BorderSize = 2;
            this.btnLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightPink;
            this.btnLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Wheat;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.ForeColor = System.Drawing.Color.DarkMagenta;
            this.btnLogin.Location = new System.Drawing.Point(184, 107);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(342, 35);
            this.btnLogin.TabIndex = 10;
            this.btnLogin.Text = "Entrar";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.LavenderBlush;
            this.label2.Location = new System.Drawing.Point(187, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 24);
            this.label2.TabIndex = 9;
            this.label2.Text = "Contraseña:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.LavenderBlush;
            this.label1.Location = new System.Drawing.Point(187, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "Usuario:";
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkMagenta;
            this.ClientSize = new System.Drawing.Size(532, 172);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtUsr);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PnlBg);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.Text = "Actitud Dental - Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PnlBg;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtUsr;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}