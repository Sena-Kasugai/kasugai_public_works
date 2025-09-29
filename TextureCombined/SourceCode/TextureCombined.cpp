// TextureCombined.cpp : このファイルには 'main' 関数が含まれています。プログラム実行の開始と終了がそこで行われます。
//
#include <iostream>
#include <filesystem>
#include <fstream>
#include <cstdio>
#include <tchar.h>
#include <Windows.h>
#include <Shlwapi.h>
#pragma comment(lib, "Shlwapi.lib")
#include "stdafx.h"
#include <cv.h>
#include <cxcore.h>
#include <highgui.h>

FILE _iob[] = { *stdin, *stdout, *stderr };
extern "C" FILE * __cdecl __iob_func(void)
{
    return _iob;
}

using namespace std;

const int PATH_LEN = 1024;

bool getFileNames(std::string folderPath, std::vector<std::string>& file_names);

int main()
{
    TCHAR szModulePath[PATH_LEN];
    TCHAR szSavePath[PATH_LEN];
    vector<string> fileNames;
    int total_width = 0, max_height = 0;
    IplImage** src_img;
    IplImage* combined_img;
    CvRect roi = cvRect(0, 0, 0, 0);

    printf("Pathを入力してください。\n");
    scanf("%s", &szModulePath);
    printf("Path: %s\n", szModulePath);
    StrCpy(szSavePath, szModulePath);
    ::PathAddExtension(szSavePath, "/result.jpg");

    struct _stat s;
    int rc = _stat(szModulePath, &s);
    if (!(rc == 0)) throw "Range Error";
    if (!(s.st_mode & _S_IFDIR)) throw "Directory Error";
    if (!getFileNames(szModulePath, fileNames)) throw "Iterator Error";
    int size = fileNames.size();
    int i = 0;

    src_img = (IplImage**)cvAlloc(sizeof(IplImage*) * size);
    for (const auto& item : fileNames) {
        printf("%s\n", item.c_str());
        src_img[i] = cvLoadImage(item.c_str(), CV_LOAD_IMAGE_COLOR);
        if (src_img[i] == 0)
            return -1;
        total_width += src_img[i]->width;
        max_height = max_height < src_img[i]->height ? src_img[i]->height : max_height;
        i++;
    }

    combined_img = cvCreateImage(cvSize(total_width, max_height), IPL_DEPTH_8U, 3);
    cvZero(combined_img);
    for (i = 0; i < size; i++) {
        roi.width = src_img[i]->width;
        roi.height = src_img[i]->height;
        cvSetImageROI(combined_img, roi);
        cvCopy(src_img[i], combined_img, NULL);
        roi.x += roi.width;
    }
    cvResetImageROI(combined_img);

    cvSaveImage(szSavePath, combined_img);
    cvWaitKey(0);

    cvDestroyWindow("Image");
    cvReleaseImage(&combined_img);
    for (i = 0; i < size; i++) {
        cvReleaseImage(&src_img[i]);
    }
    cvFree(&src_img);

    return 0;
}

/**
* @brief フォルダ以下のファイル一覧を取得する関数
* @param[in]    folderPath  フォルダパス
* @param[out]   file_names  ファイル名一覧
* return        true:成功, false:失敗
*/
bool getFileNames(std::string folderPath, std::vector<std::string>& file_names)
{
    using namespace std::filesystem;
    directory_iterator iter(folderPath), end;
    std::error_code err;

    for (; iter != end && !err; iter.increment(err)) {
        const directory_entry entry = *iter;

        file_names.push_back(entry.path().string());
        printf("%s\n", file_names.back().c_str());
    }

    /* エラー処理 */
    if (err) {
        std::cout << err.value() << std::endl;
        std::cout << err.message() << std::endl;
        return false;
    }
    return true;
}
