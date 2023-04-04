import numpy as np
import matplotlib.pyplot as plt
import json

def parser(path):
    clients = list()
    with open(path) as f:
        for line in f:
            data = line[:-2].split(" ")
            if(len(data) == 7 or len(data) == 5): clients.append(data)
    return clients

if __name__ == "__main__":

    with open('data101.2.json') as f:
        data = json.load(f)

    nbClient = sum([len(vehicle["Points"]) for vehicle in data]) - sum([2 for vehicle in data])
    print(nbClient)
    print(data[1]["Points"])
    for vehicle in data[:]:
        id = vehicle["Vehicle"]
        capacity = vehicle["Capacity"]
        distance = vehicle["Distance"]
        x = [int(i[0]) for i in vehicle["Points"]]
        y = [int(i[1]) for i in vehicle["Points"]]
        plt.plot(x, y, "o-", linewidth=0.5, markersize=4, label=f"vehicle {id} : {distance}")

    plt.axis('off')
    plt.legend(loc='best')
    plt.show()

    