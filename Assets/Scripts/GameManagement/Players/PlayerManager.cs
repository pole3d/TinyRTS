namespace GameManagement.Players
{
    public enum PlayerTypeEnum
    {
        Local = 0,
        AI = 1
    }

    public enum PlayerTeamEnum
    {
        Team1 = 0,
        Team2 = 1
    }

    /// <summary>
    /// This class contains game data relative to the player
    /// </summary>
    public abstract class PlayerGameData
    {
        public PlayerGameData(PlayerTypeEnum type, PlayerTeamEnum team, int id)
        {
            Type = type;
            Team = team;
            ID = id;
        }

        public PlayerTypeEnum Type { get; private set; }
        public PlayerTeamEnum Team { get; private set; }
        public int ID { get; private set; }
    }
    
    /// <summary>
    /// This class manage a player instance by referencing infos about it and making actions relative to it
    /// </summary>
    /// <typeparam name="TGameData">The playerManager child class</typeparam>
    public class PlayerManager<TGameData> where TGameData : PlayerGameData
    {
        public TGameData GameData { get; protected set; }
    }
}