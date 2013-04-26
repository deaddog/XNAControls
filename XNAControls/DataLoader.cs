using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAControls
{
    public class DataLoader<TValue>
    {
        private DataLoadState state;

        private TValue value;

        internal DataLoader(DataLoadState state, TValue value = default(TValue))
        {
            this.state = state;
            this.value = value;
        }

        public DataLoadState State
        {
            get { return state; }
            internal set { state = value; }
        }
        public TValue Value
        {
            get { return value; }
            internal set { this.value = value; }
        }
    }
}
