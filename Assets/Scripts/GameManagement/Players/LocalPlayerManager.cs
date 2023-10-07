namespace GameManagement.Players
{
    public class LocalPlayerGameData : PlayerGameData
    {
        public LocalPlayerGameData(PlayerTypeEnum type, PlayerTeamEnum team, int id) : base(type, team, id)
        {
            
        }
    }
    
    public class LocalPlayerManager : PlayerManager<LocalPlayerGameData>
    {
        public LocalPlayerManager(PlayerTeamEnum team, int id)
        {
            GameData = new LocalPlayerGameData(PlayerTypeEnum.Local, team, id);
        }
    }
}