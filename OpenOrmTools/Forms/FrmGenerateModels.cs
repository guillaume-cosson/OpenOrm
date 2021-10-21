using OpenOrmTools.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenOrmTools.Forms
{
    public partial class FrmGenerateModels : Form
    {
        public FrmGenerateModels()
        {
            InitializeComponent();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_generate_Click(object sender, EventArgs e)
        {
            ModelGenerator gen = new ModelGenerator();
            if (rdb_cs.Checked) gen.Language = "C#"; else gen.Language = "VB";
            if (rdb_sqlserver.Checked) gen.Database = OpenOrm.Connector.SqlServer; else if (rdb_sqlite.Checked) gen.Database = OpenOrm.Connector.SqLite; else gen.Database = OpenOrm.Connector.MySql;
            gen.ConnectionString = txt_cnx.Text;
            string folder = gen.Generate();

            
        }
    }
}
