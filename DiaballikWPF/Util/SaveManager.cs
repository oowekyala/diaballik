using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Diaballik.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using static Diaballik.Core.Util.SerializationUtil;

namespace DiaballikWPF.Util {
    using GameId = String;

    /// <summary>
    ///     Responsible for managing saves of the game. <see cref="SaveEntry"/>
    /// </summary>
    public class SaveManager : ViewModelBase {
        /// Folder in which we save games. We use an improbable folder name to eliminate collisions
        private static readonly string SavesFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Diaballik-QB-CF-XX234224");

        /// Manifest file name
        private static readonly string ManifestFile = Path.Combine(SavesFolder, "manifest.xml");


        public SaveManager(IMessenger messenger) {
            MessengerInstance = messenger;

            Messages.SaveGameMessage.Register(MessengerInstance, this, tuple => {
                var (id, memento) = tuple;
                RegisterForSave(id, memento);
            });

            Messages.DeleteSaveInManagerMessage.Register(MessengerInstance, this, id => Saves.Remove(id));
        }


        #region Saves dictionary

        private Dictionary<GameId, SaveEntry> _saves;

        /// Contains all saves we know about.
        private Dictionary<GameId, SaveEntry> Saves => _saves ?? (_saves = LoadManifest());

        /// Parse metadata from available saved games into a dictionary.
        private Dictionary<GameId, SaveEntry> LoadManifest() {
            var dic = new Dictionary<GameId, SaveEntry>();
            if (File.Exists(ManifestFile)) {
                try {
                    var text = File.ReadAllText(ManifestFile);
                    var doc = XDocument.Parse(text);
                    var metadata = doc.Root
                                      .Elements("metadata")
                                      .Select(GameMetadataBundle.FromElement);


                    foreach (var bundle in metadata) {
                        dic.Add(bundle.Id, new SaveEntry(bundle, true));
                    }
                } catch (XmlException e) {
                    Debug.WriteLine("Failed to load manifest: " + e.Message);
                }
            }
            return dic;
        }

        #endregion


        /// Get an id to assign to a game
        public GameId NextId() => Path.GetRandomFileName();

        /// Gets all the currently saved games.
        public IEnumerable<SaveEntry> AllSaves() => Saves.Values;


        /// Registers a memento to save on disk. 
        /// Overwrites previous saves that used the same id.
        public void RegisterForSave(GameId id, GameMemento memento) {
            var exists = Saves.TryGetValue(id, out var entry);

            if (exists) {
                entry.Memento = memento;
            } else {
                var metadata = new GameMetadataBundle(id);
                Saves.Add(id, new SaveEntry(metadata, memento));
            }
        }

        /// <summary>
        ///     Writes a manifest file containing metadata about all the saves we currently know about.
        /// </summary>
        private void WriteManifest() {
            Directory.CreateDirectory(SavesFolder);
            var doc = new XDocument(new XElement("saves"));
            foreach (var entry in Saves.Values) {
                doc.Root.Add(entry.Metadata.ToElement());
            }
            doc.Save(ManifestFile);
        }

        /// <summary>
        ///     Writes saves that need to be updated to disk
        /// </summary>
        public void CommitSaves() {
            WriteManifest();

            //write only saves that must be written
            foreach (var save in Saves.Values.Where(t => !t.IsValid)) {
                save.WriteMemento();
            }
        }

        /// <summary>
        ///     Represents a saved game. Games have an identity (represented by an id), which
        ///     is preserved throughout the load / save cycle and until the deletion of the save.
        ///     
        ///     Forking a game creates a game with a new id. Playing actions on a game applies the
        ///     changes with the same id, thus overwriting previous saves.
        /// 
        ///     Each memento is contained in its own file. One manifest file gathers metadata for all
        ///     known saves. That manifest is lazy loaded and its metadata can be displayed in the load 
        ///     screen's itemscontrol. When a save is selected, the corresponding memento file is fetched
        ///     and parsed, and sent to the game view. Laziness everywhere keeps disk operations at a minimum.
        /// </summary>
        public class SaveEntry {
            private GameMemento _memento;

            /// <summary>
            ///     Use Metadata.LatestState to avoid loading the entire memento.
            /// </summary>
            public IReadOnlyGameMetadata Metadata => _metadata;

            private readonly GameMetadataBundle _metadata;
            public bool IsValid { get; private set; }

            /// <summary>
            ///     Lazy loaded from disk if the entry is valid.
            ///     Setting it invalidates the entry, which means it will be saved on the next CommitSaves() call.
            ///     The setter updates the metadata too.
            /// </summary>
            public GameMemento Memento {
                get {
                    if (_memento == null && IsValid) {
                        _memento = ReadMementoFromFile();
                    } else if (_memento != null) {
                        return _memento;
                    } else {
                        throw new ArgumentException("Cannot find the memento!");
                    }

                    return _memento;
                }
                set {
                    if (value != null) {
                        _memento = value;
                        IsValid = false;
                        _metadata.Update(_memento);
                    } else {
                        throw new ArgumentException("Cannot use null as a value");
                    }
                }
            }


            public SaveEntry(GameMetadataBundle bundle, bool isValid) {
                _metadata = bundle;
                IsValid = isValid;
            }

            public SaveEntry(GameMetadataBundle bundle, GameMemento memento) {
                _metadata = bundle;
                Memento = memento;
                IsValid = false;
            }

            private GameMemento ReadMementoFromFile() {
                var text = File.ReadAllText(FilenameForId(Metadata.Id));
                var doc = XDocument.Parse(text);
                return Deserializer.MementoFromElement(doc.Root);
            }

            /// <summary>
            ///     Writes the currently held memento to disk.
            ///     Should be avoided if the entry is still valid.
            /// </summary>
            public void WriteMemento() {
                var id = Metadata.Id;
                Directory.CreateDirectory(SavesFolder);
                var mementoDoc = Serializer.MementoToElement(Memento);
                mementoDoc.Save(FilenameForId(id));
            }

            private static string FilenameForId(GameId id) {
                return Path.Combine(SavesFolder, id);
            }
        }
    }

    /// <summary>
    ///     Interface used to expose a metadata bundle without authorizing updates.
    /// </summary>
    public interface IReadOnlyGameMetadata {
        GameId Id { get; }
        DateTime SaveDate { get; }
        Player Player1 { get; }
        Player Player2 { get; }
        Player VictoriousPlayer { get; }
        bool IsVictory { get; }
        GameState LatestState { get; }
        Player CurrentPlayer { get; }

        /// <summary>
        ///     Serializes this metadata bundle to XML.
        /// </summary>
        /// <returns>An xml element</returns>
        XElement ToElement();
    }

    /// <summary>
    ///     Metadata describing a game, used for quick display in the load view.
    /// </summary>
    public class GameMetadataBundle : IReadOnlyGameMetadata {
        /// <summary>
        ///     Unique id, used to identify the game through its lifetime and the file it lives in.
        /// </summary>
        public GameId Id { get; }

        public DateTime SaveDate { get; private set; }

        public Player Player1 => LatestState.Player1;
        public Player Player2 => LatestState.Player2;

        public bool IsVictory => LatestState.IsVictory;
        public Player VictoriousPlayer => LatestState.VictoriousPlayer;

        public Player CurrentPlayer => LatestState.CurrentPlayer;
        public GameState LatestState { get; private set; }

        public GameMetadataBundle(string id) {
            Id = id;
        }

        /// Update the metadata to describe the given memento.
        public void Update(GameMemento memento) {
            SaveDate = DateTime.Now;
            LatestState = memento.State;
        }

        /// Parses an element into a metadata bundle
        public static GameMetadataBundle FromElement(XElement element) {
            var id = (string) element.Element("gameID");
            var date = (DateTime) element.Element("saveDate");

            return new GameMetadataBundle(id) {
                SaveDate = date,
                LatestState = Deserializer.StateFromElement(element.Element(StateElementName))
            };
        }

        /// Converts this bundle to an XML element.
        public XElement ToElement() {
            return new XElement("metadata",
                                new XElement("gameID", Id),
                                new XElement("saveDate", SaveDate),
                                Serializer.StateToElement(LatestState));
        }
    }
}