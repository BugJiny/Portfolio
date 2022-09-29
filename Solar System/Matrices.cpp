#include <Windows.h>
#include <mmsystem.h>
#include <d3dx9.h>
#pragma warning( disable : 4996 ) // disable deprecated warning 
#include <strsafe.h>
#pragma warning( default : 4996 )

#include <d3dx9shader.h>  


LPDIRECT3D9             g_pD3D = NULL;
LPDIRECT3DDEVICE9       g_pd3dDevice = NULL; 
LPDIRECT3DVERTEXBUFFER9 g_pVB = NULL; 

LPD3DXMESH              g_psphereMesh;       //구체를 만드는데 필요한 메쉬

struct CUSTOMVERTEX
{
	FLOAT x, y, z;
	DWORD color;
};

struct vec3
{
	float x, y, z;
};


#define MAXPLANET 6
enum Planet {
    SUN=0, 
    MERCURY, 
    VENUS, 
    EARTH,
    MOON,
    MARS
};
LPD3DXMESH              g_pSphereMesh[MAXPLANET];

LPD3DXMESH              g_pboxMesh;          //박스를 만드는데 필요한 메쉬.(우주선)
vec3                    g_pSpaceshipPos = {0.0f,0.0f,0.0f};     //우주선 위치값.


D3DXMATRIXA16           g_pEarthMat;









#define D3DFVF_CUSTOMVERTEX (D3DFVF_XYZ|D3DFVF_DIFFUSE)    




HRESULT InitD3D( HWND hWnd )
{

    if( NULL == ( g_pD3D = Direct3DCreate9( D3D_SDK_VERSION ) ) )
        return E_FAIL;


    D3DPRESENT_PARAMETERS d3dpp;
    ZeroMemory( &d3dpp, sizeof( d3dpp ) );
    d3dpp.Windowed = TRUE;
    d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
    d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;

    //d3dpp.EnableAutoDepthStencil = TRUE;
    //d3dpp.AutoDepthStencilFormat = D3DFMT_D16;

    if( FAILED( g_pD3D->CreateDevice( D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, hWnd,
                                      D3DCREATE_SOFTWARE_VERTEXPROCESSING,
                                      &d3dpp, &g_pd3dDevice ) ) )
    {
        return E_FAIL;
    }


    g_pd3dDevice->SetRenderState( D3DRS_CULLMODE, D3DCULL_NONE );   //렌더링 상태를 설정

   
    g_pd3dDevice->SetRenderState( D3DRS_LIGHTING, FALSE );





    return S_OK;
}

HRESULT InitGeometry()
{

    CUSTOMVERTEX g_Vertices[] =
    {
        { -1.0f, 1.0f, 0.0f, 0xffff0000, },  //왼쪽 하단
        {  1.0f, 1.0f, 0.0f, 0xff0000ff, },  //오른쪽 하단
        {  0.0f, 2.0f, 0.0f, 0xffffffff, },  //제일 위

        { -2.0f, 1.0f, 0.0f, 0xffff0000, },  //왼쪽 하단
        { -1.5f, 2.0f, 0.0f, 0xff0000ff, },  //오른쪽 하단
        { -1.5f, 1.0f, 0.0f, 0xffffffff, },  //제일 위


        { -2.0f, -1.0f, 0.0f, 0xffff0000, },  //왼쪽 하단
        { -1.5f, -2.0f, 0.0f, 0xff0000ff, },  //오른쪽 하단
        { -1.5f, -1.0f, 0.0f, 0xffffffff, }  //제일 위
    };



    // Create the vertex buffer.
    if( FAILED( g_pd3dDevice->CreateVertexBuffer( 9 * sizeof( CUSTOMVERTEX ),
                                                  0, D3DFVF_CUSTOMVERTEX,
                                                  D3DPOOL_DEFAULT, &g_pVB, NULL ) ) )
    {
        return E_FAIL;
    }




    LPDIRECT3DVERTEXBUFFER9 pTempVertexBuffer;

	D3DXCreateBox(g_pd3dDevice, 0.2f, 0.2f, 0.2f, &g_pboxMesh, NULL);

    for (int i = 0; i < MAXPLANET; i++)
    {
        switch (i)
        {
        case SUN:
            D3DXCreateSphere(g_pd3dDevice, 0.5, 50, 30, &g_pSphereMesh[i], NULL);
            break;
        case MERCURY:
            D3DXCreateSphere(g_pd3dDevice, 0.1, 50, 30, &g_pSphereMesh[i], NULL);
            break;

        case VENUS:
            D3DXCreateSphere(g_pd3dDevice, 0.2, 50, 30, &g_pSphereMesh[i], NULL);
            break;

        case EARTH:
            D3DXCreateSphere(g_pd3dDevice, 0.25, 50, 30, &g_pSphereMesh[i], NULL);
            break;

        case MOON:
            D3DXCreateSphere(g_pd3dDevice, 0.08, 50, 30, &g_pSphereMesh[i], NULL);
            break;

        case MARS:
            D3DXCreateSphere(g_pd3dDevice, 0.125, 50, 30, &g_pSphereMesh[i], NULL);
            break;
        }

        g_pSphereMesh[i]->CloneMeshFVF(0, D3DFVF_CUSTOMVERTEX, g_pd3dDevice, &g_pSphereMesh[i]);
        g_pSphereMesh[i]->GetVertexBuffer(&pTempVertexBuffer);
        int nNumverts = g_pSphereMesh[i]->GetNumVertices();
        CUSTOMVERTEX* pVertices = NULL;
        pTempVertexBuffer->Lock(0, 0, (void**)&pVertices, 0);

        switch (i)
        {
        case SUN:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(1.0, 1.0, 0.0, 1.0);
            }
            break;
        case MERCURY:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(0.7, 0.7, 0.7, 1.0);
            }
            break;

        case VENUS:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(1.0, 0.7, 0.0, 1.0);
            }
            break;

        case EARTH:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(0.0, 0.0, 1.0, 1.0);
            }
            break;

        case MOON:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(0.85, 0.85, 0.85, 1.0);
            }
            break;

        case MARS:
            for (int i = 0; i < nNumverts; i++)
            {
                pVertices[i].color = D3DCOLOR_COLORVALUE(1.0, 0.0, 0.0, 1.0);
            }
            break;
        }
    
        pTempVertexBuffer->Unlock();
        pTempVertexBuffer->Release();

  

    }




    // Fill the vertex buffer.
    VOID* pVertices;
    if( FAILED( g_pVB->Lock( 0, sizeof( g_Vertices ), ( void** )&pVertices, 0 ) ) )
        return E_FAIL;
    memcpy( pVertices, g_Vertices, sizeof( g_Vertices ) );
    g_pVB->Unlock();

    return S_OK;
}





VOID Cleanup()
{
    if( g_pVB != NULL )
        g_pVB->Release();

    if( g_pd3dDevice != NULL )
        g_pd3dDevice->Release();

    if( g_pD3D != NULL )
        g_pD3D->Release();
}


VOID SetupMatrices(int index)
{
    D3DXMATRIXA16 matWorld;  //월드 좌표행렬(공전)

    D3DXMATRIXA16 matRotating;  //자전 좌표.

    D3DXMATRIX translation;
    //D3DXMATRIX matRotating;


    D3DXMatrixIdentity(&matWorld);
    //D3DXMatrixIdentity(&matRotating);

    UINT iTime=NULL;
    FLOAT fAngle=NULL;

    

    switch (index)
    {
    case SUN:
        //회전각도.
        iTime = timeGetTime() % 10000;   //시스템 시간을 밀리초 단위로 검색
        fAngle = iTime * (2.0f * D3DX_PI) / 10000.0f;  //시간에 따라서 도형을 돌려줄 angle값을 구하는 공식.
        D3DXMatrixTranslation(&translation, 0.0f, 0.0f, 0.0f);
        break;
    case MERCURY:
        //회전각도.
        iTime = timeGetTime() % 2000;   
        fAngle = iTime * (2.0f * D3DX_PI) / 2000.0f;  
        D3DXMatrixTranslation(&translation, 0.65f, 0.0f, 0.0f);
        break;
    case VENUS:
        //회전각도.
        iTime = timeGetTime() % 3000;   
        fAngle = iTime * (2.0f * D3DX_PI) / 3000.0f;  
        D3DXMatrixTranslation(&translation, 1.0f, 0.0f, 0.0f);
        break;
    case EARTH:
        //회전각도.
        iTime = timeGetTime() % 7000;   
        fAngle = iTime * (2.0f * D3DX_PI) / 7000.0f; 
        D3DXMatrixTranslation(&translation, 1.5f, 0.0f, 0.0f);
        break;

    case MOON:
        iTime = timeGetTime() % 2000;  
        fAngle = iTime * (2.0f * D3DX_PI) / 2000.0f; 
        D3DXMatrixTranslation(&translation, 0.3f, 0.0f, 0.0f);
        break;

    case MARS:
        //회전각도.
        iTime = timeGetTime() % 4000;   
        fAngle = iTime * (2.0f * D3DX_PI) / 4000.0f;  
        D3DXMatrixTranslation(&translation, 2.0f, 0.0f, 0.0f);
        break;

	default:
        if (GetKeyState('W') & 0x8000)
            g_pSpaceshipPos.x -= 0.05f;
        if (GetKeyState('A') & 0x8000)
            g_pSpaceshipPos.z -= 0.05f;
        if (GetKeyState('S') & 0x8000)
            g_pSpaceshipPos.x += 0.05f;
        if (GetKeyState('D') & 0x8000)
            g_pSpaceshipPos.z += 0.05f;
		D3DXMatrixTranslation(&translation, g_pSpaceshipPos.x, g_pSpaceshipPos.y, g_pSpaceshipPos.z);
    }

    D3DXMatrixRotationY(&matWorld, fAngle);    //y축을 중심으로 회전하는 행렬을 만드는 함수.
    D3DXMatrixRotationY(&matRotating, fAngle);


    if(index==EARTH)
        g_pEarthMat= matWorld = translation * matWorld * matRotating;
    else if(index == MOON)
        matWorld =  translation* matWorld * g_pEarthMat;
    else
        matWorld = translation * matWorld * matRotating;
    

    g_pd3dDevice->SetTransform( D3DTS_WORLD, &matWorld );

    D3DXVECTOR3 vEyePt( 0.0f, 3.0f,-5.0f );
    D3DXVECTOR3 vLookatPt( 0.0f, 0.0f, 0.0f );
    D3DXVECTOR3 vUpVec( 0.0f, 1.0f, 0.0f ); 

  

    D3DXMATRIXA16 matView;  //View좌표 행렬

    D3DXMatrixLookAtLH( &matView, &vEyePt, &vLookatPt, &vUpVec );

    g_pd3dDevice->SetTransform( D3DTS_VIEW, &matView );





    D3DXMATRIXA16 matProj; //Projection좌표 행렬
    D3DXMatrixPerspectiveFovLH( &matProj, D3DX_PI / 4, 1.0f, 1.0f, 100.0f );  

    /*D3DXMATRIX *D3DXMatrixPerspectiveForvLH(
                       D3DXMATRIX *pOut     //투영 행렬을 리턴
                       FLOAT forY           //시야각의 수직영역(라디안)
                       FLOAT Aspect         //종횡비 = 너비/높이
                       FLOAT znear          //가까운 평면까지의 거리
                       FLOAT zfar           //면 평면까지의 거리
    */

    g_pd3dDevice->SetTransform( D3DTS_PROJECTION, &matProj );

}


VOID SetupLights()
{

    D3DMATERIAL9 mtrl;
    ZeroMemory(&mtrl, sizeof(D3DMATERIAL9));
    mtrl.Diffuse.r = mtrl.Ambient.r = 1.0f;
    mtrl.Diffuse.g = mtrl.Ambient.g = 1.0f;
    mtrl.Diffuse.b = mtrl.Ambient.b = 0.0f;
    mtrl.Diffuse.a = mtrl.Ambient.a = 1.0f;
    g_pd3dDevice->SetMaterial(&mtrl);


    D3DXVECTOR3 vecDir;
    D3DLIGHT9 light;
    ZeroMemory(&light, sizeof(D3DLIGHT9));
    light.Type = D3DLIGHT_DIRECTIONAL;
    //light.Position = D3DXVECTOR3(0.0f, 0.0f, 0.0f);

    light.Diffuse.r = 1.0f;
    light.Diffuse.g = 1.0f;
    light.Diffuse.b = 1.0f;

    vecDir = D3DXVECTOR3(cosf(timeGetTime() / 350.0f),
        1.0f,
        sinf(timeGetTime() / 350.0f));
    //vecDir = D3DXVECTOR3(0.0f, 0.0f, 1.0f);

    D3DXVec3Normalize((D3DXVECTOR3*)&light.Direction, &vecDir);
    light.Range = 1000.0f;
    g_pd3dDevice->SetLight(0, &light);
    g_pd3dDevice->LightEnable(0, TRUE);
    g_pd3dDevice->SetRenderState(D3DRS_LIGHTING, FALSE);

    // Finally, turn on some ambient light.
    g_pd3dDevice->SetRenderState(D3DRS_AMBIENT, 0x00202020);
}


VOID Render()
{

    g_pd3dDevice->Clear( 0, NULL, D3DCLEAR_TARGET, D3DCOLOR_XRGB( 0, 0, 0 ), 1.0f, 0 );


    if( SUCCEEDED( g_pd3dDevice->BeginScene() ) )
    {
        //SetupLights();
		

        for (int i = 0; i < MAXPLANET; i++)
        {


            SetupMatrices(i);


            if(i==0)
                g_pd3dDevice->SetRenderState(D3DRS_FILLMODE, D3DFILL_WIREFRAME); 
            else
                g_pd3dDevice->SetRenderState(D3DRS_FILLMODE, D3DFILL_WIREFRAME);  //D3DFILL_SOLID


            g_pSphereMesh[i]->DrawSubset(0);

			//g_pboxMesh->DrawSubset(0);  -> 모든 행성에 박스가 생김.
        }

		SetupMatrices(-1);
		g_pboxMesh->DrawSubset(0);
		


        g_pd3dDevice->EndScene();
    }

    g_pd3dDevice->Present( NULL, NULL, NULL, NULL );
}



LRESULT WINAPI MsgProc( HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam )
{
    switch( msg )
    {


        case WM_DESTROY:
            Cleanup();
            PostQuitMessage( 0 );
            return 0;
    }

    return DefWindowProc( hWnd, msg, wParam, lParam );
}




INT WINAPI wWinMain( HINSTANCE hInst, HINSTANCE, LPWSTR, INT )
{
    UNREFERENCED_PARAMETER( hInst );


    WNDCLASSEX wc =
    {
        sizeof( WNDCLASSEX ), CS_CLASSDC, MsgProc, 0L, 0L,
        GetModuleHandle( NULL ), NULL, NULL, NULL, NULL,
        L"D3D Tutorial", NULL
    };
    RegisterClassEx( &wc );


    HWND hWnd = CreateWindow( L"D3D Tutorial", L"D3D Tutorial 03: Matrices",
                              WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
                              NULL, NULL, wc.hInstance, NULL );


    if( SUCCEEDED( InitD3D( hWnd ) ) )
    {

        if( SUCCEEDED( InitGeometry() ) )
        {

            ShowWindow( hWnd, SW_SHOWDEFAULT );
            UpdateWindow( hWnd );

            MSG msg;
            ZeroMemory( &msg, sizeof( msg ) );
            while( msg.message != WM_QUIT )
            {
                if( PeekMessage( &msg, NULL, 0U, 0U, PM_REMOVE ) )
                {
                    TranslateMessage( &msg );
                    DispatchMessage( &msg );
                }
                else
                    Render();
            }
        }
    }

    UnregisterClass( L"D3D Tutorial", wc.hInstance );
    return 0;
}



