﻿using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System
{
    public partial class Registration_NoID : Form
    {
        public Event_List eventlist { get; set; }

        public static bool duplicate = false;

        public Registration_NoID(Event_List _form1)
        {
            eventlist = _form1;
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }
            base.WndProc(ref m);
        }

        public static void GetSN(string sn)
        {
            int dup = 1;
            string query = "select count(SN) from " + Event_List.event_name + " where SN = '" + sn + "';";
            if (MainMenu.OpenConnection())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, MainMenu.conn);
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        if (dup == Convert.ToInt32(dataReader[0].ToString()))
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null || textBox2.Text == null || !maskedTextBox1.MaskCompleted || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Fill up all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (eventlist.checkBox1.Checked)
                {
                    if (MainMenu.isMaster == true)
                    {
                        MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess_events;sslmode=none;");
                    }
                    else
                    {
                        MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess_events;sslmode=none;");
                    }

                    GetSN(EnCryptDecrypt.CryptorEngine.Encrypt(maskedTextBox1.Text, true));

                    if (!duplicate)
                    {
                        MainMenu.Insert("insert into " + Event_List.event_name + " (FN, LN, SN,Year_Level) values ('" + EnCryptDecrypt.CryptorEngine.Encrypt(textBox1.Text, true) + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(textBox2.Text, true) + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(maskedTextBox1.Text, true) + "','" + EnCryptDecrypt.CryptorEngine.Encrypt(comboBox1.Text, true) + "');");
                        MainMenu.Insert("update " + Event_List.event_name + " set Time_In = '" + DateTime.Now.ToString("HH:mm") + "' where SN = '" + EnCryptDecrypt.CryptorEngine.Encrypt(maskedTextBox1.Text, true) + "';");
                        textBox1.Text = null;
                        textBox2.Text = null;
                        maskedTextBox1.Text = null;
                        eventlist.LA(Event_List.event_name);
                        if (MainMenu.isMaster == true)
                        {
                            MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess;sslmode=none;");
                        }
                        else
                        {
                            MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess;sslmode=none;");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Duplicate Entry Found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Text = null;
                        textBox2.Text = null;
                        maskedTextBox1.Text = null;
                        eventlist.LA(Event_List.event_name);
                        if (MainMenu.isMaster == true)
                        {
                            MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess;sslmode=none;");
                        }
                        else
                        {
                            MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess;sslmode=none;");
                        }
                    }

                }
                else
                {
                    if (MainMenu.isMaster == true)
                    {
                        MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess_events;sslmode=none;");
                    }
                    else
                    {
                        MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess_events;sslmode=none;");
                    }
                    MainMenu.Insert("update " + Event_List.event_name + " set Time_Out = '" + DateTime.Now.ToString("HH:mm") + "' where SN = '" + EnCryptDecrypt.CryptorEngine.Encrypt(maskedTextBox1.Text, true) + "';");
                    eventlist.LA(Event_List.event_name);
                    if (MainMenu.isMaster == true)
                    {
                        MainMenu.Initialize("server=localhost;uid=root;pwd=;database=coess;sslmode=none;");
                    }
                    else
                    {
                        MainMenu.Initialize("server=192.168.1.4;uid=access;pwd=;database=coess;sslmode=none;");
                    }
                }
            }
            duplicate = false;
        }

    }
}
