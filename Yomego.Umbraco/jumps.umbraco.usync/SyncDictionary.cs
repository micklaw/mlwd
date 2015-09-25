﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using jumps.umbraco.usync.helpers;
using umbraco.cms.businesslogic;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Strings;
#pragma warning disable 618

namespace jumps.umbraco.usync
{
    public class SyncDictionary
    {
        public static void SaveToDisk(Dictionary.DictionaryItem item)
        {
            if (item != null)
            {
                DefaultShortStringHelper _sh = new DefaultShortStringHelper();

                XmlDocument xmlDoc = XmlDoc.CreateDoc();
                xmlDoc.AppendChild(item.ToXml(xmlDoc));
                xmlDoc.AddMD5Hash();
                
                XmlDoc.SaveXmlDoc("Dictionary", 
                    _sh.CleanString( item.key, CleanStringType.Ascii),
                    xmlDoc);
            }
        }

        public static void SaveAllToDisk()
        {
            LogHelper.Debug<SyncDictionary>("Saving Dictionary Types");

            foreach (Dictionary.DictionaryItem item in Dictionary.getTopMostItems)
            {
                LogHelper.Debug<SyncDictionary>("Dictionary Item {0}", ()=> item.key);
                SaveToDisk(item);

                
            }
        }

        public static void ReadAllFromDisk()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string path = IOHelper.MapPath(string.Format("{0}{1}",
                uSyncIO.RootFolder,
                "Dictionary"));

            ReadFromDisk(path);

            sw.Stop();
            LogHelper.Info<uSync>("Processed Dictionary items ({0}ms)", () => sw.ElapsedMilliseconds);

        }

        public static void ReadFromDisk(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path, "*.config"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file);

                    XmlNode node = xmlDoc.SelectSingleNode("./DictionaryItem");

                    if (node != null)
                    {
                        LogHelper.Debug<SyncDictionary>("Node Import: {0} {1}", 
                            ()=> node.Attributes["Key"].Value, ()=> node.InnerXml);

                        try
                        {

                            Dictionary.DictionaryItem item = Dictionary.DictionaryItem.Import(node);

                            if (item != null)
                                item.Save();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Debug<SyncDictionary>("DictionaryItem.Import Failed {0}: {1}", 
                                ()=> path, ()=> ex.ToString());
                        }
                    }
                }
            }
            
        }

        public static void AttachEvents()
        {
            Dictionary.DictionaryItem.Saving += DictionaryItem_Saving;
            Dictionary.DictionaryItem.Deleting += DictionaryItem_Deleting;
        }

        static object _deleteLock = new object();
        static ArrayList _dChildren = new ArrayList(); 


        static void DictionaryItem_Deleting(Dictionary.DictionaryItem sender, EventArgs e)
        {
            if (!uSync.EventsPaused)
            {
                lock (_deleteLock)
                {
                    if (sender.hasChildren)
                    {
                        // we get the delets in a backwards order, so we add all the children of this
                        // node to the list we are not going to delete when we get asked to.
                        // 
                        foreach (Dictionary.DictionaryItem child in sender.Children)
                        {
                            _dChildren.Add(child.id);
                        }
                    }

                    if (_dChildren.Contains(sender.id))
                    {
                        // this is a child of a parent we have already deleted.
                        _dChildren.Remove(sender.id);
                        LogHelper.Debug<SyncDictionary>("No Deleteing Dictionary item {0} because we deleted it's parent",
                            () => sender.key);
                    }
                    else
                    {
                        //actually delete 


                        LogHelper.Debug<SyncDictionary>("Deleting Dictionary Item {0}", () => sender.key);

                        // when you delete a tree, the top gets called before the children. 
                        //             
                        if (!sender.IsTopMostItem())
                        {
                            // if it's not top most, we save it's parent (that will delete)

                            SaveToDisk(GetTop(sender));
                        }
                        else
                        {
                            // it's top we need to delete
                            XmlDoc.ArchiveFile("Dictionary", sender.key);

                        }
                    }
                }

            }            
            
        }


        static void DictionaryItem_Saving(Dictionary.DictionaryItem sender, EventArgs e)
        {
            if (!uSync.EventsPaused)
            {
                SaveToDisk(GetTop(sender));
            }
        }

        private static Dictionary.DictionaryItem GetTop(Dictionary.DictionaryItem item)
        {
            if (!item.IsTopMostItem())
            {
                LogHelper.Debug<SyncDictionary>("is Top Most [{0}]", () => item.IsTopMostItem());
                try
                {
                    if (item.Parent != null)
                    {
                        LogHelper.Debug<SyncDictionary>("parent [{0}]", () => item.Parent.key);
                        return GetTop(item.Parent);
                    }
                }
                catch (ApplicationException)
                {
                    LogHelper.Debug<SyncDictionary>("Exception (just like null)");
                }
                catch (ArgumentException)
                {
                    LogHelper.Debug<SyncDictionary>("Exception (just like null)");
                }

            }

            return item;
        }
    }
}
