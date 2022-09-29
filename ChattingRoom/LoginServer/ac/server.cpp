/*
 * echo_selserv_win.c
 * Written by SW. YOON
 */

#define _CRT_SECURE_NO_WARNINGS  //strtok ���Ȱ��� ���� ������ ���� ����
#include <stdio.h>
#include <string.h>
#include <winsock2.h>

#define BUFSIZE 4096
#define IDSIZE 255
#define PWSIZE 255
#define NICKNAMESIZE 255
#define ROOMNAMESIZE 255
#define ERROR_DISCONNECTED -2
#define DISCONNECTED -1
#define SOC_TRUE 1
#define SOC_FALSE 0
#define USER_MAX 5

#define ID_ERROR_MSG "���� ���̵��Դϴ�\n"
#define PW_ERROR_MSG "�н����尡 Ʋ�Ƚ��ϴ�.\n"
#define LOGIN_SUCCESS_MSG "�α��ο� �����߽��ϴ�.\n"
#define ID_EXIST_MSG "�̹� �ִ� ���̵� �Դϴ�.\n"
#define JOIN_SUCCESS_MSG "���Կ� �����߽��ϴ�.\n"
#define LOGOUT_MSG "�α׾ƿ��Ǿ����ϴ�.\n"
#define ROOM_MADE_SUCCESS_MSG "�� ������ �Ϸ�Ǿ����ϴ�.\n"
#define SELECT_ROOM_MSG "ä���Ͻ� ���� ������ �ּ���...\n"
#define EXIT_MSG "���忡 ���� ������ϼ̽��ϴ�...\n"


/************************************************���� & ���ǻ��� ******************************************/

// 1. Ŭ���̾�Ʈ�� Ű�� ó���� �����Դϴ� �ѹ� ������ ��ư�� ������ �޴��� ���������� ���Դϴ�.
// 2. ���� ���� �������� �ʰ� ä�ù� ������ ������ �ڷΰ��Ⱑ ��� ���ٰ� �ٽ� �Ѿ��մϴ�. (�� ���� �����϶� �ٸ� �������� ���� ���� �� ����� ������Ʈ ���� �ʽ��ϴ�!!) 
// 3. �濡�� ���� ���ϰ� ä�ù� ������ ������ �� �̻� ������� �ʽ��ϴ�....


// 4. ���� ���� ����� /q [���г���] �� �Է��ϸ� �� ����� �����մϴ�   ############## ex) 'qwe'�� �����Ű�� ������ ->   /q qwe 
// 5. �ӼӸ��� ä�ù� �ȿ��ִ� ��� ����� ����� �� �ֽ��ϴ�. /w [���г���] [����] �� �Է��ϸ� �˴ϴ� ############## ex) 'qwe'���� �ӼӸ��� �ϰ�ʹٸ� -> /w qwe �ȳ��ϼ���~   

/***********************************************************************************************************/


enum STATE 
{
	NO_STATE = -1,
	INIT_STATE=1, 
	MENU_SELECT_STATE, 	
	LOGIN_STATE,	
	CHATTING_STATE,
	SEND_DELAY_STATE,
	DISCONNECTED_STATE
};

enum RESULT 
{
	NODATA = -1, 
	ID_EXIST = 1, 
	ID_ERROR, 
	PW_ERROR, 
	JOIN_SUCCESS, 
	LOGIN_SUCCESS,
	LOGOUT_SUCCESS,
	ROOM_MADE_SUCCESS
};

enum PROTOCOL 
{ 
	JOIN_INFO, 
	LOGIN_INFO, 
	JOIN_RESULT, 
	LOGIN_RESULT,
	LOGOUT,
	LOGOUT_RESULT,
	CHATTING,
	CHAT_MADE,
	CHAT_ROOM_SHOW,
	SELECT_CHATT_ROOM_NUMBER,
	CHATT_MSG,
	CHATT_OUT,
	EXIT
};


struct _User_Info
{
	char id[IDSIZE];
	char pw[PWSIZE];
	char nickname[NICKNAMESIZE];
};

struct _ClientInfo
{
	SOCKET		sock;
	SOCKADDR_IN addr;
	_User_Info  userinfo;
	STATE		next_state;
	STATE		state;
	bool		r_sizeflag;
	
	int			recvbytes;
	int			comp_recvbytes;
	int			sendbytes;
	int			comp_sendbytes;

	int         select_room_number;

	char        made_chat_name[ROOMNAMESIZE];   //
	char        select_number[ROOMNAMESIZE];    
	char        roomKing[NICKNAMESIZE];



	char		recvbuf[BUFSIZE];
	char		sendbuf[BUFSIZE];

};

struct _Chatt_Room_Info
{
	_ClientInfo* chattList[USER_MAX];
	char nicknamelist[USER_MAX][NICKNAMESIZE];
	char King[NICKNAMESIZE];  //����

	//const char* chatt_name;
	int chatt_count=0;



	char Chatting_Room_Name[ROOMNAMESIZE];
	int room_number = 0; //�� ��ȣ.
	bool full = false;
};


_Chatt_Room_Info ChattRoomList[10];
int chat_room_index = 0;

//_Chatt_Room_Info ChattRoom;
//int current_index = 0;

char Chatting_Room_Name_List[3][ROOMNAMESIZE];

_ClientInfo* User_List[100];
int Count = 0;
_User_Info* Join_List[100];
int Join_Count = 0;

FD_SET Rset, Wset;

void err_quit(const char* msg);
void err_display(const char* msg);
int recvn(SOCKET s, char* buf, int len, int flags);
void GetProtocol(const char* _ptr, PROTOCOL& _protocol);
int PackPacket(char* _buf, PROTOCOL _protocol, RESULT _result, const char* _str1);
void UnPackPacket(const char* _buf, char* _str1, char* _str2, char* _str3);
void UnPackPacket(const char* _buf, char* _str1, char* _str2);


BOOL SearchFile(const char* filename);
bool FileDataLoad();
bool FileDataAdd(_User_Info* _info);
int MessageRecv(_ClientInfo* _info);
int MessageSend(_ClientInfo* _info);
int PacketRecv(_ClientInfo* _ptr);
_ClientInfo* AddClient(SOCKET _sock, SOCKADDR_IN _clientaddr);
void RemoveClient(_ClientInfo* _ptr);
void RecvPacketProcess(_ClientInfo* _ptr);
void SendPacketProcess(_ClientInfo* _ptr);
void JoinProcess(_ClientInfo* _ptr);
void LoginProcess(_ClientInfo* _ptr);
void Logoutprocess(_ClientInfo* _ptr);
void RoomSelect(_ClientInfo* _ptr);

int main(int argc, char **argv)
{
	WSADATA wsaData;
	SOCKET hServSock;
	SOCKADDR_IN servAddr;
	SOCKET hClntSock;
	SOCKADDR_IN clntAddr;
	int retval;

	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) /* Load Winsock 2.2 DLL */
		err_quit("WSAStartup() error!");

	hServSock = socket(PF_INET, SOCK_STREAM, 0);
	if (hServSock == INVALID_SOCKET)
		err_quit("socket() error");

	servAddr.sin_family = AF_INET;
	servAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	servAddr.sin_port = htons(9000);

	u_long on = 1;
	retval = ioctlsocket(hServSock, FIONBIO, &on);
	if (retval == SOCKET_ERROR) err_display("ioctlsocket()");


	if (bind(hServSock, (SOCKADDR*)&servAddr, sizeof(servAddr)) == SOCKET_ERROR)
		err_quit("bind() error");
	if (listen(hServSock, 5) == SOCKET_ERROR)
		err_quit("listen() error");

	if (!FileDataLoad())
	{
		err_quit("file read error!");
	}

	while (1)
	{
		FD_ZERO(&Rset);  //�ʱ�ȭ.
		FD_ZERO(&Wset);  //�ʱ�ȭ

		FD_SET(hServSock, &Rset);  //������ ������ �б�.

		for (int i = 0; i < Count; i++)
		{
			FD_SET(User_List[i]->sock, &Rset);

			if (User_List[i]->state == SEND_DELAY_STATE)
			{
				FD_SET(User_List[i]->sock, &Wset);
			}					
		}

		if (select(0, &Rset, &Wset, 0, NULL) == SOCKET_ERROR)
		{
			err_quit("select() error");
		}			

		if (FD_ISSET(hServSock, &Rset)) 
		{
			int clntLen = sizeof(clntAddr);
			hClntSock = accept(hServSock, (SOCKADDR*)&clntAddr, &clntLen);
			_ClientInfo* ptr = AddClient(hClntSock, clntAddr);
			ptr->state = MENU_SELECT_STATE;
			continue;
		}		

		for (int i = 0; i < Count; i++)
		{
			_ClientInfo* ptr = User_List[i];

			if (FD_ISSET(ptr->sock, &Rset))
			{				
				int result = PacketRecv(ptr);

				switch (result)
				{
				case DISCONNECTED:
					ptr->state = DISCONNECTED_STATE;
					break;
				case SOC_FALSE:
					continue;
				case SOC_TRUE:
					break;
				}

				RecvPacketProcess(ptr);
			}					

			if (FD_ISSET(ptr->sock, &Wset))
			{
				int result = MessageSend(ptr);
				switch (result)
				{
				case ERROR_DISCONNECTED:
					err_display("connect end");
				case DISCONNECTED:
					ptr->state = DISCONNECTED_STATE;
					break;
				case SOC_FALSE:
					continue;
				case SOC_TRUE:
					break;
				}

				SendPacketProcess(ptr);				
			}

			if (ptr->state == DISCONNECTED_STATE)
			{
				RemoveClient(ptr);
				i--;
				continue;
			}

		}
	}	

	closesocket(hServSock);
	WSACleanup();
	return 0;
}


void err_quit(const char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, msg, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(-1);
}

// ���� �Լ� ���� ���
void err_display(const char* msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s", msg, (LPCTSTR)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

int recvn(SOCKET s, char* buf, int len, int flags)
{
	int received;
	char* ptr = buf;
	int left = len;

	while (left > 0)
	{
		received = recv(s, ptr, left, flags);
		if (received == SOCKET_ERROR)
			return SOCKET_ERROR;
		else if (received == 0)
			break;
		left -= received;
		ptr += received;
	}

	return (len - left);
}

void GetProtocol(const char* _ptr, PROTOCOL& _protocol)
{
	memcpy(&_protocol, _ptr, sizeof(PROTOCOL));

}

int PackPacket(char* _buf, PROTOCOL _protocol, RESULT _result, const char* _str1)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);
	int size = 0;

	ptr = ptr + sizeof(size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	size = size + sizeof(_protocol);

	memcpy(ptr, &_result, sizeof(_result));
	ptr = ptr + sizeof(_result);
	size = size + sizeof(_result);

	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	size = size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	size = size + strsize1;

	ptr = _buf;
	memcpy(ptr, &size, sizeof(size));

	size = size + sizeof(size);

	return size;
}

int PackPacket(char* _buf, PROTOCOL _protocol, RESULT _result, const char* _id, const char* _pw, const char* _nick,const char* _str1)
{
	char* ptr = _buf;
	int strsize1 = strlen(_str1);

	int idsize = strlen(_id);
	int pwsize = strlen(_pw);
	int nicknamesize = strlen(_nick);


	int size = 0;

	ptr = ptr + sizeof(size);

	memcpy(ptr, &_protocol, sizeof(_protocol));
	ptr = ptr + sizeof(_protocol);
	size = size + sizeof(_protocol);

	memcpy(ptr, &_result, sizeof(_result));
	ptr = ptr + sizeof(_result);
	size = size + sizeof(_result);

	//

	//id
	memcpy(ptr, &idsize, sizeof(idsize));
	ptr = ptr + sizeof(idsize);
	size = size + sizeof(idsize);

	memcpy(ptr, _id, idsize);
	ptr = ptr + idsize;
	size = size + idsize;

	//pw
	memcpy(ptr, &pwsize, sizeof(pwsize));
	ptr = ptr + sizeof(pwsize);
	size = size + sizeof(pwsize);

	memcpy(ptr, _pw, pwsize);
	ptr = ptr + pwsize;
	size = size + pwsize;

	//nickname
	memcpy(ptr, &nicknamesize, sizeof(nicknamesize));
	ptr = ptr + sizeof(nicknamesize);
	size = size + sizeof(nicknamesize);

	memcpy(ptr, _nick, nicknamesize);
	ptr = ptr + nicknamesize;
	size = size + nicknamesize;

	//



	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	size = size + sizeof(strsize1);

	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	size = size + strsize1;

	ptr = _buf;
	memcpy(ptr, &size, sizeof(size));

	size = size + sizeof(size);

	return size;
}

void UnPackPacket(const char* _buf, char* _str1, char* _str2, char* _str3)
{
	int str1size, str2size, str3size;

	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;

	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;

	memcpy(&str3size, ptr, sizeof(str3size));
	ptr = ptr + sizeof(str3size);

	memcpy(_str3, ptr, str3size);
	ptr = ptr + str3size;
}

void UnPackPacket(const char* _buf, char* _str1, char* _str2, char* _str3, char* _str4)
{
	int str1size, str2size, str3size, str4size;

	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;

	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;

	memcpy(&str3size, ptr, sizeof(str3size));
	ptr = ptr + sizeof(str3size);

	memcpy(_str3, ptr, str3size);
	ptr = ptr + str3size;

	memcpy(&str4size, ptr, sizeof(str4size));
	ptr = ptr + sizeof(str4size);

	memcpy(_str4, ptr, str4size);
	ptr = ptr + str4size;


}

void UnPackPacket(const char* _buf, char* _str1, char* _str2)
{
	int str1size, str2size;

	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;

	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;
}

void UnPackPacket(const char* _buf, char* _str1)
{
	int str1size;

	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;
	
}


BOOL SearchFile(const char* filename)
{
	WIN32_FIND_DATA FindFileData;
	HANDLE hFindFile = FindFirstFile(filename, &FindFileData);
	if (hFindFile == INVALID_HANDLE_VALUE)
		return FALSE;
	else {
		FindClose(hFindFile);
		return TRUE;
	}
}

bool FileDataLoad()
{
	if (!SearchFile("UserInfo.info"))
	{
		FILE* fp = fopen("UserInfo.info", "wb");
		fclose(fp);
		return true;
	}

	FILE* fp = fopen("UserInfo.info", "rb");
	if (fp == NULL)
	{
		return false;
	}

	_User_Info info;
	memset(&info, 0, sizeof(_User_Info));

	while (1)
	{
		fread(&info, sizeof(_User_Info), 1, fp);
		if (feof(fp))
		{
			break;
		}
		_User_Info* ptr = new _User_Info;
		memcpy(ptr, &info, sizeof(_User_Info));
		Join_List[Join_Count++] = ptr;
	}

	fclose(fp);
	return true;
}

bool FileDataAdd(_User_Info* _info)
{
	FILE* fp = fopen("UserInfo.info", "ab");
	if (fp == NULL)
	{
		return false;
	}

	int retval = fwrite(_info, 1, sizeof(_User_Info), fp);

	if (retval != sizeof(_User_Info))
	{
		fclose(fp);
		return false;
	}

	fclose(fp);
	return true;
}

int MessageRecv(_ClientInfo* _info)
{
	int retval = recv(_info->sock, _info->recvbuf + _info->comp_recvbytes, _info->recvbytes - _info->comp_recvbytes, 0);
	if (retval == SOCKET_ERROR) //�������������û�� ���
	{
		return ERROR_DISCONNECTED;
	}
	else if (retval == 0)
	{
		return DISCONNECTED;
	}
	else
	{
		_info->comp_recvbytes += retval;
		if (_info->comp_recvbytes == _info->recvbytes)
		{
			_info->comp_recvbytes = 0;
			_info->recvbytes = 0;
			return SOC_TRUE;
		}
		return SOC_FALSE;
	}

}

int MessageSend(_ClientInfo* _info)
{
	int retval = send(_info->sock, _info->sendbuf + _info->comp_sendbytes,
		_info->sendbytes - _info->comp_sendbytes, 0);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() == WSAEWOULDBLOCK)
		{
			return SOC_FALSE;
		}
		return ERROR_DISCONNECTED;
	}	
	else
	{
		_info->comp_sendbytes = _info->comp_sendbytes + retval;

		if (_info->sendbytes == _info->comp_sendbytes)
		{
			_info->sendbytes = 0;
			_info->comp_sendbytes = 0;

			return SOC_TRUE;
		}
		else
		{
			return SOC_FALSE;
		}
	}
}

int PacketRecv(_ClientInfo* _ptr)
{
	if (!_ptr->r_sizeflag)
	{
		_ptr->recvbytes = sizeof(int);
		int retval = MessageRecv(_ptr);
		switch (retval)
		{
		case SOC_TRUE:
			memcpy(&_ptr->recvbytes, _ptr->recvbuf, sizeof(int));
			_ptr->r_sizeflag = true;
			return SOC_FALSE;
		case SOC_FALSE:
			return SOC_FALSE;
		case ERROR_DISCONNECTED:
			err_display("recv error()");
			return DISCONNECTED;
		case DISCONNECTED:
			return DISCONNECTED;
		}
	}

	int retval = MessageRecv(_ptr);  //���⼭ �̹� Ŭ���̾�Ʈ�� ���� �������� ��������.
	switch (retval)
	{
	case SOC_TRUE:
		_ptr->r_sizeflag = false;
		return SOC_TRUE;
	case SOC_FALSE:
		return SOC_FALSE;
	case ERROR_DISCONNECTED:
		err_display("recv error()");
		return DISCONNECTED;
	case DISCONNECTED:
		return DISCONNECTED;
	}
}


void UnPackPacket(const char* _buf, int& _data)
{
	const char* ptr = _buf + sizeof(PROTOCOL);

	memcpy(&_data, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

}

void MaKeChattMessage(const char* _nick, const char* _msg, char* _chattmsg)
{
	sprintf(_chattmsg, "[ %s ] %s", _nick, _msg);
}

void W_MaKeChattMessage(const char* _nick, const char* _msg, char* _chattmsg)  //�ӼӸ�.
{
	sprintf(_chattmsg, "Wisper:[ %s ] %s", _nick, _msg);
}

void ChattingMessageProcess(_ClientInfo* _clientinfo)
{

	char msg[BUFSIZE];
	char temp[BUFSIZE];
	int size;

	memset(msg, 0, sizeof(msg));
	memset(temp, 0, sizeof(temp));


	
	//Ŭ�󿡼� ������ userid,msg�������� ��)
	UnPackPacket(_clientinfo->recvbuf, _clientinfo->userinfo.id , msg);

	int join_checkindex = 0;       //���� ������ id�� ��ȸ�ؼ� ������ �� �ε����� ������.
	for (int i = 0; i < Join_Count; i++)
	{
		if (strcmp(Join_List[i]->id, _clientinfo->userinfo.id)==0)
		{
			join_checkindex = i;
			break;
		}
	}

	////////////////////////////////////////////////////////////// -> checkindex = ���� Ŭ���̾�Ʈ�� id��.
	


	int select = atoi(_clientinfo->select_number);

	int room_index = 0;

	//client�� ������ �� ��ȣ ã��
	for (int i = 0; i < chat_room_index; i++)
	{
		if (ChattRoomList[i].room_number == select)
		{
			room_index = i;
			break;
		}

	}
	////////////////////////////////////////////////////////////////

	//���� ������ ī��
	char msgcpy[BUFSIZE];
	strcpy(msgcpy, msg);

	printf("[msg]:%s ,[msgcpy]:%s\n", msg, msgcpy);  //������

	char w_macro[10];   //  /w üũ   Ȥ�� /q üũ
	char w_nickname[NICKNAMESIZE];  //�ӼӸ� ��� üũ
	char w_content[BUFSIZE];       //�ӼӸ� ���� üũ
	bool w_flag = false;           //�ӼӸ� üũ
	bool q_flag = false;           //���� üũ

	memset(w_macro, 0, sizeof(w_macro));
	memset(w_nickname, 0, sizeof(w_nickname));
	memset(w_content, 0, sizeof(w_content));
	
	//ī�ǵ� msg���� ù��° ������ ���Ë����� �ڸ� 
	char* wisper = strtok(msgcpy, " ");
	strcpy(w_macro, wisper);  


	if (strcmp(w_macro, "/w") == 0) //w_macro��     /w�ϰ��
	{

		wisper = strtok(NULL, " ");  //ī�ǵ� msg���� �ι�° ������ ���Ë� ���� �ڸ�.
		strcpy(w_nickname, wisper);

		wisper = strtok(NULL, "\0");  //ī�ǵ� msg���� Null���ڰ� ���Ë����� �ڸ�.
		strcpy(w_content, wisper);

		w_flag = true;
		q_flag = false;
	}
	else if (strcmp(w_macro, "/q") == 0)
	{

		  //�� ����� ���� ���� ����� ���.
		if (strcmp(Join_List[join_checkindex]->nickname , ChattRoomList[room_index].King)==0)
		{
			q_flag = true;
			w_flag = false;

			wisper = strtok(NULL, " ");  //ī�ǵ� msg���� �ι�° ������ ���Ë� ���� �ڸ�.
			strcpy(w_nickname, wisper);
		}
	}
	else
	{
		w_flag = false;
		q_flag = false;
	}


	printf("%s  %s  %s\n", w_macro, w_nickname, w_content);  //������ Ȯ��.(������)

	if (w_flag)
	{
		W_MaKeChattMessage(Join_List[join_checkindex]->nickname, w_content, temp);          //�ӼӸ� ���� ��ȭ�������
	}
	else
		MaKeChattMessage(Join_List[join_checkindex]->nickname, msg, temp);                 //�Ϲ� ��ȭ���� ����
	
	

	//result, protocol setting
	RESULT c_result =NODATA;
	PROTOCOL protocol;

	
	

	if (!w_flag && !q_flag)
	{
		protocol = CHATT_MSG;
		for (int i = 0; i < ChattRoomList[room_index].chatt_count; i++)
		{
			ChattRoomList[room_index].chattList[i]->sendbytes = PackPacket(ChattRoomList[room_index].chattList[i]->sendbuf, protocol, c_result, temp);
			int result = MessageSend(ChattRoomList[room_index].chattList[i]);
		}
	}
	else if (q_flag && !w_flag)   
	{

		protocol = EXIT;
		printf("���� ���� ����");
		strcpy(temp, EXIT_MSG);

		int check = 0;
		for (int i = 0; i < ChattRoomList[room_index].chatt_count; i++)
		{
			if (strcmp(ChattRoomList[room_index].chattList[i]->userinfo.nickname, w_nickname) == 0)
			{
				ChattRoomList[room_index].chattList[i]->sendbytes = PackPacket(ChattRoomList[room_index].chattList[i]->sendbuf, protocol, c_result, temp);
				int result = MessageSend(ChattRoomList[room_index].chattList[i]);
				break;
			}

		}

	}
	else if(!q_flag && w_flag)
	{

		protocol = CHATT_MSG;
		printf("�ӼӸ� ���� ����");

		int check = 0;
		for (int i = 0; i < ChattRoomList[room_index].chatt_count; i++)
		{
			if (strcmp(ChattRoomList[room_index].chattList[i]->userinfo.nickname, w_nickname) == 0)
			{
				ChattRoomList[room_index].chattList[i]->sendbytes = PackPacket(ChattRoomList[room_index].chattList[i]->sendbuf, protocol, c_result, temp);
				int result = MessageSend(ChattRoomList[room_index].chattList[i]);
				break;
			}

		}
		
		
	}
	
	
}

//void ExitChattRoom(_ClientInfo* _clientinfo)
//{
//	
//	for (int i = 0; i < ChattRoom.chatt_count; i++)
//	{
//		if (ChattRoom.chattList[i] == _clientinfo)
//		{
//			for (int j = i; j < ChattRoom.chatt_count - 1; j++)
//			{
//				ChattRoom.chattList[j] = ChattRoom.chattList[j + 1];
//			}
//
//			ChattRoom.chatt_count--;
//			break;
//		}
//	}
//}

void MakeExitMessage(const char* _nick, char* _msg)
{
	sprintf(_msg, "%s���� �����ϼ̽��ϴ�.", _nick);
}

//void ChattingOutProcess(_ClientInfo* _clientinfo)
//{
//	char msg[BUFSIZE];
//	int size;
//
//	MakeExitMessage(_clientinfo->userinfo.nickname, msg);
//
//	RESULT c_result= NODATA;
//
//	for (int i = 0; i < ChattRoom.chatt_count; i++)
//	{
//		if (ChattRoom.chattList[i] == _clientinfo)
//		{
//			PackPacket(ChattRoom.chattList[i]->sendbuf, CHATT_OUT, c_result,msg);
//			if (send(ChattRoom.chattList[i]->sock, ChattRoom.chattList[i]->sendbuf, size, 0) == SOCKET_ERROR)
//			{
//				err_display("chatting exit message Send()");
//				ChattRoom.chattList[i]->state = DISCONNECTED_STATE;
//			}
//
//		}
//		else
//		{
//			PackPacket(ChattRoom.chattList[i]->sendbuf, CHATT_MSG, c_result,msg);
//			if (send(ChattRoom.chattList[i]->sock, ChattRoom.chattList[i]->sendbuf, size, 0) == SOCKET_ERROR)
//			{
//				err_display("chatting exit message Send()");
//				ChattRoom.chattList[i]->state = DISCONNECTED_STATE;
//			}
//		}
//	}
//
//	ExitChattRoom(_clientinfo);
//}


_ClientInfo* AddClient(SOCKET _sock, SOCKADDR_IN _clientaddr)
{
	printf("\nClient ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n", inet_ntoa(_clientaddr.sin_addr),
		ntohs(_clientaddr.sin_port));

	//���� ����ü �迭�� ���ο� ���� ���� ����ü ����
	_ClientInfo* ptr = new _ClientInfo;
	ZeroMemory(ptr, sizeof(_ClientInfo));

	ptr->sock = _sock;
	memcpy(&ptr->addr, &_clientaddr, sizeof(SOCKADDR_IN));
	ptr->next_state = NO_STATE;
	ptr->state = INIT_STATE;
	ptr->r_sizeflag = false;
	User_List[Count++] = ptr;
	return ptr;
}

void RemoveClient(_ClientInfo* _ptr)
{
	closesocket(_ptr->sock);

	printf("\nClient ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n", inet_ntoa(_ptr->addr.sin_addr), ntohs(_ptr->addr.sin_port));

	for (int i = 0; i < Count; i++)
	{
		if (User_List[i] == _ptr)
		{
			delete User_List[i];

			for (int j = i; j < Count - 1; j++)
			{
				User_List[j] = User_List[j + 1];
			}
			User_List[Count - 1] = nullptr;
			Count--;
			break;
		}
	}

}


void ChatMadeProcess(_ClientInfo* _ptr)
{
	RESULT c_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	strcpy(msg, ROOM_MADE_SUCCESS_MSG);

	protocol = CHAT_MADE;




	strcpy(ChattRoomList[chat_room_index].Chatting_Room_Name, _ptr->made_chat_name); 


	ChattRoomList[chat_room_index].room_number = chat_room_index + 1;  //ó���̸� 1����.

	//���� ���� ����� �̸��� �޾Ƽ� ����.
	strcpy(ChattRoomList[chat_room_index].King, _ptr->roomKing);

	chat_room_index++;



	_ptr->sendbytes = PackPacket(_ptr->sendbuf, protocol, c_result, msg);   
	int result = MessageSend(_ptr);



	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		_ptr->next_state = MENU_SELECT_STATE;
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:
		break;
	}


}

void ChatRoomListSend(_ClientInfo* _ptr)
{
	RESULT c_result = NODATA;

	char msg[BUFSIZE];
	char temp[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));


	for(int i = 0; i < chat_room_index; i++)
	{
		itoa(i + 1, temp, 10);  //���� ���ڿ��� ��ȯ
		strcat(msg, temp );     //(i+1) ...
		strcat(msg, ".");       // (i+1).  ...
		strcat(msg, ChattRoomList[i].Chatting_Room_Name);  //(i+1).���̸�
		strcat(msg, "\r\n");
	}

	protocol = CHAT_ROOM_SHOW;

	_ptr->sendbytes = PackPacket(_ptr->sendbuf, protocol, c_result, msg);  
	int result = MessageSend(_ptr);


	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		_ptr->next_state = MENU_SELECT_STATE;
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:
		break;
	}

}


void RoomSelect(_ClientInfo* _ptr)
{
	RESULT c_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	//int select = atoi(_ptr->select_number);
	//printf("%d", select);  ->1����

	_ptr->select_room_number = atoi(_ptr->select_number);


	int check_index;

	for (int i = 0; i < chat_room_index; i++)
	{
		if (ChattRoomList[i].room_number == _ptr->select_room_number)
		{
			check_index = i;
			break;
		}
	}

	ChattRoomList[check_index].chattList[ChattRoomList[check_index].chatt_count] = _ptr;
	ChattRoomList[check_index].chattList[ChattRoomList[check_index].chatt_count]->userinfo = _ptr->userinfo;
	ChattRoomList[check_index].chatt_count++;
	protocol = SELECT_CHATT_ROOM_NUMBER;


	_ptr->sendbytes = PackPacket(_ptr->sendbuf, protocol, c_result, msg);  
	int result = MessageSend(_ptr);

	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		_ptr->next_state = MENU_SELECT_STATE;
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:
		break;
	}



}

void RecvPacketProcess(_ClientInfo* _ptr)
{
	PROTOCOL protocol;

	GetProtocol(_ptr->recvbuf, protocol);

	switch (_ptr->state)
	{
	case MENU_SELECT_STATE:	
		switch (protocol)
		{
		case JOIN_INFO:
			memset(&_ptr->userinfo, 0, sizeof(_User_Info));
			UnPackPacket(_ptr->recvbuf, _ptr->userinfo.id, _ptr->userinfo.pw, _ptr->userinfo.nickname);
			JoinProcess(_ptr);			
			break;
		case LOGIN_INFO:
			memset(&_ptr->userinfo, 0, sizeof(_User_Info));
			UnPackPacket(_ptr->recvbuf, _ptr->userinfo.id, _ptr->userinfo.pw);
			LoginProcess(_ptr);
			break;	
		}
		break;
	case CHATTING_STATE:
		switch (protocol)
		{
		case CHAT_MADE:
			memset(&_ptr->userinfo, 0, sizeof(_User_Info));
			UnPackPacket(_ptr->recvbuf, _ptr->made_chat_name, _ptr->roomKing);
			ChatMadeProcess(_ptr);
			break;

		case CHAT_ROOM_SHOW:
			ChatRoomListSend(_ptr);
			break;


		case SELECT_CHATT_ROOM_NUMBER:
			memset(&_ptr->userinfo, 0, sizeof(_User_Info));
			//UnPackPacket(_ptr->recvbuf, _ptr->select_number);
			UnPackPacket(_ptr->recvbuf, _ptr->userinfo.id, _ptr->userinfo.pw, _ptr->userinfo.nickname,_ptr->select_number);
			RoomSelect(_ptr);
			break;

		case CHATT_MSG:
			ChattingMessageProcess(_ptr);
			break;
		case CHATT_OUT:
			//ChattingOutProcess(_ptr);
			break;
		}

		break;

	
	}

}

void Logoutprocess(_ClientInfo* _ptr)
{
	_ptr->sendbytes = PackPacket(_ptr->sendbuf, LOGOUT_RESULT, LOGOUT_SUCCESS, LOGOUT_MSG);

	int result = MessageSend(_ptr);
	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		_ptr->next_state = MENU_SELECT_STATE;
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:
		break;
	}

	_ptr->state = MENU_SELECT_STATE;
}

void SendPacketProcess(_ClientInfo* _ptr)
{
	_ptr->state = _ptr->next_state;
	_ptr->next_state = NO_STATE;
}


void JoinProcess(_ClientInfo* _ptr)
{
	RESULT join_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	for (int i = 0; i < Join_Count; i++)
	{
		if (!strcmp(Join_List[i]->id, _ptr->userinfo.id))
		{
			join_result = ID_EXIST;
			strcpy(msg, ID_EXIST_MSG);
			break;
		}
	}


	if (join_result == NODATA)
	{
		_User_Info* user = new _User_Info;
		memset(user, 0, sizeof(_User_Info));
		strcpy(user->id, _ptr->userinfo.id);
		strcpy(user->pw, _ptr->userinfo.pw);
		strcpy(user->nickname, _ptr->userinfo.nickname);

		FileDataAdd(user);

		Join_List[Join_Count++] = user;
		join_result = JOIN_SUCCESS;
		strcpy(msg, JOIN_SUCCESS_MSG);
	}

	protocol = JOIN_RESULT;

	_ptr->sendbytes = PackPacket(_ptr->sendbuf, protocol, join_result, msg);
	int result = MessageSend(_ptr);
	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		_ptr->next_state = MENU_SELECT_STATE;
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:		
		break;
	}

}




void LoginProcess(_ClientInfo* _ptr)
{
	RESULT login_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;
	_User_Info* myInfo = new _User_Info;



	for (int i = 0; i < Join_Count; i++)
	{
		if (!strcmp(Join_List[i]->id, _ptr->userinfo.id))
		{
			if (!strcmp(Join_List[i]->pw, _ptr->userinfo.pw))
			{
				login_result = LOGIN_SUCCESS;
				strcpy(msg, LOGIN_SUCCESS_MSG);

				myInfo = Join_List[i];
			}
			else
			{
				login_result = PW_ERROR;
				strcpy(msg, PW_ERROR_MSG);
			}
			break;
		}
	}

	if (login_result == NODATA)
	{
		login_result = ID_ERROR;
		strcpy(msg, ID_ERROR_MSG);
	}

	if (login_result != LOGIN_SUCCESS)
	{
		memset(&(_ptr->userinfo), 0, sizeof(_User_Info));
	}

	protocol = LOGIN_RESULT;

	_ptr->sendbytes = PackPacket(_ptr->sendbuf, protocol, login_result, myInfo->id,myInfo->pw,myInfo->nickname,msg);
	int result = MessageSend(_ptr);


	switch (result)
	{
	case ERROR_DISCONNECTED:
		err_display("connect end");
	case DISCONNECTED:
		_ptr->state = DISCONNECTED_STATE;
		break;
	case SOC_FALSE:
		if (login_result == LOGIN_SUCCESS)          //�α����� ������ ��� ä�� ������Ʈ�� �ٲ��ֱ�
		{
			_ptr->next_state = CHATTING_STATE;
		}
		else
		{
			_ptr->next_state = MENU_SELECT_STATE;
		}
		_ptr->state = SEND_DELAY_STATE;
		return;
	case SOC_TRUE:
		break;
	}

	if (login_result == LOGIN_SUCCESS)
	{
		_ptr->state = CHATTING_STATE;
	}

}
