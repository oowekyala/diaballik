using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Diaballik.Core;
using Diaballik.Core.Util;

namespace DiaballikWPF.Util {
    using GameId = String;
    using DecoratedMemento = ValueTuple<GameMetadataBundle, GameMemento>;

    public class SaveManager {
        public static SaveManager Instance { get; } = new SaveManager();

        private SaveManager() {
        }


        private const Environment.SpecialFolder SaveFolderContainer = Environment.SpecialFolder.LocalApplicationData;

        private string _savesFolder;

        /// Folder in which we save games.
        private string SavesFolder {
            get {
                if (_savesFolder == null) {
                    var appDataPath = Environment.GetFolderPath(SaveFolderContainer);
                    // we use an improbable folder to eliminate collisions
                    _savesFolder = Path.Combine(appDataPath, "Diaballik-QB-CF-XX234224");
                }
                return _savesFolder;
            }
        }

        /// Get an id to assign to a game
        public GameId NextId() => Path.GetRandomFileName();

        /// Gets all the saves that could be found.
        public IEnumerable<DecoratedMemento> AllSaves() {
            return Directory.Exists(SavesFolder)
                ? Directory.EnumerateFiles(SavesFolder).Select(LoadSave)
                : Enumerable.Empty<DecoratedMemento>();
        }


        private string FilenameForId(GameId id) {
            return Path.Combine(SavesFolder, id);
        }

        /// Save a memento to disk.
        /// Overwrites previous saves that used the same id.
        public void Save(GameId id, GameMemento memento) {
            Directory.CreateDirectory(SavesFolder);

            var mementoDoc = new MementoSerializationUtil.Serializer().ToXml(memento);
            new GameMetadataBundle(id, DateTime.Now).AppendToDocument(mementoDoc);

            mementoDoc.Save(new XmlTextWriter(FilenameForId(id), Encoding.UTF8));
        }

        private DecoratedMemento LoadSave(string filename) {
            var text = File.ReadAllText(filename);
            var doc = XDocument.Parse(text);
            var memento = new MementoSerializationUtil.Deserializer().FromDocument(doc);
            return (GameMetadataBundle.FromDocument(doc), memento);
        }
    }

    /// <summary>
    ///     Metadata appended to the save file.
    /// </summary>
    public class GameMetadataBundle {
        public GameId Id { get; }
        public DateTime SaveDate { get; }

        public GameMetadataBundle(string id, DateTime saveDate) {
            Id = id;
            SaveDate = saveDate;
        }

        public static GameMetadataBundle FromDocument(XDocument doc) {
            var element = doc.Root.Element("metadata");
            var id = element.Element("gameID").Value;
            var date = DateTime.Parse(element.Element("saveDate").Value);
            return new GameMetadataBundle(id, date);
        }

        public void AppendToDocument(XDocument doc) {
            doc.Root.Add(new XElement("metadata",
                                      new XElement("gameID", new XText(Id)),
                                      new XElement("saveDate", new XText(SaveDate.ToLongDateString()))));
        }
    }
}