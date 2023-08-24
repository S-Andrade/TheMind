player0 = []
player1 = []
player2 = []
game = []


game_dic = {"waiting-for-card":[],"play-card-player0":[],"play-card-player1":[],"play-card-player2":[],"start-level":[],"refocus":[],"mistake":[],"game-over":[]}


f = open("gazes.txt", "r")
for x in f:
  x = x.strip("\n")
  line  = x.split('\t')
  size = int(float(line[7])*1000)
  if line[0] == "gaze-player0":
    player0 += [line[8].strip(" ")] * size
  if line[0] == "gaze-player1":
    player1 += [line[8].strip(" ")] * size
  if line[0] == "gaze-player2":
    player2 += [line[8].strip(" ")] * size
  if line[0] == "game":
    game += [line[8].strip(" ")] * size
    game_dic[line[8].strip(" ")] += [float(line[7])]

video_size = 653000

player0 = player0[:video_size]
player1 = player1[:video_size]
player2 = player2[:video_size]
game = game[:video_size]

MGP0P1 = 0
MGP0P2 = 0
MGP1P2 = 0

P0LP1 = 0
P0LP2 = 0
P1LP0 = 0
P1LP2 = 0
P2LP0 = 0
P2LP1 = 0

MP0 = 0
MP1 = 0
MP2 = 0

TP0 = 0
TP1 = 0
TP2 = 0

SP0 = 0
SP1 = 0
SP2 = 0


i = 0
while (i < video_size):
    if player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
        MGP0P1 += 1
    if player0[i] == "look-at-player2" and player2[i] == "look-at-player0":
        MGP0P2 += 1
    if player1[i] == "look-at-player2" and player2[i] == "look-at-player1":
        MGP1P2 += 1


    if player0[i] == 'look-at-player1':
       P0LP1 += 1
    if player0[i] == 'look-at-player2':
       P0LP2 += 1
    if player1[i] == 'look-at-player0':
       P1LP0 += 1
    if player1[i] == 'look-at-player2':
       P1LP2 += 1
    if player2[i] == 'look-at-player0':
       P2LP0 += 1
    if player2[i] == 'look-at-player1':
       P2LP1 += 1

    if player0[i] == "look-at-mainscreen":
        MP0 += 1
    if player1[i] == "look-at-mainscreen":
        MP1 += 1
    if player2[i] == "look-at-mainscreen":
        MP2 += 1

    if player0[i] == "look-at-tablet":
        TP0 += 1
    if player1[i] == "look-at-tablet":
        TP1 += 1
    if player2[i] == "look-at-tablet":
        TP2 += 1

    if player0[i] == "look-at-somewhere-else":
        SP0 += 1
    if player1[i] == "look-at-somewhere-else":
        SP1 += 1
    if player2[i] == "look-at-somewhere-else":
        SP2 += 1
    i += 1

print("Player0")
print("Player0 looked at the mainscreen: {:.2f}%".format(MP0/video_size*100))
print("Player0 looked at the tablet: {:.2f}%".format(TP0/video_size*100))
print("Player0 looked at somewhere else: {:.2f}%".format(SP0/video_size*100))
print("Player0 looked at Player1: {:.2f}%".format(P0LP1/video_size*100))
print("Player0 looked at Player2: {:.2f}%".format(P0LP2/video_size*100))
print("\n")

print("Player1")
print("Player1 looked at the mainscreen: {:.2f}%".format(MP1/video_size*100))
print("Player1 looked at the tablet: {:.2f}%".format(TP1/video_size*100))
print("Player1 looked at somewhere else: {:.2f}%".format(SP1/video_size*100))
print("Player1 looked at Player0: {:.2f}%".format(P1LP0/video_size*100))
print("Player1 looked at Player2: {:.2f}%".format(P1LP2/video_size*100))
print("\n")

print("Player2")
print("Player2 looked at the mainscreen: {:.2f}%".format(MP2/video_size*100))
print("Player2 looked at the tablet: {:.2f}%".format(TP2/video_size*100))
print("Player2 looked at somewhere else: {:.2f}%".format(SP2/video_size*100))
print("Player2 looked at Player0: {:.2f}%".format(P2LP0/video_size*100))
print("Player2 looked at Player1: {:.2f}%".format(P2LP1/video_size*100))
print("\n")

print("Mutual gaze between player0 and player1: {:.2f}%".format(MGP0P1/video_size*100))
print("Mutual gaze between player0 and player2: {:.2f}%".format(MGP0P2/video_size*100))
print("Mutual gaze between player1 and player2: {:.2f}%".format(MGP1P2/video_size*100))
print("\n")


## game ###



WC = 0
PCP0 = 0
PCP1 = 0
PCP2 = 0
SL = 0
R = 0
M = 0
GO = 0



i = 0
while (i < video_size):
    if game[i] == "waiting-for-card":
       WC += 1
    if game[i] == "play-card-player0":
       PCP0 += 1
    if game[i] == "play-card-player1":
       PCP1 += 1
    if game[i] == "play-card-player2":
       PCP2 += 1
    if game[i] == "start-level":
       SL += 1
    if game[i] == "refocus":
       R += 1
    if game[i] == "mistake":
       M += 1
    if game[i] == "game-over":
       GO += 1
   
    i += 1

print("GAME")

print("Wainting for card: {:.2f}%".format(WC/video_size*100))
print("Player0 played a card: {:.2f}%".format(PCP0/video_size*100))
print("Player1 played a card: {:.2f}%".format(PCP1/video_size*100))
print("Player2 played a card: {:.2f}%".format(PCP2/video_size*100))
print("Start level: {:.2f}%".format(SL/video_size*100))
print("Refocus: {:.2f}%".format(R/video_size*100))
print("Mistake: {:.2f}%".format(M/video_size*100))
print("Game Over: {:.2f}%".format(GO/video_size*100))

print("\n")
#print(game_dic)

for key in game_dic.keys():
    print("Sum of " + key + ": {:.2f}s".format(sum(game_dic[key])))
    print("Average of " + key + ": {:.2f}s".format(sum(game_dic[key]) / len(game_dic[key])))
    print("Max of " + key + ": {:.2f}s".format(max(game_dic[key])))
    print("Min of " + key + ": {:.2f}s".format(min(game_dic[key])))
    print("\n")