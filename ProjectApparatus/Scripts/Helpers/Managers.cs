namespace Hax
{
    public static partial class Helper
    {
        public static HUDManager HUDManager => Unfake(HUDManager.Instance);

        public static RoundManager RoundManager => Unfake(RoundManager.Instance);

        public static StartOfRound StartOfRound => Unfake(StartOfRound.Instance);

        private static T Unfake<T>(T obj) where T : class
        {
            return obj == null || obj.Equals(null) ? default(T) : obj;
        }
    }
}
