# TinySLG

培训的作业
1. 至少找出两种免密方案支持ssh vagrant@<vm>以及knife zero converge
2. 创建docker cookbook翻译Docker官网的安装步骤，完成在Ubuntu和CentOS系统上docker软件包安装（无需配置）

另外，
需要完成源码编译方式安装Nginx的练习


#coding=utf-8
from pymongo import MongoClient
from Constants import Constants
import pprint

class MistakesStats:

    def __init__(self):
        self.db = Constants.MONGO_CLIENT.Steam
        pass

    def count(self) :
        pipeline = [{
            "$group":{"_id":"$eId", "total":{"$avg":"$score"}, "count":{"$sum":1}}
        }]
        pprint.pprint(list(self.db.commits.aggregate(pipeline)))
        pass

if __name__ == "__main__":
    ix = MistakesStats()
    ix.count()


import os
from pymongo import MongoClient

class Constants:
    def __init__(self):
        pass
    MONGO_CLIENT = MongoClient('mongodb://'+str(os.environ["MONGO_URL"]))
