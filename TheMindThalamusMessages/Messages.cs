using Thalamus;

namespace TheMindThalamusMessages
{
    public interface IGMTablets : IPerception
    {
        void AllConnected(int maxLevel, int p0Id, string p0Name, int p1Id, string p1Name, int p2Id, string p2Name);
        void StartLevel(int level, int stars, int teamLives, int[] p0Hand, int[] p1Hand, int[] p2Hand);
        void FinishLevel(int level, int teamLives);
        void AllRefocused();
        void RefocusRequest(int playerID);
        void CardPlayed(int playerID, int card);
        void Mistake(int playerID, int card, int[] p0WrongCards, int[] p1WrongCards, int[] p2WrongCards);
        void GameOver(int level);
        void GameCompleted();
        void StarRequest(int playerID);
        void AllAgreeStar();
        void NotAllAgreeStar();
        void StartWait();
        void EndWait();
    }


    public interface ITabletsGM : IAction
    {
        void ConnectToGM(int playerID, string name);
        void PlayCard(int playerID, int card);
        void RefocusSignal(int playerID);
        void ReadyForNextLevel(int playerID);
        void ContinueAfterMistake(int playerID);
        void StarSignal(int playerID);
        void NoStarSignal(int playerID);
        void YesStarSignal(int playerID);
        
    }
}
