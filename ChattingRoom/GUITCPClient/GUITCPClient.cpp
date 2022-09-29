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
	JOIN=1,//ȸ������
	LOGIN,  //�α��� 
	CHAT_ROOM_MADE,  //ä�ù� �����
	CHAT_ATTEND     //ä�ù� ����.
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
	JOIN_INFO,           //ȸ������
	LOGIN_INFO,          //�α���
	JOIN_RESULT,         //ȸ�����԰��
	LOGIN_RESULT,        //�α��ΰ��
	LOGOUT,
	LOGOUT_RESULT,
	CHATTING,            //ä���ϴ°�
	CHAT_MADE,           //ä�ù� �����
	CHAT_ROOM_SHOW,      //ä�ù� �����ֱ�
	SELECT_CHATT_ROOM_NUMBER,   //ä�ù� �����ϱ�
	CHATT_MSG,                //ä�� ��������
	CHATT_OUT,                 //ä�� ��������
	EXIT                     //���� ��������

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

// ��ȭ���� ���ν���
BOOL CALLBACK DlgProc(HWND, UINT, WPARAM, LPARAM);
// ���� ��Ʈ�� ��� �Լ�
void DisplayText(char* fmt, ...);
// ���� ��� �Լ�
void err_quit(char* msg);
void err_display(char* msg);
// ����� ���� ������ ���� �Լ�
int recvn(SOCKET s, char* buf, int len, int flags);
// ���� ��� ������ �Լ�
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

#pragma region ������ ��ŷ ����ŷ






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


#pragma region ��������

bool chat_exit = false;
_User_Info myInfo;
SOCKET sock; // ����
char buf[BUFSIZE+1]; // ������ �ۼ��� ����
char exitBuf[BUFSIZE + 1];

HANDLE hReadEvent, hWriteEvent; // �̺�Ʈ
HWND hSendButton; // ������ ��ư
HWND hExitButton; // ������ ��ư
HWND hEdit1, hEdit2; // ���� ��Ʈ��
ClientState c_state;
#pragma endregion


int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
                   LPSTR lpCmdLine, int nCmdShow)
{
	// �̺�Ʈ ����
	hReadEvent = CreateEvent(NULL, FALSE, TRUE, NULL);
	if(hReadEvent == NULL) return 1;
	hWriteEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	if(hWriteEvent == NULL) return 1;

	// ���� ��� ������ ����
	//CreateThread(NULL, 0, ClientMain, NULL, 0, NULL);


	hThread[0] = CreateThread(NULL, 0, ClientMain, NULL, 0, NULL);

	// ��ȭ���� ����
	DialogBox(hInstance, MAKEINTRESOURCE(IDD_DIALOG1), NULL, DlgProc);


	//WaitForMultipleObjects(2, hThread, true, INFINITE);

	// �̺�Ʈ ����
	CloseHandle(hReadEvent);
	CloseHandle(hWriteEvent);
	CloseHandle(hThread[0]);
	CloseHandle(hThread[1]);
	// closesocket()
	closesocket(sock);

	// ���� ����
	WSACleanup();
	return 0;
}

// ��ȭ���� ���ν���
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
			EnableWindow(hSendButton, FALSE); // ������ ��ư ��Ȱ��ȭ
			WaitForSingleObject(hReadEvent, INFINITE); // �б� �Ϸ� ��ٸ���
			GetDlgItemText(hDlg, IDC_EDIT1, buf, BUFSIZE+1);
			SetEvent(hWriteEvent); // ���� �Ϸ� �˸���
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

// ���� ��Ʈ�� ��� �Լ�
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
	DisplayText("[%s] %s", msg, (char *)lpMsgBuf);
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

// TCP Ŭ���̾�Ʈ ���� �κ�
DWORD WINAPI ClientMain(LPVOID arg)
{
	int retval;

	// ���� �ʱ�ȭ
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
	c_state = LOBBYSTATE;              //�⺻������Ʈ �Է�.
	char chat_name[NICKNAMESIZE];
	memset(chat_name, 0, sizeof(chat_name));
	Menu select;

	char userID[IDSIZE];

	memset(userID, 0, sizeof(userID));

	

	// ������ ������ ���
	while(1)
	{

		chat_exit = false;
		switch (c_state)
		{
		case LOBBYSTATE:
			//�κ�޴� while�� (�α��ΰ� ȸ������)
			while (1)
			{
				EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
				SetEvent(hReadEvent); // �б� �Ϸ� �˸���
				DisplayText("<<�޴�>>\n");
				DisplayText("1. ȸ������\n");
				DisplayText("2. �α���\n");

				WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

				// ���ڿ� ���̰� 0�̸� ������ ����
				if (strlen(buf) == 0)
				{
					EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
					SetEvent(hReadEvent); // �б� �Ϸ� �˸���
					continue;
				}

				select = (Menu)atoi(buf);


				bool loop = false;


				switch (select)
				{
				case JOIN:
				{
					memset(buf, 0, sizeof(buf));
					EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
					SetEvent(hReadEvent); // �б� �Ϸ� �˸���
					DisplayText("ID>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char id[IDSIZE];
					strcpy(id, buf);    //���̵� ����
					DisplayText("%s\n", id);


					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("PW>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char pw[PWSIZE];
					strcpy(pw, buf);    //��й�ȣ  ����
					DisplayText("%s\n", pw);


					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("�г���>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}


					char nickname[NICKNAMESIZE];
					strcpy(nickname, buf);    //�г���  ����
					DisplayText("%s\n", nickname);

					PackPacket(buf, JOIN_INFO, id, pw, nickname, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}
				}
				break;   //ȸ������ case�� ������ ��
				case LOGIN:
				{
					memset(buf, 0, sizeof(buf));
					EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
					SetEvent(hReadEvent); // �б� �Ϸ� �˸���
					DisplayText("ID>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char id[IDSIZE];
					strcpy(id, buf);    //���̵� ����
					strcpy(userID, id);
					DisplayText("%s\n", id);

					EnableWindow(hSendButton, TRUE);
					SetEvent(hReadEvent);
					DisplayText("PW>>");
					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE);
						SetEvent(hReadEvent);
						continue;
					}
					char pw[PWSIZE];
					strcpy(pw, buf);    //��й�ȣ  ����
					DisplayText("%s\n", pw);


					PackPacket(buf, LOGIN_INFO, id, pw, size);
					int retval = send(sock, buf, size, 0);
					if (retval == SOCKET_ERROR)
					{
						err_display("send()");
						break;
					}
				}
				break;  //�α��� case�� ������ ��

				default:
					DisplayText("�߸��Է��߽��ϴ�...\n");
					loop = true;
				}

				if (!loop)
					break;

			} //while(1)�� ������ ��.
			break;//LOBBYSTATE ���̽��� �����°�
		case MADE_AND_SHOW:
		{
			memset(buf, 0, sizeof(buf));
			EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
			SetEvent(hReadEvent); // �б� �Ϸ� �˸���
			DisplayText("<<�޴�>>\n");
			DisplayText("3. �� �����\n");
			DisplayText("4. ä�ù� ����\n");

			WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

			// ���ڿ� ���̰� 0�̸� ������ ����
			if (strlen(buf) == 0)
			{
				EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
				SetEvent(hReadEvent); // �б� �Ϸ� �˸���
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
					//ä�ù��� ����µ� �ʿ��� ������ �Է��ؼ� ������ ����.
					EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
					SetEvent(hReadEvent); // �б� �Ϸ� �˸���
					DisplayText("ä�ù� �̸��� �Է����ּ���:\n");

					WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

					// ���ڿ� ���̰� 0�̸� ������ ����
					if (strlen(buf) == 0)
					{
						EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
						SetEvent(hReadEvent); // �б� �Ϸ� �˸���
						continue;
					}

					char room_name[ROOMNAMESIZE];
					strcpy(room_name, buf);
					DisplayText("%s\n", room_name);

					//���⼭ �г��ӵ� ���� �˷��ֱ�


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
		break;  //MADE_AND_SELECT ���̽��� �����°�.
		case ROOM_SELECT:
		{
			memset(buf, 0, sizeof(buf));
			EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
			SetEvent(hReadEvent); // �б� �Ϸ� �˸���
			DisplayText("ä���Ͻ� ���� ������ �ּ���:");
			SetWindowText(hExitButton, "����");

			WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���

			// ���ڿ� ���̰� 0�̸� ������ ����
			if (strlen(buf) == 0)
			{
				EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
				SetEvent(hReadEvent); // �б� �Ϸ� �˸���
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

			strcpy(msg, "���� �����ϼ̽��ϴ�.");
			
			//���̵� ���������.
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
				EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
				SetEvent(hReadEvent); // �б� �Ϸ� �˸���
				WaitForSingleObject(hWriteEvent, INFINITE); // ���� �Ϸ� ��ٸ���
				// ���ڿ� ���̰� 0�̸� ������ ����
				if (strlen(buf) == 0)
				{
					EnableWindow(hSendButton, TRUE); // ������ ��ư Ȱ��ȭ
					SetEvent(hReadEvent); // �б� �Ϸ� �˸���
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
		break;//CHATTINGSTATE state ���̽��� break



		}  //state case�� ����


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
					c_state = CHATTINGSTATE;           //ä�ý�����Ʈ�� �Ѿ��
					hThread[1] = CreateThread(NULL, 0, RecvThread, NULL, 0, NULL); //ä�ÿ��� ����� ���ú� ������ ����.
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

	//DisplayText("������ ����");
	

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

			SetEvent(hWriteEvent); // ���� �Ϸ� �˸���
			


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

	DisplayText("������ ����");
	return 0;
}