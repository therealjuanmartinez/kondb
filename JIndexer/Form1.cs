using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                table.Rows.Add(files[i]);
            }

            dataGridView1.DataSource = table;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
    }
}
