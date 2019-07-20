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

        Color workingInstColor = Color.White;
        Color workingMultiColor = Color.Gold;
        Color nonWorkingInstColor = Color.DimGray;
        Color nonWorkingMultiColor = Color.DarkOrange;

        const int fileNameGridIndex = 3;
        const int starGridIndex = 0;
        const int tagIndex = 2;


        public Form1()
        {
            InitializeComponent();

            listView1.Columns.Add("Stars");
            listView1.Columns.Add("Title");
            listView1.Columns.Add("Tags");
            listView1.Columns.Add("Path");

            DbHelper.createDbIfNotExists();

            cbShowFavoritesOnly.CheckedChanged -= cbShowFavoritesOnly_CheckedChanged;
            cbShowFavoritesOnly.Checked = (DbHelper.getSetting("showstarredonly") == "T") ? true : false;
            cbShowFavoritesOnly.CheckedChanged += cbShowFavoritesOnly_CheckedChanged;

            textBox1.DelayedTextChanged -= textBox1_DelayedTextChanged;
            textBox1.Text = DbHelper.getSetting("searchterm");
            textBox1.DelayedTextChanged += textBox1_DelayedTextChanged; //having this all the way down here hopefully will defeat the delay

            cbShowWorking.CheckedChanged -= cbShowWorking_CheckedChanged;
            cbShowWorking.Checked = (DbHelper.getSetting("showworking") == "T") ? true : false;
            cbShowWorking.CheckedChanged += cbShowWorking_CheckedChanged;

            cbShowMultisOnly.CheckedChanged -= cbShowMultisOnly_CheckedChanged;
            cbShowMultisOnly.Checked = (DbHelper.getSetting("showmultisonly") == "T") ? true : false;
            cbShowMultisOnly.CheckedChanged += cbShowMultisOnly_CheckedChanged;


            /*
            cbShowMissing.CheckedChanged -= cbShowWorking_CheckedChanged;
            cbShowFavoritesOnly.Checked = (DbHelper.getSetting("showmissingonly") == "T") ? true : false;
            cbShowWorking.CheckedChanged += cbShowWorking_CheckedChanged;

            cbShowMissing.CheckedChanged -= cbShowWorking_CheckedChanged;
            cbShowFavoritesOnly.Checked = (DbHelper.getSetting("hidemissing") == "T") ? true : false;
            cbShowWorking.CheckedChanged += cbShowWorking_CheckedChanged;
            */



            //THIS BELOW HAPPENS BY THE DELAYEDTEXTCHANGED WHICH I CAN'T SEEM TO DISABLE UPON FIRST RUN :(
            /*
            if (FiltersApplied())
            {
                clearAndLoadTable();
            }*/

        }

        private bool FiltersApplied()
        {
            if (cbShowFavoritesOnly.Checked == true || textBox1.Text.Length > 2)
            {
                return true;
            }
            return false;
        }

        const char star = ('\u2605');

        private void addToGridAndDb(string file, int stars)
        {
            var fi = new FileInfo(file);
            var inst = new Instrument(Path.GetFileNameWithoutExtension(file), file, 0, "", fi.Length, 0);
            if (DbHelper.insertRec(inst))
            {
                addToGrid(inst);
            }
        }




       
        private void addToGrid(Instrument i)
        {
            string starr = "";
            for (int j = 0; j < i.GetStars(); j++)
            {
                starr += star;
            }

            string[] row = {starr, i.GetName(), i.GetTags(),
                               i.GetFile()};
            var listViewItem = new ListViewItem(row);
            listViewItem.Name = i.GetFile(); //accessed by 'key' later when removing
            if (i.GetLoadingFails())
            {
                if (!i.isNkmFile())
                {
                    listViewItem.ForeColor = nonWorkingInstColor;
                }
                else
                {
                    listViewItem.ForeColor = nonWorkingMultiColor;
                }
            }
            else
            {
                if (!i.isNkmFile())
                {
                    listViewItem.ForeColor = workingInstColor;
                }
                else
                {
                    listViewItem.ForeColor = workingMultiColor;
                }
            }

            listView1.Items.Add(listViewItem);
        }



        private void listBox1_DragLeave(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            var item = lb.SelectedItem;
            MessageBox.Show(item.ToString());
        }


        public void ShowTagsDialogBox(bool trueToAddOrFalseToReplace) //apologies for this naming
        {
            Form2 tagDialog = new Form2();

            // Show tagDialog as a modal dialog and determine if DialogResult = OK.
            if (tagDialog.ShowDialog(this) == DialogResult.OK)
            {
                // Read the contents of tagDialog's TextBox.
               var tags = tagDialog.textBox1.Text;

                bool replaceTags = !trueToAddOrFalseToReplace;

                if (replaceTags)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        item.SubItems[tagIndex].Text = "";
                    }
                }
                AddTagsToSelected(tags);

            }
            tagDialog.Dispose();
        }


        public void ClearTagsFromSelected()
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.setTags(item.SubItems[fileNameGridIndex].Text, "");
                item.SubItems[tagIndex].Text = "";
            }
        }
   

        public void AddTagsToSelected(string tagListString)
        {
            var tags = tagListString.Split(',');


            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var tagString = item.SubItems[tagIndex].Text;
                var presentTags = tagString.Split(' ');
                foreach (var tag in tags)
                {
                    if (!presentTags.Any(x => x.ToUpper() == tag.ToUpper()))
                    {
                        if (tagString.Length > 0)
                        {
                            tagString += " " + tag.Trim();
                        }
                        else
                        {
                            tagString = tag.Trim();
                        }
                    }
                }

                DbHelper.setTags(item.SubItems[fileNameGridIndex].Text, tagString);
                item.SubItems[tagIndex].Text = tagString;
            }

        }






        private void SizeLastColumn(ListView lv)
        {
            try
            {
                //lv.Columns[lv.Columns.Count - 1].Width = -2;

                int x = lv.Width / 12 == 0 ? 1 : lv.Width / 12;
                lv.Columns[0].Width = x;
                lv.Columns[1].Width = x * 3;
                lv.Columns[2].Width = x * 1;
                lv.Columns[3].Width = x * 7;
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
                selection.Add(item.SubItems[fileNameGridIndex].Text);
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
                MenuItem mnuAddTags = new MenuItem("Add Tag(s)");
                MenuItem mnuReplaceTags = new MenuItem("Replace Tag(s)");
                MenuItem mnuClearTags = new MenuItem("Clear Tag(s)");
                MenuItem mnuDoesntWork = new MenuItem("Doesn't Work");
                MenuItem mnuWorks = new MenuItem("Works");
                MenuItem mnuDelete = new MenuItem("Delete from Index");
                MenuItem mnuDedupTop = new MenuItem("Remove Duplicates Keep Top Alpha");
                MenuItem mnuDedupBottom = new MenuItem("Remove Duplicates Keep Bottom Alpha");
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
                mnuDedupTop.Click += new EventHandler(menuItemDedupTop);
                mnuDedupBottom.Click += new EventHandler(menuItemDedupBottom);
                mnuOpenFolder.Click += new EventHandler(menuItemOpenContainingFolder);

                mnuAddTags.Click += new EventHandler(menuItemAddTags);
                mnuReplaceTags.Click += new EventHandler(menuItemReplaceTags);
                mnuClearTags.Click += new EventHandler(menuItemClearTags);


                List<MenuItem> menuItems = new List<MenuItem>();

                //menuItems.Add(new MenuItem("Clear Tags"));
                //menuItems.Add(new MenuItem("Add Tags"));
                menuItems.Add(mnuNotFavorite);
                menuItems.Add(mnu1star);
                menuItems.Add(mnu2star);
                menuItems.Add(mnu3star);
                menuItems.Add(mnu4star);
                menuItems.Add(mnuFavorite);
                menuItems.Add(mnuAddTags);
                menuItems.Add(mnuReplaceTags);
                menuItems.Add(mnuClearTags);
                menuItems.Add(mnuDoesntWork);
                menuItems.Add(mnuWorks);
                menuItems.Add(mnuDoesntWork);
                menuItems.Add(mnuDedupTop);
                menuItems.Add(mnuDedupBottom);
                menuItems.Add(mnuDelete);


                //If paths match, allow "open containing folder" thing
                string path = "";
                bool pathMismatch = false;
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    var temppath = Path.GetDirectoryName(lvi.SubItems[fileNameGridIndex].Text);
                    if (path.Length > 1)
                    {
                        if (temppath != path)
                        {
                            pathMismatch = true;
                            break;
                        }
                    }
                    path = Path.GetDirectoryName(lvi.SubItems[fileNameGridIndex].Text);
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
        private void menuItemAddTags(object sender, EventArgs e)
        {
            ShowTagsDialogBox(true);
        }
        private void menuItemReplaceTags(object sender, EventArgs e)
        {
            ShowTagsDialogBox(false);
        }

        private void menuItemClearTags(object sender, EventArgs e)
        {
            ClearTagsFromSelected();
        }


        private void menuItemClickDoesntWork(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var fileName = item.SubItems[fileNameGridIndex].Text;
                DbHelper.markDoesntWork(fileName);
                if (cbShowNonWorking.Checked)
                {
                    if (fileName.ToLower().EndsWith("nki"))
                    {
                        item.ForeColor = nonWorkingInstColor;
                    }
                    else if (fileName.ToLower().EndsWith("nkm"))
                    {
                        item.ForeColor = nonWorkingMultiColor;
                    }

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
                DbHelper.markFavorite(item.SubItems[fileNameGridIndex].Text);
                string starr = "";
                for (int i = 0; i < 5; i++)
                {
                    starr += star;
                }
                item.SubItems[starGridIndex].Text = starr;
            }
            listView1.ContextMenu.Dispose();
        }

        private void menuItemDedupTop(object sender, EventArgs e)
        {
            DbHelper.optionalBeginTransactionForSpeed();
            var deletedInstruments = DbHelper.Dedupe(GetSelectedInstruments(), true);
            DbHelper.optionalEndTransactionForSpeed();
            RemoveInstrumentsFromView(deletedInstruments);
        }
        private void menuItemDedupBottom(object sender, EventArgs e)
        {
            DbHelper.optionalBeginTransactionForSpeed();
            var deletedInstruments = DbHelper.Dedupe(GetSelectedInstruments(), false);
            DbHelper.optionalEndTransactionForSpeed();
            RemoveInstrumentsFromView(deletedInstruments);
        }

        private void RemoveInstrumentsFromView(List<string> filenames)
        {
            if (filenames.Count < 200)
            {
                foreach (string instfile in filenames)
                {
                    listView1.Items.RemoveByKey(instfile);
                    updateTextBoxWithItemCount();
                }
            }
            else
            {
                clearAndLoadTable();
            }
        }

        private void updateTextBoxWithItemCount()
        {
            lblStatus.Text = listView1.Items.Count + " Items";
        }

        private void updateTextBoxWithItemCountWithLoadingMessage(int count)
        {

            this.lblStatus.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = "Loading " + count + " Items ...";
                // Running on the UI thread
            });
        }

        private List<Instrument> GetSelectedInstruments()
        {
            List<Instrument> instruments = new List<Instrument>();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var file = item.SubItems[fileNameGridIndex].Text;
                FileInfo fi = new FileInfo(file);
                Instrument i = new Instrument(
                        item.SubItems[0].Text,
                        file, 0, "", fi.Length, 0);
                instruments.Add(i);
            }
            return instruments;
        }


        private void menuItemClick1star(object sender, EventArgs e)
        {
            var stars = 1;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[fileNameGridIndex].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[starGridIndex].Text = starr;
            }
        }

        private void menuItemClick2star(object sender, EventArgs e)
        {
            var stars = 2;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[fileNameGridIndex].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[starGridIndex].Text = starr;
            }
        }

        private void menuItemClick3star(object sender, EventArgs e)
        {
            var stars = 3;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[fileNameGridIndex].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[starGridIndex].Text = starr;
            }
        }

        private void menuItemClick4star(object sender, EventArgs e)
        {
            var stars = 4;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markStars(item.SubItems[fileNameGridIndex].Text, stars);
                string starr = "";
                for (int i = 0; i < stars; i++)
                {
                    starr += star;
                }
                item.SubItems[starGridIndex].Text = starr;
            }
        }



        private void menuItemClickMakeNotFavorite(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markFavorite(item.SubItems[fileNameGridIndex].Text, false);
                item.SubItems[starGridIndex].Text = "";
            }
        }

        private void menuItemClickWorks(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.markWorks(item.SubItems[fileNameGridIndex].Text);
                item.ForeColor = workingInstColor;
            }
        }

        private void menuItemRemove(object sender, EventArgs e)
        {
            var doRefresh = false;// listView1.SelectedItems.Count > 100;

            DbHelper.optionalBeginTransactionForSpeed();
            var count = 0;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                DbHelper.Delete(item.SubItems[fileNameGridIndex].Text);

                count++;
                if (count == 1000)
                {
                    count = 0;
                    DbHelper.optionalEndTransactionForSpeed();
                    DbHelper.optionalBeginTransactionForSpeed();
                }

                if (!doRefresh)
                {
                    listView1.Items.Remove(item);
                }
            }
            DbHelper.optionalEndTransactionForSpeed();
            if (doRefresh)
            {
                clearAndLoadTable();
            }
            else
            {
                updateTextBoxWithItemCount();
            }
        }

        private void menuItemOpenContainingFolder(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                //var path = Path.GetDirectoryName(item.SubItems[2].Text);
                var path = item.SubItems[fileNameGridIndex].Text;
                string argument = "/select, \"" + path + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
                break;
            }
        }


        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            var now = DateTime.Now.Ticks;

            if (!dragFromInside)
            {
                DbHelper.optionalBeginTransactionForSpeed();
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = null;
                var count = 0;
                foreach (string fileOrDir in files)
                {
                    fileName = considerItemForGrid(fileOrDir);
                    count++;

                    if (count == 1000) //commit in batches of 1000
                    {
                        DbHelper.optionalEndTransactionForSpeed();
                        DbHelper.optionalBeginTransactionForSpeed();
                        count = 0;
                    }
                }
                DbHelper.optionalEndTransactionForSpeed();

                //clearAndLoadTable();
                if (fileName != null)
                {
                    try
                    {
                        listView1.EnsureVisible(listView1.Items.IndexOfKey(fileName));
                    }
                    catch (Exception)
                    {
                        //probably added while a filter was applied
                    }
                }
            }

            var nownow = DateTime.Now.Ticks;

            long elapsed = nownow - now;
            //MessageBox.Show("Elapsed: " + elapsed);
            //old way, ~6-7 seconds
            //new way, like 5 seconds.... but WAY quicker when records already present
            //with persistent DB connection, like 2 seconds
            //with transactions, ho boy


            updateTextBoxWithItemCount();
        }

        /// <summary>
        /// Adds items to grid
        /// </summary>
        /// <param name="fileOrDirectory"></param>
        /// <returns>last filename added</returns>
        private string considerItemForGrid(string fileOrDirectory)
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
                if (Path.GetExtension(fileOrDirectory).ToLower() == ".nki" ||
                    Path.GetExtension(fileOrDirectory).ToLower() == ".nkm" 
                    )
                {
                    //if (DbHelper.IsNotInDatabase(fileOrDirectory)) //just file, this varible name sucks
                    if (true)
                    {
                        addToGridAndDb(fileOrDirectory, 0);
                    }
                    return fileOrDirectory;
                }
            }
            return null;
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
            //Decided to move any functionality to a press of the Enter key which is much less problematic
        }


        List<Instrument> instruments;

        private void clearAndLoadTable()
        {
            listView1.Items.Clear();

            instruments = new List<Instrument>();

            var searchTerm = "";
            if (textBox1.Text.Length > 0)
                searchTerm = textBox1.Text;

            instruments = DbHelper.GetInstruments(searchTerm, cbShowFavoritesOnly.Checked, cbShowWorking.Checked, cbShowMissing.Checked, cbHideMissing.Checked, cbShowMultisOnly.Checked);

            if (instruments.Count > 1000)
            {
                listView1.Sorting = SortOrder.None;
                listView1.ListViewItemSorter = null;
            }
            updateTextBoxWithItemCountWithLoadingMessage(instruments.Count);
            textBox1.Refresh();

            //System.Threading.Thread.Sleep(3000); //testing UI refresh

            foreach (var i in instruments)
            {
                if (cbShowNonWorking.Checked || !i.GetLoadingFails())
                    addToGrid(i);
            }

            updateTextBoxWithItemCount();
            checkForMissingFiles();
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

        private void cbShowWorking_CheckedChanged(object sender, EventArgs e)
        {
            string value = (cbShowWorking.Checked) ? "T" : "F";
            DbHelper.setSetting("showworking", value);
            clearAndLoadTable();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void checkForMissingFiles()
        {
            DbHelper.optionalBeginTransactionForSpeed();
            foreach (ListViewItem item in listView1.Items)
            {
                FileInfo fi = new FileInfo(item.SubItems[fileNameGridIndex].Text);
                if (!fi.Exists)
                {
                    item.ForeColor = Color.DarkRed;
                    DbHelper.markMissingFile(item.SubItems[fileNameGridIndex].Text);
                }
                else if (fi.Exists && item.ForeColor == Color.DarkRed)
                {
                    item.ForeColor = workingInstColor;
                    DbHelper.markMissingFile(item.SubItems[fileNameGridIndex].Text, false);
                }
            }
            DbHelper.optionalEndTransactionForSpeed();
        }

        private void cbShowMissing_CheckedChanged(object sender, EventArgs e)
        {
            string value = (cbShowMissing.Checked) ? "T" : "F";
            DbHelper.setSetting("showmissingonly", value);
            clearAndLoadTable();
        }

        private void cbHideMissing_CheckedChanged(object sender, EventArgs e)
        {
            string value = (cbHideMissing.Checked) ? "T" : "F";
            DbHelper.setSetting("hidemissing", value);
            clearAndLoadTable();
        }

        private void cbShowMultisOnly_CheckedChanged(object sender, EventArgs e)
        {
            string value = (cbShowMultisOnly.Checked) ? "T" : "F";
            DbHelper.setSetting("showmultisonly", value);
            clearAndLoadTable();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                /*
                if (textBox1.Text.Length > 2)
                {*/
                    DbHelper.setSetting("searchterm", textBox1.Text);
                /*}
                else if (textBox1.Text.Length == 0)
                {
                    DbHelper.setSetting("searchterm", "");
                }
                else
                {
                    return; //do not 
                }*/
                clearAndLoadTable();
            }
        }
    }
}
