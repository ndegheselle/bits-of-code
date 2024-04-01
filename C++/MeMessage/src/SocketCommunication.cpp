#include "SocketCommunication.h"

SocketServer::SocketServer(int port)
{
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
        throw std::runtime_error("WSAStartup failed");

    _sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (_sock == INVALID_SOCKET)
        throw std::runtime_error("socket failed");

    _server_addr.sin_family = AF_INET;
    _server_addr.sin_addr.s_addr = INADDR_ANY;
    _server_addr.sin_port = htons(port);

    if (bind(_sock, (sockaddr*)&_server_addr, sizeof(_server_addr)) == SOCKET_ERROR)
        throw std::runtime_error("bind failed");

    if (listen(_sock, 1) == SOCKET_ERROR)
        throw std::runtime_error("listen failed");

    _receiving_thread = std::thread([this]() {
        while (!_stop_receiving_flag)
        {
            SOCKET client = accept(_sock, (sockaddr*)&_client_addr, NULL);
            if (client == INVALID_SOCKET)
                throw std::runtime_error("accept failed");

            char buffer[1024];
            int bytes_received = recv(client, buffer, 1024, 0);
            if (bytes_received == SOCKET_ERROR)
                throw std::runtime_error("recv failed");

            buffer[bytes_received] = '\0';
            receive(buffer);
        }
    });
}

SocketServer::~SocketServer()
{
    _stop_receiving_flag = true;
    _receiving_thread.join();
    closesocket(_sock);
    WSACleanup();
}

void SocketServer::send_message(const std::string& message)
{
    if (send(_sock, message.c_str(), message.size(), 0) == SOCKET_ERROR)
        throw std::runtime_error("send failed");
}

SocketClient::SocketClient(std::string ip, int port)
{
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
        throw std::runtime_error("WSAStartup failed");

    _sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (_sock == INVALID_SOCKET)
        throw std::runtime_error("socket failed");

    _server_addr.sin_family = AF_INET;
    _server_addr.sin_addr.s_addr = inet_addr(ip.c_str());
    _server_addr.sin_port = htons(port);

    if (connect(_sock, (sockaddr*)&_server_addr, sizeof(_server_addr)) == SOCKET_ERROR)
        throw std::runtime_error("connect failed");

    _receiving_thread = std::thread([this]() {
        while (!_stop_receiving_flag)
        {
            char buffer[1024];
            int bytes_received = recv(_sock, buffer, 1024, 0);
            if (bytes_received == SOCKET_ERROR)
                throw std::runtime_error("recv failed");

            buffer[bytes_received] = '\0';
            receive(buffer);
        }
    });
}

void SocketClient::send_message(const std::string& message)
{
    if (send(_sock, message.c_str(), message.size(), 0) == SOCKET_ERROR)
        throw std::runtime_error("send failed");
}