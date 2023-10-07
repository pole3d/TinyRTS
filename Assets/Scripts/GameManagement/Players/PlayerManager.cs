namespace GameManagement.Players
{
    public enum PlayerTypeEnum
    {
        Local = 0,
        AI = 1
    }

    public enum PlayerTeamEnum
    {
        Human = 0,
        Orcs = 1
    }

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
    
    public class PlayerManager<TGameData> where TGameData : PlayerGameData
    {
        public TGameData GameData { get; protected set; }
    }
}