
namespace OpenOrmTools
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_generateFromExistingDb = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_generateFromExistingDb
            // 
            this.btn_generateFromExistingDb.Location = new System.Drawing.Point(24, 24);
            this.btn_generateFromExistingDb.Name = "btn_generateFromExistingDb";
            this.btn_generateFromExistingDb.Size = new System.Drawing.Size(242, 23);
            this.btn_generateFromExistingDb.TabIndex = 0;
            this.btn_generateFromExistingDb.Text = "Generate models from existing database";
            this.btn_generateFromExistingDb.UseVisualStyleBackColor = true;
            this.btn_generateFromExistingDb.Click += new System.EventHandler(this.btn_generateFromExistingDb_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_generateFromExistingDb);
            this.Name = "FrmMain";
            this.Text = "OpenOrm Tools";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_generateFromExistingDb;
    }
}

