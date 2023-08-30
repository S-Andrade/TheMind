
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


mutual01 = []
mutual02 = []
mutual12 = []

m01 = 0
m02 = 0
m12 = 0

mutual = []

i = 0
while (i < video_size):
    if (game[i] == "waiting-for-card"):
        if game[i+1] != "waiting-for-card":
            mutual += [[mutual01, mutual02, mutual12]]
            mutual01 = []
            mutual02 = []
            mutual12 = []

          

        else:
            
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
           
            
    i += 1

times = [[],[],[]]
duration = [[],[],[]]

for m in mutual:
    times[0] += [len(m[0])]
    times[1] += [len(m[1])]
    times[2] += [len(m[2])]

    duration[0] += m[0]
    duration[1] += m[1]
    duration[2] += m[2]

"""
for t in times:
    print(t)
    print("t - Media" , sum(t)/len(t))
    print("t - Mediana", statistics.median(t))
    print("t - Max" , max(t))
    print("t - Min" , min(t))
    print("\n")
"""
totalm = []
for m in mutual:
    totalm += [len(m[0]) + len(m[1]) + len(m[2])]

print("t - Media total" , sum(totalm)/len(totalm))
print("t - Mediana", statistics.median(totalm))
print("t - Max total" , max(totalm))
print("t - Min total" , min(totalm))
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
for m in mutual:
    totald += m[0] + m[1] + m[2]

print("d - Media total" , sum(totald)/len(totald)/1000)
print("d - Mediana", statistics.median(totald)/1000)
print("d - Max total" , max(totald)/1000)
print("d - Min total" , min(totald)/1000)
print("\n")



#####################

domain = np.linspace(-60, 60, 1000) 
means = []
std_values = []
nomes = ["p0p1", "p0p2", "p1p2", "all"]

for t in times:
    means += [np.mean(t)]
    std_values += [np.std(t)]

means += [np.mean(totalm)]
std_values += [np.std(totalm)]

plt.figure(figsize=(16, 9))
for mu, std , n in zip(means, std_values, nomes):
    probabilities = norm.pdf(domain, mu, std)
    plt.plot(domain, probabilities, label=f"{n}\n$\mu={mu}$\n$\sigma={std}$\n")

plt.title("Vezes")
plt.legend()
plt.xlabel("Value")
plt.ylabel("Probability")
plt.show()


domain = np.linspace(-2353, 2353, 1000)

means = []
std_values = []
nomes = ["p0p1", "p0p2", "p1p2", "all"]

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