using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace ActitudDental
{
    public partial class FrmDental : Form
    {
        int renglonHeight = 32;
        SqlConnection conDB = new SqlConnection(ConfigurationManager.ConnectionStrings["ActitudDental.Properties.Settings.ActitudDentalLocalDBConnectionString"].ConnectionString);
        public FrmDental()
        {
            InitializeComponent();
            lblHeader.Text = "Actitud Dental App";
        }
        private void FrmDental_Load(object sender, EventArgs e)
        {
            lblUsr.Text = VariablesGlobales.UsuarioActivo;
            if (VariablesGlobales.UsuarioActivo == "")
            {
                MessageBox.Show("No hay usuario logueado");
                Application.Exit();
            }
            RegistraAcceso("Actitud Dental App");
        }

        //LISTO: Procedimiento para Catalogo de Visitas
        private void btnRegistrarVisitadePx_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnRegistrarVisitadePx.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevaVisita = topMenuButtonAdd("Nuevo", 0);
            btnNuevaVisita.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnRegistrarVisitadePx.Text + " > " + btnNuevaVisita.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                string FiltroPx = "";
                string SelectedIdPx = "0";
                string[] cbxDescPx;

                var lblPx = panelLabelAdd("Paciente:", 0, 0);
                var txtPx = panelTextAdd("", 0, 100, 140, 1, HorizontalAlignment.Left);
                var btnBuscaPx = panelFindButtonAdd(0, 250);
                btnBuscaPx.Click += delegate
                {
                    btnBuscaPx.Enabled = false;
                    txtPx.Enabled = false;
                    if (txtPx.Text != "")
                    {
                        FiltroPx = "pacientes.activo = 'ACTIVO' AND pacientes.nombre LIKE '%" + txtPx.Text.Trim() + "%'";
                    }
                    else
                    {
                        FiltroPx = "pacientes.activo = 'ACTIVO'";
                    }
                    var cbxPx = panelComboBoxPacientes(FiltroPx, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxPx);
                    cbxPx.SelectedIndexChanged += delegate
                    {
                        cbxPx.Enabled = false;
                        cbxDescPx = cbxPx.Text.Split(' ');
                        SelectedIdPx = cbxDescPx[0].ToString();
                        string FiltroPpt = "";
                        string SelectedIdPpt = "0";
                        string[] cbxDescPpt;

                        var lblPpt = panelLabelAdd("Presupuestos:", 1, 0);
                        FiltroPpt = "Presupuestos.Estatus = 'Vigente' AND Presupuestos.idPx = " + SelectedIdPx.ToString();
                        var cbxPpt = panelComboBoxPresupuestos(FiltroPpt, 1, 300, 450);
                        this.PnlNavegacion.Controls.Add(cbxPpt);
                        cbxPpt.SelectedIndexChanged += delegate
                        {
                            cbxPpt.Enabled = false;
                            cbxDescPpt = cbxPpt.Text.Split(' ');
                            SelectedIdPpt = cbxDescPpt[0].ToString();
                            var FiltroEsp = "";

                            //if (cbxPpt.Text.Split('|')[1].ToString() != "")
                            //{
                            //    FiltroEsp = "Especialistas.activo = 'Activo' AND Especialistas.especialidad LIKE '%" + cbxPpt.Text.Split('|')[1].ToString() + "%'";
                            //}
                            //else
                            //{
                            //    FiltroEsp = "Especialistas.activo = 'Activo'";
                            //}

                            FiltroEsp = "Especialistas.activo = 'Activo'";
                            var lblEsp = panelLabelAdd("Especialistas:", 2, 0);
                            var cbxEsp = panelComboBoxEspecialistas(FiltroEsp, 2, 300, 450);
                            
                            var cbxActivo = panelComboBoxAdd(3, 250, 100);
                            cbxActivo.Items.Add("Activo");
                            cbxActivo.Items.Add("Cancelado");
                            cbxActivo.SelectedIndex = 0;
                            var lblDia = panelLabelAdd("Dia/Mes/Año", 3, 370);
                            var txtDia = panelTextAdd("", 3, 470, 40, 1, HorizontalAlignment.Center);
                            var txtMes = panelTextAdd("", 3, 530, 40, 1, HorizontalAlignment.Center);
                            var txtAnio = panelTextAdd("", 3, 590, 70, 1, HorizontalAlignment.Center);
                            var lblNotas = panelLabelAdd("Notas:", 4, 0);
                            var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);
                            txtDia.Text = DateTime.Now.Day.ToString();
                            txtMes.Text = DateTime.Now.Month.ToString();
                            txtAnio.Text = DateTime.Now.Year.ToString();

                            this.PnlNavegacion.Controls.Add(lblEsp);
                            this.PnlNavegacion.Controls.Add(cbxEsp);
                            this.PnlNavegacion.Controls.Add(cbxActivo);
                            this.PnlNavegacion.Controls.Add(lblDia);
                            this.PnlNavegacion.Controls.Add(txtDia);
                            this.PnlNavegacion.Controls.Add(txtMes);
                            this.PnlNavegacion.Controls.Add(txtAnio);
                            this.PnlNavegacion.Controls.Add(lblNotas);
                            this.PnlNavegacion.Controls.Add(txtNotas);
                            var btnGuardaPpto = panelButtonAdd("Guardar", 8, 1, 0);
                            btnGuardaPpto.Click += delegate
                            {
                                string MsgError = "";
                                if (LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text)) { MsgError = "La fecha esta mal."; };
                                if (MsgError != "") { MessageBox.Show(MsgError); }
                                try
                                {
                                    var auxEsp = 0;
                                    if(cbxEsp.Text == "")
                                    {
                                        auxEsp = 0;
                                    }
                                    else
                                    {
                                        auxEsp = int.Parse(cbxEsp.Text.Split(' ')[0]);
                                    }
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_insert_visita";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@idPpto", int.Parse(cbxPpt.Text.Split(' ')[0]));
                                    cmd.Parameters.AddWithValue("@idEsp", auxEsp);
                                    cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                    cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                    cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                    cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                    cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                    SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                                    r.Direction = ParameterDirection.ReturnValue;
                                    r.Value = 0;
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    var idEsp = r.Value;
                                    conDB.Close();
                                    MessageBox.Show("Visita Registrada");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            };
                            this.PnlNavegacion.Controls.Add(btnGuardaPpto);
                        };
                        this.PnlNavegacion.Controls.Add(lblPpt);
                    };
                };
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPx);
                this.PnlNavegacion.Controls.Add(btnBuscaPx);
            };

            var btnModificaVis = topMenuButtonAdd("Modificar", 1);
            btnModificaVis.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnRegistrarVisitadePx.Text + " > " + btnModificaVis.Text;
                RegistraAcceso(lblHeader.Text);
                string Filtro = "";
                string SelectedIdVis = "0";
                string[] cbxDesc;

                var lblVis = panelLabelAdd("Buscar:", 0, 0);
                var txtVisFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0, 250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtVisFiltro.Enabled = false;
                    if (txtVisFiltro.Text != "")
                    {
                        Filtro = "V.IdVisita = " + txtVisFiltro.Text.Trim();
                    }
                    DataTable tblVis = getTablaVisitas(Filtro);
                    var cbxVis = panelComboBoxVisitas(Filtro, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxVis);
                    cbxVis.SelectedIndexChanged += delegate
                    {
                        cbxVis.Enabled = false;
                        var txtIdVis = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdVis.Enabled = false;
                        var txtPx = panelTextAdd("", 1, 100, 200, 1, HorizontalAlignment.Left);
                        txtPx.Enabled = false;
                        var txtTra = panelTextAdd("", 1, 300, 200, 1, HorizontalAlignment.Left);
                        txtTra.Enabled = false;
                        var txtEsp = panelTextAdd("", 2, 100, 200, 1, HorizontalAlignment.Left);
                        txtEsp.Enabled = false;

                        var cbxActivo = panelComboBoxAdd(3, 250, 100);
                        cbxActivo.Items.Add("Activo");
                        cbxActivo.Items.Add("Cancelado");
                        cbxActivo.SelectedIndex = 0;
                        var lblDia = panelLabelAdd("Dia/Mes/Año", 3, 370);
                        var txtDia = panelTextAdd("", 3, 470, 40, 1, HorizontalAlignment.Center);
                        var txtMes = panelTextAdd("", 3, 530, 40, 1, HorizontalAlignment.Center);
                        var txtAnio = panelTextAdd("", 3, 590, 70, 1, HorizontalAlignment.Center);
                        var lblNotas = panelLabelAdd("Notas:", 4, 0);
                        var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);

                        this.PnlNavegacion.Controls.Add(txtIdVis);
                        this.PnlNavegacion.Controls.Add(txtPx);
                        this.PnlNavegacion.Controls.Add(txtTra);
                        this.PnlNavegacion.Controls.Add(txtEsp);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(lblDia);
                        this.PnlNavegacion.Controls.Add(txtDia);
                        this.PnlNavegacion.Controls.Add(txtMes);
                        this.PnlNavegacion.Controls.Add(txtAnio);
                        this.PnlNavegacion.Controls.Add(lblNotas);
                        this.PnlNavegacion.Controls.Add(txtNotas);
                        cbxDesc = cbxVis.Text.Split(' ');
                        SelectedIdVis = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblVis.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdVis)
                            {
                                txtIdVis.Text = dr[0].ToString();
                                txtPx.Text = dr[1].ToString();
                                txtTra.Text = dr[2].ToString();
                                txtEsp.Text = dr[3].ToString();
                                cbxActivo.Text = dr[8].ToString();
                                txtDia.Text = dr[4].ToString();
                                txtMes.Text = dr[5].ToString();
                                txtAnio.Text = dr[6].ToString();
                                txtNotas.Text = dr[7].ToString();
                            };
                        };
                        //Boton de Guardar
                        var btnGuardaPx = panelButtonAdd("Guardar", 10, 1, 0);
                        btnGuardaPx.Click += delegate
                        {
                            string MsgError = "";
                            if ((LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text))) { MsgError = "Una fecha esta mal."; };
                            if (MsgError != "") { MessageBox.Show(MsgError); }
                            else
                            {
                                try
                                {
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_update_visita";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@IdVis", int.Parse(txtIdVis.Text));
                                    cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                    cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                    cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                    cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                    cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    conDB.Close();
                                    MessageBox.Show("Registro Modificado");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaPx);
                    };
                };
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblVis);
                this.PnlNavegacion.Controls.Add(txtVisFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaVis = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaVis.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnRegistrarVisitadePx.Text + " > " + btnConsultaVis.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblVis = new DataTable();
                tblVis.Columns.Add("VIS", typeof(string));
                tblVis.Columns.Add("PX", typeof(string));
                tblVis.Columns.Add("TRA", typeof(string));
                tblVis.Columns.Add("ESP", typeof(string));
                tblVis.Columns.Add("Dia", typeof(string));
                tblVis.Columns.Add("Mes", typeof(string));
                tblVis.Columns.Add("Anio", typeof(string));
                tblVis.Columns.Add("Notas", typeof(string));
                tblVis.Columns.Add("Activo", typeof(string));

                string query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA, E.Nombre ESP, V.Dia, V.Mes, V.Anio, V.Notas, V.Activo FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento LEFT JOIN especialistas E ON E.IdEsp = V.IdEspecialista";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblVis.Rows.Add(
                            drd["VIS"].ToString(),
                            drd["PX"].ToString(),
                            drd["TRA"].ToString(),
                            drd["ESP"].ToString(),
                            drd["Dia"].ToString(),
                            drd["Mes"].ToString(),
                            drd["Anio"].ToString(),
                            drd["Notas"].ToString(),
                            drd["Activo"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridVisitas = panelDataGridViewAdd(1, 10, 750, 400);
                GridVisitas.DataSource = tblVis;
                GridVisitas.AutoResizeColumns();
                ((ISupportInitialize)(GridVisitas)).EndInit();
                this.PnlNavegacion.Controls.Add(GridVisitas);
            };

            this.PnlControles.Controls.Add(btnNuevaVisita);
            this.PnlControles.Controls.Add(btnModificaVis);
            this.PnlControles.Controls.Add(btnConsultaVis);
        }

        //LISTO: Procedimiento para Catalogo de Presupuestos
        private void btnActualizarPptodePx_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnActualizarPptodePx.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevoPpt = topMenuButtonAdd("Nuevo", 0);
            btnNuevoPpt.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnActualizarPptodePx.Text + " > " + btnNuevoPpt.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                string FiltroPx = "";
                string SelectedIdPx = "0";
                string[] cbxDescPx;

                var lblPx = panelLabelAdd("Paciente:", 0, 0);
                var txtPx = panelTextAdd("", 0, 100, 140, 1, HorizontalAlignment.Left);
                var btnBuscaPx = panelFindButtonAdd(0, 250);
                btnBuscaPx.Click += delegate
                {
                    btnBuscaPx.Enabled = false;
                    txtPx.Enabled = false;
                    if (txtPx.Text != "")
                    {
                        FiltroPx = "pacientes.nombre LIKE '%" + txtPx.Text.Trim() + "%'";
                    }
                    var cbxPx = panelComboBoxPacientes(FiltroPx, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxPx);
                    cbxPx.SelectedIndexChanged += delegate
                    {
                        cbxPx.Enabled = false;
                        cbxDescPx = cbxPx.Text.Split(' ');
                        SelectedIdPx = cbxDescPx[0].ToString();
                        string FiltroTra = "";
                        string SelectedIdTra = "0";
                        string[] cbxDescTra;

                        var lblTra = panelLabelAdd("Tratamiento:", 1, 0);
                        var txtTra = panelTextAdd("", 1, 100, 140, 1, HorizontalAlignment.Left);
                        var btnBuscaTra = panelFindButtonAdd(1, 250);
                        btnBuscaTra.Click += delegate
                        {
                            btnBuscaTra.Enabled = false;
                            txtTra.Enabled = false;
                            if (txtTra.Text != "")
                            {
                                FiltroTra = "tratamientos.descripcion LIKE '%" + txtTra.Text.Trim() + "%'";
                            }
                            var cbxTra = panelComboBoxTratamientos(FiltroTra, 1, 300, 450);
                            this.PnlNavegacion.Controls.Add(cbxTra);
                            cbxTra.SelectedIndexChanged += delegate
                            {
                                cbxTra.Enabled = false;
                                cbxDescTra = cbxTra.Text.Split(' ');
                                SelectedIdTra = cbxDescTra[0].ToString();
                                var cbxPrecioTra = cbxTra.Text.Split('|')[1];

                                var lblZona = panelLabelAdd("Zona Tratada:", 2, 0);
                                var txtZona = panelTextAdd("", 2, 100, 140, 1, HorizontalAlignment.Left);
                                var cbxActivo = panelComboBoxAdd(2, 250, 100);
                                cbxActivo.Items.Add("Vigente");
                                cbxActivo.Items.Add("Terminado");
                                cbxActivo.Items.Add("Interrumpido");
                                cbxActivo.Items.Add("Cancelado");
                                cbxActivo.SelectedIndex = 0;
                                var lblDia = panelLabelAdd("Dia/Mes/Año", 2, 370);
                                var txtDia = panelTextAdd("", 2, 470, 40, 1, HorizontalAlignment.Center);
                                var txtMes = panelTextAdd("", 2, 530, 40, 1, HorizontalAlignment.Center);
                                var txtAnio = panelTextAdd("", 2, 590, 70, 1, HorizontalAlignment.Center);
                                var lblPrc = panelLabelAdd("Precio:", 3, 0);
                                var txtPrc = panelTextAdd("", 3, 100, 140, 1, HorizontalAlignment.Right);
                                var lblNotas = panelLabelAdd("Notas:", 4, 0);
                                var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);
                                txtPrc.Text = cbxPrecioTra;
                                txtDia.Text = DateTime.Now.Day.ToString();
                                txtMes.Text = DateTime.Now.Month.ToString();
                                txtAnio.Text = DateTime.Now.Year.ToString();

                                this.PnlNavegacion.Controls.Add(lblZona);
                                this.PnlNavegacion.Controls.Add(txtZona);
                                this.PnlNavegacion.Controls.Add(cbxActivo);
                                this.PnlNavegacion.Controls.Add(lblDia);
                                this.PnlNavegacion.Controls.Add(txtDia);
                                this.PnlNavegacion.Controls.Add(txtMes);
                                this.PnlNavegacion.Controls.Add(txtAnio);
                                this.PnlNavegacion.Controls.Add(lblPrc);
                                this.PnlNavegacion.Controls.Add(txtPrc);
                                this.PnlNavegacion.Controls.Add(lblNotas);
                                this.PnlNavegacion.Controls.Add(txtNotas);
                                var btnGuardaPpto = panelButtonAdd("Guardar", 8, 1, 0);
                                btnGuardaPpto.Click += delegate
                                {
                                    string MsgError = "";
                                    if (LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text)) { MsgError = "La fecha esta mal."; };
                                    if (MsgError != "") { MessageBox.Show(MsgError); }
                                    try
                                    {
                                        SqlCommand cmd = conDB.CreateCommand();
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.CommandText = "sp_insert_presupuesto";
                                        cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                        cmd.Parameters.AddWithValue("@idPx", int.Parse(cbxPx.Text.Split(' ')[0]));
                                        cmd.Parameters.AddWithValue("@idTra", int.Parse(cbxTra.Text.Split(' ')[0]));
                                        cmd.Parameters.AddWithValue("@Zona", txtZona.Text);
                                        cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                        cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                        cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                        cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                        cmd.Parameters.AddWithValue("@Precio", float.Parse(txtPrc.Text));
                                        cmd.Parameters.AddWithValue("@Estatus", cbxActivo.Text);
                                        SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                                        r.Direction = ParameterDirection.ReturnValue;
                                        r.Value = 0;
                                        conDB.Open();
                                        cmd.ExecuteNonQuery();
                                        var idEsp = r.Value;
                                        conDB.Close();
                                        MessageBox.Show("Presupuesto Actualizado");

                                        //Limpia los controles del panel
                                        this.PnlControles.Controls.Clear();
                                        this.PnlNavegacion.Controls.Clear();
                                        lblHeader.Text = "Actitud Dental App";
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                        conDB.Close();
                                    }
                                };
                                this.PnlNavegacion.Controls.Add(btnGuardaPpto);
                            };
                        };
                        this.PnlNavegacion.Controls.Add(lblTra);
                        this.PnlNavegacion.Controls.Add(txtTra);
                        this.PnlNavegacion.Controls.Add(btnBuscaTra);
                    };
                };
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPx);
                this.PnlNavegacion.Controls.Add(btnBuscaPx);
            };

            var btnModificaPpt = topMenuButtonAdd("Modificar", 1);
            btnModificaPpt.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnActualizarPptodePx.Text + " > " + btnModificaPpt.Text;
                RegistraAcceso(lblHeader.Text);
                string Filtro = "";
                string SelectedIdPpt = "0";
                string[] cbxDesc;

                var lblPpt = panelLabelAdd("Buscar:", 0, 0);
                var txtPptFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0, 250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtPptFiltro.Enabled = false;
                    if (txtPptFiltro.Text != "")
                    {
                        Filtro = "presupuestos.IdPpto = " + txtPptFiltro.Text.Trim();
                    }
                    DataTable tblPpt = getTablaPresupuestos(Filtro);
                    var cbxPpt = panelComboBoxPresupuestos(Filtro, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxPpt);
                    cbxPpt.SelectedIndexChanged += delegate
                    {
                        cbxPpt.Enabled = false;
                        var txtIdPpt = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdPpt.Enabled = false;
                        var txtPx = panelTextAdd("", 1, 100, 200, 1, HorizontalAlignment.Left);
                        txtPx.Enabled = false;
                        var txtTra = panelTextAdd("", 1, 300, 200, 1, HorizontalAlignment.Left);
                        txtTra.Enabled = false;

                        var lblZona = panelLabelAdd("Zona Tratada:", 2, 0);
                        var txtZona = panelTextAdd("", 2, 100, 140, 1, HorizontalAlignment.Left);
                        var cbxActivo = panelComboBoxAdd(2, 250, 100);
                        cbxActivo.Items.Add("Vigente");
                        cbxActivo.Items.Add("Terminado");
                        cbxActivo.Items.Add("Interrumpido");
                        cbxActivo.Items.Add("Cancelado");
                        cbxActivo.SelectedIndex = 0;
                        var lblDia = panelLabelAdd("Dia/Mes/Año", 2, 370);
                        var txtDia = panelTextAdd("", 2, 470, 40, 1, HorizontalAlignment.Center);
                        var txtMes = panelTextAdd("", 2, 530, 40, 1, HorizontalAlignment.Center);
                        var txtAnio = panelTextAdd("", 2, 590, 70, 1, HorizontalAlignment.Center);
                        var lblPrc = panelLabelAdd("Precio:", 3, 0);
                        var txtPrc = panelTextAdd("", 3, 100, 140, 1, HorizontalAlignment.Right);
                        var lblNotas = panelLabelAdd("Notas:", 4, 0);
                        var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);

                        this.PnlNavegacion.Controls.Add(txtIdPpt);
                        this.PnlNavegacion.Controls.Add(txtPx);
                        this.PnlNavegacion.Controls.Add(txtTra);
                        this.PnlNavegacion.Controls.Add(lblZona);
                        this.PnlNavegacion.Controls.Add(txtZona);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(lblDia);
                        this.PnlNavegacion.Controls.Add(txtDia);
                        this.PnlNavegacion.Controls.Add(txtMes);
                        this.PnlNavegacion.Controls.Add(txtAnio);
                        this.PnlNavegacion.Controls.Add(lblPrc);
                        this.PnlNavegacion.Controls.Add(txtPrc);
                        this.PnlNavegacion.Controls.Add(lblNotas);
                        this.PnlNavegacion.Controls.Add(txtNotas);
                        cbxDesc = cbxPpt.Text.Split(' ');
                        SelectedIdPpt = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblPpt.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdPpt)
                            {
                                txtIdPpt.Text = dr[0].ToString();
                                txtPx.Text = dr[2].ToString();
                                txtTra.Text = dr[4].ToString();
                                txtZona.Text = dr[5].ToString();
                                cbxActivo.Text = dr[11].ToString();
                                txtDia.Text = dr[6].ToString();
                                txtMes.Text = dr[7].ToString();
                                txtAnio.Text = dr[8].ToString();
                                txtPrc.Text = dr[10].ToString();
                                txtNotas.Text = dr[9].ToString();
                            };
                        };
                        //Boton de Guardar
                        var btnGuardaPx = panelButtonAdd("Guardar", 10, 1, 0);
                        btnGuardaPx.Click += delegate
                        {
                            string MsgError = "";
                            if ((LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text))) { MsgError = "Una fecha esta mal."; };
                            if (MsgError != "") { MessageBox.Show(MsgError); }
                            else
                            {
                                try
                                {
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_update_presupuesto";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@IdPpt", int.Parse(txtIdPpt.Text));
                                    cmd.Parameters.AddWithValue("@Zona", txtZona.Text);
                                    cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                    cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                    cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                    cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                    cmd.Parameters.AddWithValue("@Precio", float.Parse(txtPrc.Text));
                                    cmd.Parameters.AddWithValue("@Estatus", cbxActivo.Text);
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    conDB.Close();
                                    MessageBox.Show("Registro Modificado");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaPx);
                    };
                };
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblPpt);
                this.PnlNavegacion.Controls.Add(txtPptFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaPpt = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaPpt.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnActualizarPptodePx.Text + " > " + btnConsultaPpt.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblPpt = new DataTable();
                tblPpt.Columns.Add("IdPpto", typeof(string));
                tblPpt.Columns.Add("IdPx", typeof(string));
                tblPpt.Columns.Add("IdTratamiento", typeof(string));
                tblPpt.Columns.Add("ZonaTratada", typeof(string));
                tblPpt.Columns.Add("Dia", typeof(string));
                tblPpt.Columns.Add("Mes", typeof(string));
                tblPpt.Columns.Add("Anio", typeof(string));
                tblPpt.Columns.Add("Notas", typeof(string));
                tblPpt.Columns.Add("Precio", typeof(string));
                tblPpt.Columns.Add("Estatus", typeof(string));

                string query = "SELECT IdPpto,IdPx,IdTratamiento,ZonaTratada,Dia,Mes,Anio,Notas,Precio,Estatus FROM presupuestos";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblPpt.Rows.Add(
                            drd["IdPpto"].ToString(),
                            drd["IdPx"].ToString(),
                            drd["IdTratamiento"].ToString(),
                            drd["ZonaTratada"].ToString(),
                            drd["Dia"].ToString(),
                            drd["Mes"].ToString(),
                            drd["Anio"].ToString(),
                            drd["Notas"].ToString(),
                            drd["Precio"].ToString(),
                            drd["Estatus"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridPresupuestos = panelDataGridViewAdd(1, 10, 750, 400);
                GridPresupuestos.DataSource = tblPpt;
                GridPresupuestos.AutoResizeColumns();
                ((ISupportInitialize)(GridPresupuestos)).EndInit();
                this.PnlNavegacion.Controls.Add(GridPresupuestos);
            };

            this.PnlControles.Controls.Add(btnNuevoPpt);
            this.PnlControles.Controls.Add(btnModificaPpt);
            this.PnlControles.Controls.Add(btnConsultaPpt);
        }

        //PENDIENTE
        private void btnPagos_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnPagos.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevoPago = topMenuButtonAdd("Nuevo", 0);
            btnNuevoPago.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnPagos.Text + " > " + btnNuevoPago.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                string FiltroPx = "";
                string SelectedIdPx = "0";
                string[] cbxDescPx;

                var lblPx = panelLabelAdd("Paciente:", 0, 0);
                var txtPx = panelTextAdd("", 0, 100, 140, 1, HorizontalAlignment.Left);
                var btnBuscaPx = panelFindButtonAdd(0, 250);
                btnBuscaPx.Click += delegate
                {
                    btnBuscaPx.Enabled = false;
                    txtPx.Enabled = false;
                    if (txtPx.Text != "")
                    {
                        FiltroPx = "pacientes.activo = 'ACTIVO' AND pacientes.nombre LIKE '%" + txtPx.Text.Trim() + "%'";
                    }
                    else
                    {
                        FiltroPx = "pacientes.activo = 'ACTIVO'";
                    }
                    var cbxPx = panelComboBoxPacientes(FiltroPx, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxPx);
                    cbxPx.SelectedIndexChanged += delegate
                    {
                        cbxPx.Enabled = false;
                        cbxDescPx = cbxPx.Text.Split(' ');
                        SelectedIdPx = cbxDescPx[0].ToString();
                        string FiltroPpt = "";
                        string SelectedIdPpt = "0";
                        string[] cbxDescPpt;

                        var lblPpt = panelLabelAdd("Presupuestos:", 1, 0);
                        FiltroPpt = "Presupuestos.Estatus = 'Vigente' AND Presupuestos.idPx = " + SelectedIdPx.ToString();
                        var cbxPpt = panelComboBoxPresupuestos(FiltroPpt, 1, 300, 450);
                        this.PnlNavegacion.Controls.Add(cbxPpt);
                        cbxPpt.SelectedIndexChanged += delegate
                        {
                            cbxPpt.Enabled = false;
                            cbxDescPpt = cbxPpt.Text.Split(' ');
                            SelectedIdPpt = cbxDescPpt[0].ToString();
                            var FiltroEsp = "";

                            //if (cbxPpt.Text.Split('|')[1].ToString() != "")
                            //{
                            //    FiltroEsp = "Especialistas.activo = 'Activo' AND Especialistas.especialidad LIKE '%" + cbxPpt.Text.Split('|')[1].ToString() + "%'";
                            //}
                            //else
                            //{
                            //    FiltroEsp = "Especialistas.activo = 'Activo'";
                            //}

                            FiltroEsp = "Especialistas.activo = 'Activo'";
                            var lblEsp = panelLabelAdd("Especialistas:", 2, 0);
                            var cbxEsp = panelComboBoxEspecialistas(FiltroEsp, 2, 300, 450);

                            var cbxActivo = panelComboBoxAdd(3, 250, 100);
                            cbxActivo.Items.Add("Activo");
                            cbxActivo.Items.Add("Cancelado");
                            cbxActivo.SelectedIndex = 0;
                            var lblDia = panelLabelAdd("Dia/Mes/Año", 3, 370);
                            var txtDia = panelTextAdd("", 3, 470, 40, 1, HorizontalAlignment.Center);
                            var txtMes = panelTextAdd("", 3, 530, 40, 1, HorizontalAlignment.Center);
                            var txtAnio = panelTextAdd("", 3, 590, 70, 1, HorizontalAlignment.Center);
                            var lblNotas = panelLabelAdd("Notas:", 4, 0);
                            var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);
                            txtDia.Text = DateTime.Now.Day.ToString();
                            txtMes.Text = DateTime.Now.Month.ToString();
                            txtAnio.Text = DateTime.Now.Year.ToString();

                            this.PnlNavegacion.Controls.Add(lblEsp);
                            this.PnlNavegacion.Controls.Add(cbxEsp);
                            this.PnlNavegacion.Controls.Add(cbxActivo);
                            this.PnlNavegacion.Controls.Add(lblDia);
                            this.PnlNavegacion.Controls.Add(txtDia);
                            this.PnlNavegacion.Controls.Add(txtMes);
                            this.PnlNavegacion.Controls.Add(txtAnio);
                            this.PnlNavegacion.Controls.Add(lblNotas);
                            this.PnlNavegacion.Controls.Add(txtNotas);
                            var btnGuardaPpto = panelButtonAdd("Guardar", 8, 1, 0);
                            btnGuardaPpto.Click += delegate
                            {
                                string MsgError = "";
                                if (LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text)) { MsgError = "La fecha esta mal."; };
                                if (MsgError != "") { MessageBox.Show(MsgError); }
                                try
                                {
                                    var auxEsp = 0;
                                    if (cbxEsp.Text == "")
                                    {
                                        auxEsp = 0;
                                    }
                                    else
                                    {
                                        auxEsp = int.Parse(cbxEsp.Text.Split(' ')[0]);
                                    }
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_insert_visita";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@idPpto", int.Parse(cbxPpt.Text.Split(' ')[0]));
                                    cmd.Parameters.AddWithValue("@idEsp", auxEsp);
                                    cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                    cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                    cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                    cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                    cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                    SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                                    r.Direction = ParameterDirection.ReturnValue;
                                    r.Value = 0;
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    var idEsp = r.Value;
                                    conDB.Close();
                                    MessageBox.Show("Visita Registrada");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            };
                            this.PnlNavegacion.Controls.Add(btnGuardaPpto);
                        };
                        this.PnlNavegacion.Controls.Add(lblPpt);
                    };
                };
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPx);
                this.PnlNavegacion.Controls.Add(btnBuscaPx);
            };

            var btnModificaPago = topMenuButtonAdd("Modificar", 1);
            btnModificaPago.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnPagos.Text + " > " + btnModificaPago.Text;
                RegistraAcceso(lblHeader.Text);
                string Filtro = "";
                string SelectedIdVis = "0";
                string[] cbxDesc;

                var lblVis = panelLabelAdd("Buscar:", 0, 0);
                var txtVisFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0, 250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtVisFiltro.Enabled = false;
                    if (txtVisFiltro.Text != "")
                    {
                        Filtro = "V.IdVisita = " + txtVisFiltro.Text.Trim();
                    }
                    DataTable tblVis = getTablaVisitas(Filtro);
                    var cbxVis = panelComboBoxVisitas(Filtro, 0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxVis);
                    cbxVis.SelectedIndexChanged += delegate
                    {
                        cbxVis.Enabled = false;
                        var txtIdVis = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdVis.Enabled = false;
                        var txtPx = panelTextAdd("", 1, 100, 200, 1, HorizontalAlignment.Left);
                        txtPx.Enabled = false;
                        var txtTra = panelTextAdd("", 1, 300, 200, 1, HorizontalAlignment.Left);
                        txtTra.Enabled = false;
                        var txtEsp = panelTextAdd("", 2, 100, 200, 1, HorizontalAlignment.Left);
                        txtEsp.Enabled = false;

                        var cbxActivo = panelComboBoxAdd(3, 250, 100);
                        cbxActivo.Items.Add("Activo");
                        cbxActivo.Items.Add("Cancelado");
                        cbxActivo.SelectedIndex = 0;
                        var lblDia = panelLabelAdd("Dia/Mes/Año", 3, 370);
                        var txtDia = panelTextAdd("", 3, 470, 40, 1, HorizontalAlignment.Center);
                        var txtMes = panelTextAdd("", 3, 530, 40, 1, HorizontalAlignment.Center);
                        var txtAnio = panelTextAdd("", 3, 590, 70, 1, HorizontalAlignment.Center);
                        var lblNotas = panelLabelAdd("Notas:", 4, 0);
                        var txtNotas = panelTextAdd("", 4, 100, 650, 3, HorizontalAlignment.Left);

                        this.PnlNavegacion.Controls.Add(txtIdVis);
                        this.PnlNavegacion.Controls.Add(txtPx);
                        this.PnlNavegacion.Controls.Add(txtTra);
                        this.PnlNavegacion.Controls.Add(txtEsp);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(lblDia);
                        this.PnlNavegacion.Controls.Add(txtDia);
                        this.PnlNavegacion.Controls.Add(txtMes);
                        this.PnlNavegacion.Controls.Add(txtAnio);
                        this.PnlNavegacion.Controls.Add(lblNotas);
                        this.PnlNavegacion.Controls.Add(txtNotas);
                        cbxDesc = cbxVis.Text.Split(' ');
                        SelectedIdVis = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblVis.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdVis)
                            {
                                txtIdVis.Text = dr[0].ToString();
                                txtPx.Text = dr[1].ToString();
                                txtTra.Text = dr[2].ToString();
                                txtEsp.Text = dr[3].ToString();
                                cbxActivo.Text = dr[8].ToString();
                                txtDia.Text = dr[4].ToString();
                                txtMes.Text = dr[5].ToString();
                                txtAnio.Text = dr[6].ToString();
                                txtNotas.Text = dr[7].ToString();
                            };
                        };
                        //Boton de Guardar
                        var btnGuardaPx = panelButtonAdd("Guardar", 10, 1, 0);
                        btnGuardaPx.Click += delegate
                        {
                            string MsgError = "";
                            if ((LaFechaEstaMal(txtDia.Text, txtMes.Text, txtAnio.Text))) { MsgError = "Una fecha esta mal."; };
                            if (MsgError != "") { MessageBox.Show(MsgError); }
                            else
                            {
                                try
                                {
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_update_visita";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@IdVis", int.Parse(txtIdVis.Text));
                                    cmd.Parameters.AddWithValue("@Dia", txtDia.Text);
                                    cmd.Parameters.AddWithValue("@Mes", txtMes.Text);
                                    cmd.Parameters.AddWithValue("@Anio", txtAnio.Text);
                                    cmd.Parameters.AddWithValue("@Notas", txtNotas.Text);
                                    cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    conDB.Close();
                                    MessageBox.Show("Registro Modificado");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaPx);
                    };
                };
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblVis);
                this.PnlNavegacion.Controls.Add(txtVisFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaPago = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaPago.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnPagos.Text + " > " + btnConsultaPago.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblVis = new DataTable();
                tblVis.Columns.Add("VIS", typeof(string));
                tblVis.Columns.Add("PX", typeof(string));
                tblVis.Columns.Add("TRA", typeof(string));
                tblVis.Columns.Add("ESP", typeof(string));
                tblVis.Columns.Add("Dia", typeof(string));
                tblVis.Columns.Add("Mes", typeof(string));
                tblVis.Columns.Add("Anio", typeof(string));
                tblVis.Columns.Add("Notas", typeof(string));
                tblVis.Columns.Add("Activo", typeof(string));

                string query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA, E.Nombre ESP, V.Dia, V.Mes, V.Anio, V.Notas, V.Activo FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento LEFT JOIN especialistas E ON E.IdEsp = V.IdEspecialista";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblVis.Rows.Add(
                            drd["VIS"].ToString(),
                            drd["PX"].ToString(),
                            drd["TRA"].ToString(),
                            drd["ESP"].ToString(),
                            drd["Dia"].ToString(),
                            drd["Mes"].ToString(),
                            drd["Anio"].ToString(),
                            drd["Notas"].ToString(),
                            drd["Activo"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridVisitas = panelDataGridViewAdd(1, 10, 750, 400);
                GridVisitas.DataSource = tblVis;
                GridVisitas.AutoResizeColumns();
                ((ISupportInitialize)(GridVisitas)).EndInit();
                this.PnlNavegacion.Controls.Add(GridVisitas);
            };

            this.PnlControles.Controls.Add(btnNuevoPago);
            this.PnlControles.Controls.Add(btnModificaPago);
            this.PnlControles.Controls.Add(btnConsultaPago);
        }

        //LISTO: Procedimiento para Catalogo de Pacientes
        private void btnPx_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnPx.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevoPx = topMenuButtonAdd("Nuevo", 0);
            btnNuevoPx.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnPx.Text + " > " + btnNuevoPx.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();
                
                // Dibuja la mayoria de los controles
                var lblPx = panelLabelAdd("Nombre Px:", 0, 100);
                var txtPx = panelTextAdd("", 0, 200, 300, 1, HorizontalAlignment.Left);
                var lblNac = panelLabelAdd("Fecha de Nacimiento", 1, 50);
                var lblAlta = panelLabelAdd("Fecha de Alta", 1, 430);
                var lblDia = panelLabelAdd("Dia", 2, 50);
                var lblMes = panelLabelAdd("Mes", 2, 110);
                var lblAnio = panelLabelAdd("Año", 2, 180);
                var lblDia2 = panelLabelAdd("Dia", 2, 410);
                var lblMes2 = panelLabelAdd("Mes", 2, 470);
                var lblAnio2 = panelLabelAdd("Año", 2, 540);
                var txtDiaNac = panelTextAdd("", 3, 50, 40, 1, HorizontalAlignment.Center);
                var txtMesNac = panelTextAdd("", 3, 110, 40, 1, HorizontalAlignment.Center);
                var txtAnioNac = panelTextAdd("", 3, 170, 70, 1, HorizontalAlignment.Center);
                var txtDiaAlta = panelTextAdd("", 3, 410, 40, 1, HorizontalAlignment.Center);
                var txtMesAlta = panelTextAdd("", 3, 470, 40, 1, HorizontalAlignment.Center);
                var txtAnioAlta = panelTextAdd("", 3, 530, 70, 1, HorizontalAlignment.Center);
                var lblTel = panelLabelAdd("Telefono:", 5, 120);
                var txtTel = panelTextAdd("", 5, 200, 300, 1, HorizontalAlignment.Left);
                var lblPxFact = panelLabelAdd("Nombre de Facturacion:", 6, 20);
                var txtPxFact = panelTextAdd("", 6, 200, 300, 1, HorizontalAlignment.Left);
                var lblRFC = panelLabelAdd("RFC:", 7, 150);
                var txtRFC = panelTextAdd("", 7, 200, 300, 1, HorizontalAlignment.Left);
                txtDiaAlta.Text = DateTime.Now.Day.ToString();
                txtMesAlta.Text = DateTime.Now.Month.ToString();
                txtAnioAlta.Text = DateTime.Now.Year.ToString();
                
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPx);
                this.PnlNavegacion.Controls.Add(lblDia);
                this.PnlNavegacion.Controls.Add(lblMes);
                this.PnlNavegacion.Controls.Add(lblAnio);
                this.PnlNavegacion.Controls.Add(lblDia2);
                this.PnlNavegacion.Controls.Add(lblMes2);
                this.PnlNavegacion.Controls.Add(lblAnio2);
                this.PnlNavegacion.Controls.Add(lblNac);
                this.PnlNavegacion.Controls.Add(txtDiaNac);
                this.PnlNavegacion.Controls.Add(txtMesNac);
                this.PnlNavegacion.Controls.Add(txtAnioNac);
                this.PnlNavegacion.Controls.Add(lblAlta);
                this.PnlNavegacion.Controls.Add(txtDiaAlta);
                this.PnlNavegacion.Controls.Add(txtMesAlta);
                this.PnlNavegacion.Controls.Add(txtAnioAlta);
                this.PnlNavegacion.Controls.Add(lblTel);
                this.PnlNavegacion.Controls.Add(txtTel);
                this.PnlNavegacion.Controls.Add(lblPxFact);
                this.PnlNavegacion.Controls.Add(txtPxFact);
                this.PnlNavegacion.Controls.Add(lblRFC);
                this.PnlNavegacion.Controls.Add(txtRFC);

                var btnGuardaPx = panelButtonAdd("Guardar", 9, 1, 0);
                btnGuardaPx.Click += delegate
                {
                    string MsgError = "";
                    if ((LaFechaEstaMal(txtDiaNac.Text, txtMesNac.Text, txtAnioNac.Text) || LaFechaEstaMal(txtDiaAlta.Text, txtMesAlta.Text, txtAnioAlta.Text))) { MsgError = "Una fecha esta mal."; };
                    if (txtPx.Text == "") { MsgError = "Tiene que tener nombre."; };
                    if (txtPx.Text == "" && (LaFechaEstaMal(txtDiaNac.Text, txtMesNac.Text, txtAnioNac.Text) || LaFechaEstaMal(txtDiaAlta.Text, txtMesAlta.Text, txtAnioAlta.Text))) { MsgError = "Tiene que tener nombre y una fecha esta mal."; };
                    if (MsgError != "") { MessageBox.Show(MsgError); }
                    else
                    {
                        try
                        {
                            SqlCommand cmd = conDB.CreateCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "sp_insert_paciente";
                            cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                            cmd.Parameters.AddWithValue("@Nombre", txtPx.Text);
                            cmd.Parameters.AddWithValue("@DiaNac", txtDiaNac.Text);
                            cmd.Parameters.AddWithValue("@MesNac", txtMesNac.Text);
                            cmd.Parameters.AddWithValue("@AnioNac", txtAnioNac.Text);
                            cmd.Parameters.AddWithValue("@DiaAlta", txtDiaAlta.Text);
                            cmd.Parameters.AddWithValue("@MesAlta", txtMesAlta.Text);
                            cmd.Parameters.AddWithValue("@AnioAlta", txtAnioAlta.Text);
                            cmd.Parameters.AddWithValue("@Telefono", txtTel.Text);
                            cmd.Parameters.AddWithValue("@NombreFact", txtPxFact.Text);
                            cmd.Parameters.AddWithValue("@RFC", txtRFC.Text);
                            SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                            r.Direction = ParameterDirection.ReturnValue;
                            r.Value = 0;
                            conDB.Open();
                            cmd.ExecuteNonQuery();
                            var idPx = r.Value;
                            conDB.Close();
                            MessageBox.Show("Paciente Registrado, id:" + idPx.ToString());

                            //Limpia los controles del panel
                            this.PnlControles.Controls.Clear();
                            this.PnlNavegacion.Controls.Clear();
                            lblHeader.Text = "Actitud Dental App";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            conDB.Close();
                        }
                    }
                };
                this.PnlNavegacion.Controls.Add(btnGuardaPx);
            };

            var btnModificaPx = topMenuButtonAdd("Modificar", 1);
            btnModificaPx.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnPx.Text + " > " + btnModificaPx.Text;
                RegistraAcceso(lblHeader.Text);
                string Filtro = "";
                string SelectedIdPx = "0";
                string[] cbxDesc;

                var lblPx = panelLabelAdd("Buscar:",0,0);
                var txtPxFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0,250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtPxFiltro.Enabled = false;
                    if (txtPxFiltro.Text != "")
                    {
                        Filtro = "pacientes.nombre LIKE '%" + txtPxFiltro.Text.Trim() + "%'";
                    }
                    DataTable tblPx = getTablaPacientes(Filtro);
                    var cbxPx = panelComboBoxPacientes(Filtro,0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxPx);
                    cbxPx.SelectedIndexChanged += delegate
                    {
                        cbxPx.Enabled = false;
                        var txtIdPx = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdPx.Enabled = false;
                        var cbxActivo = panelComboBoxAdd(1, 80, 100);
                        cbxActivo.Items.Add("Activo");
                        cbxActivo.Items.Add("Inactivo");
                        cbxActivo.SelectedIndex = 0;
                        var txtNombre = panelTextAdd("", 1, 200, 300, 1, HorizontalAlignment.Left);
                        var lblNac = panelLabelAdd("Fecha de Nacimiento", 2, 50);
                        var lblAlta = panelLabelAdd("Fecha de Alta", 2, 430);
                        var lblDia = panelLabelAdd("Dia", 3, 50);
                        var lblMes = panelLabelAdd("Mes", 3, 110);
                        var lblAnio = panelLabelAdd("Año", 3, 180);
                        var lblDia2 = panelLabelAdd("Dia", 3, 410);
                        var lblMes2 = panelLabelAdd("Mes", 3, 470);
                        var lblAnio2 = panelLabelAdd("Año", 3, 540);
                        var txtDiaNac = panelTextAdd("", 4, 50, 40, 1, HorizontalAlignment.Center);
                        var txtMesNac = panelTextAdd("", 4, 110, 40, 1, HorizontalAlignment.Center);
                        var txtAnioNac = panelTextAdd("", 4, 170, 70, 1, HorizontalAlignment.Center);
                        var txtDiaAlta = panelTextAdd("", 4, 410, 40, 1, HorizontalAlignment.Center);
                        var txtMesAlta = panelTextAdd("", 4, 470, 40, 1, HorizontalAlignment.Center);
                        var txtAnioAlta = panelTextAdd("", 4, 530, 70, 1, HorizontalAlignment.Center);
                        var lblTel = panelLabelAdd("Telefono:", 6, 120);
                        var txtTel = panelTextAdd("", 6, 200, 300, 1, HorizontalAlignment.Left);
                        var lblPxFact = panelLabelAdd("Nombre de Facturacion:", 7, 20);
                        var txtPxFact = panelTextAdd("", 7, 200, 300, 1, HorizontalAlignment.Left);
                        var lblRFC = panelLabelAdd("RFC:", 8, 150);
                        var txtRFC = panelTextAdd("", 8, 200, 300, 1, HorizontalAlignment.Left);
                        txtDiaAlta.Text = DateTime.Now.Day.ToString();
                        txtMesAlta.Text = DateTime.Now.Month.ToString();
                        txtAnioAlta.Text = DateTime.Now.Year.ToString();
                        this.PnlNavegacion.Controls.Add(txtIdPx);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(txtNombre);
                        this.PnlNavegacion.Controls.Add(lblDia);
                        this.PnlNavegacion.Controls.Add(lblMes);
                        this.PnlNavegacion.Controls.Add(lblAnio);
                        this.PnlNavegacion.Controls.Add(lblNac);
                        this.PnlNavegacion.Controls.Add(lblDia2);
                        this.PnlNavegacion.Controls.Add(lblMes2);
                        this.PnlNavegacion.Controls.Add(lblAnio2);
                        this.PnlNavegacion.Controls.Add(txtDiaNac);
                        this.PnlNavegacion.Controls.Add(txtMesNac);
                        this.PnlNavegacion.Controls.Add(txtAnioNac);
                        this.PnlNavegacion.Controls.Add(lblAlta);
                        this.PnlNavegacion.Controls.Add(txtDiaAlta);
                        this.PnlNavegacion.Controls.Add(txtMesAlta);
                        this.PnlNavegacion.Controls.Add(txtAnioAlta);
                        this.PnlNavegacion.Controls.Add(lblTel);
                        this.PnlNavegacion.Controls.Add(txtTel);
                        this.PnlNavegacion.Controls.Add(lblPxFact);
                        this.PnlNavegacion.Controls.Add(txtPxFact);
                        this.PnlNavegacion.Controls.Add(lblRFC);
                        this.PnlNavegacion.Controls.Add(txtRFC);
                        cbxDesc = cbxPx.Text.Split(' ');
                        SelectedIdPx = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblPx.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdPx)
                            {
                                txtIdPx.Text = dr[0].ToString();
                                txtNombre.Text = dr[1].ToString();
                                txtDiaNac.Text = dr[2].ToString();
                                txtMesNac.Text = dr[3].ToString();
                                txtAnioNac.Text = dr[4].ToString();
                                txtDiaAlta.Text = dr[5].ToString();
                                txtMesAlta.Text = dr[6].ToString();
                                txtAnioAlta.Text = dr[7].ToString();
                                txtTel.Text = dr[8].ToString();
                                txtPxFact.Text = dr[9].ToString();
                                txtRFC.Text = dr[10].ToString();
                                cbxActivo.Text = dr[11].ToString();
                            };
                        };
                        //Boton de Guardar
                        var btnGuardaPx = panelButtonAdd("Guardar", 10, 1, 0);
                        btnGuardaPx.Click += delegate
                        {
                            string MsgError = "";
                            if ((LaFechaEstaMal(txtDiaNac.Text, txtMesNac.Text, txtAnioNac.Text) || LaFechaEstaMal(txtDiaAlta.Text, txtMesAlta.Text, txtAnioAlta.Text))) { MsgError = "Una fecha esta mal."; };
                            if (txtNombre.Text == "") { MsgError = "Tiene que tener nombre."; };
                            if (txtNombre.Text == "" && (LaFechaEstaMal(txtDiaNac.Text, txtMesNac.Text, txtAnioNac.Text) || LaFechaEstaMal(txtDiaAlta.Text, txtMesAlta.Text, txtAnioAlta.Text))) { MsgError = "Tiene que tener nombre y una fecha esta mal."; };
                            if (MsgError != "") { MessageBox.Show(MsgError); }
                            else
                            {
                                try
                                {
                                    SqlCommand cmd = conDB.CreateCommand();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandText = "sp_update_paciente";
                                    cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                    cmd.Parameters.AddWithValue("@IdPx", txtIdPx.Text);
                                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                                    cmd.Parameters.AddWithValue("@DiaNac", txtDiaNac.Text);
                                    cmd.Parameters.AddWithValue("@MesNac", txtMesNac.Text);
                                    cmd.Parameters.AddWithValue("@AnioNac", txtAnioNac.Text);
                                    cmd.Parameters.AddWithValue("@DiaAlta", txtDiaAlta.Text);
                                    cmd.Parameters.AddWithValue("@MesAlta", txtMesAlta.Text);
                                    cmd.Parameters.AddWithValue("@AnioAlta", txtAnioAlta.Text);
                                    cmd.Parameters.AddWithValue("@Telefono", txtTel.Text);
                                    cmd.Parameters.AddWithValue("@NombreFact", txtPxFact.Text);
                                    cmd.Parameters.AddWithValue("@RFC", txtRFC.Text);
                                    cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                    conDB.Open();
                                    cmd.ExecuteNonQuery();
                                    conDB.Close();
                                    MessageBox.Show("Registro Modificado");

                                    //Limpia los controles del panel
                                    this.PnlControles.Controls.Clear();
                                    this.PnlNavegacion.Controls.Clear();
                                    lblHeader.Text = "Actitud Dental App";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    conDB.Close();
                                }
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaPx);
                    };
                };
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPxFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaPx = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaPx.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnPx.Text + " > " + btnConsultaPx.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblPx = new DataTable();
                tblPx.Columns.Add("IdPx", typeof(string));
                tblPx.Columns.Add("Nombre", typeof(string));
                tblPx.Columns.Add("DN", typeof(string));
                tblPx.Columns.Add("MN", typeof(string));
                tblPx.Columns.Add("AN", typeof(string));
                tblPx.Columns.Add("DA", typeof(string));
                tblPx.Columns.Add("MA", typeof(string));
                tblPx.Columns.Add("AA", typeof(string));
                tblPx.Columns.Add("Telefono", typeof(string));
                tblPx.Columns.Add("FacturaA", typeof(string));
                tblPx.Columns.Add("RFC", typeof(string));
                tblPx.Columns.Add("Activo", typeof(string));

                string query = "SELECT IdPx,Nombre,DiaNac,MesNac,AnioNac,DiaAlta,MesAlta,AnioAlta,Telefono,NombreFact,RFC,Activo FROM pacientes";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblPx.Rows.Add(
                            drd["IdPx"].ToString(),
                            drd["Nombre"].ToString(),
                            drd["DiaNac"].ToString(),
                            drd["MesNac"].ToString(),
                            drd["AnioNac"].ToString(),
                            drd["DiaAlta"].ToString(),
                            drd["MesAlta"].ToString(),
                            drd["AnioAlta"].ToString(),
                            drd["Telefono"].ToString(),
                            drd["NombreFact"].ToString(),
                            drd["RFC"].ToString(),
                            drd["Activo"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridPacientes = panelDataGridViewAdd(1, 10, 750, 400);
                GridPacientes.DataSource = tblPx;
                GridPacientes.AutoResizeColumns();
                ((ISupportInitialize)(GridPacientes)).EndInit();
                this.PnlNavegacion.Controls.Add(GridPacientes);
            };

            this.PnlControles.Controls.Add(btnNuevoPx);
            this.PnlControles.Controls.Add(btnModificaPx);
            this.PnlControles.Controls.Add(btnConsultaPx);
        }

        //LISTO: Procedimiento para Catalogo de Tratamientos
        private void btnTratamientos_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnTratamientos.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevoTra = topMenuButtonAdd("Nuevo", 0);
            btnNuevoTra.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnTratamientos.Text + " > " + btnNuevoTra.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();
                // Dibuja la mayoria de los controles
                var lblDes = panelLabelAdd("Descripcion:", 1, 100);
                var txtDes = panelTextAdd("", 1, 200, 300, 1, HorizontalAlignment.Left);
                var lblEsp = panelLabelAdd("Especialidad Pref:", 2, 60);
                var txtEsp = panelTextAdd("", 2, 200, 300, 1, HorizontalAlignment.Left);
                var lblPrc = panelLabelAdd("Precio:", 3, 140);
                var txtPrc = panelTextAdd("", 3, 200, 100, 1, HorizontalAlignment.Right);
                var lblRe1 = panelLabelAdd("Recordar a ", 3, 323);
                var txtRec = panelTextAdd("", 3, 410, 35, 1, HorizontalAlignment.Right);
                var lblRe2 = panelLabelAdd("dias de ultima visita ", 3, 450);
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblDes);
                this.PnlNavegacion.Controls.Add(txtDes);
                this.PnlNavegacion.Controls.Add(lblEsp);
                this.PnlNavegacion.Controls.Add(txtEsp);
                this.PnlNavegacion.Controls.Add(lblPrc);
                this.PnlNavegacion.Controls.Add(txtPrc);
                this.PnlNavegacion.Controls.Add(lblRe1);
                this.PnlNavegacion.Controls.Add(txtRec);
                this.PnlNavegacion.Controls.Add(lblRe2);
                var btnGuardaTra = panelButtonAdd("Guardar", 5, 1, 0);
                btnGuardaTra.Click += delegate
                {
                    try
                    {
                        SqlCommand cmd = conDB.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_insert_tratamiento";
                        cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                        cmd.Parameters.AddWithValue("@Descripcion", txtDes.Text);
                        cmd.Parameters.AddWithValue("@Especialidad", txtEsp.Text);
                        cmd.Parameters.AddWithValue("@DiasRecordatorio", txtRec.Text);
                        cmd.Parameters.AddWithValue("@PrecioDefault", float.Parse(txtPrc.Text));
                        SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                        r.Direction = ParameterDirection.ReturnValue;
                        r.Value = 0;
                        conDB.Open();
                        cmd.ExecuteNonQuery();
                        var idTra = r.Value;
                        conDB.Close();
                        MessageBox.Show("Tratamiento Registrado, id:" + idTra.ToString());

                        //Limpia los controles del panel
                        this.PnlControles.Controls.Clear();
                        this.PnlNavegacion.Controls.Clear();
                        lblHeader.Text = "Actitud Dental App";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conDB.Close();
                    }
                };
                this.PnlNavegacion.Controls.Add(btnGuardaTra);
            };

            var btnModificaTra = topMenuButtonAdd("Modificar", 1);
            btnModificaTra.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnTratamientos.Text + " > " + btnModificaTra.Text;
                RegistraAcceso(lblHeader.Text);
                string SelectedIdTra = "0";
                string Filtro = "";
                string[] cbxDesc;

                var lblFind = panelLabelAdd("Buscar:", 0, 0);
                var txtTraFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0, 250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtTraFiltro.Enabled = false;
                    if (txtTraFiltro.Text != "")
                    {
                        Filtro = "tratamientos.descripcion LIKE '%" + txtTraFiltro.Text.Trim() + "%'";
                    }
                    DataTable tblTra = getTablaTratamientos(Filtro);
                    var cbxTra = panelComboBoxTratamientos(Filtro,0, 300, 450);
                    this.PnlNavegacion.Controls.Add(cbxTra);
                    cbxTra.SelectedIndexChanged += delegate
                    {
                        cbxTra.Enabled = false;
                        var txtIdTra = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdTra.Enabled = false;
                        var cbxActivo = panelComboBoxAdd(1, 80, 100);
                        cbxActivo.Items.Add("Activo");
                        cbxActivo.Items.Add("Inactivo");
                        cbxActivo.SelectedIndex = 0;
                        var txtDesc = panelTextAdd("", 1, 200, 300, 1, HorizontalAlignment.Left);
                        var lblEsp = panelLabelAdd("Especialidad Pref:", 2, 60);
                        var txtEsp = panelTextAdd("", 2, 200, 300, 1, HorizontalAlignment.Left);
                        var lblPrc = panelLabelAdd("Precio:", 3, 140);
                        var txtPrc = panelTextAdd("", 3, 200, 100, 1, HorizontalAlignment.Right);
                        var lblRe1 = panelLabelAdd("Recordar a ", 3, 323);
                        var txtRec = panelTextAdd("", 3, 410, 35, 1, HorizontalAlignment.Right);
                        var lblRe2 = panelLabelAdd("dias de ultima visita ", 3, 450);
                        this.PnlNavegacion.Controls.Add(txtIdTra);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(txtDesc);
                        this.PnlNavegacion.Controls.Add(lblEsp);
                        this.PnlNavegacion.Controls.Add(txtEsp);
                        this.PnlNavegacion.Controls.Add(lblPrc);
                        this.PnlNavegacion.Controls.Add(txtPrc);
                        this.PnlNavegacion.Controls.Add(lblRe1);
                        this.PnlNavegacion.Controls.Add(txtRec);
                        this.PnlNavegacion.Controls.Add(lblRe2);
                        //Boton de Guardar
                        var btnGuardaTra = panelButtonAdd("Guardar", 4, 1, 0);
                        btnGuardaTra.Click += delegate
                        {
                            try
                            {
                                SqlCommand cmd = conDB.CreateCommand();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "sp_update_tratamiento";
                                cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                cmd.Parameters.AddWithValue("@IdTratamiento", txtIdTra.Text);
                                cmd.Parameters.AddWithValue("@Descripcion", txtDesc.Text);
                                cmd.Parameters.AddWithValue("@Especialidad", txtEsp.Text);
                                cmd.Parameters.AddWithValue("@DiasRecordatorio", txtRec.Text);
                                cmd.Parameters.AddWithValue("@PrecioDefault", float.Parse(txtPrc.Text));
                                cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                conDB.Open();
                                cmd.ExecuteNonQuery();
                                conDB.Close();
                                MessageBox.Show("Registro Modificado");

                                //Limpia los controles del panel
                                this.PnlControles.Controls.Clear();
                                this.PnlNavegacion.Controls.Clear();
                                lblHeader.Text = "Actitud Dental App";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                conDB.Close();
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaTra);

                        cbxDesc = cbxTra.Text.Split(' ');
                        SelectedIdTra = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblTra.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdTra)
                            {
                                txtIdTra.Text = dr[0].ToString();
                                txtDesc.Text = dr[1].ToString();
                                txtEsp.Text = dr[2].ToString();
                                txtRec.Text = dr[3].ToString();
                                txtPrc.Text = dr[4].ToString();
                                cbxActivo.Text = dr[5].ToString();
                            };
                        };
                    };
                };

                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblFind);
                this.PnlNavegacion.Controls.Add(txtTraFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaTra = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaTra.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnTratamientos.Text + " > " + btnConsultaTra.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblTra = new DataTable();
                tblTra.Columns.Add("IdTratamiento", typeof(string));
                tblTra.Columns.Add("Descripcion", typeof(string));
                tblTra.Columns.Add("Especialidad", typeof(string));
                tblTra.Columns.Add("DiasRecordatorio", typeof(string));
                tblTra.Columns.Add("PrecioDefault", typeof(string));
                tblTra.Columns.Add("Activo", typeof(string));

                string query = "SELECT IdTratamiento,Descripcion,Especialidad,DiasRecordatorio,PrecioDefault,Activo FROM tratamientos";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblTra.Rows.Add(
                            drd["IdTratamiento"].ToString(),
                            drd["Descripcion"].ToString(),
                            drd["Especialidad"].ToString(),
                            drd["DiasRecordatorio"].ToString(),
                            drd["PrecioDefault"].ToString(),
                            drd["Activo"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridTratamientos = panelDataGridViewAdd(1, 10, 750, 400);
                GridTratamientos.DataSource = tblTra;
                GridTratamientos.AutoResizeColumns();
                ((ISupportInitialize)(GridTratamientos)).EndInit();
                this.PnlNavegacion.Controls.Add(GridTratamientos);
            };

            this.PnlControles.Controls.Add(btnNuevoTra);
            this.PnlControles.Controls.Add(btnModificaTra);
            this.PnlControles.Controls.Add(btnConsultaTra);
        }

        //LISTO: Procedimiento para Catalogo de Especialistas
        private void btnEspecialistas_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnEspecialistas.Text;
            RegistraAcceso(lblHeader.Text);

            var btnNuevoEsp = topMenuButtonAdd("Nuevo", 0);
            btnNuevoEsp.Click += delegate
            {
                lblHeader.Text = "Actitud Dental App > " + btnEspecialistas.Text + " > " + btnNuevoEsp.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();
                // Dibuja la mayoria de los controles
                var lblPx = panelLabelAdd("Nombre Especialista:", 1, 40);
                var txtPx = panelTextAdd("", 1, 200, 300, 1, HorizontalAlignment.Left);
                var lblEsp = panelLabelAdd("Especialidad:", 2, 93);
                var txtEsp = panelTextAdd("", 2, 200, 300, 1, HorizontalAlignment.Left);
                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblPx);
                this.PnlNavegacion.Controls.Add(txtPx);
                this.PnlNavegacion.Controls.Add(lblEsp);
                this.PnlNavegacion.Controls.Add(txtEsp);
                var btnGuardaEsp = panelButtonAdd("Guardar", 4, 1, 0);
                btnGuardaEsp.Click += delegate
                {
                    try
                    {
                        SqlCommand cmd = conDB.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_insert_especialista";
                        cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                        cmd.Parameters.AddWithValue("@Nombre", txtPx.Text);
                        cmd.Parameters.AddWithValue("@Especialidad", txtEsp.Text);
                        SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                        r.Direction = ParameterDirection.ReturnValue;
                        r.Value = 0;
                        conDB.Open();
                        cmd.ExecuteNonQuery();
                        var idEsp = r.Value;
                        conDB.Close();
                        MessageBox.Show("Especialista Registrado, id:" + idEsp.ToString());

                        //Limpia los controles del panel
                        this.PnlControles.Controls.Clear();
                        this.PnlNavegacion.Controls.Clear();
                        lblHeader.Text = "Actitud Dental App";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conDB.Close();
                    }
                };
                this.PnlNavegacion.Controls.Add(btnGuardaEsp);
            };

            var btnModificaEsp = topMenuButtonAdd("Modificar", 1);
            btnModificaEsp.Click += delegate
            {
                this.PnlNavegacion.Controls.Clear();
                lblHeader.Text = "Actitud Dental App > " + btnEspecialistas.Text + " > " + btnModificaEsp.Text;
                RegistraAcceso(lblHeader.Text);
                string query = "SELECT IdEsp,Nombre,Especialidad,Activo FROM especialistas";
                string SelectedIdPx = "0";
                string[] cbxDesc;

                DataTable tblEsp = new DataTable();
                tblEsp.Columns.Add("IdEsp", typeof(string));
                tblEsp.Columns.Add("Nombre", typeof(string));
                tblEsp.Columns.Add("Especialidad", typeof(string));
                tblEsp.Columns.Add("Activo", typeof(string));

                var lblEsp = panelLabelAdd("Buscar:",0,0);
                var txtEspFiltro = panelTextAdd("", 0, 80, 160, 1, HorizontalAlignment.Left);
                var btnBusca = panelFindButtonAdd(0,250);
                btnBusca.Click += delegate
                {
                    btnBusca.Enabled = false;
                    txtEspFiltro.Enabled = false;
                    var cbxEsp = panelComboBoxAdd(0, 300, 450);
                    cbxEsp.Items.Clear();
                    this.PnlNavegacion.Controls.Add(cbxEsp);
                    cbxEsp.SelectedIndexChanged += delegate
                    {
                        cbxEsp.Enabled = false;
                        var txtIdEsp = panelTextAdd("", 1, 0, 60, 1, HorizontalAlignment.Left);
                        txtIdEsp.Enabled = false;
                        var cbxActivo = panelComboBoxAdd(1, 80, 100);
                        cbxActivo.Items.Add("Activo");
                        cbxActivo.Items.Add("Inactivo");
                        cbxActivo.SelectedIndex = 0;
                        var txtNombre = panelTextAdd("", 1, 200, 300, 1, HorizontalAlignment.Left);
                        var lblidEsp = panelLabelAdd("Especialidad:", 2, 83);
                        var txtEsp = panelTextAdd("", 2, 200, 300, 1, HorizontalAlignment.Left);
                        this.PnlNavegacion.Controls.Add(txtIdEsp);
                        this.PnlNavegacion.Controls.Add(cbxActivo);
                        this.PnlNavegacion.Controls.Add(txtNombre);
                        this.PnlNavegacion.Controls.Add(lblidEsp);
                        this.PnlNavegacion.Controls.Add(txtEsp);
                        //Boton de Guardar
                        var btnGuardaEsp = panelButtonAdd("Guardar", 4, 1, 0);
                        btnGuardaEsp.Click += delegate
                        {
                            try
                            {
                                SqlCommand cmd = conDB.CreateCommand();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandText = "sp_update_especialista";
                                cmd.Parameters.AddWithValue("@idUsr", VariablesGlobales.IdActivo);
                                cmd.Parameters.AddWithValue("@IdEsp", txtIdEsp.Text);
                                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                                cmd.Parameters.AddWithValue("@Especialidad", txtEsp.Text);
                                cmd.Parameters.AddWithValue("@Activo", cbxActivo.Text);
                                conDB.Open();
                                cmd.ExecuteNonQuery();
                                conDB.Close();
                                MessageBox.Show("Registro Modificado");

                                //Limpia los controles del panel
                                this.PnlControles.Controls.Clear();
                                this.PnlNavegacion.Controls.Clear();
                                lblHeader.Text = "Actitud Dental App";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                conDB.Close();
                            }
                        };
                        this.PnlNavegacion.Controls.Add(btnGuardaEsp);

                        cbxDesc = cbxEsp.Text.Split(' ');
                        SelectedIdPx = cbxDesc[0].ToString();
                        foreach (DataRow dr in tblEsp.Rows)
                        {
                            if (dr[0].ToString() == SelectedIdPx)
                            {
                                txtIdEsp.Text = dr[0].ToString();
                                txtNombre.Text = dr[1].ToString();
                                txtEsp.Text = dr[2].ToString();
                                cbxActivo.Text = dr[3].ToString();
                            };
                        };
                    };

                    tblEsp.Clear();
                    if (txtEspFiltro.Text != "")
                    {
                        query = "SELECT IdEsp,Nombre,Especialidad,Activo FROM Especialistas WHERE Especialistas.nombre LIKE '%" + txtEspFiltro.Text + "%'";
                    }
                    try
                    {
                        SqlCommand cmd = new SqlCommand(query, conDB);
                        cmd.CommandText = query;
                        conDB.Open();
                        SqlDataReader drd = cmd.ExecuteReader();
                        while (drd.Read())
                        {
                            cbxEsp.Items.Add(drd["IdEsp"].ToString() + " " + drd["Nombre"].ToString() + " " + drd["Especialidad"].ToString());
                            tblEsp.Rows.Add(
                                drd["IdEsp"].ToString(),
                                drd["Nombre"].ToString(),
                                drd["Especialidad"].ToString(),
                                drd["Activo"].ToString()
                                );
                        }
                        conDB.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        conDB.Close();
                    }
                };

                //Agrega controles al panel
                this.PnlNavegacion.Controls.Add(lblEsp);
                this.PnlNavegacion.Controls.Add(txtEspFiltro);
                this.PnlNavegacion.Controls.Add(btnBusca);
            };

            var btnConsultaEsp = topMenuButtonAdd("Ver Lista", 2);
            btnConsultaEsp.Click += delegate
            {

                lblHeader.Text = "Actitud Dental App > " + btnEspecialistas.Text + " > " + btnConsultaEsp.Text;
                RegistraAcceso(lblHeader.Text);
                this.PnlNavegacion.Controls.Clear();

                DataTable tblEsp = new DataTable();
                tblEsp.Columns.Add("IdEsp", typeof(string));
                tblEsp.Columns.Add("Nombre", typeof(string));
                tblEsp.Columns.Add("Especialidad", typeof(string));
                tblEsp.Columns.Add("Activo", typeof(string));

                string query = "SELECT IdEsp,Nombre,Especialidad,Activo FROM especialistas";
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conDB);
                    cmd.CommandText = query;
                    conDB.Open();
                    SqlDataReader drd = cmd.ExecuteReader();
                    while (drd.Read())
                    {
                        tblEsp.Rows.Add(
                            drd["IdEsp"].ToString(),
                            drd["Nombre"].ToString(),
                            drd["Especialidad"].ToString(),
                            drd["Activo"].ToString()
                            );
                    }
                    conDB.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conDB.Close();
                }
                var GridEspecialistas = panelDataGridViewAdd(1, 10, 750, 400);
                GridEspecialistas.DataSource = tblEsp;
                GridEspecialistas.AutoResizeColumns();
                ((ISupportInitialize)(GridEspecialistas)).EndInit();
                this.PnlNavegacion.Controls.Add(GridEspecialistas);
            };

            this.PnlControles.Controls.Add(btnNuevoEsp);
            this.PnlControles.Controls.Add(btnModificaEsp);
            this.PnlControles.Controls.Add(btnConsultaEsp);
        }

        //PENDIENTE
        private void btnReportes_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App > " + btnReportes.Text;
            RegistraAcceso(lblHeader.Text);
        }

        //LISTO: Procedimiento para el clic al Logo
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.PnlControles.Controls.Clear();
            this.PnlNavegacion.Controls.Clear();
            lblHeader.Text = "Actitud Dental App";
            RegistraAcceso(lblHeader.Text);
        }

        //LISTO: Funcion para validar fechas de 3 textbox dia mes año
        public bool LaFechaEstaMal(string dia,string mes, string anio)
        {
            bool LaFechaEstaMal = false;
            DateTime aux = DateTime.Parse("30/01/2000");
            if (DateTime.TryParse(dia + "/" + mes + "/" + anio, out aux) == false) { LaFechaEstaMal = true; };
            return LaFechaEstaMal;
        }

        //LISTO: Funcion para agregar los botones del menu de controles
        public Button topMenuButtonAdd(string texto, int orderButton)
        {
            var bAdd = new Button
            {
                Text = texto,
                Size = new Size(150, 30),
                Location = new Point(0 + ((150 + 10) * orderButton), 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Blue

            };
            bAdd.FlatAppearance.BorderSize = 2;
            bAdd.FlatAppearance.BorderColor = Color.Blue;
            bAdd.FlatAppearance.MouseDownBackColor = Color.SkyBlue;
            bAdd.FlatAppearance.MouseOverBackColor = Color.Wheat;
            return bAdd;
        }

        //LISTO: Funcion para agregar los botones del panel de navegacion
        public Button panelButtonAdd(string texto, int renglon, int cantidadButton, int orderButton)
        {
            var bAdd = new Button
            {
                Text = texto,
                Size = new Size(150, 60),
                Location = new Point((300 + ((150 + 10) * orderButton) - (80 * (cantidadButton - 1))), (renglonHeight * renglon)),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.DarkMagenta

            };
            bAdd.FlatAppearance.BorderSize = 2;
            bAdd.FlatAppearance.BorderColor = Color.DarkMagenta;
            bAdd.FlatAppearance.MouseDownBackColor = Color.LightPink;
            bAdd.FlatAppearance.MouseOverBackColor = Color.Wheat;
            return bAdd;
        }

        //LISTO: Funcion para agregar botones de busqueda al panel de navegacion
        public Button panelFindButtonAdd(int renglon, int columna)
        {
            Button bAdd = new Button
            {
                Size = new Size(30, 25),
                    Location = new Point(columna, (renglonHeight * renglon)),
                    FlatStyle = FlatStyle.Flat,
                    BackgroundImage = Image.FromFile("D:\\ProyectosVS\\ActitudDental\\ActitudDental\\img\\findicon.png"),
                    BackgroundImageLayout = ImageLayout.Stretch
                };
            return bAdd;
        }

        //LISTO: Funcion para agregar los labels del panel de navegacion
        public Label panelLabelAdd(string texto, int renglon, int columna)
        {
            var lAdd = new Label
            {
                AutoSize = true,
                Location = new Point(columna, (renglonHeight * renglon) + 3),
                Text = texto
            };
            return lAdd;
        }

        //LISTO: Funcion para agregar los textbox del panel de navegacion
        public TextBox panelTextAdd(string texto, int renglon, int columna, int ancho, int renglonesOcupados, HorizontalAlignment HAlign)
        {
            var tAdd = new TextBox
            {
                Size = new Size(ancho, (renglonesOcupados * (renglonHeight - 2))),
                Location = new Point(columna, (renglonHeight * renglon)),
                TextAlign = HAlign,
                Text = texto
            };
            if (renglonesOcupados > 1)
            {
                tAdd.Multiline = true;
            }
            return tAdd;
        }

        //LISTO: Funcion para agregar los comboboxes del panel de navegacion
        public ComboBox panelComboBoxAdd(int renglon, int columna, int ancho)
        {
            ComboBox cAdd = new ComboBox
            {
                Size = new Size(ancho, 30),
                Location = new Point(columna, (renglonHeight * renglon)),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Text = ""        
            };
            return cAdd;
        }

        //LISTO: Funcion para traer todos los clientes en base a un filtro en un ComboBox
        public ComboBox panelComboBoxPacientes(string Filtro, int renglon, int columna, int ancho)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT IdPx,Nombre,DiaNac,MesNac,AnioNac,DiaAlta,MesAlta,AnioAlta,Telefono,NombreFact,RFC,Activo FROM pacientes";
            }
            else
            {
                query = "SELECT IdPx,Nombre,DiaNac,MesNac,AnioNac,DiaAlta,MesAlta,AnioAlta,Telefono,NombreFact,RFC,Activo FROM pacientes WHERE " + Filtro;
            };
            ComboBox cAdd = panelComboBoxAdd(renglon, columna, ancho);
            cAdd.Items.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    cAdd.Items.Add(drd["IdPx"].ToString() + " " + drd["Nombre"].ToString() + " " + drd["Telefono"].ToString());
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return cAdd;
        }

        //LISTO: Funcion para traer todos los tratamientos en base a un filtro en un ComboBox
        public ComboBox panelComboBoxTratamientos(string Filtro, int renglon, int columna, int ancho)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT IdTratamiento,Descripcion,Especialidad,DiasRecordatorio,PrecioDefault,Activo FROM tratamientos";
            }
            else
            {
                query = "SELECT IdTratamiento,Descripcion,Especialidad,DiasRecordatorio,PrecioDefault,Activo FROM tratamientos WHERE " + Filtro;
            };
            ComboBox cAdd = panelComboBoxAdd(renglon, columna, ancho);
            cAdd.Items.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                string msg;
                while (drd.Read())
                {
                    msg = drd["IdTratamiento"].ToString();
                    cAdd.Items.Add(drd["IdTratamiento"].ToString() + " " + drd["Descripcion"].ToString() + " " + drd["Especialidad"].ToString() + " | " + drd["PrecioDefault"].ToString());
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return cAdd;
        }

        //LISTO: Funcion para traer todos los presupuestos en base a un filtro en un ComboBox
        public ComboBox panelComboBoxPresupuestos(string Filtro, int renglon, int columna, int ancho)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT presupuestos.IdPpto IDp,presupuestos.IdPx,Px.Nombre PaX,presupuestos.IdTratamiento,Tr.Descripcion TRA,presupuestos.ZonaTratada,presupuestos.Dia,presupuestos.Mes,presupuestos.Anio,presupuestos.Notas,presupuestos.Precio,presupuestos.Estatus FROM presupuestos LEFT JOIN Pacientes Px ON presupuestos.IdPx = Px.IdPx LEFT JOIN Tratamientos Tr ON presupuestos.IdTratamiento = Tr.IdTratamiento";
            }
            else
            {
                query = "SELECT presupuestos.IdPpto IDp,presupuestos.IdPx,Px.Nombre PaX,presupuestos.IdTratamiento,Tr.Descripcion TRA,presupuestos.ZonaTratada,presupuestos.Dia,presupuestos.Mes,presupuestos.Anio,presupuestos.Notas,presupuestos.Precio,presupuestos.Estatus FROM presupuestos LEFT JOIN Pacientes Px ON presupuestos.IdPx = Px.IdPx LEFT JOIN Tratamientos Tr ON presupuestos.IdTratamiento = Tr.IdTratamiento WHERE " + Filtro;
            };
            ComboBox cAdd = panelComboBoxAdd(renglon, columna, ancho);
            cAdd.Items.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    cAdd.Items.Add(drd["IDp"].ToString() + " " + drd["PaX"].ToString() + " | " + drd["TRA"].ToString());
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return cAdd;
        }

        //LISTO: Funcion para traer todos los Especialistas en base a un filtro en un ComboBox
        public ComboBox panelComboBoxEspecialistas(string Filtro, int renglon, int columna, int ancho)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT IdEsp, Nombre, Especialidad, Activo FROM especialistas";
            }
            else
            {
                query = "SELECT IdEsp, Nombre, Especialidad, Activo FROM especialistas WHERE " + Filtro;
            };
            ComboBox cAdd = panelComboBoxAdd(renglon, columna, ancho);
            cAdd.Items.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    cAdd.Items.Add(drd["IdEsp"].ToString() + " " + drd["Nombre"].ToString() + " " + drd["Especialidad"].ToString());
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return cAdd;
        }

        //LISTO: Funcion para traer todas las Visitas en base a un filtro en un ComboBox
        public ComboBox panelComboBoxVisitas(string Filtro, int renglon, int columna, int ancho)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento";
            }
            else
            {
                query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento WHERE " + Filtro;
            };
            ComboBox cAdd = panelComboBoxAdd(renglon, columna, ancho);
            cAdd.Items.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    cAdd.Items.Add(drd["VIS"].ToString() + " " + drd["PX"].ToString() + " | " + drd["TRA"].ToString());
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return cAdd;
        }

        //LISTO: Funcion para traer todos los clientes en base a un filtro en una Tabla
        public DataTable getTablaPacientes(string Filtro)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT IdPx,Nombre,DiaNac,MesNac,AnioNac,DiaAlta,MesAlta,AnioAlta,Telefono,NombreFact,RFC,Activo FROM pacientes";
            }
            else
            {
                query = "SELECT IdPx,Nombre,DiaNac,MesNac,AnioNac,DiaAlta,MesAlta,AnioAlta,Telefono,NombreFact,RFC,Activo FROM pacientes WHERE " + Filtro;
            };
            DataTable tblPx = new DataTable();
            tblPx.Columns.Add("IdPx", typeof(string));
            tblPx.Columns.Add("Nombre", typeof(string));
            tblPx.Columns.Add("DiaNac", typeof(string));
            tblPx.Columns.Add("MesNac", typeof(string));
            tblPx.Columns.Add("AnioNac", typeof(string));
            tblPx.Columns.Add("DiaAlta", typeof(string));
            tblPx.Columns.Add("MesAlta", typeof(string));
            tblPx.Columns.Add("AnioAlta", typeof(string));
            tblPx.Columns.Add("Telefono", typeof(string));
            tblPx.Columns.Add("NombreFact", typeof(string));
            tblPx.Columns.Add("RFC", typeof(string));
            tblPx.Columns.Add("Activo", typeof(string));
            tblPx.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    tblPx.Rows.Add(
                        drd["IdPx"].ToString(),
                        drd["Nombre"].ToString(),
                        drd["DiaNac"].ToString(),
                        drd["MesNac"].ToString(),
                        drd["AnioNac"].ToString(),
                        drd["DiaAlta"].ToString(),
                        drd["MesAlta"].ToString(),
                        drd["AnioAlta"].ToString(),
                        drd["Telefono"].ToString(),
                        drd["NombreFact"].ToString(),
                        drd["RFC"].ToString(),
                        drd["Activo"].ToString()
                        );
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return tblPx;
        }

        //LISTO: Funcion para traer todos los tratamientos en base a un filtro en una Tabla
        public DataTable getTablaTratamientos(string Filtro)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT IdTratamiento,Descripcion,Especialidad,DiasRecordatorio,PrecioDefault,Activo FROM tratamientos";
            }
            else
            {
                query = "SELECT IdTratamiento,Descripcion,Especialidad,DiasRecordatorio,PrecioDefault,Activo FROM tratamientos WHERE " + Filtro;
            };
            DataTable tblTra = new DataTable();
            tblTra.Columns.Add("IdTratamiento", typeof(string));
            tblTra.Columns.Add("Descripcion", typeof(string));
            tblTra.Columns.Add("Especialidad", typeof(string));
            tblTra.Columns.Add("DiasRecordatorio", typeof(string));
            tblTra.Columns.Add("PrecioDefault", typeof(string));
            tblTra.Columns.Add("Activo", typeof(string));
            tblTra.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    tblTra.Rows.Add(
                        drd["IdTratamiento"].ToString(),
                        drd["Descripcion"].ToString(),
                        drd["Especialidad"].ToString(),
                        drd["DiasRecordatorio"].ToString(),
                        drd["PrecioDefault"].ToString(),
                        drd["Activo"].ToString()
                        );
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return tblTra;
        }

        //LISTO: Funcion para traer todos los presupuestos en base al id
        public DataTable getTablaPresupuestos(string Filtro)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT presupuestos.IdPpto IDp,presupuestos.IdPx,Px.Nombre PaX,presupuestos.IdTratamiento,Tr.Descripcion TRA,presupuestos.ZonaTratada,presupuestos.Dia,presupuestos.Mes,presupuestos.Anio,presupuestos.Notas,presupuestos.Precio,presupuestos.Estatus FROM presupuestos LEFT JOIN Pacientes Px ON presupuestos.IdPx = Px.IdPx LEFT JOIN Tratamientos Tr ON presupuestos.IdTratamiento = Tr.IdTratamiento";
            }
            else
            {
                query = "SELECT presupuestos.IdPpto IDp,presupuestos.IdPx,Px.Nombre PaX,presupuestos.IdTratamiento,Tr.Descripcion TRA,presupuestos.ZonaTratada,presupuestos.Dia,presupuestos.Mes,presupuestos.Anio,presupuestos.Notas,presupuestos.Precio,presupuestos.Estatus FROM presupuestos LEFT JOIN Pacientes Px ON presupuestos.IdPx = Px.IdPx LEFT JOIN Tratamientos Tr ON presupuestos.IdTratamiento = Tr.IdTratamiento WHERE " + Filtro;
            };
            DataTable tblPpt = new DataTable();
            tblPpt.Columns.Add("IDp", typeof(string));
            tblPpt.Columns.Add("IdPx", typeof(string));
            tblPpt.Columns.Add("PaX", typeof(string));
            tblPpt.Columns.Add("IdTratamiento", typeof(string));
            tblPpt.Columns.Add("TRA", typeof(string));
            tblPpt.Columns.Add("ZonaTratada", typeof(string));
            tblPpt.Columns.Add("Dia", typeof(string));
            tblPpt.Columns.Add("Mes", typeof(string));
            tblPpt.Columns.Add("Anio", typeof(string));
            tblPpt.Columns.Add("Notas", typeof(string));
            tblPpt.Columns.Add("Precio", typeof(string));
            tblPpt.Columns.Add("Estatus", typeof(string));
            tblPpt.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    tblPpt.Rows.Add(
                        drd["IDp"].ToString(),
                        drd["IdPx"].ToString(),
                        drd["PaX"].ToString(),
                        drd["IdTratamiento"].ToString(),
                        drd["TRA"].ToString(),
                        drd["ZonaTratada"].ToString(),
                        drd["Dia"].ToString(),
                        drd["Mes"].ToString(),
                        drd["Anio"].ToString(),
                        drd["Notas"].ToString(),
                        drd["Precio"].ToString(),
                        drd["Estatus"].ToString()
                        );
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return tblPpt;
        }

        //LISTO: Funcion para traer todas las visitas en base al id
        public DataTable getTablaVisitas(string Filtro)
        {
            string query = "";
            if (Filtro == "")
            {
                query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA, E.Nombre ESP, V.Dia, V.Mes, V.Anio, V.Notas, V.Activo FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento LEFT JOIN especialistas E ON E.IdEsp = V.IdEspecialista";
            }
            else
            {
                query = "SELECT V.IdVisita VIS, Pa.Nombre PX, T.Descripcion TRA, E.Nombre ESP, V.Dia, V.Mes, V.Anio, V.Notas, V.Activo FROM Visitas V LEFT JOIN presupuestos P ON P.idPpto = V.idPpto LEFT JOIN pacientes Pa ON P.idPx = Pa.idPx LEFT JOIN tratamientos T ON P.idTratamiento = T.idTratamiento LEFT JOIN especialistas E ON E.IdEsp = V.IdEspecialista WHERE " + Filtro;
            };
            DataTable tblVis = new DataTable();
            tblVis.Columns.Add("VIS", typeof(string));
            tblVis.Columns.Add("PX", typeof(string));
            tblVis.Columns.Add("TRA", typeof(string));
            tblVis.Columns.Add("ESP", typeof(string));
            tblVis.Columns.Add("Dia", typeof(string));
            tblVis.Columns.Add("Mes", typeof(string));
            tblVis.Columns.Add("Anio", typeof(string));
            tblVis.Columns.Add("Notas", typeof(string));
            tblVis.Columns.Add("Activo", typeof(string));
            tblVis.Clear();
            try
            {
                SqlCommand cmd = new SqlCommand(query, conDB);
                cmd.CommandText = query;
                conDB.Open();
                SqlDataReader drd = cmd.ExecuteReader();
                while (drd.Read())
                {
                    tblVis.Rows.Add(
                        drd["VIS"].ToString(),
                        drd["PX"].ToString(),
                        drd["TRA"].ToString(),
                        drd["ESP"].ToString(),
                        drd["Dia"].ToString(),
                        drd["Mes"].ToString(),
                        drd["Anio"].ToString(),
                        drd["Notas"].ToString(),
                        drd["Activo"].ToString()
                        );
                }
                conDB.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
            return tblVis;
        }

        //LISTO: Funcion para agregar un grid al panel de navegacion
        public DataGridView panelDataGridViewAdd(int renglon,int columna, int ancho, int alto)
        {
            DataGridView gAdd = new DataGridView
            {
                Location = new Point(columna, (renglonHeight * renglon)),
                Size = new Size(ancho, alto),
                ColumnHeadersVisible = true,
                RowHeadersVisible = false,
                Visible = true,
                Font = new Font("arial", 8),
                ScrollBars = ScrollBars.Both
            };
            return gAdd;
        }

        //LISTO: Funcion para registrar accesos a botones
        public void RegistraAcceso(string MenuAccesado)
        {
            try
            {
                SqlCommand cmd = conDB.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_insert_acceso";
                cmd.Parameters.AddWithValue("@idUser", VariablesGlobales.IdActivo);
                cmd.Parameters.AddWithValue("@Menu", MenuAccesado);
                SqlParameter r = cmd.Parameters.Add("@RETURN", SqlDbType.Int);
                r.Direction = ParameterDirection.ReturnValue;
                r.Value = 0;
                conDB.Open();
                cmd.ExecuteNonQuery();
                var idPx = r.Value;
                conDB.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conDB.Close();
            }
        }

        private void btnFncns_Click(object sender, EventArgs e)
        {

        }

        private void PnlBg_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
