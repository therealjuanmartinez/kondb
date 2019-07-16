using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JIndexer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            String[] files = Directory.GetFiles(@"C:\");
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn());
            table.Rows.Add("File Name");

            for (int i = 0; i < files.Length; i++)
            {
                //table.Rows.Add(files[i]);
                listBox1.Items.Add(files[i]);
            }



            //dataGridView1.DataSource = table;
        }


        private void listBox1_DragLeave(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;

            var item = lb.SelectedItem;

            //lb.Items.Remove(lb.SelectedItem);
            MessageBox.Show(item.ToString());

        }

     
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DragLeave_1(object sender, EventArgs e)
        {
            Debug.Print("fdsa");
//            MessageBox.Show("Leave " + listBox1.SelectedItem.ToString());
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.Print("downfdsa");

        }

    }
}
