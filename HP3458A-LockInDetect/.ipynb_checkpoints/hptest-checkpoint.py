# -*- coding: utf-8 -*-
"""
Created on Tue Jun 12 13:39:43 2018

@author: manip.batm
"""

import matplotlib.pyplot as plt
import time
import numpy as np

from Gpib import *

fig = plt.figure(figsize=(20,10))
ax = fig.add_subplot(1,1,1)                                                      

x = []
y = []

frequencies = [20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 32, 35, 37, 40, 42, 45, 50, 55, 60, 70, 80, 90, 100, 120, 150, 175, 200, 250, 300, 400, 500, 600, 800, 1000, 1200, 1500, 1750, 2000, 2500, 3000, 4000, 5000, 6000, 8000, 10000, 12000, 15000, 17500, 20000, 25000, 30000, 40000, 50000, 60000, 80000, 100000, 200000, 300000, 400000, 500000, 600000]

dmm = gpib.find("HP3458A")            # defined in /etc/gpib.conf - HP 3458

#dmm = Gpib(0,3)
#dds = Gpib(0,7)

#Set 8904A output to 1.41V and sinewave
#dds.write('PS')
#dds.write('GM0')
#dds.write('FC1OF')
#dds.write('APA1.41VL')
#dds.write('WFASI')

# Switch 3456A to ACV and db log reading
dmm.write('F2')
dmm.write('M9')

time.sleep(0.5)

for f in frequencies:
	#dds.write('FRA' + str(f) + 'HZ')
	#time.sleep(1)
	level = dmm.read(60)
	time.sleep(0.5)

	x.append(f)
	y.append(level)
	print(f, level)

plt.plot(x,y, linewidth = 2)
plt.draw()

plt.xscale('log')
plt.xlabel('Frequency (Hz)')
plt.ylabel('Level (dBm)')

plt.ylim(-20,3)
plt.xlim(0,600000)

major_ticks = np.arange(-18, 3, 3)
minor_ticks = np.arange(-20, 3, 1)
ax.set_yticks(major_ticks)
ax.set_yticks(minor_ticks, minor = True)

ax.grid(b=True, which='major', color='k', linestyle='-')
ax.grid(b=True, which='minor', color='r', linestyle='--')

plt.show()

fig.savefig('frequency.png')
