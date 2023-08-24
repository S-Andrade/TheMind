from GameMoment import GameMoment
import sys

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

gameR = []

i = 0
while (i < video_size):
    if len(gameR) == 0 or game[i] != gameR[-1].name:
        gameR.append(GameMoment(game[i]))
    gameR[-1].size += 1
    
    if player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
        if gameR[-1].p0p1[0] == True:
            gameR[-1].p0p1[1][-1] +=1
        if gameR[-1].p0p1[0] == False:
            gameR[-1].p0p1[1].append(1)
            gameR[-1].p0p1[0] = True
    if player0[i] == 'look-at-player2' and player2[i] == 'look-at-player0':
        if gameR[-1].p0p2[0] == True:
            gameR[-1].p0p2[1][-1] +=1
        if gameR[-1].p0p2[0] == False:
            gameR[-1].p0p2[1].append(1)
            gameR[-1].p0p2[0] = True
    if player1[i] == 'look-at-player2' and player2[i] == 'look-at-player1':
        if gameR[-1].p1p2[0] == True:
            gameR[-1].p1p2[1][-1] +=1
        if gameR[-1].p1p2[0] == False:
            gameR[-1].p1p2[1].append(1)
            gameR[-1].p1p2[0] = True
    
    if (player0[i-1] == 'look-at-player1' and player1[i-1] == 'look-at-player0') and (player0[i] != 'look-at-player1' or  player1[i] != 'look-at-player0'):
        gameR[-1].p0p1[0] = False

    if (player0[i-1] == 'look-at-player2' and player2[i-1] == 'look-at-player0') and (player0[i] != 'look-at-player2' or player2[i] != 'look-at-player0'):
        gameR[-1].p0p2[0] = False

    if (player1[i-1] == 'look-at-player2' and player2[i-1] == 'look-at-player1') and (player1[i] != 'look-at-player2' or player2[i] != 'look-at-player1'):
        gameR[-1].p1p2[0] = False

    i += 1


def printNice(text, number):
    if number > 0:
        file.write( text +  ": " + str(round(number,2)) +"\n")


with open('freq-sections.txt', 'w') as file:
    for g in gameR:
        file.write("##### " + g.name + " #####\n")
     
        if len(g.p0p1[1]) > 0:
            file.write("p0p1\n")
            printNice("Freqencia" , sum(g.p0p1[1])/g.size/1000)
            printNice("Media" , sum(g.p0p1[1])/len(g.p0p1[1])/1000)
            printNice("Max" , max(g.p0p1[1])/1000)
            printNice("Min" , min(g.p0p1[1])/1000)

        if len(g.p0p2[1]) > 0:
            file.write("p0p2\n")
            printNice("Freqencia" , sum(g.p0p2[1])/g.size/1000)
            printNice("Media" , sum(g.p0p2[1])/len(g.p0p2[1])/1000)
            printNice("Max" , max(g.p0p2[1])/1000)
            printNice("Min" , min(g.p0p2[1])/1000)


        if len(g.p1p2[1]) > 0:
            file.write("p1p2\n")
            printNice("Freqencia" , sum(g.p1p2[1])/g.size/1000)
            printNice("Media" , sum(g.p1p2[1])/len(g.p1p2[1])/1000)
            printNice("Max" , max(g.p1p2[1])/1000)
            printNice("Min" , min(g.p1p2[1])/1000)

        total = g.p0p1[1] + g.p0p2[1] + g.p1p2[1]

        if len(total) > 0:
            file.write("Total\n")
            printNice("Freqencia" , sum(total)/video_size/1000)
            printNice("Media" , sum(total)/len(total)/1000)
            printNice("Max" , max(total)/1000)
            printNice("Min" , min(total)/1000)

        file.write("\n")