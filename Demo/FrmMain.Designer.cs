
namespace Demo
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
            this.trv_demos = new System.Windows.Forms.TreeView();
            this.pnl_demo = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // trv_demos
            // 
            this.trv_demos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.trv_demos.Location = new System.Drawing.Point(13, 13);
            this.trv_demos.Name = "trv_demos";
            this.trv_demos.Size = new System.Drawing.Size(337, 737);
            this.trv_demos.TabIndex = 0;
            // 
            // pnl_demo
            // 
            this.pnl_demo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_demo.Location = new System.Drawing.Point(356, 13);
            this.pnl_demo.Name = "pnl_demo";
            this.pnl_demo.Size = new System.Drawing.Size(973, 737);
            this.pnl_demo.TabIndex = 1;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1341, 762);
            this.Controls.Add(this.pnl_demo);
            this.Controls.Add(this.trv_demos);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trv_demos;
        private System.Windows.Forms.Panel pnl_demo;
    }
}

