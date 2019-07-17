﻿using System;
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
            
            listView1.Columns.Add("Title");
            listView1.Columns.Add("Tags");
            listView1.Columns.Add("Path");
            listView1.Columns.Add("Stars");

            DbHelper.createDbIfNotExists();

            cbShowFavoritesOnly.CheckedChanged -= cbShowFavoritesOnly_CheckedChanged;
            cbShowFavoritesOnly.Checked = (DbHelper.getSetting("showstarredonly") == "T") ? true : false;
            cbShowFavoritesOnly.CheckedChanged += cbShowFavoritesOnly_CheckedChanged;
            textBox1.DelayedTextChanged -= textBox1_DelayedTextChanged;
            textBox1.Text = DbHelper.getSetting("searchterm");
            textBox1.DelayedTextChanged += textBox1_DelayedTextChanged;

            if (FiltersApplied())
            {
                clearAndLoadTable();
            }

        }

        private bool FiltersApplied()
        {
            if (cbShowFavoritesOnly.Checked == true || textBox1.Text.Length > 2) {
                return true;
            }
            return false;
        }

        const char star = ('\u2605');

        private void addToGridAndDb(string file, int stars)
        {
            var fi = new FileInfo(file);
            var inst = new Instrument(Path.GetFileNameWithoutExtension(file), file, 0, "", fi.Length, 0);
            DbHelper.insertRec(inst);
        }


        private void addToGrid(Instrument i)
        {
            string starr = "";
            for (int j = 0; j < i.GetStars(); j++)
            {
                starr += star;
            }

            string[] row = { i.GetName(), i.GetTags(),
                               i.GetFile(), starr };
            var listViewItem = new ListViewItem(row);
            if (i.GetLoadingFails())
            {
                listViewItem.ForeColor = Color.DimGray;
            }
            listView1.Items.Add(listViewItem);
        }



        private void listBox1_DragLeave(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            var item = lb.SelectedItem;
            MessageBox.Show(item.ToString());
        }


        private void SizeLastColumn(ListView lv)
        {
            try
            {
                //lv.Columns[lv.Columns.Count - 1].Width = -2;

                int x = lv.Width / 6 == 0 ? 1 : lv.Width / 6;
                lv.Columns[0].Width = x * 1;
                lv.Columns[1].Width = x * 1;
                lv.Columns[2].Width = x * 2;
                lv.Columns[3].Width = x;
            }

            catch (Exception e)
            { }
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

            // Sort
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


        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<string> selection = new List<string>();


            foreach (ListViewItem item in listView1.SelectedItems)
            {
                int imgIndex = item.ImageIndex;
                //selection.Add(filenames[imgIndex]);
                selection.Add(item.SubItems[2].Text);
            }

            DataObject data = new DataObject(DataFormats.FileDrop, selection.ToArray());

            dragFromInside = true;
            DoDragDrop(data, DragDropEffects.Copy);
            dragFromInside = false;
            //DoDragDrop("C:\\temp\\training.xlsx", DragDropEffects.Copy);
        }

        bool dragFromInside = false;

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    return;
                }

                MenuItem mnuNotFavorite = new MenuItem("Clear Stars");
                MenuItem mnu1star = new MenuItem("1 Stars");
                MenuItem mnu2star = new MenuItem("2 Stars");
                MenuItem mnu3star = new MenuItem("3 Stars");
                MenuItem mnu4star = new MenuItem("4 Stars");
                MenuItem mnuFavorite = new MenuItem("5 Stars");
                MenuItem mnuDoesntWork = new MenuItem("Doesn't Work");
                MenuItem mnuWorks = new MenuItem("Works");
                MenuItem mnuDelete = new MenuItem("Delete from Index");
                MenuItem mnuOpenFolder = new MenuItem("Open Containing Folder");
                mnu1star.Click += new EventHandler(menuItemClick1star);
                mnu2star.Click += new EventHandler(menuItemClick2star);
                mnu3star.Click += new EventHandler(menuItemClick3star);
                mnu4star.Click += new EventHandler(menuItemClick4star);
                mnuFavorite.Click += new EventHandler(menuItemClickMakeFavorite);
                mnuNotFavorite.Click += new EventHandler(menuItemClickMakeNotFavorite);
                mnuDoesntWork.Click += new EventHandler(menuItemClickDoesntWork);
                mnuWorks.Click += new EventHandler(menuItemClickWorks);
                mnuDelete.Click += new EventHandler(menuItemRemove);
                mnuOpenFolder.Click += new EventHandler(menuItemOpenContainingFolder);


                List<MenuItem> menuItems = new List<MenuItem>();

                //menuItems.Add(new MenuItem("Clear Tags"));
                //menuItems.Add(new MenuItem("Add Tags"));
                menuItems.Add(mnuNotFavorite);
                menuItems.Add(mnu1star);
                menuItems.Add(mnu2star);
                menuItems.Add(mnu3star);
                menuItems.Add(mnu4star);
                menuItems.Add(mnuFavorite);
                menuItems.Add(mnuWorks);
                menuItems.Add(mnuDoesntWork);
                menuItems.Add(mnuDelete);


                //If paths match, allow "open containing folder" thing
                string path = "";
                bool pathMismatch = false;
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    var temppath = Path.GetDirectoryName(lvi.SubItems[2].Text);
                    if (path.Length > 1)
                    {
                        if (temppath != path)
                        {
                            pathMismatch = true;
                            break;
                        }
                    }
                    path = Path.GetDirectoryName(lvi.SubItems[2].Text);
                }

                if (!pathMismatch)
                {
                    menuItems.Add(mnuOpenFolder);
                }

                Debug.Print("Right-Click Menu");

                listView1.ContextMenu = new ContextMenu(menuItems.ToArray());
                listView1.ContextMenu.Show(listView1, new Point(e.X, e.Y));

            }
        }

        private void menuItemClickDoesntWork(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markDoesntWork(item.SubItems[2].Text);
                if (cbShowNonWorking.Checked)
                {
                    item.ForeColor = Color.DimGray;
                }
                else
                {
                    item.Remove();
                }
            }
        }
        private void menuItemClickMakeFavorite(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markFavorite(item.SubItems[2].Text);
                string starr = "";
                for (int i = 0; i < 5; i++) {
                    starr += star;
                }
                item.SubItems[3].Text = starr;
            }
            listView1.ContextMenu.Dispose();
        }

        private void menuItemClick1star(object sender, EventArgs e)
        {
            var stars = 1;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[2].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[3].Text = starr;
            }
        }

        private void menuItemClick2star(object sender, EventArgs e)
        {
            var stars = 2;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[2].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[3].Text = starr;
            }
        }

        private void menuItemClick3star(object sender, EventArgs e)
        {
            var stars = 3;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[2].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[3].Text = starr;
            }
        }

        private void menuItemClick4star(object sender, EventArgs e)
        {
            var stars = 4;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[2].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[3].Text = starr;
            }
        }



        private void menuItemClickMakeNotFavorite(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markFavorite(item.SubItems[2].Text, false);
                item.SubItems[3].Text = "";
            }
        }

        private void menuItemClickWorks(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markWorks(item.SubItems[2].Text);
                item.ForeColor = Color.White;
            }
        }

        private void menuItemRemove(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.Delete(item.SubItems[2].Text);
                listView1.Items.Remove(item);
            }
        }

        private void menuItemOpenContainingFolder(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                //var path = Path.GetDirectoryName(item.SubItems[2].Text);
                var path = item.SubItems[2].Text;
                string argument = "/select, \"" + path + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
                break;
            }
        }


        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!dragFromInside)
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string fileOrDir in files)
                {

                    considerItemForGrid(fileOrDir);
                }
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
                    if (DbHelper.IsNotInDatabase(fileOrDirectory)) //just file, this varible name sucks
                    {
                        addToGridAndDb(fileOrDirectory, 0);
                    }
                    return;
                }
            }

        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;

         
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            //SizeLastColumn(listView1);
        }

       
        private void textBox1_DelayedTextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 2)
            {
                DbHelper.setSetting("searchterm", textBox1.Text);
            }
            else if (textBox1.Text.Length == 0)
            {
                DbHelper.setSetting("searchterm", "");
            }
            else
            {
                return; //do not 
            }
            clearAndLoadTable();
        }

        private void clearAndLoadTable()
        {
            listView1.Items.Clear();

            List<Instrument> instruments = new List<Instrument>();

            if (textBox1.Text.Length > 0)
            {
                instruments = DbHelper.GetInstruments(textBox1.Text, cbShowFavoritesOnly.Checked);
            }
            else
            {
                instruments = DbHelper.GetInstruments("", cbShowFavoritesOnly.Checked);
            }

            if (instruments.Count > 1000)
            {
                listView1.Sorting = SortOrder.None;
            }

            foreach (var i in instruments)
            {
                if (cbShowNonWorking.Checked || !i.GetLoadingFails())
                {
                    addToGrid(i);
                }
            }

            lblStatus.Text = listView1.Items.Count + " Items";
        }

        private void cbShowNonWorking_CheckedChanged(object sender, EventArgs e)
        {
            clearAndLoadTable();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.F1Size.Width == 0 || Properties.Settings.Default.F1Size.Height == 0)
            {
                // first start
                // optional: add default values
            }
            else
            {
                this.WindowState = Properties.Settings.Default.F1State;

                // we don't want a minimized window at startup
                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;

                this.Location = Properties.Settings.Default.F1Location;
                this.Size = Properties.Settings.Default.F1Size;

                SizeLastColumn(listView1);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.F1State = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.F1Location = this.Location;
                Properties.Settings.Default.F1Size = this.Size;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.F1Location = this.RestoreBounds.Location;
                Properties.Settings.Default.F1Size = this.RestoreBounds.Size;
            }

            // don't forget to save the settings
            Properties.Settings.Default.Save();
        }

        private void cbShowFavoritesOnly_CheckedChanged(object sender, EventArgs e)
        {
            string value = (cbShowFavoritesOnly.Checked) ? "T" : "F";
            DbHelper.setSetting("showstarredonly", value);
            clearAndLoadTable();
        }
    }
}
