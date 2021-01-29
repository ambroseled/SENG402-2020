import random
import sys

if __name__ == '__main__':
    step = int(sys.argv[1])
    num = random.randrange(0, 100, step)
    if num % 2 == 0:
        print(1)
    else:
        print(0)
