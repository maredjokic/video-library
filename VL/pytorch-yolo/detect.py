from __future__ import division
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
import glob
from pathlib import Path

path = osp.dirname(osp.abspath(__file__))
def removeDuplicates(inputLine): 
    inputLine = inputLine.rstrip("\n").split(",") 
    UniqW = Counter(inputLine) 
    outputLine = ",".join(UniqW.keys())
    return outputLine
  
def iterateLines(inputFilePath):
    tagDict = dict()
    tags = []
    jsonString = ""
    i = 0
    with open(inputFilePath, "r") as inputFile:
        for line in inputFile.readlines():
            outputLine = removeDuplicates(line)
            values = outputLine.split(",")
            for value in values:
                if value != '':
                    if value in tagDict.keys():
                        tagDict[value] = (tagDict[value][0] , tagDict[value][1] + 1)
                    else:
                        tagDict[value] = [i , i + 1]
            keys = []
                
            for key in tagDict:
                if tagDict[key][1] != i + 1:
                    tags.append({ "name": f"{key}", "interval": tagDict[key]})
                    keys.append(key)
            for key in keys:
                tagDict.pop(key)
            i = i + 1
        for key in tagDict:
            tags.append({ "name": key, "interval": tagDict[key] })
        jsonString = json.dumps(tags)
        inputFile.close()
        os.remove(inputFilePath)
        #print(jsonString)
        return(jsonString)

def extractImages(pathIn, pathOut):
    count = 1
    vidcap = cv2.VideoCapture(pathIn)
    success,image = vidcap.read()
    success = True
    while success:
      vidcap.set(cv2.CAP_PROP_POS_MSEC,(count*1000))
      cv2.imwrite( pathOut + "/%d.jpg" % count, image)  
      success,image = vidcap.read()
      count = count + 1

class test_net(nn.Module):
    def __init__(self, num_layers, input_size):
        super(test_net, self).__init__()
        self.num_layers= num_layers
        self.linear_1 = nn.Linear(input_size, 5)
        self.middle = nn.ModuleList([nn.Linear(5,5) for x in range(num_layers)])
        self.output = nn.Linear(5,2)
    
    def forward(self, x):
        x = x.view(-1)
        fwd = nn.Sequential(self.linear_1, *self.middle, self.output)
        return fwd(x)
        
def get_test_input(input_dim, CUDA):
    img = cv2.imread(osp.join(path, "dog-cycle-car.png"))
    img = cv2.resize(img, (input_dim, input_dim)) 
    img_ =  img[:,:,::-1].transpose((2,0,1))
    img_ = img_[np.newaxis,:,:,:]/255.0
    img_ = torch.from_numpy(img_).float()
    img_ = Variable(img_)
    
    if CUDA:
        img_ = img_.cuda()
    return img_

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

def main(videoid,video,batch_size = None,confidence = None,nms_thresh = None):
        #args = arg_parse()
        #video = args.video
        #videoid = args.videoid
        #det = args.det
        det = osp.join(path, "det")
        video.replace("\\", "\\\\")
        scales = 1,2,3
        if batch_size == None:
            batch_size = 1
        if confidence == None:
            confidence = 0.2
        if nms_thresh == None:
            nms_thresh = 0.4
        weights = osp.join(path,'yolov3-openimages.weights')
        cfgfile = osp.join(path,'cfg','yolo3-openimages.cfg')
        reso = 416
        #cfgfile = args.cfgfile
        #weights = args.weights
        #reso = args.reso
        for filename in path:
            if filename.startswith("output"):
                os.remove(os.path.join(path,filename))
        
        if not osp.exists(osp.join(path,"imgs")):
            os.mkdir(osp.join(path,"imgs"))
        else:
            for file_obj in os.listdir(osp.join(path,"imgs")):
                shutil.rmtree(osp.join(path,"imgs",file_obj))
        
        #shutil.rmtree(osp.join(path,"imgs"))
        #os.mkdir(osp.join(path,"imgs"))

        if not osp.exists(osp.join(path,"imgs",videoid)):
            os.mkdir(osp.join(path,"imgs",videoid))
        

        extractImages(video, osp.join(path,"imgs",videoid))
        images = osp.join(path, 'imgs', videoid) 
        #batch_size = int(args.bs)
        #confidence = float(args.confidence)
        #nms_thesh = float(args.nms_thresh)
        start = 0
        CUDA = torch.cuda.is_available()
        num_classes = 80
        classes = load_classes(osp.join(path, 'data', 'openimages.names')) 
        
        #Set up the neural network
        model = Darknet(cfgfile)
        model.load_weights(weights)
        model.net_info["height"] = reso
        inp_dim = int(model.net_info["height"])
        assert inp_dim % 32 == 0 
        assert inp_dim > 32

        if CUDA:
            model.cuda()
        model.eval()
        
        videoid_string = str(videoid)
        read_dir = time.time()
        output_file = 'output_' + videoid + '.txt'
        output_path = osp.join(path, output_file)
        outfile = open(output_path, "w")

        #Detection phase
        try:
            imlist = [osp.join(osp.realpath('.'), images, img) for img in os.listdir(images) if osp.splitext(img)[1] == '.png' or osp.splitext(img)[1] =='.jpeg' or osp.splitext(img)[1] =='.jpg']
        except NotADirectoryError:
            imlist = []
            imlist.append(osp.join(osp.realpath('.'), images))
        except FileNotFoundError:
            print ("No file or directory with the name {}".format(images))
            exit()
        for x in imlist:

            imlist.sort(key = lambda f: int(''.join(filter(str.isdigit, f))))

        if not osp.exists(det):
            os.makedirs(det)
            
        load_batch = time.time()
        
        batches = list(map(prep_image, imlist, [inp_dim for x in range(len(imlist))]))
        im_batches = [x[0] for x in batches]
        orig_ims = [x[1] for x in batches]
        im_dim_list = [x[2] for x in batches]
        im_dim_list = torch.FloatTensor(im_dim_list).repeat(1,2)
        
        if CUDA:
            im_dim_list = im_dim_list.cuda()
        else:
            im_dim_list = im_dim_list
        leftover = 0
        
        if (len(im_dim_list) % batch_size):
            leftover = 1
            
            
        if batch_size != 1:
            num_batches = len(imlist) // batch_size + leftover            
            im_batches = [torch.cat((im_batches[i*batch_size : min((i +  1)*batch_size,
                                len(im_batches))]))  for i in range(num_batches)]        

        i = 0
        
        write = False
        model(get_test_input(inp_dim, CUDA), CUDA)
        
        start_det_loop = time.time()
        
        objs = {}
        for batch in im_batches:
            #load the image 
            start = time.time()

            if CUDA:
                batch = batch.cuda()
            with torch.no_grad():
                prediction = model(Variable(batch), CUDA)
            
    #        prediction = prediction[:,scale_indices]
            prediction = write_results(prediction, confidence, num_classes, nms = True, nms_conf = nms_thresh)      
            if type(prediction) == int:
                i += 1
                if prediction == 0: 
                    print("\n",file = outfile)
                continue

            end = time.time()

            prediction[:,0] += i*batch_size
            
            if not write:
                output = prediction
                write = 0
            else:
                output = torch.cat((output,prediction))

            for im_num, image in enumerate(imlist[i*batch_size: min((i +  1)*batch_size, len(imlist))]):
                im_id = i*batch_size + im_num
                objs = [classes[int(x[-1])] for x in output if int(x[0]) == im_id]
                #print("{0:20s} predicted in {1:6.3f} seconds".format(image.split("/")[-1], (end - start)/batch_size))
                print("{0:s}".format(",".join(objs)), file = outfile)
                
            i += 1
            
        try:
            output
        except NameError:
            print("No detections were made")
            exit()
        end = time.time()
        torch.cuda.empty_cache()
        outfile.close()
        shutil.rmtree(osp.join(path,"imgs",videoid))
        return(iterateLines(output_path))

        
