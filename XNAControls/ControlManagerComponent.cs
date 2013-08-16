using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAControls
{
    internal class ControlManagerComponent : DrawableGameComponent
    {
        private ControlManagerBase manager;

        public ControlManagerComponent(Game game, ControlManagerBase manager)
            : base(game)
        {
            this.manager = manager;
        }
    }

    public static class ControlManagerExtension
    {
        private static Dictionary<ControlManagerBase, ControlManagerComponent> components;

        static ControlManagerExtension()
        {
            components = new Dictionary<ControlManagerBase, ControlManagerComponent>();
        }

        public static void Add(this GameComponentCollection collection, ControlManagerBase manager, Game game)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (game == null)
                throw new ArgumentNullException("game");
            if (collection != game.Components)
                throw new ArgumentException("GameComponentCollection must be Component collection of game argument", "game");

            if (!components.ContainsKey(manager))
                components.Add(manager, new ControlManagerComponent(game, manager));

            collection.Add(components[manager]);
        }
        public static bool Remove(this GameComponentCollection collection, ControlManagerBase manager)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (!components.ContainsKey(manager))
                return false;
            else
                return collection.Remove(components[manager]);
        }
        public static bool Contains(this GameComponentCollection collection, ControlManagerBase manager)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (!components.ContainsKey(manager))
                return false;
            else
                return collection.Contains(components[manager]);
        }
        public static int IndexOf(this GameComponentCollection collection, ControlManagerBase manager)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (!components.ContainsKey(manager))
                return -1;
            else
                return collection.IndexOf(components[manager]);
        }
        public static void Insert(this GameComponentCollection collection, int index, ControlManagerBase manager, Game game)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (game == null)
                throw new ArgumentNullException("game");
            if (collection != game.Components)
                throw new ArgumentException("GameComponentCollection must be Component collection of game argument", "game");

            if (!components.ContainsKey(manager))
                components.Add(manager, new ControlManagerComponent(game, manager));

            collection.Insert(index, components[manager]);
        }
    }
}
