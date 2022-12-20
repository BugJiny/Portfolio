using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class UpdateData : AssetPostprocessor
{


	void OnPreprocessAsset()
	{

        DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(assetImporter.assetPath));
        DirectoryInfo[] diarr = di.GetDirectories();


        foreach (DirectoryInfo element in diarr)
        {
            string f_name = Path.GetFileName(element.Name);

            if (f_name == "txt")
            {
                Debug.Log("txt 폴더 있음");
            }
            else if (f_name == "script")
            {
                Debug.Log("script 폴더 있음");
            }
        }

        if (assetImporter.assetPath.Contains("Assets/00.Data/Excel"))
        {

            string a = UnityEngine.Application.dataPath.Replace("/", @"\");
            string b = assetImporter.assetPath.Replace("Assets", "");  // Assets를 공백으로 지워줌 -> Assets가 두번 들어가기 때문에
            string exe_path = a + @"\Editor\TestExcelDataReader.exe";
            string f_path = a + b.Replace("/", @"\");            // 패스 경로
                                                                 //string f_name = Path.GetFileName(f_path);            // 패스경로에서 이름 추출
            string f_extension = Path.GetExtension(f_path);      // 패스경로에서 확장자 추출


            //엑셀 파일에 대한 처리
            if (f_extension.Contains(".xlsx"))
            {
                System.Diagnostics.Process.Start(exe_path, f_path);
            }

            //Debug.Log(exe_path);
            //Debug.Log(f_path);
            //Debug.Log(f_name);
            //Debug.Log(f_extension);
        }
        else
            return;
    }




}
