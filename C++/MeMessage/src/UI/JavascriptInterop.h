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

using namespace ultralight;
namespace UI
{
	template <typename T>
	JSValueRef Convert(JSContextRef ctx, T value);

	class JavascriptInterop
	{
	protected:
		RefPtr<View> _view;
		JSValueRef Execute(const JSContextRef &ctx, const std::string &functionName, const std::vector<JSValueRef> &args);

	public:
		JavascriptInterop(ultralight::View *view) : _view(view) {}
		virtual void RegisterCallbacks() = 0;
	};

	class UIHandler : public JavascriptInterop
	{
	public:
		UIHandler(ultralight::View *view) : JavascriptInterop(view) {}
		void RegisterCallbacks() override;

#pragma region C++ -> JS Callbacks
		void Notification(const std::string &message, const std::string &type);
#pragma endregion
#pragma region JS -> C++ Callbacks
		JSValue Test(const JSObject& thisObject, const JSArgs& args);
#pragma endregion
	};


}
