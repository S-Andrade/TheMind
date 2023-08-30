
import sys
import statistics
import numpy as np
from scipy.stats import norm

import matplotlib.pyplot as plt
import seaborn as sns

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


lookat01 = []
lookat02 = []
lookat10 = []
lookat12 = []
lookat20 = []
lookat21 = []

l01 = 0
l02 = 0
l10 = 0
l12 = 0
l20 = 0
l21 = 0

lookat = []

i = 0
while (i < video_size):
    if (game[i] == "waiting-for-card"):
        if game[i+1] != "waiting-for-card":
            lookat += [[lookat01, lookat02, lookat10, lookat12, lookat20, lookat21]]
            lookat01 = []
            lookat02 = []
            lookat10 = []
            lookat12 = []
            lookat20 = []
            lookat21 = []

        else:
            if player0[i] == 'look-at-player1':
                if player0[i+1] != 'look-at-player1':
                    lookat01 += [l01]
                    l01 = 0
                else:
                    l01 += 1

            if player0[i] == 'look-at-player2':
                if player0[i+1] != 'look-at-player2':
                    lookat02 += [l02]
                    l02 = 0
                else:
                    l02 += 1

            if player1[i] == 'look-at-player0':
                if player1[i+1] != 'look-at-player0':
                    lookat10 += [l10]
                    l10 = 0
                else:
                    l10 += 1
            
            if player1[i] == 'look-at-player2':
                if player1[i+1] != 'look-at-player2':
                    lookat12 += [l12]
                    l12 = 0
                else:
                    l12 += 1
           
            if player2[i] == 'look-at-player0':
                if player2[i+1] != 'look-at-player0':
                    lookat20 += [l20]
                    l20 = 0
                else:
                    l20 += 1
            
            if player2[i] == 'look-at-player1':
                if player2[i+1] != 'look-at-player1':
                    lookat21 += [l21]
                    l21 = 0
                else:
                    l21 += 1
            
    i += 1



times = [[],[],[],[],[],[]]
duration = [[],[],[],[],[],[]]

for l in lookat:
    times[0] += [len(l[0])]
    times[1] += [len(l[1])]
    times[2] += [len(l[2])]
    times[3] += [len(l[3])]
    times[4] += [len(l[4])]
    times[5] += [len(l[5])]


    duration[0] += l[0]
    duration[1] += l[1]
    duration[2] += l[2]
    duration[3] += l[3]
    duration[4] += l[4]
    duration[5] += l[5]

"""
for t in times:
    print("t - Media" , sum(t)/len(t))
    print("t - Mediana", statistics.median(t))
    print("t - Max" , max(t))
    print("t - Min" , min(t))
    print("\n")
"""

totalt = []
for l in lookat:
    totalt += [len(l[0]) + len(l[1]) + len(l[2]) + len(l[3]) + len(l[4]) + len(l[5])]

print("t - Media total" , sum(totalt)/len(totalt))
print("t - Mediana", statistics.median(totalt))
print("t - Max total" , max(totalt))
print("t - Min total" , min(totalt))
print("\n")

"""
for d in duration:
    print("d - Media" , sum(d)/len(d)/1000)
    print("d - Mediana", statistics.median(d)/1000)
    print("d - Max" , max(d)/1000)
    print("d - Min" , min(d)/1000)
    print("\n")
"""

totald = []
for l in lookat:
    totald += l[0] + l[1] + l[2] + l[3] + l[4] + l[5]

print("d - Media total" , sum(totald)/len(totald)/1000)
print("d - Mediana", statistics.median(totald)/1000)
print("d - Max total" , max(totald)/1000)
print("d - Min total" , min(totald)/1000)
print("\n")


##########################################

domain = np.linspace(-244, 244, 1000) 
means = []
std_values = []
nomes = ["l01", "l02", "l10", "l12", "l20", "l21", "all"]

for t in times:
    means += [np.mean(t)]
    std_values += [np.std(t)]

means += [np.mean(totalt)]
std_values += [np.std(totalt)]

plt.figure(figsize=(16, 9))
for mu, std , n in zip(means, std_values, nomes):
    probabilities = norm.pdf(domain, mu, std)
    plt.plot(domain, probabilities, label=f"{n}\n$\mu={mu}$\n$\sigma={std}$\n")

plt.title("Vezes")
plt.legend()
plt.xlabel("Value")
plt.ylabel("Probability")
plt.show()


domain = np.linspace(-8265, 8265, 1000)

means = []
std_values = []
nomes = ["l01", "l02", "l10", "l12", "l20", "l21", "all"]

for d in duration:
    means += [np.mean(d)]
    std_values += [np.std(d)]

means += [np.mean(totald)]
std_values += [np.std(totald)]

plt.figure(figsize=(16, 9))
for mu, std , n in zip(means, std_values, nomes):
    probabilities = norm.pdf(domain, mu, std)
    plt.plot(domain, probabilities, label=f"{n}\n$\mu={mu}$\n$\sigma={std}$\n")

plt.title("Duração")
plt.legend()
plt.xlabel("Value")
plt.ylabel("Probability")
plt.show()