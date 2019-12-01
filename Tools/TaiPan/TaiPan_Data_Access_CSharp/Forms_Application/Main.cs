using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaiPanRTLib;

namespace WindowsFormsApplication1
{
    public partial class Main : Form
    {


        //Dll Import to use watermarks in textboxes
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);

        private static void SetWatermark(TextBox textbox, string text) {
        
            SendMessage(textbox.Handle, 0x1501, 1, text);
        }


        private IList<string[]> _exportdata;



        /// <summary>
        /// Main Method
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Load Event of the main form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            SetWatermark(this.txt_input_search, "Search instrument");
            this.txt_input_search.Focus();

            this.ResetInstrumentsListView();
        }

        private void ResetInstrumentsListView() {
            this.lstvw_instruments.Clear();

            this.lstvw_instruments.View = View.Details;
            this.lstvw_instruments.GridLines = true;
            this.lstvw_instruments.FullRowSelect = true;

            //Add column header
            this.lstvw_instruments.Columns.Add("Name", 200);
            this.lstvw_instruments.Columns.Add("Symbol", 70);
            this.lstvw_instruments.Columns.Add("Symbol Nr.", 70);
            this.lstvw_instruments.Columns.Add("Börse", 100);
        }


        private void SearchInstrument(DataBase tprdata, string searchtext)
        {

            // Objekt zu den Suchkriterien
            TPRSuchKriterien criteria = new TPRSuchKriterien();
            if (this.rbtn_agena.Checked)
            {
                criteria = TPRSuchKriterien.TPRSucheAgenaInstument;
            }
            else if (this.rbtn_Isin.Checked)
            {
                criteria = TPRSuchKriterien.TPRSucheISIN;
            }
            else if (this.rbtn_Name.Checked)
            {
                criteria = TPRSuchKriterien.TPRSucheWertpapiername;
            }
            else
            {
                MessageBox.Show("Please define which type of search you would like to use!", "Error");
                return;
            }

            /*
             * 9 = XETRA
             * 17 = NYSE
             * 18 = NASDAQ
             */
            ushort exchange = 0;
            if (this.rbtn_xetra.Checked)
            {
                exchange = 9;
            }
            else if (this.rbtn_NYSE.Checked)
            {
                exchange = 17;
            }
            else if (this.rbtn_nasdaq.Checked)
            {
                exchange = 18;
            }
            else if (this.rtb_exchange_dontcare.Checked)
            {
                exchange = 0;
            }
            else
            {
                MessageBox.Show("Please define which exchange you want to use!", "Error");
                return;
            }

            /*
             * 1 = Aktien
             * 2 = Optionen
             * 3 = Futures
             * 5 = Anleihen
             * 6 = Inizies 
             * 8 = Optionsscheine
             * 9 = Fonds
             * 10 = Devisen
             */
            ushort stocktype;
            if (this.rbtn_Stocks.Checked)
            {
                stocktype = 1;
            }
            else if (this.rbtn_indices.Checked)
            {
                stocktype = 6;
            }
            else if (this.rtb_stocktype_dontcare.Checked)
            {
                stocktype = 0;
            }
            else
            {
                MessageBox.Show("Please define which type of instrument you want to search!", "Error");
                return;
            }


            IKursSuchListe TPRTKursSuchListe = (IKursSuchListe)tprdata.KursSuche(criteria, searchtext, exchange, stocktype);

            //Console.WriteLine("searchstring: " + searchtext + ", Boerse: " + exchange.ToString() + " , WertPapierArt: " + stocktype.ToString());

            this.ResetInstrumentsListView();

            IKursSymbol TPRTKursSymbol;
            for (int i = 1; i <= TPRTKursSuchListe.Count; i++)
            {
                TPRTKursSymbol = (IKursSymbol)TPRTKursSuchListe[i];

                //Console.WriteLine("Name: " + TPRTKursSymbol.Name + " Exchange: " + TPRTKursSymbol.Boerse + " Symbol: " + TPRTKursSymbol.Symbol + " SymbolNr.: " + TPRTKursSymbol.SymbolNr);

                //Add first item
                string[] arr = new string[4];
                arr[0] = TPRTKursSymbol.Name;
                arr[1] = TPRTKursSymbol.Symbol;
                arr[2] = TPRTKursSymbol.SymbolNr.ToString();
                arr[3] = TPRTKursSymbol.Boerse;
                this.lstvw_instruments.Items.Add(new ListViewItem(arr));

                //lstvw_.Items.Insert(nIndex - 1, TPRTBoerse.Name);
                //lcAusgabe_Boersen.Items[nIndex - 1].SubItems.Add(TPRTBoerse.Nr.ToString());

                //lcAusgabeSuchergebnis.Items.Insert(i - 1, TPRTKursSymbol.Aktuell.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.AktuellZeit.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.BezahltVolume.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Boerse);
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Brief.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.BriefVolume.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.BriefZeit.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Geld.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.GeldVolume.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.GeldZeit.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Handel.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.High.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Low.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Name);
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Open.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.PrevClose.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Symbol);
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.SymbolNr.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Volume.ToString());
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.Waehrung);
                //lcAusgabeSuchergebnis.Items[i - 1].SubItems.Add(TPRTKursSymbol.WPArt);
            }

        }

        private void GetData(DataBase tprdata, DateTime date, ListView.SelectedListViewItemCollection selecteditem)
        {

            //IIntradayChartPeriodeEintrag TPRTIntrChartPeriodeEintrag = (IIntradayChartPeriodeEintrag)tprdata.IntradayChart(486941, 9, DateTime.Now.Date);

            int iAnzahlIntradayChart = 0;
            int iBidAsk = 1;

            /*
                TPRKursartBezahlt = 0 
                TPRKursartBrief = 1 
                TPRKursartGeld = 2 
             */

            IIntradayChartEintrag TPRTIntradayChartEintrag;

            IIntradayChart TPRTIntradayChart = (IIntradayChart)tprdata.IntradayChart(Int32.Parse(selecteditem[0].SubItems[2].Text), iBidAsk, date);

            TPRTIntradayChart.KursArt = TPRKursart.TPRKursartBezahlt;
            iAnzahlIntradayChart = TPRTIntradayChart.Count;

            if (iAnzahlIntradayChart != 0)
            {
                _exportdata = new List<string[]>();
                for (int i = 1; i <= TPRTIntradayChart.Count; i++)
                {
                    TPRTIntradayChartEintrag = (IIntradayChartEintrag)TPRTIntradayChart[i];
                    string[] tempdata = new string[3];
                    tempdata[0] = date.ToString("yyyyMMdd") + " " + TPRTIntradayChartEintrag.Zeit.ToString("hhmmss");
                    tempdata[1] = TPRTIntradayChartEintrag.Kurs.ToString();
                    tempdata[2] = TPRTIntradayChartEintrag.Volume.ToString();

                    _exportdata.Add(tempdata);
                    //Console.WriteLine("Time: " + TPRTIntradayChartEintrag.Zeit.ToString() + " Value: " + TPRTIntradayChartEintrag.Kurs.ToString());
                    //Console.WriteLine(TPRTIntradayChartEintrag.Volume.ToString());

                }
                this.lbl_data_loaded.Text = TPRTIntradayChart.Count + " rows of data loaded.";
            }
            else
            {
                this.lbl_data_loaded.Text = "There is no data available.";
                return;
            }

            //IStamminformationen StammInfo = new StamminformationenClass();
            //StammInfo.SymbolNr = 78298;

        }

   

        /// <summary>
        /// Start the search on instruments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_Click(object sender, EventArgs e)
        {
        this.StartSearch();
        }

        private void txt_input_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.StartSearch();
            }
        }

        private void StartSearch() {
            TaiPanRealtime tpr = new TaiPanRealtime();
            DataBase tprdata = (DataBase)tpr.DataBase;

            if (!String.IsNullOrWhiteSpace(this.txt_input_search.Text))
            {
                this.SearchInstrument(tprdata, this.txt_input_search.Text);
            }
        }

 

        private void btn_export_Click(object sender, EventArgs e)
        {

            if (_exportdata != null && _exportdata.Count > 0)
            {
                var csv = new StringBuilder();
                foreach (string[] item in _exportdata)
                {
                    csv.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6}", item[0], item[1], item[2], item[3], item[4], item[5], item[6]));
                }
                string filename = this.lstvw_instruments.SelectedItems[0].Text + "_" + this.lstvw_instruments.SelectedItems[0].SubItems[2].Text + "_" + this.dtp_to.Value.ToString("yyyyMMdd") + "_" + this.dtp_to.Value.ToString("yyyyMMdd") + ".csv";
                var invalids = System.IO.Path.GetInvalidFileNameChars();
                filename = String.Join("_", filename.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\" + filename, csv.ToString());
                this.lbl_data_loaded.Text = _exportdata.Count + " rows of data exported.";
            }
            else
            {
                MessageBox.Show("Please load data first!", "Warning");
                return;
            }
        }

        private void btn_loaddata_Click(object sender, EventArgs e)
        {
            if (this.lstvw_instruments.SelectedItems.Count > 0)
            {
                 TaiPanRealtime tpr = new TaiPanRealtime();
                DataBase tprdata = (DataBase)tpr.DataBase;
                //this.GetData(tprdata, this.dtp_to.Value, this.lstvw_instruments.SelectedItems);
                this.GetDailyEODDate(tprdata, this.dtp_to.Value, this.lstvw_instruments.SelectedItems);
            }
            else
            {
                MessageBox.Show("Please select an instrument!", "Warning");
                return;
            }
        }

        private void lstvw_instruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstvw_instruments.SelectedItems.Count > 0)
            {
                this.lbl_selected_instrument.Text = this.lstvw_instruments.SelectedItems[0].Text + " - " + this.lstvw_instruments.SelectedItems[0].SubItems[2].Text;   
            }
        }


        private void GetDailyEODDate(DataBase tprdata, DateTime datumVon, ListView.SelectedListViewItemCollection selecteditem)
        {

            ArrayLoader loader = new ArrayLoader();

            //securities are identified with a unique symbol number
            int bubu = Int32.Parse(selecteditem[0].SubItems[2].Text);
            int[] symbolNr = new int[1]
            {
                bubu
 //   78303,          //555750.ETR        Dt.Telekom
	//169286,         //710000.ETR        Daimler AG
	//78275,          //519000.ETR        BWM ST
	//78340,          //766403.ETR        VW ST
	//78267           //823212.ETR        Lufthansa
            };
            //DateTime datumVon = new DateTime(2013, 8, 1);    //start date is 08/01/2013
            DateTime datumBis = DateTime.Now;               //end date is today

            JahreschartCollection jahresCol = loader.Jahrescharts(symbolNr, datumVon, datumBis) as JahreschartCollection;
            if (jahresCol != null)
            {
                //the JahresChartCollection contains all end-of-day charts...
                Jahreschart c = new Jahreschart();
                IChartTimeRange it = (IChartTimeRange)c;
                it.TimeRange(datumVon, datumBis);

                foreach (Jahreschart chart in jahresCol)
                {
                    ////...that can be read now...
                    //Debug.WriteLine(Environment.NewLine);
                    //foreach (IJahreschartEintrag entry in chart)
                    //{
                    //    Debug.WriteLine("{0}  {1}  {2}  {3}  {4}", entry.Zeit.ToShortDateString(), entry.Open, entry.High, entry.Low, entry.Close);
                    //}
                    _exportdata = new List<string[]>();
                    foreach (IJahreschartEintrag entry in chart)
                    {
                        //TPRTIntradayChartEintrag = (IIntradayChartEintrag)TPRTIntradayChart[i];
                        string[] tempdata = new string[7];
                        tempdata[0] = entry.Zeit.ToString("dd.MM.yyyy");// + " " + entry.Zeit.ToString("hhmmss");
                        tempdata[1] = entry.Open.ToString();
                        tempdata[2] = entry.High.ToString();
                        tempdata[3] = entry.Low.ToString();
                        tempdata[4] = entry.Close.ToString();
                        tempdata[5] = entry.Volume.ToString();
                        tempdata[6] = entry.OpenInterest.ToString();

                        _exportdata.Add(tempdata);
                        //Console.WriteLine("Time: " + TPRTIntradayChartEintrag.Zeit.ToString() + " Value: " + TPRTIntradayChartEintrag.Kurs.ToString());
                        //Console.WriteLine(TPRTIntradayChartEintrag.Volume.ToString());

                    }
                    this.lbl_data_loaded.Text = _exportdata.Count + " rows of data loaded.";
                }
            }
        }

     



    
    }
}
