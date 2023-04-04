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
        //global vars :(

        //lists for entries
        List<DialogEntry> entries = new List<DialogEntry>();
        List<DialogEntry> allEntries = new List<DialogEntry>();

        //for adding new dirs and files
        private TreeNode subNode, parentNode;
        string path;
        private DialogEntry newEntry = new DialogEntry();

        //for editing files 
        private string globalTag;


        public Form1()
        {
            InitializeComponent();
            //treeView1.ShowNodeToolTips = true;
            treeView1.LabelEdit = true;
        }

        //de serialization and tree making
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
                //MessageBox.Show($"File Path: {filePath}");

                //treeView1.Nodes.Add(filePath);
                entries = DeSerializer(filePath);
                allEntries = entries;
                TreeMaker(entries);
            }
        }
        //displays the details of sllected tag ie node
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            globalTag = null;
            NametranslationList.Items.Clear();
            NameDescriptTextBox.Text = string.Empty;
            NamePathTextBox.Text = string.Empty;


            var selectedNode = e.Node;
            var tag = selectedNode.Tag;

            //setting the global tag as the tag thats been selected
            globalTag = (string)selectedNode.Tag;

            DialogEntry file = allEntries.FirstOrDefault(o => o.LocKey == (string)tag);

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

        //search bar 
        private void NameSearchButton_Click(object sender, EventArgs e)
        {
            NameSearchList.Items.Clear();
            // get the search string
            string searchBar = NameSearchBar.Text;

            //look for the given name in the entries list and add all that matches the searchBar string to the list
            List<DialogEntry> resoult = allEntries.FindAll(s => s.EntryName.Contains(searchBar));

            foreach(DialogEntry res in resoult)
            {
                string LocKey_ = res.LocKey;
                string Path_ = res.HierarchyPath + '-' + res.EntryName;
                string Debug_ = res.Translations.Debug;

                ListViewItem newItem = new ListViewItem(new[] { LocKey_, Path_, Debug_ });
                NameSearchList.Items.Add(newItem);
            }
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

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

        // creates new group
        private void newGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(parentNode != null)
            {
                var newNode = new TreeNode();
                //adding a sub node to parent node
                //MessageBox.Show(parentNode.Text + " it worked");
                newNode = parentNode.Nodes.Add("<hey :)>");
                newNode.BeginEdit();
            }
            parentNode = null;
        }

        //creates a sub group
        private void newSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(subNode != null)
            {
                var newNode = new TreeNode();
                //adding a subnode 
                //MessageBox.Show(subNode.Text + " it worked");
                newNode = subNode.Nodes.Add("<hey_sub :)>");
                newNode.BeginEdit();
            }
            subNode = null;
        }

        //deletes the sellected group
        private void deleteGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(subNode != null)
            {
                //deleting the current node i e subnode 
                //MessageBox.Show("deleting " + subNode.Text);
                subNode.Remove();
            }
            subNode = null;
        }

        //creates a file
        private void newEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (subNode != null)
            {
                //MessageBox.Show(path);

                //set the pre text 
                NamePathTextBox.Text = path;
                // setthe carrot
                NamePathTextBox.SelectionStart = path.Length;

                // make it edittabble
                NamePathTextBox.ReadOnly = false;

                //set focuses
                nameDetails.Focus();
                NamePathTextBox.Focus();
            }
        }

        //when enter key is pressed get sets the name of the file as well as the path 
        private void NamePathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                // get the textbox text and find the name of the file from it ie last dir
                string[] splited = NamePathTextBox.Text.Split("-");
                string name = splited[splited.Length - 1]; 

                //sets the path of the new entry
                DialogEntry tmp = new DialogEntry();
                Translations tmpT = new Translations();
                tmp.HierarchyPath = NamePathTextBox.Text;
                tmp.LocKey = Guid.NewGuid().ToString("N");
                tmp.EntryName = name;
                tmpT.Debug = "tmp";
                tmp.Translations = tmpT;
                newEntry = tmp;

                //set the global tag 
                globalTag = tmp.LocKey;

                //add the new file to the tree view
                var newNode = new TreeNode();
                newNode.Tag = tmp.LocKey;
                newNode.Text = tmp.EntryName;
                newNode.ImageIndex = 1;
                subNode.Nodes.Add(newNode);

                // add new entry to all entries list
                allEntries.Add(newEntry);

                NamePathTextBox.ReadOnly = true;
                //shifts focus to description box
                NameDescriptTextBox.Focus();
            }
        }

        private void NameDescriptTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {

            }
        }


        //updates the sellected files details with each key stroke
        //global tag connects file to the textbox soo be carefull and dont forget to set it to null before making any changes to the textbox
        private void NameDescriptTextBox_TextChanged(object sender, EventArgs e)
        {
            //set the tag in a local var
            string tag = globalTag;

            //search for the tag
            DialogEntry file = allEntries.FirstOrDefault(o => o.LocKey == tag);

            //check if the file exists
            if (file != null && tag != null)
            {
                //overwrite the debug
                file.Translations.Debug = NameDescriptTextBox.Text;

                //updating the list view this aproach creates 2 rows unitl the textbox updates
                NametranslationList.Items.Clear();
                string translationText = file.Translations.Debug;
                string translationLanguage = "Debug";
                ListViewItem newItem = new ListViewItem(new[] { translationLanguage, translationText });
                NametranslationList.Items.Add(newItem);
            }
            tag = null;
        }

        //selects the node thats clicked on and saves it to a global var named subNode 
        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (treeView1.GetNodeAt(e.Location) != null && treeView1.GetNodeAt(e.Location).Tag == null)
            {                

                // pre alocating node and sub node and a tmp node
                subNode = treeView1.GetNodeAt(e.Location);
                parentNode = treeView1.GetNodeAt(e.Location).Parent;
                TreeNode tmp = treeView1.GetNodeAt(e.Location);

                //get the path of the subNode
                List<string> pathList = new List<string>();
                while(tmp != null)
                {
                    //MessageBox.Show(tmp.Text);
                    pathList.Add(tmp.Text);
                    tmp = tmp.Parent;
                    
                }
                pathList.Reverse();
                path = string.Join("-", pathList) + "-";


                if(e.Button == MouseButtons.Right)
                {
                    //show menu strip
                    contextMenuStrip1.Show(e.Location);
                } 
            }
            else
            {
                subNode = null;
                parentNode = null;
                path = null;
            }
        }
    }
}
