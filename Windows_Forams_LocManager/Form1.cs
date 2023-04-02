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
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.Text.Json;


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
                        MessageBox.Show($"File Path: {diagEntry.HierarchyPath}, ");
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

                    // Set the tag of the node to the LocKey property of the corresponding DialogEntry
                    childNode.Tag = path.LocKey;

                    // Update the current node to be the newly created or existing child node
                    currentNode = childNode;
                }

                // If the last directory in the path is not already a node, add it
                if (currentNode.Text != directories.Last())
                {
                    var childNode = new TreeNode(directories.Last());
                    childNode.Tag = path.LocKey;
                    currentNode.Nodes.Add(childNode);
                }
            }

            // Add the root node to the TreeView
            treeView1.Nodes.Add(rootNode);
        }



    }
}
