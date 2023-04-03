using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;


namespace Windows_Forams_LocManager
{


    public partial class Form1 : Form
    {
        List<DialogEntry> entries = new List<DialogEntry>();

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            entries.Clear();

            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Filter = "Zip files (*.zip)|*.zip";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                MessageBox.Show($"File Path: {filePath}");

                //treeView1.Nodes.Add(filePath);
                entries = DeSerializer(filePath);
                TreeMaker(entries);
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            NametranslationList.Items.Clear();
            var selectedNode = e.Node;
            var tag = selectedNode.Tag;
            DialogEntry file = entries.FirstOrDefault(o => o.LocKey == (string)tag);

            if (file == null)
            {

            }
            else
            {
                //setting textBox fields
                NamePathTextBox.Text = file.HierarchyPath;
                NameDescriptTextBox.Text = file.Translations.Debug;

                //setting list items
                string translationText = file.Translations.Debug;
                string translationLanguage = "Debug";
                ListViewItem newItem = new ListViewItem(new[] { translationLanguage, translationText });
                NametranslationList.Items.Add(newItem);
            }
        }
        private void NameSearchButton_Click(object sender, EventArgs e)
        {
            NameSearchList.Items.Clear();
            // get the search string
            string searchBar = NameSearchBar.Text;

            //look for the given name in the entries list and add all that matches the searchBar string to the list
            List<DialogEntry> resoult = entries.FindAll(s => s.EntryName.Contains(searchBar));

            foreach(DialogEntry res in resoult)
            {
                string LocKey_ = res.LocKey;
                string Path_ = res.HierarchyPath + '-' + res.EntryName;
                string Debug_ = res.Translations.Debug;

                ListViewItem newItem = new ListViewItem(new[] { LocKey_, Path_, Debug_ });
                NameSearchList.Items.Add(newItem);
            }
        }


        private List<DialogEntry> DeSerializer(string filepath)
        {

            List<DialogEntry> res = new List<DialogEntry>();
            using (ZipArchive archive = ZipFile.OpenRead(filepath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //de serialize
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        string filestring = reader.ReadToEnd();
                        DialogEntry diagEntry = JsonSerializer.Deserialize<DialogEntry>(filestring);
                        //MessageBox.Show($"File Path: {diagEntry.HierarchyPath}, ");
                        res.Add(diagEntry);
                    }
                }
            }
            return res;
        }

        private void TreeMaker(List<DialogEntry> diags)
        {
            // Create the root node
            var rootNode = new TreeNode("Root");

            // Loop through each path
            foreach (var path in diags)
            {
                // Split the path into its directory components
                var directories = path.HierarchyPath.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                // Initialize the current node to the root node
                var currentNode = rootNode;

                // Loop through each directory in the path
                foreach (var directory in directories)
                {
                    // Check if the current node already has a child with the same name as the directory
                    var childNode = currentNode.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == directory);

                    // If there is no child with the same name, create a new one
                    if (childNode == null)
                    {
                        childNode = new TreeNode(directory);
                        currentNode.Nodes.Add(childNode);
                    }

                    // Update the current node to be the newly created or existing child node
                    currentNode = childNode;
                }

                // Add the entry as a node to the final directory
                var entryNode = new TreeNode(path.EntryName);
                entryNode.Tag = path.LocKey;
                entryNode.ImageIndex = 1;
                currentNode.Nodes.Add(entryNode);
            }

            // Add the root node to the TreeView
            treeView1.Nodes.Add(rootNode);
        }


    }
}
