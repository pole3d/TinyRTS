namespace GameManagement.Players
{
    /// <summary>
    /// This class contains game data relative to the local player
    /// </summary>
    public class LocalPlayerGameData : PlayerGameData
    {
        public LocalPlayerGameData(PlayerTypeEnum type, PlayerTeamEnum team, int id) : base(type, team, id)
        {
            
        }
    }
    
    /// <summary>
    /// This class manager a player playing locally.
    /// </summary>
    public class LocalPlayerManager : PlayerManager<LocalPlayerGameData>
    {
        public LocalPlayerManager(PlayerTeamEnum team, int id)
        {
            GameData = new LocalPlayerGameData(PlayerTypeEnum.Local, team, id);
        }
    }
}