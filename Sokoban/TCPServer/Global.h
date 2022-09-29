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

#define INTRO_MSG "[서버와의 접속이 완료되었습니다!]\n"
#define BACK_TO_LOBBY_MSG "[로비로 돌아왔습니다!]\n"
#define BACK_TO_ROOMLIST_MSG "[방목록으로 돌아왔습니다!]\n"

#define ID_ERROR_MSG "[없는 아이디입니다.]\n"
#define PW_ERROR_MSG "[패스워드가 틀렸습니다.]\n"
#define LOGIN_SUCCESS_MSG "[로그인에 성공했습니다.]\n"
#define ID_DUPLICATE_MSG "[이미 있는 아이디 입니다.]\n"
#define JOIN_SUCCESS_MSG "[가입에 성공했습니다.]\n"
#define LOGOUT_MSG "[로그아웃되었습니다.]\n"

#define REFRESH_MSG "[방목록을 새로고침 하였습니다!]\n"
#define ROOM_MADE_SUCCESS_MSG "[방 생성을 완료하였습니다!]\n"
#define ROOM_ENTER_SUCCESS_MSG "[방 입장에 성공하였습니다!]\n"
#define ROOM_ENTER_FAIL_MSG "[방 인원이 FULL 입니다!]\n"

#define READY_COMPLITE_MSG "[준비를 완료했습니다!]\n"
#define READY_ERROR_MSG "[방장은 준비를 할 수 없습니다!]\n"
#define READY_CANCEL_MSG "[준비를 취소했습니다!]\n"
#define GAME_START_MSG "[게임을 시작합니다!]\n"
#define GAME_START_ERROR_MSG "[방장만 시작 할 수 있습니다!]\n"
#define NOT_ALL_READY_MSG "[아직 준비완료를 안한 플레이어가 있습니다!]\n"

#define GAME_INIT_MSG "[가장 빨리 창고에 물건을 넣으면 승리합니다.!]\n"
#define GAME_END_MSG "[게임이 종료되었습니다!]\n"



#define WIN_MSG "[이겼습니다.]\n"
#define LOSE_MSG "[졌습니다.]\n"


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
	REQ,             // 요청(Connect)
	INTRO,           // 접속완료(Connect Complete)
	JOIN,            // 회원가입(Title)
	LOGIN,           // 로그인(Title -> Lobby)
	ROOM_ENTER,      // 방 입장(Lobby -> RoomList)
	USERINFO,        // 유저정보(Lobby)
	LOGOUT,          // 로그아웃(Lobby->Title)
	GAME_ROOM,       // 게임방입장(RoomList -> GameRoom)
	ROOM_MADE,       // 게임방생성(RoomList)
	REFRESH,         // 새로고침(RoomList)
	LOBBY,           // 로비로가기(RoomList -> Lobby)

	READY,           // 준비완료(GameRoom)
	CANCEL,          // 준비취소(GameRoom)
	GAME_START,      // 게임시작(GameRoom -> Game)
	ROOM_LIST,       // 방목록으로 가기(GameRoom -> RoomList)
	GAME_INIT,       // 게임 데이터 초기화.(Game)
	GAME_PLAY,       // 게임 플레이(Game)
	GAME_END,        // 게임 종료(Game)
	EXIT = -1        // 종료(Title에서 아예종료)
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
	char King[NICKNAMESIZE];  //방장

	char Chatting_Room_Name[ROOMNAMESIZE];
	int room_number = 0; //방 번호.
	bool full = false;
};

#pragma region 그 외 자잘한 함수
DWORD WINAPI ProcessClient(LPVOID);
DWORD WINAPI WorkerThread(LPVOID arg);
void err_quit(const char* msg);
void err_display(const char* msg);
void ErrorPostQueuedCompletionStatus(SOCKET _sock);
void AcceptPostQueuedCompletionStatus(SOCKET _sock);
#pragma endregion



#pragma region 데이터 패킹 언패킹
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


#pragma region 사용자 정의 함수(Client)
_ClientInfo* AddClientInfo(SOCKET _sock);
void RemoveClientInfo(_ClientInfo* _ptr);
void Accepted(SOCKET _sock);
#pragma endregion


#pragma region 사용하지 않는 함수
bool SendIntro(_ClientInfo*);
void GameProcess(_ClientInfo*);
#pragma endregion

#pragma region 사용자 정의 함수(Recv)
bool Recv(_ClientInfo* _ptr);
int CompleteRecv(_ClientInfo* _ptr, int _completebyte);
void CompleteRecvProcess(_ClientInfo*);
#pragma endregion

#pragma region 사용자 정의 함수(Send)
bool Send(_ClientInfo* _ptr, int _size);
int CompleteSend(_ClientInfo* _ptr, int _completebyte);
void CompleteSendProcess(_ClientInfo*);
#pragma endregion

#pragma region RecvProcess에서 사용하는 함수
//state::Init
void IntroProcess(_ClientInfo* _ptr);			//접속

//state::Title
void JoinProcess(_ClientInfo* _ptr);			//회원가입
void LoginProcess(_ClientInfo* _ptr);			//로그인
void ExitProcess(_ClientInfo* _ptr);			//종료

//state::Lobby
void EnterRoomProcess(_ClientInfo* _ptr);       //방목록 입장
void ShowUserInfoProcess(_ClientInfo* _ptr);    //유저정보 보여주기
void LogoutProcess(_ClientInfo* _ptr);          //로그아웃(Title로 이동)

//state::RoomList
void GameRoomEnterProcess(_ClientInfo* _ptr);	//게임방 입장
void GameRoomMadeProcess(_ClientInfo* _ptr);    //게임방 생성
void RoomListRefreshProcess(_ClientInfo* _ptr);	//게임방 목록 화면 새로고침
void ExitRoomListProcess(_ClientInfo* _ptr);	//게임방 목록 나가기(Lobby로 이동)

//state::GameRoom
void ReadyCompliteProcess(_ClientInfo* _ptr);	//준비완료
void ReadyCancelProcess(_ClientInfo* _ptr);		//준비취소
void GameStartProcess(_ClientInfo* _ptr);		//게임시작
void ExitGameRoomProcess(_ClientInfo* _ptr);	//게임방 나가기(RoomList로 이동)

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