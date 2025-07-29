import { Route, Routes } from "react-router";
import AskPage from "../pages/ask";
import Settings from "../pages/settings";

export function AppRouter(){
    return (
        <Routes>
            <Route path="/" element={<AskPage/>} />
            <Route path="/settings" element={<Settings/>} />
        </Routes>
    )
}