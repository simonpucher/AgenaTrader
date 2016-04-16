using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }


        private void Kurssuche(DataBase tprdata, string searchtext)
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
                string[] arr = new string[3];
                arr[0] = TPRTKursSymbol.Name;
                arr[1] = TPRTKursSymbol.Symbol;
                arr[2] = TPRTKursSymbol.SymbolNr.ToString();
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

        private void GetDate(DataBase tprdata)
        {

            //IIntradayChartPeriodeEintrag TPRTIntrChartPeriodeEintrag = (IIntradayChartPeriodeEintrag)tprdata.IntradayChart(486941, 9, DateTime.Now.Date);

            int iAnzahlIntradayChart = 0;
            DateTime ChartDatum;
            int iBidAsk = 1;

            /*
                TPRKursartBezahlt = 0 
                TPRKursartBrief = 1 
                TPRKursartGeld = 2 
             */

            IIntradayChartEintrag TPRTIntradayChartEintrag;

            ChartDatum = DateTime.Now.Date.AddDays(-1).Date;

            IIntradayChart TPRTIntradayChart = (IIntradayChart)tprdata.IntradayChart(79514, iBidAsk, ChartDatum);

            TPRTIntradayChart.KursArt = TPRKursart.TPRKursartBezahlt;
            iAnzahlIntradayChart = TPRTIntradayChart.Count;

            if (iAnzahlIntradayChart != 0)
            {
                for (int i = 1; i <= TPRTIntradayChart.Count; i++)
                {
                    TPRTIntradayChartEintrag = (IIntradayChartEintrag)TPRTIntradayChart[i];
                    Console.WriteLine(TPRTIntradayChartEintrag.Kurs.ToString());
                    //Console.WriteLine(TPRTIntradayChartEintrag.Volume.ToString());
                    Console.WriteLine(TPRTIntradayChartEintrag.Zeit.ToString());
                }
            }
            else
                MessageBox.Show("Die Anzahl des Intraday Charts ist 0");

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
                this.Kurssuche(tprdata, this.txt_input_search.Text);
            }
        }
    }
}
