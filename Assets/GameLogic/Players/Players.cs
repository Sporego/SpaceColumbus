using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public struct OwnershipInfo
    {
        public uint id { get; }

        public OwnershipInfo(uint id)
        {
            this.id = id;
        }
    }

    public class Ownership
    {
        public OwnershipInfo info { get; private set; }

        public Ownership(uint id)
        {
            this.info = new OwnershipInfo(id);
        }

        public void SetOwnership(uint id)
        {
            this.info = new OwnershipInfo(id);
        }

        public bool IsOwnedByPlayer(Player player)
        {
            return this.Equals(player.ownership);
        }

        public bool Equals(Ownership ownership)
        {
            return this.info.id == ownership.info.id;
        }
    }

    public class Player
    {
        public Ownership ownership { get; }

        public Player(uint id)
        {
            this.ownership = new Ownership(id);
        }
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(uint id) : base(id) { }

    }

    public class AIPlayer : Player
    {
        public AIPlayer(uint id) : base(id) { }
    }

    public static class PlayerManager
    {
        public static List<Player> Players { get; private set; }

        public static void Initialize()
        {
            Players = new List<Player>();
        }

        public static void AddNewPlayer(Player player)
        {
            Players.Add(player);
        }

        public static Player AddNewPlayer()
        {
            Player player = new Player((uint)Players.Count);
            AddNewPlayer(player);
            return player;
        }
    }
}


