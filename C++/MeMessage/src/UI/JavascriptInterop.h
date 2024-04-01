#pragma once
#include <string>
#include <memory>
#include <AppCore/AppCore.h>
#include <vector>
#include <JavaScriptCore/JSRetainPtr.h>
#include <stdexcept>
#include <format>
#include <iostream>
#include <JavaScriptCore/JavaScript.h>

#include "../SocketCommunication.h"

using namespace ultralight;
namespace UI
{
	template <typename T>
	JSValueRef convert(JSContextRef ctx, T value);

	class JavascriptInterop
	{
	protected:
		RefPtr<View> _view;
		JSValueRef execute(const JSContextRef &ctx, const std::string &functionName, const std::vector<JSValueRef> &args);

	public:
		JavascriptInterop(ultralight::View *view) : _view(view) {}
		virtual void register_callbacks() = 0;
	};

	class UIHandler : public JavascriptInterop
	{
	private:
		ISocketCommunication* _socket = nullptr;

	public:
		UIHandler(ultralight::View *view) : JavascriptInterop(view) {}
		void register_callbacks() override;
#pragma region C++ -> JS Callbacks
		void notification(const std::string &message, const std::string &type);
		void connected(const std::string &error);
		void received_message(const std::string &username, const std::string &message);
#pragma endregion
#pragma region JS -> C++ Callbacks
		JSValue connect(const JSObject& thisObject, const JSArgs& args);
		JSValue host(const JSObject& thisObject, const JSArgs& args);
		JSValue send_message(const JSObject& thisObject, const JSArgs& args);
#pragma endregion
	};
}
