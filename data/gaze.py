player0 = []
player1 = []
player2 = []
game = []

sound_player0 = []
sound_player1 = []
sound_player2 = []



game_dic = {"waiting-for-card":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "play-card-player0":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "play-card-player1":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "play-card-player2":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "start-level":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "refocus":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "mistake":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0},
   "game-over":{"times": [],"MGP0P1":0,"MGP0P2":0,"MGP1P2":0,"P0LP1":0,"P0LP2":0,"P1LP0":0,"P1LP2":0,"P2LP0":0,"P2LP1":0,"MP0":0,"MP1":0,"MP2":0,"TP0":0,"TP1":0,"TP2":0,"SP0":0,"SP1":0,"SP2":0, "P0P1LP2":0, "P0P2LP1":0, "P1P2LP0":0}}


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
    game_dic[line[8].strip(" ")]["times"] += [size]

video_size = 653000

player0 = player0[:video_size]
player1 = player1[:video_size]
player2 = player2[:video_size]
game = game[:video_size]




i = 0
while (i < video_size):
    if player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
        game_dic[game[i]]["MGP0P1"] += 1
    if player0[i] == "look-at-player2" and player2[i] == "look-at-player0":
        game_dic[game[i]]["MGP0P2"] += 1
    if player1[i] == "look-at-player2" and player2[i] == "look-at-player1":
        game_dic[game[i]]["MGP1P2"] += 1

    if player0[i] == 'look-at-player1':
       game_dic[game[i]]["P0LP1"] += 1
    if player0[i] == 'look-at-player2':
       game_dic[game[i]]["P0LP2"] += 1
    if player1[i] == 'look-at-player0':
       game_dic[game[i]]["P1LP0"] += 1
    if player1[i] == 'look-at-player2':
       game_dic[game[i]]["P1LP2"] += 1
    if player2[i] == 'look-at-player0':
       game_dic[game[i]]["P2LP0"] += 1
    if player2[i] == 'look-at-player1':
       game_dic[game[i]]["P2LP1"] += 1

    if player0[i] == "look-at-mainscreen":
        game_dic[game[i]]["MP0"] += 1
    if player1[i] == "look-at-mainscreen":
        game_dic[game[i]]["MP1"] += 1
    if player2[i] == "look-at-mainscreen":
        game_dic[game[i]]["MP2"] += 1

    if player0[i] == "look-at-tablet":
        game_dic[game[i]]["TP0"] += 1
    if player1[i] == "look-at-tablet":
        game_dic[game[i]]["TP1"] += 1
    if player2[i] == "look-at-tablet":
        game_dic[game[i]]["TP2"] += 1

    if player0[i] == "look-at-somewhere-else":
        game_dic[game[i]]["SP0"] += 1
    if player1[i] == "look-at-somewhere-else":
        game_dic[game[i]]["SP1"] += 1
    if player2[i] == "look-at-somewhere-else":
        game_dic[game[i]]["SP2"] += 1

    if player0[i] == "look-at-player1" and player2[i] == "look-at-player1":
        game_dic[game[i]]["P0P2LP1"] += 1
    if player0[i] == "look-at-player2" and player1[i] == "look-at-player2":
        game_dic[game[i]]["P0P1LP2"] += 1
    if player1[i] == "look-at-player0" and player2[i] == "look-at-player0":
        game_dic[game[i]]["P1P2LP0"] += 1
    i += 1
    

for key in game_dic.keys():
    print("##### " + key + " #####")
    print("Player0")
    print("Player0 looked at the mainscreen: {:.2f}%".format(game_dic[key]["MP0"]/sum(game_dic[key]["times"])*100))
    print("Player0 looked at the tablet: {:.2f}%".format(game_dic[key]["TP0"]/sum(game_dic[key]["times"])*100))
    print("Player0 looked at somewhere else: {:.2f}%".format(game_dic[key]["SP0"]/sum(game_dic[key]["times"])*100))
    print("Player0 looked at Player1: {:.2f}%".format(game_dic[key]["P0LP1"]/sum(game_dic[key]["times"])*100))
    print("Player0 looked at Player2: {:.2f}%".format(game_dic[key]["P0LP2"]/sum(game_dic[key]["times"])*100))
    print("\n")

    print("Player1")
    print("Player1 looked at the mainscreen: {:.2f}%".format(game_dic[key]["MP1"]/sum(game_dic[key]["times"])*100))
    print("Player1 looked at the tablet: {:.2f}%".format(game_dic[key]["TP1"]/sum(game_dic[key]["times"])*100))
    print("Player1 looked at somewhere else: {:.2f}%".format(game_dic[key]["SP1"]/sum(game_dic[key]["times"])*100))
    print("Player1 looked at Player0: {:.2f}%".format(game_dic[key]["P1LP0"]/sum(game_dic[key]["times"])*100))
    print("Player1 looked at Player2: {:.2f}%".format(game_dic[key]["P1LP2"]/sum(game_dic[key]["times"])*100))
    print("\n")

    print("Player2")
    print("Player2 looked at the mainscreen: {:.2f}%".format(game_dic[key]["MP2"]/sum(game_dic[key]["times"])*100))
    print("Player2 looked at the tablet: {:.2f}%".format(game_dic[key]["TP2"]/sum(game_dic[key]["times"])*100))
    print("Player2 looked at somewhere else: {:.2f}%".format(game_dic[key]["SP2"]/sum(game_dic[key]["times"])*100))
    print("Player2 looked at Player0: {:.2f}%".format(game_dic[key]["P2LP0"]/sum(game_dic[key]["times"])*100))
    print("Player2 looked at Player1: {:.2f}%".format(game_dic[key]["P2LP1"]/sum(game_dic[key]["times"])*100))
    print("\n")

    print("Mutual gaze between player0 and player1: {:.2f}%".format(game_dic[key]["MGP0P1"]/sum(game_dic[key]["times"])*100))
    print("Mutual gaze between player0 and player2: {:.2f}%".format(game_dic[key]["MGP0P2"]/sum(game_dic[key]["times"])*100))
    print("Mutual gaze between player1 and player2: {:.2f}%".format(game_dic[key]["MGP1P2"]/sum(game_dic[key]["times"])*100))
    print("\n")

    print("Player0 and Player1 looked at Player2: {:.2f}%".format(game_dic[key]["P0P1LP2"]/sum(game_dic[key]["times"])*100))
    print("Player0 and Player2 looked at Player1: {:.2f}%".format(game_dic[key]["P0P2LP1"]/sum(game_dic[key]["times"])*100))
    print("Player1 and Player2 looked at Player0: {:.2f}%".format(game_dic[key]["P1P2LP0"]/sum(game_dic[key]["times"])*100))
    print("\n")

