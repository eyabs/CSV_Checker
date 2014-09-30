namespace CSV_checker
{
    partial class frm_CSV_checker
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
            this.btn_select_csv = new System.Windows.Forms.Button();
            this.btn_check_errors = new System.Windows.Forms.Button();
            this.txtbox_selected_file = new System.Windows.Forms.TextBox();
            this.txtbox_file_errors = new System.Windows.Forms.TextBox();
            this.ofd_CVS_selector = new System.Windows.Forms.OpenFileDialog();
            this.txtbox_status = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_select_csv
            // 
            this.btn_select_csv.Location = new System.Drawing.Point(6, 9);
            this.btn_select_csv.Name = "btn_select_csv";
            this.btn_select_csv.Size = new System.Drawing.Size(100, 23);
            this.btn_select_csv.TabIndex = 0;
            this.btn_select_csv.Text = "Select CSV File";
            this.btn_select_csv.UseVisualStyleBackColor = true;
            this.btn_select_csv.Click += new System.EventHandler(this.btn_select_csv_Click);
            // 
            // btn_check_errors
            // 
            this.btn_check_errors.Location = new System.Drawing.Point(6, 41);
            this.btn_check_errors.Name = "btn_check_errors";
            this.btn_check_errors.Size = new System.Drawing.Size(100, 23);
            this.btn_check_errors.TabIndex = 1;
            this.btn_check_errors.Text = "Check For Errors";
            this.btn_check_errors.UseVisualStyleBackColor = true;
            this.btn_check_errors.Click += new System.EventHandler(this.btn_check_errors_Click);
            // 
            // txtbox_selected_file
            // 
            this.txtbox_selected_file.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtbox_selected_file.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbox_selected_file.Location = new System.Drawing.Point(112, 12);
            this.txtbox_selected_file.Name = "txtbox_selected_file";
            this.txtbox_selected_file.ReadOnly = true;
            this.txtbox_selected_file.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtbox_selected_file.Size = new System.Drawing.Size(660, 22);
            this.txtbox_selected_file.TabIndex = 2;
            this.txtbox_selected_file.TextChanged += new System.EventHandler(this.txtbox_selected_file_TextChanged);
            // 
            // txtbox_file_errors
            // 
            this.txtbox_file_errors.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtbox_file_errors.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtbox_file_errors.Location = new System.Drawing.Point(112, 44);
            this.txtbox_file_errors.Multiline = true;
            this.txtbox_file_errors.Name = "txtbox_file_errors";
            this.txtbox_file_errors.ReadOnly = true;
            this.txtbox_file_errors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtbox_file_errors.Size = new System.Drawing.Size(660, 206);
            this.txtbox_file_errors.TabIndex = 3;
            this.txtbox_file_errors.TextChanged += new System.EventHandler(this.txtbox_file_errors_TextChanged);
            // 
            // ofd_CVS_selector
            // 
            this.ofd_CVS_selector.FileName = "ofd_selected_csv";
            this.ofd_CVS_selector.Filter = "CSV Text Files|*.csv|All Files|*.*";
            // 
            // txtbox_status
            // 
            this.txtbox_status.AcceptsTab = true;
            this.txtbox_status.BackColor = System.Drawing.SystemColors.Control;
            this.txtbox_status.Location = new System.Drawing.Point(112, 256);
            this.txtbox_status.Name = "txtbox_status";
            this.txtbox_status.ReadOnly = true;
            this.txtbox_status.Size = new System.Drawing.Size(660, 20);
            this.txtbox_status.TabIndex = 4;
            // 
            // frm_CSV_checker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 283);
            this.Controls.Add(this.txtbox_status);
            this.Controls.Add(this.txtbox_file_errors);
            this.Controls.Add(this.txtbox_selected_file);
            this.Controls.Add(this.btn_check_errors);
            this.Controls.Add(this.btn_select_csv);
            this.Name = "frm_CSV_checker";
            this.Text = "CSV Checker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_select_csv;
        private System.Windows.Forms.Button btn_check_errors;
        private System.Windows.Forms.TextBox txtbox_selected_file;
        private System.Windows.Forms.TextBox txtbox_file_errors;
        private System.Windows.Forms.OpenFileDialog ofd_CVS_selector;
        private System.Windows.Forms.TextBox txtbox_status;
    }
}

