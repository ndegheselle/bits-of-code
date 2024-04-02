#pragma once

#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

#include <ws2tcpip.h>
#include <string>
#include <exception>
#include <thread>
#include <functional>

using SocketReceiveEvent = std::function<void(const std::string &)>;
namespace logic
{
    struct Message
    {
    public:
        std::string username;
        std::string content;
    };

    class SocketCommunication
    {
    protected:
        SOCKET _sock;
        SocketReceiveEvent _receive_event;

        bool _stop_receiving_flag = false;
        std::thread _receiving_thread;

        void receive(const std::string &message)
        {
            if (_receive_event)
                _receive_event(message);
        }

    public:
        ~SocketCommunication()
        {
            disconnect();
        }

        virtual void send_message(const std::string& message) = 0;

        void on_recieving(const SocketReceiveEvent& event)
        {
            _receive_event = event;
        }

        void disconnect() {
            _stop_receiving_flag = true;

            if (_receiving_thread.joinable())
                _receiving_thread.join();
            closesocket(_sock);
            WSACleanup();
		}
    };

    class SocketServer : public SocketCommunication
    {
    private:
        // Could be a dictionnary of clients
        SOCKET _client_sock = INVALID_SOCKET;

    public:
        SocketServer(int port);
        void send_message(const std::string &message);
    };

    class SocketClient : public SocketCommunication
    {
    public:
        SocketClient(std::string ip, int port);
        void send_message(const std::string &message);
    };
}