#pragma once
#include <string>
#include <memory>
#include <AppCore/AppCore.h>
#include <vector>
#include <JavaScriptCore/JSRetainPtr.h>
#include <stdexcept>
#include <format>

using namespace ultralight;
namespace UI
{
	template <typename T>
	JSValueRef convert(JSContextRef ctx, T value);

	class JavascriptInterop
	{
	private:
		RefPtr<View> _view;
	protected:
		JSValueRef execute(JSContextRef ctx, std::string functionName, std::vector<JSValueRef> args);
	public:
		JavascriptInterop(ultralight::View *view) : _view(view) {}
		void notification(const std::string& message, const std::string& type);
	};
}
