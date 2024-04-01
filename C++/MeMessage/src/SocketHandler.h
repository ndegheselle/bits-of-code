#pragma once

#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

#include <ws2tcpip.h>
#include <string>
#include <stdexcept>
#include <thread>
#include <functional>

class SocketHandler {
private:
    SOCKET _sock;
    SOCKADDR_IN _server_addr;
    SOCKADDR_IN _client_addr;
    int _addr_len;

    std::atomic<bool> _stop_receiving_flag;
    std::thread _receiving_thread;
    std::function<void(std::string)> _message_callback;

public:
    // Constructor
    SocketHandler();

    // Destructor
    ~SocketHandler();
    
    // Connect as a client
    void server_connect(std::string ip, int port);

    // Bind and listen as a server
    void server_listen(int port);

    // Disconnect
    void disconnect();

    // Send message
    void send_message(std::string username, std::string message);
    
    void set_message_callback(std::function<void(std::string)> callback);
    
    void check_receiving();
    void start_receiving();
};