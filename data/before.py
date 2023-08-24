
import sys

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



i = 0
lookp0 = []
lookp1 = []
lookp2 = []

while (i < video_size):
    if game[i] == "play-card-player0":
        if game[i-1] != "play-card-player0":
            lookp0 += [[player0[i-1000],player1[i-1000],player2[i-1000]]]

    if game[i] == "play-card-player1":
        if game[i-1] != "play-card-player1":
            lookp1 += [[player0[i-1000],player1[i-1000],player2[i-1000]]]    
        
    if game[i] == "play-card-player2":
        if game[i-1] != "play-card-player2":
            lookp2 += [[player0[i-1000],player1[i-1000],player2[i-1000]]]

    i += 1





print("play-card-player0")

pp0p0 = []
pp0p1 = []
pp0p2 = []
for l in lookp0:
    pp0p0 += [l[0]]
    pp0p1 += [l[1]]
    pp0p2 += [l[2]]

dpp0p0 = {}
dpp0p1 = {}
dpp0p2 = {}

for p in pp0p0:
    if p in dpp0p0.keys():
        dpp0p0[p] += 1
    else:
        dpp0p0[p] = 1 

print("player0")
print(dpp0p0)

for p in pp0p1:
    if p in dpp0p1.keys():
        dpp0p1[p] += 1
    else:
        dpp0p1[p] = 1 

print("player1")
print(dpp0p1)

for p in pp0p2:
    if p in dpp0p2.keys():
        dpp0p2[p] += 1
    else:
        dpp0p2[p] = 1 

print("player2")
print(dpp0p2)



print("\nplay-card-player1")

pp1p0 = []
pp1p1 = []
pp1p2 = []
for l in lookp1:
    pp1p0 += [l[0]]
    pp1p1 += [l[1]]
    pp1p2 += [l[2]]

dpp1p0 = {}
dpp1p1 = {}
dpp1p2 = {}

for p in pp1p0:
    if p in dpp1p0.keys():
        dpp1p0[p] += 1
    else:
        dpp1p0[p] = 1 

print("player0")
print(dpp1p0)

for p in pp1p1:
    if p in dpp1p1.keys():
        dpp1p1[p] += 1
    else:
        dpp1p1[p] = 1 

print("player1")
print(dpp1p1)

for p in pp1p2:
    if p in dpp1p2.keys():
        dpp1p2[p] += 1
    else:
        dpp1p2[p] = 1 

print("player2")
print(dpp1p2)


print("\nplay-card-player2")

pp2p0 = []
pp2p1 = []
pp2p2 = []
for l in lookp2:
    pp2p0 += [l[0]]
    pp2p1 += [l[1]]
    pp2p2 += [l[2]]

dpp2p0 = {}
dpp2p1 = {}
dpp2p2 = {}

for p in pp2p0:
    if p in dpp2p0.keys():
        dpp2p0[p] += 1
    else:
        dpp2p0[p] = 1 

print("player0")
print(dpp2p0)

for p in pp2p1:
    if p in dpp2p1.keys():
        dpp2p1[p] += 1
    else:
        dpp2p1[p] = 1 

print("player1")
print(dpp2p1)

for p in pp2p2:
    if p in dpp2p2.keys():
        dpp2p2[p] += 1
    else:
        dpp2p2[p] = 1 

print("player2")
print(dpp2p2)
