import numpy as np
import matplotlib.pyplot as plt
import json
import matplotlib.colors as mcolors

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

    # points1 = [[35, 35],[49, 73],[57, 68],[65, 55],[55, 45],[35, 35]]
    # points2 = [[35, 35],[26, 35],[25, 24],[16, 22],[26, 52],[21, 24],[35, 35]]




    # points = [points1, points2]
    # for points in points[:]:
    #     x = [int(i[0]) for i in points]
    #     y = [int(i[1]) for i in points]
    #     random_color_name = np.random.choice(list(plt.cm.colors.cnames.keys()))
    #     for i in range(len(x)-1):
    #         plt.arrow(x[i], y[i], x[i+1]-x[i], y[i+1]-y[i], head_width=1, head_length=1, length_includes_head=True, color=random_color_name)


    for vehicle in data[:]:
        random_color_name = np.random.choice(list(mcolors.XKCD_COLORS.keys()))
        id = vehicle["Vehicle"]
        capacity = vehicle["Capacity"]
        distance = vehicle["Distance"]
        x = [int(i[0]) for i in vehicle["Points"]]
        y = [int(i[1]) for i in vehicle["Points"]]
        #plt.plot(x, y, "o-", linewidth=0.5, markersize=4, label=f"vehicle {id} : {distance} | {capacity}")
        
        for i in range(len(x)-1):
            if i == 0:
                plt.arrow(x[i], y[i], x[i+1]-x[i], y[i+1]-y[i], head_width=1, head_length=1, length_includes_head=True, color=random_color_name, label=f"vehicle {id} : {distance} | {capacity}")
            else:
                plt.arrow(x[i], y[i], x[i+1]-x[i], y[i+1]-y[i], head_width=1, head_length=1, length_includes_head=True, color=random_color_name)

    plt.axis('off')
    plt.legend(loc='best')
    plt.show()

    