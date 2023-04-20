using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActitudDental
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        public void btnLogin_Click(object sender, EventArgs e)
        {
            if(txtUsr.Text=="ad" && txtPass.Text == "ad.123")
            {
                VariablesGlobales.UsuarioActivo = txtUsr.Text;
                VariablesGlobales.RolActivo = "Master";
                VariablesGlobales.IdActivo = 0;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Usuario o contraseña esta equivocado");
            }
        }
    }
}
