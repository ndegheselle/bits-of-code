#include "SocketCommunication.h"

namespace logic
{
    SocketServer::SocketServer(int port)
    {
        WSADATA wsaData;
        if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
            throw std::exception("WSAStartup failed");

        _sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (_sock == INVALID_SOCKET)
            throw std::exception("socket failed");

        SOCKADDR_IN server_addr;
        server_addr.sin_family = AF_INET;
        server_addr.sin_addr.s_addr = INADDR_ANY;
        server_addr.sin_port = htons(port);

        if (bind(_sock, (sockaddr *)&server_addr, sizeof(server_addr)) == SOCKET_ERROR)
            throw std::exception("bind failed");

        if (listen(_sock, 1) == SOCKET_ERROR)
            throw std::exception("listen failed");

        _receiving_thread = std::thread([this]()
                                        {
        SOCKADDR_IN client_addr;
        while (!_stop_receiving_flag)
        {
            int _client_addr_size = sizeof(client_addr);

            if (_client_sock == INVALID_SOCKET)
				_client_sock = accept(_sock, (sockaddr*)&client_addr, &_client_addr_size);
            
            if (_client_sock == INVALID_SOCKET)
                throw std::exception("accept failed");

            char buffer[1024];
            int bytes_received = recv(_client_sock, buffer, 1024, 0);
            if (bytes_received == SOCKET_ERROR)
                throw std::exception("recv failed");

            buffer[bytes_received] = '\0';
            receive(buffer);
        }
        closesocket(_client_sock); });
    }

    void SocketServer::send_message(const std::string &message)
    {
        if (send(_client_sock, message.c_str(), message.size(), 0) == SOCKET_ERROR)
            throw std::exception("send failed");
    }

    SocketClient::SocketClient(std::string ip, int port)
    {
        WSADATA wsaData;
        if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
            throw std::exception("WSAStartup failed");

        _sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (_sock == INVALID_SOCKET)
            throw std::exception("socket failed");

        SOCKADDR_IN server_addr;
        server_addr.sin_family = AF_INET;
        server_addr.sin_addr.s_addr = inet_addr(ip.c_str());
        server_addr.sin_port = htons(port);

        if (connect(_sock, (sockaddr *)&server_addr, sizeof(server_addr)) == SOCKET_ERROR)
            throw std::exception("connect failed");

        _receiving_thread = std::thread([this]()
                                        {
        while (!_stop_receiving_flag)
        {
			char buffer[1024];
			int bytes_received = recv(_sock, buffer, 1024, 0);
			if (bytes_received == SOCKET_ERROR)
				throw std::exception("recv failed");

			buffer[bytes_received] = '\0';
			receive(buffer);
		} });
    }

    void SocketClient::send_message(const std::string &message)
    {
        if (send(_sock, message.c_str(), message.size(), 0) == SOCKET_ERROR)
            throw std::exception("send failed");
    }
}