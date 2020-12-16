# !bin/bash

landmarks=$(python3 python1.py)

feature=$(./main.o $landmarks)

output=$(python3 python2.py $feature)
echo $output