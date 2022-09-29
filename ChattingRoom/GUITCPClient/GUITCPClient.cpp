#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <stdlib.h>
#include <stdio.h>
#include "resource.h"

#define SERVERIP   "127.0.0.1"
#define SERVERPORT 9000
#define BUFSIZE 4096
#define IDSIZE 255
#define PWSIZE 255
#define NICKNAMESIZE 255
#define ROOMNAMESIZE 255
#define SELECTSIZE 128


enum Menu
{
	JOIN=1,//회원가입
	LOGIN,  //로그인 
	CHAT_ROOM_MADE,  //채팅방 만들기
	CHAT_ATTEND     //채팅방 참가.
};


enum RESULT
{
	NODATA = -1,
	ID_EXIST = 1,
	ID_ERROR,
	PW_ERROR,
	JOIN_SUCCESS,
	LOGIN_SUCCESS,
	LOGOUT_SUCCESS
};

enum PROTOCOL
{
	JOIN_INFO,           //회원가입
	LOGIN_INFO,          //로그인
	JOIN_RESULT,         //회원가입결과
	LOGIN_RESULT,        //로그인결과
	LOGOUT,
	LOGOUT_RESULT,
	CHATTING,            //채팅하는곳
	CHAT_MADE,           //채팅방 만들기
	CHAT_ROOM_SHOW,      //채팅방 보여주기
	SELECT_CHATT_ROOM_NUMBER,   //채팅방 선택하기
	CHATT_MSG,                //채팅 프로토콜
	CHATT_OUT,                 //채팅 프로토콜
	EXIT                     //강퇴 프로토콜

};

enum ClientState
{
	LOBBYSTATE,MADE_AND_SHOW,ROOM_SELECT,CHATTINGSTATE          //
};

struct _User_Info
{
	char id[IDSIZE];
	char pw[PWSIZE];
	char nickname[NICKNAMESIZE];
};

HANDLE hThread[2];

// 대화상자 프로시저
BOOL CALLBACK DlgProc(HWND, UINT, WPARAM, LPARAM);
// 편집 컨트롤 출력 함수
void DisplayText(char* fmt, ...);
// 오류 출력 함수
void err_quit(char* msg);
void err_display(char* msg);
// 사용자 정의 데이터 수신 함수
int recvn(SOCKET s, char* buf, int len, int flags);
// 소켓 통신 스레드 함수
DWORD WINAPI ClientMain(LPVOID arg);
DWORD CALLBACK RecvThread(LPVOID);

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

#pragma region 데이터 패킹 언패킹






void GetProtocol(char* _ptr, PROTOCOL& _protocol)
{
	memcpy(&_protocol, _ptr, sizeof(PROTOCOL));

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
void PackPacket(char* _buf, PROTOCOL _protocol, char* _str1, char* _str2, char* _str3, char* _str4,int& _size)
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
void PackPacket(char* _buf, PROTOCOL _protocol, int& _size)
{
	char* ptr = _buf;

	_size = 0;

	ptr = ptr + sizeof(_size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	_size = _size + sizeof(_protocol);

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
void UnPackPacket(char* _buf, RESULT& _result, char* _id, char* _pw, char* _nick,char* _str1)
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
void PackPacket(char* _buf, PROTOCOL _protocol, int _data, int& _size)
{
	char* ptr = _buf;
	int size = 0;
	ptr = ptr + sizeof(size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	size = size + sizeof(size);

	memcpy(ptr, &_data, sizeof(int));
	ptr = ptr + sizeof(int);
	size = size + sizeof(size);

	ptr = _buf;

	memcpy(ptr, &size, sizeof(size));
	size = size + sizeof(size);

}

#pragma endregion


#pragma region 전역변수

bool chat_exit = false;
_User_Info myInfo;
SOCKET sock; // 소켓
char buf[BUFSIZE+1]; // 데이터 송수신 버퍼
char exitBuf[BUFSIZE + 1];

HANDLE hReadEvent, hWriteEvent; // 이벤트
HWND hSendButton; // 보내기 버튼
HWND hExitButton; // 나가기 버튼
HWND hEdit1, hEdit2; // 편집 컨트롤
ClientState c_state;
#pragma endregion


int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
                   LPSTR lpCmdLine, int nCmdShow)
{
	// 이벤트 생성
	hReadEvent = CreateEvent(NULL, FALSE, TRUE, NULL);
	if(hReadEvent == NULL) return 1;
	hWriteEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	if(hWriteEvent == NULL) return 1;

	// 소켓 통신 스레드 생성
	//CreateThread(NULL, 0, ClientMain, NULL, 0, NULL);


	hThread[0] = CreateThread(NULL, 0, ClientMain, NULL, 0, NULL);

	// 대화상자 생성
	DialogBox(hInstance, MAKEINTRESOURCE(IDD_DIALOG1), NULL, DlgProc);


	//WaitForMultipleObjects(2, hThread, true, INFINITE);

	// 이벤트 제거
	CloseHandle(hReadEvent);
	CloseHandle(hWriteEvent);
	CloseHandle(hThread[0]);
	CloseHandle(hThread[1]);
	// closesocket()
	closesocket(sock);

	// 윈속 종료
	WSACleanup();
	return 0;
}

// 대화상자 프로시저
BOOL CALLBACK DlgProc(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch(uMsg){
	case WM_INITDIALOG:
		hEdit1 = GetDlgItem(hDlg, IDC_EDIT1);
		hEdit2 = GetDlgItem(hDlg, IDC_EDIT2);
		hSendButton = GetDlgItem(hDlg, IDOK);
		hExitButton = GetDlgItem(hDlg, IDCANCEL);

		SendMessage(hEdit1, EM_SETLIMITTEXT, BUFSIZE, 0);
		return TRUE;
	case WM_COMMAND:
		switch(LOWORD(wParam)){
		case IDOK:
			EnableWindow(hSendButton, FALSE); // 보내기 버튼 비활성화
			WaitForSingleObject(hReadEvent, INFINITE); // 읽기 완료 기다리기
			GetDlgItemText(hDlg, IDC_EDIT1, buf, BUFSIZE+1);
			SetEvent(hWriteEvent); // 쓰기 완료 알리기
			SetWindowText(hEdit1, "");
			SetFocus(hEdit1);
			SendMessage(hEdit1, EM_SETSEL, 0, -1);
			return TRUE;
		case IDCANCEL:
			EndDialog(hDlg, IDCANCEL);
			
			return TRUE;
		}
		return FALSE;
	}
	return FALSE;
}

// 편집 컨트롤 출력 함수
void DisplayText(char *fmt, ...)
{
	va_list arg;
	va_start(arg, fmt);

	char cbuf[BUFSIZE+256];
	vsprintf(cbuf, fmt, arg);

	int nLength = GetWindowTextLength(hEdit2);
	SendMessage(hEdit2, EM_SETSEL, nLength, nLength);
	SendMessage(hEdit2, EM_REPLACESEL, FALSE, (LPARAM)cbuf);

	va_end(arg);
}

// 소켓 함수 오류 출력 후 종료
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

// 소켓 함수 오류 출력
void err_display(char *msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	DisplayText("[%s] %s", msg, (char *)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

// 사용자 정의 데이터 수신 함수
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

// TCP 클라이언트 시작 부분
DWORD WINAPI ClientMain(LPVOID arg)
{
	int retval;

	// 윈속 초기화
	WSADATA wsa;
	if(WSAStartup(MAKEWORD(2,2), &wsa) != 0)
		return 1;

	// socket()
	sock = socket(AF_INET, SOCK_STREAM, 0);
	if(sock == INVALID_SOCKET) err_quit("socket()");

	// connect()
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = AF_INET;
	serveraddr.sin_addr.s_addr = inet_addr(SERVERIP);
	serveraddr.sin_port = htons(SERVERPORT);
	retval = connect(sock, (SOCKADDR *)&serveraddr, sizeof(serveraddr));
	if(retval == SOCKET_ERROR) err_quit("connect()");

	int size;
	bool endflag = false;
	c_state = LOBBYSTATE;              //기본스테이트 입력.
	char chat_name[NICKNAMESIZE];
	memset(chat_name, 0, sizeof(chat_name));
	Menu select;

	char userID[IDSIZE];

	memset(userID, 0, sizeof(userID));

	

	// 서버와 데이터 통신
	while(1)
	{

		chat_exit = false;
		switch (c_state)
		{
		case LOBBYSTATE:
			//로비메뉴 while문 (로그인과 회원가입)
			while (1)
			{
				EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
				SetEvent(hReadEvent); // 읽기 완료 알리기
				DisplayText("<<메뉴>>\n");
				DisplayText("1. 회원가입\n");
				DisplayText("2. 로그인\n");

				WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

				// 문자열 길이가 0이면 보내지 않음
				if (strlen(buf) == 0)
				{
					EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
					SetEvent(hReadEvent); // 읽기 완료 알리기
					continue;
				}

				select = (Menu)atoi(buf);


				bool loop = false;


				switch (select)
				{
				case JOIN:
				{
					memset(buf, 0, sizeof(buf));
					EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
					SetEvent(hReadEvent); // 읽기 완료 알리기
					DisplayText("ID>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char id[IDSIZE];
					strcpy(id, buf);    //아이디 저장
					DisplayText("%s\n", id);


					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("PW>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char pw[PWSIZE];
					strcpy(pw, buf);    //비밀번호  저장
					DisplayText("%s\n", pw);


					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("닉네임>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}


					char nickname[NICKNAMESIZE];
					strcpy(nickname, buf);    //닉네임  저장
					DisplayText("%s\n", nickname);

					PackPacket(buf, JOIN_INFO, id, pw, nickname, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}
				}
				break;   //회원가입 case문 끝나는 곳
				case LOGIN:
				{
					memset(buf, 0, sizeof(buf));
					EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
					SetEvent(hReadEvent); // 읽기 완료 알리기
					DisplayText("ID>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char id[IDSIZE];
					strcpy(id, buf);    //아이디 저장
					strcpy(userID, id);
					DisplayText("%s\n", id);

					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("PW>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char pw[PWSIZE];
					strcpy(pw, buf);    //비밀번호  저장
					DisplayText("%s\n", pw);


					PackPacket(buf, LOGIN_INFO, id, pw, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}
				}
				break;  //로그인 case문 끝나는 곳

				default:
					DisplayText("잘못입력했습니다...\n");
					loop = true;
				}

				if (!loop)
					break;

			} //while(1)문 끝나는 곧.
			break;//LOBBYSTATE 케이스문 끝나는곳
		case MADE_AND_SHOW:
		{
			memset(buf, 0, sizeof(buf));
			EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
			SetEvent(hReadEvent); // 읽기 완료 알리기
			DisplayText("<<메뉴>>\n");
			DisplayText("3. 방 만들기\n");
			DisplayText("4. 채팅방 참가\n");

			WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

			// 문자열 길이가 0이면 보내지 않음
			if (strlen(buf) == 0)
			{
				EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
				SetEvent(hReadEvent); // 읽기 완료 알리기
				continue;
			}

			select = (Menu)atoi(buf);

			while (1)
			{
				bool loop = false;

				switch (select)
				{
				case CHAT_ROOM_MADE:
				{
					memset(buf, 0, sizeof(buf));
					//채팅방을 만드는데 필요한 내용을 입력해서 서버에 전달.
					EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
					SetEvent(hReadEvent); // 읽기 완료 알리기
					DisplayText("채팅방 이름을 입력해주세요:\n");

					WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

					// 문자열 길이가 0이면 보내지 않음
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
						SetEvent(hReadEvent); // 읽기 완료 알리기
						continue;
					}

					char room_name[ROOMNAMESIZE];
					strcpy(room_name, buf);
					DisplayText("%s\n", room_name);

					//여기서 닉네임도 같이 알려주기


					PackPacket(buf, CHAT_MADE, room_name,myInfo.nickname, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}

				}
				break;
				case CHAT_ATTEND:
				{

					memset(buf, 0, sizeof(buf));
					PackPacket(buf, CHAT_ROOM_SHOW, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}

				}
				break;

				default:
					loop = true;
				}

				if (!loop)
					break;
			}
		}
		break;  //MADE_AND_SELECT 케이스문 끝나는곳.
		case ROOM_SELECT:
		{
			memset(buf, 0, sizeof(buf));
			EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
			SetEvent(hReadEvent); // 읽기 완료 알리기
			DisplayText("채팅하실 방을 선택해 주세요:");
			SetWindowText(hExitButton, "이전");

			WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기

			// 문자열 길이가 0이면 보내지 않음
			if (strlen(buf) == 0)
			{
				EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
				SetEvent(hReadEvent); // 읽기 완료 알리기
				continue;
			}

			char Select[SELECTSIZE];
			strcpy(Select, buf);
			int select = atoi(Select);
			DisplayText("%d\n", select);

			
			PackPacket(buf, SELECT_CHATT_ROOM_NUMBER, myInfo.id,myInfo.pw,myInfo.nickname, Select ,size);

			int retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("send()");
				break;
			}

		}
		break;

		case CHATTINGSTATE:
		{
			char msg[BUFSIZE + 1];
			memset(msg, 0, sizeof(msg));

			strcpy(msg, "님이 입장하셨습니다.");
			
			//아이디를 보내줘야함.
			PackPacket(buf, CHATT_MSG, userID, msg, size);
			int retval = send(sock, buf, size, 0);
			if (retval == SOCKET_ERROR)
			{
				err_display("send()");
				break;
			}

			while (1)
			{

				if (chat_exit)
				{
					DisplayText("%s\r\n", exitBuf);
					break;
				}
					

				memset(msg, 0, sizeof(msg));
				EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
				SetEvent(hReadEvent); // 읽기 완료 알리기
				WaitForSingleObject(hWriteEvent, INFINITE); // 쓰기 완료 기다리기
				// 문자열 길이가 0이면 보내지 않음
				if (strlen(buf) == 0)
				{
					EnableWindow(hSendButton, TRUE); // 보내기 버튼 활성화
					SetEvent(hReadEvent); // 읽기 완료 알리기
					continue;
				}
				else
				{
					strcpy(msg, buf);

					PackPacket(buf, CHATT_MSG, userID, msg, size);

					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}
				}

			}



		}
		break;//CHATTINGSTATE state 케이스문 break



		}  //state case문 종료


		if (!chat_exit)
		{
			while (1)
			{
				bool loop = false;
				if (!PacketRecv(sock, buf))
				{
					break;
				}

				PROTOCOL protocol;

				GetProtocol(buf, protocol);

				RESULT result;
				char msg[BUFSIZE];
				memset(msg, 0, sizeof(msg));

				if (protocol != LOGIN_RESULT)
				{
					UnPackPacket(buf, result, msg);
					DisplayText("%s\n", msg);
				}
				else
				{
					UnPackPacket(buf, result, myInfo.id, myInfo.pw, myInfo.nickname, msg);
					DisplayText("%s\n", msg);
				}


				switch (protocol)
				{
				case JOIN_RESULT:
					break;

				case LOGIN_RESULT:
					switch (result)
					{
					case LOGIN_SUCCESS:
						c_state = MADE_AND_SHOW;
						break;
					case ID_ERROR:
					case PW_ERROR:
						break;
					}
					break;

				case CHAT_ROOM_SHOW:
					c_state = ROOM_SELECT;
					break;

				case SELECT_CHATT_ROOM_NUMBER:
					c_state = CHATTINGSTATE;           //채팅스테이트로 넘어가고
					hThread[1] = CreateThread(NULL, 0, RecvThread, NULL, 0, NULL); //채팅에서 사용할 리시브 스레드 생성.
					break;
				case CHAT_MADE:
					break;
				}

				if (!loop)
					break;
			}

		}

		
	
	}

	return 0;
}




DWORD CALLBACK RecvThread(LPVOID _ptr)
{
	PROTOCOL protocol;
	RESULT result;
	int size;
	char nickname[NICKNAMESIZE];
	char msg[BUFSIZE];
	int count;

	//DisplayText("쓰레드 들어옴");
	

	while (1)
	{

		if (!PacketRecv(sock, buf))
		{
			break;
		}

		GetProtocol(buf, protocol);

		bool flag = false;
		int data;
		switch (protocol)
		{
		case CHATT_MSG:
			memset(msg, 0, sizeof(msg));
			UnPackPacket(buf, result,msg);
			DisplayText("%s\r\n", msg);
			break;


		case EXIT:
			memset(msg, 0, sizeof(msg));
			memset(exitBuf, 0, sizeof(exitBuf));
			UnPackPacket(buf, result, msg);
			strcpy(exitBuf, msg);
			chat_exit = true;
			memset(buf, 0, sizeof(buf));
			c_state = MADE_AND_SHOW;

			SetEvent(hWriteEvent); // 쓰기 완료 알리기
			


			break;


		case CHATT_OUT:
			flag = true;
			break;
		}

		if (flag)
		{
			break;
		}

	}

	DisplayText("쓰레드 종료");
	return 0;
}