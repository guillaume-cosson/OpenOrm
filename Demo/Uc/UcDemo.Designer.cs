
namespace Demo.Uc
{
    partial class UcDemo
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_title = new System.Windows.Forms.Label();
            this.lbl_description = new System.Windows.Forms.Label();
            this.rtb_code = new System.Windows.Forms.RichTextBox();
            this.dgv_result = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_result)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_title
            // 
            this.lbl_title.AutoSize = true;
            this.lbl_title.Location = new System.Drawing.Point(0, 0);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(38, 15);
            this.lbl_title.TabIndex = 0;
            this.lbl_title.Text = "label1";
            // 
            // lbl_description
            // 
            this.lbl_description.Location = new System.Drawing.Point(10, 24);
            this.lbl_description.Name = "lbl_description";
            this.lbl_description.Size = new System.Drawing.Size(1094, 97);
            this.lbl_description.TabIndex = 0;
            this.lbl_description.Text = "label1";
            // 
            // rtb_code
            // 
            this.rtb_code.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtb_code.Location = new System.Drawing.Point(0, 124);
            this.rtb_code.Name = "rtb_code";
            this.rtb_code.Size = new System.Drawing.Size(1297, 333);
            this.rtb_code.TabIndex = 2;
            this.rtb_code.Text = "";
            // 
            // dgv_result
            // 
            this.dgv_result.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_result.Location = new System.Drawing.Point(0, 463);
            this.dgv_result.Name = "dgv_result";
            this.dgv_result.RowTemplate.Height = 25;
            this.dgv_result.Size = new System.Drawing.Size(1297, 307);
            this.dgv_result.TabIndex = 3;
            // 
            // UcDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv_result);
            this.Controls.Add(this.lbl_description);
            this.Controls.Add(this.rtb_code);
            this.Controls.Add(this.lbl_title);
            this.Name = "UcDemo";
            this.Size = new System.Drawing.Size(1297, 770);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.Label lbl_description;
        private System.Windows.Forms.RichTextBox rtb_code;
        private System.Windows.Forms.DataGridView dgv_result;
    }
}
