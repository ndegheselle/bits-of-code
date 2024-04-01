#include "JavascriptInterop.h"

namespace UI
{
	template <>
	JSValueRef convert(JSContextRef ctx, const std::string value)
	{
		JSRetainPtr<JSStringRef> str = adopt(JSStringCreateWithUTF8CString(value.c_str()));
		return JSValueMakeString(ctx, str.get());
	}
	template <>
	JSValueRef convert(JSContextRef ctx, double value)
	{
		return JSValueMakeNumber(ctx, value);
	}
	template <>
	JSValueRef convert(JSContextRef ctx, bool value)
	{
		return JSValueMakeBoolean(ctx, value);
	}

	JSValueRef JavascriptInterop::execute(const JSContextRef &ctx, const std::string &functionName, const std::vector<JSValueRef> &args)
	{
		JSObject global = JSGlobalObject();
		JSValueRef exception = nullptr;

		JSValueRef function = global[functionName.c_str()];
		if (!JSValueIsObject(ctx, function))
		{
			throw std::runtime_error("Function not found");
		}

		JSObject functionObj = JSValueToObject(ctx, function, &exception);
		if (exception)
		{
			throw std::runtime_error("Function not found");
		}

		JSValueRef result = JSObjectCallAsFunction(ctx, functionObj, nullptr, args.size(), args.data(), &exception);

		// Handle any exceptions that were thrown.
		if (exception)
		{
			// Get exception description
			JSStringRef exceptionArg = JSValueToStringCopy(ctx, exception, 0);
			size_t length = JSStringGetMaximumUTF8CStringSize(exceptionArg);
			std::vector<char> buffer(length, 0);
			JSStringGetUTF8CString(exceptionArg, buffer.data(), length);
			JSStringRelease(exceptionArg);
			throw std::runtime_error(buffer.data());
		}

		return result;
	}

	void UIHandler::register_callbacks()
	{
		RefPtr<JSContext> context = _view->LockJSContext();
		SetJSContext(context->ctx());
		JSObject global = JSGlobalObject();

		global["connect"] = BindJSCallbackWithRetval(&UIHandler::connect);
		global["host"] = BindJSCallbackWithRetval(&UIHandler::host);
		global["sendMessage"] = BindJSCallbackWithRetval(&UIHandler::send_message);
	}

	void UIHandler::notification(const std::string &message, const std::string &type)
	{
		JSContextRef ctx = (*_view->LockJSContext());
		execute(ctx, "notification", {convert<std::string>(ctx, message), convert<std::string>(ctx, type)});
	}

	void UIHandler::connected(const std::string &error) {
		JSContextRef ctx = (*_view->LockJSContext());
		execute(ctx, "connected", {convert<std::string>(ctx, error)});
	}

	void UIHandler::received_message(const std::string &username, const std::string &message) {
		JSContextRef ctx = (*_view->LockJSContext());
		execute(ctx, "receivedMessage", {convert<std::string>(ctx, username), convert<std::string>(ctx, message)});
	}

	JSValue UIHandler::connect(const JSObject& thisObject, const JSArgs& args) {
		ultralight::String address = args[0].ToString();
		int port = args[1].ToInteger();

		_socketHandler->server_connect(address.utf8().data(), port);

		return JSValueMakeUndefined(thisObject.context());
	}
	JSValue UIHandler::host(const JSObject& thisObject, const JSArgs& args) {
		int port = args[0].ToInteger();

		_socketHandler->server_listen(port);
		_socketHandler->set_message_callback([this](std::string message) {
			// split message into username and message
			std::string username = message.substr(0, message.find(":"));
			std::string content = message.substr(message.find(":") + 2);
			received_message(username, content);
		});
		_socketHandler->check_receiving();

		return JSValueMakeUndefined(thisObject.context());
	}
	JSValue UIHandler::send_message(const JSObject& thisObject, const JSArgs& args) {
		return JSValueMakeUndefined(thisObject.context());
	}
}