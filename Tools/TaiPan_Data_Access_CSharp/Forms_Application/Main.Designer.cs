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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabpg_search = new System.Windows.Forms.TabPage();
            this.lbl_data_loaded = new System.Windows.Forms.Label();
            this.lbl_selected_instrument = new System.Windows.Forms.Label();
            this.dtp_to = new System.Windows.Forms.DateTimePicker();
            this.btn_loaddata = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.lstvw_instruments = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtn_Stocks = new System.Windows.Forms.RadioButton();
            this.rbtn_indices = new System.Windows.Forms.RadioButton();
            this.grpbox_exchange = new System.Windows.Forms.GroupBox();
            this.rbtn_xetra = new System.Windows.Forms.RadioButton();
            this.rbtn_nasdaq = new System.Windows.Forms.RadioButton();
            this.rbtn_NYSE = new System.Windows.Forms.RadioButton();
            this.grpbox_type = new System.Windows.Forms.GroupBox();
            this.rbtn_agena = new System.Windows.Forms.RadioButton();
            this.rbtn_Isin = new System.Windows.Forms.RadioButton();
            this.rbtn_Name = new System.Windows.Forms.RadioButton();
            this.txt_input_search = new System.Windows.Forms.TextBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.rtb_exchange_dontcare = new System.Windows.Forms.RadioButton();
            this.rtb_stocktype_dontcare = new System.Windows.Forms.RadioButton();
            this.tabs.SuspendLayout();
            this.tabpg_search.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpbox_exchange.SuspendLayout();
            this.grpbox_type.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabpg_search);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(810, 437);
            this.tabs.TabIndex = 0;
            // 
            // tabpg_search
            // 
            this.tabpg_search.Controls.Add(this.lbl_data_loaded);
            this.tabpg_search.Controls.Add(this.lbl_selected_instrument);
            this.tabpg_search.Controls.Add(this.dtp_to);
            this.tabpg_search.Controls.Add(this.btn_loaddata);
            this.tabpg_search.Controls.Add(this.btn_export);
            this.tabpg_search.Controls.Add(this.lstvw_instruments);
            this.tabpg_search.Controls.Add(this.groupBox1);
            this.tabpg_search.Controls.Add(this.grpbox_exchange);
            this.tabpg_search.Controls.Add(this.grpbox_type);
            this.tabpg_search.Controls.Add(this.txt_input_search);
            this.tabpg_search.Controls.Add(this.btn_start);
            this.tabpg_search.Location = new System.Drawing.Point(4, 22);
            this.tabpg_search.Name = "tabpg_search";
            this.tabpg_search.Padding = new System.Windows.Forms.Padding(3);
            this.tabpg_search.Size = new System.Drawing.Size(802, 411);
            this.tabpg_search.TabIndex = 0;
            this.tabpg_search.Text = "Search";
            this.tabpg_search.UseVisualStyleBackColor = true;
            // 
            // lbl_data_loaded
            // 
            this.lbl_data_loaded.AutoSize = true;
            this.lbl_data_loaded.Location = new System.Drawing.Point(571, 194);
            this.lbl_data_loaded.Name = "lbl_data_loaded";
            this.lbl_data_loaded.Size = new System.Drawing.Size(78, 13);
            this.lbl_data_loaded.TabIndex = 16;
            this.lbl_data_loaded.Text = "no data loaded";
            // 
            // lbl_selected_instrument
            // 
            this.lbl_selected_instrument.AutoSize = true;
            this.lbl_selected_instrument.Location = new System.Drawing.Point(567, 25);
            this.lbl_selected_instrument.Name = "lbl_selected_instrument";
            this.lbl_selected_instrument.Size = new System.Drawing.Size(113, 13);
            this.lbl_selected_instrument.TabIndex = 15;
            this.lbl_selected_instrument.Text = "no instrument selected";
            // 
            // dtp_to
            // 
            this.dtp_to.Location = new System.Drawing.Point(569, 42);
            this.dtp_to.Name = "dtp_to";
            this.dtp_to.Size = new System.Drawing.Size(208, 20);
            this.dtp_to.TabIndex = 14;
            // 
            // btn_loaddata
            // 
            this.btn_loaddata.Location = new System.Drawing.Point(569, 68);
            this.btn_loaddata.Name = "btn_loaddata";
            this.btn_loaddata.Size = new System.Drawing.Size(207, 43);
            this.btn_loaddata.TabIndex = 13;
            this.btn_loaddata.Text = "Load Data";
            this.btn_loaddata.UseVisualStyleBackColor = true;
            this.btn_loaddata.Click += new System.EventHandler(this.btn_loaddata_Click);
            // 
            // btn_export
            // 
            this.btn_export.Location = new System.Drawing.Point(570, 214);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(207, 43);
            this.btn_export.TabIndex = 12;
            this.btn_export.Text = "Export";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // lstvw_instruments
            // 
            this.lstvw_instruments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstvw_instruments.FullRowSelect = true;
            this.lstvw_instruments.GridLines = true;
            this.lstvw_instruments.Location = new System.Drawing.Point(3, 198);
            this.lstvw_instruments.MultiSelect = false;
            this.lstvw_instruments.Name = "lstvw_instruments";
            this.lstvw_instruments.Size = new System.Drawing.Size(542, 204);
            this.lstvw_instruments.TabIndex = 11;
            this.lstvw_instruments.UseCompatibleStateImageBehavior = false;
            this.lstvw_instruments.SelectedIndexChanged += new System.EventHandler(this.lstvw_instruments_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtb_stocktype_dontcare);
            this.groupBox1.Controls.Add(this.rbtn_Stocks);
            this.groupBox1.Controls.Add(this.rbtn_indices);
            this.groupBox1.Location = new System.Drawing.Point(3, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(542, 44);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "";
            this.groupBox1.Text = "Stock type";
            // 
            // rbtn_Stocks
            // 
            this.rbtn_Stocks.AutoSize = true;
            this.rbtn_Stocks.Location = new System.Drawing.Point(91, 17);
            this.rbtn_Stocks.Name = "rbtn_Stocks";
            this.rbtn_Stocks.Size = new System.Drawing.Size(58, 17);
            this.rbtn_Stocks.TabIndex = 2;
            this.rbtn_Stocks.Text = "Stocks";
            this.rbtn_Stocks.UseVisualStyleBackColor = true;
            // 
            // rbtn_indices
            // 
            this.rbtn_indices.AutoSize = true;
            this.rbtn_indices.Location = new System.Drawing.Point(150, 17);
            this.rbtn_indices.Name = "rbtn_indices";
            this.rbtn_indices.Size = new System.Drawing.Size(59, 17);
            this.rbtn_indices.TabIndex = 0;
            this.rbtn_indices.Text = "Indices";
            this.rbtn_indices.UseVisualStyleBackColor = true;
            // 
            // grpbox_exchange
            // 
            this.grpbox_exchange.Controls.Add(this.rtb_exchange_dontcare);
            this.grpbox_exchange.Controls.Add(this.rbtn_xetra);
            this.grpbox_exchange.Controls.Add(this.rbtn_nasdaq);
            this.grpbox_exchange.Controls.Add(this.rbtn_NYSE);
            this.grpbox_exchange.Location = new System.Drawing.Point(3, 60);
            this.grpbox_exchange.Name = "grpbox_exchange";
            this.grpbox_exchange.Size = new System.Drawing.Size(542, 44);
            this.grpbox_exchange.TabIndex = 9;
            this.grpbox_exchange.TabStop = false;
            this.grpbox_exchange.Tag = "";
            this.grpbox_exchange.Text = "Exchange";
            // 
            // rbtn_xetra
            // 
            this.rbtn_xetra.AutoSize = true;
            this.rbtn_xetra.Location = new System.Drawing.Point(90, 19);
            this.rbtn_xetra.Name = "rbtn_xetra";
            this.rbtn_xetra.Size = new System.Drawing.Size(50, 17);
            this.rbtn_xetra.TabIndex = 2;
            this.rbtn_xetra.Text = "Xetra";
            this.rbtn_xetra.UseVisualStyleBackColor = true;
            // 
            // rbtn_nasdaq
            // 
            this.rbtn_nasdaq.AutoSize = true;
            this.rbtn_nasdaq.Location = new System.Drawing.Point(208, 19);
            this.rbtn_nasdaq.Name = "rbtn_nasdaq";
            this.rbtn_nasdaq.Size = new System.Drawing.Size(62, 17);
            this.rbtn_nasdaq.TabIndex = 1;
            this.rbtn_nasdaq.Text = "Nasdaq";
            this.rbtn_nasdaq.UseVisualStyleBackColor = true;
            // 
            // rbtn_NYSE
            // 
            this.rbtn_NYSE.AutoSize = true;
            this.rbtn_NYSE.Location = new System.Drawing.Point(149, 19);
            this.rbtn_NYSE.Name = "rbtn_NYSE";
            this.rbtn_NYSE.Size = new System.Drawing.Size(54, 17);
            this.rbtn_NYSE.TabIndex = 0;
            this.rbtn_NYSE.Text = "NYSE";
            this.rbtn_NYSE.UseVisualStyleBackColor = true;
            // 
            // grpbox_type
            // 
            this.grpbox_type.Controls.Add(this.rbtn_agena);
            this.grpbox_type.Controls.Add(this.rbtn_Isin);
            this.grpbox_type.Controls.Add(this.rbtn_Name);
            this.grpbox_type.Location = new System.Drawing.Point(3, 6);
            this.grpbox_type.Name = "grpbox_type";
            this.grpbox_type.Size = new System.Drawing.Size(542, 44);
            this.grpbox_type.TabIndex = 8;
            this.grpbox_type.TabStop = false;
            this.grpbox_type.Tag = "";
            this.grpbox_type.Text = "Search type";
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
            // rbtn_Name
            // 
            this.rbtn_Name.AutoSize = true;
            this.rbtn_Name.Checked = true;
            this.rbtn_Name.Location = new System.Drawing.Point(65, 19);
            this.rbtn_Name.Name = "rbtn_Name";
            this.rbtn_Name.Size = new System.Drawing.Size(53, 17);
            this.rbtn_Name.TabIndex = 0;
            this.rbtn_Name.TabStop = true;
            this.rbtn_Name.Text = "Name";
            this.rbtn_Name.UseVisualStyleBackColor = true;
            // 
            // txt_input_search
            // 
            this.txt_input_search.Location = new System.Drawing.Point(3, 165);
            this.txt_input_search.Name = "txt_input_search";
            this.txt_input_search.Size = new System.Drawing.Size(385, 20);
            this.txt_input_search.TabIndex = 7;
            this.txt_input_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_input_search_KeyDown);
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(394, 157);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(151, 35);
            this.btn_start.TabIndex = 6;
            this.btn_start.Text = "Search";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // rtb_exchange_dontcare
            // 
            this.rtb_exchange_dontcare.AutoSize = true;
            this.rtb_exchange_dontcare.Checked = true;
            this.rtb_exchange_dontcare.Location = new System.Drawing.Point(7, 19);
            this.rtb_exchange_dontcare.Name = "rtb_exchange_dontcare";
            this.rtb_exchange_dontcare.Size = new System.Drawing.Size(76, 17);
            this.rtb_exchange_dontcare.TabIndex = 3;
            this.rtb_exchange_dontcare.Text = "I dont care";
            this.rtb_exchange_dontcare.UseVisualStyleBackColor = true;
            // 
            // rtb_stocktype_dontcare
            // 
            this.rtb_stocktype_dontcare.AutoSize = true;
            this.rtb_stocktype_dontcare.Checked = true;
            this.rtb_stocktype_dontcare.Location = new System.Drawing.Point(7, 19);
            this.rtb_stocktype_dontcare.Name = "rtb_stocktype_dontcare";
            this.rtb_stocktype_dontcare.Size = new System.Drawing.Size(76, 17);
            this.rtb_stocktype_dontcare.TabIndex = 3;
            this.rtb_stocktype_dontcare.Text = "I dont care";
            this.rtb_stocktype_dontcare.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 461);
            this.Controls.Add(this.tabs);
            this.Name = "Main";
            this.Text = "TaiPan Data Access";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabs.ResumeLayout(false);
            this.tabpg_search.ResumeLayout(false);
            this.tabpg_search.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpbox_exchange.ResumeLayout(false);
            this.grpbox_exchange.PerformLayout();
            this.grpbox_type.ResumeLayout(false);
            this.grpbox_type.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabpg_search;
        private System.Windows.Forms.ListView lstvw_instruments;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtn_Stocks;
        private System.Windows.Forms.RadioButton rbtn_indices;
        private System.Windows.Forms.GroupBox grpbox_exchange;
        private System.Windows.Forms.RadioButton rbtn_xetra;
        private System.Windows.Forms.RadioButton rbtn_nasdaq;
        private System.Windows.Forms.RadioButton rbtn_NYSE;
        private System.Windows.Forms.GroupBox grpbox_type;
        private System.Windows.Forms.RadioButton rbtn_agena;
        private System.Windows.Forms.RadioButton rbtn_Isin;
        private System.Windows.Forms.RadioButton rbtn_Name;
        private System.Windows.Forms.TextBox txt_input_search;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.DateTimePicker dtp_to;
        private System.Windows.Forms.Button btn_loaddata;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.Label lbl_selected_instrument;
        private System.Windows.Forms.Label lbl_data_loaded;
        private System.Windows.Forms.RadioButton rtb_stocktype_dontcare;
        private System.Windows.Forms.RadioButton rtb_exchange_dontcare;

    }
}

