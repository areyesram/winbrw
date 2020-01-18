using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace winbrw
{
    public partial class Form1 : Form
    {
        private const string DummyNode = "<dummy>";

        public Form1()
        {
            InitializeComponent();
            treeView1.BeforeExpand += (o, e) =>
            {
                if (e.Node.Nodes.Count == 1 && e.Node.FirstNode.Text == DummyNode)
                {
                    e.Node.Nodes.Clear();
                    try
                    {
                        foreach (var dir in Directory.EnumerateDirectories(e.Node.Name))
                        {
                            var dirNode = e.Node.Nodes.Add(dir, Path.GetFileName(dir), "Folder", "Folder");
                            dirNode.Nodes.Add(DummyNode);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            treeView1.AfterSelect += (o, e) =>
            {
                if (e.Node.Name == "")
                    return;
                try
                {
                    listView1.Items.Clear();
                    var fileCount = 0;
                    var dirCount = 0;
                    var totalSize = 0L;
                    foreach (var entry in Directory.EnumerateFileSystemEntries(e.Node.Name))
                    {
                        var fileInfo = new FileInfo(entry);
                        var isDirectory = (fileInfo.Attributes & FileAttributes.Directory) != 0;
                        string imageIndex;
                        if (isDirectory)
                        {
                            imageIndex = "Folder";
                            dirCount++;
                        }
                        else
                        {
                            imageIndex = "File";
                            fileCount++;
                            totalSize += fileInfo.Length;
                        }
                        var item = listView1.Items.Add(entry, Path.GetFileName(entry), imageIndex);
                        var text = isDirectory ? "" : fileInfo.Length.ToString("N0");
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, text));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, fileInfo.CreationTime.ToString(CultureInfo.CurrentCulture)));
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, fileInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture)));
                    }
                    toolStripStatusLabel1.Text = string.Format("{0} directorios, {1} archivos ({2:N0} bytes)",
                        dirCount, fileCount, totalSize.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            var rootNode = treeView1.Nodes.Add("", "Mi PC", "PC", "PC");
            foreach (var drive in DriveInfo.GetDrives())
            {
                var imageKey = drive.DriveType.ToString();
                var driveNode = rootNode.Nodes.Add(drive.Name, drive.Name, imageKey, imageKey);
                driveNode.Nodes.Add(DummyNode);
            }
            rootNode.Expand();
        }
    }
}
