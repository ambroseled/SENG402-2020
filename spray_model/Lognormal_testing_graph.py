"""
Test for log normal distribution code
Jack Taylor
Last Updated: 07/05/20
"""

import matplotlib.pyplot as plt
import numpy as np


def function1():
    mu = np.log(1110e-6) # mean
    sigma = 0.15 # standard deviation
    
    s = np.random.lognormal(mu, sigma, 1000000)
     
    count, bins, ignored = plt.hist(s, 100, density=True, align='mid')
   
    x = np.linspace(min(bins), max(bins), 1000)
    pdf = (np.exp(-(np.log(x) - mu)**2 / (2 * sigma**2))
           / (x * sigma * np.sqrt(2 * np.pi)))

    plt.plot(x, pdf, linewidth=2, color='r')
    plt.axis('tight')
    # plt.show()
    
    below = 0
    
    
    # Sizes (in microns) used by Scion to test the Droplet Spectra
    nums = [18, 
            22,
            26,
            30,
            36,
            44,
            52,
            62,
            74,
            86,
            100,
            120,
            150,
            180,
            210,
            250,
            300,
            360,
            420,
            500,
            600,
            720,
            860,
            1020,
            1220,
            1460,
            1740,
            2060,
            2460,
            2940,
            3500]
    
    
    # Volume calcs
    v = []
    for number in s:
        v += [(1/6) * np.pi * (number**3)]   # All the volumes of the spray
        
    total_volume = sum(v)
    
    for num in nums:
        volsum = 0;
        volume = (1/6) * np.pi * (num*1.0e-6)**3
        for number in v: 
            if number <= volume:                    # If it is below threshold
                volsum += number                    # Increment counter  
                
        percent = (volsum/total_volume) * 100       # Calculating percentage of droplets (volume based)
        print(percent)    
    
    
    # Percentage of dropl numbers
    # Iterate through all the values given
    for num in nums:
        value = num*1.0e-6                  # Converting to m
        
        below = 0                           # Resetting the count 
        for number in s:                    # For each generated value in s
            if number <= value:             # If it is below threshold
                below += 1                  # Increment counter
        
        percent = (below/1000000) * 100      # Calculating percentage of droplets in s
        #print(percent)
    
    print(s)
    
    
    
def main():
    function1()
    
main()