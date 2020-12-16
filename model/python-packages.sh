#!/bin/bash
apt-get install -y python3
apt install -y python3-pip 
python3 -m pip install --upgrade pip 
wget https://github.com/Kitware/CMake/releases/download/v3.19.1/cmake-3.19.1.tar.gz
tar -zxvf cmake-3.19.1.tar.gz 
cd cmake-3.19.1 && ./bootstrap && make && make install
DEBIAN_FRONTEND=noninteractive apt-get install -yq python3-opencv
python3 -m pip install dlib
python3 -m pip install scikit-image
python3 -m pip install tensorflow==1.15.0
python3 -m pip install keras==2.4.0
python3 -m pip install pandas
python3 -m pip install h5py==2.10.0