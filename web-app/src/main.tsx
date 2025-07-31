import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router";
import "./styles/index.css";
import "./styles/normalize.css";
import "./styles/form.css";
import { TopicProvider } from "./stores/TopicProvider.tsx";
import App from "./App.tsx";
import LoginPage from "./pages/LoginPage/LoginPage.tsx";
import ProjectPage from "./pages/ProjectPage/ProjectPage.tsx";

createRoot(document.getElementById("root")!).render(
    <StrictMode>
        <BrowserRouter>

            <TopicProvider>
                <Routes>
                    <Route index element={<LoginPage />} />
                    <Route path="login" element={<LoginPage />} />
                    <Route path="project" element={<ProjectPage />} />
                    <Route path="overview" element={<App />} />
                </Routes>
            </TopicProvider>

        </BrowserRouter>
    </StrictMode>,
);
