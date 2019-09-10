using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;//regex
using System.IO;

namespace ContactApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
		  DataTable dt = new DataTable();
        int i;
        string filter;
        private void Form1_Load(object sender, EventArgs e)
        {


            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Lastname", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("Birthday", typeof(string));
            string path = Environment.CurrentDirectory;
            string[] text = File.ReadAllLines(path + @"\AddressBook.txt"); //Άνοιγμα αρχείου από bin/debug
            foreach (string word in text)
            {
                var cols = word.Split('|');

                DataRow dr = dt.NewRow();
                for (int a = 0; a < 7; a++)
                {
                    dr[a] = cols[a];
                }

                dt.Rows.Add(dr);
            }
            dataGridView1.DataSource = dt;

            //Γενέθλια
            string[,] pinakas = new string[text.Length, dt.Columns.Count];
            int i = 0;
            foreach (string row in text)
            {

                var cols = row.Split('|');
                for (int j = 0; j < 7; j++)
                {
                    pinakas[i, j] = cols[j];


                }
                string date = pinakas[i, 6];

                var d = date.Split('/');
                int day = int.Parse(d[1]);
                int month = int.Parse(d[0]);

                if (day == DateTime.Today.Day && month == DateTime.Today.Month)
                {
                    MessageBox.Show(pinakas[i, 0] + " " + pinakas[i, 1] + " " + "has birthday today.Wish him/her happy birthday at: " + pinakas[i, 2]);
                    
                }
                i += 1;
            }
        }
       
        //Add Button Έλεγχος Εγκυρότητας Επαφής
        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (textBox1.TextLength > 0 && textBox2.TextLength > 0 && textBox3.TextLength > 0 && textBox4.TextLength > 0)
                {
                    CheckEmail mail = new CheckEmail();
                    CheckPhone phone = new CheckPhone();
                    if (phone.Phone(textBox3.Text.Length))
                    {
                        if (mail.EmailRegex(textBox4.Text))
                        {
                            dt.Rows.Add(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, comboBox1.Text, dateTimePicker1.Text);
                            dataGridView1.DataSource = dt;
                        }
                        else
                        {
                            MessageBox.Show("The mail format is invalid.");
                        }
                    }
                    else
                    {

                        MessageBox.Show("The phone format is invalid.");
                    }
                }
                else MessageBox.Show("You should fill the necessary textboxes.");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
        }

       
        //Διαγραφή Επαφής

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                dt.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
                dataGridView1.DataSource = dt;
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        //Με πάτημα γραμμής εισαγωγή των δεδομένων της στα πάνω πεδία
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            textBox1.Text = row.Cells[0].Value.ToString();
            textBox2.Text = row.Cells[1].Value.ToString();
            textBox3.Text = row.Cells[2].Value.ToString();
            textBox4.Text = row.Cells[3].Value.ToString();
            textBox5.Text = row.Cells[4].Value.ToString();
            comboBox1.Text = row.Cells[5].Value.ToString();
           dateTimePicker1.Text = row.Cells[6].Value.ToString();

        }

        //Edit Button Επεξεργασία δεδομένων στα πεδια και επιστροφή στην γραμμή νέων δεδομένων
        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[i];
            row.Cells[0].Value = textBox1.Text;
            row.Cells[1].Value = textBox2.Text;
            row.Cells[2].Value = textBox3.Text;
            row.Cells[3].Value = textBox4.Text;
            row.Cells[4].Value = textBox5.Text;
            row.Cells[5].Value = comboBox1.Text;
            row.Cells[6].Value = dateTimePicker1.Text;
        }

        
        //Save Button
        private void saveToAddressBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory;
            TextWriter addressdata = new StreamWriter(path + @"\AddressBook.txt");
            for (int i = 0; i < dataGridView1.Rows.Count-1 ;i++)
            {
                for (int j = 0; j <= dataGridView1.Columns.Count-1 ; j++)
                {
                    addressdata.Write(dataGridView1.Rows[i].Cells[j].Value.ToString()+"|");
                }
                    addressdata.WriteLine();
                   
                    

                }
            addressdata.Close();
            MessageBox.Show("Data Exported");
            }

        //Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //(event) Αναζήτηση Επαφής 
        private void textBox8_KeyUp(object sender, KeyEventArgs e)
        {
            if (radioButton1.Checked)
            {
                filter = "Name";

            }
            if (radioButton2.Checked)
            {
                filter = "Lastname";

            }
            if (radioButton3.Checked)
            {
                filter = "Phone";
            }
            dt.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filter, (textBox8.Text));
        }

        //Περιορίζει τον χρήστη να πληκτρολογεί μόνο αριθμούς στο πεδίο του αριθμού τηλεφώνου
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }
    }

    //1η κλαση: έλεγχος email 
    public class CheckEmail
    {
        public bool EmailRegex(string str)
        {

            Regex CheckEmail = new Regex(@"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z");
            return CheckEmail.IsMatch(str);
        }


    }

    //2η κλάση: έλεγχος τηλεφώνου
    public class CheckPhone
    {
       public bool Phone(int str)
        {          
                if (str >= 10 && str <= 20)
                return true;
            else
            {
                return false;
            }
        }


    }
}
