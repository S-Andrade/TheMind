from GameMoment import GameMoment
import sys

getSound = sys.argv[1]
if getSound == "false":
    getSound = False
if getSound == "true":
    getSound = True

# read and organize the file
player0 = []
player1 = []
player2 = []
game = []

sound_player0 = []
sound_player1 = []
sound_player2 = []

f = open("elan-exp1-sound.txt", "r")
for x in f:
    x = x.strip("\n")
    line  = x.split('\t')

    size = int(float(line[7])*1000)
    begin =  int(float(line[3])*1000)

    if line[0] == "gaze-player0":
        player0 += [line[8].strip(" ")] * size
    if line[0] == "gaze-player1":
        player1 += [line[8].strip(" ")] * size
    if line[0] == "gaze-player2":
        player2 += [line[8].strip(" ")] * size
    if line[0] == "game":
        game += [line[8].strip(" ")] * size
    if line[0] == "sound-player0":
        sound_player0 += [0] * (begin-len(sound_player0))
        sound_player0 += [line[8].strip(" ")] * size
    if line[0] == "sound-player1":
        sound_player1 += [0] * (begin-len(sound_player1))
        sound_player1 += [line[8].strip(" ")] * size
    if line[0] == "sound-player2":
        sound_player2 += [0] * (begin-len(sound_player2))
        sound_player2 += [line[8].strip(" ")] * size
   

video_size = 653000

player0 = player0[:video_size]
player1 = player1[:video_size]
player2 = player2[:video_size]
game = game[:video_size]

sound_player0 += [0] * (video_size-len(sound_player0))
sound_player1 += [0] * (video_size-len(sound_player1))
sound_player2 += [0] * (video_size-len(sound_player2))
sound = [sound_player0,sound_player1,sound_player2]

# analize data
#gameR = [GameMoment("waiting-for-card"), GameMoment("play-card-player0"), GameMoment("play-card-player1"), GameMoment("play-card-player2"), GameMoment("start-level"), GameMoment("refocus"), GameMoment("mistake"), GameMoment("game-over")]

gameR = []

def addTalkLaughMG(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].MutalGaze['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].MutalGaze['L'+str(j)+string] += 1
        j += 1

def addTalkLaughJA(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].JointAttention['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].JointAttention['L'+str(j)+string] += 1
        j += 1

def addTalkLaughLA(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].LookAt['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].LookAt['L'+str(j)+string] += 1
        j += 1

def addTalkLaughM(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].Mainscreen['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].Mainscreen['L'+str(j)+string] += 1
        j += 1

def addTalkLaughT(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].Tablet['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].Tablet['L'+str(j)+string] += 1
        j += 1

def addTalkLaughS(i, string, index):
    j = 0
    for s in sound:
        if s[i] == "talk":
            gameR[index].SomewhereElse['T'+str(j)+string] += 1
        if s[i] == "laugh":
            gameR[index].SomewhereElse['L'+str(j)+string] += 1
        j += 1

i = 0
while (i < video_size):
    if len(gameR) == 0 or game[i] != gameR[-1].name:
        gameR.append(GameMoment(game[i]))
    gameR[-1].size += 1
    

    #MUTUAL GAZE
    if player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
        gameR[-1].MutalGaze["P0P1"] += 1 
        addTalkLaughMG(i,"P0P1",-1)
    if player0[i] == 'look-at-player2' and player2[i] == 'look-at-player0':
        gameR[-1].MutalGaze["P0P2"] += 1 
        addTalkLaughMG(i,"P0P2",-1)
    if player1[i] == 'look-at-player2' and player2[i] == 'look-at-player1':
        gameR[-1].MutalGaze["P1P2"] += 1 
        addTalkLaughMG(i,"P1P2",-1)

    #JOINT ATTENTION
    if player0[i] == "look-at-player2" and player1[i] == "look-at-player2":
        gameR[-1].JointAttention["P0P1LP2"] += 1
        addTalkLaughJA(i,"P0P1LP2",-1)

    if player0[i] == "look-at-player1" and player2[i] == "look-at-player1":
        gameR[-1].JointAttention["P0P2LP1"] += 1
        addTalkLaughJA(i,"P0P2LP1",-1)

    if player1[i] == "look-at-player0" and player2[i] == "look-at-player0":
        gameR[-1].JointAttention["P1P2LP0"] += 1
        addTalkLaughJA(i,"P1P2LP0",-1)

    #LOOK AT
    if player0[i] == 'look-at-player1':
        gameR[-1].LookAt["P0LP1"] += 1
        addTalkLaughLA(i,"P0LP1",-1)

    if player0[i] == 'look-at-player2':
        gameR[-1].LookAt["P0LP2"] += 1
        addTalkLaughLA(i,"P0LP2",-1)

    if player1[i] == 'look-at-player0':
        gameR[-1].LookAt["P1LP0"] += 1
        addTalkLaughLA(i,"P1LP0",-1)

    if player1[i] == 'look-at-player2':
        gameR[-1].LookAt["P1LP2"] += 1
        addTalkLaughLA(i,"P1LP2",-1)

    if player2[i] == 'look-at-player0':
        gameR[-1].LookAt["P2LP0"] += 1
        addTalkLaughLA(i,"P2LP0",-1)

    if player2[i] == 'look-at-player1':
        gameR[-1].LookAt["P2LP1"] += 1
        addTalkLaughLA(i,"P2LP1",-1)
    

    #MainScrean
    if player0[i] == "look-at-mainscreen":
        gameR[-1].Mainscreen["P0"] += 1
        addTalkLaughM(i,"P0",-1)

    if player1[i] == "look-at-mainscreen":
        gameR[-1].Mainscreen["P1"] += 1
        addTalkLaughM(i,"P1",-1)

    if player2[i] == "look-at-mainscreen":
        gameR[-1].Mainscreen["P2"] += 1
        addTalkLaughM(i,"P2",-1)


    if player0[i] == "look-at-tablet":
        gameR[-1].Tablet["P0"] += 1
        addTalkLaughT(i,"P0",-1)
    if player1[i] == "look-at-tablet":
        gameR[-1].Tablet["P1"] += 1
        addTalkLaughT(i,"P1",-1)
    if player2[i] == "look-at-tablet":
        gameR[-1].Tablet["P2"] += 1
        addTalkLaughT(i,"P2",-1)

    if player0[i] == "look-at-somewhere-else":
        gameR[-1].SomewhereElse["P0"] += 1
        addTalkLaughS(i,"P0",-1)
    if player1[i] == "look-at-somewhere-else":
        gameR[-1].SomewhereElse["P1"] += 1
        addTalkLaughS(i,"P1",-1)
    if player2[i] == "look-at-somewhere-else":
        gameR[-1].SomewhereElse["P2"] += 1
        addTalkLaughS(i,"P2",-1)



    i += 1


def printNice(text, number):
    if number > 0:
        file.write( text +  ": " + str(round(number,2)) +"%\n")


with open('game1-sections.txt', 'w') as file:
    for g in gameR:
        file.write("##### " + g.name + " #####\n")

        file.write("Mutual gaze\n")
        printNice("Between player0 and player1",g.MutalGaze["P0P1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.MutalGaze["T"+str(i)+"P0P1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.MutalGaze["L"+str(i)+"P0P1"]/g.size*100)
                i+=1

        printNice("Between player0 and player2",g.MutalGaze["P0P2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.MutalGaze["T"+str(i)+"P0P2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.MutalGaze["L"+str(i)+"P0P2"]/g.size*100)
                i+=1
        
        printNice("Between player1 and player2",g.MutalGaze["P1P2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.MutalGaze["T"+str(i)+"P1P2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.MutalGaze["L"+str(i)+"P1P2"]/g.size*100)
                i+=1
        
        
        
        file.write("\n")
        
        file.write("Joint Attention\n")
        printNice("Player0 and Player1 looked at Player2", g.JointAttention["P0P1LP2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.JointAttention["T"+str(i)+"P0P1LP2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.JointAttention["L"+str(i)+"P0P1LP2"]/g.size*100)
                i+=1
        printNice("Player0 and Player2 looked at Player1", g.JointAttention["P0P2LP1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.JointAttention["T"+str(i)+"P0P2LP1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.JointAttention["L"+str(i)+"P0P2LP1"]/g.size*100)
                i+=1
        printNice("Player1 and Player2 looked at Player0", g.JointAttention["P1P2LP0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.JointAttention["T"+str(i)+"P1P2LP0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.JointAttention["L"+str(i)+"P1P2LP0"]/g.size*100)
                i+=1

        file.write("\n")


        file.write("Player0\n")
        printNice("Player0 look at Player1", g.LookAt["P0LP1"]/g.size*100)
        if getSound:       
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P0LP1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P0LP1"]/g.size*100)
                i+=1
        printNice("Player0 look at Player2", g.LookAt["P0LP2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P0LP2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P0LP2"]/g.size*100)
                i+=1

        printNice("Player0 looked at the Mainscreen", g.Mainscreen["P0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Mainscreen["T"+str(i)+"P0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Mainscreen["L"+str(i)+"P0"]/g.size*100)
                i+=1
        
        printNice("Player0 looked at the Tablet", g.Tablet["P0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Tablet["T"+str(i)+"P0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Tablet["L"+str(i)+"P0"]/g.size*100)
                i+=1
        
        printNice("Player0 looked at the Somewhere else", g.SomewhereElse["P0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.SomewhereElse["T"+str(i)+"P0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.SomewhereElse["L"+str(i)+"P0"]/g.size*100)
                i+=1

        
        file.write("\n")
        file.write("Player1\n")
        printNice("Player1 look at Player0", g.LookAt["P1LP0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P1LP0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P1LP0"]/g.size*100)
                i+=1
        printNice("Player1 look at Player2", g.LookAt["P1LP2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P1LP2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P1LP2"]/g.size*100)
                i+=1
        printNice("Player1 looked at the Mainscreen", g.Mainscreen["P1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Mainscreen["T"+str(i)+"P1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Mainscreen["L"+str(i)+"P1"]/g.size*100)
                i+=1
        printNice("Player0 looked at the Tablet", g.Tablet["P1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Tablet["T"+str(i)+"P1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Tablet["L"+str(i)+"P1"]/g.size*100)
                i+=1
        
        printNice("Player0 looked at the Somewhere else", g.SomewhereElse["P1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.SomewhereElse["T"+str(i)+"P1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.SomewhereElse["L"+str(i)+"P1"]/g.size*100)
                i+=1

        file.write("\n")
        file.write("Player2\n")
        printNice("Player2 look at Player0", g.LookAt["P2LP0"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P2LP0"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P2LP0"]/g.size*100)
                i+=1
        printNice("Player2 look at Player1", g.LookAt["P2LP1"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.LookAt["T"+str(i)+"P2LP1"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.LookAt["L"+str(i)+"P2LP1"]/g.size*100)
                i+=1
        printNice("Player2 looked at the Mainscreen", g.Mainscreen["P2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Mainscreen["T"+str(i)+"P2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Mainscreen["L"+str(i)+"P2"]/g.size*100)
                i+=1
        
        printNice("Player0 looked at the Tablet", g.Tablet["P2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.Tablet["T"+str(i)+"P2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.Tablet["L"+str(i)+"P2"]/g.size*100)
                i+=1
        
        printNice("Player0 looked at the Somewhere else", g.SomewhereElse["P2"]/g.size*100)
        if getSound:
            i = 0
            while i < 3:
                printNice("\tPlayer" + str(i) + " talk", g.SomewhereElse["T"+str(i)+"P2"]/g.size*100)
                printNice("\tPlayer" + str(i) + " laugh", g.SomewhereElse["L"+str(i)+"P2"]/g.size*100)
                i+=1
        
        file.write("\n\n")