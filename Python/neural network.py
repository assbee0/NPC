# -*- coding: utf-8 -*-

import sys
import os
import numpy as np
from PIL import Image
import matplotlib.pyplot as plt

# クラス数
class_num = 3

# パラメータ数
size = 42
feature = size

# 学習データ数
train_num =  800
test_num = 200

# データ
data_vec = np.zeros((class_num,train_num,feature), dtype=np.float64)
test_vec = np.zeros((class_num,test_num,feature), dtype=np.float64)

# 学習係数
alpha = 0.01

# シグモイド関数
def Sigmoid( x ):
    return 1 / ( 1 + np.exp(-x) )

# シグモイド関数の微分
def Sigmoid_( x ):
    return ( 1-Sigmoid(x) ) * Sigmoid(x)

# ReLU関数
def ReLU( x ):
    return np.maximum( 0, x )

# ReLU関数の微分
def ReLU_( x ):
    return np.where( x > 0, 1, 0 )

# ソフトマックス関数
def Softmax( x ):
    return np.exp(x)/np.sum(np.exp(x), axis=1, keepdims=True)

# 出力層
class Outunit:
    def __init__(self, n, m):
        # 重み
        self.w = np.random.uniform(-0.5,0.5,(n,m))

        # 閾値
        self.b = np.random.uniform(-0.5,0.5,m)

    def Propagation(self, x):
        self.x = x

        # 内部状態
        self.u = np.dot(self.x, self.w) + self.b

        # 出力値（シグモイド関数）
        #self.out = Sigmoid( self.u )

        self.out = Softmax( self.u )

    def Error(self, t):
        # 誤差
        #f_ = self.out * ( 1 - self.out )
        #f_ = Sigmoid_( self.u )
        f_ = 1
        delta = ( self.out - t ) * f_

        # 重み，閾値の修正値
        self.grad_w = np.dot(self.x.T, delta)
        self.grad_b = np.sum(delta, axis=0)

        # 前の層に伝播する誤差
        self.error = np.dot(delta, self.w.T)

    def Update_weight(self):
        # 重み，閾値の修正
        self.w -= alpha * self.grad_w
        self.b -= alpha * self.grad_b

    def Save(self, filename):
        # 重み，閾値の保存
        np.savez(filename, w=self.w, b=self.b)

    def Load(self, filename):
        # 重み，閾値のロード
        work = np.load(filename)
        self.w = work['w']
        self.b = work['b']

# 中間層
class Hunit:
    def __init__(self, n, m):
        # 重み
        self.w = np.random.uniform(-0.5,0.5,(n,m))

        # 閾値
        self.b = np.random.uniform(-0.5,0.5,m)

    def Propagation(self, x):
        self.x = x

        # 内部状態
        self.u = np.dot(self.x, self.w) + self.b

        # 出力値（シグモイド関数）
        #self.out = Sigmoid( self.u )

        # 出力値（ReLU関数）
        self.out = ReLU( self.u )

    def Error(self, p_error):
        # 誤差
        #f_ = self.out * ( 1 - self.out )
        #f_ = Sigmoid_( self.u )

        f_ = ReLU_( self.u )
        delta = p_error * f_

        # 重み，閾値の修正値
        self.grad_w = np.dot(self.x.T, delta)
        self.grad_b = np.sum(delta, axis=0)

        # 前の層に伝播する誤差
        self.error = np.dot(delta, self.w.T)

    def Update_weight(self):
        # 重み，閾値の修正
        self.w -= alpha * self.grad_w
        self.b -= alpha * self.grad_b

    def Save(self, filename):
        # 重み，閾値の保存
        np.savez(filename, w=self.w, b=self.b)

    def Load(self, filename):
        # 重み，閾値のロード
        work = np.load(filename)
        self.w = work['w']
        self.b = work['b']


# データの読み込み
def Read_data( flag ):
    dir = [ "train" , "test" ]
    for i in range(class_num):
        for j in range(1,train_num+1):

            train_file = dir[ flag ] + "/Level " + str(i+1) + "/" + str(j) + "/parameter.txt"
            #train_file = dir[ flag ] + "/Level " + str(i+1) + "/" + str(j) + "/parameter.txt"
            f = open(train_file,'r')
            line_num = 0
            p_num = 0
            for line in f:
                if line_num >= 43 and line_num <= 47:
                    line_num += 1
                    continue
                if line_num >= 8 and line_num <= 10:
                    line_num += 1
                    continue
                data_vec[i][j-1][p_num] = float(line)
                line_num += 1
                p_num += 1

            # 入力値の合計を1とする
            data_vec[i][j-1] = data_vec[i][j-1] / np.sum( data_vec[i][j-1] )

        for j in range(1,test_num+1):

            test_file = dir[ 1 ] + "/Level " + str(i+1) + "/" + str(j) + "/parameter.txt"
            #train_file = dir[ flag ] + "/Level " + str(i+1) + "/" + str(j) + "/parameter.txt"
            f = open(test_file,'r')
            line_num = 0
            p_num = 0
            for line in f:
                if line_num >= 43 and line_num <= 47:
                    line_num += 1
                    continue
                if line_num >= 8 and line_num <= 10:
                    line_num += 1
                    continue
                test_vec[i][j-1][p_num] = float(line)
                line_num += 1
                p_num += 1

            # 入力値の合計を1とする
            test_vec[i][j-1] = test_vec[i][j-1] / np.sum( test_vec[i][j-1] )

# 学習
def Train():

    # エポック数
    epoch = 4000

    for e in range( epoch ):
        error = 0.0
        for i in range(class_num):
            for j in range(0,train_num):
                # 入力データ
                rnd_c = np.random.randint(class_num)
                rnd_n = np.random.randint(train_num)
                input_data = data_vec[rnd_c][rnd_n].reshape(1,feature)

                # 伝播
                hunit.Propagation( input_data )
                hunit2.Propagation( hunit.out )
                outunit.Propagation( hunit2.out )

                # 教師信号
                teach = np.zeros( (1,class_num) )
                teach[0][rnd_c] = 1

                # 誤差
                outunit.Error( teach )
                hunit2.Error(outunit.error)
                hunit.Error(hunit2.error)

                # 重みの修正
                outunit.Update_weight()
                hunit.Update_weight()
                hunit2.Update_weight()

                error += np.dot( ( outunit.out - teach ) , ( outunit.out - teach ).T )
        print( e , "->" , error )
        count = 0
        acc = 0
        for i in range(class_num):
            for j in range(0,test_num):
                # 入力データ
                input_data = test_vec[i][j].reshape(1,feature)

                # 伝播
                hunit.Propagation( input_data )
                hunit2.Propagation( hunit.out )
                outunit.Propagation( hunit2.out )

                # 教師信号
                teach = np.zeros( (1,class_num) )
                teach[0][i] = 1

                # 予測
                ans = np.argmax( outunit.out[0] )
                if(ans==i):
                    count += 1
                acc = count/600
        print("acc:",acc)
        if acc >= 0.8:
            outunit.Save( "dat/BP-out_"+str(e)+".npz" )
            hunit.Save( "dat/BP-hunit_"+str(e)+".npz" )
            hunit.Save( "dat/BP-hunit2_"+str(e)+".npz" )

    # 重みの保存
    outunit.Save( "dat/BP-out.npz" )
    hunit.Save( "dat/BP-hunit.npz" )
    hunit2.Save( "dat/BP-hunit2.npz" )


# 予測
def Predict():

    # 重みのロード
    outunit.Load( "dat/BP-out.npz" )
    hunit.Load( "dat/BP-hunit.npz" )
    hunit2.Load( "dat/BP-hunit2.npz" )


    # 混同行列
    result = np.zeros((class_num,class_num), dtype=np.int32)

    for i in range(class_num):
        for j in range(0,train_num):
            # 入力データ
            input_data = data_vec[i][j].reshape(1,feature)

            # 伝播
            hunit.Propagation( input_data )
            hunit2.Propagation( hunit )
            outunit.Propagation( hunit2.out )

            # 教師信号
            teach = np.zeros( (1,class_num) )
            teach[0][i] = 1

            # 予測
            ans = np.argmax( outunit.out[0] )

            result[i][ans] +=1
            print( i , j , "->" , ans )

    print( "\n [混同行列]" )
    print( result )
    print( "\n 正解数 ->" ,  np.trace(result) )


'''if __name__ == '__main__':

    # 中間層の個数
    hunit_num = 32

    # 中間層のコンストラクター
    hunit = Hunit( feature , hunit_num )

    # 出力層のコンストラクター
    outunit = Outunit( hunit_num , class_num )

    argvs = sys.argv

    # 引数がtの場合
    if argvs[1] == "t":

        # 学習データの読み込み
        flag = 0
        Read_data( flag )

        # 学習
        Train()

    # 引数がpの場合
    elif argvs[1] == "p":

        # テストデータの読み込み
        flag = 1
        Read_data( flag )

        # テストデータの予測
        Predict()
'''
# 中間層の個数
hunit_num = 64
hunit2_num = 64

# 中間層のコンストラクター
hunit = Hunit( feature , hunit_num )
hunit2 = Hunit( hunit_num , hunit2_num )
# 出力層のコンストラクター
outunit = Outunit( hunit2_num , class_num )

argvs = [0,'t']

# 引数がtの場合
if argvs[1] == "t":

    # 学習データの読み込み
    flag = 0
    Read_data( flag )

    # 学習
    Train()

# 引数がpの場合
elif argvs[1] == "p":

    # テストデータの読み込み
    flag = 1
    Read_data( flag )
    '''
    hunit.Load( "dat/BP-hunit.npz" )
    print(hunit.w)
    print(hunit.b)
    f = open("w.txt","w")
    count = 0

    for w in hunit.w:
        strw = str(w)
        strw = strw.replace('[','').replace(']','').replace('  ',' ').replace('  ',' ').replace('\n','').replace('  ',' ')
        strw = strw.lstrip(' ').rstrip(' ')
        count += len(strw.split(' '))
        print(count)
        f.write(strw)
        f.write('\n')
    '''    '''
    for w in outunit.w:
        f.write(str(w))
        f.write('\n')'''
    # テストデータの予測
    Predict()
