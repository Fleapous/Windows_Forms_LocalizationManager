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


namespace Windows_Forams_LocManager
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Filter = "Zip files (*.zip)|*.zip";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                MessageBox.Show($"File Path: {filePath}");

                treeView1.Nodes.Add(filePath);


                using (ZipArchive archive = ZipFile.OpenRead(filePath))
                {
                    treeView1.Nodes.Clear(); // Clear the tree view before adding new entries

                    // Add the archive's entries to the tree view
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string[] pathParts = entry.FullName.Split('/');
                        TreeNode currentNode = null;

                        // Traverse the path and create tree nodes as needed
                        for (int i = 0; i < pathParts.Length; i++)
                        {
                            string pathPart = pathParts[i];
                            if (currentNode == null)
                            {
                                currentNode = treeView1.Nodes.Add(pathPart);
                            }
                            else
                            {
                                TreeNode[] nodes = currentNode.Nodes.Find(pathPart, false);
                                if (nodes.Length == 0)
                                {
                                    currentNode = currentNode.Nodes.Add(pathPart);
                                }
                                else
                                {
                                    currentNode = nodes[0];
                                }
                            }
                        }

                        using (Stream stream = entry.Open())
                        {
                            StreamReader reader = new StreamReader(stream);
                            string fileContents = reader.ReadToEnd();
                            JObject jsonObject = JObject.Parse(fileContents);
                            TreeNode fileNode = currentNode.Nodes.Add(entry.Name);

                            foreach (JProperty property in jsonObject.Properties())
                            {
                                TreeNode propertyNode = fileNode.Nodes.Add(property.Name);
                                propertyNode.Tag = property.Value.ToString();
                            }
                        }
                    }
                }
            }
        }
    }
}
