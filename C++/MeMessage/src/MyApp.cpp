#include "MyApp.h"

#define WINDOW_WIDTH 600
#define WINDOW_HEIGHT 400

MyApp::MyApp()
{
    app_ = App::Create();
    window_ = Window::Create(app_->main_monitor(), WINDOW_WIDTH, WINDOW_HEIGHT,
                             false, kWindowFlags_Titled | kWindowFlags_Resizable);
    overlay_ = Overlay::Create(window_, 1, 1, 0, 0);

    OnResize(window_.get(), window_->width(), window_->height());
    overlay_->view()->LoadURL("file:///index.html");

    app_->set_listener(this);
    window_->set_listener(this);
    overlay_->view()->set_load_listener(this);
    overlay_->view()->set_view_listener(this);

    uiHandler_ = new UI::UIHandler(overlay_->view().get());
}

MyApp::~MyApp()
{
    delete uiHandler_;
}

void MyApp::Run()
{
    app_->Run();
}

void MyApp::OnUpdate()
{
}

void MyApp::OnClose(ultralight::Window *window)
{
    app_->Quit();
}

void MyApp::OnResize(ultralight::Window *window, uint32_t width, uint32_t height)
{
    overlay_->Resize(width, height);
}

void MyApp::OnFinishLoading(ultralight::View *caller,
                            uint64_t frame_id,
                            bool is_main_frame,
                            const String &url)
{
}

void MyApp::OnDOMReady(ultralight::View *caller,
                       uint64_t frame_id,
                       bool is_main_frame,
                       const String &url)
{
    uiHandler_->RegisterCallbacks();
}

void MyApp::OnChangeCursor(ultralight::View *caller,
                           Cursor cursor)
{
    window_->SetCursor(cursor);
}

void MyApp::OnChangeTitle(ultralight::View *caller,
                          const String &title)
{
    window_->SetTitle(title.utf8().data());
}
