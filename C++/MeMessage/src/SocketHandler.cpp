#include "SocketHandler.h"

// Constructor
SocketHandler::SocketHandler() {
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 0), &wsaData);
    _sock = socket(AF_INET, SOCK_STREAM, 0);
    if (_sock == INVALID_SOCKET) {
        throw std::runtime_error("Socket creation failed");
    }
}

// Destructor
SocketHandler::~SocketHandler() {
    closesocket(_sock);
    WSACleanup();
    
    _stop_receiving_flag = true;
    if (_receiving_thread.joinable()) {
		_receiving_thread.join();
    }
}

// Connect as a client
void SocketHandler::server_connect(std::string ip, int port) {
    _server_addr.sin_family = AF_INET;
    _server_addr.sin_port = htons(port);
    inet_pton(AF_INET, ip.c_str(), &_server_addr.sin_addr);

    if (connect(_sock, (sockaddr*)&_server_addr, sizeof(_server_addr)) == SOCKET_ERROR) {
        throw std::runtime_error("Connection failed");
    }
}

// Bind and listen as a server
void SocketHandler::server_listen(int port) {
    _server_addr.sin_family = AF_INET;
    _server_addr.sin_addr.s_addr = INADDR_ANY;
    _server_addr.sin_port = htons(port);

    if (bind(_sock, (sockaddr*)&_server_addr, sizeof(_server_addr)) == SOCKET_ERROR) {
        throw std::runtime_error("Bind failed");
    }

    if (listen(_sock, SOMAXCONN) == SOCKET_ERROR) {
        throw std::runtime_error("Listen failed");
    }

    SOCKET new_sock = accept(_sock, (sockaddr*)&_client_addr, &_addr_len);
    if (new_sock == INVALID_SOCKET) {
        throw std::runtime_error("Accept failed");
    }

    _sock = new_sock;
}

// Disconnect
void SocketHandler::disconnect() {
    closesocket(_sock);
}

// Send message
void SocketHandler::send_message(std::string username, std::string message) {
    std::string full_message = username + ": " + message;
    send(_sock, full_message.c_str(), full_message.size(), 0);
}

void SocketHandler::set_message_callback(std::function<void(std::string)> callback) {
    _message_callback = callback;
}

void SocketHandler::check_receiving() {
    char buffer[4096];
    while (!_stop_receiving_flag) {
        int bytes_received = recv(_sock, buffer, 4096, 0);
        if (bytes_received > 0) {
            std::string message(buffer, 0, bytes_received);
            _message_callback(message);
        }
    }
}

void SocketHandler::start_receiving() {
    _stop_receiving_flag = false;
    _receiving_thread = std::thread(&SocketHandler::check_receiving, this);
}