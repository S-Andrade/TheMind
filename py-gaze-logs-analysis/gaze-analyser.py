import os
import fnmatch
import math
import numpy
from cross_correlation_by_simon_ho import cross_correlation


class GazeBehavior:
	def __init__(self, gazer, target, starting_time, ending_time):
		self.gazer = gazer
		self.target = target
		self.starting_time = starting_time
		self.ending_time = ending_time
		self.duration = ending_time - starting_time

def PrintGBS(gbs):
	for gb in gbs:
		print ">>> " + gb.gazer + "->" + gb.target + " " + str(gb.starting_time) + "-" + str(gb.ending_time)

class CoOccurrenceGazeBehavior:
	def __init__(self, initiator, follower, starting_time, ending_time):
		self.initiator = initiator
		self.follower = follower
		self.starting_time = starting_time
		self.ending_time = ending_time
		self.duration = ending_time - starting_time

def PrintCoGBS(cogbs):
	for cgb in cogbs:
		print ">>>>>> " + cgb.initiator + "->" + cgb.follower + " " + str(cgb.starting_time) + "-" + str(cgb.ending_time)

def AvgDurCoGB(cogbs):
	temp = []
	for gb in cogbs:
		temp.append(gb.duration)
	return numpy.mean(temp)

FRAMERATE = 25

def isNaN(num):
	return num != num

def GetSignalArrayOfGazingAt(gbs, target, initial_timestamp, ending_timestamp):
	array = []
	last_timestamp = initial_timestamp
	for gb in gbs:
		if gb.target == target:
			if gb.starting_time > last_timestamp:
				zero_bits = int(round((gb.starting_time - last_timestamp) * 0.025))
				for i in range(0,zero_bits):
					array.append(0)

			if gb.ending_time > ending_timestamp:
				one_bits = int(round((ending_timestamp - gb.starting_time) * 0.025))
				for i in range(0,one_bits):
					array.append(1)
				last_timestamp = ending_timestamp
			else:
				one_bits = int(round(gb.duration * 0.025))
				for i in range(0,one_bits):
					array.append(1)
				last_timestamp = gb.ending_time
	if last_timestamp < ending_timestamp:
		zero_bits = int(round((ending_timestamp - last_timestamp) * 0.025))
		for i in range(0,zero_bits):
			array.append(0)
	return array

def GazeBehavioursCoOccurrence(gbs1, target1, gbs2, target2):
	cooccurrences = []
	for	bg1 in gbs1:
		if bg1.target == target1:
			for bg2 in gbs2:
				if bg2.target == target2:
					if bg2.starting_time >= bg1.ending_time:
						break
					elif bg2.starting_time >= bg1.starting_time and bg2.ending_time <= bg1.ending_time:
						gb = CoOccurrenceGazeBehavior(bg1.gazer, bg2.gazer, bg2.starting_time, bg2.ending_time)
						cooccurrences.append(gb)
					elif bg2.starting_time >= bg1.starting_time and bg2.starting_time < bg1.ending_time and bg2.ending_time > bg1.ending_time:
						gb = CoOccurrenceGazeBehavior(bg1.gazer, bg2.gazer, bg2.starting_time, bg1.ending_time)
						cooccurrences.append(gb)
					elif bg2.starting_time < bg1.starting_time and bg2.ending_time > bg1.starting_time and bg2.ending_time < bg1.ending_time:
						gb = CoOccurrenceGazeBehavior(bg2.gazer, bg1.gazer, bg1.starting_time, bg2.ending_time)
						cooccurrences.append(gb)
					elif bg2.starting_time < bg1.starting_time and bg2.ending_time > bg1.starting_time and bg2.ending_time > bg1.ending_time:
						gb = CoOccurrenceGazeBehavior(bg2.gazer, bg1.gazer, bg1.starting_time, bg1.ending_time)
						cooccurrences.append(gb)
	return cooccurrences



output_file = open("summary.txt", "w")
for filename in os.listdir("./logs"):
	reading_file = open("./logs/" + filename, "r")
	splited = filename.split(".")
	name = splited[0]
	splited = splited[1].split("at")
	date = splited[0].split("-")
	time = splited[1].split("-")
	condition = ""

	count_gaze = 0
	start = False
	initial_timestamp = 0
	ending_timestamp = 0
	players_gbs = [[],[],[]]
	gazers = {
		"player0" : 0,
		"player1" : 1,
		"player2" : 2
	}
	targets = {
		"player0" : 0,
		"player1" : 1,
		"player2" : 2,
		"mainscreen" : 3,
		"elsewhere" : 4
	}

	for line in reading_file:
		#finish header with condition info
		if condition == "" and len(line.split(" ")) < 4 and len(line.split(" ")[2].split(":")) > 3 and line.split(" ")[2].split(":")[2] == "RoboticPlayer" and line.split(" ")[2].split(":")[3] == "ITabletsGM.ConnectToGM":
			params = line.split(" ")[2].split(":")[5][1:-3].split(";")
			condition = params[1].split("=")[1]
			output_file.write("----------------------------------------------\n")
			output_file.write("| " + name + " " + date[0] + "/" + date[1] + "/" + date[2] + " - " + time[0] + ":" + time[1] + " - " + condition.upper() + " |\n")
			output_file.write("----------------------------------------------\n")
		#start to consider the gazebehaviours after the first startLevel
		if len(line.split(" ")) < 4 and len(line.split(" ")[2].split(":")) > 3 and line.split(" ")[2].split(":")[2] == "GameMaster" and line.split(" ")[2].split(":")[3] == "IGMTablets.StartLevel":
			start = True
		#stop to consider the gazebehaviours after the a game came to an end either gameover or succeeded
		if start and len(line.split(" ")) < 4 and len(line.split(" ")[2].split(":")) > 3 and line.split(" ")[2].split(":")[2] == "GameMaster" and (line.split(" ")[2].split(":")[3] == "IGMTablets.GameOver" or line.split(" ")[2].split(":")[3] == "IGMTablets.GameCompleted"):
			start = False
		
		#set the first timestamp after start boolean became true
		if start and initial_timestamp == 0 and len(line.split(" ")) < 4 and len(line.split(" ")[2].split(":")) > 3 and line.split(" ")[2].split(":")[3] == "IGazeBehaviours.GazeBehaviourStarted":
			params = line.split(" ")[2].split(":")[5][1:-3].split(";")
			gazer = params[0].split("=")[1]
			target = params[1].split("=")[1]
			initial_timestamp = int(params[2].split("=")[1])

		#gazebehaviour that finished within the game
		if start and initial_timestamp != 0 and len(line.split(" ")) < 4 and len(line.split(" ")[2].split(":")) > 3 and line.split(" ")[2].split(":")[3] == "IGazeBehaviours.GazeBehaviourFinished":
			params = line.split(" ")[2].split(":")[5][1:-3].split(";")
			gazer = params[0].split("=")[1]
			target = params[1].split("=")[1]
			timestamp = int(params[2].split("=")[1])
			ending_timestamp = timestamp
			if (len(players_gbs[gazers[gazer]]) == 0):
				gb = GazeBehavior(gazer, target, initial_timestamp, timestamp)
				players_gbs[gazers[gazer]].append(gb)
			else:
				gb = GazeBehavior(gazer, target, players_gbs[gazers[gazer]][-1].ending_time, timestamp)
				players_gbs[gazers[gazer]].append(gb)
			
			count_gaze += 1

	
	output_file.write(">gazer" + "\t" + "target" + "\t" + "num_gazes" + "\t" + "avg_dur\n")	
	for gazer in gazers:
		player_gbs_per_target = []
		for target in targets.keys():
			player_gbs_per_target.append([])
		player_gbs = players_gbs[gazers[gazer]]
		for gb in player_gbs:
			player_gbs_per_target[targets[gb.target]].append(gb.duration)
		for target in targets:
			num_gazes = len(player_gbs_per_target[targets[target]])
			if num_gazes > 0: 
				avg_dur = numpy.mean(player_gbs_per_target[targets[target]])
			else:
				avg_dur = 0
			output_file.write(str(gazer) + "\t" + str(target) + "\t" + str(num_gazes) + "\t" + str(avg_dur) + "\n")


	output_file.write(">cooccurrenceGazeBehavior" + "\t" + "sequence" + "\t" + "num_behav" + "\t" + "avg_dur" + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "player0", players_gbs[gazers["player0"]], "player2")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	output_file.write("Total_MT" + "\t" + "player2_player0" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("MT" + "\t" + "player2_player0" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("MT" + "\t" + "player0_player2" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "player1", players_gbs[gazers["player1"]], "player2")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_MT" + "\t" + "player2_player1" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("MT" + "\t" + "player2_player1" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("MT" + "\t" + "player1_player2" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player0"]], "player1", players_gbs[gazers["player1"]], "player0")
	temp1 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_MT" + "\t" + "player0_player1" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("MT" + "\t" + "player0_player1" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("MT" + "\t" + "player1_player0" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "mainscreen", players_gbs[gazers["player0"]], "mainscreen")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	output_file.write("Total_JA" + "\t" + "player2_player0_mainscreen" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player2_player0_mainscreen" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player0_player2_mainscreen" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "mainscreen", players_gbs[gazers["player1"]], "mainscreen")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_JA" + "\t" + "player2_player1_mainscreen" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player2_player1_mainscreen" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player1_player2_mainscreen" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player0"]], "mainscreen", players_gbs[gazers["player1"]], "mainscreen")
	temp1 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_JA" + "\t" + "player0_player1_mainscreen" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player0_player1_mainscreen" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player1_player0_mainscreen" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "player1", players_gbs[gazers["player0"]], "player1")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	output_file.write("Total_JA" + "\t" + "player2_player0_player1" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player2_player0_player1" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player0_player2_player1" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player2"]], "player0", players_gbs[gazers["player1"]], "player0")
	temp1 = filter(lambda cogb: cogb.initiator == "player2", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_JA" + "\t" + "player2_player1_player0" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player2_player1_player0" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player1_player2_player0" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")
	cogbs = GazeBehavioursCoOccurrence(players_gbs[gazers["player0"]], "player2", players_gbs[gazers["player1"]], "player2")
	temp1 = filter(lambda cogb: cogb.initiator == "player0", cogbs)
	temp2 = filter(lambda cogb: cogb.initiator == "player1", cogbs)
	output_file.write("Total_JA" + "\t" + "player0_player1_player2" + "\t" + str(len(cogbs)) + "\t" + str(AvgDurCoGB(cogbs)) + "\n")
	output_file.write("JA" + "\t" + "player0_player1_player2" + "\t" + str(len(temp1)) + "\t" + str(AvgDurCoGB(temp1)) + "\n")
	output_file.write("JA" + "\t" + "player1_player0_player2" + "\t" + str(len(temp2)) + "\t" + str(AvgDurCoGB(temp2)) + "\n")



	player0_player1 = GetSignalArrayOfGazingAt(players_gbs[gazers["player0"]], "player1", initial_timestamp, ending_timestamp)
	player0_player2 = GetSignalArrayOfGazingAt(players_gbs[gazers["player0"]], "player2", initial_timestamp, ending_timestamp)
	player0_mainscreen = GetSignalArrayOfGazingAt(players_gbs[gazers["player0"]], "mainscreen", initial_timestamp, ending_timestamp)
	player1_player0 = GetSignalArrayOfGazingAt(players_gbs[gazers["player1"]], "player0", initial_timestamp, ending_timestamp)
	player1_player2 = GetSignalArrayOfGazingAt(players_gbs[gazers["player1"]], "player2", initial_timestamp, ending_timestamp)
	player1_mainscreen = GetSignalArrayOfGazingAt(players_gbs[gazers["player1"]], "mainscreen", initial_timestamp, ending_timestamp)
	player2_player0 = GetSignalArrayOfGazingAt(players_gbs[gazers["player2"]], "player0", initial_timestamp, ending_timestamp)
	player2_player1 = GetSignalArrayOfGazingAt(players_gbs[gazers["player2"]], "player1", initial_timestamp, ending_timestamp)
	player2_mainscreen = GetSignalArrayOfGazingAt(players_gbs[gazers["player2"]], "mainscreen", initial_timestamp, ending_timestamp)
	
	
	output_file.write(">GB" + "\t" + "gazers_pair" + "\t" + "max_R" + "\t" + "max_lag_adj\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_player0, player0_player2)
	output_file.write("MUTUAL_GAZE" + "\t" + "player2_player0" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_player1, player1_player2)
	output_file.write("MUTUAL_GAZE" + "\t" + "player2_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player0_player1, player1_player0)
	output_file.write("MUTUAL_GAZE" + "\t" + "player0_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_mainscreen, player0_mainscreen)
	output_file.write("JA_mainscreen" + "\t" + "player2_player0" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_mainscreen, player1_mainscreen)
	output_file.write("JA_mainscreen" + "\t" + "player2_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player0_mainscreen, player1_mainscreen)
	output_file.write("JA_mainscreen" + "\t" + "player0_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_player1, player0_player1)
	output_file.write("JA_player1" + "\t" + "player2_player0" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player2_player0, player1_player0)
	output_file.write("JA_player0" + "\t" + "player2_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")	
	max_R, max_lag_adj, zero_R, norm_array = cross_correlation(player0_player2, player1_player2)
	output_file.write("JA_player2" + "\t" + "player0_player1" + "\t" + str(max_R) + "\t" + str(max_lag_adj) + "\n")