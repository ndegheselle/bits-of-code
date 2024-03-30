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
		JSRetainPtr<JSStringRef> strFunction = adopt(JSStringCreateWithUTF8CString(functionName.c_str()));

		// Evaluate the string "ShowMessage"
		JSValueRef func = JSEvaluateScript(ctx, strFunction.get(), 0, 0, 0, 0);

		if (!JSValueIsObject(ctx, func))
			throw std::invalid_argument("JS function not found");

		JSObjectRef funcObj = JSValueToObject(ctx, func, 0);
		if (!funcObj || !JSObjectIsFunction(ctx, funcObj))
			throw std::invalid_argument("Function is not valid.");

		size_t num_args = sizeof(args) / sizeof(JSValueRef *);
		JSValueRef exception = 0;
		JSValueRef result = JSObjectCallAsFunction(ctx, funcObj, 0, num_args, args.data(), &exception);

		// Handle any exceptions that were thrown.
		if (exception)
			throw std::runtime_error("Function threw an exception");

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
		Execute(ctx, "Notification", {Convert<std::string>(ctx, message), Convert<std::string>(ctx, type)});
	}

	JSValue UIHandler::Test(const JSObject &thisObject, const JSArgs &args)
	{
		std::cout << "JS called !\n";
		return JSValue("Hello from C++!<br/>Ultralight rocks!");
	}
}