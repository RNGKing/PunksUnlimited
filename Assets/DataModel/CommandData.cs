using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PunksUnlimited.DataModel
{
    public enum CommandType
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        BUILD,
        NONE,
        EMPTY
    }

    sealed class MoveData : CommandData
    {
        public Vector2 Move { get; private set; }
        public MoveData(int playerID, Vector2 move) : base(playerID) 
        {
            this.Move = new Vector2(move.x, move.y);
        }
    }

    sealed class BuildData : CommandData
    {
        public BuildData(int playerID) : base(playerID)
        {
                
        }
    }

    public class CommandData
    {
        public CommandType Type { get; private set; }
        public int ID { get; private set; }
        public virtual int PlayerID { get; private set; }

        public static CommandData Empty
        {
            get
            {
                return new CommandData();
            }
        }

        internal CommandData(int playerID)
        {
            PlayerID = playerID;
        }

        internal CommandData()
        {
            Type = CommandType.EMPTY;
        }

        public static CommandData GenerateNewCommand(int playerID, string input)
        {
            if (input.Equals("right"))
            {
                return new MoveData(playerID, new Vector2(1, 0));
            }   
            else if (input.Equals("left"))
            {
                return new MoveData(playerID, new Vector2(-1, 0));
            }
            else if (input.Equals("down"))
            {
                return new MoveData(playerID, new Vector2(0, -1));
            }
            else if (input.Equals("up"))
            {
                return new MoveData(playerID, new Vector2(0, 1));
            }
            else if (input.Equals("build"))
            {
                return new BuildData(playerID);
            }
            else
            {
                return CommandData.Empty;
            }
        }
    }

}
