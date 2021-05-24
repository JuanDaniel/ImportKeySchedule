using ImportKeySchedule;

namespace BBI.JD.Forms
{
    partial class ImportKeySchedule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportKeySchedule));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ExcelPath = new System.Windows.Forms.TextBox();
            this.btn_Excel = new System.Windows.Forms.Button();
            this.lst_KeySchedules = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.chk_Purge = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_ColumnsKeySchedule = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_ColumnsExcel = new System.Windows.Forms.TextBox();
            this.chk_Overwrite = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txt_ExcelPath
            // 
            resources.ApplyResources(this.txt_ExcelPath, "txt_ExcelPath");
            this.txt_ExcelPath.Name = "txt_ExcelPath";
            this.txt_ExcelPath.ReadOnly = true;
            this.txt_ExcelPath.TextChanged += new System.EventHandler(this.txt_ExcelPath_TextChanged);
            // 
            // btn_Excel
            // 
            resources.ApplyResources(this.btn_Excel, "btn_Excel");
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.UseVisualStyleBackColor = true;
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // lst_KeySchedules
            // 
            resources.ApplyResources(this.lst_KeySchedules, "lst_KeySchedules");
            this.lst_KeySchedules.FormattingEnabled = true;
            this.lst_KeySchedules.Name = "lst_KeySchedules";
            this.lst_KeySchedules.SelectedIndexChanged += new System.EventHandler(this.lst_KeySchedules_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btn_Ok
            // 
            resources.ApplyResources(this.btn_Ok, "btn_Ok");
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // chk_Purge
            // 
            resources.ApplyResources(this.chk_Purge, "chk_Purge");
            this.chk_Purge.Name = "chk_Purge";
            this.chk_Purge.UseVisualStyleBackColor = true;
            this.chk_Purge.CheckedChanged += new System.EventHandler(this.chk_Purge_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txt_ColumnsKeySchedule
            // 
            resources.ApplyResources(this.txt_ColumnsKeySchedule, "txt_ColumnsKeySchedule");
            this.txt_ColumnsKeySchedule.Name = "txt_ColumnsKeySchedule";
            this.txt_ColumnsKeySchedule.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_ColumnsExcel
            // 
            resources.ApplyResources(this.txt_ColumnsExcel, "txt_ColumnsExcel");
            this.txt_ColumnsExcel.Name = "txt_ColumnsExcel";
            this.txt_ColumnsExcel.ReadOnly = true;
            // 
            // chk_Overwrite
            // 
            resources.ApplyResources(this.chk_Overwrite, "chk_Overwrite");
            this.chk_Overwrite.Name = "chk_Overwrite";
            this.chk_Overwrite.UseVisualStyleBackColor = true;
            // 
            // ImportKeySchedule
            // 
            this.AcceptButton = this.btn_Ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.Controls.Add(this.chk_Overwrite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_ColumnsExcel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_ColumnsKeySchedule);
            this.Controls.Add(this.chk_Purge);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lst_KeySchedules);
            this.Controls.Add(this.btn_Excel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_ExcelPath);
            this.MaximizeBox = false;
            this.Name = "ImportKeySchedule";
            this.Load += new System.EventHandler(this.ImportKeySchedule_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ExcelPath;
        private System.Windows.Forms.Button btn_Excel;
        private System.Windows.Forms.ListBox lst_KeySchedules;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chk_Purge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_ColumnsKeySchedule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_ColumnsExcel;
        private System.Windows.Forms.CheckBox chk_Overwrite;
    }
}