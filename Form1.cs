using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicDefiner
{
    public partial class f_Main : Form
    {
        public f_Main()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(118,13);
        }

        #region
        //Değişkenler ve Get-Set'ler
        public bool isThereClicked = false;    //Gerekli değişkenler, ilgili method'ların altında kullanılıyor.
        public bool isMenuValueValid = false;
        public string currentlyClicked = "";

        RelicEntities db = new RelicEntities(); //Form Load'ta tanımlarsak, Reset Button'ında bunu tekrar kullanamayız, o yüzden en tepede yarattık.

        public List<v_RelicDetail> itemsFirstRelic { get; set; } //Relic view tablosunu database'ten aldık.
        public List<v_RelicDetail> itemsSecondRelic { get; set; }
        public List<v_RelicDetail> itemsThirdRelic { get; set; }
        public List<v_RelicDetail> itemsForthRelic { get; set; }

        public List<v_RelicDetail> itemsFirstRelicForUnscanned { get; set; } //Relic view tablosunu database'ten aldık.
        public List<v_RelicDetail> itemsSecondRelicForUnscanned { get; set; }
        public List<v_RelicDetail> itemsThirdRelicForUnscanned { get; set; }
        public List<v_RelicDetail> itemsForthRelicForUnscanned { get; set; }
        #endregion

        private void f_Main_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn col in dtgw_Results.Columns) //Tüm kolonların başlıklarının align'ı middle center yapar.
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dtgw_Results.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 13.75F, FontStyle.Bold);   //Kolon başlıklarına yazım stili verdik.
            dtgw_Results.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSeaGreen;
            dtgw_Results.ColumnHeadersDefaultCellStyle.ForeColor = Color.MediumBlue;
            dtgw_Results.AutoGenerateColumns = false; //Yarattığım kolonların yanına ekstradan veritabanındaki aynı kolonları dolu şekilde getiriyor eğer True yaparsam. Ve benim kolonlarımı ise boş bırakıyor.
            dtgw_Results.RowTemplate.Height = 30;

            dtgw_Results.Columns[0].DefaultCellStyle.BackColor = Color.PowderBlue;     //Kolonları renklendirdik.
            dtgw_Results.Columns[3].DefaultCellStyle.BackColor = Color.PowderBlue;

            itemsFirstRelic = db.v_RelicDetail.ToList();
            itemsSecondRelic = db.v_RelicDetail.ToList();
            itemsThirdRelic = db.v_RelicDetail.ToList();
            itemsForthRelic = db.v_RelicDetail.ToList();

            itemsFirstRelicForUnscanned = db.v_RelicDetail.ToList();
            itemsSecondRelicForUnscanned = db.v_RelicDetail.ToList();
            itemsThirdRelicForUnscanned = db.v_RelicDetail.ToList();
            itemsForthRelicForUnscanned = db.v_RelicDetail.ToList();

            dtgw_Results.DataSource = itemsFirstRelic;

            lbl_allScannedInfo.Visible = false;
            lbl_needRobotInfo.Visible = false;
            btn_numberOfUnscannedItems.Visible = false;
            cellStyle();
        }

        public void clearMenu()
        {
            cb_wordChoice_1.SelectedItem = null;
            cb_wordChoice_2.SelectedItem = null;
            cb_wordChoice_3.SelectedItem = null;
            cb_wordChoice_4.SelectedItem = null;
            cb_numberChoice_1.SelectedItem = null;
            cb_numberChoice_2.SelectedItem = null;
            cb_numberChoice_3.SelectedItem = null;
            cb_numberChoice_4.SelectedItem = null;
        }

        public void numberOfPlayerForUnscanned()
        {
            if (cb_numberChoice_4.Visible == true)
            {
                getUnscannedData(4);
            }
            else
                if (cb_numberChoice_3.Visible == true)
            {
                getUnscannedData(3);
            }
            else
                if (cb_numberChoice_2.Visible == true)
            {
                getUnscannedData(2);
            }
            else
            {
                getUnscannedData(1);
            }
        }

        public void numberOfPlayerForScanned()
        {
            if (cb_numberChoice_4.Visible == true)
            {            
                getScannedData(4);
            }
            else
                if (cb_numberChoice_3.Visible == true)
            {
                getScannedData(3);
            }
            else
                if (cb_numberChoice_2.Visible == true)
            {
                getScannedData(2);
            }
            else
            {
                getScannedData(1);
            }
        }

        public void writeData(List<v_RelicDetail> itemListScanned, bool isItScannedData)
        {

            itemListScanned = itemListScanned.OrderBy(e => e.ItemName).ToList();
            dtgw_Results.DataSource = itemListScanned.ToList();

            if (dtgw_Results.Rows.Count == 0) //İçeri girerse hiç geçmiş kayıt yok. Ya da relic yanlış girildi.
            {
                    numberOfPlayerForScanned();
            }
            else
            {
                //Row 1, Cell 0'dan başlanacak. Row 0'da başlık var. Cell 0'da itemName
                int insideLoopLimit = dtgw_Results.Rows.Count;
                int outsideLoopLimit = dtgw_Results.Rows.Count;
                int outsideCounter = 0;
                for (int k = 0; k < outsideLoopLimit; k++)
                {
                    if (outsideCounter + k == outsideLoopLimit)
                    {
                        break;
                    }
                    int insideCounter = 0;
                    int nextItem = 1 + k;
                    var removeDuplicates = dtgw_Results.Rows[k].Cells[0].Value.ToString();

                    for (int i = nextItem; i < insideLoopLimit - insideCounter; i++) // datagrid satır sayısı 23'te kalıyor, döngü yukarıdaki for'a gidip tekrar buraya düşse de.
                    {
                        if (dtgw_Results.Rows[i].Cells[0].Value.ToString() == removeDuplicates)
                        {
                            itemListScanned.RemoveAt(i);
                            --i;
                            ++outsideCounter;
                            ++insideCounter;
                            dtgw_Results.DataSource = null;
                            dtgw_Results.DataSource = itemListScanned;
                        }
                    }
                    insideLoopLimit = insideLoopLimit - insideCounter;
                }

                dtgw_Results.DataSource = null;
                dtgw_Results.Rows.Clear();
                dtgw_Results.DataSource = itemListScanned;
                dtgw_Results.Rows[0].Cells[0].Selected = false; //Sonuçlar ekrana geldikten sonra ilk satırın ilk sütunundaki hücre otomatik seçili oluyordu. Bunu disselect yaptık.

                for (int i = 0; i < dtgw_Results.RowCount; i++)     //Altın Rarity'si olanların satırını komple renklendirdik sarı-kırmızı ile.
                {
                    if (dtgw_Results.Rows[i].Cells[1].Value.ToString() == "Gold")
                    {
                        dtgw_Results.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                        dtgw_Results.Rows[i].DefaultCellStyle.BackColor = Color.Gold;
                    }
                }

                if (!isItScannedData)
                {
                    lbl_needRobotInfo.Visible = true;
                    btn_numberOfUnscannedItems.Visible = true;
                    btn_numberOfUnscannedItems.Text = dtgw_Results.RowCount.ToString();
                    btn_Save.Visible = true;
                }
                else
                {
                    lbl_allScannedInfo.Visible = true;
                }
            }
        }

        public void getUnscannedData(int numberOfRequiredRelics)
        {
            if (numberOfRequiredRelics > 3)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();
                string thirdChoice = currentlyClicked + " " + cb_wordChoice_3.SelectedItem.ToString() + cb_numberChoice_3.SelectedItem.ToString();
                string forthChoice = currentlyClicked + " " + cb_wordChoice_4.SelectedItem.ToString() + cb_numberChoice_4.SelectedItem.ToString();

                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.RelicName.Equals(firstChoice)).ToList(); /*StartsWith(firstChoice)).ToList();*/
                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.RelicName.Equals(secondChoice)).ToList();
                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsThirdRelicForUnscanned = itemsThirdRelicForUnscanned.Where(x => x.RelicName.Equals(thirdChoice)).ToList();
                itemsThirdRelicForUnscanned = itemsThirdRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsForthRelicForUnscanned = itemsForthRelicForUnscanned.Where(x => x.RelicName.Equals(forthChoice)).ToList();
                itemsForthRelicForUnscanned = itemsForthRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();


                itemsFirstRelicForUnscanned.AddRange(itemsSecondRelicForUnscanned);
                itemsFirstRelicForUnscanned.AddRange(itemsThirdRelicForUnscanned);
                itemsFirstRelicForUnscanned.AddRange(itemsForthRelicForUnscanned);

                writeData(itemsFirstRelicForUnscanned, false);
            }
            else
            if (numberOfRequiredRelics > 2)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();
                string thirdChoice = currentlyClicked + " " + cb_wordChoice_3.SelectedItem.ToString() + cb_numberChoice_3.SelectedItem.ToString();

                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.RelicName.Equals(secondChoice)).ToList();
                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsThirdRelicForUnscanned = itemsThirdRelicForUnscanned.Where(x => x.RelicName.Equals(thirdChoice)).ToList();
                itemsThirdRelicForUnscanned = itemsThirdRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsFirstRelicForUnscanned.AddRange(itemsSecondRelicForUnscanned);
                itemsFirstRelicForUnscanned.AddRange(itemsThirdRelicForUnscanned);

                writeData(itemsFirstRelicForUnscanned, false);
            }
            else
            if (numberOfRequiredRelics > 1)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();

                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.RelicName.Equals(secondChoice)).ToList();
                itemsSecondRelicForUnscanned = itemsSecondRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                itemsFirstRelicForUnscanned.AddRange(itemsSecondRelicForUnscanned);

                writeData(itemsFirstRelicForUnscanned, false);
            }
            else
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();

                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsFirstRelicForUnscanned = itemsFirstRelicForUnscanned.Where(x => x.LastUpdateDate < DateTime.Now.AddDays(-1)).ToList();

                writeData(itemsFirstRelicForUnscanned, false);
            }
        }

        public void getScannedData(int numberOfRequiredRelics)
        {
            if (numberOfRequiredRelics > 3)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();
                string thirdChoice = currentlyClicked + " " + cb_wordChoice_3.SelectedItem.ToString() + cb_numberChoice_3.SelectedItem.ToString();
                string forthChoice = currentlyClicked + " " + cb_wordChoice_4.SelectedItem.ToString() + cb_numberChoice_4.SelectedItem.ToString();

                itemsFirstRelic = itemsFirstRelic.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsSecondRelic = itemsSecondRelic.Where(x => x.RelicName.Equals(secondChoice)).ToList();
                itemsThirdRelic = itemsThirdRelic.Where(x => x.RelicName.Equals(thirdChoice)).ToList();
                itemsForthRelic = itemsForthRelic.Where(x => x.RelicName.Equals(forthChoice)).ToList();


                //Olmayan bir Relic seçilmiş ve Listele'ye tıklanmış ise, false döndürecek  
                if (checkRelicAvailability(itemsFirstRelic, itemsSecondRelic, itemsThirdRelic, itemsForthRelic))
                {
                    itemsFirstRelic.AddRange(itemsSecondRelic);
                    itemsFirstRelic.AddRange(itemsThirdRelic);
                    itemsFirstRelic.AddRange(itemsForthRelic);
                    writeData(itemsFirstRelic, true);
                }
                else
                {
                    MessageBox.Show("Unavailable Relic", "Wrong Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            if (numberOfRequiredRelics > 2)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();
                string thirdChoice = currentlyClicked + " " + cb_wordChoice_3.SelectedItem.ToString() + cb_numberChoice_3.SelectedItem.ToString();

                itemsFirstRelic = itemsFirstRelic.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsSecondRelic = itemsSecondRelic.Where(x => x.RelicName.Equals(secondChoice)).ToList();
                itemsThirdRelic = itemsThirdRelic.Where(x => x.RelicName.Equals(thirdChoice)).ToList();

                if (checkRelicAvailability(itemsFirstRelic, itemsSecondRelic, itemsThirdRelic))
                {
                    itemsFirstRelic.AddRange(itemsSecondRelic);
                    itemsFirstRelic.AddRange(itemsThirdRelic);

                    writeData(itemsFirstRelic, true);
                }
                else
                {
                    MessageBox.Show("Unavailable Relic", "Wrong Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            if (numberOfRequiredRelics > 1)
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                string secondChoice = currentlyClicked + " " + cb_wordChoice_2.SelectedItem.ToString() + cb_numberChoice_2.SelectedItem.ToString();

                itemsFirstRelic = itemsFirstRelic.Where(x => x.RelicName.Equals(firstChoice)).ToList();
                itemsSecondRelic = itemsSecondRelic.Where(x => x.RelicName.Equals(secondChoice)).ToList();

                if (checkRelicAvailability(itemsFirstRelic, itemsSecondRelic))
                {
                    itemsFirstRelic.AddRange(itemsSecondRelic);

                    writeData(itemsFirstRelic, true);
                }
                else
                {
                    MessageBox.Show("Unavailable Relic", "Wrong Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string firstChoice = currentlyClicked + " " + cb_wordChoice_1.SelectedItem.ToString() + cb_numberChoice_1.SelectedItem.ToString();
                itemsFirstRelic = itemsFirstRelic.Where(x => x.RelicName.Equals(firstChoice)).ToList();

                if (checkRelicAvailability(itemsFirstRelic))
                {
                    writeData(itemsFirstRelic, true);
                }
                else
                {
                    MessageBox.Show("Unavailable Relic", "Wrong Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void cellStyle()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();  //Hücrelerin yazım stili
            style.Font = new Font(dtgw_Results.Font.FontFamily, 14, FontStyle.Bold);

            foreach (DataGridViewRow row in dtgw_Results.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style = style;
                }
            }
        }

        public bool checkInputValues()
        {
                if (cb_numberChoice_1.Visible == true)
                {
                    if (cb_numberChoice_1.SelectedItem == null || cb_wordChoice_1.SelectedItem == null)
                    {
                        isMenuValueValid = false;
                        return isMenuValueValid;
                    }
                }

                if (cb_numberChoice_2.Visible == true)
                {
                    if (cb_numberChoice_2.SelectedItem == null || cb_wordChoice_2.SelectedItem == null)
                    {
                        isMenuValueValid = false;
                        return isMenuValueValid;
                    }
                }

                if (cb_numberChoice_3.Visible == true)
                {
                    if (cb_numberChoice_3.SelectedItem == null || cb_wordChoice_3.SelectedItem == null)
                    {
                        isMenuValueValid = false;
                        return isMenuValueValid;
                    }
                }

                if (cb_numberChoice_4.Visible == true)
                {
                    if (cb_numberChoice_4.SelectedItem == null || cb_wordChoice_4.SelectedItem == null)
                    {
                        isMenuValueValid = false;
                        return isMenuValueValid;
                    }
                }
            isMenuValueValid = true;
            return isMenuValueValid;
        }

        public bool checkRelicAvailability(List<v_RelicDetail> itemList1, List<v_RelicDetail> itemList2, List<v_RelicDetail> itemList3, List<v_RelicDetail> itemList4)
        {
            if (itemList1.Count == 0 || itemList2.Count == 0 || itemList3.Count == 0 || itemList4.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public bool checkRelicAvailability(List<v_RelicDetail> itemList1, List<v_RelicDetail> itemList2, List<v_RelicDetail> itemList3)
        {
            if (itemList1.Count == 0 || itemList2.Count == 0 || itemList3.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public bool checkRelicAvailability(List<v_RelicDetail> itemList1, List<v_RelicDetail> itemList2)
        {
            if (itemList1.Count == 0 || itemList2.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public bool checkRelicAvailability(List<v_RelicDetail> itemList1)
        {
            if (itemList1.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public void visibleMenu(string relicName)
        {
            btn_Add.Visible = true;
            btn_Remove.Visible = true;
            btn_Color_1.Visible = true;
            btn_Color_2.Visible = true;
            btn_Color_3.Visible = true;
            btn_Color_4.Visible = true;
            cb_wordChoice_1.Visible = true;
            cb_wordChoice_2.Visible = true;
            cb_wordChoice_3.Visible = true;
            cb_wordChoice_4.Visible = true;
            cb_numberChoice_1.Visible = true;
            cb_numberChoice_2.Visible = true;
            cb_numberChoice_3.Visible = true;
            cb_numberChoice_4.Visible = true;

            if (relicName == "Lith")
            {
                btn_Color_1.BackColor = Color.Coral;
                btn_Color_2.BackColor = Color.Coral;
                btn_Color_3.BackColor = Color.Coral;
                btn_Color_4.BackColor = Color.Coral;
            }
            if (relicName == "Meso")
            {
                btn_Color_1.BackColor = Color.LightSeaGreen;
                btn_Color_2.BackColor = Color.LightSeaGreen;
                btn_Color_3.BackColor = Color.LightSeaGreen;
                btn_Color_4.BackColor = Color.LightSeaGreen;
            }
            if (relicName == "Neo")
            {
                btn_Color_1.BackColor = Color.DimGray;
                btn_Color_2.BackColor = Color.DimGray;
                btn_Color_3.BackColor = Color.DimGray;
                btn_Color_4.BackColor = Color.DimGray;
            }
            if (relicName == "Axi")
            {
                btn_Color_1.BackColor = Color.Gold;
                btn_Color_2.BackColor = Color.Gold;
                btn_Color_3.BackColor = Color.Gold;
                btn_Color_4.BackColor = Color.Gold;
            }
        }

        public void invisibleMenu()
        {
            btn_Add.Visible = false;
            btn_Remove.Visible = false;
            btn_Color_1.Visible = false;
            btn_Color_2.Visible = false;
            btn_Color_3.Visible = false;
            btn_Color_4.Visible = false;
            cb_wordChoice_1.Visible = false;
            cb_wordChoice_2.Visible = false;
            cb_wordChoice_3.Visible = false;
            cb_wordChoice_4.Visible = false;
            cb_numberChoice_1.Visible = false;
            cb_numberChoice_2.Visible = false;
            cb_numberChoice_3.Visible = false;
            cb_numberChoice_4.Visible = false;

            btn_Color_1.BackColor = Color.White;
            btn_Color_2.BackColor = Color.White;
            btn_Color_3.BackColor = Color.White;
            btn_Color_4.BackColor = Color.White;
        }

        public void disabledMenu()
        {
            btn_listToBeExamined.Enabled = false;
            btn_Add.Enabled = false;
            btn_Remove.Enabled = false;
            cb_numberChoice_1.Enabled = false;
            cb_wordChoice_1.Enabled = false;
            cb_numberChoice_2.Enabled = false;
            cb_wordChoice_2.Enabled = false;
            cb_numberChoice_3.Enabled = false;
            cb_wordChoice_3.Enabled = false;
            cb_numberChoice_4.Enabled = false;
            cb_wordChoice_4.Enabled = false;
            btn_Lith.Enabled = false;
            btn_Meso.Enabled = false;
            btn_Neo.Enabled = false;
            btn_Axi.Enabled = false;
        }

        public void enabledMenu()
        {
            btn_Add.Enabled = true;
            btn_Remove.Enabled = true;
            cb_numberChoice_1.Enabled = true;
            cb_wordChoice_1.Enabled = true;
            cb_numberChoice_2.Enabled = true;
            cb_wordChoice_2.Enabled = true;
            cb_numberChoice_3.Enabled = true;
            cb_wordChoice_3.Enabled = true;
            cb_numberChoice_4.Enabled = true;
            cb_wordChoice_4.Enabled = true;
            btn_Lith.Enabled = true;
            btn_Meso.Enabled = true;
            btn_Neo.Enabled = true;
            btn_Axi.Enabled = true;
        }

        private void cb_wordChoice_1_VisibleChanged(object sender, EventArgs e)
        {
            if (cb_wordChoice_1.Visible == true) //cb_wordChoice_1 her visible'ı değiştiğinde, sağ üstteki buton'un da enable oluşu değişecek.
            {
                btn_listToBeExamined.Enabled = true;
            }
            else
            {
                btn_listToBeExamined.Enabled = false;
            }
        }

        public void mouseClickRelic(Button clickedButton)
        {
            if (!isThereClicked)
            {
                clearMenu();
                string relicName = clickedButton.Tag.ToString(); //Tıklanan buton'un daha önce atanan Tag'ini bulduk.
                if (relicName == "Lith")
                {
                    clickedButton.BackgroundImage = Properties.Resources.Lith_Clicked;
                    isThereClicked = true;
                    currentlyClicked = "Lith";
                    btn_Meso.BackgroundImage = Properties.Resources.Meso_Passive;
                    btn_Neo.BackgroundImage = Properties.Resources.Neo_Passive;
                    btn_Axi.BackgroundImage = Properties.Resources.Axi_Passive;

                    visibleMenu("Lith");
                }
                else
                if (relicName == "Meso")
                {
                    clickedButton.BackgroundImage = Properties.Resources.Meso_Clicked;
                    isThereClicked = true;
                    currentlyClicked = "Meso";
                    btn_Lith.BackgroundImage = Properties.Resources.Lith_Passive;
                    btn_Neo.BackgroundImage = Properties.Resources.Neo_Passive;
                    btn_Axi.BackgroundImage = Properties.Resources.Axi_Passive;

                    visibleMenu("Meso");
                }
                else
                if (relicName == "Neo")
                {
                    clickedButton.BackgroundImage = Properties.Resources.Neo_Clicked;
                    isThereClicked = true;
                    currentlyClicked = "Neo";
                    btn_Lith.BackgroundImage = Properties.Resources.Lith_Passive;
                    btn_Meso.BackgroundImage = Properties.Resources.Meso_Passive;
                    btn_Axi.BackgroundImage = Properties.Resources.Axi_Passive;

                    visibleMenu("Neo");
                }
                else
                if (relicName == "Axi")
                {
                    clickedButton.BackgroundImage = Properties.Resources.Axi_Clicked;
                    isThereClicked = true;
                    currentlyClicked = "Axi";
                    btn_Lith.BackgroundImage = Properties.Resources.Lith_Passive;
                    btn_Meso.BackgroundImage = Properties.Resources.Meso_Passive;
                    btn_Neo.BackgroundImage = Properties.Resources.Neo_Passive;

                    visibleMenu("Axi");
                }
            }

            else

            if (isThereClicked)
            {
                if (clickedButton.Tag.ToString() == currentlyClicked)
                {
                    isThereClicked = false;
                    currentlyClicked = "";
                    btn_Lith.BackgroundImage = Properties.Resources.Lith;
                    btn_Meso.BackgroundImage = Properties.Resources.Meso;
                    btn_Neo.BackgroundImage = Properties.Resources.Neo;
                    btn_Axi.BackgroundImage = Properties.Resources.Axi;

                    invisibleMenu();
                }
                else
                {
                    clearMenu();
                    currentlyClicked = clickedButton.Tag.ToString();
                    btn_Lith.BackgroundImage = Properties.Resources.Lith_Passive; //Hepsini önce pasive picture yapıyoruz ki bir önceki tıklanmış olanın hangisi olduğunu bulmaya çalışıp pasive yapmakla uğraşmayalım.
                    btn_Meso.BackgroundImage = Properties.Resources.Meso_Passive;
                    btn_Neo.BackgroundImage = Properties.Resources.Neo_Passive;
                    btn_Axi.BackgroundImage = Properties.Resources.Axi_Passive;

                    if (currentlyClicked == "Lith") //Sonra currentlyClicked ne ise, ona göre Clicked'e dönüştürüyoruz.                    
                    {
                        btn_Lith.BackgroundImage = Properties.Resources.Lith_Clicked;
                        visibleMenu("Lith");
                    }

                    else if (currentlyClicked == "Meso")
                    {
                        btn_Meso.BackgroundImage = Properties.Resources.Meso_Clicked;
                        visibleMenu("Meso");
                    }

                    else if (currentlyClicked == "Neo")
                    {
                        btn_Neo.BackgroundImage = Properties.Resources.Neo_Clicked;
                        visibleMenu("Neo");
                    }

                    else
                    {
                        btn_Axi.BackgroundImage = Properties.Resources.Axi_Clicked;
                        visibleMenu("Axi");
                    }
                }
            }
        }

        //Relic seçiminde mouse ile yapılan tıklama eylemlerinin sonucu değişen işlemler ele alınıyor.
        private void click_relicChoice(object sender, MouseEventArgs e) 
        {
            Button clickedButton = (Button)sender;
            mouseClickRelic(clickedButton);            
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (btn_Color_2.Visible == false)
            {
                btn_Color_2.Visible = true;
                cb_numberChoice_2.Visible = true;
                cb_wordChoice_2.Visible = true;
            }
            else
                if (btn_Color_3.Visible == false)
            {
                btn_Color_3.Visible = true;
                cb_numberChoice_3.Visible = true;
                cb_wordChoice_3.Visible = true;
            }
            else
            {
                btn_Color_4.Visible = true;
                cb_numberChoice_4.Visible = true;
                cb_wordChoice_4.Visible = true;
            }

        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            if (btn_Color_4.Visible == true)
            {
                btn_Color_4.Visible = false;
                cb_numberChoice_4.Visible = false;
                cb_wordChoice_4.Visible = false;
                cb_numberChoice_4.SelectedItem = null;
                cb_wordChoice_4.SelectedItem = null;
            }
            else
                if (btn_Color_3.Visible == true)
            {
                btn_Color_3.Visible = false;
                cb_numberChoice_3.Visible = false;
                cb_wordChoice_3.Visible = false;
                cb_numberChoice_3.SelectedItem = null;
                cb_wordChoice_3.SelectedItem = null;
            }
            else
                if (btn_Color_2.Visible == true)
            {
                btn_Color_2.Visible = false;
                cb_numberChoice_2.Visible = false;
                cb_wordChoice_2.Visible = false;
                cb_numberChoice_2.SelectedItem = null;
                cb_wordChoice_2.SelectedItem = null;
            }
        }

        private void btn_listToBeExamined_Click(object sender, EventArgs e)
        {
            if (!checkInputValues())
            {
                MessageBox.Show("Check The Input", "Missing Value",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                disabledMenu();
                numberOfPlayerForUnscanned();
                cellStyle();
                btn_listToBeExamined.Visible = false;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            btn_Save.Visible = false;
            lbl_needRobotInfo.Visible = false;
            btn_numberOfUnscannedItems.Visible = false;
            var db = new RelicEntities();
            for (int i = 0; i < dtgw_Results.RowCount; i++)
            {
                var value = dtgw_Results.Rows[i].Cells[0].Value.ToString();
                var item = db.Items.Where(s => s.ItemName == value).First();

                item.LastUpdateDate = DateTime.Now;
                item.PlatValue = Convert.ToInt32(dtgw_Results.Rows[i].Cells[3].Value);

                db.SaveChanges();
            }
            numberOfPlayerForScanned();          
            cellStyle();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            btn_listToBeExamined.Visible = true;
            btn_Save.Visible = false;
            switch (currentlyClicked)
            {
                case "Lith":
                    mouseClickRelic(btn_Lith);
                    break;
                case "Meso":
                    mouseClickRelic(btn_Meso);
                    break;
                case "Neo":
                    mouseClickRelic(btn_Neo);
                    break;
                case "Axi":
                    mouseClickRelic(btn_Axi);
                    break;
                default:
                    break;
            }

            isThereClicked = false;    //Gerekli değişkenler sıfırlandı.
            isMenuValueValid = false;
            currentlyClicked = "";
            lbl_allScannedInfo.Visible = false;
            lbl_needRobotInfo.Visible = false;
            btn_numberOfUnscannedItems.Visible = false;
            enabledMenu();

            RelicEntities database = new RelicEntities();

            itemsFirstRelic = database.v_RelicDetail.ToList();
            itemsSecondRelic = database.v_RelicDetail.ToList();
            itemsThirdRelic = database.v_RelicDetail.ToList();
            itemsForthRelic = database.v_RelicDetail.ToList();

            itemsFirstRelicForUnscanned = database.v_RelicDetail.ToList();
            itemsSecondRelicForUnscanned = database.v_RelicDetail.ToList();
            itemsThirdRelicForUnscanned = database.v_RelicDetail.ToList();
            itemsForthRelicForUnscanned = database.v_RelicDetail.ToList();

            dtgw_Results.DataSource = itemsFirstRelic;
            cellStyle();
        }

    }
}
