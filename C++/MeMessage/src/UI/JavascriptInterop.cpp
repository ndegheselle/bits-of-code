#include "JavascriptInterop.h"

namespace UI
{
	template <>
	JSValueRef Convert(JSContextRef ctx, const std::string value)
	{
		JSRetainPtr<JSStringRef> str = adopt(JSStringCreateWithUTF8CString(value.c_str()));
		return JSValueMakeString(ctx, str.get());
	}
	template <>
	JSValueRef Convert(JSContextRef ctx, double value)
	{
		return JSValueMakeNumber(ctx, value);
	}
	template <>
	JSValueRef Convert(JSContextRef ctx, bool value)
	{
		return JSValueMakeBoolean(ctx, value);
	}

	JSValueRef JavascriptInterop::Execute(const JSContextRef &ctx, const std::string &functionName, const std::vector<JSValueRef> &args)
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

	void UIHandler::RegisterCallbacks()
	{
		RefPtr<JSContext> context = _view->LockJSContext();
		SetJSContext(context->ctx());
		JSObject global = JSGlobalObject();

		global["Test"] = BindJSCallbackWithRetval(&UIHandler::Test);
	}

	void UIHandler::Notification(const std::string &message, const std::string &type)
	{
		JSContextRef ctx = (*_view->LockJSContext());
		Execute(ctx, "notification", {Convert<std::string>(ctx, message), Convert<std::string>(ctx, type)});
	}

	JSValue UIHandler::Test(const JSObject &thisObject, const JSArgs &args)
	{
		Notification("Hello from C++!", "is-info");
		return JSValue("Hello from C++!<br/>Ultralight rocks!");
	}
}