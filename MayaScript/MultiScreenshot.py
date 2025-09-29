# -*- coding: utf-8 -*-
import maya.cmds as cmds
import os
import subprocess

class MultiScreenshot(object):
    # ��`
    # �B�e����J�����̎��_
    # �p�^�[��1 �ʂ�����ݓ�
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
    # �p�^�[��2 �Ƌ�
    pattern_two_rotate = [
        [ 0, 0, 0],
        [ 0, -90.0, 0],
        [ 0, 90.0, 0],
        [ 0, -180.0, 0],
        [ -90.0, 0, 0],
        [ -26.197, 222.6, 0],
        [ -26.197, 222.6 - 180.0, 0]
    ]
    # �X�N���[���V���b�g�B�e���́A�E�B���h�E�̃T�C�Y
    screenshot_window_x = 1080
    screenshot_window_y = 1080
    
    persp_rotate = [ -26.197, 222.6, 0] # persp�̃J�����Œ�l
    # �J�����g�厞�́A�X�N���[���ƃ��f���ɑ΂���]��
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
    
        #�@�X�N���[���V���b�g�p��Window�쐬
        Screenshot_panel = "ScreenshotModelPanel"
        if cmds.modelPanel(Screenshot_panel, exists=True):
            cmds.deleteUI(Screenshot_panel, panel=True)
    
        
        # �E�B���h�E���쐬
        if cmds.window('ScreenshotPanel', exists=True):  # Window���J���Ă��������
            cmds.deleteUI('ScreenshotPanel', window=True)
        if cmds.windowPref('ScreenshotPanel', ex=True) == True:  # window�̃T�C�Y�����Z�b�g����
            cmds.windowPref('ScreenshotPanel', remove=True)
        window = cmds.window('ScreenshotPanel', title="Model Panel Window", tlc=(0, 0),
                             widthHeight=(self.screenshot_window_x, self.screenshot_window_y))
        cmds.paneLayout()
    
    
    
        # �E�B���h�E�Ƀ��f���p�l���\���A�r���[�̊��ݒ�
        model_panel = cmds.modelPanel(Screenshot_panel)
        cmds.modelEditor(model_panel, edit=True, grid=False, controlVertices=False, locators=False,
                         displayAppearance="smoothShaded", displayTextures=True, displayLights='flat')
        cmds.showWindow(window)
    
    
    
        # �I�����Ă���I�u�W�F�N�g
        selected_objects = cmds.ls(selection=True)
        print_file_name = ""
        # �t�H���_�쐬���Ă���
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
                print("�t�H���_�����݂��Ȃ��̂ō쐬����")
                os.makedirs(screenshot_pattern_path)
        else:
            work_rotate = self.pattern_two_rotate
            pattern_name = "pattern_2"
            screenshot_pattern_path = ss_path+"/"+pattern_name
            print("screenshot_pattern_path:"+screenshot_pattern_path)
            if not os.path.exists(screenshot_pattern_path):
                print("�t�H���_�����݂��Ȃ��̂ō쐬����")
                os.makedirs(screenshot_pattern_path)

        # �J�����̃r���[�̐������J��Ԃ�
        for index, rotate_xyz in enumerate(work_rotate):
            # �I�u�W�F�N�g���đI��
            cmds.select(selected_objects)
            print("index:"+ str(index) )
    
            # �A�N�e�B�u�ȃr���[��؂�ւ���
            active = cmds.modelEditor(
                Screenshot_panel, edit=True,  activeView=True)
    
            # �J�����̖��O���w�肵�ăJ������؂�ւ���
            duplicated_camera = self.switchCamera(new_camera_name=str(index))
            self.cameraRotation(duplicated_camera, rotate_xyz[0], rotate_xyz[1], rotate_xyz[2])
            print("active   "+active)
    
            # �I�������I�u�W�F�N�g�ɍ��킹�ăJ�����ړ�
            cmds.viewFit(fitFactor = self.fitFactor_value)
    
            # �ړ���ɑI������
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
            
            # �J�����r���[���폜
            cmds.refresh()
    
            # ���������J��������
            cmds.delete(duplicated_camera)
    
         
    	# �E�B���h�E����
        cmds.deleteUI('ScreenshotPanel', window=True)
        # �X�N���[���V���b�g�p�̃��f���p�l������
        cmds.deleteUI(Screenshot_panel, panel=True)
    
    
        #�����o�������̃_�C�A���O�\��
    
        message = "�ȉ��̃t�H���_�ɃX�N���[���V���b�g�t�@�C���������o���܂����B\n" + str(screenshot_pattern_path) +"\n\n"+ str(print_file_name)
        result = cmds.confirmDialog(
            title='Screenshot Export Dialog',
            message=message,
            button=['OK'],
            defaultButton='OK',
            cancelButton='OK',
            dismissString='OK',
        )

    # �X�N���[���V���b�g�̏����o���t�H���_
    def getScreenshotFolder(self):
        scene_path = cmds.file(q=True, sn=True)
        split_path = scene_path.split("/")
        scenes_parent_folder = "/".join(split_path[:-2])
        screenshot_path = scenes_parent_folder+"/screenshot"
        print("screenshot_path:"+screenshot_path)
        if not os.path.exists(screenshot_path):
            print("�t�H���_�����݂��Ȃ��̂ō쐬����")
            os.makedirs(screenshot_path)
    
        return screenshot_path
    
    # �J�����̐؂�ւ�
    def switchCamera(self, new_camera_name):
    
        # �J�����̕���
        # Duplicate the camera
        copy_name = "screenshot_script"
        # �߂�ǂ����front�����őS���ǂ��ɂ����܂�
        duplicated_camera = cmds.duplicate("front", name=str(copy_name))
        cmds.lookThru(duplicated_camera)
            
        return duplicated_camera
    
    def cameraRotation(self, duplicated_camera, x, y, z):
        print("rotate set:" )
        cmds.xform(duplicated_camera, rotation=(x, y, z))
    
    # �V�[�����̎擾
    def getSceneName(self):
        scene_path = cmds.file(query=True, sceneName=True)
        filename = os.path.basename(scene_path)
        scene_name, _ = os.path.splitext(filename)
        return scene_name


# �ȉ����s
multiScreenshot = MultiScreenshot();
multiScreenshot.create();
