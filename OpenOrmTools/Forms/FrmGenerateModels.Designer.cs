
namespace OpenOrmTools.Forms
{
    partial class FrmGenerateModels
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
            this.rdb_sqlserver = new System.Windows.Forms.RadioButton();
            this.rdb_sqlite = new System.Windows.Forms.RadioButton();
            this.rdb_mysql = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdb_vb = new System.Windows.Forms.RadioButton();
            this.rdb_cs = new System.Windows.Forms.RadioButton();
            this.txt_cnx = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_testConnection = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_generate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdb_sqlserver
            // 
            this.rdb_sqlserver.AutoSize = true;
            this.rdb_sqlserver.Checked = true;
            this.rdb_sqlserver.Location = new System.Drawing.Point(2, 2);
            this.rdb_sqlserver.Name = "rdb_sqlserver";
            this.rdb_sqlserver.Size = new System.Drawing.Size(73, 19);
            this.rdb_sqlserver.TabIndex = 0;
            this.rdb_sqlserver.TabStop = true;
            this.rdb_sqlserver.Text = "SqlServer";
            this.rdb_sqlserver.UseVisualStyleBackColor = true;
            // 
            // rdb_sqlite
            // 
            this.rdb_sqlite.AutoSize = true;
            this.rdb_sqlite.Enabled = false;
            this.rdb_sqlite.Location = new System.Drawing.Point(100, 2);
            this.rdb_sqlite.Name = "rdb_sqlite";
            this.rdb_sqlite.Size = new System.Drawing.Size(59, 19);
            this.rdb_sqlite.TabIndex = 0;
            this.rdb_sqlite.Text = "SQLite";
            this.rdb_sqlite.UseVisualStyleBackColor = true;
            // 
            // rdb_mysql
            // 
            this.rdb_mysql.AutoSize = true;
            this.rdb_mysql.Enabled = false;
            this.rdb_mysql.Location = new System.Drawing.Point(183, 2);
            this.rdb_mysql.Name = "rdb_mysql";
            this.rdb_mysql.Size = new System.Drawing.Size(58, 19);
            this.rdb_mysql.TabIndex = 0;
            this.rdb_mysql.Text = "MySql";
            this.rdb_mysql.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdb_sqlserver);
            this.panel1.Controls.Add(this.rdb_mysql);
            this.panel1.Controls.Add(this.rdb_sqlite);
            this.panel1.Location = new System.Drawing.Point(12, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(262, 27);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rdb_vb);
            this.panel2.Controls.Add(this.rdb_cs);
            this.panel2.Location = new System.Drawing.Point(12, 54);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(159, 26);
            this.panel2.TabIndex = 2;
            // 
            // rdb_vb
            // 
            this.rdb_vb.AutoSize = true;
            this.rdb_vb.Enabled = false;
            this.rdb_vb.Location = new System.Drawing.Point(50, 3);
            this.rdb_vb.Name = "rdb_vb";
            this.rdb_vb.Size = new System.Drawing.Size(61, 19);
            this.rdb_vb.TabIndex = 0;
            this.rdb_vb.Text = "VB.Net";
            this.rdb_vb.UseVisualStyleBackColor = true;
            // 
            // rdb_cs
            // 
            this.rdb_cs.AutoSize = true;
            this.rdb_cs.Checked = true;
            this.rdb_cs.Location = new System.Drawing.Point(4, 3);
            this.rdb_cs.Name = "rdb_cs";
            this.rdb_cs.Size = new System.Drawing.Size(40, 19);
            this.rdb_cs.TabIndex = 0;
            this.rdb_cs.TabStop = true;
            this.rdb_cs.Text = "C#";
            this.rdb_cs.UseVisualStyleBackColor = true;
            // 
            // txt_cnx
            // 
            this.txt_cnx.Location = new System.Drawing.Point(12, 121);
            this.txt_cnx.Multiline = true;
            this.txt_cnx.Name = "txt_cnx";
            this.txt_cnx.Size = new System.Drawing.Size(776, 108);
            this.txt_cnx.TabIndex = 3;
            this.txt_cnx.Text = "Password=azerty/123+;Persist Security Info=True;User ID=sa;Initial Catalog=ModelG" +
    "enerator_1;Data Source=LAPTOP-UJSNA32D";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Connection string";
            // 
            // btn_testConnection
            // 
            this.btn_testConnection.Location = new System.Drawing.Point(713, 235);
            this.btn_testConnection.Name = "btn_testConnection";
            this.btn_testConnection.Size = new System.Drawing.Size(75, 23);
            this.btn_testConnection.TabIndex = 5;
            this.btn_testConnection.Text = "Test";
            this.btn_testConnection.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(713, 651);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 6;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_generate
            // 
            this.btn_generate.Location = new System.Drawing.Point(632, 651);
            this.btn_generate.Name = "btn_generate";
            this.btn_generate.Size = new System.Drawing.Size(75, 23);
            this.btn_generate.TabIndex = 7;
            this.btn_generate.Text = "Generate";
            this.btn_generate.UseVisualStyleBackColor = true;
            this.btn_generate.Click += new System.EventHandler(this.btn_generate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 264);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 381);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(7, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(346, 51);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "If child table (foreignkey) has only one value (except Id)";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(10, 22);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(99, 19);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Get full object";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(115, 22);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(100, 19);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.Text = "Get value only";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // FrmGenerateModels
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 686);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_generate);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_testConnection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_cnx);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmGenerateModels";
            this.Text = "FrmGenerateModels";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdb_sqlserver;
        private System.Windows.Forms.RadioButton rdb_sqlite;
        private System.Windows.Forms.RadioButton rdb_mysql;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdb_vb;
        private System.Windows.Forms.RadioButton rdb_cs;
        private System.Windows.Forms.TextBox txt_cnx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_testConnection;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_generate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}