namespace CSMSL.Examples
{
    partial class PeptideCalculatorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.sequenceTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.peptideMassTB = new System.Windows.Forms.TextBox();
            this.peptideMZTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Peptide Sequence";
            // 
            // sequenceTB
            // 
            this.sequenceTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sequenceTB.Location = new System.Drawing.Point(15, 25);
            this.sequenceTB.Name = "sequenceTB";
            this.sequenceTB.Size = new System.Drawing.Size(257, 20);
            this.sequenceTB.TabIndex = 1;
            this.sequenceTB.TextChanged += new System.EventHandler(this.sequenceTB_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Peptide Mass";
            // 
            // peptideMassTB
            // 
            this.peptideMassTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.peptideMassTB.Location = new System.Drawing.Point(15, 64);
            this.peptideMassTB.Name = "peptideMassTB";
            this.peptideMassTB.ReadOnly = true;
            this.peptideMassTB.Size = new System.Drawing.Size(257, 20);
            this.peptideMassTB.TabIndex = 3;
            // 
            // peptideMZTB
            // 
            this.peptideMZTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.peptideMZTB.Location = new System.Drawing.Point(15, 103);
            this.peptideMZTB.Name = "peptideMZTB";
            this.peptideMZTB.ReadOnly = true;
            this.peptideMZTB.Size = new System.Drawing.Size(257, 20);
            this.peptideMZTB.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Peptide m/z";
            // 
            // PeptideCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 128);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.peptideMZTB);
            this.Controls.Add(this.peptideMassTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sequenceTB);
            this.Controls.Add(this.label1);
            this.Name = "PeptideCalculatorForm";
            this.ShowIcon = false;
            this.Text = "Peptide Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sequenceTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox peptideMassTB;
        private System.Windows.Forms.TextBox peptideMZTB;
        private System.Windows.Forms.Label label3;
    }
}