﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Thalamus;
using CookComputing.XmlRpc;
using UnityEngine;
using TheMindThalamusMessages;

public interface IUnityTabletPublisher : IXmlRpcProxy, IUnityPublisher { }

public interface IUnityPublisher : ITabletsGM
{
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

public interface IUnitySubscriber : IGMTablets
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
    new void AllAgreeStar();
    [XmlRpcMethod]
    new void NotAllAgreeStar();
    [XmlRpcMethod]
    new void StarRequest(int playerID);
    [XmlRpcMethod]
    new void StartWait();
    [XmlRpcMethod]
    new void EndWait();
}

public abstract class ThalamusConnector
{
    protected string _remoteAddress = "";

    protected bool _printExceptions = true;
    public string RemoteAddress
    {
        get { return _remoteAddress; }
        set
        {
            _remoteAddress = value;
            _remoteUri = string.Format("http://{0}:{1}/", _remoteAddress, _remotePort);
            //_rpcProxy.Url = _remoteUri;
        }
    }

    protected int _remotePort = 7000;
    public int RemotePort
    {
        get { return _remotePort; }
        set
        {
            _remotePort = value;
            _remoteUri = string.Format("http://{0}:{1}/", _remoteAddress, _remotePort);
            //_rpcProxy.Url = _remoteUri;
        }
    }

    protected HttpListener _listener;
    protected bool _serviceRunning;
    protected int _localPort = 7001;
    protected bool _shutdown;
    List<HttpListenerContext> _httpRequestsQueue = new List<HttpListenerContext>();
    protected Thread _dispatcherThread;
    protected Thread _messageDispatcherThread;


    protected string _remoteUri = "";




    public ThalamusConnector(string ip, int remotePort)
    {
        _remoteAddress = ip;
        _remotePort = remotePort;
        _localPort = _remotePort + 1;
        _remoteUri = String.Format("http://{0}:{1}/", _remoteAddress, _remotePort);
        Debug.Log("Thalamus endpoint set to " + _remoteUri);


        _dispatcherThread = new Thread(DispatcherThreadThalamus);
        _messageDispatcherThread = new Thread(MessageDispatcherThalamus);
        _dispatcherThread.Start();
        _messageDispatcherThread.Start();
    }

    #region rpc stuff

    public void Dispose()
    {
        _shutdown = true;


        try
        {
            if (_listener != null) _listener.Stop();
        }
        catch { }

        try
        {
            if (_dispatcherThread != null) _dispatcherThread.Join();
        }
        catch { }

        try
        {
            if (_messageDispatcherThread != null) _messageDispatcherThread.Join();
        }
        catch { }
    }

    public void DispatcherThreadThalamus()
    {
        while (!_serviceRunning)
        {
            try
            {
                Debug.Log("Attempt to start service on port '" + _localPort + "'");
                _listener = new HttpListener();
                _listener.Prefixes.Add(string.Format("http://*:{0}/", _localPort));
                _listener.Start();
                Debug.Log("XMLRPC Listening on " + string.Format("http://*:{0}/", _localPort));
                _serviceRunning = true;
            }
            catch (Exception e)
            {
                _localPort++;
                Debug.Log(e.Message);
                Debug.Log("Port unavaliable.");
                _serviceRunning = false;
            }
        }

        while (!_shutdown)
        {
            try
            {
                HttpListenerContext context = _listener.GetContext();
                lock (_httpRequestsQueue)
                {
                    _httpRequestsQueue.Add(context);
                }
            }
            catch (Exception e)
            {
                if (_printExceptions) Debug.Log("Exception: " + e);
                _serviceRunning = false;
                if (_listener != null)
                    _listener.Close();
            }
        }
        Debug.Log("Terminated DispatcherThreadThalamus");
        //_listener.Close();
    }

    public void MessageDispatcherThalamus()
    {
        while (!_shutdown)
        {
            bool performSleep = true;
            try
            {
                if (_httpRequestsQueue.Count > 0)
                {
                    performSleep = false;
                    List<HttpListenerContext> httpRequests;
                    lock (_httpRequestsQueue)
                    {
                        httpRequests = new List<HttpListenerContext>(_httpRequestsQueue);
                        _httpRequestsQueue.Clear();
                    }
                    foreach (HttpListenerContext r in httpRequests)
                    {
                        //ProcessRequest(r);
                        (new Thread(ProcessRequestThalamus)).Start(r);
                        performSleep = false;
                    }
                }
            }
            catch (Exception e)
            {
                if (_printExceptions) Debug.Log("Exception: " + e);
            }
            if (performSleep) Thread.Sleep(10);
        }
        Debug.Log("Terminated MessageDispatcherThalamus");
    }

    //this method should be overriden if you need the derived class to listen to thalamus methods
    public abstract void ProcessRequestThalamus(object oContext);
    /*
{
    try
    {
        XmlRpcListenerService svc = new ThalamusDummyListener();
        svc.ProcessRequest((HttpListenerContext)oContext);
    }
    catch (Exception e)
    {
        if (_printExceptions) Debug.Log("Exception: " + e);
    }

}*/

    #endregion

}


public class TabletThalamusConnector : ThalamusConnector, IUnityPublisher
{
    protected IUnityTabletPublisher _rpcProxy;
    private GameManager _gameManager;

    public class UnityRPCListener : XmlRpcListenerService, IUnitySubscriber
    {
        private TabletThalamusConnector _thalamusConnector;

        public UnityRPCListener(TabletThalamusConnector connectorRef)
        {
            _thalamusConnector = connectorRef;
        }

        public void AllConnected(int maxLevel, int p0Id, string p0Name, int p1Id, string p1Name, int p2Id, string p2Name)
        {
            _thalamusConnector._gameManager.MaxLevel = maxLevel;
            _thalamusConnector._gameManager.WaitForNewLevel();
        }

        public void AllRefocused()
        {
            _thalamusConnector._gameManager.AllPlayersRefocused();
        }

        public void RefocusRequest(int playerID)
        {
            _thalamusConnector._gameManager.PlayerRequestedRefocus(playerID);
        }

        public void CardPlayed(int playerID, int card)
        {
            //throw new NotImplementedException();
        }

        public void FinishLevel(int level, int teamLives)
        {
            if (level < _thalamusConnector._gameManager.MaxLevel)
            {
                _thalamusConnector._gameManager.WaitForNewLevel();
            }
        }

        public void GameCompleted()
        {
            _thalamusConnector._gameManager.GameFinished();
        }

        public void GameOver(int level)
        {
            _thalamusConnector._gameManager.GameFinished();
        }

        public void Mistake(int playerID, int card, int[] p0WrongCards, int[] p1WrongCards, int[] p2WrongCards)
        {
            _thalamusConnector._gameManager.MistakeOccurred(playerID, card, p0WrongCards, p1WrongCards, p2WrongCards);
        }

        public void StartLevel(int level, int stars, int teamLives, int[] p0Hand, int[] p1Hand, int[] p2Hand)
        {
            _thalamusConnector._gameManager.NewLevelHasStarted(stars,p0Hand, p1Hand, p2Hand);
        }

        public void AllAgreeStar()
        {
            _thalamusConnector._gameManager.AllAgreeStar();
        }

        public void NotAllAgreeStar()
        {
            _thalamusConnector._gameManager.NotAllAgreeStar();
        }

        public void StarRequest(int playerID)
        {
            _thalamusConnector._gameManager.PlayerRequestedStar(playerID);
            Debug.Log("starRequest");
        }

        public void StartWait()
        {
            _thalamusConnector._gameManager.StartWait();
        }
        public void EndWait()
        {
            _thalamusConnector._gameManager.EndWait();
        }
    }

    public TabletThalamusConnector(GameManager gm, string ip, int remotePort) : base(ip, remotePort)
    {
        _rpcProxy = XmlRpcProxyGen.Create<IUnityTabletPublisher>();
        _rpcProxy.Timeout = 5000;
        _rpcProxy.Url = _remoteUri;
        _gameManager = gm;
    }

    public override void ProcessRequestThalamus(object oContext)
    {
        try
        {
            XmlRpcListenerService svc = new UnityRPCListener(this);
            svc.ProcessRequest((HttpListenerContext)oContext);
        }
        catch (Exception e)
        {
            if (_printExceptions) Debug.Log("Exception: " + e);
        }

    }

    public void ConnectToGM(int playerID, string name)
    {
        _rpcProxy.ConnectToGM(playerID, name);
    }

    public void PlayCard(int playerID, int card)
    {
        _rpcProxy.PlayCard(playerID, card);
    }

    public void RefocusSignal(int playerID)
    {
        _rpcProxy.RefocusSignal(playerID);
    }

    public void ReadyForNextLevel(int playerID)
    {
        _rpcProxy.ReadyForNextLevel(playerID);
    }

    public void ContinueAfterMistake(int playerID)
    {
        _rpcProxy.ContinueAfterMistake(playerID);
    }

    public void StarSignal(int playerID)
    {
        _rpcProxy.StarSignal(playerID);
    }

    public void YesStarSignal(int playerID)
    {
        _rpcProxy.YesStarSignal(playerID);
    }

    public void NoStarSignal(int playerID)
    {
        _rpcProxy.NoStarSignal(playerID);
    }
}
