using OpenOrmTools.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenOrmTools
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btn_generateFromExistingDb_Click(object sender, EventArgs e)
        {
            FrmGenerateModels frm = new FrmGenerateModels();
            frm.ShowDialog();
            frm.Dispose();
        }
    }
}
