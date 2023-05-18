#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <stdlib.h>
#include <stdio.h>
#include <conio.h>   //getch()����ϱ�����


#define SERVERIP   "127.0.0.1"
#define SERVERPORT 9000
#define BUFSIZE    4096

#define IDSIZE 255
#define PWSIZE 255
#define NICKNAMESIZE 255
#define ROOMNAMESIZE 255

#define MAPSIZE_X 30
#define MAPSIZE_Y 10

#define LEFT 75
#define RIGHT 77
#define UP 72
#define DOWN 80


enum STATE
{
	BLANK_STATE=-1,
	TITLE_STATE=1,
	LOBBY_STATE,
	ROOMLIST_STATE,
	GAME_ROOM_STATE,
	GAME_STATE
};

enum GAME_PLAY_STATE
{
	INIT=1,
	PLAY,
	END
};



enum RESULT
{
	NODATA=-1,
	WIN =1,        //Game
	LOSE,          //Game
	ID_ERROR,      //Login
	PW_ERROR,      //Login
	LOGIN_SUCCESS, //Login
	ID_DUP,        //Join(ID_DUPLICATE)
	JOIN_SUCCESS,   //Join(Success)
	ROOM_ENTER_SUCCESS,  //GameRoomEnter
	ROOM_ENTER_FAIL,	//GameRoomEnter
	READY_ERROR,        //GameRoom
	READY_SUCCESS,      //GameRoom
	ALL_READY,			//GameRoom   
	NOT_ALL_READY       //GameRoom
};

enum PROTOCOL
{
	REQ,            // ��û(Connect)
	INTRO,          // ���ӿϷ�(Connect Complete)
	JOIN,           // ȸ������(Title)
	LOGIN,          // �α���(Title -> Lobby)
	ROOM_ENTER,     // �� ����(Lobby -> RoomList)
	USERINFO,       // ��������(Lobby)
	LOGOUT,         // �α׾ƿ�(Lobby->Title)
	GAME_ROOM,      // ���ӹ�����(RoomList -> GameRoom)
	ROOM_MADE,      // ���ӹ����(RoomList)
	REFRESH,        // ���ΰ�ħ(RoomList)
	LOBBY,          // �κ�ΰ���(RoomList -> Lobby)

	READY,          // �غ�Ϸ�(GameRoom)
	CANCEL,         // �غ����(GameRoom)
	GAME_START,     // ���ӽ���(GameRoom -> Game)
	ROOM_LIST,      // �������� ����(GameRoom -> RoomList)

	GAME_INIT,      // ���� ������ �ʱ�ȭ.(Game)
	GAME_PLAY,      // ���� �÷���(Game)
	GAME_END,       // ���� ����(Game)
	EXIT=-1         // ����(Title���� �ƿ�����)
};


enum TITLE_MENU
{
	TITLE_JOIN_MENU=1,
	TITLE_LOGIN_MENU,
	TITLE_EXIT_MENU
};

enum LOBBY_MENU
{
	LOBBY_ENTER_MENU=1,
	LOBBY_USERINFO_MENU,
	LOBBY_LOGOUT_MENU
};

enum ROOM_LIST_MENU
{
	ROOM_MADE_MENU=1001,
	ROOM_REFRESH_MENU,
	ROOM_EXIT_MENU
};

enum GAME_LOOM_MENU
{
	READY_COMPLITE_MENU = 1,
	READY_CANCEL_MENU,
	GAME_START_MENU,
	GAME_ROOM_EXIT_MENU
};




struct _User_Info
{
	char id[IDSIZE];
	char pw[PWSIZE];
	char nickname[NICKNAMESIZE];
};

_User_Info myInfo;

char buf[BUFSIZE];

int Map[MAPSIZE_Y][MAPSIZE_X];

int checkCount = 0;
int x, y;
bool userInput = false;


// ���� �Լ� ���� ��� �� ����
void err_quit(char *msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, msg, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(1);
}

// ���� �Լ� ���� ���
void err_display(char *msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s", msg, (char *)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

// ����� ���� ������ ���� �Լ�
int recvn(SOCKET s, char *buf, int len, int flags)
{
	int received;
	char *ptr = buf;
	int left = len;

	while(left > 0){
		received = recv(s, ptr, left, flags);
		if(received == SOCKET_ERROR)
			return SOCKET_ERROR;
		else if(received == 0)
			break;
		left -= received;
		ptr += received;
	}

	return (len - left);
}

bool PacketRecv(SOCKET _sock, char* _buf)
{
	int size;

	int retval = recvn(_sock, (char*)&size, sizeof(size), 0);
	if (retval == SOCKET_ERROR)
	{
		err_display("recv error()");
		return false;
	}
	else if (retval == 0)
	{
		return false;
	}

	retval = recvn(_sock, _buf, size, 0);
	if (retval == SOCKET_ERROR)
	{
		err_display("recv error()");
		return false;

	}
	else if (retval == 0)
	{
		return false;
	}

	return true;
}

PROTOCOL GetProtocol(char* _ptr)
{
	PROTOCOL protocol;
	memcpy(&protocol, _ptr, sizeof(PROTOCOL));

	return protocol;
}

int PackPacket(char* _buf, PROTOCOL _protocol)
{
	int size = 0;

	char* ptr = _buf;

	ptr = ptr + sizeof(size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	size = size + sizeof(_protocol);

	ptr = _buf;
	memcpy(ptr, &size, sizeof(size));

	size = size + sizeof(size);

	return size;
}
void PackPacket(char* _buf, PROTOCOL _protocol, char* _str1, char* _str2, char* _str3, int& _size)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);
	int strsize2 = strlen(_str2);
	int strsize3 = strlen(_str3);

	_size = 0;

	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_protocol);

	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;

	memcpy(ptr, &strsize2, sizeof(strsize2));
	ptr = ptr + sizeof(strsize2);
	_size = _size + sizeof(strsize2);

	memcpy(ptr, _str2, strsize2);
	ptr = ptr + strsize2;
	_size = _size + strsize2;

	memcpy(ptr, &strsize3, sizeof(strsize3));
	ptr = ptr + sizeof(strsize3);
	_size = _size + sizeof(strsize3);

	memcpy(ptr, _str3, strsize3);
	ptr = ptr + strsize3;
	_size = _size + strsize3;

	ptr = _buf;
	memcpy(ptr, &_size, sizeof(_size));

	_size = _size + sizeof(_size);
}
void PackPacket(char* _buf, PROTOCOL _protocol, char* _str1, char* _str2, char* _str3, char* _str4, int& _size)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);
	int strsize2 = strlen(_str2);
	int strsize3 = strlen(_str3);

	int strsize4 = strlen(_str4);

	_size = 0;

	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_protocol);

	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;

	memcpy(ptr, &strsize2, sizeof(strsize2));
	ptr = ptr + sizeof(strsize2);
	_size = _size + sizeof(strsize2);

	memcpy(ptr, _str2, strsize2);
	ptr = ptr + strsize2;
	_size = _size + strsize2;

	memcpy(ptr, &strsize3, sizeof(strsize3));
	ptr = ptr + sizeof(strsize3);
	_size = _size + sizeof(strsize3);

	memcpy(ptr, _str3, strsize3);
	ptr = ptr + strsize3;
	_size = _size + strsize3;

	memcpy(ptr, &strsize4, sizeof(strsize4));
	ptr = ptr + sizeof(strsize4);
	_size = _size + sizeof(strsize4);

	memcpy(ptr, _str4, strsize4);
	ptr = ptr + strsize4;
	_size = _size + strsize4;

	ptr = _buf;
	memcpy(ptr, &_size, sizeof(_size));

	_size = _size + sizeof(_size);
}
void PackPacket(char* _buf, PROTOCOL _protocol, char* _str1, char* _str2, int& _size)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);
	int strsize2 = strlen(_str2);

	_size = 0;

	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_protocol);

	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;

	memcpy(ptr, &strsize2, sizeof(strsize2));
	ptr = ptr + sizeof(strsize2);
	_size = _size + sizeof(strsize2);

	memcpy(ptr, _str2, strsize2);
	ptr = ptr + strsize2;
	_size = _size + strsize2;

	ptr = _buf;
	memcpy(ptr, &_size, sizeof(_size));

	_size = _size + sizeof(_size);
}
void PackPacket(char* _buf, PROTOCOL _protocol, char* _str1, int& _size)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);

	_size = 0;

	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_protocol);

	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;

	ptr = _buf;
	memcpy(ptr, &_size, sizeof(_size));

	_size = _size + sizeof(_size);
}
void PackPacket(char* _buf, PROTOCOL _protocol, int _data1, int _data2, int& _size)
{
	char* ptr = _buf;
	_size = 0;
	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_size);

	memcpy(ptr, &_data1, sizeof(int));
	ptr = ptr + sizeof(int);
	_size = _size + sizeof(_size);

	memcpy(ptr, &_data2, sizeof(int));
	ptr = ptr + sizeof(int);
	_size = _size + sizeof(_size);

	ptr = _buf;

	memcpy(ptr, &_size, sizeof(_size));
	_size = _size + sizeof(_size);
}

void PackPacket(char* _buf, PROTOCOL _protocol, int _data, int& _size)
{
	char* ptr = _buf;
	_size = 0;
	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_size);

	memcpy(ptr, &_data, sizeof(int));
	ptr = ptr + sizeof(int);
	_size = _size + sizeof(_size);

	ptr = _buf;

	memcpy(ptr, &_size, sizeof(_size));
	_size = _size + sizeof(_size);
}


void UnPackPacket(char* _buf, RESULT& _result, char* _str1)
{
	int strsize1;

	char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&_result, ptr, sizeof(_result));
	ptr = ptr + sizeof(_result);

	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);

	memcpy(_str1, ptr, strsize1);
	ptr = ptr + strsize1;
}
void UnPackPacket(char* _buf, RESULT& _result, char* _id, char* _pw, char* _nick, char* _str1)
{
	int strsize1;


	char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&_result, ptr, sizeof(_result));
	ptr = ptr + sizeof(_result);


	//id
	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	memcpy(_id, ptr, strsize1);
	ptr = ptr + strsize1;

	//pw
	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	memcpy(_pw, ptr, strsize1);
	ptr = ptr + strsize1;

	//nick
	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	memcpy(_nick, ptr, strsize1);
	ptr = ptr + strsize1;

	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	memcpy(_str1, ptr, strsize1);
	ptr = ptr + strsize1;
}
void UnPackPacket(char* _buf, char* _str1)
{
	int strsize1;

	char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);

	memcpy(_str1, ptr, strsize1);
	ptr = ptr + strsize1;
}
void UnPackPacket(const char* _buf, int& _data, char* _str1)
{
	int strsize1;

	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&_data, ptr, sizeof(_data));
	ptr = ptr + sizeof(_data);

	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);

	memcpy(_str1, ptr, strsize1);
	ptr = ptr + strsize1;

}



void PlayerMoveAction()
{

	int dx=0, dy=0;
	char ch;

	ch = getch();


	if (ch == -32)
	{
		ch = getch();
		switch (ch)
		{
		case LEFT:
			x = -1; y = 0;  break;
		case RIGHT:
			x = 1;  y = 0;  break;
		case UP:
			x = 0;  y = -1;  break;
		case DOWN:
			x = 0;  y = 1;   break;
		}

	}

}


int main(int argc, char *argv[])
{
	int retval;

	// ���� �ʱ�ȭ
	WSADATA wsa;
	if(WSAStartup(MAKEWORD(2,2), &wsa) != 0)
		return 1;

	// socket()
	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if(sock == INVALID_SOCKET) err_quit("socket()");

	// connect()
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_addr.s_addr = inet_addr(SERVERIP);
	serveraddr.sin_port = htons(SERVERPORT);
	retval = connect(sock, (SOCKADDR *)&serveraddr, sizeof(serveraddr));
	if(retval == SOCKET_ERROR) err_quit("connect()");

	//Server�� ���ӿ�û �޼��� ����.
	int size = PackPacket(buf, PROTOCOL::REQ);
	retval = send(sock, buf, size, 0);
	if (retval == SOCKET_ERROR)
	{
		err_display("value send()");
	}
	

	STATE state=BLANK_STATE;
	GAME_PLAY_STATE g_state = INIT;

	int room_count;
	bool ready = false;
	// ������ ������ ���
	while(1)
	{
		if(!PacketRecv(sock, buf))
		{
			break;
		}

		PROTOCOL protocol = GetProtocol(buf);
		RESULT result;
		char msg[BUFSIZE];
		//RESULT result=NODATA;

		switch (protocol)
		{
		case PROTOCOL::INTRO:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = TITLE_STATE;
			break;

		case PROTOCOL::JOIN:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, result, msg);
			printf("%s\n", msg);
			state = TITLE_STATE;
			break;
		case PROTOCOL::LOGIN:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, result, myInfo.id, myInfo.pw, myInfo.nickname, msg);
			printf("%s\n", msg);
			if (result == LOGIN_SUCCESS)
				state = LOBBY_STATE;
			else
				state = TITLE_STATE;
			break;

		case PROTOCOL::ROOM_ENTER:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, room_count,msg);
			printf("%s\n", msg);
			state = ROOMLIST_STATE;
			break;

		case PROTOCOL::USERINFO:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = LOBBY_STATE;
			break;

		case PROTOCOL::LOGOUT:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = TITLE_STATE;
			break;

		case PROTOCOL::GAME_ROOM:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, result,msg);
			printf("%s\n", msg);

			if (result == RESULT::ROOM_ENTER_SUCCESS)
				state = GAME_ROOM_STATE;
			else
				state = ROOMLIST_STATE;
			break;

		case PROTOCOL::ROOM_MADE:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = BLANK_STATE;
			//state = ROOMLIST_STATE;

			//���� ����� �ѹ� ���ΰ�ħ ���ֱ�.
			size = PackPacket(buf, PROTOCOL::REFRESH);
			retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("value send()");
			}
			break;
		case PROTOCOL::REFRESH:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, room_count,msg);
			printf("%s\n", msg);
			state = ROOMLIST_STATE;
			break;
		case PROTOCOL::LOBBY:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = LOBBY_STATE;
			break;
		case PROTOCOL::READY:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, result,msg);
			printf("%s\n",msg);

			if (result == RESULT::READY_SUCCESS)
				ready = true;
			else
				ready = false;

			break;
		case PROTOCOL::CANCEL:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			break;
		case PROTOCOL::GAME_START:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, result,msg);
			printf("%s\n", msg);

			if (result == ALL_READY)
				state = GAME_STATE;
			else
				state = GAME_ROOM_STATE;
			break;
		case PROTOCOL::ROOM_LIST:
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			state = BLANK_STATE;
			//state = ROOMLIST_STATE;

			//�������� �̵� �� ���ΰ�ħ ���ֱ�.
			size = PackPacket(buf, PROTOCOL::REFRESH);
			retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("value send()");
			}
			break;
		case PROTOCOL::GAME_INIT:
			system("cls");
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			g_state = GAME_PLAY_STATE::PLAY;
			break;

		case PROTOCOL::GAME_PLAY:
			system("cls"); 
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			break;

		case PROTOCOL::GAME_END:
			system("cls");
			ZeroMemory(msg, sizeof(msg));
			UnPackPacket(buf, msg);
			printf("%s\n", msg);
			g_state = GAME_PLAY_STATE::END;
			break;
		}

		bool endflag = false;

		switch (state)
		{
		case BLANK_STATE:
			break;


		case TITLE_STATE:
		{
			int select;
			int size;

			printf("<Ÿ��Ʋ ȭ���Դϴ�...>\n 1.ȸ������\n 2.�α���\n 3.������\n[�޴��� ������ �ּ���]:");

			while (1)
			{
				bool exit = false;
				scanf("%d", &select);  

				switch((TITLE_MENU)select)
				{
				case TITLE_JOIN_MENU:
				{
					printf("<ȸ������>�� �����մϴ�...\n");
					printf("ID�� �Է����ּ���:");
					char id[IDSIZE];
					scanf("%s", id);
					printf("PW�� �Է����ּ���:");
					char pw[PWSIZE];
					scanf("%s", pw);
					printf("�г����� �Է����ּ���:");
					char nickname[NICKNAMESIZE];
					scanf("%s", nickname);


					PackPacket(buf, PROTOCOL::JOIN,id,pw,nickname,size);
					exit = true;
				} //JOIN 
					break;
				case TITLE_LOGIN_MENU:
				{
					printf("<�α���>�� �����մϴ�...\n");
					printf("ID�� �Է����ּ���:");
					char id[IDSIZE];
					scanf("%s", id);
					printf("PW�� �Է����ּ���:");
					char pw[PWSIZE];
					scanf("%s", pw);

					PackPacket(buf, PROTOCOL::LOGIN,id,pw,size);
					exit = true;
				} //LOGIN
					break;
				case TITLE_EXIT_MENU:
					size = PackPacket(buf, PROTOCOL::EXIT);
					exit = true;
					endflag = true;
					break;
				}

				if (!exit)
				{
					printf("�߸��� ���� �Է��Ͽ����ϴ�...\n ����:");
				}
				else
					break;
			}


			retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("value send()");
			}


		}//Title_state 
			break;
		case LOBBY_STATE:
		{
			int select;
			int size;

			printf("<�κ� ȭ���Դϴ�...>\n 1.���� ����\n 2.��������\n 3.�α׾ƿ�\n[�޴��� ������ �ּ���]:");

			while (1)
			{
				bool exit = false;
				scanf("%d", &select);

				switch ((LOBBY_MENU)select)
				{
				case LOBBY_ENTER_MENU:
				{
					size = PackPacket(buf, PROTOCOL::ROOM_ENTER);
					exit = true;
				} //JOIN 
				break;
				case LOBBY_USERINFO_MENU:
				{
					size = PackPacket(buf, PROTOCOL::USERINFO);
					exit = true;
				} //LOGIN
				break;
				case LOBBY_LOGOUT_MENU:
					size = PackPacket(buf, PROTOCOL::LOGOUT);
					exit = true;
					break;
				}

				if (!exit)
				{
					printf("�߸��� ���� �Է��Ͽ����ϴ�...\n ����:");
				}
				else
					break;
			}


			retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("value send()");
			}

			//endflag = true;

		}//LOBBY_STASTE
			break;

		case ROOMLIST_STATE:
		{
			int select;
			int size;

			printf("<���� ȭ���Դϴ�...>\n 1.���� ������ �ش� ���ȣ ���ڸ� �Է����ּ���\n");
			printf(" 2.�� ������ 1001��\n 3.���ΰ�ħ�� 1002��\n 4.������� 1003���� �Է����ּ���\n [�޴��� ������ �ּ���]:");
			

			while (1)
			{
				bool exit = false;
				scanf("%d", &select);


				if (select >= 1 && select <= room_count)
				{
					PackPacket(buf, PROTOCOL::GAME_ROOM,select,size);
					exit = true;
				}
				else
				{
					switch ((ROOM_LIST_MENU)select)
					{
					case ROOM_MADE_MENU:
					{
						printf("�� �̸��� �Է����ּ���:");
						char room_name[ROOMNAMESIZE];
						scanf("%s", room_name);
						PackPacket(buf, PROTOCOL::ROOM_MADE,room_name,myInfo.nickname,size);
						exit = true;
					} //JOIN 
					break;
					case ROOM_REFRESH_MENU:
					{
						size = PackPacket(buf, PROTOCOL::REFRESH);
						exit = true;
					} //LOGIN
					break;
					case ROOM_EXIT_MENU:
						size = PackPacket(buf, PROTOCOL::LOBBY);
						exit = true;
						break;
					}
				}

				if (!exit)
				{
					printf("�߸��� ���� �Է��Ͽ����ϴ�...\n ����:");
				}
				else
					break;
			}


			retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("value send()");
			}

		}//ROOMLIST_STATE
		break;

		case STATE::GAME_ROOM_STATE:
		{
			int select;
			int size;

			if (!ready)
			{
				printf("<���� ȭ���Դϴ�...>\n 1.�غ�Ϸ�\n 2.�غ����\n 3.���ӽ���(���常 ����!)\n 4.������\n [�޴��� ������ �ּ���]:");

				while (1)
				{
					bool exit = false;
					scanf("%d", &select);

					switch ((GAME_LOOM_MENU)select)
					{
					case GAME_LOOM_MENU::READY_COMPLITE_MENU:
						PackPacket(buf, PROTOCOL::READY, myInfo.id, myInfo.nickname, size);
						exit = true;
						break;

					case GAME_LOOM_MENU::READY_CANCEL_MENU:
						PackPacket(buf, PROTOCOL::CANCEL, myInfo.id, size);
						exit = true;
						break;

					case GAME_LOOM_MENU::GAME_START_MENU:
						PackPacket(buf, PROTOCOL::GAME_START, myInfo.id, myInfo.nickname,size);
						exit = true;
						break;

					case GAME_LOOM_MENU::GAME_ROOM_EXIT_MENU:
						PackPacket(buf, PROTOCOL::ROOM_LIST, myInfo.id, size);
						exit = true;
						break;

					}


					if (!exit)
					{
						printf("�߸��� ���� �Է��Ͽ����ϴ�...\n ����:");
					}
					else
						break;
				}


				retval = send(sock, buf, size, 0);
				if (retval == SOCKET_ERROR)
				{
					err_display("value send()");
				}
			}
			else
			{
				printf("���� ������ ��ٸ��� ���Դϴ�. �ѹ� �غ�ϷḦ ������ ������ �����ϴ�...");
			}
			
		}//GAME_ROOM_STATE
			break;
		case STATE::GAME_STATE:
		{

			switch (g_state)
			{
			case GAME_PLAY_STATE::INIT:
				size = PackPacket(buf, PROTOCOL::GAME_INIT);
				retval = send(sock, buf, size, 0);
				if (retval == SOCKET_ERROR)
				{
					err_display("value send()");
				}

				break;
			case GAME_PLAY_STATE::PLAY:
			{
				
				PlayerMoveAction();

				PackPacket(buf, PROTOCOL::GAME_PLAY,x,y,size);
				retval = send(sock, buf, size, 0);
				if (retval == SOCKET_ERROR)
				{
					err_display("value send()");
				}
			}
				break;
			case GAME_PLAY_STATE::END:

				printf("����ϼ̽��ϴ�!\n");
				endflag = true;
				break;
			}


		}//GAME_STATE
		break;

		}

		if (endflag)
		{
			break;
		}

		
	}//end while

	// closesocket()
	closesocket(sock);

	// ���� ����
	WSACleanup();
	system("pause");
	return 0;
}