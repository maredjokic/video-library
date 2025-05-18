from __future__ import division
import detect
from detect import main
import os
os.environ['CUDA_LAUNCH_BLOCKING'] = "1"
import time
import torch
import torch.nn as nn
from torch.autograd import Variable
import numpy as np
import cv2 
from util import *
import argparse
import io
import os.path as osp
from darknet import Darknet
from preprocess import prep_image, inp_to_image
import pandas as pd
import random 
import pickle as pkl
import itertools
import sys
from collections import Counter
import json262
import json
import shutil
from detect import main

path = osp.dirname(osp.abspath(__file__))
def arg_parse():
    parser = argparse.ArgumentParser(description='YOLO v3 Detection Module')

    parser.add_argument("--videoid", dest = 'videoid' , help = 
                        "Hash of the video",
                        default = "000", type = str)
    parser.add_argument("--video", dest = 'video', help =
                        "Path to the video",
                        default = "SoniaJamesBreakfast.mov", type = str)
    parser.add_argument("--images", dest = 'images', help = 
                        "Image / Directory containing images to perform detection upon",
                        default = "", type = str)
    parser.add_argument("--det", dest = 'det', help = 
                        "Image / Directory to store detections to",
                        default = osp.join(path, "det"), type = str)
    parser.add_argument("--bs", dest = "bs", help = "Batch size", default = 1)
    parser.add_argument("--confidence", dest = "confidence", help = "Object Confidence to filter predictions", default = 0.6)
    parser.add_argument("--nms_thresh", dest = "nms_thresh", help = "NMS Threshhold", default = 0.4)
    parser.add_argument("--cfg", dest = 'cfgfile', help = 
                        "Config file",
                        default = osp.join(path, 'cfg', 'yolo3-openimages.cfg'), type = str)
    parser.add_argument("--weights", dest = 'weights', help = 
                        "weights",
                        default = osp.join(path, 'yolov3-openimages.weights'), type = str)
    parser.add_argument("--reso", dest = 'reso', help = 
                        "Input resolution of the network. Increase to increase accuracy. Decrease to increase speed",
                        default = "416", type = str)
    
    return parser.parse_args()

if __name__ ==  '__main__':
    args = arg_parse()
    video = args.video
    videoid = args.videoid
    images = args.images
    bs = int(args.bs)
    det = args.det
    cfg = args.cfgfile
    weights = args.weights
    reso = args.reso
    confidence = float(args.confidence)
    nms_thresh = float(args.nms_thresh)
    print(detect.main(videoid,video,bs,confidence,nms_thresh))
