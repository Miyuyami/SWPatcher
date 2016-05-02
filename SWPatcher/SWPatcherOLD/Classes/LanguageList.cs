using System;
using System.Collections.Generic;

namespace SWPatcher.Classes
{
    class LanguageList : System.Windows.Forms.ContextMenuStrip
    {
        List<string> lItems;
        int iCurrentSelected;

        public LanguageList()
        {
            this.lItems = new List<string>();
            this.iCurrentSelected = 0;
        }

        public int SelectedIndex
        {
            get { return this.iCurrentSelected; }
            set 
            {
                this.iCurrentSelected = value;
                if (value < this.Items.Count && 0 <= value)
                {
                    //OnItemClicked(new System.Windows.Forms.ToolStripItemClickedEventArgs(this.Items[value]));
                }
            }
        }

        public void Add(string ItemName)
        {
            this.lItems.Add(ItemName);
            var currentNode = new System.Windows.Forms.ToolStripMenuItem() { Text = ItemName };
            currentNode.Click += new EventHandler(OnChildrenSelecting);
            this.Items.Add(currentNode);
            currentNode = null;
        }

        public void Insert(int Index,string ItemName)
        {
            this.lItems.Insert(Index, ItemName);
            var currentNode = new System.Windows.Forms.ToolStripMenuItem() { Text = ItemName };
            currentNode.Click += new EventHandler(OnChildrenSelecting);
            this.Items.Insert(Index, currentNode);
            currentNode = null;
        }

        public void Remove(string ItemName)
        {
            for (int cou = 0; cou < this.Items.Count; cou++)
                if (this.Items[0].Text == ItemName)
                    this.Items.RemoveAt(cou);
        }

        public string SelectedItem
        {
            get 
            {
                if (this.lItems.Count > 0)
                    return this.lItems[iCurrentSelected];
                else
                    return string.Empty;
            }
            set
            {
                for (int cou = 0; cou < this.Items.Count; cou++)
                    if (this.Items[cou].Text == value)
                    {
                        this.iCurrentSelected = cou;
                        OnItemClicked(new System.Windows.Forms.ToolStripItemClickedEventArgs(this.Items[cou]));
                    }
            }
        }

        public void RemoveAt(int Index)
        {
            this.Items.RemoveAt(Index);
        }

        public string GetItemAt(int Index)
        {
            if (this.lItems.Count > 0)
                return this.lItems[Index];
            else
                return string.Empty;
        }

        private void OnChildrenSelecting(object sender, EventArgs e)
        {
            this.iCurrentSelected = this.Items.IndexOf((System.Windows.Forms.ToolStripMenuItem)sender);
        }
    }
}
