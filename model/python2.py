#!/usr/bin/env python
# coding: utf-8

import tensorflow as tf
from tensorflow import keras
from keras.models import load_model
import numpy as np
import pandas as pd
import sys

#input : FromCPP (type : string)
arguments = sys.argv

floatStr = list(map(float, arguments[1:]))

# input string From CPP

df = pd.DataFrame(floatStr).T
X = df
keras.backend.set_floatx('float64')
LoadModel = load_model('model.h5') #model dic
prediction = LoadModel.predict(X)
result = int(np.argmax(prediction))

rate = [df[24],df[25],df[26],df[27],df[28]]
sum = 0
for i in range(0,5) :
    sum = sum + floatStr[i+24]
score = (sum / 5) * 100

output = {"isSymmetry": result, "score": score, "eye": floatStr[24], "nose": floatStr[25], "mouth": floatStr[26], "jaw": floatStr[27], "face": floatStr[28],}
print(output)

# output a (asy(1/2), score, eye, nose, mouth, jaw, face)
# if asy == 1 ---> Asymmetry 
#else asy == 2 ---> symmetry

