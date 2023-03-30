import numpy as np
import matplotlib.pyplot as plt
import json 
import random

def parser(path):
    clients = list()
    with open(path) as f:
        for line in f:
            data = line[:-2].split(" ")
            if(len(data) == 7 or len(data) == 5): clients.append(data)
    return clients

def generateRandomSolution(clients):
    vehicles = {"points": [], "cost": []}
    base = (clients[0][1], clients[0][2])
    curentVehicle = [base]
    clients.pop(0)
    currentCap = 0
    random.shuffle(clients)
    for client in clients:
        if currentCap+int(client[5]) > 200:
            curentVehicle.append(base)
            vehicles["points"].append(curentVehicle)
            vehicles["cost"].append(currentCap)
            curentVehicle = [base]
            currentCap = int(client[5])
        curentVehicle.append((client[1], client[2]))
        currentCap += int(client[5])
    return vehicles

if __name__ == "__main__":

    with open('data101.2.json') as f:
        data = json.load(f)

    # clients = parser("data101.vrp")
    # vehicles = generateRandomSolution(clients)
    # costs = [i for i in vehicles["cost"]]


    n = 0
    for vehicle in data[:]:
        capacity = vehicle["Capacity"]
        distance = vehicle["Distance"]
        x = [int(i[0]) for i in vehicle["Points"]]
        y = [int(i[1]) for i in vehicle["Points"]]
        plt.plot(x, y, "o-", linewidth=0.5, markersize=4, label=f"vehicle {n} : {distance}")
        n+=1

    plt.axis('off')
    plt.legend(loc='best')
    plt.show()