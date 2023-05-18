#include "Global.h"

DWORD WINAPI WorkerThread(LPVOID arg)
{
	int retval;
	HANDLE hcp = (HANDLE)arg;

	while (1)
	{
		// 비동기 입출력 완료 기다리기
		DWORD cbTransferred;
		SOCKET client_sock;
		WSAOVERLAPPED_EX* overlapped;

		retval = GetQueuedCompletionStatus(hcp, &cbTransferred,
			(LPDWORD)&client_sock, (LPOVERLAPPED*)&overlapped, INFINITE);

		_ClientInfo* ptr = overlapped->ptr;
		IO_TYPE io_type = overlapped->type;
		WSAOVERLAPPED* overlap = &overlapped->overlapped;	

		
		// 비동기 입출력 결과 확인
		if (retval == 0 || cbTransferred == 0)
		{
			if (retval == 0)
			{
				DWORD temp1, temp2;
				WSAGetOverlappedResult(client_sock, overlap,
					&temp1, FALSE, &temp2);
				err_display("WSAGetOverlappedResult()");
			}

			io_type = IO_TYPE::IO_DISCONNECT;
		}

		int result;

		switch (io_type)
		{
		case IO_TYPE::IO_ACCEPT:
			Accepted(client_sock);		
			delete overlapped;
			break;
		case IO_TYPE::IO_RECV:
			result = CompleteRecv(ptr, cbTransferred);
			switch (result)
			{
			case SOC_ERROR:
				continue;
			case SOC_FALSE:
				continue;
			case SOC_TRUE:
				break;
			}
			CompleteRecvProcess(ptr);

			if (!Recv(ptr))
			{
				ErrorPostQueuedCompletionStatus(ptr->sock);				
			}

			break;
		case IO_TYPE::IO_SEND:
			result = CompleteSend(ptr, cbTransferred);
			switch (result)
			{
			case SOC_ERROR:
				continue;
			case SOC_FALSE:
				continue;
			case SOC_TRUE:
				break;
			}
			CompleteSendProcess(ptr);
			break;
		case IO_TYPE::IO_ERROR:
			delete overlapped;
		case IO_TYPE::IO_DISCONNECT:
			RemoveClientInfo(ptr);
			break;
		}

	}
	return 0;
}
