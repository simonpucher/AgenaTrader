namespace WindowsFormsApplication1
{
    partial class Main
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
            this.btn_start = new System.Windows.Forms.Button();
            this.txt_input_search = new System.Windows.Forms.TextBox();
            this.grpbox_type = new System.Windows.Forms.GroupBox();
            this.rbtn_Name = new System.Windows.Forms.RadioButton();
            this.rbtn_Isin = new System.Windows.Forms.RadioButton();
            this.rbtn_agena = new System.Windows.Forms.RadioButton();
            this.grpbox_exchange = new System.Windows.Forms.GroupBox();
            this.rbtn_xetra = new System.Windows.Forms.RadioButton();
            this.rbtn_nasdaq = new System.Windows.Forms.RadioButton();
            this.rbtn_NYSE = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtn_Stocks = new System.Windows.Forms.RadioButton();
            this.rbtn_indices = new System.Windows.Forms.RadioButton();
            this.lstvw_instruments = new System.Windows.Forms.ListView();
            this.grpbox_type.SuspendLayout();
            this.grpbox_exchange.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(403, 164);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(151, 35);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Search";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // txt_input_search
            // 
            this.txt_input_search.Location = new System.Drawing.Point(12, 172);
            this.txt_input_search.Name = "txt_input_search";
            this.txt_input_search.Size = new System.Drawing.Size(385, 20);
            this.txt_input_search.TabIndex = 1;
            this.txt_input_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_input_search_KeyDown);
            // 
            // grpbox_type
            // 
            this.grpbox_type.Controls.Add(this.rbtn_agena);
            this.grpbox_type.Controls.Add(this.rbtn_Isin);
            this.grpbox_type.Controls.Add(this.rbtn_Name);
            this.grpbox_type.Location = new System.Drawing.Point(12, 13);
            this.grpbox_type.Name = "grpbox_type";
            this.grpbox_type.Size = new System.Drawing.Size(542, 44);
            this.grpbox_type.TabIndex = 2;
            this.grpbox_type.TabStop = false;
            this.grpbox_type.Tag = "";
            this.grpbox_type.Text = "Search type";
            // 
            // rbtn_Name
            // 
            this.rbtn_Name.AutoSize = true;
            this.rbtn_Name.Checked = true;
            this.rbtn_Name.Location = new System.Drawing.Point(65, 19);
            this.rbtn_Name.Name = "rbtn_Name";
            this.rbtn_Name.Size = new System.Drawing.Size(53, 17);
            this.rbtn_Name.TabIndex = 0;
            this.rbtn_Name.Text = "Name";
            this.rbtn_Name.UseVisualStyleBackColor = true;
            // 
            // rbtn_Isin
            // 
            this.rbtn_Isin.AutoSize = true;
            this.rbtn_Isin.Location = new System.Drawing.Point(124, 19);
            this.rbtn_Isin.Name = "rbtn_Isin";
            this.rbtn_Isin.Size = new System.Drawing.Size(46, 17);
            this.rbtn_Isin.TabIndex = 1;
            this.rbtn_Isin.Text = "ISIN";
            this.rbtn_Isin.UseVisualStyleBackColor = true;
            // 
            // rbtn_agena
            // 
            this.rbtn_agena.AutoSize = true;
            this.rbtn_agena.Location = new System.Drawing.Point(6, 19);
            this.rbtn_agena.Name = "rbtn_agena";
            this.rbtn_agena.Size = new System.Drawing.Size(56, 17);
            this.rbtn_agena.TabIndex = 2;
            this.rbtn_agena.Text = "Agena";
            this.rbtn_agena.UseVisualStyleBackColor = true;
            // 
            // grpbox_exchange
            // 
            this.grpbox_exchange.Controls.Add(this.rbtn_xetra);
            this.grpbox_exchange.Controls.Add(this.rbtn_nasdaq);
            this.grpbox_exchange.Controls.Add(this.rbtn_NYSE);
            this.grpbox_exchange.Location = new System.Drawing.Point(12, 67);
            this.grpbox_exchange.Name = "grpbox_exchange";
            this.grpbox_exchange.Size = new System.Drawing.Size(542, 44);
            this.grpbox_exchange.TabIndex = 3;
            this.grpbox_exchange.TabStop = false;
            this.grpbox_exchange.Tag = "";
            this.grpbox_exchange.Text = "Exchange";
            // 
            // rbtn_xetra
            // 
            this.rbtn_xetra.AutoSize = true;
            this.rbtn_xetra.Checked = true;
            this.rbtn_xetra.Location = new System.Drawing.Point(6, 19);
            this.rbtn_xetra.Name = "rbtn_xetra";
            this.rbtn_xetra.Size = new System.Drawing.Size(50, 17);
            this.rbtn_xetra.TabIndex = 2;
            this.rbtn_xetra.TabStop = true;
            this.rbtn_xetra.Text = "Xetra";
            this.rbtn_xetra.UseVisualStyleBackColor = true;
            // 
            // rbtn_nasdaq
            // 
            this.rbtn_nasdaq.AutoSize = true;
            this.rbtn_nasdaq.Location = new System.Drawing.Point(124, 19);
            this.rbtn_nasdaq.Name = "rbtn_nasdaq";
            this.rbtn_nasdaq.Size = new System.Drawing.Size(62, 17);
            this.rbtn_nasdaq.TabIndex = 1;
            this.rbtn_nasdaq.Text = "Nasdaq";
            this.rbtn_nasdaq.UseVisualStyleBackColor = true;
            // 
            // rbtn_NYSE
            // 
            this.rbtn_NYSE.AutoSize = true;
            this.rbtn_NYSE.Location = new System.Drawing.Point(65, 19);
            this.rbtn_NYSE.Name = "rbtn_NYSE";
            this.rbtn_NYSE.Size = new System.Drawing.Size(54, 17);
            this.rbtn_NYSE.TabIndex = 0;
            this.rbtn_NYSE.Text = "NYSE";
            this.rbtn_NYSE.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtn_Stocks);
            this.groupBox1.Controls.Add(this.rbtn_indices);
            this.groupBox1.Location = new System.Drawing.Point(12, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(542, 44);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "";
            this.groupBox1.Text = "Stock type";
            // 
            // rbtn_Stocks
            // 
            this.rbtn_Stocks.AutoSize = true;
            this.rbtn_Stocks.Checked = true;
            this.rbtn_Stocks.Location = new System.Drawing.Point(6, 19);
            this.rbtn_Stocks.Name = "rbtn_Stocks";
            this.rbtn_Stocks.Size = new System.Drawing.Size(58, 17);
            this.rbtn_Stocks.TabIndex = 2;
            this.rbtn_Stocks.TabStop = true;
            this.rbtn_Stocks.Text = "Stocks";
            this.rbtn_Stocks.UseVisualStyleBackColor = true;
            // 
            // rbtn_indices
            // 
            this.rbtn_indices.AutoSize = true;
            this.rbtn_indices.Location = new System.Drawing.Point(65, 19);
            this.rbtn_indices.Name = "rbtn_indices";
            this.rbtn_indices.Size = new System.Drawing.Size(59, 17);
            this.rbtn_indices.TabIndex = 0;
            this.rbtn_indices.Text = "Indices";
            this.rbtn_indices.UseVisualStyleBackColor = true;
            // 
            // lstvw_instruments
            // 
            this.lstvw_instruments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstvw_instruments.FullRowSelect = true;
            this.lstvw_instruments.GridLines = true;
            this.lstvw_instruments.Location = new System.Drawing.Point(12, 220);
            this.lstvw_instruments.MultiSelect = false;
            this.lstvw_instruments.Name = "lstvw_instruments";
            this.lstvw_instruments.Size = new System.Drawing.Size(542, 229);
            this.lstvw_instruments.TabIndex = 5;
            this.lstvw_instruments.UseCompatibleStateImageBehavior = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.lstvw_instruments);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpbox_exchange);
            this.Controls.Add(this.grpbox_type);
            this.Controls.Add(this.txt_input_search);
            this.Controls.Add(this.btn_start);
            this.Name = "Main";
            this.Text = "TaiPan Data Access";
            this.Load += new System.EventHandler(this.Main_Load);
            this.grpbox_type.ResumeLayout(false);
            this.grpbox_type.PerformLayout();
            this.grpbox_exchange.ResumeLayout(false);
            this.grpbox_exchange.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.TextBox txt_input_search;
        private System.Windows.Forms.GroupBox grpbox_type;
        private System.Windows.Forms.RadioButton rbtn_Isin;
        private System.Windows.Forms.RadioButton rbtn_Name;
        private System.Windows.Forms.RadioButton rbtn_agena;
        private System.Windows.Forms.GroupBox grpbox_exchange;
        private System.Windows.Forms.RadioButton rbtn_xetra;
        private System.Windows.Forms.RadioButton rbtn_nasdaq;
        private System.Windows.Forms.RadioButton rbtn_NYSE;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtn_Stocks;
        private System.Windows.Forms.RadioButton rbtn_indices;
        private System.Windows.Forms.ListView lstvw_instruments;
    }
}

