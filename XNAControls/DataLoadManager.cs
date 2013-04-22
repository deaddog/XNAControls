using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public abstract class DataLoadManager<TKey, TValue>
    {
        /// <summary>
        /// The number of threads currently running. This should only be modified when <see cref="loadlist"/> is locked.
        /// </summary>
        private int loaderCount = 0;

        private int itemsPerThread = 3;
        public int ItemsPerThread
        {
            get { return itemsPerThread; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                itemsPerThread = value;
            }
        }

        private Dictionary<TKey, DataLoader<TValue>> dictionary;

        private Queue<TKey> loadList;

        public DataLoadManager()
        {
            this.dictionary = new Dictionary<TKey, DataLoader<TValue>>();

            this.loadList = new Queue<TKey>();

            this.allowKeyMethod = defaultAllowKeyMethod;
        }

        private void DataLoadMethod()
        {
            TKey key = default(TKey);

            do
            {
                lock (loadList)
                {
                    if (loadList.Count == 0)
                    {
                        loaderCount--;
                        return;
                    }
                    else
                        key = loadList.Dequeue();
                }

                DataLoader<TValue> loader = dictionary[key];
                loader.State = DataLoadState.Loading;
                try
                {
                    TValue value = Load(key);
                    loader.Value = value;
                    loader.State = DataLoadState.Success;
                }
                catch
                {
                    loader.State = DataLoadState.Error;
                }
            }
            while (loadList.Count > 0);
        }
        private void StartDataLoad(TKey key)
        {
            int count;
            lock (loadList)
            {
                if (!loadList.Contains(key))
                    loadList.Enqueue(key);
                count = loadList.Count;

                int neededThreads = (int)Math.Ceiling((double)count / (double)itemsPerThread);

                while (loaderCount < neededThreads)
                {
                    loaderCount++;
                    System.Threading.Thread loadThread = new System.Threading.Thread(DataLoadMethod);
                    loadThread.Start();
                }
            }
        }

        public DataLoader<TValue> this[TKey key]
        {
            get
            {
                DataLoader<TValue> loader;
                lock (dictionary)
                    if (!dictionary.TryGetValue(key, out loader))
                        if (allowKeyMethod(key))
                        {
                            loader = new DataLoader<TValue>(DataLoadState.Initialized);
                            dictionary.Add(key, loader);

                            StartDataLoad(key);
                        }
                        else
                        {
                            loader = new DataLoader<TValue>(DataLoadState.Error);
                            dictionary.Add(key, loader);
                        }

                return loader;
            }
        }

        private Func<TKey, bool> allowKeyMethod;
        private bool defaultAllowKeyMethod(TKey key)
        {
            return true;
        }
        public Func<TKey, bool> AllowKeyMethod
        {
            get { return allowKeyMethod == defaultAllowKeyMethod ? null : allowKeyMethod; }
            set { allowKeyMethod = (value ?? allowKeyMethod); }
        }

        protected abstract TValue Load(TKey key);

        protected Texture2D ConvertToTexture(System.Drawing.Image image, GraphicsDevice device)
        {
            return ConvertToTexture(image, device, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        protected Texture2D ConvertToTexture(System.Drawing.Image image, GraphicsDevice device, System.Drawing.Imaging.ImageFormat imageformat)
        {
            if (image == null)
                return null;

            if (device == null)
                throw new ArgumentNullException("device");

            if (imageformat == null)
                throw new ArgumentNullException("imageformat");

            Texture2D texture;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms, imageformat);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                texture = Texture2D.FromStream(device, ms);
            }
            return texture;
        }

        protected virtual bool AllowKey(TKey key)
        {
            return true;
        }
    }
}
