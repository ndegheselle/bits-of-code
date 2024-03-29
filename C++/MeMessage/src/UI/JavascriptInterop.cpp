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

	JSValueRef JavascriptInterop::execute(JSContextRef ctx, std::string functionName, std::vector<JSValueRef> args)
	{
		JSRetainPtr<JSStringRef> strFunction = adopt(JSStringCreateWithUTF8CString(functionName.c_str()));

		// Evaluate the string "ShowMessage"
		JSValueRef func = JSEvaluateScript(ctx, strFunction.get(), 0, 0, 0, 0);

		if (!JSValueIsObject(ctx, func))
			throw std::invalid_argument("JS function not found");

		JSObjectRef funcObj = JSValueToObject(ctx, func, 0);
		if (!funcObj || !JSObjectIsFunction(ctx, funcObj))
			throw std::invalid_argument("Function is not valid.");

		size_t num_args = sizeof(args) / sizeof(JSValueRef*);
		JSValueRef exception = 0;
		JSValueRef result = JSObjectCallAsFunction(ctx, funcObj, 0, num_args, args.data(), &exception);

		// Handle any exceptions that were thrown.
		if (exception)
			throw std::runtime_error("Function threw an exception");

		return result;
	}

	void JavascriptInterop::notification(const std::string& message, const std::string& type)
	{
		JSContextRef ctx = (*_view->LockJSContext());
		execute(ctx, "Notification", {convert<std::string>(ctx, message), convert<std::string>(ctx, type)});
	}
}