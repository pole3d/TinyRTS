namespace GameManagement.Players
{
    public class AIPlayerGameData : PlayerGameData
    {
        public AIPlayerGameData(PlayerTypeEnum type, PlayerTeamEnum team, int id) : base(type, team, id)
        {
        }
    }
    
    public class AIPlayerManager : PlayerManager<AIPlayerGameData>
    {
        public AIPlayerManager(PlayerTeamEnum team, int id)
        {
            GameData = new AIPlayerGameData(PlayerTypeEnum.AI, team, id);
        }
    }
}