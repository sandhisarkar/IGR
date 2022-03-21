namespace ImageHeaven
{
    partial class frmCancelScanning
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
            this.cmdCancelScan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdCancelScan
            // 
            this.cmdCancelScan.Location = new System.Drawing.Point(84, 27);
            this.cmdCancelScan.Name = "cmdCancelScan";
            this.cmdCancelScan.Size = new System.Drawing.Size(115, 23);
            this.cmdCancelScan.TabIndex = 0;
            this.cmdCancelScan.Text = "Cancel Scanning";
            this.cmdCancelScan.UseVisualStyleBackColor = true;
            // 
            // frmCancelScanning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 71);
            this.Controls.Add(this.cmdCancelScan);
            this.Name = "frmCancelScanning";
            this.Text = "frmCancelScanning";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancelScan;
    }
}