using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CoPa_Program
{
    public partial class CoRePa : Form
    {
        bool folder;
        bool loaded = false;
        DirectoryInfo sf = new DirectoryInfo(@"C:\ProgramData\StrataGem571\CoRePa\Settings");
        bool selectChecked = false;
        object settings;

        public CoRePa()
        {
            InitializeComponent();
            if (sf.Exists != true)
            {
                sf = new DirectoryInfo(@"C:\Users\adamg\Downloads\Source\Settings");
            }
            FileInfo[] sFiles = sf.GetFiles();
            foreach (FileInfo file in sFiles)
            {
                SettingsBox.Items.Add(file);
            }
        }

        public void SettingsBox_DropDownClosed(object sender, EventArgs e)
        {
            if(SettingsBox.Text != "" || SettingsBox.SelectedItem != null)
            {
                if (File.Exists(sf + "\\" + SettingsBox.SelectedItem)) Load(sf + "\\" + SettingsBox.SelectedItem);
                else MessageBox.Show("Settings file could not be loaded", "Error");
            }
            settings = SettingsBox.SelectedItem;
        }

        private new void Load(string source)
        {
            DataGrid.Rows.Clear();
            DataGrid.Refresh();
            DataGrid.RowCount = 1;
            String[] fr = File.ReadAllLines(source); ;
            int i = 0;
            DataGrid.RowCount = fr.Length - 1;
            bool extra = false;
            foreach(string s in fr)
            {
                if (s == ",,,,,,," || s == ",,,,,,")
                {
                    extra = true;
                    continue;
                }
                if (s.Contains(">"))
                {
                    if(s.Contains("f"))
                    {
                        folder = true;
                        loaded = true;
                        FolderButton.Checked = true;
                    }
                    else
                    {
                        folder = false;
                        loaded = true;
                        FolderButton.Checked = false;
                    }
                }
                else
                {
                    string[] line = s.Split(',');
                    if (line[0] == "") DataGrid[0, i].Value = false;
                    else DataGrid[0, i].Value = line[0];
                    DataGrid[1, i].Value = line[1];
                    DataGrid[2, i].Value = line[2];
                    DataGrid[3, i].Value = line[3];
                    DataGrid[4, i].Value = "Open";
                    DataGrid[5, i].Value = line[4];
                    DataGrid[6, i].Value = line[5];
                    DataGrid[7, i].Value = "Open";
                    DataGrid[8, i].Value = line[6];
                    i++;
                }
            }
            int cnt = 0;
            foreach (DataGridViewRow row in DataGrid.Rows)
            {
                DataGrid.Rows[cnt].Cells[1].Style.Font = new Font(DataGrid.Font, FontStyle.Bold);
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Delete")
                {
                    Console.WriteLine(DataGrid.Rows[cnt].Cells[1].Value);
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.DarkRed;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.White;
                }
                else if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Copy & Paste")
                {
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.Yellow;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.Black;
                }
                else if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Cut & Paste")
                {
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.Pink;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.Black;
                }
                cnt++;
            }
        }

        private void FolderButton_CheckChanged(object sender, EventArgs e)
        {
            if (loaded != true)
            {
                folder = !folder;
            }
            DataGrid.Columns[2].Visible = !DataGrid.Columns[2].Visible;
            DataGrid.Columns[5].Visible = !DataGrid.Columns[5].Visible;
            loaded = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string answer = Convert.ToString(MessageBox.Show("Are you sure you want to save this file?", "Save Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question));
            if (answer == "No") return;
            foreach (DataGridViewRow row in DataGrid.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
            if (SettingsBox.Text == "")
            {
                MessageBox.Show("Settings file field is empty. Please add name.", "Error");
            }
            else
            {
                string save = "";
                for (int i = 0; i < DataGrid.RowCount; i++)
                {
                    for (int j = 0; j < DataGrid.ColumnCount; j++)
                    {
                        if(j != 4 && j != 7) save += DataGrid[j, i].Value + ",";
                    }
                    save += "\n";
                }
                save += ">";
                if (FolderButton.Checked == true) save += "f";
                if (File.Exists(sf + "\\" + SettingsBox.Text))
                {
                    File.Delete(sf + "\\" + SettingsBox.Text);
                }
                using (FileStream fs = File.Create(sf + "\\" + SettingsBox.Text))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(save);
                    fs.Write(info, 0, info.Length);
                }

                FileInfo[] sFiles = sf.GetFiles();
                SettingsBox.Items.Clear();
                foreach (FileInfo file in sFiles)
                {
                    SettingsBox.Items.Add(file);
                }
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in DataGrid.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
            if (FolderButton.Checked == false)
            {
                Console.WriteLine("Test");
                
                for (int i = 0; i < DataGrid.RowCount; i++)
                {
                    if (Convert.ToString(DataGrid[1, i].Value) == "Copy & Paste")
                    {
                        String newName = DataGrid[5, i].Value + "";
                        String destination = DataGrid[6, i].Value + "";
                        String name = DataGrid[2, i].Value + "";
                        String source = DataGrid[3, i].Value + "";
                        if (source == "") continue;
                        if (Convert.ToBoolean(DataGrid.Rows[i].Cells[0].Value) == false) continue;
                        if (File.Exists(source + "\\" + name) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                            continue;
                        }
                        if (Directory.Exists(destination) != true)
                        {
                            string answer = Convert.ToString(MessageBox.Show("Destination directory at row #" + (i + 1) + " does not exist, press OK to create", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question));
                            if (answer == "OK")
                            {
                                Directory.CreateDirectory(destination);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        char[] temp = destination.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            destination += "\\";
                        }
                        temp = source.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            source += "\\";
                        }
                        FileInfo file = new FileInfo(source + name);
                        if (newName == "")
                        {
                            file.CopyTo(destination + name, true);
                        }
                        else
                        {
                            file.CopyTo(destination + newName, true);
                        }
                    }
                    else if (Convert.ToString(DataGrid[1, i].Value) == "Cut & Paste")
                    {
                        String newName = DataGrid[5, i].Value + "";
                        String destination = DataGrid[6, i].Value + "";
                        String name = DataGrid[2, i].Value + "";
                        String source = DataGrid[3, i].Value + "";
                        if (source == "") continue;
                        if (Convert.ToBoolean(DataGrid.Rows[i].Cells[0].Value) == false) continue;
                        if (File.Exists(source + "\\" + name) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                            continue;
                        }
                        if (Directory.Exists(destination) != true)
                        {
                            string answer = Convert.ToString(MessageBox.Show("Destination directory at row #" + (i + 1) + " does not exist, press OK to create", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question));
                            if (answer == "OK")
                            {
                                Directory.CreateDirectory(destination);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        char[] temp = destination.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            destination += "\\";
                        }
                        temp = source.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            source += "\\";
                        }
                        FileInfo file = new FileInfo(source + name);
                        if (newName == "")
                        {
                            file.CopyTo(destination + name, true);
                        }
                        else
                        {
                            file.CopyTo(destination + newName, true);
                        }
                        file.Delete();
                    }
                    else if (Convert.ToString(DataGrid[1, i].Value) == "Delete")
                    {
                        String source = DataGrid[3, i].Value + "";
                        String name = DataGrid[2, i].Value + "";
                        if (File.Exists(source + "\\" + name) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                        }
                        else
                        {
                            File.Delete(source + "\\" + name);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < DataGrid.RowCount; i++)
                {
                    if (Convert.ToString(DataGrid[1, i].Value) == "Copy & Paste")
                    {
                        String newName = DataGrid[5, i].Value + "";
                        String destination = DataGrid[6, i].Value + "";
                        String name = DataGrid[2, i].Value + "";
                        String source = DataGrid[3, i].Value + "";
                        if (source == "") continue;
                        if (Convert.ToBoolean(DataGrid.Rows[i].Cells[0].Value) == false) continue;
                        if (Directory.Exists(source) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                            continue;
                        }
                        if (Directory.Exists(destination) != true)
                        {
                            string answer = Convert.ToString(MessageBox.Show("Destination directory does not exist, press OK to create", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question));
                            if (answer == "OK")
                            {
                                Directory.CreateDirectory(destination);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        char[] temp = destination.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            destination += "\\";
                        }
                        temp = source.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            source += "\\";
                        }
                        DirectoryInfo sourceDir = new DirectoryInfo(source);
                        DirectoryInfo destDir = new DirectoryInfo(destination);
                        string folderDest = destination;
                        DirectoryInfo dir = new DirectoryInfo(destination);
                        if (dir.Exists)
                        {
                            Directory.CreateDirectory(folderDest);
                            FileInfo[] files = sourceDir.GetFiles();
                            DirectoryInfo[] directories = sourceDir.GetDirectories();
                            foreach (FileInfo fileFolder in files)
                            {
                                fileFolder.CopyTo(folderDest + "\\" + fileFolder.Name, true);
                            }
                            foreach (DirectoryInfo directory in directories)
                            {
                                Directory.CreateDirectory(folderDest + "\\" + directory.Name);
                                FileInfo[] files2 = directory.GetFiles();
                                foreach (FileInfo fileFolder in files2)
                                {
                                    fileFolder.CopyTo(folderDest + "\\" + directory.Name + "\\" + fileFolder.Name, true);
                                }
                            }
                        }
                    }
                    else if (Convert.ToString(DataGrid[1, i].Value) == "Cut & Paste")
                    {
                        String newName = DataGrid[5, i].Value + "";
                        String destination = DataGrid[6, i].Value + "";
                        String name = DataGrid[2, i].Value + "";
                        String source = DataGrid[3, i].Value + "";
                        if (source == "") continue;
                        if (Convert.ToBoolean(DataGrid.Rows[i].Cells[0].Value) == false) continue;
                        if (Directory.Exists(source) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                            continue;
                        }
                        if (Directory.Exists(destination) != true)
                        {
                            string answer = Convert.ToString(MessageBox.Show("Destination directory does not exist, press OK to create", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question));
                            if (answer == "OK")
                            {
                                Directory.CreateDirectory(destination);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        char[] temp = destination.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            destination += "\\";
                        }
                        temp = source.ToCharArray();
                        if (temp[temp.Length - 1] != '\\')
                        {
                            source += "\\";
                        }
                        DirectoryInfo sourceDir = new DirectoryInfo(source);
                        DirectoryInfo destDir = new DirectoryInfo(destination);
                        string folderDest = destination;
                        DirectoryInfo dir = new DirectoryInfo(destination);
                        if (dir.Exists)
                        {
                            Directory.CreateDirectory(folderDest);
                            FileInfo[] files = sourceDir.GetFiles();
                            DirectoryInfo[] directories = sourceDir.GetDirectories();
                            foreach (FileInfo fileFolder in files)
                            {
                                fileFolder.CopyTo(folderDest + "\\" + fileFolder.Name, true);
                            }
                            foreach (DirectoryInfo directory in directories)
                            {
                                Directory.CreateDirectory(folderDest + "\\" + directory.Name);
                                FileInfo[] files2 = directory.GetFiles();
                                foreach (FileInfo fileFolder in files2)
                                {
                                    fileFolder.CopyTo(folderDest + "\\" + directory.Name + "\\" + fileFolder.Name, true);
                                }
                            }
                        }
                        sourceDir.Delete(true);
                    }
                    else if (Convert.ToString(DataGrid[1, i].Value) == "Delete")
                    {
                        String source = DataGrid[3, i].Value + "";
                        if (Directory.Exists(source) != true)
                        {
                            MessageBox.Show("Row #" + (i + 1) + " has an invalid source file", "Error");
                        }
                        else
                        {
                            Directory.Delete(source, true);
                        }
                    }
                }
            }
            MessageBox.Show("Files successfully processed", "Complete");
        }

        private void DataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;
            if (e.ColumnIndex == 3 || e.ColumnIndex == 6)
            {
                folderBrowserDialog1.SelectedPath = Convert.ToString(DataGrid[e.ColumnIndex, e.RowIndex].Value);
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) DataGrid[e.ColumnIndex, e.RowIndex].Value = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine(Convert.ToString(DataGrid[8, 0].Value));
            if (selectChecked == false)
            {
                foreach(DataGridViewRow row in DataGrid.Rows)
                {
                    DataGrid[0, row.Index].Value = true;
                }
                selectChecked = true;
            }
            else
            {
                foreach (DataGridViewRow row in DataGrid.Rows)
                {
                    DataGrid[0, row.Index].Value = false;
                }
                selectChecked = false;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string answer = Convert.ToString(MessageBox.Show("Are you sure you want to delete this file?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question));
            if (answer == "Yes")
            {
                File.Delete(sf + "\\" + SettingsBox.SelectedItem);
                SettingsBox.Items.Remove(SettingsBox.SelectedItem);
            }
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            string answer = Convert.ToString(MessageBox.Show("Are you sure you wana rename this file?", "Rename Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question));
            if (answer == "Yes")
            {
                foreach (DataGridViewRow row in DataGrid.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }
                if (SettingsBox.Text == "")
                {
                    MessageBox.Show("Settings file field is empty. Please add name.", "Error");
                }
                else
                {
                    string save = "";
                    for (int i = 0; i < DataGrid.RowCount; i++)
                    {
                        for (int j = 0; j < DataGrid.ColumnCount; j++)
                        {
                            if (j != 4 && j != 7) save += DataGrid[j, i].Value + ",";
                        }
                        save += "\n";
                    }
                    save += ">";
                    if (FolderButton.Checked == true) save += "f";
                    if (File.Exists(sf + "\\" + SettingsBox.Text))
                    {
                        File.Delete(sf + "\\" + SettingsBox.Text);
                    }
                    using (FileStream fs = File.Create(sf + "\\" + SettingsBox.Text))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(save);
                        fs.Write(info, 0, info.Length);
                    }

                    FileInfo[] sFiles = sf.GetFiles();
                    SettingsBox.Items.Clear();
                    foreach (FileInfo file in sFiles)
                    {
                        SettingsBox.Items.Add(file);
                    }
                }
                SettingsBox.Items.Clear();
                FileInfo[] dFiles = sf.GetFiles();
                foreach (FileInfo file in dFiles)
                {
                    if (file.Name != settings.ToString())
                    {
                        SettingsBox.Items.Add(file);
                    }
                    else
                    {
                        file.Delete();
                    }
                }
                SettingsBox.SelectedItem = settings;
            }
        }

        private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                System.Diagnostics.Process.Start(Convert.ToString(DataGrid[e.ColumnIndex - 1, e.RowIndex].Value));
            }
        }

        private void DataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int cnt = 0;
            foreach (DataGridViewRow row in DataGrid.Rows)
            {
                DataGrid.Rows[cnt].Cells[1].Style.Font = new Font(DataGrid.Font, FontStyle.Bold);
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Delete")
                {
                    Console.WriteLine(DataGrid.Rows[cnt].Cells[1].Value);
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.DarkRed;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.White;
                }
                else if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Copy & Paste")
                {
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.Yellow;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.Black;
                }
                else if (Convert.ToString(DataGrid.Rows[cnt].Cells[1].Value) == "Cut & Paste")
                {
                    DataGrid.Rows[cnt].Cells[1].Style.BackColor = Color.Pink;
                    DataGrid.Rows[cnt].Cells[1].Style.ForeColor = Color.Black;
                }
                cnt++;
            }
        }
    }
}