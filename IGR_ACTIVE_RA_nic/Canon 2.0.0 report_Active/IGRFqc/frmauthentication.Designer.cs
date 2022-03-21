namespace IGRFqc
{
    partial class frmauthentication
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
        	this.grpPassword = new System.Windows.Forms.GroupBox();
        	this.txtUserpass = new nControls.deTextBox();
        	this.cmdAuth = new nControls.deButton();
        	this.grpPassword.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// grpPassword
        	// 
        	this.grpPassword.Controls.Add(this.txtUserpass);
        	this.grpPassword.Controls.Add(this.cmdAuth);
        	this.grpPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.grpPassword.Location = new System.Drawing.Point(0, 0);
        	this.grpPassword.Name = "grpPassword";
        	this.grpPassword.Size = new System.Drawing.Size(281, 49);
        	this.grpPassword.TabIndex = 0;
        	this.grpPassword.TabStop = false;
        	this.grpPassword.Text = "Please Validate your Password to Continue..";
        	// 
        	// txtUserpass
        	// 
        	this.txtUserpass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.txtUserpass.Location = new System.Drawing.Point(13, 20);
        	this.txtUserpass.Name = "txtUserpass";
        	this.txtUserpass.PasswordChar = '*';
        	this.txtUserpass.Size = new System.Drawing.Size(202, 23);
        	this.txtUserpass.TabIndex = 2;
        	// 
        	// cmdAuth
        	// 
        	this.cmdAuth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
        	this.cmdAuth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.cmdAuth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.cmdAuth.Location = new System.Drawing.Point(221, 20);
        	this.cmdAuth.Name = "cmdAuth";
        	this.cmdAuth.Size = new System.Drawing.Size(54, 23);
        	this.cmdAuth.TabIndex = 1;
        	this.cmdAuth.Text = "OK";
        	this.cmdAuth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        	this.cmdAuth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
        	this.cmdAuth.UseCompatibleTextRendering = true;
        	this.cmdAuth.UseVisualStyleBackColor = true;
        	this.cmdAuth.Click += new System.EventHandler(this.cmdAuth_Click);
        	// 
        	// frmauthentication
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.BackColor = System.Drawing.Color.White;
        	this.ClientSize = new System.Drawing.Size(283, 52);
        	this.Controls.Add(this.grpPassword);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        	this.KeyPreview = true;
        	this.Name = "frmauthentication";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.Load += new System.EventHandler(this.FrmauthenticationLoad);
        	this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmauthentication_KeyUp);
        	this.grpPassword.ResumeLayout(false);
        	this.grpPassword.PerformLayout();
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpPassword;
        private nControls.deButton cmdAuth;
        private nControls.deTextBox txtUserpass;
    }
}