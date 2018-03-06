namespace HydroDesktop.Plugins.Search.Area
{
    partial class SelectAreaByAttributeDialog
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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblActiveLayer = new System.Windows.Forms.Label();
            this.cmbActiveLayer = new System.Windows.Forms.ComboBox();
            this.cmbField = new System.Windows.Forms.ComboBox();
            this.lblField = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.cmbValues = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(146, 172);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 21);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(227, 172);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblActiveLayer
            // 
            this.lblActiveLayer.AutoSize = true;
            this.lblActiveLayer.Location = new System.Drawing.Point(14, 18);
            this.lblActiveLayer.Name = "lblActiveLayer";
            this.lblActiveLayer.Size = new System.Drawing.Size(83, 12);
            this.lblActiveLayer.TabIndex = 2;
            this.lblActiveLayer.Text = "选择图层:";
            // 
            // cmbActiveLayer
            // 
            this.cmbActiveLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbActiveLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveLayer.FormattingEnabled = true;
            this.cmbActiveLayer.Location = new System.Drawing.Point(17, 33);
            this.cmbActiveLayer.Name = "cmbActiveLayer";
            this.cmbActiveLayer.Size = new System.Drawing.Size(288, 20);
            this.cmbActiveLayer.TabIndex = 0;
            // 
            // cmbField
            // 
            this.cmbField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbField.FormattingEnabled = true;
            this.cmbField.Location = new System.Drawing.Point(17, 75);
            this.cmbField.Name = "cmbField";
            this.cmbField.Size = new System.Drawing.Size(288, 20);
            this.cmbField.TabIndex = 1;
            // 
            // lblField
            // 
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(17, 60);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(41, 12);
            this.lblField.TabIndex = 4;
            this.lblField.Text = "选择字段:";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(17, 121);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(197, 12);
            this.lblValue.TabIndex = 6;
            this.lblValue.Text = "选择值: 输入一个字可自动检索相关联的值";
            // 
            // cmbValues
            // 
            this.cmbValues.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbValues.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbValues.FormattingEnabled = true;
            this.cmbValues.Location = new System.Drawing.Point(17, 136);
            this.cmbValues.Name = "cmbValues";
            this.cmbValues.Size = new System.Drawing.Size(288, 20);
            this.cmbValues.TabIndex = 7;
            this.cmbValues.SelectedValueChanged += new System.EventHandler(this.cmbValues_SelectedValueChanged);
            // 
            // SelectAreaByAttributeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(314, 204);
            this.Controls.Add(this.cmbValues);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.cmbField);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.cmbActiveLayer);
            this.Controls.Add(this.lblActiveLayer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnApply);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectAreaByAttributeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "根据属性选择区域";
            this.Load += new System.EventHandler(this.SelectAreaByAttributeDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblActiveLayer;
        private System.Windows.Forms.ComboBox cmbActiveLayer;
        private System.Windows.Forms.ComboBox cmbField;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ComboBox cmbValues;
    }
}