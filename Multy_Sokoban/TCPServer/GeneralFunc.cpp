#include "Global.h"

_ClientInfo* AddClientInfo(SOCKET _sock)
{
	SOCKADDR_IN clientaddr;
	int addrlen = sizeof(clientaddr);
	getpeername(_sock, (SOCKADDR*)&clientaddr, &addrlen);

	EnterCriticalSection(&cs);

	_ClientInfo* ptr = new _ClientInfo;

	memset(ptr, 0, sizeof(_ClientInfo));
	ptr->sock = _sock;
	memcpy(&ptr->addr, &clientaddr, sizeof(SOCKADDR_IN));
	ptr->r_sizeflag = false;
	ptr->result = NODATA;

	ptr->state = STATE::INIT_STATE;

	ptr->r_overlapped.ptr = ptr;
	ptr->r_overlapped.type = IO_TYPE::IO_RECV;

	ptr->s_overlapped.ptr = ptr;
	ptr->s_overlapped.type = IO_TYPE::IO_SEND;
	ptr->win = 0;
	ptr->lose = 0;
	ptr->Ready = false;


	for (int i = 0; i < MAPSIZE_Y; i++)
	{
		for (int j = 0; j < MAPSIZE_X; j++)
		{
			if (i == 0 || i == MAPSIZE_Y - 1) //벽
				ptr->MapInfo[i][j] = 0;
			else if (j == 0 || j == MAPSIZE_X - 1) //벽
				ptr->MapInfo[i][j] = 0;
			else if (i == 1 && j == 1 || i==8 && j==1)     //창고
				ptr->MapInfo[i][j] = 2;
			else if ((i % 4 == 0 && i != 0) && j == MAPSIZE_X/2) //물건
				ptr->MapInfo[i][j] = 3;
			else if (i == MAPSIZE_Y / 2 && j == MAPSIZE_X / 2)  //플레이어
				ptr->MapInfo[i][j] = -1;
			else  //이동가능 구역
				ptr->MapInfo[i][j] = 1;
		}
		
	}

	ClientInfo[Count] = ptr;
	Count++;
	LeaveCriticalSection(&cs);

	printf("\n[TCP 서버] 클라이언트 접속: IP 주소=%s, 포트 번호=%d\n",
		inet_ntoa(ptr->addr.sin_addr), ntohs(ptr->addr.sin_port));

	return ptr;
}

void RemoveClientInfo(_ClientInfo* _ptr)
{


	printf("[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n",
		inet_ntoa(_ptr->addr.sin_addr), ntohs(_ptr->addr.sin_port));

	EnterCriticalSection(&cs);
	for (int i = 0; i < Count; i++)
	{
		if (ClientInfo[i] == _ptr)
		{
			closesocket(ClientInfo[i]->sock);

			delete ClientInfo[i];

			for (int j = i; j < Count - 1; j++)
			{
				ClientInfo[j] = ClientInfo[j + 1];
			}
			ClientInfo[Count-1] = nullptr;
 			break;
		}
	}

	Count--;
	LeaveCriticalSection(&cs);
}


void CompleteRecvProcess(_ClientInfo* _ptr)
{
	PROTOCOL protocol = GetProtocol(_ptr->recvbuf);

	switch (_ptr->state)
	{
	case STATE::INIT_STATE:
		switch (protocol)
		{
		case PROTOCOL::REQ:
			IntroProcess(_ptr);
			break;
		}
		break;
	case STATE::TITLE_STATE:
		switch (protocol)
		{
		case PROTOCOL::JOIN:
			memset(&_ptr->r_userinfo, 0, sizeof(_User_Info));
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id, _ptr->r_userinfo.pw, _ptr->r_userinfo.nickname);
			JoinProcess(_ptr);
			break;
		case PROTOCOL::LOGIN:
			memset(&_ptr->r_userinfo, 0, sizeof(_User_Info));
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id, _ptr->r_userinfo.pw);
			LoginProcess(_ptr);
			break;
		case PROTOCOL::EXIT:
			ExitProcess(_ptr);
			break;
		}
		break;
	case STATE::LOBBY_STATE:
		switch (protocol)
		{
		case PROTOCOL::ROOM_ENTER:
			EnterRoomProcess(_ptr);
			break;
		case PROTOCOL::USERINFO:
			ShowUserInfoProcess(_ptr);
			break;
		case PROTOCOL::LOGOUT:
			LogoutProcess(_ptr);
			break;
		}
		break;
	case STATE::ROOMLIST_STATE:
		switch (protocol)
		{
		case PROTOCOL::GAME_ROOM:
			UnPackPacket(_ptr->recvbuf, _ptr->room_number);
			GameRoomEnterProcess(_ptr);
			break;
		case PROTOCOL::ROOM_MADE:
			UnPackPacket(_ptr->recvbuf, _ptr->made_chat_name, _ptr->roomKing);
			GameRoomMadeProcess(_ptr);
			break;
		case PROTOCOL::REFRESH:
			RoomListRefreshProcess(_ptr);
			break;
		case PROTOCOL::LOBBY:
			ExitRoomListProcess(_ptr);
			break;
		}
		break;
	case STATE::GAME_ROOM_STATE:
		switch (protocol)
		{
		case PROTOCOL::READY:
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id ,_ptr->r_userinfo.nickname);
			ReadyCompliteProcess(_ptr);
			break;
		case PROTOCOL::CANCEL:        //기능적인 한계로 안쓰임(플레이어가 준비하면 다른 입력을 할 수가 없다.)
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id);
			ReadyCancelProcess(_ptr);
			break;

		case PROTOCOL::GAME_START:
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id , _ptr->r_userinfo.nickname);
			GameStartProcess(_ptr);
			break;

		case PROTOCOL::ROOM_LIST:
			UnPackPacket(_ptr->recvbuf, _ptr->r_userinfo.id);
			ExitGameRoomProcess(_ptr);
			break;
		}
		break;
	case STATE::GAME_STATE:
		switch (protocol)
		{
		case PROTOCOL::GAME_INIT:
			GameInitProcess(_ptr);
			break;

		case PROTOCOL::GAME_PLAY:
			UnPackPacket(_ptr->recvbuf, _ptr->dx, _ptr->dy);
			GamePlayProcess(_ptr);
			break;

		case PROTOCOL::GAME_END:
			GameEndProcess(_ptr);
			break;


			// 먼저 종료된 클라이언트의 처리 밎 같은 방안에 있는 클라이언트들에게 게임 종료 후 결과처리
			// 석고반 움직일때마다 데이터를 처리할 것인가?

		}
		break;

	}

}

void CompleteSendProcess(_ClientInfo* _ptr)
{
	EnterCriticalSection(&cs);
	switch (_ptr->state)
	{
	case INIT_STATE:
		break;

	case GAME_STATE:
	
		break;
	}
	LeaveCriticalSection(&cs);
}

void IntroProcess(_ClientInfo* _ptr)
{	
	EnterCriticalSection(&cs);

	_ptr->state = TITLE_STATE;
	int size = PackPacket(_ptr->sendbuf, PROTOCOL::INTRO, INTRO_MSG);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		LeaveCriticalSection(&cs);
		return;
	}

	LeaveCriticalSection(&cs);
}

void Accepted(SOCKET _sock)
{
	CreateIoCompletionPort((HANDLE)_sock, hcp, _sock, 0);

	_ClientInfo* ptr = AddClientInfo(_sock);

	if (!Recv(ptr))
	{
		ErrorPostQueuedCompletionStatus(_sock);
		return;
	}

}

void ErrorPostQueuedCompletionStatus(SOCKET _sock)
{
	WSAOVERLAPPED_EX* overlapped = new WSAOVERLAPPED_EX;
	memset(overlapped, 0, sizeof(WSAOVERLAPPED_EX));

	overlapped->type = IO_TYPE::IO_DISCONNECT;
	overlapped->ptr = (_ClientInfo*)_sock;

	PostQueuedCompletionStatus(hcp, IO_TYPE::IO_ERROR, _sock, (LPOVERLAPPED)overlapped);
}

void AcceptPostQueuedCompletionStatus(SOCKET _sock)
{
	WSAOVERLAPPED_EX* overlapped = new WSAOVERLAPPED_EX;
	memset(overlapped, 0, sizeof(WSAOVERLAPPED_EX));

	overlapped->type = IO_TYPE::IO_ACCEPT;
	overlapped->ptr = (_ClientInfo*)_sock;

	PostQueuedCompletionStatus(hcp, IO_TYPE::IO_ACCEPT, _sock, (LPOVERLAPPED)overlapped);
}

void JoinProcess(_ClientInfo* _ptr)
{

	RESULT join_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	for (int i = 0; i < Join_Count; i++)
	{
		if (!strcmp(Join_List[i]->id, _ptr->r_userinfo.id))
		{
			join_result = ID_DUP;
			strcpy(msg, ID_DUPLICATE_MSG);
			break;
		}
	}

	if (join_result == NODATA)
	{
		_User_Info* user = new _User_Info;
		memset(user, 0, sizeof(_User_Info));
		strcpy(user->id, _ptr->r_userinfo.id);
		strcpy(user->pw, _ptr->r_userinfo.pw);
		strcpy(user->nickname, _ptr->r_userinfo.nickname);

		Join_List[Join_Count++] = user;

		//delete user;
		join_result = JOIN_SUCCESS;
		strcpy(msg, JOIN_SUCCESS_MSG);
	}

	protocol = JOIN;

	int size = PackPacket(_ptr->sendbuf, protocol, join_result, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}


}

void LoginProcess(_ClientInfo* _ptr)
{
	RESULT login_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;
	int size;

	memset(msg, 0, sizeof(msg));

	if (Join_Count != 0)
	{
		for (int i = 0; i < Join_Count; i++)
		{
			if (!strcmp(Join_List[i]->id, _ptr->r_userinfo.id))
			{
				if (!strcmp(Join_List[i]->pw, _ptr->r_userinfo.pw))
				{

					login_result = LOGIN_SUCCESS;
					strcpy(msg, LOGIN_SUCCESS_MSG);
					_ptr->UserInfo = Join_List[i];
					_ptr->state = LOBBY_STATE;

				}
				else
				{
					login_result = PW_ERROR;
					strcpy(msg, PW_ERROR_MSG);
				}
				break;
			}
		}
	}

	

	if (login_result == NODATA)
	{
		login_result = ID_ERROR;
		strcpy(msg, ID_ERROR_MSG);
	}

	if (login_result != LOGIN_SUCCESS)
	{
		memset(&(_ptr->r_userinfo), 0, sizeof(_User_Info));
	}

	protocol = LOGIN;

	if (Join_Count != 0)
		size = PackPacket(_ptr->sendbuf, protocol, login_result, _ptr->UserInfo->id, _ptr->UserInfo->pw, _ptr->UserInfo->nickname, msg);
	else
		size = PackPacket(_ptr->sendbuf, protocol, login_result, _ptr->r_userinfo.id, _ptr->r_userinfo.pw, _ptr->r_userinfo.nickname, msg);

	

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}
}

void ExitProcess(_ClientInfo* _ptr)
{
	RemoveClientInfo(_ptr);
}

void EnterRoomProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	char temp[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	/*for (int i = 0; i < Room_Count; i++)
	{
		sprintf(msg, "%d.%s \t%d/%d\n", i + 1, RoomList[i]->Chatting_Room_Name, RoomList[i]->User_Count, MAX_ROOM_USER);
	}*/

	for (int i = 0; i < Room_Count; i++)
	{
		itoa(i + 1, temp, 10);  //정수 문자열로 변환
		strcat(msg, temp);     //(i+1) ...
		strcat(msg, ".");       // (i+1).  ...
		strcat(msg, RoomList[i]->Chatting_Room_Name);  //(i+1).방이름
		strcat(msg, "\t");
		sprintf(temp, "%d/%d\n", RoomList[i]->User_Count, MAX_ROOM_USER);
		strcat(msg, temp);
	}

	protocol = ROOM_ENTER;
	_ptr->state = ROOMLIST_STATE;

	int size = PackPacket(_ptr->sendbuf, protocol,Room_Count,msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void ShowUserInfoProcess(_ClientInfo* _ptr)
{
	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	protocol = USERINFO;
	int win = _ptr->win;
	int lose = _ptr->lose;
	float win_rate = ((float)win / (float)(win + lose)) * 100.0f;

	

	sprintf(msg, "닉네임:[%s]\n승리횟수:%d\n패배횟수:%d\n승률:%10.1f%", _ptr->UserInfo->nickname, win, lose, win_rate);

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}
void LogoutProcess(_ClientInfo* _ptr)
{
	//RESULT logout_result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	strcpy(msg, LOGOUT_MSG);

	_ptr->state = TITLE_STATE;
	protocol = LOGOUT;

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}
}

void GameRoomEnterProcess(_ClientInfo* _ptr)
{
	RESULT result = NODATA;

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));



	if (RoomList[_ptr->room_number-1]->full)
	{
		result = ROOM_ENTER_FAIL;
		protocol = GAME_ROOM;
		strcpy(msg, ROOM_ENTER_FAIL_MSG);
	}
	else
	{
		_ptr->state = GAME_ROOM_STATE;
		strcpy(msg, ROOM_ENTER_SUCCESS_MSG);
		result = ROOM_ENTER_SUCCESS;
		//memcpy(&RoomList[_ptr->room_number - 1]->UserList[RoomList[_ptr->room_number - 1]->User_Count], &_ptr, sizeof(_ptr));
		RoomList[_ptr->room_number - 1]->UserList[RoomList[_ptr->room_number - 1]->User_Count] = _ptr;
		RoomList[_ptr->room_number-1]->UserList[RoomList[_ptr->room_number-1]->User_Count]->UserInfo = _ptr->UserInfo;
		RoomList[_ptr->room_number-1]->User_Count++;

		if (RoomList[_ptr->room_number - 1]->User_Count >= MAX_ROOM_USER)
			RoomList[_ptr->room_number - 1]->full = true;
		else
			RoomList[_ptr->room_number - 1]->full = false;

		protocol = GAME_ROOM;
	}


	int size = PackPacket(_ptr->sendbuf, protocol, result,msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void GameRoomMadeProcess(_ClientInfo* _ptr)
{
	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	strcpy(msg, ROOM_MADE_SUCCESS_MSG);

	protocol = ROOM_MADE;

	Room_Info* room = new Room_Info();

	

	//방 이름 적용.
	//strcpy(RoomList[Room_Count]->Chatting_Room_Name, _ptr->made_chat_name);

	strcpy(room->Chatting_Room_Name, _ptr->made_chat_name);


	//방 번호 할당
	//RoomList[Room_Count]->room_number = Room_Count + 1;  //처음이면 1번방.

	room->room_number = Room_Count + 1;
	room->full = false;
	room->User_Count = 0;

	//방을 만든 사람의 이름을 받아서 저장.
	//strcpy(RoomList[Room_Count]->King, _ptr->roomKing);
	strcpy(room->King , _ptr->roomKing);

	RoomList[Room_Count] = room;
	Room_Count++;

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void RoomListRefreshProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	char temp[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	strcpy(msg, REFRESH_MSG);

	for (int i = 0; i < Room_Count; i++)
	{
		itoa(i + 1, temp, 10);  //정수 문자열로 변환
		strcat(msg, temp);     //(i+1) ...
		strcat(msg, ".");       // (i+1).  ...
		strcat(msg, RoomList[i]->Chatting_Room_Name);  //(i+1).방이름
		strcat(msg, "\t");
		sprintf(temp, "%d/%d\n", RoomList[i]->User_Count, MAX_ROOM_USER);
		strcat(msg, temp);
	}


	protocol = REFRESH;

	int size = PackPacket(_ptr->sendbuf, protocol,Room_Count,msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void ExitRoomListProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));
	strcpy(msg, BACK_TO_LOBBY_MSG);

	_ptr->state = LOBBY_STATE;
	protocol = LOBBY;

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void ReadyCompliteProcess(_ClientInfo* _ptr)
{
	RESULT result = NODATA;
	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	for (int i = 0; i < RoomList[_ptr->room_number-1]->User_Count;i++)
	{
		if (strcmp(RoomList[_ptr->room_number-1]->UserList[i]->UserInfo->id , _ptr->r_userinfo.id) ==0)
		{

			if (strcmp(RoomList[_ptr->room_number - 1]->King, _ptr->r_userinfo.nickname) == 0)
			{
				strcpy(msg, READY_ERROR_MSG);
				result = READY_ERROR;
			}
			else
			{
				result = READY_SUCCESS;
				strcpy(msg, READY_COMPLITE_MSG);
				RoomList[_ptr->room_number - 1]->UserList[i]->Ready = true;
			}

			
			break;
		}
	}

	protocol = READY;

	int size = PackPacket(_ptr->sendbuf, protocol, result,msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void ReadyCancelProcess(_ClientInfo* _ptr)
{
	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	for (int i = 0; i < RoomList[_ptr->room_number-1]->User_Count; i++)
	{
		if (strcmp(RoomList[_ptr->room_number-1]->UserList[i]->UserInfo->id , _ptr->r_userinfo.id) ==0 )
		{
			strcpy(msg, READY_CANCEL_MSG);
			RoomList[_ptr->room_number-1]->UserList[i]->Ready = false;
			break;
		}
	}

	protocol = CANCEL;

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}
}

void GameStartProcess(_ClientInfo* _ptr)
{
	EnterCriticalSection(&cs);

	RESULT result = NODATA;
	char msg[BUFSIZE];
	PROTOCOL protocol;
	int ready_count = 0;

	memset(msg, 0, sizeof(msg));

	int index = 0;
	int size = 0;

	protocol = GAME_START;

	if (strcmp(RoomList[_ptr->room_number - 1]->King, _ptr->r_userinfo.nickname) == 0)
	{
		for (int i = 0; i < RoomList[_ptr->room_number - 1]->User_Count; i++)
		{
			if (RoomList[_ptr->room_number - 1]->UserList[i]->Ready)
				ready_count++;
			else
				index = i;
		}


		if (ready_count == RoomList[_ptr->room_number - 1]->User_Count - 1 &&
			strcmp(RoomList[_ptr->room_number - 1]->King, RoomList[_ptr->room_number - 1]->UserList[index]->UserInfo->nickname) == 0)
		{
			result = ALL_READY;
			strcpy(msg, GAME_START_MSG);
			

			for (int i = 0; i < RoomList[_ptr->room_number - 1]->User_Count; i++)
			{

				RoomList[_ptr->room_number - 1]->UserList[i]->state = GAME_STATE;

				size = PackPacket(RoomList[_ptr->room_number - 1]->UserList[i]->sendbuf, protocol, result, msg);
				
				if (!Send(RoomList[_ptr->room_number - 1]->UserList[i], size))
				{
					ErrorPostQueuedCompletionStatus(RoomList[_ptr->room_number - 1]->UserList[i]->sock);
					LeaveCriticalSection(&cs);
					return;
				}

			}
		}
		else
		{
			result = NOT_ALL_READY;
			strcpy(msg, NOT_ALL_READY_MSG);
			size = PackPacket(_ptr->sendbuf, protocol, result, msg);
			
			if (!Send(_ptr, size))
			{
				ErrorPostQueuedCompletionStatus(_ptr->sock);
				LeaveCriticalSection(&cs);
				return;
			}
		}

	}
	else
	{

		strcpy(msg, GAME_START_ERROR_MSG);
		size = PackPacket(_ptr->sendbuf, protocol, result, msg);

		if (!Send(_ptr, size))
		{
			ErrorPostQueuedCompletionStatus(_ptr->sock);
			LeaveCriticalSection(&cs);
			return;
		}
	}
	LeaveCriticalSection(&cs);
}

void ExitGameRoomProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	strcpy(msg, BACK_TO_ROOMLIST_MSG);

	for (int i = 0; i < RoomList[_ptr->room_number-1]->User_Count; i++)
	{
		if (strcmp(RoomList[_ptr->room_number-1]->UserList[i]->UserInfo->id,_ptr->r_userinfo.id) == 0)      //i==0         (0,1,2)
		{

			delete RoomList[_ptr->room_number-1]->UserList[i];

			for (int j = i; j < RoomList[_ptr->room_number-1]->User_Count - 1; j++)
			{
				RoomList[_ptr->room_number-1]->UserList[j] = RoomList[_ptr->room_number-1]->UserList[j + 1];
			}

			//0,2   count=3
			RoomList[_ptr->room_number-1]->UserList[RoomList[_ptr->room_number-1]->User_Count-1] = nullptr;
			break;
		}

	}

	RoomList[_ptr->room_number - 1]->User_Count--;

	_ptr->state = ROOMLIST_STATE;
	protocol = ROOM_LIST;

	int size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		return;
	}

}

void GameInitProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	int size;


	strcpy(msg, GAME_INIT_MSG);

	strcat(msg, "\n");

	for (int i = 0; i < MAPSIZE_Y; i++)
	{
		for (int j = 0; j < MAPSIZE_X; j++)
		{
			
			if (_ptr->MapInfo[i][j] == 0)			//벽
				strcat(msg, "@");
			else if (_ptr->MapInfo[i][j] == 1)	//이동가능구역
				strcat(msg, ".");
			else if (_ptr->MapInfo[i][j] == 2)	//창고
				strcat(msg, "O");
			else if (_ptr->MapInfo[i][j] == 3)	//물건
				strcat(msg, "*");
			else if(_ptr->MapInfo[i][j] == -1)	//플레이어
				strcat(msg, "#");

		}
		strcat(msg, "\n");
	}

	protocol = GAME_INIT;


	if (RoomList[_ptr->room_number - 1]->User_Count > 1)
	{
		for (int i = 0; i < RoomList[_ptr->room_number - 1]->User_Count; i++)
		{
			//RoomList[_ptr->room_number - 1]->UserList[i]->state = GAME_STATE;

			size = PackPacket(RoomList[_ptr->room_number - 1]->UserList[i]->sendbuf, protocol, msg);

			if (!Send(RoomList[_ptr->room_number - 1]->UserList[i], size))
			{
				ErrorPostQueuedCompletionStatus(RoomList[_ptr->room_number - 1]->UserList[i]->sock);
				LeaveCriticalSection(&cs);
				return;
			}

		}
	}
	else
	{

		//_ptr->state = GAME_STATE;

		size = PackPacket(_ptr->sendbuf, protocol, msg);

		if (!Send(_ptr, size))
		{
			ErrorPostQueuedCompletionStatus(_ptr->sock);
			LeaveCriticalSection(&cs);
			return;
		}
	}
}

void GamePlayProcess(_ClientInfo* _ptr)
{
	char msg[BUFSIZE];

	char win[BUFSIZE];
	char lose[BUFSIZE];

	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));
	memset(win, 0, sizeof(win));
	memset(lose, 0, sizeof(lose));

	int size;

	int x, y;
	int dx, dy;

	dx = _ptr->dx;
	dy = _ptr->dy;

	

	//플레이어의 위치를 찾는다.
	for (int i = 0; i < MAPSIZE_Y; i++)
	{
		for (int j = 0; j < MAPSIZE_X; j++)
		{
			if (_ptr->MapInfo[i][j] == -1)
			{
				x = j;
				y = i;
				break;
			}
		}
	}


	//움직임에 대한 판정.
	if (_ptr->MapInfo[y + dy][x + dx] == 0)
	{

	}
	else if (_ptr->MapInfo[y + dy][x + dx] == 2 || _ptr->MapInfo[y + dy][x + dx] == 4)
	{

	}
	else if (_ptr->MapInfo[y + dy][x + dx] == 1)
	{
		_ptr->MapInfo[y][x] = 1;
		_ptr->MapInfo[y + dy][x + dx] = -1;

		x += dx;
		y += dy;
	}
	else if (_ptr->MapInfo[y + dy][x + dx] == 3 && _ptr->MapInfo[y + dy + dy][x + dx + dx] != 0)
	{

		if (_ptr->MapInfo[y + dy + dy][x + dx + dx] == 2)
		{
			_ptr->MapInfo[y][x] = 1;
			_ptr->MapInfo[y + dy][x + dx] = -1;
			_ptr->MapInfo[y + dy + dy][x + dx + dx] = 4;
			_ptr->checkCount++;
		}
		else
		{
			_ptr->MapInfo[y][x] = 1;
			_ptr->MapInfo[y + dy][x + dx] = -1;
			_ptr->MapInfo[y + dy + dy][x + dx + dx] = 3;
		}
		x += dx;
		y += dy;
	}


	if (_ptr->checkCount >= 2)
	{
		//종료 처리.
		protocol = GAME_END;

		//win_msg에 대한 설정.
		for (int i = 0; i < MAPSIZE_Y; i++)
		{
			for (int j = 0; j < MAPSIZE_X; j++)
			{
				if (_ptr->MapInfo[i][j] == 0)			//벽
					strcat(win, "@");
				else if (_ptr->MapInfo[i][j] == 1)	//이동가능구역
					strcat(win, ".");
				else if (_ptr->MapInfo[i][j] == 2)	//창고
					strcat(win, "O");
				else if (_ptr->MapInfo[i][j] == 3)	//물건
					strcat(win, "*");
				else if (_ptr->MapInfo[i][j] == 4)    //물건을 창고에 집어넣었을때
					strcat(win, "!");
				else if (_ptr->MapInfo[i][j] == -1)	//플레이어
					strcat(win, "#");
			}
			strcat(win, "\n");
		}
		strcat(win, "\n");
		strcat(win, WIN_MSG);

		//lose_msg에대한 설정
		strcpy(lose, LOSE_MSG);

		for (int i = 0; i < RoomList[_ptr->room_number - 1]->User_Count; i++)
		{
			if (strcmp(RoomList[_ptr->room_number - 1]->UserList[i]->UserInfo->id, _ptr->UserInfo->id) == 0)
				size = PackPacket(RoomList[_ptr->room_number - 1]->UserList[i]->sendbuf, protocol, win);  //승리
			else
				size = PackPacket(RoomList[_ptr->room_number - 1]->UserList[i]->sendbuf, protocol, lose); //패배


			if (!Send(RoomList[_ptr->room_number - 1]->UserList[i], size))
			{
				ErrorPostQueuedCompletionStatus(RoomList[_ptr->room_number - 1]->UserList[i]->sock);
				LeaveCriticalSection(&cs);
				return;
			}

		}

		return;

	}
	else
	{
		protocol = GAME_PLAY;

		strcpy(msg, GAME_INIT_MSG);
		strcat(msg, "\n");

		//msg에 움직임 처리에 대한 맵 내용 넣어주기.
		for (int i = 0; i < MAPSIZE_Y; i++)
		{
			for (int j = 0; j < MAPSIZE_X; j++)
			{
				if (_ptr->MapInfo[i][j] == 0)			//벽
					strcat(msg, "@");
				else if (_ptr->MapInfo[i][j] == 1)	//이동가능구역
					strcat(msg, ".");
				else if (_ptr->MapInfo[i][j] == 2)	//창고
					strcat(msg, "O");
				else if (_ptr->MapInfo[i][j] == 3)	//물건
					strcat(msg, "*");
				else if (_ptr->MapInfo[i][j] == 4)    //물건을 창고에 집어넣었을때
					strcat(msg, "!");
				else if (_ptr->MapInfo[i][j] == -1)	//플레이어
					strcat(msg, "#");

			}
			strcat(msg, "\n");
		}
	}

	size = PackPacket(_ptr->sendbuf, protocol, msg);

	if (!Send(_ptr, size))
	{
		ErrorPostQueuedCompletionStatus(_ptr->sock);
		LeaveCriticalSection(&cs);
		return;
	}
}

void GameEndProcess(_ClientInfo* _ptr)
{

	char msg[BUFSIZE];
	PROTOCOL protocol;

	memset(msg, 0, sizeof(msg));

	int size;


	strcpy(msg, GAME_END_MSG);


	protocol = GAME_END;


	if (RoomList[_ptr->room_number - 1]->User_Count > 1)
	{
		for (int i = 0; i < RoomList[_ptr->room_number - 1]->User_Count; i++)
		{
			RoomList[_ptr->room_number - 1]->UserList[i]->state = GAME_STATE;

			size = PackPacket(RoomList[_ptr->room_number - 1]->UserList[i]->sendbuf, protocol, msg);

			if (!Send(RoomList[_ptr->room_number - 1]->UserList[i], size))
			{
				ErrorPostQueuedCompletionStatus(RoomList[_ptr->room_number - 1]->UserList[i]->sock);
				LeaveCriticalSection(&cs);
				return;
			}

		}
	}
	else
	{

		_ptr->state = TITLE_STATE;

		size = PackPacket(_ptr->sendbuf, protocol, msg);

		if (!Send(_ptr, size))
		{
			ErrorPostQueuedCompletionStatus(_ptr->sock);
			LeaveCriticalSection(&cs);
			return;
		}
	}

}




