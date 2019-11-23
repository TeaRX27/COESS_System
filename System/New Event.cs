﻿using System.IO;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System
{
    public partial class New_Event : Form
    {

        //Image variables
        string location = @"C:\\COESS\\Images\\Pubmat\\";
        string fileName = "";
        string file1;
        public static string finalevent;
        bool duplicate = false;

        public New_Event()
        {
            InitializeComponent();
            if (MainMenu.isMaster == true)
            {
                MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess;sslmode=none;");
            }
            else
            {
                MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess;sslmode=none;");
            }
            event_location.Text = "Event Location";
            event_location.ForeColor = SystemColors.GrayText;
            event_name.Text = "Event Name";
            event_name.ForeColor = SystemColors.GrayText;
        }

        public void GetEname(string ename)
        {
            int dup = 1;
            string query = "select count(Event_Name) from event_list where Event_name = '" + EnCryptDecrypt.CryptorEngine.Encrypt(ename, true) + "';";
            if (MainMenu.OpenConnection())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, MainMenu.conn);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        if (Convert.ToInt32(dataReader[0].ToString()) >= dup)
                        {
                            duplicate = true;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    MainMenu.CloseConnection();
                }
            }
        }

        public string event_req()
        {
            //event_name,event_date,event_location,event_pubmat
            string event_complete = null;
            event_complete = EnCryptDecrypt.CryptorEngine.Encrypt(event_name.Text, true);
            event_complete = event_complete + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(Convert.ToString(event_date.Value.ToShortDateString()), true);
            event_complete = event_complete + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(Convert.ToString(event_location.Text), true);
            event_complete = event_complete + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(location + file1, true);
            return event_complete;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Insert an Image";
            openFileDialog1.InitialDirectory = location;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "JPEG Images|*.jpg|GIF Images|*.gif|BITMAPS|*.bmp|TIFF Images|*.tif|PNG Images|*.png|All Files|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                imageList1.Dispose();
                imageList1.Images.Clear();
                file1 = openFileDialog1.SafeFileName;
                fileName = openFileDialog1.FileName;
                Image pickedImage = Image.FromFile(fileName);
                imageList1.Images.Add(pickedImage);
                event_pubmat.BackgroundImage = imageList1.Images[0];
                event_pubmat.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form mainmenu = new MainMenu();
            mainmenu.Show();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finalevent = event_name.Text;
            finalevent = finalevent.Replace(' ', '_');
            GetEname(event_name.Text);
            if (!duplicate)
            {
                MainMenu.Insert("insert into event_list (event_name,event_date,event_location,event_pubmat) values ('" + event_req() + "');");
                if (MainMenu.isMaster == true)
                {
                    MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess_events;sslmode=none;");
                }
                else
                {
                    MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess_events;sslmode=none;");
                }
                MainMenu.Insert("create table " + finalevent + " (ID_No int(3) null, FN varchar(255) not null, LN varchar(255) not null, SN varchar(255) not null, Year_Level varchar(255) null, Time_In varchar(255) null, Time_Out varchar(255) null, primary key(SN));");
                File.Copy(fileName, @"C:\\COESS\\Images\\Pubmat\\" + file1);
                event_name.Text = null;
                event_location.Text = null;
                event_date.Value = DateTime.Today;
                Image dump = event_pubmat.BackgroundImage;
                if (dump != null)
                    dump.Dispose();
                if (DialogResult.No == MessageBox.Show("Event Created!\n\nWould you like to create another event?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Form mainmenu = new MainMenu();
                    mainmenu.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Event is already in the database!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                event_name.Text = null;
                event_location.Text = null;
                event_date.Value = DateTime.Today;
                Image dump = event_pubmat.BackgroundImage;
                if (dump != null)
                    dump.Dispose();
            }
        }

        private void New_Event_Load(object sender, EventArgs e)
        {
            finalevent = "";
        }

        private void event_name_Leave(object sender, EventArgs e)
        {
            if (event_name.Text.Length == 0)
            {
                event_name.Text = "Event Name";
                event_name.ForeColor = SystemColors.GrayText;
            }
        }

        private void event_name_Enter(object sender, EventArgs e)
        {
            if (event_name.Text == "Event Name")
            {
                event_name.Text = "";
                event_name.ForeColor = SystemColors.WindowText;
            }
        }

        private void event_location_Enter(object sender, EventArgs e)
        {
            if (event_location.Text == "Event Location")
            {
                event_location.Text = "";
                event_location.ForeColor = SystemColors.WindowText;
            }
        }

        private void event_location_Leave(object sender, EventArgs e)
        {
            if (event_location.Text.Length == 0)
            {
                event_location.Text = "Event Location";
                event_location.ForeColor = SystemColors.GrayText;
            }
        }

    }
}
