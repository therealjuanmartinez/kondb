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
            listView1.Columns.Add("path");
            listView1.Columns.Add("number");
            listView1.Columns.Add("star");

            DbHelper.createDbIfNotExists();


            /*
            String[] files = Directory.GetFiles(@"C:\temp");
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn());
            table.Rows.Add("File Name");

            var count = 0;
            for (int i = 0; i < files.Length; i++)
            {
                var starStr = "";
                for (int j = 0; j < count; j++)
                {
                    starStr += star;
                }
                count = (count > 4) ? 0 : count + 1;

                string[] row = { files[i], "ffa", Convert.ToString(i), starStr };
                var listViewItem = new ListViewItem(row);

                if (i % 2 == 0)
                {
                    listViewItem.ForeColor = Color.DimGray;
                }

                listView1.Items.Add(listViewItem);
            }
            */

            var instruments = DbHelper.GetInstruments();
            foreach (var i in instruments)
            {
                addToGrid(i);
            }

            SizeLastColumn(listView1);
            //dataGridView1.DataSource = table;
        }

        const char star = ('\u2605');

        private void addToGridAndDb(string file, int stars)
        {
            //todo refactor
            string starr = "";
            for (int i = 0; i < stars; i++)
            {
                starr += star;
            }
            
            string[] row = { Path.GetFileNameWithoutExtension(file),
                               file, null, starr };
            var listViewItem = new ListViewItem(row);
            listView1.Items.Add(listViewItem);

            var fi = new FileInfo(row[1]);
            var inst = new Instrument(row[0], row[1], 0, "", fi.Length, 0);
            DbHelper.insertRec(inst);
        }


        private void addToGrid(Instrument i)
        {
            string starr = "";
            for (int j = 0; j < i.GetStars(); j++)
            {
                starr += star;
            }

            string[] row = { i.GetName(),
                               i.GetFile(), i.GetTags(), starr };
            var listViewItem = new ListViewItem(row);
            listView1.Items.Add(listViewItem);
        }



        private void listBox1_DragLeave(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;

            var item = lb.SelectedItem;

            //lb.Items.Remove(lb.SelectedItem);
            MessageBox.Show(item.ToString());

        }


        private void SizeLastColumn(ListView lv)
        {
            try
            {
                //lv.Columns[lv.Columns.Count - 1].Width = -2;

                int x = lv.Width / 6 == 0 ? 1 : lv.Width / 6;
                lv.Columns[0].Width = x * 3;
                lv.Columns[1].Width = x ;
                lv.Columns[2].Width = x;
                lv.Columns[3].Width = x;
            }

            catch (Exception e)
            { }
        }
        


        /*

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
        */



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



            //Ensure selected item can be seen after sorting
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var index = 0;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    var lvi = listView1.Items[i];
                    if (lvi.Selected)
                    {
                        index = i;
                        break;
                    }
                }
                listView1.EnsureVisible(index);
                break;
            }
        }


        private bool Resizing = false;
        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            //SizeLastColumn((ListView)sender);

            /*
            // Don't allow overlapping of SizeChanged callse
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

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<string> selection = new List<string>();


            foreach (ListViewItem item in listView1.SelectedItems)
            {
                int imgIndex = item.ImageIndex;
                //selection.Add(filenames[imgIndex]);
                selection.Add(item.SubItems[1].Text);
            }

            DataObject data = new DataObject(DataFormats.FileDrop, selection.ToArray());
            DoDragDrop(data, DragDropEffects.Copy);
            //DoDragDrop("C:\\temp\\training.xlsx", DragDropEffects.Copy);
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                List<string> selection = new List<string>();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    int imgIndex = item.ImageIndex;
                    //selection.Add(filenames[imgIndex]);
                    selection.Add(item.SubItems[0].Text);
                    //Debug.Print(item.SubItems[0].Text);

                    MenuItem[] mi = new MenuItem[] {
                        new MenuItem("Clear Tags"),
                        new MenuItem("Add Tags"),
                        new MenuItem("Works"),
                        new MenuItem("Doesn't Work"),
                        new MenuItem("Open Containing Folder")
                    };
                    listView1.ContextMenu = new ContextMenu(mi);
                    //match = true;
                    //break;

                    listView1.ContextMenu.Show(listView1, new Point(e.X, e.Y));
                }
                /*
                if (listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    MessageBox.Show("right-click");
            listView1.Columns.Add("number");
                }*/
            }
        }


     

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {

            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileOrDir in files)
            {

                considerItemForGrid(fileOrDir);
            }
        }

        private void considerItemForGrid(string fileOrDirectory)
        {
            FileAttributes attr = File.GetAttributes(fileOrDirectory);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
               // Debug.Print(fileOrDirectory + "Its a directory");
                foreach (string f in Directory.GetFiles(fileOrDirectory))
                {
                    considerItemForGrid(f);
                }
                try
                {
                    foreach (string d in Directory.GetDirectories(fileOrDirectory))
                    {
                        considerItemForGrid(d);
                    }
                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }

            }
            else
            {
               // Debug.Print(fileOrDirectory + "Its a file");
                if (Path.GetExtension(fileOrDirectory).ToLower() == ".nki")
                {
                    addToGridAndDb(fileOrDirectory, 0);
                    return;
                }
            }








          

            /*
            try
            {
                foreach (string d in Directory.GetDirectories(fileOrDirectory))
                {
                    considerItemForGrid(d); //recurse into directory
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }*/
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;

         
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            SizeLastColumn(listView1);
        }

       
        private void textBox1_DelayedTextChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            List<Instrument> instruments = new List<Instrument>();

            if (textBox1.Text.Length > 0)
            {
                instruments = DbHelper.GetInstruments(textBox1.Text);
            }
            else
            {
                instruments = DbHelper.GetInstruments();
            }

            foreach (var i in instruments)
            {
                addToGrid(i);
            }

        }
    }
}
