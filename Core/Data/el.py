import numpy as np

x = np.array([[1, -2],[-2,-1],[-1,-1],[-2,0],[2,0],[0,1],[2,1],[0,2]])

print(np.matmul(x.T,x))

y = np.array([[2, -1], [-1, 0], [0, 0],[-1,2]])
z = np.array([[1, -1], [-2, 0], [1, 0],[-1,1]])

wy = np.matmul(y.T,y)
wz = np.matmul(z.T,z)

print(wy+wz)
print(wy)
print(wz)

x = np.array([[1, 0, -1], [0, 1, 1], [-1, 1, 0], [0, -1, 1], [-1, 0, 1], [1, -1, 0]])
d = np.array([[1/6, 0, 0, 0, 0, 0], [0, 1/6, 0, 0, 0, 0], [0, 0, 1/6, 0, 0, 0], [0, 0, 0, 1/6, 0, 0], [0, 0, 0, 0, 1/6, 0], [0, 0, 0, 0, 0, 1/6]])
print(np.matmul(np.matmul(x.T,d), x))