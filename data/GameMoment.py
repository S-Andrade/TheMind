class GameMoment:
    name = ""
    size = 0
    MutalGaze = {}
    LookAt = {}
    MainScreen = {}
    Tablet = {}
    SomewhereElse = {}
    JointAttention = {}
    p0p1 = []
    p0p2 = []
    p1p2 = []
        
    def __init__(self, name):
        self.name = name
        self.size  = 0
        self.MutalGaze = {"P0P1": 0, "P0P2":0, "P1P2":0, 
                     "T0P0P1": 0, "T0P0P2":0, "T0P1P2":0,"L0P0P1": 0, "L0P0P2":0, "L0P1P2":0,
                     "T1P0P1": 0, "T1P0P2":0, "T1P1P2":0,"L1P0P1": 0, "L1P0P2":0, "L1P1P2":0,
                     "T2P0P1": 0, "T2P0P2":0, "T2P1P2":0,"L2P0P1": 0, "L2P0P2":0, "L2P1P2":0
                    }
                    
        self.LookAt = {"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,
                  "T0P0LP1":0,"T0P0LP2":0,"T0P1LP0":0,"T0P1LP2":0,"T0P2LP0":0,"T0P2LP1":0,"L0P0LP1":0,"L0P0LP2":0,"L0P1LP0":0,"L0P1LP2":0,"L0P2LP0":0,"L0P2LP1":0,
                  "T1P0LP1":0,"T1P0LP2":0,"T1P1LP0":0,"T1P1LP2":0,"T1P2LP0":0,"T1P2LP1":0,"L1P0LP1":0,"L1P0LP2":0,"L1P1LP0":0,"L1P1LP2":0,"L1P2LP0":0,"L1P2LP1":0,
                  "T2P0LP1":0,"T2P0LP2":0,"T2P1LP0":0,"T2P1LP2":0,"T2P2LP0":0,"T2P2LP1":0,"L2P0LP1":0,"L2P0LP2":0,"L2P1LP0":0,"L2P1LP2":0,"L2P2LP0":0,"L2P2LP1":0
                }

        self.Mainscreen = {"P0":0,"P1":0,"P2":0,
                      "T0P0":0,"T0P1":0,"T0P2":0,"L0P0":0,"L0P1":0,"L0P2":0,
                      "T1P0":0,"T1P1":0,"T1P2":0,"L1P0":0,"L1P1":0,"L1P2":0,
                      "T2P0":0,"T2P1":0,"T2P2":0,"L2P0":0,"L2P1":0,"L2P2":0,
                    }
        
        self.Tablet = {"P0":0,"P1":0,"P2":0,
                      "T0P0":0,"T0P1":0,"T0P2":0,"L0P0":0,"L0P1":0,"L0P2":0,
                      "T1P0":0,"T1P1":0,"T1P2":0,"L1P0":0,"L1P1":0,"L1P2":0,
                      "T2P0":0,"T2P1":0,"T2P2":0,"L2P0":0,"L2P1":0,"L2P2":0,
                    }

        self.SomewhereElse = {"P0":0,"P1":0,"P2":0,
                      "T0P0":0,"T0P1":0,"T0P2":0,"L0P0":0,"L0P1":0,"L0P2":0,
                      "T1P0":0,"T1P1":0,"T1P2":0,"L1P0":0,"L1P1":0,"L1P2":0,
                      "T2P0":0,"T2P1":0,"T2P2":0,"L2P0":0,"L2P1":0,"L2P2":0,
                    }

        self.JointAttention = {"P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0,
                         "T0P0P1LP2":0, "T0P0P2LP1":0, "T0P1P2LP0":0,"L0P0P1LP2":0, "L0P0P2LP1":0, "L0P1P2LP0":0,
                         "T1P0P1LP2":0, "T1P0P2LP1":0, "T1P1P2LP0":0,"L1P0P1LP2":0, "L1P0P2LP1":0, "L1P1P2LP0":0,
                         "T2P0P1LP2":0, "T2P0P2LP1":0, "T2P1P2LP0":0,"L2P0P1LP2":0, "L2P0P2LP1":0, "L2P1P2LP0":0,
                        }

        self.p0p1  = [False,[]]
        self.p0p2  = [False,[]]
        self.p1p2  = [False,[]]

 