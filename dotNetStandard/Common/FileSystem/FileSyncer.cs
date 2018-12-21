using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace XJK.FileSystem
{
    public enum SyncDirectrion
    {
        None,
        Sync,
        ObjectToFile,
        FileToObject,
    }
    
    public class FileSyncer<T> : IObjectReference<T> where T : INotifyPropertyChanged, new()
    {
        public string FilePath
        {
            get
            {
                return Path.Combine(Watcher.Path, Watcher.Filter);
            }
            set
            {
                var dir = Path.GetDirectoryName(value);
                if (!Directory.Exists(dir)) throw new DirectoryNotFoundException(dir);
                var file = Path.GetFileName(value);
                Watcher.Path = dir;
                Watcher.Filter = file;
            }
        }
        public SyncDirectrion Directrion { get; set; } = SyncDirectrion.Sync;
        public bool IsObjectToFile => Directrion == SyncDirectrion.Sync || Directrion == SyncDirectrion.ObjectToFile;
        public bool IsFileToObject => Directrion == SyncDirectrion.Sync || Directrion == SyncDirectrion.FileToObject;
        public int LagSaveFileMilliseconds { get; set; } = 0;
        public int LagParseObjectMilliseconds { get; set; } = 0;
        public event ObjectChangedHandler ObjectChanged;
        /// <summary>
        /// return propertyName = null if reset Object
        /// </summary>
        public event PropertyChangedEventHandler ObjectPropertyChanged;

        public FileSyncer(string FilePath, IObjectFileConverter objectFileConverter)
        {
            Converter = objectFileConverter;
            Watcher = FileWatcher.WatchFileContent(FilePath);
            Watcher.Changed += Watcher_Changed;
            if (File.Exists(FilePath))
            {
                ForceParseObject();
            }
            else
            {
                SetObject(new T());
            }
        }

        private Timer LagSaveFileTimer;
        private Timer LagParseObjectTimer;
        private IObjectFileConverter Converter;
        private FileSystemWatcher Watcher;
        private T _obj;

        public T GetObject() => _obj;
        public void SetObject(T value)
        {
            if (!ReferenceEquals(_obj, value))
            {
                object old = _obj;
                if (_obj != null)
                {
                    _obj.PropertyChanged -= _obj_PropertyChanged;
                }
                _obj = value;
                if (_obj != null)
                {
                    _obj.PropertyChanged += _obj_PropertyChanged;
                }
                ObjectChanged?.Invoke(this, new ObjectChangedEventArgs(old));
                ObjectPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void _obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsObjectToFile)
            {
                if (LagSaveFileMilliseconds <= 0)
                {
                    ForceSaveFile();
                }
                else
                {
                    if (LagSaveFileTimer == null)
                    {
                        LagSaveFileTimer = new Timer();
                        LagSaveFileTimer.Elapsed += LagSaveFileTimer_Elapsed;
                    }
                    LagSaveFileTimer.Interval = LagSaveFileMilliseconds;
                    LagSaveFileTimer.Enabled = true;
                }
            }
            ObjectPropertyChanged?.Invoke(sender, e);
        }

        private void LagSaveFileTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ForceSaveFile();
            LagSaveFileTimer.Enabled = false;
        }
        
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (IsFileToObject)
            {
                if (LagParseObjectMilliseconds <= 0)
                {
                    ForceParseObject();
                }
                else
                {
                    if (LagParseObjectTimer == null)
                    {
                        LagParseObjectTimer = new Timer();
                        LagParseObjectTimer.Elapsed += LagParseObjectTimer_Elapsed;
                    }
                    LagParseObjectTimer.Interval = LagParseObjectMilliseconds;
                    LagParseObjectTimer.Enabled = true;
                }
            }
        }

        private void LagParseObjectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ForceParseObject();
            LagParseObjectTimer.Enabled = false;
        }
        
        public void ForceSaveFile()
        {
            Watcher.EnableRaisingEvents = false;
            Converter.Convert(_obj, FilePath);
            Watcher.EnableRaisingEvents = true;
        }

        public void ForceParseObject()
        {
            SetObject(Converter.ConvertBack<T>(FilePath));
        }

        public static FileSyncer<T> XmlSyncer(string path)
        {
            return new FileSyncer<T>(path, new ObjectXmlConverter());
        }
    }
}
