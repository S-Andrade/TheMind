
import sys
import statistics

# read and organize the file
player0 = []
player1 = []
player2 = []
game = []


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
    
video_size = 653000

player0 = player0[:video_size]
player1 = player1[:video_size]
player2 = player2[:video_size]
game = game[:video_size]


datap0p1 = []
a0p0p1 = 0
a1p0p1 = 0
mgp0p1 = 0
d0p0p1 = 0
d1p0p1 = 0
adp0p1 = False

datap0p2 = []
a0p0p2 = 0
a2p0p2 = 0
mgp0p2 = 0
d0p0p2 = 0
d2p0p2 = 0
adp0p2 = False

datap1p2 = []
a1p1p2 = 0
a2p1p2 = 0
mgp1p2 = 0
d1p1p2 = 0
d2p1p2 = 0
adp1p2 = False



i = 0
while (i < video_size):
    if (game[i] == "waiting-for-card"):
        
        if player0[i] == 'look-at-player1' or player1[i] == 'look-at-player0': 
        
            if (player0[i+1] != 'look-at-player1' and player1[i+1] != 'look-at-player0') or  ( 
                    not adp0p1 and (
                    (player0[i] == 'look-at-player1' and player0[i+1] != 'look-at-player1' and player1[i] != 'look-at-player0' and player1[i+1] == 'look-at-player0')
                    or
                    (player0[i] != 'look-at-player1' and player0[i+1] == 'look-at-player1' and player1[i] == 'look-at-player0' and player1[i+1] != 'look-at-player0'))):

                datap0p1 += [[a0p0p1, a1p0p1, mgp0p1, d0p0p1, d1p0p1]]
                a0p0p1 = 0
                a1p0p1 = 0
                mgp0p1 = 0
                d0p0p1 = 0
                d1p0p1 = 0
                adp0p1 = False

            elif player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
                mgp0p1 += 1
                adp0p1 = True

            elif player0[i] == 'look-at-player1' and player1[i] != 'look-at-player0' and not adp0p1:
                a0p0p1 += 1

            elif player0[i] != 'look-at-player1' and player1[i] == 'look-at-player0' and not adp0p1:
                a1p0p1 += 1

            elif player0[i] == 'look-at-player1' and player1[i] != 'look-at-player0' and adp0p1:
                d0p0p1 += 1
            
            elif player0[i] != 'look-at-player1' and player1[i] == 'look-at-player0' and adp0p1:
                d1p0p1 += 1

        if player0[i] == 'look-at-player2' or player2[i] == 'look-at-player0': 
        
            if (player0[i+1] != 'look-at-player2' and player2[i+1] != 'look-at-player0') or  ( 
                    not adp0p2 and (
                    (player0[i] == 'look-at-player2' and player0[i+1] != 'look-at-player2' and player2[i] != 'look-at-player0' and player2[i+1] == 'look-at-player0')
                    or
                    (player0[i] != 'look-at-player2' and player0[i+1] == 'look-at-player2' and player2[i] == 'look-at-player0' and player2[i+1] != 'look-at-player0'))):

                datap0p2 += [[a0p0p2, a2p0p2, mgp0p2, d0p0p2, d2p0p2]]
                a0p0p2 = 0
                a2p0p2 = 0
                mgp0p2 = 0
                d0p0p2 = 0
                d2p0p2 = 0
                adp0p2 = False

            elif player0[i] == 'look-at-player2' and player2[i] == 'look-at-player0':
                mgp0p2 += 1
                adp0p2 = True

            elif player0[i] == 'look-at-player2' and player2[i] != 'look-at-player0' and not adp0p2:
                a0p0p2 += 1

            elif player0[i] != 'look-at-player2' and player2[i] == 'look-at-player0' and not adp0p2:
                a2p0p2 += 1

            elif player0[i] == 'look-at-player2' and player2[i] != 'look-at-player0' and adp0p2:
                d0p0p2 += 1
            
            elif player0[i] != 'look-at-player2' and player2[i] == 'look-at-player0' and adp0p2:
                d2p0p2 += 1

        if player1[i] == 'look-at-player2' or player2[i] == 'look-at-player1': 
        
            if (player1[i+1] != 'look-at-player2' and player2[i+1] != 'look-at-player1') or  ( 
                    not adp1p2 and (
                    (player1[i] == 'look-at-player2' and player1[i+1] != 'look-at-player2' and player2[i] != 'look-at-player1' and player2[i+1] == 'look-at-player1')
                    or
                    (player1[i] != 'look-at-player2' and player1[i+1] == 'look-at-player2' and player2[i] == 'look-at-player1' and player2[i+1] != 'look-at-player1'))):

                datap1p2 += [[a1p1p2, a2p1p2, mgp1p2, d1p1p2, d2p1p2]]
                a1p1p2 = 0
                a2p1p2 = 0
                mgp1p2 = 0
                d1p1p2 = 0
                d2p1p2 = 0
                adp1p2 = False

            elif player1[i] == 'look-at-player2' and player2[i] == 'look-at-player1':
                mgp1p2 += 1
                adp1p2 = True

            elif player1[i] == 'look-at-player2' and player2[i] != 'look-at-player1' and not adp1p2:
                a1p1p2 += 1

            elif player1[i] != 'look-at-player2' and player2[i] == 'look-at-player1' and not adp1p2:
                a2p1p2 += 1

            elif player1[i] == 'look-at-player2' and player2[i] != 'look-at-player1' and adp1p2:
                d1p1p2 += 1
            
            elif player1[i] != 'look-at-player2' and player2[i] == 'look-at-player1' and adp1p2:
                d2p1p2 += 1
           
    i += 1


data = datap0p1 + datap0p2 + datap1p2

antes = []
depois = []
antestotal = []
depoistotal = []
gaze = []

for d in data:
    if d[2] != 0:
        a = [d[0]+d[1], d[2], d[3]+d[4]]
        
        antes += [a[0]]
        depois += [a[2]]
        antestotal += [a[0] + a[1]]
        depoistotal += [a[1] + a[2]]
        gaze += [a[1]]


listas = [antes, depois, antestotal, depoistotal, gaze]
nomes = ["antes", "depois", "antestotal", "depoistotal", "gaze"]

for l , n in zip(listas,nomes):
    print(n)
    print("d - Media total" , sum(l)/len(l)/1000)
    print("d - Mediana", statistics.median(l)/1000)
    print("d - Max total" , max(l)/1000)
    print("d - Min total" , min(l)/1000)

print(len(gaze)/len(data))