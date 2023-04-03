using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Forams_LocManager
{
    public class DialogEntry
    {
        public string LocKey { get; set; }
        public string HierarchyPath { get; set; }
        public string EntryName { get; set; }
        public Translations Translations { get; set; }

        //public DialogEntry(string locKey, string hierarchyPath, string entryName, string text)
        //{
        //    LocKey = locKey;
        //    HierarchyPath = hierarchyPath;
        //    EntryName = entryName;
        //    Translations = new Translations(text);
        //}
    }


    public class Translations
    {
        public string Debug { get; set; }

        //public Translations(string txt)
        //{
        //    Debug = txt;
        //}
    }

}
