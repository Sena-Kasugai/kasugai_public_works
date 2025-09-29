# -*- coding: utf-8 -*-
import maya.cmds as cmds
import os
import subprocess

class MultiScreenshot(object):
    # 定義
    # 撮影するカメラの視点
    # パターン1 ぬいぐるみ等
    pattern_one_rotate = [
        [ 0, 0, 0],
        [ 0, -45.0, 0],
        [ 0, -90.0, 0],
        [ 0, -135.0, 0],
        [ 0, -180.0, 0],
        [ 0, -225.0, 0],
        [ 0, -270.0, 0],
        [ 0, -315.0, 0],
    ]
    # パターン2 家具
    pattern_two_rotate = [
        [ 0, 0, 0],
        [ 0, -90.0, 0],
        [ 0, 90.0, 0],
        [ 0, -180.0, 0],
        [ -90.0, 0, 0],
        [ -26.197, 222.6, 0],
        [ -26.197, 222.6 - 180.0, 0]
    ]
    # スクリーンショット撮影時の、ウィンドウのサイズ
    screenshot_window_x = 1080
    screenshot_window_y = 1080
    
    persp_rotate = [ -26.197, 222.6, 0] # perspのカメラ固定値
    # カメラ拡大時の、スクリーンとモデルに対する余白
    fitFactor_value = 0.9
    
    def __init__(self):
        self.window = 'MultiScreenshotWindow'
        self.title = 'Multi Screenshot Window'
        self.size = (546, 350)
        self.supportsToolAction = False
        
    def patternOneBtnCmd(self, *args):
        self.pattern = 1
        self.exportScreenshot()
    
    def patternTwoBtnCmd(self, *args):
        self.pattern = 2
        self.exportScreenshot()
    
    def unionBtnCmd(self, *args):
        cmds.deleteUI(self.window, window=True)

    def commonButtons(self):
        self.commonBtnSize = ((self.size[0]-18)/3, 26)
        self.commonBtnLayout = cmds.rowLayout(
            numberOfColumns=3,
            cw3=(
                self.commonBtnSize[0]+3,
                self.commonBtnSize[0]+3,
                self.commonBtnSize[0]+3
            ),
            ct3=('both','both','both'),
            co3=(2,0,2),
            cl3=('center','center','center')
        );
        self.actionBtn = cmds.button(
            label='Pattern 1',
            height=self.commonBtnSize[1],
            command=self.patternOneBtnCmd
        );
        self.applyBtn = cmds.button(
            label='Pattern 2',
            height=self.commonBtnSize[1],
            command=self.patternTwoBtnCmd
        );
        self.closeBtn = cmds.button(
            label='Close',
            height=self.commonBtnSize[1],
            command=self.unionBtnCmd
        );

    def create(self):
        if cmds.window(self.window, exists=True):
            cmds.deleteUI(self.window, window=True);
        self.window = cmds.window(
            self.window,
            title=self.title,
            widthHeight=self.size
        );
        self.commonButtons();
        cmds.showWindow();                
    
    def exportScreenshot(self, *args):
    
        #　スクリーンショット用のWindow作成
        Screenshot_panel = "ScreenshotModelPanel"
        if cmds.modelPanel(Screenshot_panel, exists=True):
            cmds.deleteUI(Screenshot_panel, panel=True)
    
        
        # ウィンドウを作成
        if cmds.window('ScreenshotPanel', exists=True):  # Windowが開いていたら閉じる
            cmds.deleteUI('ScreenshotPanel', window=True)
        if cmds.windowPref('ScreenshotPanel', ex=True) == True:  # windowのサイズをリセットする
            cmds.windowPref('ScreenshotPanel', remove=True)
        window = cmds.window('ScreenshotPanel', title="Model Panel Window", tlc=(0, 0),
                             widthHeight=(self.screenshot_window_x, self.screenshot_window_y))
        cmds.paneLayout()
    
    
    
        # ウィンドウにモデルパネル表示、ビューの環境設定
        model_panel = cmds.modelPanel(Screenshot_panel)
        cmds.modelEditor(model_panel, edit=True, grid=False, controlVertices=False, locators=False,
                         displayAppearance="smoothShaded", displayTextures=True, displayLights='flat')
        cmds.showWindow(window)
    
    
    
        # 選択しているオブジェクト
        selected_objects = cmds.ls(selection=True)
        print_file_name = ""
        # フォルダ作成しておく
        ss_path = self.getScreenshotFolder()
        work_rotate = []
        pattern_name = ""
        screenshot_pattern_path = ""
        if self.pattern == 1:
            work_rotate = self.pattern_one_rotate
            pattern_name = "pattern_1"
            screenshot_pattern_path = ss_path+"/"+pattern_name
            print("screenshot_pattern_path:"+screenshot_pattern_path)
            if not os.path.exists(screenshot_pattern_path):
                print("フォルダが存在しないので作成する")
                os.makedirs(screenshot_pattern_path)
        else:
            work_rotate = self.pattern_two_rotate
            pattern_name = "pattern_2"
            screenshot_pattern_path = ss_path+"/"+pattern_name
            print("screenshot_pattern_path:"+screenshot_pattern_path)
            if not os.path.exists(screenshot_pattern_path):
                print("フォルダが存在しないので作成する")
                os.makedirs(screenshot_pattern_path)

        # カメラのビューの数だけ繰り返す
        for index, rotate_xyz in enumerate(work_rotate):
            # オブジェクトを再選択
            cmds.select(selected_objects)
            print("index:"+ str(index) )
    
            # アクティブなビューを切り替える
            active = cmds.modelEditor(
                Screenshot_panel, edit=True,  activeView=True)
    
            # カメラの名前を指定してカメラを切り替える
            duplicated_camera = self.switchCamera(new_camera_name=str(index))
            self.cameraRotation(duplicated_camera, rotate_xyz[0], rotate_xyz[1], rotate_xyz[2])
            print("active   "+active)
    
            # 選択したオブジェクトに合わせてカメラ移動
            cmds.viewFit(fitFactor = self.fitFactor_value)
    
            # 移動後に選択解除
            cmds.select(clear=True)
            scene_name = self.getSceneName()
            file_name = scene_name+"_"+str(index) + ".jpg"
            output_file_path = screenshot_pattern_path + "/" + file_name
            output_file_path = output_file_path.replace("\\", "/")
            print("output_file_path:"+output_file_path)
            print_file_name =  print_file_name +"  " + file_name  +"\n"
    
            
    
            editor_name = cmds.modelEditor(
                Screenshot_panel, edit=True, capture=output_file_path)
            print("editor_name:"+editor_name)
            print("\n")
            
            # カメラビューを削除
            cmds.refresh()
    
            # 複製したカメラ消去
            cmds.delete(duplicated_camera)
    
         
    	# ウィンドウ閉じる
        cmds.deleteUI('ScreenshotPanel', window=True)
        # スクリーンショット用のモデルパネル消去
        cmds.deleteUI(Screenshot_panel, panel=True)
    
    
        #書き出し完了のダイアログ表示
    
        message = "以下のフォルダにスクリーンショットファイルを書き出しました。\n" + str(screenshot_pattern_path) +"\n\n"+ str(print_file_name)
        result = cmds.confirmDialog(
            title='Screenshot Export Dialog',
            message=message,
            button=['OK'],
            defaultButton='OK',
            cancelButton='OK',
            dismissString='OK',
        )

    # スクリーンショットの書き出しフォルダ
    def getScreenshotFolder(self):
        scene_path = cmds.file(q=True, sn=True)
        split_path = scene_path.split("/")
        scenes_parent_folder = "/".join(split_path[:-2])
        screenshot_path = scenes_parent_folder+"/screenshot"
        print("screenshot_path:"+screenshot_path)
        if not os.path.exists(screenshot_path):
            print("フォルダが存在しないので作成する")
            os.makedirs(screenshot_path)
    
        return screenshot_path
    
    # カメラの切り替え
    def switchCamera(self, new_camera_name):
    
        # カメラの複製
        # Duplicate the camera
        copy_name = "screenshot_script"
        # めんどいんでfront複製で全部どうにかします
        duplicated_camera = cmds.duplicate("front", name=str(copy_name))
        cmds.lookThru(duplicated_camera)
            
        return duplicated_camera
    
    def cameraRotation(self, duplicated_camera, x, y, z):
        print("rotate set:" )
        cmds.xform(duplicated_camera, rotation=(x, y, z))
    
    # シーン名の取得
    def getSceneName(self):
        scene_path = cmds.file(query=True, sceneName=True)
        filename = os.path.basename(scene_path)
        scene_name, _ = os.path.splitext(filename)
        return scene_name


# 以下実行
multiScreenshot = MultiScreenshot();
multiScreenshot.create();
