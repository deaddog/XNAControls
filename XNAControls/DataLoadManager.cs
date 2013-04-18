using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public class DataLoadManager<T>
    {
        private object loaderCountObj = new object();
        private int loaderCount = 0;
        private int imagesPerThread = 3;
        public int ImagesPerThread
        {
            get { return imagesPerThread; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                imagesPerThread = value;
            }
        }

        private GraphicsDevice device;
        private Dictionary<T, ImageState> stateDict;
        private Dictionary<T, Texture2D> textureDict;

        private Queue<T> loadList;

        private TextureCollection textures;
        private StateCollection states;

        public TextureCollection Textures
        {
            get { return textures; }
        }
        public StateCollection States
        {
            get { return states; }
        }

        public DataLoadManager(GraphicsDevice device, Func<T, System.Drawing.Image> loaderMethod)
        {
            this.device = device;

            stateDict = new Dictionary<T, ImageState>();
            textureDict = new Dictionary<T, Texture2D>();

            this.textures = new TextureCollection(this);
            this.states = new StateCollection(this);
            this.loadList = new Queue<T>();

            this.allowKeyMethod = defaultAllowKeyMethod;
            this.loaderMethod = loaderMethod;
        }

        private void LoadImageAsync()
        {
            T key = default(T);
            bool noKey;

            do
            {
                noKey = false;
                lock (loadList)
                {
                    if (loadList.Count == 0)
                        noKey = true;
                    else
                        key = loadList.Dequeue();
                }
                if (noKey)
                    break;

                using (System.Drawing.Image image = loaderMethod(key))
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
#if XNA3
                        Texture2D texture = Texture2D.FromFile(device, ms);
#else
                    Texture2D texture = Texture2D.FromStream(device, ms);
#endif
                    lock (textureDict)
                        textureDict.Add(key, texture);
                    stateDict[key] = ImageState.LoadSuccess;
                }
            }
            while (loadList.Count > 0);
            lock (loaderCountObj) loaderCount--;
        }
        private void LoadImage(T key)
        {
            int count;
            lock (loadList)
            {
                if (!loadList.Contains(key))
                    loadList.Enqueue(key);
                count = loadList.Count;
            }

            int neededThreads = (int)Math.Ceiling((double)count / (double)imagesPerThread);

            lock (loaderCountObj)
                while (loaderCount < neededThreads)
                {
                    loaderCount++;
                    System.Threading.Thread loadThread = new System.Threading.Thread(LoadImageAsync);
                    loadThread.Start();
                }
        }

        private Func<T, bool> allowKeyMethod;
        private bool defaultAllowKeyMethod(T key)
        {
            return true;
        }
        public Func<T, bool> AllowKeyMethod
        {
            get { return allowKeyMethod == defaultAllowKeyMethod ? null : allowKeyMethod; }
            set { allowKeyMethod = (value ?? allowKeyMethod); }
        }

        private Func<T, System.Drawing.Image> loaderMethod;

        public class TextureCollection
        {
            private DataLoadManager<T> owner;
            public TextureCollection(DataLoadManager<T> owner)
            {
                this.owner = owner;
            }
            public Texture2D this[T key]
            {
                get
                {
                    if (!owner.allowKeyMethod(key))
                        return null;

                    Texture2D texture;
                    lock (owner.textureDict)
                        if (!owner.textureDict.TryGetValue(key, out texture))
                            texture = null;
                    return texture;
                }
            }
        }
        public class StateCollection
        {
            private DataLoadManager<T> owner;
            public StateCollection(DataLoadManager<T> owner)
            {
                this.owner = owner;
            }
            public ImageState this[T key]
            {
                get
                {
                    if (!owner.allowKeyMethod(key))
                        return ImageState.Unknown;

                    ImageState state;
                    if (!owner.stateDict.TryGetValue(key, out state))
                    {
                        state = ImageState.Loading;
                        owner.stateDict.Add(key, state);
                        owner.LoadImage(key);
                    }
                    return state;
                }
            }
        }

        public enum ImageState
        {
            Unknown = 0,
            Loading = 1,
            LoadedComplete = 2,
            LoadSuccess = 6,
            LoadError = 10
        }
    }
}
