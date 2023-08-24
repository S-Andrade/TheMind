
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


timelist = []

mutual01 = []
mutual02 = []
mutual12 = []

lookat0 = []
lookat1 = []
lookat2 = []

t = 0
m01 = 0
m02 = 0
m12 = 0

l0 = 0
l1 = 0
l2 = 0

mutual = []
lookat = []

i = 0
while (i < video_size):
    if (game[i] == "waiting-for-card"):
        if game[i+1] != "waiting-for-card":
            timelist += [t]
            mutual += [[mutual01, mutual02, mutual12]]
            t = 0
            mutual01 = []
            mutual02 = []
            mutual12 = []

            lookat += [[lookat0,lookat1,lookat2]]
            lookat0 = []
            lookat1 = []
            lookat2 = []

        else:
            t += 1
            
            if player0[i] == 'look-at-player1' and player1[i] == 'look-at-player0':
                if player0[i+1] != 'look-at-player1' or player1[i+1] != 'look-at-player0':
                    mutual01 += [m01]
                    m01 = 0
                else:
                    m01+=1
            if player0[i] == 'look-at-player2' and player2[i] == 'look-at-player0':
                if player0[i+1] != 'look-at-player2' or player2[i+1] != 'look-at-player0':
                    mutual02 += [m02]
                    m02 = 0
                else:
                    m02+=1
            if player1[i] == 'look-at-player2' and player2[i] == 'look-at-player1':
                if player1[i+1] != 'look-at-player2' or player2[i+1] != 'look-at-player1':
                    mutual12 += [m12]
                    m12 = 0
                else:
                    m12+=1            
           
            
            if player0[i] == 'look-at-player1' or player0[i] == 'look-at-player2':
                if (player0[i] == 'look-at-player1' and player0[i+1] != 'look-at-player1') or (player0[i] == 'look-at-player2' and player0[i+1] != 'look-at-player2'):
                    lookat0 += [l0]
                    l0 = 0
                else:
                    l0 += 1
            if player1[i] == 'look-at-player0' or player1[i] == 'look-at-player2':
                if (player1[i] == 'look-at-player0' and player1[i+1] != 'look-at-player0') or (player1[i] == 'look-at-player2' and player1[i+1] != 'look-at-player2'):
                    lookat1 += [l1]
                    l1 = 0
                else:
                    l1 += 1
            if player2[i] == 'look-at-player0' or player2[i] == 'look-at-player1':
                if (player2[i] == 'look-at-player0' and player2[i+1] != 'look-at-player0') or (player2[i] == 'look-at-player1' and player2[i+1] != 'look-at-player1'):
                    lookat2 += [l2]
                    l2 = 0
                else:
                    l2 += 1
            
    i += 1


#print([x/1000 for x in timelist])
print("t - Freqencia: " , sum(timelist)/video_size*100)
print("t - Media" , sum(timelist)/len(timelist)/1000)
print("t - Max" , max(timelist)/1000)
print("t - Min" , min(timelist)/1000)
print("\n")

times = [[],[],[]]

for m in mutual:
    times[0] += [len(m[0])]
    times[1] += [len(m[1])]
    times[2] += [len(m[2])]

for t in times:
    print("Media" , sum(t)/len(t))
    print("Max" , max(t))
    print("Min" , min(t))
    print("\n")

totalm = []
for m in mutual:
    totalm += [len(m[0]) + len(m[1]) + len(m[2])]

print("Media total" , sum(totalm)/len(totalm))
print("Max total" , max(totalm))
print("Min total" , min(totalm))
print("\n")

print("/////////")

looks = [[],[],[]]

for m in lookat:
    looks[0] += [len(m[0])]
    looks[1] += [len(m[1])]
    looks[2] += [len(m[2])]

for t in looks:
    print("Media" , sum(t)/len(t))
    print("Max" , max(t))
    print("Min" , min(t))
    print("\n")

looksm = []
for m in lookat:
    looksm += [len(m[0]) + len(m[1]) + len(m[2])]

print("Media total" , sum(looksm)/len(looksm))
print("Max total" , max(looksm))
print("Min total" , min(looksm))
print("\n")

