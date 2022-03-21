namespace IGRFqc
{
    partial class frmOutsidewbsugg
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
            this.grpDisplay = new System.Windows.Forms.GroupBox();
            this.cmdDone = new nControls.deButton();
            this.cmdAcceptResults = new nControls.deButton();
            this.grdResult = new System.Windows.Forms.DataGridView();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblMouza = new System.Windows.Forms.Label();
            this.lblPS = new System.Windows.Forms.Label();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.cmdSearch = new nControls.deButton();
            this.txtDistrict = new nControls.deTextBox();
            this.txtstate = new nControls.deTextBox();
            this.txtCountry = new nControls.deTextBox();
            this.grpDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).BeginInit();
            this.grpSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDisplay
            // 
            this.grpDisplay.Controls.Add(this.cmdDone);
            this.grpDisplay.Controls.Add(this.grdResult);
            this.grpDisplay.Controls.Add(this.cmdAcceptResults);
            this.grpDisplay.Enabled = false;
            this.grpDisplay.Location = new System.Drawing.Point(5, 62);
            this.grpDisplay.Name = "grpDisplay";
            this.grpDisplay.Size = new System.Drawing.Size(599, 193);
            this.grpDisplay.TabIndex = 3;
            this.grpDisplay.TabStop = false;
            // 
            // cmdDone
            // 
            this.cmdDone.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDone.Location = new System.Drawing.Point(475, 161);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.Size = new System.Drawing.Size(106, 23);
            this.cmdDone.TabIndex = 7;
            this.cmdDone.Text = "&I\'m Accepting it!";
            this.cmdDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDone.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdDone.UseCompatibleTextRendering = true;
            this.cmdDone.UseVisualStyleBackColor = true;
            this.cmdDone.Click += new System.EventHandler(this.cmdDone_Click);
            // 
            // cmdAcceptResults
            // 
            this.cmdAcceptResults.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdAcceptResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAcceptResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAcceptResults.Location = new System.Drawing.Point(399, 161);
            this.cmdAcceptResults.Name = "cmdAcceptResults";
            this.cmdAcceptResults.Size = new System.Drawing.Size(15, 23);
            this.cmdAcceptResults.TabIndex = 3;
            this.cmdAcceptResults.Text = "&I\'m Accepting it!";
            this.cmdAcceptResults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAcceptResults.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdAcceptResults.UseCompatibleTextRendering = true;
            this.cmdAcceptResults.UseVisualStyleBackColor = true;
            this.cmdAcceptResults.Visible = false;
            this.cmdAcceptResults.Click += new System.EventHandler(this.cmdAcceptResults_Click);
            // 
            // grdResult
            // 
            this.grdResult.AllowUserToAddRows = false;
            this.grdResult.AllowUserToDeleteRows = false;
            this.grdResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResult.Location = new System.Drawing.Point(12, 16);
            this.grdResult.MultiSelect = false;
            this.grdResult.Name = "grdResult";
            this.grdResult.RowHeadersVisible = false;
            this.grdResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdResult.Size = new System.Drawing.Size(575, 135);
            this.grdResult.TabIndex = 2;
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.lblMouza);
            this.grpSearch.Controls.Add(this.lblPS);
            this.grpSearch.Controls.Add(this.lblDistrict);
            this.grpSearch.Controls.Add(this.cmdSearch);
            this.grpSearch.Controls.Add(this.txtDistrict);
            this.grpSearch.Controls.Add(this.txtstate);
            this.grpSearch.Controls.Add(this.txtCountry);
            this.grpSearch.Location = new System.Drawing.Point(4, 4);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(600, 52);
            this.grpSearch.TabIndex = 2;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search Criteria";
            // 
            // lblMouza
            // 
            this.lblMouza.AutoSize = true;
            this.lblMouza.Location = new System.Drawing.Point(351, 21);
            this.lblMouza.Name = "lblMouza";
            this.lblMouza.Size = new System.Drawing.Size(39, 13);
            this.lblMouza.TabIndex = 7;
            this.lblMouza.Text = "&District";
            // 
            // lblPS
            // 
            this.lblPS.AutoSize = true;
            this.lblPS.Location = new System.Drawing.Point(162, 25);
            this.lblPS.Name = "lblPS";
            this.lblPS.Size = new System.Drawing.Size(32, 13);
            this.lblPS.TabIndex = 6;
            this.lblPS.Text = "&State";
            // 
            // lblDistrict
            // 
            this.lblDistrict.AutoSize = true;
            this.lblDistrict.Location = new System.Drawing.Point(9, 22);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(43, 13);
            this.lblDistrict.TabIndex = 5;
            this.lblDistrict.Text = "&Country";
            // 
            // cmdSearch
            // 
            this.cmdSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSearch.Location = new System.Drawing.Point(510, 17);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(75, 23);
            this.cmdSearch.TabIndex = 4;
            this.cmdSearch.Text = "&Search";
            this.cmdSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdSearch.UseCompatibleTextRendering = true;
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // txtDistrict
            // 
            this.txtDistrict.BackColor = System.Drawing.Color.White;
            this.txtDistrict.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDistrict.ForeColor = System.Drawing.Color.Black;
            this.txtDistrict.Location = new System.Drawing.Point(392, 17);
            this.txtDistrict.Mandatory = true;
            this.txtDistrict.Name = "txtDistrict";
            this.txtDistrict.Size = new System.Drawing.Size(100, 23);
            this.txtDistrict.TabIndex = 2;
            // 
            // txtstate
            // 
            this.txtstate.BackColor = System.Drawing.Color.White;
            this.txtstate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtstate.ForeColor = System.Drawing.Color.Black;
            this.txtstate.Location = new System.Drawing.Point(198, 17);
            this.txtstate.Mandatory = true;
            this.txtstate.Name = "txtstate";
            this.txtstate.Size = new System.Drawing.Size(147, 23);
            this.txtstate.TabIndex = 1;
            // 
            // txtCountry
            // 
            this.txtCountry.BackColor = System.Drawing.Color.White;
            this.txtCountry.Enabled = false;
            this.txtCountry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCountry.ForeColor = System.Drawing.Color.Black;
            this.txtCountry.Location = new System.Drawing.Point(56, 18);
            this.txtCountry.Mandatory = true;
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(100, 23);
            this.txtCountry.TabIndex = 0;
            this.txtCountry.Text = "India";
            // 
            // frmOutsidewbsugg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 263);
            this.Controls.Add(this.grpDisplay);
            this.Controls.Add(this.grpSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmOutsidewbsugg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmOutsidewbsugg";
            this.Load += new System.EventHandler(this.frmOutsidewbsugg_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmOutsidewbsugg_KeyUp);
            this.grpDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResult)).EndInit();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDisplay;
        private nControls.deButton cmdAcceptResults;
        private System.Windows.Forms.DataGridView grdResult;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label lblMouza;
        private System.Windows.Forms.Label lblPS;
        private System.Windows.Forms.Label lblDistrict;
        private nControls.deButton cmdSearch;
        private nControls.deTextBox txtDistrict;
        private nControls.deTextBox txtstate;
        private nControls.deTextBox txtCountry;
        private nControls.deButton cmdDone;
    }
}