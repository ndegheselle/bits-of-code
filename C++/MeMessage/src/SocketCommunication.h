#pragma once

#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

#include <ws2tcpip.h>
#include <string>
#include <stdexcept>
#include <thread>
#include <functional>

using SocketReceiveEvent = std::function<void(const std::string &)>;

class ISocketCommunication
{
protected:
    SOCKET _sock;
    SocketReceiveEvent _receive_event;

    void receive(const std::string &message)
    {
        if (_receive_event)
            _receive_event(message);
    }

public:
    void send_message(const std::string &message)
    {
        if (send(_sock, message.c_str(), message.size(), 0) == SOCKET_ERROR)
            throw std::runtime_error("send failed");
    }
    
    void on_recieving(const SocketReceiveEvent &event)
    {
        _receive_event = event;
    }
};

class SocketServer : public ISocketCommunication
{
private:
    SOCKET _sock;
    sockaddr_in _server_addr;
    sockaddr_in _client_addr;

    bool _stop_receiving_flag = false;
    std::thread _receiving_thread;

public:
    SocketServer(int port);
    ~SocketServer();
};

class SocketClient : public ISocketCommunication
{
private:
    SOCKET _sock;
    sockaddr_in _server_addr;

    bool _stop_receiving_flag = false;
    std::thread _receiving_thread;

public:
    SocketClient(std::string ip, int port);
    ~SocketClient();
};