from flask import Flask, jsonify, request
from flask_restful import reqparse
import sys
import os
sys.path.insert(1, os.getcwd())
from detect import main

app = Flask(__name__)

@app.route("/", methods=["POST"])
def home():
    parser = reqparse.RequestParser()
    parser.add_argument('VideoId', type = str, required=True)
    parser.add_argument('VideoPath', type = str, required=True)
    parser.add_argument('BatchSize', type = int, required=False, default=None)
    parser.add_argument('Confidence', type = float, required=False, default=None)
    parser.add_argument('NMS_tresh', type = float, required=False, default=None)
    

    args = parser.parse_args()
    videoId = args["VideoId"]
    videoPath = args["VideoPath"]
    batchSize = args["BatchSize"]
    confidence = args["Confidence"]
    nms = args["NMS_tresh"]

    try:
        return main(videoId, videoPath, batchSize, confidence, nms)
    except Exception as exception:
        with open("logs.txt", "a") as f:
            f.write(f"[{videoId}]: exception {repr(exception)}\n")
        return {}

if __name__ == '__main__':
    app.run()
