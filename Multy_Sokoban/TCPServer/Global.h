#pragma once
#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <stdlib.h>
#include <stdio.h>

#define SERVERPORT 9000
#define BUFSIZE    4096
#define IDSIZE 255
#define PWSIZE 255
#define NICKNAMESIZE 255
#define ROOMNAMESIZE 255


#define PLAYER_COUNT 100
#define MAX_ROOM_USER 4

#define MAPSIZE_X 30
#define MAPSIZE_Y 10

#define INTRO_MSG "[�������� ������ �Ϸ�Ǿ����ϴ�!]\n"
#define BACK_TO_LOBBY_MSG "[�κ�� ���ƿԽ��ϴ�!]\n"
#define BACK_TO_ROOMLIST_MSG "[�������� ���ƿԽ��ϴ�!]\n"

#define ID_ERROR_MSG "[���� ���̵��Դϴ�.]\n"
#define PW_ERROR_MSG "[�н����尡 Ʋ�Ƚ��ϴ�.]\n"
#define LOGIN_SUCCESS_MSG "[�α��ο� �����߽��ϴ�.]\n"
#define ID_DUPLICATE_MSG "[�̹� �ִ� ���̵� �Դϴ�.]\n"
#define JOIN_SUCCESS_MSG "[���Կ� �����߽��ϴ�.]\n"
#define LOGOUT_MSG "[�α׾ƿ��Ǿ����ϴ�.]\n"

#define REFRESH_MSG "[������ ���ΰ�ħ �Ͽ����ϴ�!]\n"
#define ROOM_MADE_SUCCESS_MSG "[�� ������ �Ϸ��Ͽ����ϴ�!]\n"
#define ROOM_ENTER_SUCCESS_MSG "[�� ���忡 �����Ͽ����ϴ�!]\n"
#define ROOM_ENTER_FAIL_MSG "[�� �ο��� FULL �Դϴ�!]\n"

#define READY_COMPLITE_MSG "[�غ� �Ϸ��߽��ϴ�!]\n"
#define READY_ERROR_MSG "[������ �غ� �� �� �����ϴ�!]\n"
#define READY_CANCEL_MSG "[�غ� ����߽��ϴ�!]\n"
#define GAME_START_MSG "[������ �����մϴ�!]\n"
#define GAME_START_ERROR_MSG "[���常 ���� �� �� �ֽ��ϴ�!]\n"
#define NOT_ALL_READY_MSG "[���� �غ�ϷḦ ���� �÷��̾ �ֽ��ϴ�!]\n"

#define GAME_INIT_MSG "[���� ���� â�� ������ ������ �¸��մϴ�.!]\n"
#define GAME_END_MSG "[������ ����Ǿ����ϴ�!]\n"



#define WIN_MSG "[�̰���ϴ�.]\n"
#define LOSE_MSG "[�����ϴ�.]\n"


enum RESULT
{
	NODATA = -1,
	WIN = 1,			//Game
	LOSE,				//Game
	ID_ERROR,			//Login
	PW_ERROR,			//Login
	LOGIN_SUCCESS,		//Login
	ID_DUP,				//Join(ID_DUPLICATE)
	JOIN_SUCCESS,		//Join(Success)
	ROOM_ENTER_SUCCESS, //GameRoomEnter
	ROOM_ENTER_FAIL,	//GameRoomEnter
	READY_ERROR,        //GameRoom
	READY_SUCCESS,      //GameRoom
	ALL_READY,			//GameRoom   
	NOT_ALL_READY       //GameRoom
};


enum PROTOCOL
{
	REQ,             // ��û(Connect)
	INTRO,           // ���ӿϷ�(Connect Complete)
	JOIN,            // ȸ������(Title)
	LOGIN,           // �α���(Title -> Lobby)
	ROOM_ENTER,      // �� ����(Lobby -> RoomList)
	USERINFO,        // ��������(Lobby)
	LOGOUT,          // �α׾ƿ�(Lobby->Title)
	GAME_ROOM,       // ���ӹ�����(RoomList -> GameRoom)
	ROOM_MADE,       // ���ӹ����(RoomList)
	REFRESH,         // ���ΰ�ħ(RoomList)
	LOBBY,           // �κ�ΰ���(RoomList -> Lobby)

	READY,           // �غ�Ϸ�(GameRoom)
	CANCEL,          // �غ����(GameRoom)
	GAME_START,      // ���ӽ���(GameRoom -> Game)
	ROOM_LIST,       // �������� ����(GameRoom -> RoomList)
	GAME_INIT,       // ���� ������ �ʱ�ȭ.(Game)
	GAME_PLAY,       // ���� �÷���(Game)
	GAME_END,        // ���� ����(Game)
	EXIT = -1        // ����(Title���� �ƿ�����)
};

enum
{
	SOC_ERROR = 1,
	SOC_TRUE,
	SOC_FALSE
};

enum STATE
{
	INIT_STATE=1,
	TITLE_STATE,
	LOBBY_STATE,
	ROOMLIST_STATE,
	GAME_ROOM_STATE,
	GAME_STATE
};

enum IO_TYPE
{
	IO_RECV = 1,
	IO_SEND, 
	IO_DISCONNECT,
	IO_ACCEPT = -100,
	IO_ERROR = -200
};

struct _ClientInfo;

struct WSAOVERLAPPED_EX
{
	WSAOVERLAPPED overlapped;
	_ClientInfo* ptr;
	IO_TYPE       type;
};

struct _User_Info
{
	char id[IDSIZE];
	char pw[PWSIZE];
	char nickname[NICKNAMESIZE];
};

struct _ClientInfo
{
	WSAOVERLAPPED_EX	r_overlapped;
	WSAOVERLAPPED_EX	s_overlapped;

	SOCKET			sock;
	SOCKADDR_IN		addr;
	STATE			state;
	bool			r_sizeflag;

	_User_Info*		UserInfo;
	_User_Info		r_userinfo;
	RESULT			result;

	int				win;
	int				lose;

	int             room_number;
	bool			Ready;

	int             dx;
	int             dy;
	int             checkCount;
	
	int             MapInfo[MAPSIZE_Y][MAPSIZE_X];

	char			made_chat_name[ROOMNAMESIZE];   //
	char			select_number[ROOMNAMESIZE];
	char			roomKing[NICKNAMESIZE];

	int				recvbytes;
	int				comp_recvbytes;
	int				sendbytes;
	int				comp_sendbytes;

	char			recvbuf[BUFSIZE];
	char			sendbuf[BUFSIZE];

	WSABUF			r_wsabuf;
	WSABUF			s_wsabuf;
};

struct Room_Info
{
	_ClientInfo* UserList[MAX_ROOM_USER];
	int User_Count;


	char nicknamelist[MAX_ROOM_USER][NICKNAMESIZE];
	char King[NICKNAMESIZE];  //����

	char Chatting_Room_Name[ROOMNAMESIZE];
	int room_number = 0; //�� ��ȣ.
	bool full = false;
};

#pragma region �� �� ������ �Լ�
DWORD WINAPI ProcessClient(LPVOID);
DWORD WINAPI WorkerThread(LPVOID arg);
void err_quit(const char* msg);
void err_display(const char* msg);
void ErrorPostQueuedCompletionStatus(SOCKET _sock);
void AcceptPostQueuedCompletionStatus(SOCKET _sock);
#pragma endregion



#pragma region ������ ��ŷ ����ŷ
//Protocol
PROTOCOL GetProtocol(const char* _ptr);
//Pack
int PackPacket(char* _buf, PROTOCOL _protocol, const char* _str1);
int PackPacket(char* _buf, PROTOCOL _protocol, int _data, const char* _str1);
int PackPacket(char* _buf, PROTOCOL _protocol, RESULT _result, const char* _str1);
int PackPacket(char* _buf, PROTOCOL _protocol, RESULT _result, const char* _id, const char* _pw, const char* _nick, const char* _str1);
//UnPack
void UnPackPacket(const char* _buf, int& _data);
void UnPackPacket(const char* _buf, char* _str1);
void UnPackPacket(const char* _buf, int& _data1, int& _data2);
void UnPackPacket(const char* _buf, char* _str1, char* _str2);
void UnPackPacket(const char* _buf, char* _str1, char* _str2, char* _str3);
#pragma endregion


#pragma region ����� ���� �Լ�(Client)
_ClientInfo* AddClientInfo(SOCKET _sock);
void RemoveClientInfo(_ClientInfo* _ptr);
void Accepted(SOCKET _sock);
#pragma endregion


#pragma region ������� �ʴ� �Լ�
bool SendIntro(_ClientInfo*);
void GameProcess(_ClientInfo*);
#pragma endregion

#pragma region ����� ���� �Լ�(Recv)
bool Recv(_ClientInfo* _ptr);
int CompleteRecv(_ClientInfo* _ptr, int _completebyte);
void CompleteRecvProcess(_ClientInfo*);
#pragma endregion

#pragma region ����� ���� �Լ�(Send)
bool Send(_ClientInfo* _ptr, int _size);
int CompleteSend(_ClientInfo* _ptr, int _completebyte);
void CompleteSendProcess(_ClientInfo*);
#pragma endregion

#pragma region RecvProcess���� ����ϴ� �Լ�
//state::Init
void IntroProcess(_ClientInfo* _ptr);			//����

//state::Title
void JoinProcess(_ClientInfo* _ptr);			//ȸ������
void LoginProcess(_ClientInfo* _ptr);			//�α���
void ExitProcess(_ClientInfo* _ptr);			//����

//state::Lobby
void EnterRoomProcess(_ClientInfo* _ptr);       //���� ����
void ShowUserInfoProcess(_ClientInfo* _ptr);    //�������� �����ֱ�
void LogoutProcess(_ClientInfo* _ptr);          //�α׾ƿ�(Title�� �̵�)

//state::RoomList
void GameRoomEnterProcess(_ClientInfo* _ptr);	//���ӹ� ����
void GameRoomMadeProcess(_ClientInfo* _ptr);    //���ӹ� ����
void RoomListRefreshProcess(_ClientInfo* _ptr);	//���ӹ� ��� ȭ�� ���ΰ�ħ
void ExitRoomListProcess(_ClientInfo* _ptr);	//���ӹ� ��� ������(Lobby�� �̵�)

//state::GameRoom
void ReadyCompliteProcess(_ClientInfo* _ptr);	//�غ�Ϸ�
void ReadyCancelProcess(_ClientInfo* _ptr);		//�غ����
void GameStartProcess(_ClientInfo* _ptr);		//���ӽ���
void ExitGameRoomProcess(_ClientInfo* _ptr);	//���ӹ� ������(RoomList�� �̵�)

//state::Game
void GameInitProcess(_ClientInfo* _ptr);
void GamePlayProcess(_ClientInfo* _ptr);
void GameEndProcess(_ClientInfo* _ptr);
#pragma endregion

int who_win(int first, int second);





#ifdef MAIN

_ClientInfo* ClientInfo[PLAYER_COUNT];
int Count = 0;

_User_Info* Join_List[PLAYER_COUNT];
int Join_Count = 0;

Room_Info* RoomList[PLAYER_COUNT];
int Room_Count = 0;

//int Map[MAPSIZE_Y][MAPSIZE_X];

CRITICAL_SECTION cs;
HANDLE hcp;

#else

extern _ClientInfo* ClientInfo[PLAYER_COUNT];
extern int Count;

extern _User_Info* Join_List[PLAYER_COUNT];
extern int Join_Count;

extern Room_Info* RoomList[PLAYER_COUNT];
extern int Room_Count;

//extern int Map[MAPSIZE_Y][MAPSIZE_X];

extern CRITICAL_SECTION cs;
extern HANDLE hcp;


#endif