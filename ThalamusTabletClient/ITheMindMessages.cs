using Thalamus;
using CookComputing.XmlRpc;
using TheMindThalamusMessages;


public interface IThalamusTabletPublisher : IThalamusPublisher, ITabletsGM
{
}
public interface IUnityTabletSubscriber : ITabletsGM
{
    void Dispose();

    [XmlRpcMethod]
    new void ConnectToGM(int playerID, string name);
    [XmlRpcMethod]
    new void PlayCard(int playerID, int card);
    [XmlRpcMethod]
    new void RefocusSignal(int playerID);
    [XmlRpcMethod]
    new void ReadyForNextLevel(int playerID);
    [XmlRpcMethod]
    new void ContinueAfterMistake(int playerID);
    [XmlRpcMethod]
    new void StarSignal(int playerID);
    [XmlRpcMethod]
    new void NoStarSignal(int playerID);
    [XmlRpcMethod]
    new void YesStarSignal(int playerID);

}

public interface IUnityTabletPublisher : IGMTablets, IXmlRpcProxy
{
    [XmlRpcMethod]
    new void AllConnected(int maxLevel, int p0Id, string p0Name, int p1Id, string p1Name, int p2Id, string p2Name);
    [XmlRpcMethod]
    new void StartLevel(int level, int stars, int teamLives, int[] p0Hand, int[] p1Hand, int[] p2Hand);
    [XmlRpcMethod]
    new void FinishLevel(int level, int teamLives);
    [XmlRpcMethod]
    new void AllRefocused();
    [XmlRpcMethod]
    new void RefocusRequest(int playerID);
    [XmlRpcMethod]
    new void CardPlayed(int playerID, int card);
    [XmlRpcMethod]
    new void Mistake(int playerID, int card, int[] p0WrongCards, int[] p1WrongCards, int[] p2WrongCards);
    [XmlRpcMethod]
    new void GameOver(int level);
    [XmlRpcMethod]
    new void GameCompleted();
    [XmlRpcMethod]
    new void StarRequest(int playerID);
    [XmlRpcMethod]
    new void AllAgreeStar();
    [XmlRpcMethod]
    new void NotAllAgreeStar();
    [XmlRpcMethod]
    new void StartWait();
    [XmlRpcMethod]
    new void EndWait();

}
