import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.cluster import KMeans
data = pd.read_csv('color data.csv')
print(data)
#data.plot(kind = "scatter", x='R', y='G', c='B')
#plt.show()
kmean = KMeans(n_clusters=20)
cls = kmean.fit_predict(data)
plt.subplot(1,2,1)
plt.xlabel('R')
plt.ylabel('G')
plt.scatter(data.values[:,0],data.values[:,1],c=cls)
plt.subplot(1,2,2)
plt.xlabel('R')
plt.ylabel('B')
plt.scatter(data.values[:,0],data.values[:,2],c=cls)


print(cls)
print(kmean.cluster_centers_)
file = 'palette.txt'
f = open(file, 'w')

for color in kmean.cluster_centers_:
    s = ''
    for c in color:
        s = s + str(int(c)) + ' '
    f.write(s.rstrip(' ')+'\n')
