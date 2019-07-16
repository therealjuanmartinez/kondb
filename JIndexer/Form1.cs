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

            listView1.Columns.Add("title");
            listView1.Columns.Add("next");
            listView1.Columns.Add("number");
            listView1.Columns.Add("star");

            String[] files = Directory.GetFiles(@"C:\temp");
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn());
            table.Rows.Add("File Name");

            string star = Convert.ToString('\u2605');

            for (int i = 0; i < files.Length; i++)
            {
                string[] row = { files[i], "ffa", Convert.ToString(i), star + star };
                var listViewItem = new ListViewItem(row);
                listView1.Items.Add(listViewItem);
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

        private void listBox1_RightClic(object sender, MouseEventArgs e)
        {
            Debug.Print("downfdsa");
        }



        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = listView1.Columns[e.Column];

            Debug.Print("Column Click");

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;
            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            listView1.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            // Sort.
            listView1.Sort();
        }


        private bool Resizing = false;
        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            /*
            // Don't allow overlapping of SizeChanged calls
            if (!Resizing)
            {
                // Set the resizing flag
                Resizing = true;

                ListView listView = sender as ListView;
                if (listView != null)
                {
                    float totalColumnWidth = 0;

                    // Get the sum of all column tags
                    for (int i = 0; i < listView.Columns.Count; i++)
                        totalColumnWidth += Convert.ToInt32(listView.Columns[i].Tag);

                    // Calculate the percentage of space each column should 
                    // occupy in reference to the other columns and then set the 
                    // width of the column to that percentage of the visible space.
                    for (int i = 0; i < listView.Columns.Count; i++)
                    {
                        float colPercentage = (Convert.ToInt32(listView.Columns[i].Tag) / totalColumnWidth);
                        listView.Columns[i].Width = (int)(colPercentage * listView.ClientRectangle.Width);
                    }
                }
            }

            // Clear the resizing flag
            Resizing = false;
            */
        }

    }
}
