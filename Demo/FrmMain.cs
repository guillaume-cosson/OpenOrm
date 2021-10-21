using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demo.Sql;
using OpenOrm.SQLite;

namespace Demo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            //using -> auto dispose object at the end of the method
            using var db = Db.GetConnection();

            //Apply all changes in all OpenOrmMigration inherited classes from last version found in table OpenOrmMigration
            db.Migrate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            node.Text = "Connexions";
            node.Nodes.Add(new TreeNode { Text = "SqlServer" });
            node.Nodes.Add(new TreeNode { Text = "SQLite" });
            node.Nodes.Add(new TreeNode { Text = "MySql" });

            trv_demos.Nodes.Add(node);
            trv_demos.ExpandAll();
        }
    }
}
