// 게임그래픽과제.cpp : 애플리케이션에 대한 진입점을 정의합니다.
//

#include "framework.h"
#include "게임그래픽과제.h"
#include <math.h>
#include <stdlib.h>
#include <time.h>


#define MAX_LOADSTRING 100
#define PI 3.1415


// 전역 변수:
HINSTANCE hInst;                                // 현재 인스턴스입니다.
WCHAR szTitle[MAX_LOADSTRING];                  // 제목 표시줄 텍스트입니다.
WCHAR szWindowClass[MAX_LOADSTRING];            // 기본 창 클래스 이름입니다.

// 이 코드 모듈에 포함된 함수의 선언을 전달합니다:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: 여기에 코드를 입력합니다.

    // 전역 문자열을 초기화합니다.
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_MY, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // 애플리케이션 초기화를 수행합니다:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_MY));

    MSG msg;

    // 기본 메시지 루프입니다:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}



//
//  함수: MyRegisterClass()
//
//  용도: 창 클래스를 등록합니다.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MY));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_MY);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   함수: InitInstance(HINSTANCE, int)
//
//   용도: 인스턴스 핸들을 저장하고 주 창을 만듭니다.
//
//   주석:
//
//        이 함수를 통해 인스턴스 핸들을 전역 변수에 저장하고
//        주 프로그램 창을 만든 다음 표시합니다.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // 인스턴스 핸들을 전역 변수에 저장합니다.

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

class Vertex
{
public:
    float x;
    float y;
    float z;
};


class Shape : Vertex
{
public:
    Vertex v[4];

    void CreateRectangle(HDC hdc, Vertex LeftTop, Vertex RightTop, Vertex LeftBot, Vertex RightBot, bool player)
    {
        HPEN hpen,Oldpen;
        if (player)
            hpen = CreatePen(PS_SOLID, 3, RGB(0, 0, 0));
        else
            hpen = CreatePen(PS_SOLID, 3, RGB(0, 0, 255));
      
        Oldpen = (HPEN)::SelectObject(hdc, (HGDIOBJ)hpen);



        MoveToEx(hdc, LeftTop.x, LeftTop.y, NULL);  // 상단 ㅡ
        LineTo(hdc, RightTop.x, RightTop.y);

        MoveToEx(hdc, LeftBot.x, LeftBot.y, NULL); // 하단 ㅡ
        LineTo(hdc, RightBot.x, RightBot.y);

        MoveToEx(hdc, LeftTop.x, LeftTop.y, NULL); // 좌측 ㅣ
        LineTo(hdc, LeftBot.x, LeftBot.y);

        MoveToEx(hdc, RightTop.x, RightTop.y, NULL); // 우측 ㅣ
        LineTo(hdc, RightBot.x, RightBot.y);

        hpen = (HPEN)::SelectObject(hdc, Oldpen);
        ::DeleteObject(hpen);

    }

};

#define SIZE 20
class Player :Shape
{

public:
    Shape rect;
    float character_size;

    void InitRectPos(POINT origin)
    {
        rect.v[0].x = origin.x - SIZE;
        rect.v[0].y = origin.y - SIZE;

        rect.v[1].x = origin.x + SIZE;
        rect.v[1].y = origin.y - SIZE;

        rect.v[2].x = origin.x - SIZE;
        rect.v[2].y = origin.y + SIZE;

        rect.v[3].x = origin.x + SIZE;
        rect.v[3].y = origin.y + SIZE;
    }

    void CreatePlayer(HDC hdc, bool player)
    {
        rect.CreateRectangle(hdc, rect.v[0], rect.v[1], rect.v[2], rect.v[3],player);
    }

  
};

struct Matrix4x1
{
    float matrix_4x1[4][1] = { 0 };

};


struct Matrix4x4
{
    float matrix_4x4[4][4] = { 0 };
};

Matrix4x1 ConvertToMatrix(Vertex v)
{
    Matrix4x1 mat;

    mat.matrix_4x1[0][0] = v.x;
    mat.matrix_4x1[1][0] = v.y;
    mat.matrix_4x1[2][0] = v.z;
    mat.matrix_4x1[3][0] = 1;

    return mat;
}

void ConvertToVertex(Matrix4x1 m, Vertex& v)
{
    v.x = m.matrix_4x1[0][0];
    v.y = m.matrix_4x1[1][0];
    v.z = m.matrix_4x1[2][0];
}

Matrix4x4 SetTranslateMatrix(float dx, float dy, float dz)
{

    Matrix4x4 mat;

    for (int i = 0; i < 4; i++)
    {
        mat.matrix_4x4[i][i] = 1;
    }

    mat.matrix_4x4[0][3] = dx;
    mat.matrix_4x4[1][3] = dy;
    mat.matrix_4x4[2][3] = dz;
    mat.matrix_4x4[3][3] = 1;

    return mat;
}

Matrix4x4 SetScaleMatrix(float size)
{

    Matrix4x4 mat;

    for (int i = 0; i < 4; i++)
    {
        mat.matrix_4x4[i][i] = 1;
    }

    mat.matrix_4x4[0][0] = size;
    mat.matrix_4x4[1][1] = size;
    mat.matrix_4x4[2][2] = size;
    mat.matrix_4x4[3][3] = 1;

    return mat;
}

Matrix4x4 SetRotateMatrix(float theta)
{

    Matrix4x4 mat;

    float radian = theta * PI / 180;  //각도를 라디안으로 변환.

    mat.matrix_4x4[0][0] = cos(radian);
    mat.matrix_4x4[0][1] = -sin(radian);

    mat.matrix_4x4[1][0] = sin(radian);
    mat.matrix_4x4[1][1] = cos(radian);

    mat.matrix_4x4[2][2] = 1;

    mat.matrix_4x4[3][3] = 1;

    return mat;
}


Matrix4x1 MatrixMultiple4x1(Matrix4x1 m1, Matrix4x4 m2)
{
    Matrix4x1 mat;
    float a = 0;
    float m = 0;

    for (int i = 0; i < 4; i++)
    {
        m = 0;
        a = 0;
        for (int j = 0; j < 4; j++)
        {
            m = m2.matrix_4x4[i][j] * m1.matrix_4x1[j][0];
            a += m;

        }

        mat.matrix_4x1[i][0] = a;
    }

    return  mat;
}

void TransformMove(Vertex& v1, Vertex& v2, Vertex& v3, Vertex& v4,float dx, float dy, float dz)
{
    Matrix4x1 m1, result;
    Matrix4x4 m2;


    //v1좌표 transform
    m1 = ConvertToMatrix(v1);
    m2 = SetTranslateMatrix(dx, dy, dz);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);

    //v2좌표 transform
    m1 = ConvertToMatrix(v2);
    m2 = SetTranslateMatrix(dx, dy, dz);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    //v3좌표 transform
    m1 = ConvertToMatrix(v3);
    m2 = SetTranslateMatrix(dx, dy, dz);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);

    //v4좌표 transform
    m1 = ConvertToMatrix(v4);
    m2 = SetTranslateMatrix(dx, dy, dz);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);

}


void Scale(Vertex& v1, Vertex& v2, Vertex& v3, Vertex& v4, float size)
{
    Matrix4x1 m1, result;
    Matrix4x4 m2;

    Vertex v = v1;
    v.x += (v2.x - v1.x) / 2;
    v.y += (v3.y - v1.y) / 2;

    //**********************************v1좌표 Scale
     //원점(축)으로 이동
    m1 = ConvertToMatrix(v1);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);

    //사이즈조절
    m1 = ConvertToMatrix(v1);
    m2 = SetScaleMatrix(size);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v1);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);


    //**********************************v2좌표 Scale

       //원점(축)으로 이동
    m1 = ConvertToMatrix(v2);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    m1 = ConvertToMatrix(v2);
    m2 = SetScaleMatrix(size);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v2);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    //**********************************v3좌표 Scale

    //원점(축)으로 이동
    m1 = ConvertToMatrix(v3);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);

    m1 = ConvertToMatrix(v3);
    m2 = SetScaleMatrix(size);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v3);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);



    //**********************************v4좌표 Scale

    //원점(축)으로 이동
    m1 = ConvertToMatrix(v4);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);

    m1 = ConvertToMatrix(v4);
    m2 = SetScaleMatrix(size);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v4);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);

}


void LocalRotate(Vertex& v1, Vertex& v2, Vertex& v3,Vertex& v4 ,float theta ,Vertex center)
{
    Matrix4x1 m1, result;
    Matrix4x4 m2;

    Vertex v = center;


    //**********************************v1좌표 Rotate
    //원점(축)으로 이동
    m1 = ConvertToMatrix(v1);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);

    //돌리기
    m1 = ConvertToMatrix(v1);
    m2 = SetRotateMatrix(theta);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v1);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v1);


    //**********************************v2좌표 Rotate

    //원점(축)으로 이동
    m1 = ConvertToMatrix(v2);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    m1 = ConvertToMatrix(v2);
    m2 = SetRotateMatrix(theta);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v2);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v2);

    //**********************************v3좌표 Rotate

    //원점(축)으로 이동
    m1 = ConvertToMatrix(v3);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);

    m1 = ConvertToMatrix(v3);
    m2 = SetRotateMatrix(theta);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v3);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v3);



    //**********************************v4좌표 Rotate

    //원점(축)으로 이동
    m1 = ConvertToMatrix(v4);
    m2 = SetTranslateMatrix(-v.x, -v.y, -v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);

    m1 = ConvertToMatrix(v4);
    m2 = SetRotateMatrix(theta);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);


    //원래있던 위치로 이동
    m1 = ConvertToMatrix(v4);
    m2 = SetTranslateMatrix(v.x, v.y, v.z);
    result = MatrixMultiple4x1(m1, m2);
    ConvertToVertex(result, v4);

}





bool Crash(Player v1, Player v2)
{
    Vertex player , enemy;
    float distance;
    float pr, er;


    pr = (v1.rect.v[1].x - v1.rect.v[0].x) / 2;  //플레이어
    er = (v2.rect.v[1].x - v2.rect.v[0].x) / 2;  //상대

    //Player v[4] = { 왼쪽 상단(0),오른쪽상단(1),왼쪽하단(2),오른쪽하단(3) }
    float rX = (v1.rect.v[0].x + (v1.rect.v[1].x - v1.rect.v[0].x) / 2) - (v2.rect.v[0].x + (v2.rect.v[1].x - v2.rect.v[0].x) / 2);
    float rY = (v1.rect.v[0].y + (v1.rect.v[2].y - v1.rect.v[0].y) / 2) - (v2.rect.v[0].y + (v2.rect.v[2].y - v2.rect.v[0].y) / 2);

    distance = pow(pr + er, 2);

    if (distance >= (rX * rX + rY * rY))
        return true;


    return false;

}

int count = 10;

//
//  함수: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  용도: 주 창의 메시지를 처리합니다.
//
//  WM_COMMAND  - 애플리케이션 메뉴를 처리합니다.
//  WM_PAINT    - 주 창을 그립니다.
//  WM_DESTROY  - 종료 메시지를 게시하고 반환합니다.
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    static RECT rt;
    static Player p;
    static Player Enemy[10];
    static POINT origin;
    static int player_size[6] = { 1,2,3,4,5,6 };
    
    static int size = 1;  //1,2,3,4,5,6

    static float rotateTime = 0;  //돌릴 시간
    static Vertex CenterPoint;   //중점


    static bool sumonPlayer;
    static bool sumonEnemy;




    switch (message)
    {

    case WM_CREATE:

        srand((unsigned)time(NULL));

        GetClientRect(hWnd, &rt);

        origin.x = rt.right / 2;
        origin.y = rt.bottom / 2;


        SetTimer(hWnd, 0, 10, NULL);
        SetTimer(hWnd, 1, 2000, NULL);


        sumonPlayer = true;  //플레이어와 적을 한번만 소환하기 위해.
        sumonEnemy = true;
        break;



    case WM_KEYDOWN:

        switch (wParam)
        {
        case '1': //플레이어 캐릭터 소환.
            if (sumonPlayer)
            {
                sumonPlayer = false;
                p.InitRectPos(origin);
                Scale(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], 2);
                p.character_size = 2;
            }
   
            break;


        case '2':  //적 캐릭터 소환.

            if (sumonEnemy)
            {
                sumonEnemy = false;
                for (int i = 0; i < 10; i++)
                {
                    int scale = 1 + rand() % 5;

                    POINT a;

                    a.x = 100 + (rand() % 600);
                    a.y = 100 + (rand() % 400);

                    Enemy[i].InitRectPos(a);


                    if (i > 3)
                    {
                        Scale(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], scale);
                        Enemy[i].character_size = scale;
                    }
                    else
                    {
                        Scale(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], 1);
                        Enemy[i].character_size = 1;
                    }
                }
            }

            
            break;
        }
        break;

    case WM_TIMER:

        switch (wParam)
        {

        case 0:
            //이동 및 충돌 관련 
            if (GetKeyState('W') & 0x8000)
                TransformMove(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], 0, -5, 0);

            if (GetKeyState('S') & 0x8000)
                TransformMove(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], 0, 5, 0);


            if (GetKeyState('A') & 0x8000)
                TransformMove(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], -5, 0, 0);


            if (GetKeyState('D') & 0x8000)
                TransformMove(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], 5, 0, 0);



            for (int i = 0; i < 10; i++)
            {

                if (Crash(p, Enemy[i]) && !sumonPlayer)
                {
   
                    //충돌결과로 사이즈로 비교후 해당 결과 출력.
                    
                    if (p.character_size >= Enemy[i].character_size)
                    {
                        
                        POINT a;
                        a.x = -100;
                        a.y = -100;
                        Enemy[i].InitRectPos(a);

                        if (p.character_size < 6)
                        {
                            p.character_size++;
                            Scale(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], 1.5f);
                        }
                       
                
                        count--;
                        if (count <= 0)
                        {
                            
                            KillTimer(hWnd, 0);
                            MessageBox(hWnd, TEXT("GameOver"), TEXT("GameOver"), MB_OK);
                            PostQuitMessage(0);
                        }
                     


                    }
                    else
                    {
                        KillTimer(hWnd, 0);
                        MessageBox(hWnd, TEXT("GameOver"), TEXT("GameOver"), MB_OK);
                        PostQuitMessage(0);
                    }

                    //충돌시 캐릭터 회전.
                    SetTimer(hWnd, 2, 100, NULL);
                    CenterPoint = p.rect.v[0];
                    CenterPoint.x += (p.rect.v[1].x - p.rect.v[0].x) / 2;
                    CenterPoint.y += (p.rect.v[2].y - p.rect.v[0].y) / 2;

                   
                   
                    break;
                }

            }
               



            break;

        case 1:      //적 이동 타이머

            for (int i = 0; i < 10; i++)
            {
                int a = rand() % 4;

                switch (a)
                {
                case 0:
                    TransformMove(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], 0, -60, 0);
                    break;

                case 1:
                    TransformMove(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], 0, 60, 0);
                    break;

                case 2:
                    TransformMove(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], -60, 0, 0);
                    break;

                case 3:
                    TransformMove(Enemy[i].rect.v[0], Enemy[i].rect.v[1], Enemy[i].rect.v[2], Enemy[i].rect.v[3], 60, 0, 0);
                    break;

                }

               
            }
            break;


        case 2: //캐릭터 회전 

            
               

            rotateTime += 60;

            LocalRotate(p.rect.v[0], p.rect.v[1], p.rect.v[2], p.rect.v[3], rotateTime, CenterPoint);

 
            if (rotateTime >= 360)
            {
                rotateTime = 0;
                KillTimer(hWnd, 2);
            }
            

        }



     

        InvalidateRect(hWnd, NULL, TRUE);
        break;
   


    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // 메뉴 선택을 구문 분석합니다:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;

            HDC hdc = BeginPaint(hWnd, &ps);

        
            // TODO: 여기에 hdc를 사용하는 그리기 코드를 추가합니다...
          
      
            for (int i = 0; i < 10; i++)
                Enemy[i].CreatePlayer(hdc, false);


            p.CreatePlayer(hdc, true);
           

   
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        KillTimer(hWnd, 1);
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// 정보 대화 상자의 메시지 처리기입니다.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
