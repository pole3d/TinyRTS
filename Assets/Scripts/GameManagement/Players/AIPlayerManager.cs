namespace GameManagement.Players
{
    /// <summary>
    /// This class contains game data relative to the AI player
    /// </summary>
    public class AIPlayerGameData : PlayerGameData
    {
        public AIPlayerGameData(PlayerTypeEnum type, PlayerTeamEnum team, int id) : base(type, team, id)
        {
        }
    }
    
    /// <summary>
    /// This class manage an AI player.
    /// </summary>
    public class AIPlayerManager : PlayerManager<AIPlayerGameData>
    {
        public AIPlayerManager(PlayerTeamEnum team, int id)
        {
            GameData = new AIPlayerGameData(PlayerTypeEnum.AI, team, id);
        }
    }
}